<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class HelpForm
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
        rootLayout = New System.Windows.Forms.TableLayoutPanel()
        lstTopics = New System.Windows.Forms.ListBox()
        rightLayout = New System.Windows.Forms.TableLayoutPanel()
        lblTopicTitle = New System.Windows.Forms.Label()
        pnlTry = New System.Windows.Forms.FlowLayoutPanel()
        lblTry = New System.Windows.Forms.Label()
        txtTry = New System.Windows.Forms.TextBox()
        lblTryIn = New System.Windows.Forms.Label()
        numTryPages = New System.Windows.Forms.NumericUpDown()
        lblTryPages = New System.Windows.Forms.Label()
        lblTryResult = New System.Windows.Forms.Label()
        rtbHelp = New System.Windows.Forms.RichTextBox()
        rootLayout.SuspendLayout()
        rightLayout.SuspendLayout()
        pnlTry.SuspendLayout()
        CType(numTryPages, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        '
        ' rootLayout
        '
        rootLayout.ColumnCount = 2
        rootLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 230F))
        rootLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rootLayout.Controls.Add(lstTopics, 0, 0)
        rootLayout.Controls.Add(rightLayout, 1, 0)
        rootLayout.Dock = System.Windows.Forms.DockStyle.Fill
        rootLayout.Location = New System.Drawing.Point(0, 0)
        rootLayout.Name = "rootLayout"
        rootLayout.Padding = New System.Windows.Forms.Padding(10)
        rootLayout.RowCount = 1
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rootLayout.Size = New System.Drawing.Size(940, 680)
        rootLayout.TabIndex = 0
        '
        ' lstTopics
        '
        lstTopics.BorderStyle = System.Windows.Forms.BorderStyle.None
        lstTopics.Dock = System.Windows.Forms.DockStyle.Fill
        lstTopics.Font = New System.Drawing.Font("Segoe UI", 10.5F)
        lstTopics.IntegralHeight = False
        lstTopics.ItemHeight = 19
        lstTopics.Margin = New System.Windows.Forms.Padding(0, 0, 10, 0)
        lstTopics.Name = "lstTopics"
        lstTopics.TabIndex = 0
        lstTopics.Tag = "card"
        '
        ' rightLayout
        '
        rightLayout.ColumnCount = 1
        rightLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rightLayout.Controls.Add(lblTopicTitle, 0, 0)
        rightLayout.Controls.Add(pnlTry, 0, 1)
        rightLayout.Controls.Add(rtbHelp, 0, 2)
        rightLayout.Dock = System.Windows.Forms.DockStyle.Fill
        rightLayout.Margin = New System.Windows.Forms.Padding(0)
        rightLayout.Name = "rightLayout"
        rightLayout.RowCount = 3
        rightLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rightLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rightLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rightLayout.TabIndex = 1
        '
        ' lblTopicTitle
        '
        lblTopicTitle.AutoSize = True
        lblTopicTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 15F)
        lblTopicTitle.Margin = New System.Windows.Forms.Padding(3, 0, 3, 8)
        lblTopicTitle.Name = "lblTopicTitle"
        lblTopicTitle.TabIndex = 0
        lblTopicTitle.Text = "Help"
        '
        ' pnlTry
        '
        pnlTry.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        pnlTry.AutoSize = True
        pnlTry.Controls.Add(lblTry)
        pnlTry.Controls.Add(txtTry)
        pnlTry.Controls.Add(lblTryIn)
        pnlTry.Controls.Add(numTryPages)
        pnlTry.Controls.Add(lblTryPages)
        pnlTry.Controls.Add(lblTryResult)
        pnlTry.Margin = New System.Windows.Forms.Padding(0, 0, 0, 10)
        pnlTry.Name = "pnlTry"
        pnlTry.Padding = New System.Windows.Forms.Padding(8)
        pnlTry.TabIndex = 1
        pnlTry.Tag = "card"
        pnlTry.Visible = False
        '
        ' lblTry
        '
        lblTry.AutoSize = True
        lblTry.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        lblTry.Margin = New System.Windows.Forms.Padding(3, 6, 3, 3)
        lblTry.Name = "lblTry"
        lblTry.TabIndex = 0
        lblTry.Text = "Try it: pages"
        '
        ' txtTry
        '
        txtTry.Font = New System.Drawing.Font("Segoe UI", 10F)
        txtTry.Name = "txtTry"
        txtTry.PlaceholderText = "1-3, 5, 8-end"
        txtTry.Size = New System.Drawing.Size(200, 25)
        txtTry.TabIndex = 1
        txtTry.Text = "1-3, 5, 8-end"
        '
        ' lblTryIn
        '
        lblTryIn.AutoSize = True
        lblTryIn.Margin = New System.Windows.Forms.Padding(3, 7, 3, 3)
        lblTryIn.Name = "lblTryIn"
        lblTryIn.TabIndex = 2
        lblTryIn.Text = "in a PDF with"
        '
        ' numTryPages
        '
        numTryPages.Font = New System.Drawing.Font("Segoe UI", 10F)
        numTryPages.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numTryPages.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numTryPages.Name = "numTryPages"
        numTryPages.Size = New System.Drawing.Size(64, 25)
        numTryPages.TabIndex = 3
        numTryPages.Value = New Decimal(New Integer() {10, 0, 0, 0})
        '
        ' lblTryPages
        '
        lblTryPages.AutoSize = True
        lblTryPages.Margin = New System.Windows.Forms.Padding(3, 7, 3, 3)
        lblTryPages.Name = "lblTryPages"
        pnlTry.SetFlowBreak(lblTryPages, True)
        lblTryPages.TabIndex = 4
        lblTryPages.Text = "pages"
        '
        ' lblTryResult
        '
        lblTryResult.AutoSize = True
        lblTryResult.Font = New System.Drawing.Font("Segoe UI", 10F)
        lblTryResult.Margin = New System.Windows.Forms.Padding(3, 6, 3, 3)
        lblTryResult.MaximumSize = New System.Drawing.Size(640, 0)
        lblTryResult.Name = "lblTryResult"
        lblTryResult.TabIndex = 5
        '
        ' rtbHelp
        '
        rtbHelp.BorderStyle = System.Windows.Forms.BorderStyle.None
        rtbHelp.DetectUrls = False
        rtbHelp.Dock = System.Windows.Forms.DockStyle.Fill
        rtbHelp.Name = "rtbHelp"
        rtbHelp.ReadOnly = True
        rtbHelp.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical
        rtbHelp.TabIndex = 2
        rtbHelp.TabStop = False
        rtbHelp.Text = ""
        '
        ' HelpForm
        '
        AutoScaleDimensions = New System.Drawing.SizeF(96F, 96F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        ClientSize = New System.Drawing.Size(940, 680)
        Controls.Add(rootLayout)
        Font = New System.Drawing.Font("Segoe UI", 9F)
        KeyPreview = True
        MinimumSize = New System.Drawing.Size(720, 480)
        Name = "HelpForm"
        ShowInTaskbar = False
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Text = "PDF Manager Help"
        rootLayout.ResumeLayout(False)
        rightLayout.ResumeLayout(False)
        rightLayout.PerformLayout()
        pnlTry.ResumeLayout(False)
        pnlTry.PerformLayout()
        CType(numTryPages, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents rootLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lstTopics As System.Windows.Forms.ListBox
    Friend WithEvents rightLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblTopicTitle As System.Windows.Forms.Label
    Friend WithEvents pnlTry As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblTry As System.Windows.Forms.Label
    Friend WithEvents txtTry As System.Windows.Forms.TextBox
    Friend WithEvents lblTryIn As System.Windows.Forms.Label
    Friend WithEvents numTryPages As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblTryPages As System.Windows.Forms.Label
    Friend WithEvents lblTryResult As System.Windows.Forms.Label
    Friend WithEvents rtbHelp As System.Windows.Forms.RichTextBox
End Class
