Imports System.Windows.Forms

''' <summary>Step 3's "Protect with a password": the password and limits every new PDF gets.</summary>
Public Class ProtectDialog

    Private _palette As Palette = Palette.LightPalette

    Public Sub New()
        Me.New(Nothing)
    End Sub

    ''' <param name="current">The settings to start from, or Nothing for empty boxes.</param>
    Public Sub New(current As ProtectionSettings)
        InitializeComponent()
        If current IsNot Nothing Then
            txtOpen.Text = current.OpenPassword
            txtConfirm.Text = current.ConfirmPassword
            txtPermissions.Text = current.PermissionsPassword
            chkPrint.Checked = current.AllowPrinting
            chkCopy.Checked = current.AllowCopying
            chkEdit.Checked = current.AllowEditing
        End If
    End Sub

    ''' <summary>What was typed.</summary>
    Public ReadOnly Property Settings As ProtectionSettings
        Get
            Return New ProtectionSettings With {
                .OpenPassword = txtOpen.Text, .ConfirmPassword = txtConfirm.Text, .PermissionsPassword = txtPermissions.Text,
                .AllowPrinting = chkPrint.Checked, .AllowCopying = chkCopy.Checked, .AllowEditing = chkEdit.Checked}
        End Get
    End Property

    Friend Sub ApplyPalette(p As Palette)
        _palette = p
        Theme.Apply(Me, p)
        lblProblem.ForeColor = p.ErrorText
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        Theme.UseDarkTitleBar(Me, _palette.IsDark)
    End Sub

    Private Sub chkShow_CheckedChanged(sender As Object, e As EventArgs) Handles chkShow.CheckedChanged
        For Each box In {txtOpen, txtConfirm, txtPermissions}
            box.UseSystemPasswordChar = Not chkShow.Checked
        Next
    End Sub

    ''' <summary>Only closes with settings that will work.</summary>
    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Dim issue = Planner.ValidateProtection(Settings)
        If issue IsNot Nothing Then
            lblProblem.Text = issue.Message
            Select Case issue.Field
                Case RequestField.ConfirmPassword : txtConfirm.Focus()
                Case RequestField.PermissionsPassword : txtPermissions.Focus()
                Case Else : txtOpen.Focus()
            End Select
            Return
        End If
        DialogResult = DialogResult.OK
    End Sub

End Class
