<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PageViewForm
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
        lblHint = New System.Windows.Forms.Label()
        flowTools = New System.Windows.Forms.FlowLayoutPanel()
        btnRemove = New System.Windows.Forms.Button()
        btnSelectAll = New System.Windows.Forms.Button()
        btnMoveFirst = New System.Windows.Forms.Button()
        btnMoveLast = New System.Windows.Forms.Button()
        btnReset = New System.Windows.Forms.Button()
        lblSize = New System.Windows.Forms.Label()
        trkSize = New System.Windows.Forms.TrackBar()
        pageGrid = New PageGrid()
        pnlBottom = New System.Windows.Forms.TableLayoutPanel()
        lblSummary = New System.Windows.Forms.Label()
        btnDone = New System.Windows.Forms.Button()
        btnCancel = New System.Windows.Forms.Button()
        rootLayout.SuspendLayout()
        flowTools.SuspendLayout()
        CType(trkSize, System.ComponentModel.ISupportInitialize).BeginInit()
        pnlBottom.SuspendLayout()
        SuspendLayout()
        '
        ' rootLayout
        '
        rootLayout.ColumnCount = 1
        rootLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rootLayout.Controls.Add(lblHint, 0, 0)
        rootLayout.Controls.Add(flowTools, 0, 1)
        rootLayout.Controls.Add(pageGrid, 0, 2)
        rootLayout.Controls.Add(pnlBottom, 0, 3)
        rootLayout.Dock = System.Windows.Forms.DockStyle.Fill
        rootLayout.Location = New System.Drawing.Point(0, 0)
        rootLayout.Name = "rootLayout"
        rootLayout.Padding = New System.Windows.Forms.Padding(12)
        rootLayout.RowCount = 4
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.Size = New System.Drawing.Size(1000, 720)
        rootLayout.TabIndex = 0
        '
        ' lblHint
        '
        lblHint.AutoSize = True
        lblHint.Font = New System.Drawing.Font("Segoe UI", 10F)
        lblHint.Margin = New System.Windows.Forms.Padding(3, 0, 3, 8)
        lblHint.MaximumSize = New System.Drawing.Size(960, 0)
        lblHint.Name = "lblHint"
        lblHint.TabIndex = 0
        lblHint.Text = "Drag pages to put them in a new order. Click a page's ✕, or select pages and press Delete, to leave them out. " &
            "Ctrl+click or Shift+click selects several pages; Ctrl+mouse wheel changes the picture size."
        '
        ' flowTools
        '
        flowTools.AutoSize = True
        flowTools.Controls.Add(btnRemove)
        flowTools.Controls.Add(btnSelectAll)
        flowTools.Controls.Add(btnMoveFirst)
        flowTools.Controls.Add(btnMoveLast)
        flowTools.Controls.Add(btnReset)
        flowTools.Controls.Add(lblSize)
        flowTools.Controls.Add(trkSize)
        flowTools.Dock = System.Windows.Forms.DockStyle.Fill
        flowTools.Margin = New System.Windows.Forms.Padding(0, 0, 0, 6)
        flowTools.Name = "flowTools"
        flowTools.TabIndex = 1
        flowTools.WrapContents = True
        '
        ' btnRemove
        '
        btnRemove.Name = "btnRemove"
        btnRemove.Size = New System.Drawing.Size(170, 36)
        btnRemove.TabIndex = 0
        btnRemove.Text = "Remove Selected"
        btnRemove.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        ' btnSelectAll
        '
        btnSelectAll.Name = "btnSelectAll"
        btnSelectAll.Size = New System.Drawing.Size(120, 36)
        btnSelectAll.TabIndex = 1
        btnSelectAll.Text = "Select All"
        btnSelectAll.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        ' btnMoveFirst
        '
        btnMoveFirst.Name = "btnMoveFirst"
        btnMoveFirst.Size = New System.Drawing.Size(140, 36)
        btnMoveFirst.TabIndex = 2
        btnMoveFirst.Text = "Move to Start"
        btnMoveFirst.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        ' btnMoveLast
        '
        btnMoveLast.Name = "btnMoveLast"
        btnMoveLast.Size = New System.Drawing.Size(130, 36)
        btnMoveLast.TabIndex = 3
        btnMoveLast.Text = "Move to End"
        btnMoveLast.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        ' btnReset
        '
        btnReset.Name = "btnReset"
        btnReset.Size = New System.Drawing.Size(125, 36)
        btnReset.TabIndex = 4
        btnReset.Text = "Start Over"
        btnReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        '
        ' lblSize
        '
        lblSize.AutoSize = True
        lblSize.Margin = New System.Windows.Forms.Padding(24, 10, 0, 3)
        lblSize.Name = "lblSize"
        lblSize.TabIndex = 5
        lblSize.Text = "Picture size:"
        '
        ' trkSize
        '
        trkSize.AutoSize = False
        trkSize.LargeChange = 40
        trkSize.Maximum = 240
        trkSize.Minimum = 80
        trkSize.Name = "trkSize"
        trkSize.Size = New System.Drawing.Size(160, 32)
        trkSize.SmallChange = 20
        trkSize.TabIndex = 6
        trkSize.TickFrequency = 40
        trkSize.Value = 130
        '
        ' pageGrid
        '
        pageGrid.Dock = System.Windows.Forms.DockStyle.Fill
        pageGrid.Font = New System.Drawing.Font("Segoe UI", 9F)
        pageGrid.Margin = New System.Windows.Forms.Padding(3)
        pageGrid.Name = "pageGrid"
        pageGrid.TabIndex = 2
        '
        ' pnlBottom
        '
        pnlBottom.AutoSize = True
        pnlBottom.ColumnCount = 3
        pnlBottom.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        pnlBottom.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        pnlBottom.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        pnlBottom.Controls.Add(lblSummary, 0, 0)
        pnlBottom.Controls.Add(btnDone, 1, 0)
        pnlBottom.Controls.Add(btnCancel, 2, 0)
        pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill
        pnlBottom.Margin = New System.Windows.Forms.Padding(0, 8, 0, 0)
        pnlBottom.Name = "pnlBottom"
        pnlBottom.RowCount = 1
        pnlBottom.RowStyles.Add(New System.Windows.Forms.RowStyle())
        pnlBottom.TabIndex = 3
        '
        ' lblSummary
        '
        lblSummary.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblSummary.AutoSize = True
        lblSummary.Font = New System.Drawing.Font("Segoe UI", 10F)
        lblSummary.Name = "lblSummary"
        lblSummary.TabIndex = 0
        '
        ' btnDone
        '
        btnDone.Font = New System.Drawing.Font("Segoe UI Semibold", 11F)
        btnDone.Name = "btnDone"
        btnDone.Size = New System.Drawing.Size(190, 42)
        btnDone.TabIndex = 1
        btnDone.Tag = "primary"
        btnDone.Text = "Use These Pages"
        '
        ' btnCancel
        '
        btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New System.Drawing.Size(110, 42)
        btnCancel.TabIndex = 2
        btnCancel.Text = "Cancel"
        '
        ' PageViewForm
        '
        AcceptButton = btnDone
        AutoScaleDimensions = New System.Drawing.SizeF(96F, 96F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        CancelButton = btnCancel
        ClientSize = New System.Drawing.Size(1000, 720)
        Controls.Add(rootLayout)
        Font = New System.Drawing.Font("Segoe UI", 9F)
        MinimizeBox = False
        MinimumSize = New System.Drawing.Size(640, 480)
        Name = "PageViewForm"
        ShowInTaskbar = False
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Text = "Arrange Pages"
        rootLayout.ResumeLayout(False)
        rootLayout.PerformLayout()
        flowTools.ResumeLayout(False)
        flowTools.PerformLayout()
        CType(trkSize, System.ComponentModel.ISupportInitialize).EndInit()
        pnlBottom.ResumeLayout(False)
        pnlBottom.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents rootLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblHint As System.Windows.Forms.Label
    Friend WithEvents flowTools As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents btnSelectAll As System.Windows.Forms.Button
    Friend WithEvents btnMoveFirst As System.Windows.Forms.Button
    Friend WithEvents btnMoveLast As System.Windows.Forms.Button
    Friend WithEvents btnReset As System.Windows.Forms.Button
    Friend WithEvents lblSize As System.Windows.Forms.Label
    Friend WithEvents trkSize As System.Windows.Forms.TrackBar
    Friend WithEvents pageGrid As PageGrid
    Friend WithEvents pnlBottom As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblSummary As System.Windows.Forms.Label
    Friend WithEvents btnDone As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
End Class
