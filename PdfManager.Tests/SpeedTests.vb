Imports System.Diagnostics
Imports System.IO
Imports Xunit
Imports Xunit.Abstractions

''' <summary>
''' Big batches: checks they still come out complete and in order, and prints how long they took
''' (run with --logger "console;verbosity=detailed" to see the times).
''' </summary>
Public Class SpeedTests
    Implements IDisposable

    Private ReadOnly _f As New Fixtures()
    Private ReadOnly _output As ITestOutputHelper

    Public Sub New(output As ITestOutputHelper)
        _output = output
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _f.Dispose()
    End Sub

    Private Function Out() As String
        Dim folder = Path.Combine(_f.Folder, "out")
        Directory.CreateDirectory(folder)
        Return folder
    End Function

    ''' <summary>Runs a request with the given number of QPDF runs at once and prints how long it took.</summary>
    Private Function Timed(label As String, request As OperationRequest, maxParallel As Integer, Optional progress As Action(Of Integer, Integer) = Nothing) As OperationResult
        Dim runner As New OperationRunner(_f.Tool, Nothing, progress, maxParallel)
        Dim watch = Stopwatch.StartNew()
        Dim result = runner.RunAsync(Planner.Plan(request), Threading.CancellationToken.None).GetAwaiter().GetResult()
        watch.Stop()
        _output.WriteLine($"{label}, {If(maxParallel = 1, "one at a time", $"{maxParallel} at once")}: {watch.ElapsedMilliseconds} ms")
        Return result
    End Function

    <Fact>
    Public Sub Encrypting_many_files_keeps_every_file_in_list_order()
        Dim items = Enumerable.Range(1, 16).Select(Function(i) _f.Item(_f.MakePdf($"f{i:D2}.pdf", "F", 3))).ToArray()
        Dim request = Function(folder As String) New OperationRequest With {
            .Operation = PdfOperation.Encrypt, .Items = items,
            .OpenPassword = "pw", .ConfirmPassword = "pw",
            .OutputFolder = folder, .OutputName = "locked"}

        ' The old way, for comparison.
        Dim serial = Timed("Encrypt 16 files", request(Path.Combine(_f.Folder, "serial")), 1)
        Assert.Equal(RunOutcome.Succeeded, serial.Outcome)

        Dim progress As New List(Of Integer)
        Dim result = Timed("Encrypt 16 files", request(Out()), OperationRunner.DefaultParallelism(),
                           Sub(done, total)
                               SyncLock progress
                                   progress.Add(done)
                               End SyncLock
                           End Sub)

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.Equal(Enumerable.Range(1, 16).Select(Function(i) $"f{i:D2}-locked.pdf"),
                     result.OutputFiles.Select(AddressOf Path.GetFileName))
        Assert.Equal("Created 16 files (48 pages in total).", result.Message)
        Assert.Equal(16, progress.Max())
    End Sub

    <Fact>
    Public Sub Splitting_a_long_pdf_into_single_pages_keeps_page_order()
        Dim a = _f.Item(_f.MakePdf("long.pdf", "L", 60))
        Dim request = Function(folder As String) New OperationRequest With {
            .Operation = PdfOperation.ExtractPages, .Items = {a}, .ExtractPages = "all",
            .ExtractOutput = ExtractOutput.EachPage, .OutputFolder = folder, .OutputName = "p"}

        Assert.Equal(RunOutcome.Succeeded, Timed("Split 60 pages", request(Path.Combine(_f.Folder, "serial")), 1).Outcome)
        Dim result = Timed("Split 60 pages", request(Out()), OperationRunner.DefaultParallelism())

        Assert.Equal(RunOutcome.Succeeded, result.Outcome)
        Assert.Equal(60, result.OutputFiles.Count)
        Assert.Equal("Created 60 files (60 pages in total).", result.Message)
        Assert.Equal({"L37"}, _f.PageLabels(result.OutputFiles(36)))
    End Sub

    <Fact>
    Public Sub A_failing_file_stops_the_batch_and_saves_nothing()
        Dim items As New List(Of PdfItem)
        For i = 1 To 6
            items.Add(_f.Item(_f.MakePdf($"g{i}.pdf", "G", 2)))
        Next
        ' Break one file after it was checked, so QPDF fails on it mid-batch.
        File.WriteAllText(items(3).Path, "not a pdf")
        Dim result = _f.Run(New OperationRequest With {
            .Operation = PdfOperation.Encrypt, .Items = items.ToArray(),
            .OpenPassword = "pw", .ConfirmPassword = "pw",
            .OutputFolder = Out(), .OutputName = "x"})

        Assert.Equal(RunOutcome.Failed, result.Outcome)
        Assert.Contains("g4.pdf", result.Message)
        Assert.Empty(Directory.GetFiles(Out()))
    End Sub

End Class
