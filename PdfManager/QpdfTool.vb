Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading

Public Class QpdfResult
    Public Property ExitCode As Integer
    Public Property Output As String = ""

    ''' <summary>QPDF exits with 0 when all is well, 3 when it finished with warnings and 2 on errors.</summary>
    Public ReadOnly Property Succeeded As Boolean
        Get
            Return ExitCode = 0 OrElse ExitCode = 3
        End Get
    End Property

    Public ReadOnly Property HasWarnings As Boolean
        Get
            Return ExitCode = 3
        End Get
    End Property

    ''' <summary>The most useful line of what QPDF printed, for showing to a person.</summary>
    Public ReadOnly Property FirstLine As String
        Get
            Dim line = Output.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
            Return If(line, "").Trim()
        End Get
    End Property
End Class

''' <summary>Finds and runs the QPDF command-line program.</summary>
Public Class QpdfTool

    Private Shared ReadOnly WorkRoot As String = Path.Combine(Path.GetTempPath(), "PdfManager")
    Private Shared ReadOnly SecretPattern As New Regex("^(--(?:password|user-password|owner-password)=).+$", RegexOptions.CultureInvariant)
    Private Shared ReadOnly NoBomUtf8 As New UTF8Encoding(False)

    Public Sub New(exePath As String)
        Me.ExePath = exePath
    End Sub

    Public ReadOnly Property ExePath As String

    ''' <summary>Looks for QPDF: the copy chosen earlier, the copy beside this program, then the usual places.</summary>
    Public Shared Function Locate(savedPath As String) As String
        If Not String.IsNullOrWhiteSpace(savedPath) AndAlso File.Exists(savedPath) Then Return savedPath

        Dim bundled = Path.Combine(AppContext.BaseDirectory, "qpdf", "bin", "qpdf.exe")
        If File.Exists(bundled) Then Return bundled

        Dim searchPath = If(Environment.GetEnvironmentVariable("PATH"), "")
        For Each folder In searchPath.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            Try
                Dim candidate = Path.Combine(folder.Trim().Trim(""""c), "qpdf.exe")
                If File.Exists(candidate) Then Return candidate
            Catch ex As ArgumentException
                ' A malformed PATH entry. Keep looking.
            End Try
        Next

        For Each root In {Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                          Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}
            If String.IsNullOrEmpty(root) OrElse Not Directory.Exists(root) Then Continue For
            For Each folder In Directory.GetDirectories(root, "qpdf*").OrderByDescending(Function(d) d)
                Dim candidate = Path.Combine(folder, "bin", "qpdf.exe")
                If File.Exists(candidate) Then Return candidate
            Next
        Next

        Return Nothing
    End Function

    ''' <summary>Returns the version text, such as "12.4.2", or Nothing when this isn't a working QPDF.</summary>
    Public Async Function GetVersionAsync() As Task(Of String)
        Try
            Dim result = Await RunDirectAsync({"--version"}, CancellationToken.None).ConfigureAwait(False)
            If result.ExitCode <> 0 Then Return Nothing
            Dim match = Regex.Match(result.Output, "qpdf version (\S+)", RegexOptions.IgnoreCase)
            Return If(match.Success, match.Groups(1).Value, Nothing)
        Catch ex As Exception When TypeOf ex Is ComponentModel.Win32Exception OrElse TypeOf ex Is IOException
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Runs QPDF with its instructions in a short-lived file, not on the command line. That
    ''' keeps passwords out of the process list and removes the limit on how many files fit.
    ''' </summary>
    Public Async Function RunAsync(args As IEnumerable(Of String), token As CancellationToken) As Task(Of QpdfResult)
        Directory.CreateDirectory(WorkRoot)
        Dim argFile = Path.Combine(WorkRoot, Guid.NewGuid().ToString("N") & ".args")
        Try
            ' One argument per line, in UTF-8, so non-English names and passwords survive.
            Await File.WriteAllTextAsync(argFile, String.Join(vbLf, args) & vbLf, NoBomUtf8, token).ConfigureAwait(False)
            Return Await RunDirectAsync({"@" & argFile}, token).ConfigureAwait(False)
        Finally
            TryDeleteFile(argFile)
        End Try
    End Function

    Private Async Function RunDirectAsync(args As IEnumerable(Of String), token As CancellationToken) As Task(Of QpdfResult)
        Dim info As New ProcessStartInfo(ExePath) With {
            .UseShellExecute = False,
            .CreateNoWindow = True,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .StandardOutputEncoding = Encoding.UTF8,
            .StandardErrorEncoding = Encoding.UTF8
        }
        For Each arg In args
            info.ArgumentList.Add(arg)
        Next

        Using process As New Process With {.StartInfo = info}
            process.Start()
            Dim outputTask = process.StandardOutput.ReadToEndAsync()
            Dim errorTask = process.StandardError.ReadToEndAsync()
            Try
                Await process.WaitForExitAsync(token).ConfigureAwait(False)
            Catch ex As OperationCanceledException
                Try
                    process.Kill(True)
                    process.WaitForExit()
                Catch killError As Exception When TypeOf killError Is InvalidOperationException OrElse TypeOf killError Is ComponentModel.Win32Exception
                    ' It finished on its own in the meantime.
                End Try
                Throw
            End Try

            Dim stdout = Await outputTask.ConfigureAwait(False)
            Dim stderr = Await errorTask.ConfigureAwait(False)
            Return New QpdfResult With {
                .ExitCode = process.ExitCode,
                .Output = (stderr & vbLf & stdout).Trim()
            }
        End Using
    End Function

    ''' <summary>
    ''' An unlocked, tidied copy of a PDF, kept in memory only (QPDF writes it to its output, never
    ''' to disk). Used to show pages of locked PDFs. Nothing when QPDF can't read the file.
    ''' </summary>
    Public Async Function ReadUnlockedAsync(pdfPath As String, password As String, token As CancellationToken) As Task(Of Byte())
        Directory.CreateDirectory(WorkRoot)
        Dim argFile = Path.Combine(WorkRoot, Guid.NewGuid().ToString("N") & ".args")
        Dim args As New List(Of String)
        If Not String.IsNullOrEmpty(password) Then args.Add("--password=" & password)
        args.AddRange({"--decrypt", pdfPath, "-"})
        Try
            Await File.WriteAllTextAsync(argFile, String.Join(vbLf, args) & vbLf, NoBomUtf8, token).ConfigureAwait(False)
            Dim info As New ProcessStartInfo(ExePath) With {
                .UseShellExecute = False,
                .CreateNoWindow = True,
                .RedirectStandardOutput = True,
                .RedirectStandardError = True
            }
            info.ArgumentList.Add("@" & argFile)
            Using process As New Process With {.StartInfo = info}, output As New MemoryStream()
                process.Start()
                Dim copyTask = process.StandardOutput.BaseStream.CopyToAsync(output, token)
                Dim errorTask = process.StandardError.ReadToEndAsync()
                Try
                    Await process.WaitForExitAsync(token).ConfigureAwait(False)
                    Await copyTask.ConfigureAwait(False)
                    Await errorTask.ConfigureAwait(False)
                Catch ex As OperationCanceledException
                    Try
                        process.Kill(True)
                    Catch killError As Exception When TypeOf killError Is InvalidOperationException OrElse TypeOf killError Is ComponentModel.Win32Exception
                    End Try
                    Throw
                End Try
                If process.ExitCode <> 0 AndAlso process.ExitCode <> 3 Then Return Nothing
                Return output.ToArray()
            End Using
        Finally
            TryDeleteFile(argFile)
        End Try
    End Function

    ''' <summary>A readable form of the command for the details panel, with passwords hidden.</summary>
    Public Shared Function DescribeCommand(args As IEnumerable(Of String)) As String
        Dim shown = args.Select(Function(a)
                                    Dim masked = SecretPattern.Replace(a, "$1••••")
                                    Return If(masked.Contains(" "c), """" & masked & """", masked)
                                End Function)
        Return "qpdf " & String.Join(" ", shown)
    End Function

    ''' <summary>Removes anything an earlier run couldn't clean up, such as after a power cut.</summary>
    Public Shared Sub CleanUpLeftovers()
        Try
            If Not Directory.Exists(WorkRoot) Then Return
            Dim cutoff = DateTime.UtcNow.AddDays(-1)
            For Each entry In New DirectoryInfo(WorkRoot).GetFileSystemInfos()
                If entry.LastWriteTimeUtc >= cutoff Then Continue For
                If TypeOf entry Is DirectoryInfo Then
                    DirectCast(entry, DirectoryInfo).Delete(True)
                Else
                    entry.Delete()
                End If
            Next
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
            ' Nothing depends on this. It will be tried again next time.
        End Try
    End Sub

    Friend Shared Function NewWorkFolder() As String
        Dim folder = Path.Combine(WorkRoot, Guid.NewGuid().ToString("N"))
        Directory.CreateDirectory(folder)
        Return folder
    End Function

    Private Shared Sub TryDeleteFile(filePath As String)
        Try
            File.Delete(filePath)
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
            ' CleanUpLeftovers removes it on a later start.
        End Try
    End Sub

End Class
