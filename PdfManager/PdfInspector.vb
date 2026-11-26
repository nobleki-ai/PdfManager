Imports System.Threading

''' <summary>Asks QPDF about a PDF: how many pages it has and whether it needs a password.</summary>
Public Module PdfInspector

    ''' <summary>Fills in the item's state and page count, using its password if it has one.</summary>
    Public Async Function InspectAsync(tool As QpdfTool, item As PdfItem, Optional token As CancellationToken = Nothing) As Task
        Dim args As New List(Of String)
        If Not String.IsNullOrEmpty(item.Password) Then args.Add("--password=" & item.Password)
        args.Add("--show-npages")
        args.Add(item.Path)

        Dim result = Await tool.RunAsync(args, token)

        If result.Succeeded Then
            Dim count As Integer
            Dim line = result.Output.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()
            If line IsNot Nothing AndAlso Integer.TryParse(line.Trim(), count) Then
                item.PageCount = count
                item.Problem = Nothing
                item.State = PdfState.Ready
                Return
            End If
        End If

        If result.Output.Contains("invalid password", StringComparison.OrdinalIgnoreCase) Then
            item.IsEncrypted = True
            item.Password = Nothing
            item.Problem = "This file needs a password. Double-click it to type the password."
            item.State = PdfState.PasswordNeeded
        Else
            item.Problem = If(result.FirstLine.Length > 0, result.FirstLine, "QPDF couldn't read this file.")
            item.State = PdfState.Unreadable
        End If
    End Function

    ''' <summary>
    ''' Finds out whether the file is locked or limited in any way: QPDF's --is-encrypted ends with
    ''' 0 when it is and 2 when it isn't, and needs no password to tell.
    ''' </summary>
    Public Async Function CheckEncryptionAsync(tool As QpdfTool, item As PdfItem, Optional token As CancellationToken = Nothing) As Task
        If item.IsEncrypted.HasValue Then Return
        Dim result = Await tool.RunAsync({"--is-encrypted", item.Path}, token)
        If result.ExitCode = 0 Then
            item.IsEncrypted = True
        ElseIf result.ExitCode = 2 Then
            item.IsEncrypted = False
        End If
    End Function

End Module
