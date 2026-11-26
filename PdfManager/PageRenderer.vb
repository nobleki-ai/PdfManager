Imports System.Collections.Concurrent
Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports Windows.Data.Pdf
Imports Windows.Storage.Streams

''' <summary>
''' Draws small pictures of PDF pages with Windows' own PDF renderer (part of Windows 10 and 11,
''' nothing to install). QPDF does the real work on the files; this is only for looking at them.
''' Pictures are kept for the rest of the session, so reopening the page view is instant.
''' </summary>
Public NotInheritable Class PageRenderer
    Implements IDisposable

    ''' <summary>How wide the pictures are drawn, in pixels; the page view shrinks them to fit.</summary>
    Public Const PictureWidth As Integer = 240

    Private Shared ReadOnly Pictures As New ConcurrentDictionary(Of String, Bitmap)(StringComparer.OrdinalIgnoreCase)

    ' Open PDFs, so each is read once while the page view is open. One page is drawn at a time.
    Private ReadOnly _documents As New Dictionary(Of String, (Stream As Stream, Document As PdfDocument))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _tool As QpdfTool
    Private ReadOnly _gate As New SemaphoreSlim(1)
    Private _disposed As Boolean

    ''' <summary>Why the last page couldn't be drawn, for the tests and for looking into problems.</summary>
    Friend Property LastProblem As String

    ''' <summary>A picture drawn earlier, or Nothing.</summary>
    Public Shared Function Cached(page As PageRef) As Bitmap
        Dim picture As Bitmap = Nothing
        Pictures.TryGetValue(KeyOf(page), picture)
        Return picture
    End Function

    ''' <summary>Draws a page, or returns Nothing when Windows can't show it (QPDF may still be able to use it).</summary>
    Public Async Function RenderAsync(page As PageRef, token As CancellationToken) As Task(Of Bitmap)
        Dim key = KeyOf(page)
        Dim picture As Bitmap = Nothing
        If Pictures.TryGetValue(key, picture) Then Return picture

        Await _gate.WaitAsync(token).ConfigureAwait(False)
        Try
            If _disposed Then Return Nothing
            If Pictures.TryGetValue(key, picture) Then Return picture
            Dim document = Await OpenAsync(page.Item, token).ConfigureAwait(False)
            If document Is Nothing OrElse page.PageNumber > document.PageCount Then Return Nothing

            Using pdfPage = document.GetPage(CUInt(page.PageNumber - 1)),
                  output As New InMemoryRandomAccessStream()
                Dim options As New PdfPageRenderOptions With {.DestinationWidth = CUInt(PictureWidth)}
                Await pdfPage.RenderToStreamAsync(output, options).AsTask(token).ConfigureAwait(False)
                Using png = output.AsStreamForRead(), decoded = Image.FromStream(png)
                    picture = New Bitmap(decoded)
                End Using
            End Using
            Pictures(key) = picture
            Return picture
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException OrElse
                                   TypeOf ex Is ArgumentException OrElse TypeOf ex Is Runtime.InteropServices.COMException OrElse
                                   TypeOf ex Is InvalidOperationException
            LastProblem = ex.GetType().Name & ": " & ex.Message
            Return Nothing
        Finally
            _gate.Release()
        End Try
    End Function

    ''' <summary>Opens a PDF for drawing, once. Nothing (remembered) when neither Windows nor QPDF can open it.</summary>
    Private Async Function OpenAsync(item As PdfItem, token As CancellationToken) As Task(Of PdfDocument)
        Dim open As (Stream As Stream, Document As PdfDocument) = Nothing
        If _documents.TryGetValue(item.Path, open) Then Return open.Document

        ' Unlocked PDFs are read straight from the file, shared so it stays free for QPDF and other programs.
        If String.IsNullOrEmpty(item.Password) Then
            Dim stream As New FileStream(item.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite Or FileShare.Delete)
            Try
                Dim document = Await PdfDocument.LoadFromStreamAsync(stream.AsRandomAccessStream()).AsTask(token).ConfigureAwait(False)
                _documents(item.Path) = (stream, document)
                Return document
            Catch ex As Exception When TypeOf ex Is Runtime.InteropServices.COMException OrElse TypeOf ex Is ArgumentException
                stream.Dispose()
                LastProblem = $"Windows couldn't open {item.FileName}: {ex.Message}"
            End Try
        End If

        ' Locked PDFs (Windows can't open the strongest locks), and any Windows can't read as they
        ' are: QPDF hands over an unlocked, tidied copy, held in memory only and never saved.
        If _tool IsNot Nothing Then
            Dim bytes = Await _tool.ReadUnlockedAsync(item.Path, item.Password, token).ConfigureAwait(False)
            If bytes IsNot Nothing AndAlso bytes.Length > 0 Then
                Dim memory As New MemoryStream(bytes, writable:=False)
                Try
                    Dim document = Await PdfDocument.LoadFromStreamAsync(memory.AsRandomAccessStream()).AsTask(token).ConfigureAwait(False)
                    _documents(item.Path) = (memory, document)
                    Return document
                Catch ex As Exception When TypeOf ex Is Runtime.InteropServices.COMException OrElse TypeOf ex Is ArgumentException
                    memory.Dispose()
                    LastProblem = $"Windows couldn't open QPDF's copy of {item.FileName}: {ex.Message}"
                End Try
            Else
                LastProblem = $"QPDF couldn't read {item.FileName}."
            End If
        End If

        _documents(item.Path) = (Nothing, Nothing)
        Return Nothing
    End Function

    ''' <summary>The same page of the same file, changed or not, has its own picture.</summary>
    Private Shared Function KeyOf(page As PageRef) As String
        Dim stamp = Stamps.GetOrAdd(page.Item.Path,
            Function(p)
                Try
                    Return File.GetLastWriteTimeUtc(p).Ticks
                Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
                    Return 0L
                End Try
            End Function)
        Return $"{page.Item.Path}|{stamp}|{page.PageNumber}"
    End Function

    ' When each file was last changed, looked up once per page view rather than on every repaint.
    Private Shared ReadOnly Stamps As New ConcurrentDictionary(Of String, Long)(StringComparer.OrdinalIgnoreCase)

    ''' <param name="tool">QPDF, to unlock locked PDFs for drawing; without it only unlocked PDFs that Windows can read are drawn.</param>
    Public Sub New(Optional tool As QpdfTool = Nothing)
        _tool = tool
        Stamps.Clear()
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _gate.Wait()
        Try
            _disposed = True
            For Each open In _documents.Values
                open.Stream?.Dispose()
            Next
            _documents.Clear()
        Finally
            _gate.Release()
        End Try
    End Sub

End Class
