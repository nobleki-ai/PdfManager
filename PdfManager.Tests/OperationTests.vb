Imports System.IO
Imports Xunit

''' <summary>Runs each operation through the real QPDF and checks which pages ended up where.</summary>
Public Class OperationTests
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

    Private Shared Function Labels(prefix As String, ParamArray pages As Integer()) As List(Of String)
        Return pages.Select(Function(p) prefix & p).ToList()
    End Function

    <Fact>
    Public Sub Combine_joins_every_page_in_list_order()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 3))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 2))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Combine, .Items = {b, a}, .OutputFolder = Out(), .OutputName = "combined"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.Equal(Path.Combine(Out(), "combined.pdf"), result.OutputFiles.Single())
        Assert.Equal({"B1", "B2", "A1", "A2", "A3"}, _f.PageLabels(result.OutputFiles(0)))
        Assert.Equal("Created combined.pdf (5 pages).", result.Message)
    End Sub

    <Fact>
    Public Sub Combine_keeping_details_works_with_a_locked_first_file_and_removes_its_lock()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 2, password:="pw"), "pw")
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 1))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Combine, .Items = {a, b}, .KeepFirstFileDetails = True,
            .OutputFolder = Out(), .OutputName = "kept"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.False(_f.NeedsPassword(result.OutputFiles(0)))
        Assert.Equal({"A1", "A2", "B1"}, _f.PageLabels(result.OutputFiles(0)))
    End Sub

    <Fact>
    Public Sub Merge_takes_the_chosen_pages_from_each_file()
        ' The person's own example: pages 1-10 of the first PDF and page 5 of the second.
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 12))
        a.PagesText = "1-10"
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 6))
        b.PagesText = "5"
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.MergePages, .Items = {a, b}, .OutputFolder = Out(), .OutputName = "merged"})

        Dim expected = Labels("A", 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)
        expected.Add("B5")
        Assert.Equal(expected, _f.PageLabels(result.OutputFiles(0)))
    End Sub

    <Fact>
    Public Sub Merge_understands_lists_end_and_odd()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 6))
        a.PagesText = "6, 1-2, 4-end"
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 5))
        b.PagesText = "odd"
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.MergePages, .Items = {a, b}, .OutputFolder = Out(), .OutputName = "m"})

        Assert.Equal({"A6", "A1", "A2", "A4", "A5", "A6", "B1", "B3", "B5"}, _f.PageLabels(result.OutputFiles(0)))
    End Sub

    <Fact>
    Public Sub Extract_into_one_pdf()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 8))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.ExtractPages, .Items = {a}, .ExtractPages = "2, 4-5",
            .OutputFolder = Out(), .OutputName = "picked"})

        Assert.Equal({"A2", "A4", "A5"}, _f.PageLabels(result.OutputFiles(0)))
    End Sub

    <Fact>
    Public Sub Extract_one_pdf_per_page_names_the_files_after_the_output_name()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 8))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.ExtractPages, .Items = {a}, .ExtractPages = "3-5",
            .ExtractOutput = ExtractOutput.EachPage, .OutputFolder = Out(), .OutputName = "page"})

        Dim names = result.OutputFiles.Select(AddressOf Path.GetFileName).OrderBy(Function(n) n).ToList()
        Assert.Equal({"page-1.pdf", "page-2.pdf", "page-3.pdf"}, names)
        Assert.Equal({"A3"}, _f.PageLabels(Path.Combine(Out(), "page-1.pdf")))
        Assert.Equal({"A5"}, _f.PageLabels(Path.Combine(Out(), "page-3.pdf")))
    End Sub

    <Fact>
    Public Sub Extract_every_n_pages_from_every_file()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 4))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 4))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.ExtractPages, .Items = {a, b}, .ExtractPages = "all",
            .ExtractOutput = ExtractOutput.EveryNPages, .SplitEvery = 2, .OutputFolder = Out(), .OutputName = "part"})

        Assert.Equal(4, result.OutputFiles.Count)
        Dim first = result.OutputFiles.First(Function(f) Path.GetFileName(f).StartsWith("part-a-1-2", StringComparison.Ordinal))
        Assert.Equal({"A1", "A2"}, _f.PageLabels(first))
        Assert.Contains(result.OutputFiles, Function(f) Path.GetFileName(f) = "part-b-3-4.pdf")
    End Sub

    <Fact>
    Public Sub Encrypt_protects_each_file_with_the_password()
        Dim a = _f.Item(_f.MakePdf("Résumé.pdf", "A", 2))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 1, password:="old"), "old")
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Encrypt, .Items = {a, b},
            .OpenPassword = "pässwörd 1", .ConfirmPassword = "pässwörd 1",
            .OutputFolder = Out(), .OutputName = "protected"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Dim names = result.OutputFiles.Select(AddressOf Path.GetFileName).ToList()
        Assert.Equal({"Résumé-protected.pdf", "b-protected.pdf"}, names)
        For Each output In result.OutputFiles
            Assert.True(_f.NeedsPassword(output))
        Next
        Assert.Equal({"A1", "A2"}, _f.PageLabels(result.OutputFiles(0), "pässwörd 1"))
        Assert.Equal({"B1"}, _f.PageLabels(result.OutputFiles(1), "pässwörd 1"))
    End Sub

    <Fact>
    Public Sub Encrypt_with_only_a_permissions_password_opens_freely()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 1))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Encrypt, .Items = {a}, .PermissionsPassword = "boss",
            .AllowPrinting = False, .AllowCopying = False,
            .OutputFolder = Out(), .OutputName = "locked"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.False(_f.NeedsPassword(result.OutputFiles(0)))
        Assert.Equal({"A1"}, _f.PageLabels(result.OutputFiles(0)))
    End Sub

    <Theory>
    <InlineData(90, 90)>
    <InlineData(-90, 270)>
    <InlineData(180, 180)>
    Public Sub Rotate_turns_only_the_chosen_pages(angle As Integer, expected As Integer)
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 4))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Rotate, .Items = {a}, .RotateAngle = angle, .RotatePages = "2-3",
            .OutputFolder = Out(), .OutputName = "turned"})

        Dim output = result.OutputFiles.Single()
        Assert.Equal({"A1", "A2", "A3", "A4"}, _f.PageLabels(output))
        Assert.Equal(0, _f.PageRotation(output, 1))
        Assert.Equal(expected, _f.PageRotation(output, 2))
        Assert.Equal(expected, _f.PageRotation(output, 3))
        Assert.Equal(0, _f.PageRotation(output, 4))
    End Sub

    <Fact>
    Public Sub Rotate_keeps_a_locked_file_locked()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 2, password:="pw"), "pw")
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Rotate, .Items = {a}, .OutputFolder = Out(), .OutputName = "turned"})

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.True(_f.NeedsPassword(result.OutputFiles(0)))
        Assert.Equal(90, PageRotationWithPassword(result.OutputFiles(0), "pw"))
    End Sub

    Private Function PageRotationWithPassword(pdf As String, password As String) As Integer
        Dim match = Text.RegularExpressions.Regex.Match(_f.PageText(pdf, 1, password), "/Rotate (-?\d+)")
        Return If(match.Success, Integer.Parse(match.Groups(1).Value), 0)
    End Function

    <Theory>
    <InlineData(StampMode.StampOnly)>
    <InlineData(StampMode.BackgroundOnly)>
    Public Sub Stamp_or_background_puts_the_chosen_page_on_the_chosen_pages(mode As StampMode)
        Dim doc = _f.Item(_f.MakePdf("letter.pdf", "A", 3))
        Dim layer = _f.Item(_f.MakePdf("layer.pdf", "S", 2))
        Dim request As New OperationRequest With {
            .Operation = PdfOperation.Stamp, .Items = {doc}, .StampMode = mode,
            .StampPages = "1,3", .OutputFolder = Out(), .OutputName = "marked"}
        If mode = StampMode.StampOnly Then
            request.StampItem = layer
            request.StampPageNumber = 2
        Else
            request.BackgroundItem = layer
            request.BackgroundPageNumber = 2
        End If
        Dim plan = Planner.Plan(request)
        Assert.Contains(If(mode = StampMode.StampOnly, "--overlay", "--underlay"), plan.Jobs(0).Arguments)
        Assert.DoesNotContain(If(mode = StampMode.StampOnly, "--underlay", "--overlay"), plan.Jobs(0).Arguments)
        Dim result = _f.Run(request)

        Dim output = result.OutputFiles.Single()
        Assert.Equal(3, _f.PageCount(output))
        Assert.Contains("(A1) Tj", _f.PageText(output, 1))
        Assert.Contains("(S2) Tj", _f.PageText(output, 1))
        Assert.DoesNotContain("(S2) Tj", _f.PageText(output, 2))
        Assert.Contains("(S2) Tj", _f.PageText(output, 3))
        Assert.DoesNotContain("(S1) Tj", _f.PageText(output, 3))
    End Sub

    <Fact>
    Public Sub Both_adds_the_stamp_and_the_background_in_one_go()
        Dim doc = _f.Item(_f.MakePdf("letter.pdf", "A", 3))
        Dim stamp = _f.Item(_f.MakePdf("stamp.pdf", "S", 2))
        Dim background = _f.Item(_f.MakePdf("paper.pdf", "B", 3, password:="bg"), "bg")
        Dim request As New OperationRequest With {
            .Operation = PdfOperation.Stamp, .Items = {doc}, .StampMode = StampMode.Both,
            .StampItem = stamp, .StampPageNumber = 2, .BackgroundItem = background, .BackgroundPageNumber = 3,
            .StampPages = "2-end", .OutputFolder = Out(), .OutputName = "both"}

        Dim plan = Planner.Plan(request)
        Assert.Single(plan.Jobs)
        Assert.Contains("--overlay", plan.Jobs(0).Arguments)
        Assert.Contains("--underlay", plan.Jobs(0).Arguments)

        Dim result = _f.Run(request)
        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Dim output = result.OutputFiles.Single()
        Assert.Equal({"A1", "A2", "A3"}.ToList(), {1, 2, 3}.Select(Function(p) If(_f.PageText(output, p).Contains($"(A{p}) Tj"), $"A{p}", "?")).ToList())
        Dim page1 = _f.PageText(output, 1)
        Assert.DoesNotContain("(S2) Tj", page1)
        Assert.DoesNotContain("(B3) Tj", page1)
        For Each page In {2, 3}
            Dim text = _f.PageText(output, page)
            Assert.Contains("(S2) Tj", text)
            Assert.Contains("(B3) Tj", text)
        Next
    End Sub

    <Fact>
    Public Sub Both_needs_both_pdfs()
        Dim doc = _f.Item(_f.MakePdf("letter.pdf", "A", 1))
        Dim stamp = _f.Item(_f.MakePdf("stamp.pdf", "S", 1))
        Dim issue = Planner.Validate(New OperationRequest With {
            .Operation = PdfOperation.Stamp, .Items = {doc}, .StampMode = StampMode.Both, .StampItem = stamp,
            .OutputFolder = Out(), .OutputName = "both"})
        Assert.Equal(RequestField.BackgroundFile, issue.Field)
        Assert.Contains("background", issue.Message)
    End Sub

    <Fact>
    Public Sub Stamp_page_beyond_the_stamp_pdf_is_explained()
        Dim doc = _f.Item(_f.MakePdf("letter.pdf", "A", 1))
        Dim stamp = _f.Item(_f.MakePdf("stamp.pdf", "S", 1))
        Dim issue = Planner.Validate(New OperationRequest With {
            .Operation = PdfOperation.Stamp, .Items = {doc}, .StampItem = stamp, .StampPageNumber = 3,
            .OutputFolder = Out(), .OutputName = "marked"})
        Assert.Equal(RequestField.StampFile, issue.Field)
        Assert.Contains("only 1 page", issue.Message)
    End Sub

    <Fact>
    Public Sub Reduce_size_levels_get_smaller_in_order_and_keep_the_pages()
        Dim photo = _f.Item(_f.MakePhotoPdf("photo.pdf"))
        Dim sizes As New Dictionary(Of CompressLevel, Long)
        For Each level In {CompressLevel.Normal, CompressLevel.Medium, CompressLevel.High, CompressLevel.Extreme}
            Dim result = _f.Run(New OperationRequest With {
                .Operation = PdfOperation.Compress, .Items = {photo}, .CompressLevel = level,
                .OutputFolder = Out(), .OutputName = "small-" & level.ToString()})
            Assert.Equal(RunOutcome.Succeeded, result.Outcome)
            Assert.Equal(1, _f.PageCount(result.OutputFiles(0)))
            sizes(level) = New FileInfo(result.OutputFiles(0)).Length
            Assert.Equal(photo.SizeBytes, result.InputBytes)
        Next

        Assert.True(sizes(CompressLevel.Medium) < photo.SizeBytes * 0.7, $"Medium: {sizes(CompressLevel.Medium)} vs {photo.SizeBytes}")
        Assert.True(sizes(CompressLevel.High) < sizes(CompressLevel.Medium))
        Assert.True(sizes(CompressLevel.Extreme) < sizes(CompressLevel.High))
    End Sub

    <Fact>
    Public Sub Reduce_size_reports_the_saving()
        Dim photo = _f.Item(_f.MakePhotoPdf("photo.pdf"))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Compress, .Items = {photo}, .CompressLevel = CompressLevel.High,
            .OutputFolder = Out(), .OutputName = "smaller"})
        Assert.Matches("KB → \d+ KB, \d+% smaller\.", result.Message)
    End Sub

    <Fact>
    Public Sub Locked_files_are_recognised_and_unlock_with_their_password()
        Dim pdf = _f.MakePdf("secret.pdf", "S", 3, password:="open sesame")
        Dim locked = _f.Item(pdf)
        Assert.Equal(PdfState.PasswordNeeded, locked.State)

        Dim wrong = _f.Item(pdf, "nope")
        Assert.Equal(PdfState.PasswordNeeded, wrong.State)

        Dim unlocked = _f.Item(pdf, "open sesame")
        Assert.Equal(PdfState.Ready, unlocked.State)
        Assert.Equal(3, unlocked.PageCount)
    End Sub

    <Fact>
    Public Sub A_damaged_file_is_reported_not_used()
        Dim bad = Path.Combine(_f.Folder, "bad.pdf")
        File.WriteAllText(bad, "this is not a pdf")
        Dim item = _f.Item(bad)
        Assert.Equal(PdfState.Unreadable, item.State)

        Dim issue = Planner.Validate(New OperationRequest With {
            .Operation = PdfOperation.Encrypt, .Items = {item}, .OpenPassword = "x", .ConfirmPassword = "x",
            .OutputFolder = Out(), .OutputName = "p"})
        Assert.Contains("can't be read", issue.Message)
    End Sub

    <Fact>
    Public Sub A_failed_run_leaves_existing_files_untouched()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 2))
        Dim existing = Path.Combine(Out(), "keep.pdf")
        File.WriteAllText(existing, "precious")

        ' Damage the source after it was read, so QPDF fails halfway.
        File.WriteAllText(a.Path, "broken now")
        Dim runner As New OperationRunner(_f.Tool, Nothing)
        Dim plan = Planner.Plan(New OperationRequest With {
            .Operation = PdfOperation.Combine, .Items = {a, a}, .OutputFolder = Out(), .OutputName = "keep"})
        Dim result = runner.RunAsync(plan, Threading.CancellationToken.None).GetAwaiter().GetResult()

        Assert.Equal(RunOutcome.Failed, result.Outcome)
        Assert.Equal("precious", File.ReadAllText(existing))
    End Sub

End Class
