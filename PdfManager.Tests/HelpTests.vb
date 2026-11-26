Imports System.IO
Imports System.Reflection
Imports System.Windows.Forms
Imports Xunit

''' <summary>The Help window, and the page-range examples it shows checked against QPDF itself.</summary>
Public Class HelpTests
    Implements IDisposable

    Private ReadOnly _f As New Fixtures()

    Public Sub Dispose() Implements IDisposable.Dispose
        _f.Dispose()
    End Sub

    <Fact>
    Public Sub Every_page_range_example_in_Help_picks_what_QPDF_really_extracts()
        Dim pdf = _f.Item(_f.MakePdf("ten.pdf", "P", 10))
        Dim outFolder = Path.Combine(_f.Folder, "out")
        Dim n = 0
        For Each example In HelpForm.PageRangeExamples
            Dim promised = PageRange.Expand(example.Text, 10)
            Assert.NotNull(promised)
            n += 1
            Dim result = _f.Run(New OperationRequest With {
                .Operation = PdfOperation.ExtractPages, .Items = {pdf}, .ExtractPages = example.Text,
                .OutputFolder = outFolder, .OutputName = $"example-{n}"})
            Assert.Equal(RunOutcome.Succeeded, result.Outcome)
            Dim actual = _f.PageLabels(result.OutputFiles.Single())
            Assert.Equal(promised.Select(Function(p) "P" & p).ToList(), actual)
        Next
    End Sub

    <Theory>
    <InlineData("1-3, 5, 8-end", 10, "1,2,3,5,8,9,10")>
    <InlineData("end-1", 4, "4,3,2,1")>
    <InlineData("odd", 5, "1,3,5")>
    <InlineData("even", 5, "2,4")>
    <InlineData("", 3, "1,2,3")>
    <InlineData("last", 7, "7")>
    Public Sub Expand_lists_the_pages_in_order(text As String, count As Integer, expected As String)
        Assert.Equal(expected, String.Join(",", PageRange.Expand(text, count)))
    End Sub

    <Fact>
    Public Sub Expand_returns_nothing_for_a_mistake()
        Assert.Null(PageRange.Expand("12", 10))
        Assert.Null(PageRange.Expand("abc", 10))
    End Sub

    <Fact>
    Public Sub Help_button_F1_and_page_range_links_open_the_right_topics()
        WindowTests.WithWindow(New WindowTests.FakePrompts(),
            Function(form)
                form.btnHelp.PerformClick()
                Dim help = form.ShowHelp()
                Assert.True(help.Visible)
                Assert.Equal(HelpForm.TopicGettingStarted, help.CurrentTopic)

                ' Every topic has something to say.
                For Each title In help.TopicTitles
                    help.ShowTopic(title)
                    Assert.True(help.TopicText.Length > 200, title)
                Next

                ' Pictures of the window are shown in the topics about it.
                For Each expected In {(HelpForm.TopicGettingStarted, 2), (HelpForm.TopicMerge, 1), (HelpForm.TopicExtract, 1),
                                      (HelpForm.TopicArrange, 2), (HelpForm.TopicEncrypt, 2), (HelpForm.TopicRotate, 1),
                                      (HelpForm.TopicStamp, 1), (HelpForm.TopicCompress, 1)}
                    help.ShowTopic(expected.Item1)
                    Assert.True(expected.Item2 = help.PictureCount, $"{expected.Item1}: {help.PictureCount} pictures")
                Next
                help.ShowTopic(HelpForm.TopicWhatsNew)
                Assert.Contains("Remove the password", help.TopicText)

                ' F1 shows the topic for the operation in use.
                form.rbOpRotate.PerformClick()
                Dim processKey = GetType(MainForm).GetMethod("ProcessCmdKey", BindingFlags.Instance Or BindingFlags.NonPublic)
                processKey.Invoke(form, New Object() {New Message(), Keys.F1})
                Assert.Equal(HelpForm.TopicRotate, help.CurrentTopic)

                ' The link next to a page box goes to Page ranges, with the Try it box.
                form.rbOpExtract.PerformClick()
                form.ShowHelp(HelpForm.TopicPageRanges)
                Assert.Equal(HelpForm.TopicPageRanges, help.CurrentTopic)
                Assert.Contains("1-3, 5, 8-end", help.TopicText)
                Assert.Contains("Picks: 1, 2, 3, 5, 8, 9, 10", help.TopicText)

                help.TryRange("end-1", 4)
                Assert.Equal("Picks 4 pages: 4, 3, 2, 1", help.TryResultText)
                help.TryRange("9", 4)
                Assert.Contains("There is no page 9", help.TryResultText)

                ' It follows the theme.
                form.SetThemeChoice(ThemeChoice.Dark)
                Assert.Equal(Palette.DarkPalette.Surface, help.rtbHelp.BackColor)
                help.Close()
                Return Task.CompletedTask
            End Function)
    End Sub
End Class
