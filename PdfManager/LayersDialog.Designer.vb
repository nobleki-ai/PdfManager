<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class LayersDialog
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
        flowMode = New System.Windows.Forms.FlowLayoutPanel()
        rbStampOnly = New System.Windows.Forms.RadioButton()
        rbBackgroundOnly = New System.Windows.Forms.RadioButton()
        rbBoth = New System.Windows.Forms.RadioButton()
        lblStamp = New System.Windows.Forms.Label()
        txtStamp = New System.Windows.Forms.TextBox()
        btnStamp = New System.Windows.Forms.Button()
        lblStampPage = New System.Windows.Forms.Label()
        numStampPage = New System.Windows.Forms.NumericUpDown()
        lblBackground = New System.Windows.Forms.Label()
        txtBackground = New System.Windows.Forms.TextBox()
        btnBackground = New System.Windows.Forms.Button()
        lblBackgroundPage = New System.Windows.Forms.Label()
        numBackgroundPage = New System.Windows.Forms.NumericUpDown()
        lblPages = New System.Windows.Forms.Label()
        txtPages = New System.Windows.Forms.TextBox()
        lblPagesHint = New System.Windows.Forms.Label()
        lblProblem = New System.Windows.Forms.Label()
        flowButtons = New System.Windows.Forms.FlowLayoutPanel()
        btnCancel = New System.Windows.Forms.Button()
        btnOk = New System.Windows.Forms.Button()
        tblLayout.SuspendLayout()
        flowMode.SuspendLayout()
        CType(numStampPage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(numBackgroundPage, System.ComponentModel.ISupportInitialize).BeginInit()
        flowButtons.SuspendLayout()
        SuspendLayout()
        '
        ' tblLayout
        '
        tblLayout.ColumnCount = 5
        tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblLayout.Controls.Add(lblInfo, 0, 0)
        tblLayout.SetColumnSpan(lblInfo, 5)
        tblLayout.Controls.Add(flowMode, 0, 1)
        tblLayout.SetColumnSpan(flowMode, 5)
        tblLayout.Controls.Add(lblStamp, 0, 2)
        tblLayout.Controls.Add(txtStamp, 1, 2)
        tblLayout.Controls.Add(btnStamp, 2, 2)
        tblLayout.Controls.Add(lblStampPage, 3, 2)
        tblLayout.Controls.Add(numStampPage, 4, 2)
        tblLayout.Controls.Add(lblBackground, 0, 3)
        tblLayout.Controls.Add(txtBackground, 1, 3)
        tblLayout.Controls.Add(btnBackground, 2, 3)
        tblLayout.Controls.Add(lblBackgroundPage, 3, 3)
        tblLayout.Controls.Add(numBackgroundPage, 4, 3)
        tblLayout.Controls.Add(lblPages, 0, 4)
        tblLayout.Controls.Add(txtPages, 1, 4)
        tblLayout.Controls.Add(lblPagesHint, 2, 4)
        tblLayout.SetColumnSpan(lblPagesHint, 3)
        tblLayout.Controls.Add(lblProblem, 0, 5)
        tblLayout.SetColumnSpan(lblProblem, 5)
        tblLayout.Controls.Add(flowButtons, 0, 6)
        tblLayout.SetColumnSpan(flowButtons, 5)
        tblLayout.AutoSize = True
        tblLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        tblLayout.Location = New System.Drawing.Point(0, 0)
        tblLayout.Name = "tblLayout"
        tblLayout.Padding = New System.Windows.Forms.Padding(14)
        tblLayout.RowCount = 7
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
        lblInfo.Margin = New System.Windows.Forms.Padding(3, 0, 3, 8)
        lblInfo.MaximumSize = New System.Drawing.Size(640, 0)
        lblInfo.Name = "lblInfo"
        lblInfo.Text = "Every new PDF this makes gets a page of another PDF on top (a stamp: logo, watermark, ""Approved"") and/or behind (a background: letterhead)."
        '
        ' flowMode
        '
        flowMode.AutoSize = True
        flowMode.Controls.Add(rbStampOnly)
        flowMode.Controls.Add(rbBackgroundOnly)
        flowMode.Controls.Add(rbBoth)
        flowMode.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        flowMode.Name = "flowMode"
        flowMode.TabIndex = 0
        '
        ' rbStampOnly
        '
        rbStampOnly.AutoSize = True
        rbStampOnly.Checked = True
        rbStampOnly.Name = "rbStampOnly"
        rbStampOnly.TabStop = True
        rbStampOnly.Text = "A stamp only"
        '
        ' rbBackgroundOnly
        '
        rbBackgroundOnly.AutoSize = True
        rbBackgroundOnly.Margin = New System.Windows.Forms.Padding(16, 3, 3, 3)
        rbBackgroundOnly.Name = "rbBackgroundOnly"
        rbBackgroundOnly.Text = "A background only"
        '
        ' rbBoth
        '
        rbBoth.AutoSize = True
        rbBoth.Margin = New System.Windows.Forms.Padding(16, 3, 3, 3)
        rbBoth.Name = "rbBoth"
        rbBoth.Text = "Both"
        '
        ' lblStamp
        '
        lblStamp.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblStamp.AutoSize = True
        lblStamp.Name = "lblStamp"
        lblStamp.Text = "Stamp PDF:"
        '
        ' txtStamp
        '
        txtStamp.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        txtStamp.MinimumSize = New System.Drawing.Size(260, 0)
        txtStamp.Name = "txtStamp"
        txtStamp.ReadOnly = True
        txtStamp.TabStop = False
        '
        ' btnStamp
        '
        btnStamp.AutoSize = True
        btnStamp.MinimumSize = New System.Drawing.Size(96, 34)
        btnStamp.Name = "btnStamp"
        btnStamp.TabIndex = 1
        btnStamp.Text = "Choose..."
        '
        ' lblStampPage
        '
        lblStampPage.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblStampPage.AutoSize = True
        lblStampPage.Margin = New System.Windows.Forms.Padding(10, 0, 3, 0)
        lblStampPage.Name = "lblStampPage"
        lblStampPage.Text = "use page"
        '
        ' numStampPage
        '
        numStampPage.Anchor = System.Windows.Forms.AnchorStyles.Left
        numStampPage.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numStampPage.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numStampPage.Name = "numStampPage"
        numStampPage.Size = New System.Drawing.Size(64, 25)
        numStampPage.TabIndex = 2
        numStampPage.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        ' lblBackground
        '
        lblBackground.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblBackground.AutoSize = True
        lblBackground.Name = "lblBackground"
        lblBackground.Text = "Background PDF:"
        '
        ' txtBackground
        '
        txtBackground.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        txtBackground.MinimumSize = New System.Drawing.Size(260, 0)
        txtBackground.Name = "txtBackground"
        txtBackground.ReadOnly = True
        txtBackground.TabStop = False
        '
        ' btnBackground
        '
        btnBackground.AutoSize = True
        btnBackground.MinimumSize = New System.Drawing.Size(96, 34)
        btnBackground.Name = "btnBackground"
        btnBackground.TabIndex = 3
        btnBackground.Text = "Choose..."
        '
        ' lblBackgroundPage
        '
        lblBackgroundPage.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblBackgroundPage.AutoSize = True
        lblBackgroundPage.Margin = New System.Windows.Forms.Padding(10, 0, 3, 0)
        lblBackgroundPage.Name = "lblBackgroundPage"
        lblBackgroundPage.Text = "use page"
        '
        ' numBackgroundPage
        '
        numBackgroundPage.Anchor = System.Windows.Forms.AnchorStyles.Left
        numBackgroundPage.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numBackgroundPage.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numBackgroundPage.Name = "numBackgroundPage"
        numBackgroundPage.Size = New System.Drawing.Size(64, 25)
        numBackgroundPage.TabIndex = 4
        numBackgroundPage.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        ' lblPages
        '
        lblPages.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblPages.AutoSize = True
        lblPages.Name = "lblPages"
        lblPages.Text = "Put them on pages:"
        '
        ' txtPages
        '
        txtPages.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        txtPages.Name = "txtPages"
        txtPages.PlaceholderText = "All"
        txtPages.TabIndex = 5
        '
        ' lblPagesHint
        '
        lblPagesHint.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblPagesHint.AutoSize = True
        lblPagesHint.Name = "lblPagesHint"
        lblPagesHint.Tag = "muted"
        lblPagesHint.Text = "of each new PDF, e.g. 1 or 2-end"
        '
        ' lblProblem
        '
        lblProblem.AutoSize = True
        lblProblem.Margin = New System.Windows.Forms.Padding(3, 10, 3, 0)
        lblProblem.MaximumSize = New System.Drawing.Size(640, 0)
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
        ' LayersDialog
        '
        AcceptButton = btnOk
        AutoScaleDimensions = New System.Drawing.SizeF(96F, 96F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        AutoSize = True
        AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        CancelButton = btnCancel
        ClientSize = New System.Drawing.Size(700, 320)
        Controls.Add(tblLayout)
        Font = New System.Drawing.Font("Segoe UI", 9.5F)
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "LayersDialog"
        ShowInTaskbar = False
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Text = "Add a stamp / background to the new PDFs"
        tblLayout.ResumeLayout(False)
        tblLayout.PerformLayout()
        flowMode.ResumeLayout(False)
        flowMode.PerformLayout()
        CType(numStampPage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(numBackgroundPage, System.ComponentModel.ISupportInitialize).EndInit()
        flowButtons.ResumeLayout(False)
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents tblLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblInfo As System.Windows.Forms.Label
    Friend WithEvents flowMode As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents rbStampOnly As System.Windows.Forms.RadioButton
    Friend WithEvents rbBackgroundOnly As System.Windows.Forms.RadioButton
    Friend WithEvents rbBoth As System.Windows.Forms.RadioButton
    Friend WithEvents lblStamp As System.Windows.Forms.Label
    Friend WithEvents txtStamp As System.Windows.Forms.TextBox
    Friend WithEvents btnStamp As System.Windows.Forms.Button
    Friend WithEvents lblStampPage As System.Windows.Forms.Label
    Friend WithEvents numStampPage As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblBackground As System.Windows.Forms.Label
    Friend WithEvents txtBackground As System.Windows.Forms.TextBox
    Friend WithEvents btnBackground As System.Windows.Forms.Button
    Friend WithEvents lblBackgroundPage As System.Windows.Forms.Label
    Friend WithEvents numBackgroundPage As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblPages As System.Windows.Forms.Label
    Friend WithEvents txtPages As System.Windows.Forms.TextBox
    Friend WithEvents lblPagesHint As System.Windows.Forms.Label
    Friend WithEvents lblProblem As System.Windows.Forms.Label
    Friend WithEvents flowButtons As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnOk As System.Windows.Forms.Button
End Class
