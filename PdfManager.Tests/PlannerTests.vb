Imports System.IO
Imports Xunit

Public Class PageRangeTests

    <Theory>
    <InlineData("", "1-z")>
    <InlineData("All", "1-z")>
    <InlineData("1-10", "1-10")>
    <InlineData(" 1 - 3 , 5 ; 8-end ", "1-3,5,8-z")>
    <InlineData("5", "5")>
    <InlineData("last", "z")>
    <InlineData("10-1", "10-1")>
    <InlineData("ODD", "1-z:odd")>
    <InlineData("even", "1-z:even")>
    Public Sub Turns_what_people_type_into_qpdf_ranges(text As String, expected As String)
        Dim result = PageRange.Parse(text, 12)
        Assert.True(result.IsValid, result.ErrorMessage)
        Assert.Equal(expected, result.QpdfRange)
    End Sub

    <Theory>
    <InlineData("0", "start at 1")>
    <InlineData("13", "There is no page 13")>
    <InlineData("1-20", "There is no page 20")>
    <InlineData("abc", "isn't a page")>
    <InlineData("1,,x", "isn't a page")>
    <InlineData("99999999999", "too large")>
    Public Sub Catches_mistakes_with_a_plain_message(text As String, expected As String)
        Dim result = PageRange.Parse(text, 12)
        Assert.False(result.IsValid)
        Assert.Contains(expected, result.ErrorMessage)
    End Sub

    <Fact>
    Public Sub Even_pages_of_a_one_page_file_are_refused()
        Assert.False(PageRange.Parse("even", 1).IsValid)
    End Sub

End Class

Public Class PlannerTests
    Implements IDisposable

    Private ReadOnly _folder As String = Path.Combine(Path.GetTempPath(), "PdfManagerPlanner", Guid.NewGuid().ToString("N"))

    Public Sub New()
        Directory.CreateDirectory(_folder)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Directory.Delete(_folder, True)
    End Sub

    Private Function ReadyItem(name As String, pages As Integer) As PdfItem
        Dim filePath = Path.Combine(_folder, name)
        File.WriteAllText(filePath, "x")
        Return New PdfItem(filePath) With {.PageCount = pages, .State = PdfState.Ready}
    End Function

    Private Function Request(op As PdfOperation, ParamArray items As PdfItem()) As OperationRequest
        Return New OperationRequest With {.Operation = op, .Items = items, .OutputFolder = _folder, .OutputName = "result"}
    End Function

    <Fact>
    Public Sub Combine_needs_two_files()
        Dim issue = Planner.Validate(Request(PdfOperation.Combine, ReadyItem("a.pdf", 1)))
        Assert.Equal(RequestField.FileList, issue.Field)
    End Sub

    <Fact>
    Public Sub Merge_with_nothing_ticked_says_so()
        Dim issue = Planner.Validate(Request(PdfOperation.MergePages))
        Assert.Contains("Tick", issue.Message)
    End Sub

    <Fact>
    Public Sub Merge_points_at_the_file_with_an_impossible_page()
        Dim a = ReadyItem("a.pdf", 3)
        a.PagesText = "9"
        Dim issue = Planner.Validate(Request(PdfOperation.MergePages, a))
        Assert.Contains("a.pdf", issue.Message)
        Assert.Contains("no page 9", issue.Message)
    End Sub

    <Fact>
    Public Sub Refuses_to_overwrite_an_input()
        Dim a = ReadyItem("a.pdf", 1)
        Dim b = ReadyItem("b.pdf", 1)
        Dim r = Request(PdfOperation.Combine, a, b)
        r.OutputName = "A.PDF"
        Assert.Equal(RequestField.OutputName, Planner.Validate(r).Field)
    End Sub

    <Theory>
    <InlineData("", RequestField.OutputName)>
    <InlineData("bad:name", RequestField.OutputName)>
    <InlineData("CON", RequestField.OutputName)>
    Public Sub Checks_the_output_name(name As String, field As RequestField)
        Dim r = Request(PdfOperation.Combine, ReadyItem("a.pdf", 1), ReadyItem("b.pdf", 1))
        r.OutputName = name
        Assert.Equal(field, Planner.Validate(r).Field)
    End Sub

    <Fact>
    Public Sub Checks_the_output_folder()
        Dim r = Request(PdfOperation.Combine, ReadyItem("a.pdf", 1), ReadyItem("b.pdf", 1))
        r.OutputFolder = "relative\folder"
        Assert.Equal(RequestField.OutputFolder, Planner.Validate(r).Field)
    End Sub

    <Fact>
    Public Sub Encryption_password_rules()
        Dim r = Request(PdfOperation.Encrypt, ReadyItem("a.pdf", 1))
        Assert.Equal(RequestField.OpenPassword, Planner.Validate(r).Field)

        r.OpenPassword = "one"
        r.ConfirmPassword = "two"
        Assert.Equal(RequestField.ConfirmPassword, Planner.Validate(r).Field)

        r.ConfirmPassword = "one"
        Assert.Null(Planner.Validate(r))

        r.AllowPrinting = False
        Assert.Equal(RequestField.PermissionsPassword, Planner.Validate(r).Field)
        r.PermissionsPassword = "one"
        Assert.Equal(RequestField.PermissionsPassword, Planner.Validate(r).Field)
        r.PermissionsPassword = "boss"
        Assert.Null(Planner.Validate(r))
    End Sub

    <Fact>
    Public Sub Same_named_files_from_different_folders_get_different_results()
        Dim sub1 = Directory.CreateDirectory(Path.Combine(_folder, "one")).FullName
        Dim sub2 = Directory.CreateDirectory(Path.Combine(_folder, "two")).FullName
        File.WriteAllText(Path.Combine(sub1, "x.pdf"), "x")
        File.WriteAllText(Path.Combine(sub2, "x.pdf"), "x")
        Dim r = Request(PdfOperation.Encrypt,
                        New PdfItem(Path.Combine(sub1, "x.pdf")) With {.State = PdfState.Ready},
                        New PdfItem(Path.Combine(sub2, "x.pdf")) With {.State = PdfState.Ready})
        r.OutputName = "locked"
        Assert.Equal({"x-locked", "x-locked (2)"}, Planner.OutputTargets(r).Select(Function(t) t.BaseName))
    End Sub

    <Fact>
    Public Sub Passwords_never_appear_in_the_details()
        Dim shown = QpdfTool.DescribeCommand({"--password=hunter2", "--user-password=abc", "--owner-password=def", "C:\My Files\a.pdf"})
        Assert.DoesNotContain("hunter2", shown)
        Assert.DoesNotContain("abc", shown)
        Assert.DoesNotContain("def", shown)
        Assert.Contains("""C:\My Files\a.pdf""", shown)
    End Sub

    <Fact>
    Public Sub Finds_files_that_would_be_replaced()
        Dim a = ReadyItem("a.pdf", 1)
        Dim b = ReadyItem("b.pdf", 1)
        File.WriteAllText(Path.Combine(_folder, "result.pdf"), "old")
        Assert.Equal({"result.pdf"}, Planner.FindConflicts(Request(PdfOperation.Combine, a, b)))
    End Sub

End Class
