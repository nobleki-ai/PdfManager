Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports System.Windows.Forms
Imports Xunit

''' <summary>Arrange Pages: page pictures, dragging pages into a new order, leaving pages out, and saving with QPDF.</summary>
Public Class ArrangeTests
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

    Private Shared Function Pages(ParamArray refs As (Item As PdfItem, Page As Integer)()) As List(Of PageRef)
        Return refs.Select(Function(r) New PageRef(r.Item, r.Page)).ToList()
    End Function

    <Fact>
    Public Sub Neighbouring_pages_are_joined_into_short_ranges()
        Assert.Equal("1-3,7,6", Planner.PageList({1, 2, 3, 7, 6}))
        Assert.Equal("5", Planner.PageList({5}))
        Assert.Equal("2,1", Planner.PageList({2, 1}))
        Assert.Equal("1-2,4-5", Planner.PageList({1, 2, 4, 5}))
    End Sub

    <Fact>
    Public Sub Saving_keeps_the_new_order_and_leaves_out_removed_pages()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 4))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 3, password:="pw"), "pw")
        ' B2 first, then A1-A2, B3, A4 (A3 and B1 left out).
        Dim arranged = Pages((b, 2), (a, 1), (a, 2), (b, 3), (a, 4))
        Dim request As New OperationRequest With {
            .Operation = PdfOperation.ArrangePages, .Items = {a, b}, .ArrangedPages = arranged,
            .OutputFolder = Out(), .OutputName = "arranged"}

        Dim plan = Planner.Plan(request)
        Assert.Equal({"--empty", "--pages", b.Path, "--password=pw", "2", a.Path, "1-2", b.Path, "--password=pw", "3", a.Path, "4", "--", Planner.OutputToken},
                     plan.Jobs.Single().Arguments)

        Dim result = _f.Run(request)
        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.Equal({"B2", "A1", "A2", "B3", "A4"}, _f.PageLabels(result.OutputFiles.Single()))
        Assert.False(_f.NeedsPassword(result.OutputFiles(0)))
    End Sub

    <Fact>
    Public Sub Without_arranging_every_page_is_saved_in_list_order()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 2))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 1))
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.ArrangePages, .Items = {b, a}, .OutputFolder = Out(), .OutputName = "all"})
        Assert.Equal({"B1", "A1", "A2"}, _f.PageLabels(result.OutputFiles.Single()))
    End Sub

    <Fact>
    Public Sub Removing_every_page_is_refused()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 2))
        Dim issue = Planner.Validate(New OperationRequest With {
            .Operation = PdfOperation.ArrangePages, .Items = {a}, .ArrangedPages = New List(Of PageRef),
            .OutputFolder = Out(), .OutputName = "none"})
        Assert.NotNull(issue)
        Assert.Equal(RequestField.ArrangePages, issue.Field)
    End Sub

    <Fact>
    Public Sub Windows_draws_a_picture_of_each_page()
        Dim a = _f.Item(_f.MakePdf("pic.pdf", "P", 2))
        Using renderer As New PageRenderer(_f.Tool)
            Dim picture = renderer.RenderAsync(New PageRef(a, 2), CancellationToken.None).GetAwaiter().GetResult()
            Assert.True(picture IsNot Nothing, renderer.LastProblem)
            ' Windows draws at the screen's scale: 240 at 100%, 300 at 125% and so on.
            Assert.True(picture.Width >= PageRenderer.PictureWidth, $"Width {picture.Width}")
            Assert.True(picture.Height > picture.Width, "A portrait page should be taller than wide.")
            ' Not blank: the page's label is drawn in black on white.
            Dim dark = 0
            For y = 0 To picture.Height - 1 Step 2
                For x = 0 To picture.Width - 1 Step 2
                    If picture.GetPixel(x, y).GetBrightness() < 0.3 Then dark += 1
                Next
            Next
            Assert.True(dark > 20, $"Expected some dark (text) pixels, found {dark}.")
        End Using
    End Sub

    <Fact>
    Public Sub A_locked_pdf_is_drawn_with_its_password()
        Dim a = _f.Item(_f.MakePdf("locked.pdf", "L", 1, password:="secret"), "secret")
        Using renderer As New PageRenderer(_f.Tool)
            Dim picture = renderer.RenderAsync(New PageRef(a, 1), CancellationToken.None).GetAwaiter().GetResult()
            Assert.True(picture IsNot Nothing, renderer.LastProblem)
        End Using
    End Sub

    <Fact>
    Public Sub Dragging_selected_pages_moves_them_together_and_delete_removes_them()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 5))
        Using grid As New PageGrid()
            grid.SetPages(OperationRequest.AllPages({a}), {a})
            Dim order = Function() String.Join(",", grid.Pages.Select(Function(p) p.PageNumber))

            ' Pages 4 and 5 dropped in front of page 2.
            grid.SelectIndexes(3, 4)
            grid.MoveSelectedTo(1)
            Assert.Equal("1,4,5,2,3", order())

            ' Page 1 dropped after the last page.
            grid.SelectIndexes(0)
            grid.MoveSelectedTo(grid.Pages.Count)
            Assert.Equal("4,5,2,3,1", order())

            ' Dropping a page where it already is changes nothing.
            grid.SelectIndexes(2)
            grid.MoveSelectedTo(2)
            Assert.Equal("4,5,2,3,1", order())

            grid.SelectIndexes(1, 3)
            grid.RemoveSelected()
            Assert.Equal("4,2,1", order())
        End Using
    End Sub

    <Fact>
    Public Sub Summary_says_what_was_left_out_and_reordered()
        Dim a = _f.Item(_f.MakePdf("a.pdf", "A", 3))
        Dim b = _f.Item(_f.MakePdf("b.pdf", "B", 2))
        Assert.Equal("5 pages from 2 PDFs", PageViewForm.Describe(OperationRequest.AllPages({a, b}), {a, b}))
        Assert.Equal("2 pages from 2 PDFs  ·  3 left out  ·  new order",
                     PageViewForm.Describe(Pages((b, 1), (a, 2)), {a, b}))
        Assert.Equal("2 pages from 1 PDF  ·  3 left out", PageViewForm.Describe(Pages((a, 1), (a, 3)), {a, b}))
    End Sub

    <Fact>
    Public Sub Window_saves_the_arranged_pages_and_keeps_up_with_the_list()
        Dim a = _f.MakePdf("a.pdf", "A", 3)
        Dim b = _f.MakePdf("b.pdf", "B", 2)
        Dim results = Path.Combine(_f.Folder, "results")

        WindowTests.WithWindow(New WindowTests.FakePrompts(),
            Async Function(form)
                Await form.AddPathsAsync({a, b})
                form.tabOperations.SelectedTab = form.tabArrange
                Assert.True(form.rbOpArrange.Checked)
                Assert.Equal("Save Pages", form.btnRun.Text)
                Assert.Contains("all 5 pages", form.lblArrangeSummary.Text)

                ' Arrange in the page view: B2 first, and A2 left out.
                Using view = Await form.CreatePageViewAsync()
                    Assert.NotNull(view)
                    Assert.Equal(5, view.Grid.Pages.Count)
                    view.Grid.SelectIndexes(4)
                    view.Grid.MoveSelectedTo(0)
                    view.Grid.SelectIndexes(2)
                    view.Grid.RemoveSelected()
                    form.UseArrangement(view.Arrangement)
                End Using
                Assert.Equal({"b.pdf p2", "a.pdf p1", "a.pdf p3", "b.pdf p1"}, form.Arrangement.Select(Function(p) p.ToString()))
                Assert.Contains("1 left out", form.lblArrangeSummary.Text)

                ' A PDF added later joins the end.
                Dim c = _f.MakePdf("c.pdf", "C", 1)
                Await form.AddPathsAsync({c})
                Assert.Equal("c.pdf p1", form.Arrangement.Last().ToString())

                form.txtOutputFolder.Text = results
                form.txtOutputName.Text = "arranged"
                Await form.RunOperationAsync()
                Assert.Equal(MainForm.StatusKind.Success, form.LastStatusKind)
                Assert.Equal({"B2", "A1", "A3", "B1", "C1"}, _f.PageLabels(Path.Combine(results, "arranged.pdf")))

                ' Start Over: back to every page in list order.
                form.btnArrangeReset.PerformClick()
                Assert.Null(form.Arrangement)
            End Function)
    End Sub

End Class
