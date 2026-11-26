Imports System.IO
Imports System.Threading

Public Enum RunOutcome
    Succeeded
    Failed
    Cancelled
End Enum

Public Class OperationResult
    Public Property Outcome As RunOutcome
    Public Property HadWarnings As Boolean
    Public Property Message As String = ""
    Public Property OutputFiles As New List(Of String)
    Public Property InputBytes As Long?
    Public Property OutputBytes As Long
End Class

''' <summary>
''' Carries out a plan. Every result is written to a private working folder first and only moved
''' to the output folder once all of it has worked, so a failure can't damage an existing file.
''' </summary>
Public Class OperationRunner

    Private ReadOnly _tool As QpdfTool
    Private ReadOnly _log As Action(Of String)
    Private ReadOnly _progress As Action(Of Integer, Integer)
    Private ReadOnly _maxParallel As Integer
    Private ReadOnly _logLock As New Object()

    ''' <param name="progress">Told (finished, total) as each QPDF run ends, from any thread.</param>
    ''' <param name="maxParallel">How many QPDF runs may go at once; 0 picks from the number of processor cores.</param>
    Public Sub New(tool As QpdfTool, log As Action(Of String), Optional progress As Action(Of Integer, Integer) = Nothing, Optional maxParallel As Integer = 0)
        _tool = tool
        _log = log
        _progress = progress
        _maxParallel = If(maxParallel > 0, maxParallel, DefaultParallelism())
    End Sub

    ''' <summary>
    ''' Each QPDF run uses one processor core and spends part of its time waiting for the disk,
    ''' so a few at once finish a batch much sooner. Kept below the core count so the PC stays responsive.
    ''' </summary>
    Public Shared Function DefaultParallelism() As Integer
        Return Math.Clamp(Environment.ProcessorCount - 1, 1, 8)
    End Function

    Public Async Function RunAsync(plan As OperationPlan, token As CancellationToken) As Task(Of OperationResult)
        Dim workFolder = QpdfTool.NewWorkFolder()
        ' Stops the other runs as soon as one fails; the person's Cancel still comes through token.
        Using stopOthers = CancellationTokenSource.CreateLinkedTokenSource(token),
              gate As New SemaphoreSlim(_maxParallel)
            Try
                Dim jobCount = plan.Jobs.Count
                Dim outcomes(jobCount - 1) As QpdfResult
                Dim firstFailure As String = Nothing
                Dim done = 0

                ' Pass 1: every QPDF run, several at once. Each writes into its own folder.
                Dim runs = Enumerable.Range(0, jobCount).Select(
                    Async Function(i) As Task
                        Await gate.WaitAsync(stopOthers.Token).ConfigureAwait(False)
                        Try
                            Dim job = plan.Jobs(i)
                            Dim jobFolder = Path.Combine(workFolder, i.ToString())
                            Directory.CreateDirectory(jobFolder)

                            ' Working names are plain on purpose; the real names are applied when the results are moved.
                            Dim workOutput = Path.Combine(jobFolder, If(job.Target.IsSplit, "out-%d.pdf", "out.pdf"))
                            Dim args = job.Arguments.Select(Function(a) If(a = Planner.OutputToken, workOutput, a)).ToList()

                            Dim result = Await _tool.RunAsync(args, stopOthers.Token).ConfigureAwait(False)
                            outcomes(i) = result

                            ' One block per run, so lines from runs going at the same time don't interleave.
                            Dim lines As New List(Of String) From {job.Description & ":", QpdfTool.DescribeCommand(args)}
                            If result.Output.Length > 0 Then lines.Add(result.Output)
                            Log(String.Join(Environment.NewLine, lines))

                            If Not result.Succeeded Then
                                SyncLock _logLock
                                    If firstFailure Is Nothing Then firstFailure = $"{job.Description} failed: {result.FirstLine}"
                                End SyncLock
                                stopOthers.Cancel()
                                Return
                            End If
                            Report(Interlocked.Increment(done), jobCount)
                        Finally
                            gate.Release()
                        End Try
                    End Function).ToList()

                Try
                    Await Task.WhenAll(runs).ConfigureAwait(False)
                Catch ex As OperationCanceledException When firstFailure IsNot Nothing AndAlso Not token.IsCancellationRequested
                    ' The others were stopped because one failed; that failure is reported below.
                End Try
                token.ThrowIfCancellationRequested()
                If firstFailure IsNot Nothing Then
                    Return New OperationResult With {.Outcome = RunOutcome.Failed, .Message = firstFailure}
                End If

                ' Results in list order; a split run's pieces in page order (QPDF zero-pads their numbers).
                Dim hadWarnings = outcomes.Any(Function(r) r.HasWarnings)
                Dim finished As New List(Of (Source As String, Destination As String))
                Dim readPasswords As New List(Of String)
                For i = 0 To jobCount - 1
                    Dim job = plan.Jobs(i)
                    Dim jobFolder = Path.Combine(workFolder, i.ToString())
                    For Each produced In Directory.GetFiles(jobFolder, "*.pdf").OrderBy(Function(f) f, StringComparer.Ordinal)
                        finished.Add((produced, Path.Combine(plan.OutputFolder, FinalName(job.Target, produced))))
                        readPasswords.Add(If(job.ReadPassword IsNot Nothing, job.ReadPassword, plan.ResultPassword))
                    Next
                Next

                If finished.Count = 0 Then
                    Return New OperationResult With {
                        .Outcome = RunOutcome.Failed,
                        .Message = "QPDF finished but didn't produce a file. See the details below."
                    }
                End If

                ' Pass 2: read every result back before anything reaches the output folder, also several at once.
                Dim outputBytes = finished.Sum(Function(f) New FileInfo(f.Source).Length)
                Dim checks = Await Task.WhenAll(Enumerable.Range(0, finished.Count).Select(
                    Async Function(i) As Task(Of Integer?)
                        Await gate.WaitAsync(token).ConfigureAwait(False)
                        Try
                            Return Await PageCountAsync(finished(i).Source, readPasswords(i), token).ConfigureAwait(False)
                        Finally
                            gate.Release()
                        End Try
                    End Function)).ConfigureAwait(False)
                If checks.Any(Function(c) Not c.HasValue) Then
                    Return New OperationResult With {
                        .Outcome = RunOutcome.Failed,
                        .Message = "The new PDF didn't read back correctly, so it wasn't saved. See the details below."
                    }
                End If
                Dim totalPages = checks.Sum(Function(c) c.Value)

                Dim saved = MoveResults(finished, plan.OutputFolder)
                Return New OperationResult With {
                    .Outcome = RunOutcome.Succeeded,
                    .HadWarnings = hadWarnings,
                    .OutputFiles = saved,
                    .Message = DescribeSuccess(saved, totalPages, hadWarnings) & DescribeSaving(plan.InputBytes, outputBytes),
                    .InputBytes = plan.InputBytes,
                    .OutputBytes = outputBytes
                }

            Catch ex As OperationCanceledException
                Log("Cancelled. Nothing was saved.")
                Return New OperationResult With {.Outcome = RunOutcome.Cancelled, .Message = "Cancelled. Nothing was saved."}

            Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
                Log(ex.Message)
                Return New OperationResult With {
                    .Outcome = RunOutcome.Failed,
                    .Message = "Couldn't save the result. Check the folder is writable and the file isn't open in another program."
                }

            Finally
                Try
                    Directory.Delete(workFolder, True)
                Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
                    ' QpdfTool.CleanUpLeftovers removes it on a later start.
                End Try
            End Try
        End Using
    End Function

    Private Sub Report(finished As Integer, total As Integer)
        _progress?.Invoke(finished, total)
    End Sub

    ''' <summary>The number of pages in a PDF, or Nothing when QPDF can't read it.</summary>
    Private Async Function PageCountAsync(filePath As String, password As String, token As CancellationToken) As Task(Of Integer?)
        Dim args As New List(Of String)
        If Not String.IsNullOrEmpty(password) Then args.Add("--password=" & password)
        args.Add("--show-npages")
        args.Add(filePath)

        Dim result = Await _tool.RunAsync(args, token).ConfigureAwait(False)
        If Not result.Succeeded Then
            Log(result.Output)
            Return Nothing
        End If

        Dim count As Integer
        Dim line = result.Output.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()
        If line IsNot Nothing AndAlso Integer.TryParse(line.Trim(), count) Then Return count
        Return Nothing
    End Function

    Private Shared Function FinalName(target As OutputTarget, produced As String) As String
        If Not target.IsSplit Then Return target.BaseName & ".pdf"
        ' out-03.pdf becomes myname-03.pdf
        Return target.BaseName & Path.GetFileName(produced).Substring("out".Length)
    End Function

    Private Shared Function MoveResults(results As List(Of (Source As String, Destination As String)), folder As String) As List(Of String)
        Directory.CreateDirectory(folder)
        Dim saved As New List(Of String)
        For Each item In results
            File.Move(item.Source, item.Destination, True)
            saved.Add(item.Destination)
        Next
        Return saved
    End Function

    Private Shared Function DescribeSuccess(saved As List(Of String), totalPages As Integer, hadWarnings As Boolean) As String
        Dim pages = If(totalPages = 1, "1 page", $"{totalPages} pages")
        Dim message = If(saved.Count = 1,
                         $"Created {Path.GetFileName(saved(0))} ({pages}).",
                         $"Created {saved.Count} files ({pages} in total).")
        If hadWarnings Then message &= " QPDF noted some minor issues; see the details."
        Return message
    End Function

    ''' <summary>For Reduce Size: how much smaller the result is.</summary>
    Private Shared Function DescribeSaving(inputBytes As Long?, outputBytes As Long) As String
        If Not inputBytes.HasValue OrElse inputBytes.Value <= 0 Then Return ""
        If outputBytes >= inputBytes.Value Then
            Return $" It's already as small as this level can make it ({SizeText(outputBytes)}); try a stronger level."
        End If
        Dim percent = CInt(Math.Round(100.0 * (inputBytes.Value - outputBytes) / inputBytes.Value))
        Return $" {SizeText(inputBytes.Value)} → {SizeText(outputBytes)}, {percent}% smaller."
    End Function

    Friend Shared Function SizeText(bytes As Long) As String
        If bytes < 1024 Then Return $"{bytes} B"
        If bytes < 1024 * 1024 Then Return $"{bytes / 1024.0:0} KB"
        Return $"{bytes / (1024.0 * 1024):0.0} MB"
    End Function

    Private Sub Log(text As String)
        If _log Is Nothing Then Return
        SyncLock _logLock
            _log(text)
        End SyncLock
    End Sub

End Class
