Imports System.Text.RegularExpressions

''' <summary>The outcome of reading a page list such as "1-3, 5, 8-end".</summary>
Public Class PageRangeResult
    Public Property IsValid As Boolean
    ''' <summary>The same pages in the form QPDF expects, such as "1-3,5,8-z".</summary>
    Public Property QpdfRange As String
    Public Property ErrorMessage As String
End Class

''' <summary>Turns the page lists people type into QPDF page ranges, and catches mistakes first.</summary>
Public Module PageRange

    Private ReadOnly TokenPattern As New Regex(
        "^\s*(\d+|end|last|z)\s*(?:-\s*(\d+|end|last|z)\s*)?$",
        RegexOptions.IgnoreCase Or RegexOptions.CultureInvariant)

    Public Function IsAll(text As String) As Boolean
        Return String.IsNullOrWhiteSpace(text) OrElse text.Trim().Equals("all", StringComparison.OrdinalIgnoreCase)
    End Function

    ''' <param name="text">What was typed. Blank or "All" means every page.</param>
    ''' <param name="pageCount">How many pages the file has, when known, so impossible pages are caught here.</param>
    Public Function Parse(text As String, Optional pageCount As Integer? = Nothing) As PageRangeResult
        If IsAll(text) Then Return Valid("1-z")

        Dim trimmed = text.Trim()
        If trimmed.Equals("odd", StringComparison.OrdinalIgnoreCase) Then Return Valid("1-z:odd")
        If trimmed.Equals("even", StringComparison.OrdinalIgnoreCase) Then
            If pageCount.HasValue AndAlso pageCount.Value < 2 Then
                Return Invalid("That file has no even pages.")
            End If
            Return Valid("1-z:even")
        End If

        Dim parts As New List(Of String)
        For Each token In trimmed.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries)
            If String.IsNullOrWhiteSpace(token) Then Continue For

            Dim match = TokenPattern.Match(token)
            If Not match.Success Then
                Return Invalid($"""{token.Trim()}"" isn't a page. Try something like 1-3, 5, 8-end, odd or even.")
            End If

            Dim first As String = Nothing
            Dim problem = ReadPage(match.Groups(1).Value, pageCount, first)
            If problem IsNot Nothing Then Return Invalid(problem)

            If match.Groups(2).Success Then
                Dim last As String = Nothing
                problem = ReadPage(match.Groups(2).Value, pageCount, last)
                If problem IsNot Nothing Then Return Invalid(problem)
                parts.Add($"{first}-{last}")
            Else
                parts.Add(first)
            End If
        Next

        If parts.Count = 0 Then Return Invalid("Type the pages you want, for example 1-3, 5, 8-end.")
        Return Valid(String.Join(",", parts))
    End Function

    ''' <summary>
    ''' The page numbers a page list picks, in order, for a PDF with the given number of pages,
    ''' worked out the same way QPDF does. Nothing when the list isn't valid.
    ''' </summary>
    Public Function Expand(text As String, pageCount As Integer) As List(Of Integer)
        Dim parsed = Parse(text, pageCount)
        If Not parsed.IsValid OrElse pageCount < 1 Then Return Nothing

        Dim range = parsed.QpdfRange
        Dim filter As String = Nothing
        Dim colon = range.IndexOf(":"c)
        If colon >= 0 Then
            filter = range.Substring(colon + 1)
            range = range.Substring(0, colon)
        End If

        Dim pages As New List(Of Integer)
        For Each token In range.Split(","c)
            Dim ends = token.Split("-"c)
            Dim first = If(ends(0) = "z", pageCount, Integer.Parse(ends(0)))
            Dim last = If(ends.Length = 1, first, If(ends(1) = "z", pageCount, Integer.Parse(ends(1))))
            Dim direction = If(last >= first, 1, -1)
            For page = first To last Step direction
                pages.Add(page)
            Next
        Next

        ' odd and even count positions in the list: the 1st, 3rd, 5th… or the 2nd, 4th, 6th…
        If filter = "odd" Then pages = pages.Where(Function(p, i) i Mod 2 = 0).ToList()
        If filter = "even" Then pages = pages.Where(Function(p, i) i Mod 2 = 1).ToList()
        Return pages
    End Function

    Private Function ReadPage(text As String, pageCount As Integer?, ByRef result As String) As String
        If text.Equals("end", StringComparison.OrdinalIgnoreCase) OrElse
           text.Equals("last", StringComparison.OrdinalIgnoreCase) OrElse
           text.Equals("z", StringComparison.OrdinalIgnoreCase) Then
            result = "z"
            Return Nothing
        End If

        Dim number As Integer
        If Not Integer.TryParse(text, number) Then Return $"Page {text} is too large."
        If number < 1 Then Return "Page numbers start at 1."
        If pageCount.HasValue AndAlso number > pageCount.Value Then
            Dim pages = If(pageCount.Value = 1, "1 page", $"{pageCount.Value} pages")
            Return $"There is no page {number}: that file has {pages}."
        End If

        result = number.ToString()
        Return Nothing
    End Function

    Private Function Valid(range As String) As PageRangeResult
        Return New PageRangeResult With {.IsValid = True, .QpdfRange = range}
    End Function

    Private Function Invalid(message As String) As PageRangeResult
        Return New PageRangeResult With {.IsValid = False, .ErrorMessage = message}
    End Function

End Module
