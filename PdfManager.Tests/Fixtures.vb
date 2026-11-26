Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

''' <summary>Makes small PDFs whose pages say who they are ("A1", "A2"…) and reads those labels back.</summary>
Public Class Fixtures
    Implements IDisposable

    Public ReadOnly Property Folder As String = Path.Combine(Path.GetTempPath(), "PdfManagerTests", Guid.NewGuid().ToString("N"))
    Public ReadOnly Property Tool As QpdfTool

    Public Sub New()
        Directory.CreateDirectory(Folder)
        Dim exe = QpdfTool.Locate(Nothing)
        If exe Is Nothing Then Throw New InvalidOperationException("QPDF wasn't found next to the tests.")
        Tool = New QpdfTool(exe)
    End Sub

    ''' <summary>Creates a PDF with the given number of pages, labelled with the prefix and page number.</summary>
    Public Function MakePdf(fileName As String, label As String, pages As Integer, Optional password As String = Nothing) As String
        Dim objects As New List(Of String) From {
            "<< /Type /Catalog /Pages 2 0 R >>",
            $"<< /Type /Pages /Kids [{String.Join(" ", Enumerable.Range(1, pages).Select(Function(p) $"{2 + p * 2} 0 R"))}] /Count {pages} >>",
            "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"
        }
        For p = 1 To pages
            Dim text = $"BT /F1 48 Tf 72 700 Td ({label}{p}) Tj ET"
            objects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R >> >> /Contents {3 + p * 2} 0 R >>")
            objects.Add($"<< /Length {text.Length} >>" & vbLf & "stream" & vbLf & text & vbLf & "endstream")
        Next

        Dim sb As New StringBuilder("%PDF-1.4" & vbLf)
        Dim offsets As New List(Of Integer)
        For i = 0 To objects.Count - 1
            offsets.Add(sb.Length)
            sb.Append($"{i + 1} 0 obj{vbLf}{objects(i)}{vbLf}endobj{vbLf}")
        Next
        Dim xref = sb.Length
        sb.Append($"xref{vbLf}0 {objects.Count + 1}{vbCrLf}0000000000 65535 f{vbCrLf}")
        For Each offset In offsets
            sb.Append($"{offset:D10} 00000 n{vbCrLf}")
        Next
        sb.Append($"trailer{vbLf}<< /Size {objects.Count + 1} /Root 1 0 R >>{vbLf}startxref{vbLf}{xref}{vbLf}%%EOF{vbLf}")

        Dim target = Path.Combine(Folder, fileName)
        If password Is Nothing Then
            File.WriteAllText(target, sb.ToString(), Encoding.ASCII)
        Else
            Dim plain = Path.Combine(Folder, "plain-" & Guid.NewGuid().ToString("N") & ".pdf")
            File.WriteAllText(plain, sb.ToString(), Encoding.ASCII)
            Dim result = Tool.RunAsync({"--encrypt", "--user-password=" & password, "--owner-password=" & password & "-owner", "--bits=256", "--", plain, target}, CancellationToken.None).GetAwaiter().GetResult()
            If Not result.Succeeded Then Throw New InvalidOperationException(result.Output)
        End If
        Return target
    End Function

    ''' <summary>The labels on each page of a PDF, in page order, such as {"A1", "A2", "B5"}.</summary>
    Public Function PageLabels(pdf As String, Optional password As String = Nothing) As List(Of String)
        Dim json = Path.Combine(Folder, Guid.NewGuid().ToString("N") & ".txt")
        Dim labels As New List(Of String)
        Dim count = PageCount(pdf, password)
        For page = 1 To count
            Dim args As New List(Of String)
            If password IsNot Nothing Then args.Add("--password=" & password)
            args.AddRange({"--decrypt", "--qdf", "--object-streams=disable", "--empty", "--pages", pdf})
            If password IsNot Nothing Then args.Add("--password=" & password)
            args.AddRange({page.ToString(), "--", json})
            Dim result = Tool.RunAsync(args, CancellationToken.None).GetAwaiter().GetResult()
            If Not result.Succeeded Then Throw New InvalidOperationException(result.Output)
            Dim match = Regex.Match(File.ReadAllText(json, Encoding.Latin1), "\((\w+)\) Tj")
            labels.Add(If(match.Success, match.Groups(1).Value, "?"))
        Next
        Return labels
    End Function

    ''' <summary>One page of a PDF written out as plain readable text (QPDF's QDF form).</summary>
    Public Function PageText(pdf As String, page As Integer, Optional password As String = Nothing) As String
        Dim qdf = Path.Combine(Folder, Guid.NewGuid().ToString("N") & ".txt")
        Dim args As New List(Of String)
        If password IsNot Nothing Then args.Add("--password=" & password)
        args.AddRange({"--decrypt", "--qdf", "--object-streams=disable", "--empty", "--pages", pdf})
        If password IsNot Nothing Then args.Add("--password=" & password)
        args.AddRange({page.ToString(), "--", qdf})
        Dim result = Tool.RunAsync(args, CancellationToken.None).GetAwaiter().GetResult()
        If Not result.Succeeded Then Throw New InvalidOperationException(result.Output)
        Return File.ReadAllText(qdf, Encoding.Latin1)
    End Function

    ''' <summary>How far a page is turned: 0, 90, 180 or 270.</summary>
    Public Function PageRotation(pdf As String, page As Integer) As Integer
        Dim match = Regex.Match(PageText(pdf, page), "/Rotate (-?\d+)")
        If Not match.Success Then Return 0
        Return ((Integer.Parse(match.Groups(1).Value) Mod 360) + 360) Mod 360
    End Function

    ''' <summary>A one-page PDF holding a large, high-quality photo-like picture, for Reduce Size.</summary>
    Public Function MakePhotoPdf(fileName As String) As String
        Const w = 1200, h = 900
        Dim jpeg As Byte()
        Using picture As New Drawing.Bitmap(w, h)
            Using g = Drawing.Graphics.FromImage(picture),
                  brush As New Drawing.Drawing2D.LinearGradientBrush(New Drawing.Point(0, 0), New Drawing.Point(w, h), Drawing.Color.SteelBlue, Drawing.Color.Orange)
                g.FillRectangle(brush, 0, 0, w, h)
                Dim random As New Random(7)
                For i = 1 To 400
                    Using dot As New Drawing.SolidBrush(Drawing.Color.FromArgb(120, random.Next(256), random.Next(256), random.Next(256)))
                        g.FillEllipse(dot, random.Next(w), random.Next(h), random.Next(20, 200), random.Next(20, 200))
                    End Using
                Next
            End Using
            Dim encoder = Drawing.Imaging.ImageCodecInfo.GetImageEncoders().First(Function(c) c.MimeType = "image/jpeg")
            Using options As New Drawing.Imaging.EncoderParameters(1), stream As New MemoryStream()
                options.Param(0) = New Drawing.Imaging.EncoderParameter(Drawing.Imaging.Encoder.Quality, 95L)
                picture.Save(stream, encoder, options)
                jpeg = stream.ToArray()
            End Using
        End Using

        Dim content = "q 540 0 0 405 36 300 cm /Im1 Do Q"
        Dim objects = {
            "<< /Type /Catalog /Pages 2 0 R >>",
            "<< /Type /Pages /Kids [3 0 R] /Count 1 >>",
            "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /XObject << /Im1 5 0 R >> >> /Contents 4 0 R >>",
            $"<< /Length {content.Length} >>{vbLf}stream{vbLf}{content}{vbLf}endstream"
        }
        Dim latin = Encoding.Latin1
        Using output As New MemoryStream()
            Dim write = Sub(text As String)
                            Dim bytes = latin.GetBytes(text)
                            output.Write(bytes, 0, bytes.Length)
                        End Sub
            Dim offsets As New List(Of Long)
            write("%PDF-1.4" & vbLf)
            For i = 0 To objects.Length - 1
                offsets.Add(output.Length)
                write($"{i + 1} 0 obj{vbLf}{objects(i)}{vbLf}endobj{vbLf}")
            Next
            offsets.Add(output.Length)
            write($"5 0 obj{vbLf}<< /Type /XObject /Subtype /Image /Width {w} /Height {h} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {jpeg.Length} >>{vbLf}stream{vbLf}")
            output.Write(jpeg, 0, jpeg.Length)
            write($"{vbLf}endstream{vbLf}endobj{vbLf}")
            Dim xref = output.Length
            write($"xref{vbLf}0 6{vbCrLf}0000000000 65535 f{vbCrLf}")
            For Each offset In offsets
                write($"{offset:D10} 00000 n{vbCrLf}")
            Next
            write($"trailer{vbLf}<< /Size 6 /Root 1 0 R >>{vbLf}startxref{vbLf}{xref}{vbLf}%%EOF{vbLf}")
            Dim target = Path.Combine(Folder, fileName)
            File.WriteAllBytes(target, output.ToArray())
            Return target
        End Using
    End Function

    Public Function PageCount(pdf As String, Optional password As String = Nothing) As Integer
        Dim args As New List(Of String)
        If password IsNot Nothing Then args.Add("--password=" & password)
        args.AddRange({"--show-npages", pdf})
        Dim result = Tool.RunAsync(args, CancellationToken.None).GetAwaiter().GetResult()
        If Not result.Succeeded Then Throw New InvalidOperationException(result.Output)
        Return Integer.Parse(result.Output.Trim())
    End Function

    ''' <summary>True when the file can't be opened without a password.</summary>
    Public Function NeedsPassword(pdf As String) As Boolean
        Dim result = Tool.RunAsync({"--requires-password", pdf}, CancellationToken.None).GetAwaiter().GetResult()
        Return result.ExitCode = 0
    End Function

    ''' <summary>A list item ready to use, read by QPDF the way the app reads it.</summary>
    Public Function Item(pdf As String, Optional password As String = Nothing) As PdfItem
        Dim result As New PdfItem(pdf) With {.Password = password}
        PdfInspector.InspectAsync(Tool, result).GetAwaiter().GetResult()
        Return result
    End Function

    Public Function Run(request As OperationRequest) As OperationResult
        Dim issue = Planner.Validate(request)
        If issue IsNot Nothing Then Throw New InvalidOperationException(issue.Message)
        Dim runner As New OperationRunner(Tool, Nothing)
        Return runner.RunAsync(Planner.Plan(request), CancellationToken.None).GetAwaiter().GetResult()
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        Try
            Directory.Delete(Folder, True)
        Catch ex As IOException
        End Try
    End Sub

End Class
