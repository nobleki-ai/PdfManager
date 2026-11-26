Imports System.IO
Imports Xunit

''' <summary>Step 3's extras: every new PDF can also get a password and/or a stamp and background.</summary>
Public Class ExtrasTests
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

    Private Shared Function Lock(password As String) As ProtectionSettings
        Return New ProtectionSettings With {.OpenPassword = password, .ConfirmPassword = password}
    End Function

    <Fact>
    Public Sub Combined_pdf_can_also_get_a_password()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 2))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 1))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Combine, .Items = {a, b}, .ExtraProtection = Lock("new pw"),
            .OutputFolder = Out(), .OutputName = "combined"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Dim output = result.OutputFiles.Single()
        Assert.True(_f.NeedsPassword(output))
        Assert.Equal({"A1", "A2", "B1"}, _f.PageLabels(output, "new pw"))
    End Sub

    <Fact>
    Public Sub Keeping_the_first_files_details_still_works_with_a_new_password()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 1, password:="old"), "old")
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 1))
        Dim request As New OperationRequest With {
            .Operation = PdfOperation.Combine, .Items = {a, b}, .KeepFirstFileDetails = True, .ExtraProtection = Lock("new"),
            .OutputFolder = Out(), .OutputName = "kept"}
        Assert.DoesNotContain("--decrypt", Planner.Plan(request).Jobs.Single().Arguments)

        Dim result = _f.Run(request)
        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.Equal({"A1", "B1"}, _f.PageLabels(result.OutputFiles(0), "new"))
    End Sub

    <Fact>
    Public Sub Each_split_piece_gets_the_stamp_background_and_password()
        Dim doc = _f.Item(_f.MakePdf("doc.pdf", "D", 3))
        Dim stamp = _f.Item(_f.MakePdf("stamp.pdf", "S", 1))
        Dim paper = _f.Item(_f.MakePdf("paper.pdf", "P", 2, password:="bg"), "bg")
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.ExtractPages, .Items = {doc}, .ExtractPages = "1-3", .ExtractOutput = ExtractOutput.EachPage,
            .ExtraProtection = Lock("pw"),
            .ExtraLayers = New LayerSettings With {.Mode = StampMode.Both, .StampItem = stamp, .BackgroundItem = paper, .BackgroundPageNumber = 2},
            .OutputFolder = Out(), .OutputName = "page"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.Equal(3, result.OutputFiles.Count)
        For i = 0 To 2
            Dim piece = result.OutputFiles.OrderBy(Function(f) f).ToList()(i)
            Assert.True(_f.NeedsPassword(piece), piece)
            Dim text = _f.PageText(piece, 1, "pw")
            Assert.Contains($"(D{i + 1}) Tj", text)
            Assert.Contains("(S1) Tj", text)
            Assert.Contains("(P2) Tj", text)
        Next
    End Sub

    <Fact>
    Public Sub Stamp_goes_only_on_the_chosen_pages_of_the_new_pdf()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 2))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 2))
        Dim stamp = _f.Item(_f.MakePdf("stamp.pdf", "S", 1))
        ' Page 3 of the combined PDF is B1.
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Combine, .Items = {a, b},
            .ExtraLayers = New LayerSettings With {.StampItem = stamp, .Pages = "3"},
            .OutputFolder = Out(), .OutputName = "stamped"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Dim output = result.OutputFiles.Single()
        Assert.False(_f.NeedsPassword(output))
        Assert.DoesNotContain("(S1) Tj", _f.PageText(output, 2))
        Assert.Contains("(S1) Tj", _f.PageText(output, 3))
        Assert.Contains("(B1) Tj", _f.PageText(output, 3))
    End Sub

    <Fact>
    Public Sub A_locked_pdf_rotated_gets_the_new_password_instead_of_its_old_one()
        Dim locked = _f.Item(_f.MakePdf("locked.pdf", "L", 1, password:="old"), "old")
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Rotate, .Items = {locked}, .RotateAngle = 90, .ExtraProtection = Lock("fresh"),
            .OutputFolder = Out(), .OutputName = "turned"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Dim output = result.OutputFiles.Single()
        Assert.Equal({"L1"}, _f.PageLabels(output, "fresh"))
        Assert.Throws(Of InvalidOperationException)(Function() _f.PageCount(output, "old"))
    End Sub

    <Fact>
    Public Sub Encrypt_and_Stamp_ignore_the_extras_they_already_do()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 1))
        Dim encrypt As New OperationRequest With {
            .Operation = PdfOperation.Encrypt, .Items = {a}, .OpenPassword = "x", .ConfirmPassword = "x",
            .ExtraProtection = Lock("y"), .OutputFolder = Out(), .OutputName = "e"}
        Assert.False(encrypt.UsesExtraProtection)
        Assert.Single(Planner.Plan(encrypt).Jobs.Single().Arguments.Where(Function(arg) arg = "--encrypt"))

        Dim stamp As New OperationRequest With {
            .Operation = PdfOperation.Stamp, .ExtraLayers = New LayerSettings()}
        Assert.False(stamp.UsesExtraLayers)
    End Sub

    <Fact>
    Public Sub Extras_with_mistakes_are_caught_before_anything_runs()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 1))
        Dim mismatch = Planner.Validate(New OperationRequest With {
            .Operation = PdfOperation.Combine, .Items = {a, a},
            .ExtraProtection = New ProtectionSettings With {.OpenPassword = "a", .ConfirmPassword = "b"},
            .OutputFolder = Out(), .OutputName = "x"})
        Assert.Equal(RequestField.ExtraPassword, mismatch.Field)
        Assert.Contains("don't match", mismatch.Message)

        Dim noStamp = Planner.Validate(New OperationRequest With {
            .Operation = PdfOperation.Rotate, .Items = {a}, .ExtraLayers = New LayerSettings(),
            .OutputFolder = Out(), .OutputName = "x"})
        Assert.Equal(RequestField.ExtraStamp, noStamp.Field)

        Dim badPages = Planner.Validate(New OperationRequest With {
            .Operation = PdfOperation.Rotate, .Items = {a},
            .ExtraLayers = New LayerSettings With {.StampItem = a, .Pages = "abc"},
            .OutputFolder = Out(), .OutputName = "x"})
        Assert.Equal(RequestField.ExtraStamp, badPages.Field)
    End Sub

    <Fact>
    Public Sub Window_applies_the_extras_and_greys_them_out_where_they_dont_apply()
        Dim a = _f.MakePdf("a.pdf", "A", 1)
        Dim b = _f.MakePdf("b.pdf", "B", 1)
        Dim results = Path.Combine(_f.Folder, "results")

        WindowTests.WithWindow(New WindowTests.FakePrompts(),
            Async Function(form)
                Await form.AddPathsAsync({a, b})
                form.SetExtras(Lock("pw"), Nothing)
                Assert.True(form.chkExtraPassword.Checked)
                Assert.Contains("protected with a password", form.lblPreview.Text)

                form.tabOperations.SelectedTab = form.tabEncrypt
                Assert.False(form.chkExtraPassword.Enabled)
                Assert.True(form.chkExtraStamp.Enabled)
                form.tabOperations.SelectedTab = form.tabStamp
                Assert.False(form.chkExtraStamp.Enabled)
                Assert.True(form.chkExtraPassword.Enabled)

                form.tabOperations.SelectedTab = form.tabCombine
                form.txtOutputFolder.Text = results
                form.txtOutputName.Text = "both"
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)
            End Function)

        Dim output = Path.Combine(results, "both.pdf")
        Assert.True(_f.NeedsPassword(output))
        Assert.Equal({"A1", "B1"}, _f.PageLabels(output, "pw"))
    End Sub

End Class
