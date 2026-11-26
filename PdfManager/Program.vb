Imports System.Windows.Forms

''' <summary>Where the program starts.</summary>
Public Module Program

    <STAThread>
    Public Function Main(args As String()) As Integer
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)

        Using form As New MainForm()
            form.ApplyStartup(StartupOptions.Parse(args))
            Application.Run(form)
        End Using

        Return Environment.ExitCode
    End Function

End Module

''' <summary>
''' What the program was started with: PDFs or folders to add, plus optionally
''' --tab=combine|merge|extract|encrypt and --screenshot file.png.
''' </summary>
Public Class StartupOptions

    Public Property ScreenshotPath As String
    Public Property TabName As String
    ''' <summary>light, dark or auto: a look for this run only, without changing the saved choice.</summary>
    Public Property ThemeName As String
    ''' <summary>--show-help[=topic] opens the Help window at start. Empty means the first topic.</summary>
    Public Property HelpTopic As String
    ''' <summary>--show-pages opens the page thumbnail view once the PDFs given are read.</summary>
    Public Property ShowPages As Boolean
    Public ReadOnly Property Paths As New List(Of String)

    Public Shared Function Parse(args As IEnumerable(Of String)) As StartupOptions
        Dim options As New StartupOptions()
        Dim list = args.ToList()
        Dim i = 0
        While i < list.Count
            Dim arg = list(i)
            If arg.Equals("--screenshot", StringComparison.OrdinalIgnoreCase) Then
                If i + 1 < list.Count Then
                    options.ScreenshotPath = list(i + 1)
                    i += 1
                End If
            ElseIf arg.StartsWith("--screenshot=", StringComparison.OrdinalIgnoreCase) Then
                options.ScreenshotPath = arg.Substring("--screenshot=".Length)
            ElseIf arg.Equals("--tab", StringComparison.OrdinalIgnoreCase) Then
                If i + 1 < list.Count Then
                    options.TabName = list(i + 1)
                    i += 1
                End If
            ElseIf arg.StartsWith("--tab=", StringComparison.OrdinalIgnoreCase) Then
                options.TabName = arg.Substring("--tab=".Length)
            ElseIf arg.Equals("--show-help", StringComparison.OrdinalIgnoreCase) Then
                options.HelpTopic = ""
            ElseIf arg.StartsWith("--show-help=", StringComparison.OrdinalIgnoreCase) Then
                options.HelpTopic = arg.Substring("--show-help=".Length)
            ElseIf arg.Equals("--show-pages", StringComparison.OrdinalIgnoreCase) Then
                options.ShowPages = True
            ElseIf arg.StartsWith("--theme=", StringComparison.OrdinalIgnoreCase) Then
                options.ThemeName = arg.Substring("--theme=".Length)
            ElseIf Not arg.StartsWith("--", StringComparison.Ordinal) Then
                options.Paths.Add(arg)
            End If
            i += 1
        End While
        Return options
    End Function

End Class
