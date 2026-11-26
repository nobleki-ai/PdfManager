Imports System.Windows.Forms

''' <summary>Asks for the password of a locked PDF.</summary>
Public Class PasswordDialog

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(fileName As String, wasWrong As Boolean)
        InitializeComponent()
        lblPrompt.Text = If(wasWrong,
                            $"That password didn't open {fileName}. Please try again:",
                            $"{fileName} is locked. Type its password:")
    End Sub

    Friend ReadOnly Property Password As String
        Get
            Return txtPassword.Text
        End Get
    End Property

    Private Sub chkShow_CheckedChanged(sender As Object, e As EventArgs) Handles chkShow.CheckedChanged
        txtPassword.UseSystemPasswordChar = Not chkShow.Checked
    End Sub

End Class

''' <summary>The questions the window asks the person. Tests answer them without showing anything.</summary>
Public Interface IUserPrompts
    ''' <returns>The password typed, or Nothing if the person cancelled.</returns>
    Function AskPassword(fileName As String, wasWrong As Boolean) As String
    Function ConfirmReplace(fileNames As IReadOnlyList(Of String)) As Boolean
End Interface

Friend Class DialogPrompts
    Implements IUserPrompts

    Private ReadOnly _owner As IWin32Window

    Public Sub New(owner As IWin32Window)
        _owner = owner
    End Sub

    Public Function AskPassword(fileName As String, wasWrong As Boolean) As String Implements IUserPrompts.AskPassword
        Using dialog As New PasswordDialog(fileName, wasWrong)
            Dim main = TryCast(_owner, MainForm)
            If main IsNot Nothing Then
                Theme.Apply(dialog, main.CurrentPalette)
                dialog.Icon = main.Icon
                AddHandler dialog.HandleCreated, Sub() Theme.UseDarkTitleBar(dialog, main.CurrentPalette.IsDark)
            End If
            Return If(dialog.ShowDialog(_owner) = DialogResult.OK, dialog.Password, Nothing)
        End Using
    End Function

    Public Function ConfirmReplace(fileNames As IReadOnlyList(Of String)) As Boolean Implements IUserPrompts.ConfirmReplace
        Dim shown = String.Join(vbLf, fileNames.Take(8))
        If fileNames.Count > 8 Then shown &= vbLf & $"…and {fileNames.Count - 8} more"
        Dim message = $"These files could be replaced:{vbLf}{vbLf}{shown}{vbLf}{vbLf}Replace them?"
        Return MessageBox.Show(_owner, message, "Replace existing files?",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = DialogResult.Yes
    End Function
End Class
