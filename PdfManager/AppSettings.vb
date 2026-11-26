Imports System.IO
Imports System.Text.Json

''' <summary>The few choices remembered between runs. Passwords are never stored here.</summary>
Public Class AppSettings

    Public Property QpdfPath As String
    Public Property LastInputFolder As String
    Public Property IncludeSubfolders As Boolean
    ''' <summary>"Light", "Dark" or "Auto" (same as Windows).</summary>
    Public Property Theme As String = "Auto"

    Private Shared ReadOnly Property FilePath As String
        Get
            Return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "PdfManager", "settings.json")
        End Get
    End Property

    Public Shared Function Load() As AppSettings
        Try
            If File.Exists(FilePath) Then
                Return If(JsonSerializer.Deserialize(Of AppSettings)(File.ReadAllText(FilePath)), New AppSettings())
            End If
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is JsonException
            ' A damaged settings file just means starting with the defaults.
        End Try
        Return New AppSettings()
    End Function

    Public Sub Save()
        Try
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath))
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Me))
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
            ' Not being able to remember settings shouldn't get in the way of closing.
        End Try
    End Sub

End Class
