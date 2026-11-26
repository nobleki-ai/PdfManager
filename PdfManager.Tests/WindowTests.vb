Imports System.IO
Imports System.Threading
Imports System.Windows.Forms
Imports Xunit

''' <summary>Drives the real window the way a person would, without taking focus.</summary>
Public Class WindowTests
    Implements IDisposable

    Private ReadOnly _f As New Fixtures()

    Public Sub Dispose() Implements IDisposable.Dispose
        _f.Dispose()
    End Sub

    Friend Class FakePrompts
        Implements IUserPrompts
        Public Passwords As New Queue(Of String)
        Public ReplaceAnswer As Boolean = True
        Public ReplaceAsked As Integer

        Public Function AskPassword(fileName As String, wasWrong As Boolean) As String Implements IUserPrompts.AskPassword
            Return If(Passwords.Count > 0, Passwords.Dequeue(), Nothing)
        End Function

        Public Function ConfirmReplace(fileNames As IReadOnlyList(Of String)) As Boolean Implements IUserPrompts.ConfirmReplace
            ReplaceAsked += 1
            Return ReplaceAnswer
        End Function
    End Class

    ''' <summary>Opens the window off-screen on its own thread, runs the steps, then closes it.</summary>
    Friend Shared Sub WithWindow(prompts As IUserPrompts, steps As Func(Of MainForm, Task))
        Dim failure As Exception = Nothing
        Dim thread As New Thread(
            Sub()
                Using form As New MainForm()
                    form.QuietMode = True
                    form.PersistSettings = False
                    form.Prompts = prompts
                    form.ShowInTaskbar = False
                    form.StartPosition = FormStartPosition.Manual
                    form.Location = New Drawing.Point(-20000, -20000)
                    AddHandler form.Shown,
                        Async Sub()
                            Try
                                Await form.StartupTask
                                Await steps(form)
                            Catch ex As Exception
                                failure = ex
                            Finally
                                form.Close()
                            End Try
                        End Sub
                    Application.Run(form)
                End Using
            End Sub)
        thread.SetApartmentState(ApartmentState.STA)
        thread.Start()
        Assert.True(thread.Join(TimeSpan.FromMinutes(2)), "The window didn't finish in time.")
        If failure IsNot Nothing Then Throw New Exception("Window test failed: " & failure.Message, failure)
    End Sub

    <Fact>
    Public Sub Merge_tab_uses_only_ticked_pdfs_and_their_pages()
        Dim a = _f.MakePdf("first.pdf", "A", 12)
        Dim b = _f.MakePdf("second.pdf", "B", 6)
        Dim c = _f.MakePdf("third.pdf", "C", 3)
        Dim outFolder = Path.Combine(_f.Folder, "results")

        WithWindow(New FakePrompts(),
            Async Function(form)
                Await form.AddPathsAsync({_f.Folder})
                Assert.Equal(3, form.grid.Rows.Count)

                form.tabOperations.SelectedTab = form.tabMerge
                Assert.True(form.colUse.Visible)
                Assert.True(form.colPagesToUse.Visible)
                Assert.Equal("Merge Pages", form.btnRun.Text)
                Assert.Equal(_f.Folder, form.txtOutputFolder.Text)

                Dim rows = form.grid.Rows
                rows(0).Cells(form.colPagesToUse.Index).Value = "1-10"
                rows(1).Cells(form.colPagesToUse.Index).Value = "5"
                rows(2).Cells(form.colUse.Index).Value = False
                form.txtOutputFolder.Text = outFolder
                form.txtOutputName.Text = "my merge.pdf"

                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)
            End Function)

        Dim expected = Enumerable.Range(1, 10).Select(Function(p) "A" & p).ToList()
        expected.Add("B5")
        Assert.Equal(expected, _f.PageLabels(Path.Combine(outFolder, "my merge.pdf")))
    End Sub

    <Fact>
    Public Sub Combine_tab_follows_the_list_order_after_moving_a_row()
        _f.MakePdf("a.pdf", "A", 1)
        _f.MakePdf("b.pdf", "B", 1)
        Dim outFolder = Path.Combine(_f.Folder, "results")

        WithWindow(New FakePrompts(),
            Async Function(form)
                Await form.AddPathsAsync({Path.Combine(_f.Folder, "a.pdf"), Path.Combine(_f.Folder, "b.pdf")})
                Assert.False(form.colUse.Visible)
                form.grid.ClearSelection()
                form.grid.Rows(1).Selected = True
                form.btnMoveUp.PerformClick()
                form.txtOutputFolder.Text = outFolder
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)
            End Function)

        Assert.Equal({"B1", "A1"}, _f.PageLabels(Path.Combine(outFolder, "combined.pdf")))
    End Sub

    <Fact>
    Public Sub A_locked_pdf_asks_for_its_password_and_then_works()
        Dim pdf = _f.MakePdf("locked.pdf", "L", 2, password:="right")
        Dim prompts As New FakePrompts()
        prompts.Passwords.Enqueue("wrong")
        prompts.Passwords.Enqueue("right")
        Dim outFolder = Path.Combine(_f.Folder, "results")

        WithWindow(prompts,
            Async Function(form)
                Await form.AddPathsAsync({pdf})
                form.tabOperations.SelectedTab = form.tabExtract
                Assert.Equal("locked-pages", form.txtOutputName.Text)
                form.txtExtractPages.Text = "2"
                form.txtOutputFolder.Text = outFolder
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)
            End Function)

        Assert.Equal({"L2"}, _f.PageLabels(Path.Combine(outFolder, "locked-pages.pdf")))
    End Sub

    <Fact>
    Public Sub Declining_to_replace_leaves_the_existing_file()
        _f.MakePdf("a.pdf", "A", 1)
        Dim outFolder = Directory.CreateDirectory(Path.Combine(_f.Folder, "results")).FullName
        Dim existing = Path.Combine(outFolder, "a-protected.pdf")
        File.WriteAllText(existing, "keep me")
        Dim prompts As New FakePrompts With {.ReplaceAnswer = False}

        WithWindow(prompts,
            Async Function(form)
                Await form.AddPathsAsync({Path.Combine(_f.Folder, "a.pdf")})
                form.tabOperations.SelectedTab = form.tabEncrypt
                form.txtOpenPassword.Text = "pw"
                form.txtConfirmPassword.Text = "pw"
                form.txtOutputFolder.Text = outFolder
                Await form.RunOperationAsync()
                Assert.Contains("Nothing was changed", form.lblStatus.Text)
            End Function)

        Assert.Equal(1, prompts.ReplaceAsked)
        Assert.Equal("keep me", File.ReadAllText(existing))
    End Sub

    <Fact>
    Public Sub Big_operation_buttons_and_theme_buttons_work()
        WithWindow(New FakePrompts(),
            Function(form)
                form.rbOpEncrypt.PerformClick()
                Assert.Same(form.tabEncrypt, form.tabOperations.SelectedTab)
                Assert.Equal("Encrypt PDFs", form.btnRun.Text)

                form.tabOperations.SelectedTab = form.tabMerge
                Assert.True(form.rbOpMerge.Checked)
                Assert.False(form.rbOpEncrypt.Checked)
                Assert.True(form.colPagesToUse.Visible)

                form.rbThemeDark.PerformClick()
                Assert.True(form.CurrentPalette.IsDark)
                Assert.Equal(Palette.DarkPalette.Back, form.rootLayout.BackColor)
                Assert.Equal(Palette.DarkPalette.Text, form.lblStep1.ForeColor)

                form.rbThemeLight.PerformClick()
                Assert.False(form.CurrentPalette.IsDark)
                Assert.Equal(Palette.LightPalette.Back, form.rootLayout.BackColor)
                Assert.NotNull(form.Icon)
                Assert.NotNull(form.picLogo.Image)
                Return Task.CompletedTask
            End Function)
    End Sub

    <Fact>
    Public Sub New_operation_buttons_run_rotate_stamp_and_reduce_size()
        Dim pdf = _f.MakePdf("doc.pdf", "A", 2)
        Dim stampPdf = Path.Combine(Directory.CreateDirectory(Path.Combine(_f.Folder, "stamps")).FullName, "mark.pdf")
        File.Copy(_f.MakePdf("mark-src.pdf", "S", 1), stampPdf)
        Dim outFolder = Path.Combine(_f.Folder, "results")

        WithWindow(New FakePrompts(),
            Async Function(form)
                Await form.AddPathsAsync({pdf})
                form.txtOutputFolder.Text = outFolder

                form.rbOpRotate.PerformClick()
                Assert.Same(form.tabRotate, form.tabOperations.SelectedTab)
                Assert.Equal("doc-rotated", form.txtOutputName.Text)
                form.rbRotate180.Checked = True
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)

                form.rbOpStamp.PerformClick()
                Assert.Equal("Add Stamp", form.btnRun.Text)
                form.rbStampUnder.Checked = True
                Assert.Equal("Add Background", form.btnRun.Text)
                Assert.Equal("doc-with-background", form.txtOutputName.Text)
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Failure, form.LastStatusKind)
                Assert.Contains("Choose the PDF to use as the background", form.lblStatus.Text)
                Assert.False(form.btnStampBrowse.Enabled)
                Assert.True(form.btnBackgroundBrowse.Enabled)
                Await form.SetBackgroundFileAsync(stampPdf)
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)

                ' Both at once: the stamp row comes back and both PDFs are used.
                form.rbStampBoth.Checked = True
                Assert.True(form.btnStampBrowse.Enabled)
                Assert.True(form.btnBackgroundBrowse.Enabled)
                ' "&&" is how a button shows a single "&".
                Assert.Equal("Add Stamp && Background", form.btnRun.Text)
                Assert.Equal("doc-stamped", form.txtOutputName.Text)
                Await form.RunOperationAsync()
                Assert.Contains("Choose the PDF to use as the stamp", form.lblStatus.Text)
                Await form.SetStampFileAsync(stampPdf)
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)

                form.rbOpCompress.PerformClick()
                Assert.Equal("Reduce Size", form.btnRun.Text)
                Assert.True(form.rbCompressMedium.Checked)
                form.rbCompressExtreme.Checked = True
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)
            End Function)

        Assert.Equal(180, _f.PageRotation(Path.Combine(outFolder, "doc-rotated.pdf"), 1))
        Assert.Contains("(S1) Tj", _f.PageText(Path.Combine(outFolder, "doc-with-background.pdf"), 2))
        Dim both = _f.PageText(Path.Combine(outFolder, "doc-stamped.pdf"), 1)
        Assert.Equal(2, Text.RegularExpressions.Regex.Matches(both, "\(S1\) Tj").Count)
        Assert.Equal({"A1", "A2"}, _f.PageLabels(Path.Combine(outFolder, "doc-smaller.pdf")))
    End Sub

    <Fact>
    Public Sub A_mistake_is_explained_in_the_window()
        _f.MakePdf("a.pdf", "A", 3)
        WithWindow(New FakePrompts(),
            Async Function(form)
                Await form.AddPathsAsync({Path.Combine(_f.Folder, "a.pdf")})
                form.tabOperations.SelectedTab = form.tabExtract
                form.txtExtractPages.Text = "7"
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Failure, form.LastStatusKind)
                Assert.Contains("There is no page 7", form.lblStatus.Text)
            End Function)
    End Sub



End Class
