<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ProtectDialog
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Private components As System.ComponentModel.IContainer

    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        tblLayout = New System.Windows.Forms.TableLayoutPanel()
        lblInfo = New System.Windows.Forms.Label()
        lblOpen = New System.Windows.Forms.Label()
        txtOpen = New System.Windows.Forms.TextBox()
        lblConfirm = New System.Windows.Forms.Label()
        txtConfirm = New System.Windows.Forms.TextBox()
        chkShow = New System.Windows.Forms.CheckBox()
        lblPermissions = New System.Windows.Forms.Label()
        txtPermissions = New System.Windows.Forms.TextBox()
        lblAllow = New System.Windows.Forms.Label()
        flowAllow = New System.Windows.Forms.FlowLayoutPanel()
        chkPrint = New System.Windows.Forms.CheckBox()
        chkCopy = New System.Windows.Forms.CheckBox()
        chkEdit = New System.Windows.Forms.CheckBox()
        lblProblem = New System.Windows.Forms.Label()
        flowButtons = New System.Windows.Forms.FlowLayoutPanel()
        btnCancel = New System.Windows.Forms.Button()
        btnOk = New System.Windows.Forms.Button()
        tblLayout.SuspendLayout()
        flowAllow.SuspendLayout()
        flowButtons.SuspendLayout()
        SuspendLayout()
        '
        ' tblLayout
        '
        tblLayout.ColumnCount = 2
        tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        tblLayout.Controls.Add(lblInfo, 0, 0)
        tblLayout.SetColumnSpan(lblInfo, 2)
        tblLayout.Controls.Add(lblOpen, 0, 1)
        tblLayout.Controls.Add(txtOpen, 1, 1)
        tblLayout.Controls.Add(lblConfirm, 0, 2)
        tblLayout.Controls.Add(txtConfirm, 1, 2)
        tblLayout.Controls.Add(chkShow, 1, 3)
        tblLayout.Controls.Add(lblPermissions, 0, 4)
        tblLayout.Controls.Add(txtPermissions, 1, 4)
        tblLayout.Controls.Add(lblAllow, 0, 5)
        tblLayout.Controls.Add(flowAllow, 1, 5)
        tblLayout.Controls.Add(lblProblem, 0, 6)
        tblLayout.SetColumnSpan(lblProblem, 2)
        tblLayout.Controls.Add(flowButtons, 0, 7)
        tblLayout.SetColumnSpan(flowButtons, 2)
        tblLayout.AutoSize = True
        tblLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        tblLayout.Location = New System.Drawing.Point(0, 0)
        tblLayout.Name = "tblLayout"
        tblLayout.Padding = New System.Windows.Forms.Padding(14)
        tblLayout.RowCount = 8
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblLayout.TabIndex = 0
        '
        ' lblInfo
        '
        lblInfo.AutoSize = True
        lblInfo.Margin = New System.Windows.Forms.Padding(3, 0, 3, 12)
        lblInfo.MaximumSize = New System.Drawing.Size(500, 0)
        lblInfo.Name = "lblInfo"
        lblInfo.Text = "Every new PDF this makes gets this password, using strong AES 256-bit encryption. A forgotten password can't be recovered."
        '
        ' lblOpen
        '
        lblOpen.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblOpen.AutoSize = True
        lblOpen.Name = "lblOpen"
        lblOpen.Text = "Password to &open:"
        '
        ' txtOpen
        '
        txtOpen.Dock = System.Windows.Forms.DockStyle.Fill
        txtOpen.Name = "txtOpen"
        txtOpen.TabIndex = 1
        txtOpen.UseSystemPasswordChar = True
        '
        ' lblConfirm
        '
        lblConfirm.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblConfirm.AutoSize = True
        lblConfirm.Name = "lblConfirm"
        lblConfirm.Text = "Type it again:"
        '
        ' txtConfirm
        '
        txtConfirm.Dock = System.Windows.Forms.DockStyle.Fill
        txtConfirm.Name = "txtConfirm"
        txtConfirm.TabIndex = 2
        txtConfirm.UseSystemPasswordChar = True
        '
        ' chkShow
        '
        chkShow.AutoSize = True
        chkShow.Name = "chkShow"
        chkShow.TabIndex = 3
        chkShow.Text = "Show passwords"
        '
        ' lblPermissions
        '
        lblPermissions.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblPermissions.AutoSize = True
        lblPermissions.Name = "lblPermissions"
        lblPermissions.Text = "Permissions password (optional):"
        '
        ' txtPermissions
        '
        txtPermissions.Dock = System.Windows.Forms.DockStyle.Fill
        txtPermissions.Name = "txtPermissions"
        txtPermissions.TabIndex = 4
        txtPermissions.UseSystemPasswordChar = True
        '
        ' lblAllow
        '
        lblAllow.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblAllow.AutoSize = True
        lblAllow.Name = "lblAllow"
        lblAllow.Text = "People who open it may:"
        '
        ' flowAllow
        '
        flowAllow.AutoSize = True
        flowAllow.Controls.Add(chkPrint)
        flowAllow.Controls.Add(chkCopy)
        flowAllow.Controls.Add(chkEdit)
        flowAllow.Margin = New System.Windows.Forms.Padding(0)
        flowAllow.Name = "flowAllow"
        flowAllow.TabIndex = 5
        '
        ' chkPrint
        '
        chkPrint.AutoSize = True
        chkPrint.Checked = True
        chkPrint.CheckState = System.Windows.Forms.CheckState.Checked
        chkPrint.Name = "chkPrint"
        chkPrint.Text = "Print"
        '
        ' chkCopy
        '
        chkCopy.AutoSize = True
        chkCopy.Checked = True
        chkCopy.CheckState = System.Windows.Forms.CheckState.Checked
        chkCopy.Name = "chkCopy"
        chkCopy.Text = "Copy text"
        '
        ' chkEdit
        '
        chkEdit.AutoSize = True
        chkEdit.Checked = True
        chkEdit.CheckState = System.Windows.Forms.CheckState.Checked
        chkEdit.Name = "chkEdit"
        chkEdit.Text = "Edit"
        '
        ' lblProblem
        '
        lblProblem.AutoSize = True
        lblProblem.Margin = New System.Windows.Forms.Padding(3, 10, 3, 0)
        lblProblem.MaximumSize = New System.Drawing.Size(500, 0)
        lblProblem.Name = "lblProblem"
        '
        ' flowButtons
        '
        flowButtons.AutoSize = True
        flowButtons.Controls.Add(btnCancel)
        flowButtons.Controls.Add(btnOk)
        flowButtons.Dock = System.Windows.Forms.DockStyle.Fill
        flowButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft
        flowButtons.Margin = New System.Windows.Forms.Padding(0, 12, 0, 0)
        flowButtons.Name = "flowButtons"
        flowButtons.TabIndex = 6
        '
        ' btnCancel
        '
        btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New System.Drawing.Size(100, 36)
        btnCancel.TabIndex = 1
        btnCancel.Text = "Cancel"
        '
        ' btnOk
        '
        btnOk.Name = "btnOk"
        btnOk.Size = New System.Drawing.Size(100, 36)
        btnOk.TabIndex = 0
        btnOk.Tag = "primary"
        btnOk.Text = "OK"
        '
        ' ProtectDialog
        '
        AcceptButton = btnOk
        AutoScaleDimensions = New System.Drawing.SizeF(96F, 96F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        AutoSize = True
        AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        CancelButton = btnCancel
        ClientSize = New System.Drawing.Size(540, 300)
        Controls.Add(tblLayout)
        Font = New System.Drawing.Font("Segoe UI", 9.5F)
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "ProtectDialog"
        ShowInTaskbar = False
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Text = "Protect the new PDFs with a password"
        tblLayout.ResumeLayout(False)
        tblLayout.PerformLayout()
        flowAllow.ResumeLayout(False)
        flowAllow.PerformLayout()
        flowButtons.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents tblLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents lblOpen As System.Windows.Forms.Label
    Friend WithEvents txtOpen As System.Windows.Forms.TextBox
    Friend WithEvents lblConfirm As System.Windows.Forms.Label
    Friend WithEvents txtConfirm As System.Windows.Forms.TextBox
    Friend WithEvents chkShow As System.Windows.Forms.CheckBox
    Friend WithEvents lblPermissions As System.Windows.Forms.Label
    Friend WithEvents txtPermissions As System.Windows.Forms.TextBox
    Friend WithEvents lblAllow As System.Windows.Forms.Label
    Friend WithEvents flowAllow As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents chkPrint As System.Windows.Forms.CheckBox
    Friend WithEvents chkCopy As System.Windows.Forms.CheckBox
    Friend WithEvents chkEdit As System.Windows.Forms.CheckBox
    Friend WithEvents lblProblem As System.Windows.Forms.Label
    Friend WithEvents flowButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOk As System.Windows.Forms.Button
End Class
