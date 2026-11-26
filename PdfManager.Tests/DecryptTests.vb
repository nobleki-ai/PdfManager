Imports System.IO
Imports System.Threading
Imports Xunit

''' <summary>Remove the password (decrypt), on the Encrypt page.</summary>
Public Class DecryptTests
    Implements IDisposable

    Private ReadOnly _f As New Fixtures()

    Public Sub Dispose() Implements IDisposable.Dispose
        _f.Dispose()
    End Sub

    Private Function Out() As String
        Dim folder = Path.Combine(_f.Folder, "out")
        Directory.CreateDirectory(folder)
        Return folder
    End Function

    ''' <summary>A PDF anyone can open, but with printing and copying stopped by a permissions password.</summary>
    Private Function MakeLimitedPdf(fileName As String) As String
        Dim plain = _f.MakePdf("plain-" & fileName, "L", 2)
        Dim target = Path.Combine(_f.Folder, fileName)
        Dim result = _f.Tool.RunAsync({"--encrypt", "--user-password=", "--owner-password=boss", "--bits=256", "--print=none", "--extract=n", "--",
                                       plain, target}, CancellationToken.None).GetAwaiter().GetResult()
        Assert.True(result.Succeeded, result.Output)
        Return target
    End Function

    Private Function EncryptionReport(pdf As String) As String
        Return _f.Tool.RunAsync({"--show-encryption", pdf}, CancellationToken.None).GetAwaiter().GetResult().Output
    End Function

    <Fact>
    Public Sub Removes_the_password_from_a_locked_pdf()
        Dim locked = _f.Item(_f.MakePdf("Locked file.pdf", "A", 3, password:="pässwörd"), "pässwörd")
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Decrypt, .Items = {locked}, .OutputFolder = Out(), .OutputName = "Locked file-unlocked"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Dim output = result.OutputFiles.Single()
        Assert.Equal("Locked file-unlocked.pdf", Path.GetFileName(output))
        Assert.False(_f.NeedsPassword(output))
        Assert.Contains("not encrypted", EncryptionReport(output))
        Assert.Equal({"A1", "A2", "A3"}, _f.PageLabels(output))
        ' The original keeps its password.
        Assert.True(_f.NeedsPassword(locked.Path))
    End Sub

    <Fact>
    Public Sub Removes_limits_without_knowing_the_permissions_password()
        Dim limited = _f.Item(MakeLimitedPdf("limited.pdf"))
        Assert.Equal(PdfState.Ready, limited.State)
        PdfInspector.CheckEncryptionAsync(_f.Tool, limited).GetAwaiter().GetResult()
        Assert.True(limited.IsEncrypted)

        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Decrypt, .Items = {limited}, .OutputFolder = Out(), .OutputName = "free"})
        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.Contains("not encrypted", EncryptionReport(result.OutputFiles.Single()))
    End Sub

    <Fact>
    Public Sub Each_file_gets_its_own_unlocked_copy()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 1, password:="one"), "one")
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 2, password:="two"), "two")
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Decrypt, .Items = {a, b}, .OutputFolder = Out(), .OutputName = "unlocked"})
        Assert.Equal({"a-unlocked.pdf", "b-unlocked.pdf"}, result.OutputFiles.Select(AddressOf Path.GetFileName))
        Assert.Equal({"B1", "B2"}, _f.PageLabels(result.OutputFiles(1)))
    End Sub

    <Fact>
    Public Sub Knows_which_pdfs_have_no_password()
        Dim plain = _f.Item(_f.MakePdf("plain.pdf", "P", 1))
        PdfInspector.CheckEncryptionAsync(_f.Tool, plain).GetAwaiter().GetResult()
        Assert.False(plain.IsEncrypted)
    End Sub

    <Fact>
    Public Sub Window_unlocks_locked_pdfs_and_skips_the_others()
        Dim locked = _f.MakePdf("locked.pdf", "K", 2, password:="secret")
        Dim limited = MakeLimitedPdf("limited.pdf")
        _f.MakePdf("plain.pdf", "P", 1)
        Dim results = Path.Combine(_f.Folder, "results")
        Dim prompts As New WindowTests.FakePrompts()
        prompts.Passwords.Enqueue("secret")

        WindowTests.WithWindow(prompts,
            Async Function(form)
                Await form.AddPathsAsync({_f.Folder})
                form.tabOperations.SelectedTab = form.tabEncrypt
                form.rbRemovePassword.Checked = True
                Assert.Equal(PdfOperation.Decrypt, form.CurrentOperation)
                Assert.Equal("Remove Passwords", form.btnRun.Text)
                Assert.False(form.txtOpenPassword.Enabled)
                Assert.True(form.rbOpEncrypt.Checked)

                form.txtOutputFolder.Text = results
                form.txtOutputName.Text = "unlocked"
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)
                ' plain.pdf and the two unlocked originals the test PDFs were made from.
                Assert.Contains("3 PDFs had no password and were skipped", form.lblStatus.Text)
            End Function)

        Dim made = Directory.GetFiles(results).Select(AddressOf Path.GetFileName).OrderBy(Function(n) n).ToList()
        Assert.Equal({"limited-unlocked.pdf", "locked-unlocked.pdf"}, made)
        Assert.Equal({"K1", "K2"}, _f.PageLabels(Path.Combine(results, "locked-unlocked.pdf")))
        Assert.Contains("not encrypted", EncryptionReport(Path.Combine(results, "limited-unlocked.pdf")))
    End Sub

    <Fact>
    Public Sub Window_says_so_when_nothing_has_a_password()
        _f.MakePdf("plain.pdf", "P", 1)
        WindowTests.WithWindow(New WindowTests.FakePrompts(),
            Async Function(form)
                Await form.AddPathsAsync({_f.Folder})
                form.tabOperations.SelectedTab = form.tabEncrypt
                form.rbRemovePassword.Checked = True
                form.txtOutputFolder.Text = Path.Combine(_f.Folder, "results")
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Failure, form.LastStatusKind)
                Assert.Contains("no password", form.lblStatus.Text)
            End Function)
        Assert.False(Directory.Exists(Path.Combine(_f.Folder, "results")))
    End Sub

End Class
