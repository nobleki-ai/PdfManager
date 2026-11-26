<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        pnlHeader = New System.Windows.Forms.Panel()
        picLogo = New System.Windows.Forms.PictureBox()
        lblAppTitle = New System.Windows.Forms.Label()
        lblAppSubtitle = New System.Windows.Forms.Label()
        flowTheme = New System.Windows.Forms.FlowLayoutPanel()
        lblTheme = New System.Windows.Forms.Label()
        btnHelp = New System.Windows.Forms.Button()
        lnkMergeRangeHelp = New System.Windows.Forms.LinkLabel()
        lnkExtractRangeHelp = New System.Windows.Forms.LinkLabel()
        lnkRotateRangeHelp = New System.Windows.Forms.LinkLabel()
        lnkStampRangeHelp = New System.Windows.Forms.LinkLabel()
        rbThemeLight = New System.Windows.Forms.RadioButton()
        rbThemeDark = New System.Windows.Forms.RadioButton()
        rbThemeAuto = New System.Windows.Forms.RadioButton()
        flowOperations = New System.Windows.Forms.FlowLayoutPanel()
        rbOpCombine = New System.Windows.Forms.RadioButton()
        rbOpMerge = New System.Windows.Forms.RadioButton()
        rbOpExtract = New System.Windows.Forms.RadioButton()
        rbOpEncrypt = New System.Windows.Forms.RadioButton()
        rbOpRotate = New System.Windows.Forms.RadioButton()
        rbOpStamp = New System.Windows.Forms.RadioButton()
        rbOpCompress = New System.Windows.Forms.RadioButton()
        rbOpArrange = New System.Windows.Forms.RadioButton()
        tabArrange = New System.Windows.Forms.TabPage()
        lblArrangeInfo = New System.Windows.Forms.Label()
        btnShowPages = New System.Windows.Forms.Button()
        btnArrangeReset = New System.Windows.Forms.Button()
        lblArrangeSummary = New System.Windows.Forms.Label()
        tabRotate = New System.Windows.Forms.TabPage()
        lblRotateInfo = New System.Windows.Forms.Label()
        pnlRotateAngle = New System.Windows.Forms.Panel()
        lblRotateTurn = New System.Windows.Forms.Label()
        rbRotateRight = New System.Windows.Forms.RadioButton()
        rbRotateLeft = New System.Windows.Forms.RadioButton()
        rbRotate180 = New System.Windows.Forms.RadioButton()
        lblRotatePages = New System.Windows.Forms.Label()
        txtRotatePages = New System.Windows.Forms.TextBox()
        lblRotateHint = New System.Windows.Forms.Label()
        tabStamp = New System.Windows.Forms.TabPage()
        lblStampInfo = New System.Windows.Forms.Label()
        pnlStampMode = New System.Windows.Forms.Panel()
        lblStampAs = New System.Windows.Forms.Label()
        rbStampOver = New System.Windows.Forms.RadioButton()
        rbStampUnder = New System.Windows.Forms.RadioButton()
        lblStampFile = New System.Windows.Forms.Label()
        txtStampFile = New System.Windows.Forms.TextBox()
        btnStampBrowse = New System.Windows.Forms.Button()
        lblStampPages = New System.Windows.Forms.Label()
        txtStampPages = New System.Windows.Forms.TextBox()
        lblStampUse = New System.Windows.Forms.Label()
        numStampPage = New System.Windows.Forms.NumericUpDown()
        rbStampBoth = New System.Windows.Forms.RadioButton()
        lblBackgroundFile = New System.Windows.Forms.Label()
        txtBackgroundFile = New System.Windows.Forms.TextBox()
        btnBackgroundBrowse = New System.Windows.Forms.Button()
        lblBackgroundUse = New System.Windows.Forms.Label()
        numBackgroundPage = New System.Windows.Forms.NumericUpDown()
        tabCompress = New System.Windows.Forms.TabPage()
        lblCompressInfo = New System.Windows.Forms.Label()
        pnlCompressLevel = New System.Windows.Forms.Panel()
        rbCompressNormal = New System.Windows.Forms.RadioButton()
        rbCompressMedium = New System.Windows.Forms.RadioButton()
        rbCompressHigh = New System.Windows.Forms.RadioButton()
        rbCompressExtreme = New System.Windows.Forms.RadioButton()
        rootLayout = New System.Windows.Forms.TableLayoutPanel()
        pnlQpdfMissing = New System.Windows.Forms.FlowLayoutPanel()
        lblQpdfMissing = New System.Windows.Forms.Label()
        btnLocateQpdf = New System.Windows.Forms.Button()
        lnkDownloadQpdf = New System.Windows.Forms.LinkLabel()
        lblStep1 = New System.Windows.Forms.Label()
        flowToolbar = New System.Windows.Forms.FlowLayoutPanel()
        btnAddPdf = New System.Windows.Forms.Button()
        btnAddFolder = New System.Windows.Forms.Button()
        chkSubfolders = New System.Windows.Forms.CheckBox()
        btnMoveUp = New System.Windows.Forms.Button()
        btnMoveDown = New System.Windows.Forms.Button()
        btnSort = New System.Windows.Forms.Button()
        btnRemove = New System.Windows.Forms.Button()
        btnClear = New System.Windows.Forms.Button()
        grid = New System.Windows.Forms.DataGridView()
        colUse = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        colPosition = New System.Windows.Forms.DataGridViewTextBoxColumn()
        colFileName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        colPageCount = New System.Windows.Forms.DataGridViewTextBoxColumn()
        colPagesToUse = New System.Windows.Forms.DataGridViewTextBoxColumn()
        colSize = New System.Windows.Forms.DataGridViewTextBoxColumn()
        colStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
        colFolder = New System.Windows.Forms.DataGridViewTextBoxColumn()
        lblStep2 = New System.Windows.Forms.Label()
        tabOperations = New OptionsTabControl()
        tabCombine = New System.Windows.Forms.TabPage()
        lblCombineInfo = New System.Windows.Forms.Label()
        chkKeepDetails = New System.Windows.Forms.CheckBox()
        tabMerge = New System.Windows.Forms.TabPage()
        lblMergeInfo = New System.Windows.Forms.Label()
        btnUseAll = New System.Windows.Forms.Button()
        btnUseNone = New System.Windows.Forms.Button()
        lblMergeSummary = New System.Windows.Forms.Label()
        tabExtract = New System.Windows.Forms.TabPage()
        lblExtractPages = New System.Windows.Forms.Label()
        txtExtractPages = New System.Windows.Forms.TextBox()
        lblExtractHint = New System.Windows.Forms.Label()
        pnlExtractFrom = New System.Windows.Forms.Panel()
        lblExtractFrom = New System.Windows.Forms.Label()
        rbExtractSelected = New System.Windows.Forms.RadioButton()
        rbExtractAll = New System.Windows.Forms.RadioButton()
        pnlExtractOutput = New System.Windows.Forms.Panel()
        lblExtractSave = New System.Windows.Forms.Label()
        rbExtractOne = New System.Windows.Forms.RadioButton()
        rbExtractEach = New System.Windows.Forms.RadioButton()
        rbExtractEvery = New System.Windows.Forms.RadioButton()
        numSplitEvery = New System.Windows.Forms.NumericUpDown()
        lblSplitPages = New System.Windows.Forms.Label()
        tabEncrypt = New System.Windows.Forms.TabPage()
        lblOpenPassword = New System.Windows.Forms.Label()
        txtOpenPassword = New System.Windows.Forms.TextBox()
        lblConfirmPassword = New System.Windows.Forms.Label()
        txtConfirmPassword = New System.Windows.Forms.TextBox()
        chkShowPasswords = New System.Windows.Forms.CheckBox()
        lblPermissionsPassword = New System.Windows.Forms.Label()
        txtPermissionsPassword = New System.Windows.Forms.TextBox()
        lblAllow = New System.Windows.Forms.Label()
        chkAllowPrinting = New System.Windows.Forms.CheckBox()
        chkAllowCopying = New System.Windows.Forms.CheckBox()
        chkAllowEditing = New System.Windows.Forms.CheckBox()
        lblEncryptInfo = New System.Windows.Forms.Label()
        pnlEncryptMode = New System.Windows.Forms.Panel()
        lblEncryptMode = New System.Windows.Forms.Label()
        rbAddPassword = New System.Windows.Forms.RadioButton()
        rbRemovePassword = New System.Windows.Forms.RadioButton()
        lblStep3 = New System.Windows.Forms.Label()
        tblOutput = New System.Windows.Forms.TableLayoutPanel()
        lblOutputFolder = New System.Windows.Forms.Label()
        txtOutputFolder = New System.Windows.Forms.TextBox()
        btnBrowse = New System.Windows.Forms.Button()
        lblOutputName = New System.Windows.Forms.Label()
        txtOutputName = New System.Windows.Forms.TextBox()
        lblPdfExtension = New System.Windows.Forms.Label()
        lblPreview = New System.Windows.Forms.Label()
        lblExtras = New System.Windows.Forms.Label()
        flowExtras = New System.Windows.Forms.FlowLayoutPanel()
        chkExtraPassword = New System.Windows.Forms.CheckBox()
        btnExtraPassword = New System.Windows.Forms.Button()
        chkExtraStamp = New System.Windows.Forms.CheckBox()
        btnExtraStamp = New System.Windows.Forms.Button()
        lblExtrasSummary = New System.Windows.Forms.Label()
        tblActions = New System.Windows.Forms.TableLayoutPanel()
        lblStatus = New System.Windows.Forms.Label()
        lnkDetails = New System.Windows.Forms.LinkLabel()
        progressBar = New System.Windows.Forms.ProgressBar()
        btnOpenFile = New System.Windows.Forms.Button()
        btnOpenFolder = New System.Windows.Forms.Button()
        btnCancel = New System.Windows.Forms.Button()
        btnRun = New System.Windows.Forms.Button()
        txtLog = New System.Windows.Forms.TextBox()
        statusStrip = New System.Windows.Forms.StatusStrip()
        lblQpdfStatus = New System.Windows.Forms.ToolStripStatusLabel()
        lnkChangeQpdf = New System.Windows.Forms.ToolStripStatusLabel()
        screenshotTimer = New System.Windows.Forms.Timer(components)
        pnlHeader.SuspendLayout()
        CType(picLogo, ComponentModel.ISupportInitialize).BeginInit()
        flowTheme.SuspendLayout()
        flowOperations.SuspendLayout()
        tabRotate.SuspendLayout()
        pnlRotateAngle.SuspendLayout()
        tabStamp.SuspendLayout()
        pnlStampMode.SuspendLayout()
        CType(numStampPage, ComponentModel.ISupportInitialize).BeginInit()
        CType(numBackgroundPage, ComponentModel.ISupportInitialize).BeginInit()
        tabCompress.SuspendLayout()
        tabArrange.SuspendLayout()
        pnlCompressLevel.SuspendLayout()
        rootLayout.SuspendLayout()
        pnlQpdfMissing.SuspendLayout()
        flowToolbar.SuspendLayout()
        CType(grid, ComponentModel.ISupportInitialize).BeginInit()
        tabOperations.SuspendLayout()
        tabCombine.SuspendLayout()
        tabMerge.SuspendLayout()
        tabExtract.SuspendLayout()
        pnlExtractFrom.SuspendLayout()
        pnlExtractOutput.SuspendLayout()
        CType(numSplitEvery, ComponentModel.ISupportInitialize).BeginInit()
        tabEncrypt.SuspendLayout()
        pnlEncryptMode.SuspendLayout()
        tblOutput.SuspendLayout()
        tblActions.SuspendLayout()
        statusStrip.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlHeader
        ' 
        pnlHeader.Controls.Add(picLogo)
        pnlHeader.Controls.Add(lblAppTitle)
        pnlHeader.Controls.Add(lblAppSubtitle)
        pnlHeader.Controls.Add(flowTheme)
        pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        pnlHeader.Location = New System.Drawing.Point(0, 0)
        pnlHeader.Name = "pnlHeader"
        pnlHeader.Size = New System.Drawing.Size(1268, 70)
        pnlHeader.TabIndex = 0
        pnlHeader.Tag = "card"
        ' 
        ' picLogo
        ' 
        picLogo.Location = New System.Drawing.Point(17, 11)
        picLogo.Name = "picLogo"
        picLogo.Size = New System.Drawing.Size(48, 48)
        picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        picLogo.TabIndex = 0
        picLogo.TabStop = False
        ' 
        ' lblAppTitle
        ' 
        lblAppTitle.AutoSize = True
        lblAppTitle.Font = New System.Drawing.Font("Segoe UI Semibold", 15F)
        lblAppTitle.Location = New System.Drawing.Point(74, 7)
        lblAppTitle.Name = "lblAppTitle"
        lblAppTitle.Size = New System.Drawing.Size(169, 35)
        lblAppTitle.TabIndex = 1
        lblAppTitle.Text = "PDF Manager"
        ' 
        ' lblAppSubtitle
        ' 
        lblAppSubtitle.AutoSize = True
        lblAppSubtitle.Location = New System.Drawing.Point(76, 41)
        lblAppSubtitle.Name = "lblAppSubtitle"
        lblAppSubtitle.Size = New System.Drawing.Size(288, 20)
        lblAppSubtitle.TabIndex = 2
        lblAppSubtitle.Tag = "muted"
        lblAppSubtitle.Text = "Combine, merge, extract and protect PDFs"
        ' 
        ' flowTheme
        ' 
        flowTheme.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right
        flowTheme.AutoSize = True
        flowTheme.Controls.Add(btnHelp)
        flowTheme.Controls.Add(lblTheme)
        flowTheme.Controls.Add(rbThemeLight)
        flowTheme.Controls.Add(rbThemeDark)
        flowTheme.Controls.Add(rbThemeAuto)
        flowTheme.Location = New System.Drawing.Point(677, 13)
        flowTheme.Name = "flowTheme"
        flowTheme.Size = New System.Drawing.Size(575, 46)
        flowTheme.TabIndex = 3
        flowTheme.WrapContents = False
        ' 
        ' btnHelp
        '
        btnHelp.AutoSize = True
        btnHelp.Margin = New System.Windows.Forms.Padding(0, 4, 24, 4)
        btnHelp.MinimumSize = New System.Drawing.Size(118, 45)
        btnHelp.Name = "btnHelp"
        btnHelp.Size = New System.Drawing.Size(118, 45)
        btnHelp.TabIndex = 0
        btnHelp.Text = "Help"
        btnHelp.UseVisualStyleBackColor = True
        '
        ' lblTheme
        '
        lblTheme.AutoSize = True
        lblTheme.Location = New System.Drawing.Point(3, 13)
        lblTheme.Margin = New System.Windows.Forms.Padding(3, 13, 7, 3)
        lblTheme.Name = "lblTheme"
        lblTheme.Size = New System.Drawing.Size(57, 20)
        lblTheme.TabIndex = 0
        lblTheme.Text = "Theme:"
        ' 
        ' rbThemeLight
        ' 
        rbThemeLight.Appearance = System.Windows.Forms.Appearance.Button
        rbThemeLight.AutoSize = True
        rbThemeLight.Location = New System.Drawing.Point(67, 3)
        rbThemeLight.Margin = New System.Windows.Forms.Padding(0, 3, 4, 3)
        rbThemeLight.MinimumSize = New System.Drawing.Size(96, 39)
        rbThemeLight.Name = "rbThemeLight"
        rbThemeLight.Size = New System.Drawing.Size(96, 39)
        rbThemeLight.TabIndex = 1
        rbThemeLight.Tag = "toggle"
        rbThemeLight.Text = "Light"
        rbThemeLight.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbThemeLight.UseVisualStyleBackColor = True
        ' 
        ' rbThemeDark
        ' 
        rbThemeDark.Appearance = System.Windows.Forms.Appearance.Button
        rbThemeDark.AutoSize = True
        rbThemeDark.Location = New System.Drawing.Point(167, 3)
        rbThemeDark.Margin = New System.Windows.Forms.Padding(0, 3, 4, 3)
        rbThemeDark.MinimumSize = New System.Drawing.Size(96, 39)
        rbThemeDark.Name = "rbThemeDark"
        rbThemeDark.Size = New System.Drawing.Size(96, 39)
        rbThemeDark.TabIndex = 2
        rbThemeDark.Tag = "toggle"
        rbThemeDark.Text = "Dark"
        rbThemeDark.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbThemeDark.UseVisualStyleBackColor = True
        ' 
        ' rbThemeAuto
        ' 
        rbThemeAuto.Appearance = System.Windows.Forms.Appearance.Button
        rbThemeAuto.AutoSize = True
        rbThemeAuto.Checked = True
        rbThemeAuto.Location = New System.Drawing.Point(267, 3)
        rbThemeAuto.Margin = New System.Windows.Forms.Padding(0, 3, 0, 3)
        rbThemeAuto.MinimumSize = New System.Drawing.Size(96, 39)
        rbThemeAuto.Name = "rbThemeAuto"
        rbThemeAuto.Size = New System.Drawing.Size(139, 39)
        rbThemeAuto.TabIndex = 3
        rbThemeAuto.TabStop = True
        rbThemeAuto.Tag = "toggle"
        rbThemeAuto.Text = "Same as Windows"
        rbThemeAuto.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbThemeAuto.UseVisualStyleBackColor = True
        ' 
        ' flowOperations
        ' 
        flowOperations.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        flowOperations.AutoSize = True
        flowOperations.Controls.Add(rbOpCombine)
        flowOperations.Controls.Add(rbOpMerge)
        flowOperations.Controls.Add(rbOpExtract)
        flowOperations.Controls.Add(rbOpArrange)
        flowOperations.Controls.Add(rbOpEncrypt)
        flowOperations.Controls.Add(rbOpRotate)
        flowOperations.Controls.Add(rbOpStamp)
        flowOperations.Controls.Add(rbOpCompress)
        flowOperations.Location = New System.Drawing.Point(15, 117)
        flowOperations.Margin = New System.Windows.Forms.Padding(0, 0, 0, 2)
        flowOperations.Name = "flowOperations"
        flowOperations.Size = New System.Drawing.Size(1238, 91)
        flowOperations.TabIndex = 5
        flowOperations.WrapContents = False
        ' 
        ' rbOpCombine
        ' 
        rbOpCombine.Appearance = System.Windows.Forms.Appearance.Button
        rbOpCombine.Checked = True
        rbOpCombine.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpCombine.Location = New System.Drawing.Point(3, 3)
        rbOpCombine.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpCombine.Name = "rbOpCombine"
        rbOpCombine.Size = New System.Drawing.Size(158, 85)
        rbOpCombine.TabIndex = 0
        rbOpCombine.TabStop = True
        rbOpCombine.Tag = "toggle"
        rbOpCombine.Text = "Combine"
        rbOpCombine.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpCombine.UseVisualStyleBackColor = True
        ' 
        ' rbOpMerge
        ' 
        rbOpMerge.Appearance = System.Windows.Forms.Appearance.Button
        rbOpMerge.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpMerge.Location = New System.Drawing.Point(179, 3)
        rbOpMerge.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpMerge.Name = "rbOpMerge"
        rbOpMerge.Size = New System.Drawing.Size(158, 85)
        rbOpMerge.TabIndex = 1
        rbOpMerge.Tag = "toggle"
        rbOpMerge.Text = "Merge Pages"
        rbOpMerge.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpMerge.UseVisualStyleBackColor = True
        ' 
        ' rbOpExtract
        ' 
        rbOpExtract.Appearance = System.Windows.Forms.Appearance.Button
        rbOpExtract.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpExtract.Location = New System.Drawing.Point(355, 3)
        rbOpExtract.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpExtract.Name = "rbOpExtract"
        rbOpExtract.Size = New System.Drawing.Size(158, 85)
        rbOpExtract.TabIndex = 2
        rbOpExtract.Tag = "toggle"
        rbOpExtract.Text = "Extract Pages"
        rbOpExtract.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpExtract.UseVisualStyleBackColor = True
        ' 
        ' rbOpEncrypt
        ' 
        rbOpEncrypt.Appearance = System.Windows.Forms.Appearance.Button
        rbOpEncrypt.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpEncrypt.Location = New System.Drawing.Point(531, 3)
        rbOpEncrypt.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpEncrypt.Name = "rbOpEncrypt"
        rbOpEncrypt.Size = New System.Drawing.Size(158, 85)
        rbOpEncrypt.TabIndex = 3
        rbOpEncrypt.Tag = "toggle"
        rbOpEncrypt.Text = "Encrypt / Decrypt"
        rbOpEncrypt.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpEncrypt.UseVisualStyleBackColor = True
        ' 
        ' rbOpRotate
        ' 
        rbOpRotate.Appearance = System.Windows.Forms.Appearance.Button
        rbOpRotate.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpRotate.Location = New System.Drawing.Point(707, 3)
        rbOpRotate.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpRotate.Name = "rbOpRotate"
        rbOpRotate.Size = New System.Drawing.Size(158, 85)
        rbOpRotate.TabIndex = 4
        rbOpRotate.Tag = "toggle"
        rbOpRotate.Text = "Rotate"
        rbOpRotate.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpRotate.UseVisualStyleBackColor = True
        ' 
        ' rbOpStamp
        ' 
        rbOpStamp.Appearance = System.Windows.Forms.Appearance.Button
        rbOpStamp.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpStamp.Location = New System.Drawing.Point(883, 3)
        rbOpStamp.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpStamp.Name = "rbOpStamp"
        rbOpStamp.Size = New System.Drawing.Size(158, 85)
        rbOpStamp.TabIndex = 5
        rbOpStamp.Tag = "toggle"
        rbOpStamp.Text = "Stamp && Background"
        rbOpStamp.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpStamp.UseVisualStyleBackColor = True
        ' 
        ' rbOpCompress
        ' 
        rbOpCompress.Appearance = System.Windows.Forms.Appearance.Button
        rbOpCompress.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpCompress.Location = New System.Drawing.Point(1059, 3)
        rbOpCompress.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpCompress.Name = "rbOpCompress"
        rbOpCompress.Size = New System.Drawing.Size(158, 85)
        rbOpCompress.TabIndex = 6
        rbOpCompress.Tag = "toggle"
        rbOpCompress.Text = "Reduce Size"
        rbOpCompress.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpCompress.UseVisualStyleBackColor = True
        ' 
        ' rbOpArrange
        ' 
        rbOpArrange.Appearance = System.Windows.Forms.Appearance.Button
        rbOpArrange.Font = New System.Drawing.Font("Segoe UI Semibold", 10F)
        rbOpArrange.Margin = New System.Windows.Forms.Padding(3, 3, 9, 3)
        rbOpArrange.Name = "rbOpArrange"
        rbOpArrange.Size = New System.Drawing.Size(158, 85)
        rbOpArrange.TabIndex = 7
        rbOpArrange.Tag = "toggle"
        rbOpArrange.Text = "Arrange Pages"
        rbOpArrange.TextAlign = Drawing.ContentAlignment.MiddleCenter
        rbOpArrange.UseVisualStyleBackColor = True
        ' 
        ' tabArrange
        ' 
        tabArrange.Controls.Add(lblArrangeInfo)
        tabArrange.Controls.Add(btnShowPages)
        tabArrange.Controls.Add(btnArrangeReset)
        tabArrange.Controls.Add(lblArrangeSummary)
        tabArrange.Location = New System.Drawing.Point(4, 29)
        tabArrange.Name = "tabArrange"
        tabArrange.Padding = New System.Windows.Forms.Padding(3)
        tabArrange.Size = New System.Drawing.Size(1224, 148)
        tabArrange.TabIndex = 7
        tabArrange.Text = "Arrange Pages"
        ' 
        ' lblArrangeInfo
        ' 
        lblArrangeInfo.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblArrangeInfo.Location = New System.Drawing.Point(13, 11)
        lblArrangeInfo.Name = "lblArrangeInfo"
        lblArrangeInfo.Size = New System.Drawing.Size(1199, 48)
        lblArrangeInfo.TabIndex = 0
        lblArrangeInfo.Text = "See every page of the PDFs in the list as a small picture. Drag pages into a new order and remove the ones you don't want, then save them as one new PDF."
        ' 
        ' btnShowPages
        ' 
        btnShowPages.Font = New System.Drawing.Font("Segoe UI Semibold", 10.5F)
        btnShowPages.Location = New System.Drawing.Point(13, 64)
        btnShowPages.Name = "btnShowPages"
        btnShowPages.Size = New System.Drawing.Size(290, 48)
        btnShowPages.TabIndex = 1
        btnShowPages.Text = "Show Page Thumbnails..."
        btnShowPages.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        btnShowPages.UseVisualStyleBackColor = True
        ' 
        ' btnArrangeReset
        ' 
        btnArrangeReset.Location = New System.Drawing.Point(315, 64)
        btnArrangeReset.Name = "btnArrangeReset"
        btnArrangeReset.Size = New System.Drawing.Size(160, 48)
        btnArrangeReset.TabIndex = 2
        btnArrangeReset.Text = "Start Over"
        btnArrangeReset.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        btnArrangeReset.UseVisualStyleBackColor = True
        ' 
        ' lblArrangeSummary
        ' 
        lblArrangeSummary.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblArrangeSummary.Font = New System.Drawing.Font("Segoe UI", 10F)
        lblArrangeSummary.Location = New System.Drawing.Point(490, 75)
        lblArrangeSummary.Name = "lblArrangeSummary"
        lblArrangeSummary.Size = New System.Drawing.Size(720, 28)
        lblArrangeSummary.TabIndex = 3
        ' 
        ' pnlEncryptMode
        ' 
        pnlEncryptMode.Controls.Add(lblEncryptMode)
        pnlEncryptMode.Controls.Add(rbAddPassword)
        pnlEncryptMode.Controls.Add(rbRemovePassword)
        pnlEncryptMode.Location = New System.Drawing.Point(13, 8)
        pnlEncryptMode.Name = "pnlEncryptMode"
        pnlEncryptMode.Size = New System.Drawing.Size(900, 32)
        pnlEncryptMode.TabIndex = 12
        ' 
        ' lblEncryptMode
        ' 
        lblEncryptMode.AutoSize = True
        lblEncryptMode.Font = New System.Drawing.Font("Segoe UI Semibold", 9F)
        lblEncryptMode.Location = New System.Drawing.Point(0, 5)
        lblEncryptMode.Name = "lblEncryptMode"
        lblEncryptMode.Size = New System.Drawing.Size(85, 20)
        lblEncryptMode.TabIndex = 0
        lblEncryptMode.Text = "What to do:"
        ' 
        ' rbAddPassword
        ' 
        rbAddPassword.AutoSize = True
        rbAddPassword.Checked = True
        rbAddPassword.Location = New System.Drawing.Point(146, 3)
        rbAddPassword.Name = "rbAddPassword"
        rbAddPassword.Size = New System.Drawing.Size(197, 24)
        rbAddPassword.TabIndex = 1
        rbAddPassword.TabStop = True
        rbAddPassword.Text = "&Add a password (encrypt)"
        rbAddPassword.UseVisualStyleBackColor = True
        ' 
        ' rbRemovePassword
        ' 
        rbRemovePassword.AutoSize = True
        rbRemovePassword.Location = New System.Drawing.Point(402, 3)
        rbRemovePassword.Name = "rbRemovePassword"
        rbRemovePassword.Size = New System.Drawing.Size(250, 24)
        rbRemovePassword.TabIndex = 2
        rbRemovePassword.Text = "&Remove the password (decrypt)"
        rbRemovePassword.UseVisualStyleBackColor = True
        ' 
        ' lblExtras
        ' 
        lblExtras.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblExtras.AutoSize = True
        lblExtras.Name = "lblExtras"
        lblExtras.TabIndex = 7
        lblExtras.Text = "Also:"
        ' 
        ' flowExtras
        ' 
        flowExtras.AutoSize = True
        flowExtras.Controls.Add(chkExtraPassword)
        flowExtras.Controls.Add(btnExtraPassword)
        flowExtras.Controls.Add(chkExtraStamp)
        flowExtras.Controls.Add(btnExtraStamp)
        flowExtras.Controls.Add(lblExtrasSummary)
        flowExtras.Dock = System.Windows.Forms.DockStyle.Fill
        flowExtras.Margin = New System.Windows.Forms.Padding(0, 2, 0, 0)
        flowExtras.Name = "flowExtras"
        flowExtras.TabIndex = 8
        flowExtras.WrapContents = False
        ' 
        ' chkExtraPassword
        ' 
        chkExtraPassword.AutoSize = True
        chkExtraPassword.Margin = New System.Windows.Forms.Padding(3, 10, 3, 3)
        chkExtraPassword.Name = "chkExtraPassword"
        chkExtraPassword.TabIndex = 0
        chkExtraPassword.Text = "Protect with a password"
        chkExtraPassword.UseVisualStyleBackColor = True
        ' 
        ' btnExtraPassword
        ' 
        btnExtraPassword.AutoSize = True
        btnExtraPassword.MinimumSize = New System.Drawing.Size(118, 40)
        btnExtraPassword.Name = "btnExtraPassword"
        btnExtraPassword.TabIndex = 1
        btnExtraPassword.Text = "Password..."
        btnExtraPassword.UseVisualStyleBackColor = True
        ' 
        ' chkExtraStamp
        ' 
        chkExtraStamp.AutoSize = True
        chkExtraStamp.Margin = New System.Windows.Forms.Padding(30, 10, 3, 3)
        chkExtraStamp.Name = "chkExtraStamp"
        chkExtraStamp.TabIndex = 2
        chkExtraStamp.Text = "Add a stamp / background"
        chkExtraStamp.UseVisualStyleBackColor = True
        ' 
        ' btnExtraStamp
        ' 
        btnExtraStamp.AutoSize = True
        btnExtraStamp.MinimumSize = New System.Drawing.Size(118, 40)
        btnExtraStamp.Name = "btnExtraStamp"
        btnExtraStamp.TabIndex = 3
        btnExtraStamp.Text = "Stamp && Background..."
        btnExtraStamp.UseVisualStyleBackColor = True
        ' 
        ' lblExtrasSummary
        ' 
        lblExtrasSummary.AutoSize = True
        lblExtrasSummary.Margin = New System.Windows.Forms.Padding(16, 11, 3, 3)
        lblExtrasSummary.Name = "lblExtrasSummary"
        lblExtrasSummary.TabIndex = 4
        lblExtrasSummary.Tag = "muted"
        ' 
        ' tabRotate
        ' 
        tabRotate.Controls.Add(lblRotateInfo)
        tabRotate.Controls.Add(pnlRotateAngle)
        tabRotate.Controls.Add(lblRotatePages)
        tabRotate.Controls.Add(txtRotatePages)
        tabRotate.Controls.Add(lblRotateHint)
        tabRotate.Controls.Add(lnkRotateRangeHelp)
        tabRotate.Location = New System.Drawing.Point(4, 29)
        tabRotate.Name = "tabRotate"
        tabRotate.Padding = New System.Windows.Forms.Padding(3)
        tabRotate.Size = New System.Drawing.Size(1224, 148)
        tabRotate.TabIndex = 4
        tabRotate.Text = "Rotate"
        ' 
        ' lblRotateInfo
        ' 
        lblRotateInfo.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblRotateInfo.Location = New System.Drawing.Point(13, 13)
        lblRotateInfo.Name = "lblRotateInfo"
        lblRotateInfo.Size = New System.Drawing.Size(1047, 24)
        lblRotateInfo.TabIndex = 0
        lblRotateInfo.Text = "Turns pages the right way up. Every PDF in the list gets its own rotated copy; your original files aren't changed."
        ' 
        ' pnlRotateAngle
        ' 
        pnlRotateAngle.Controls.Add(lblRotateTurn)
        pnlRotateAngle.Controls.Add(rbRotateRight)
        pnlRotateAngle.Controls.Add(rbRotateLeft)
        pnlRotateAngle.Controls.Add(rbRotate180)
        pnlRotateAngle.Location = New System.Drawing.Point(13, 46)
        pnlRotateAngle.Name = "pnlRotateAngle"
        pnlRotateAngle.Size = New System.Drawing.Size(982, 33)
        pnlRotateAngle.TabIndex = 1
        ' 
        ' lblRotateTurn
        ' 
        lblRotateTurn.AutoSize = True
        lblRotateTurn.Location = New System.Drawing.Point(0, 7)
        lblRotateTurn.Name = "lblRotateTurn"
        lblRotateTurn.Size = New System.Drawing.Size(110, 20)
        lblRotateTurn.TabIndex = 0
        lblRotateTurn.Text = "Turn the pages:"
        ' 
        ' rbRotateRight
        ' 
        rbRotateRight.AutoSize = True
        rbRotateRight.Checked = True
        rbRotateRight.Location = New System.Drawing.Point(146, 4)
        rbRotateRight.Name = "rbRotateRight"
        rbRotateRight.Size = New System.Drawing.Size(119, 24)
        rbRotateRight.TabIndex = 1
        rbRotateRight.TabStop = True
        rbRotateRight.Text = "90° clockwise"
        rbRotateRight.UseVisualStyleBackColor = True
        ' 
        ' rbRotateLeft
        ' 
        rbRotateLeft.AutoSize = True
        rbRotateLeft.Location = New System.Drawing.Point(316, 4)
        rbRotateLeft.Name = "rbRotateLeft"
        rbRotateLeft.Size = New System.Drawing.Size(144, 24)
        rbRotateLeft.TabIndex = 2
        rbRotateLeft.Text = "90° anticlockwise"
        rbRotateLeft.UseVisualStyleBackColor = True
        ' 
        ' rbRotate180
        ' 
        rbRotate180.AutoSize = True
        rbRotate180.Location = New System.Drawing.Point(513, 4)
        rbRotate180.Name = "rbRotate180"
        rbRotate180.Size = New System.Drawing.Size(161, 24)
        rbRotate180.TabIndex = 3
        rbRotate180.Text = "Upside down (180°)"
        rbRotate180.UseVisualStyleBackColor = True
        ' 
        ' lblRotatePages
        ' 
        lblRotatePages.AutoSize = True
        lblRotatePages.Location = New System.Drawing.Point(13, 95)
        lblRotatePages.Name = "lblRotatePages"
        lblRotatePages.Size = New System.Drawing.Size(98, 20)
        lblRotatePages.TabIndex = 2
        lblRotatePages.Text = "Pages to turn:"
        ' 
        ' txtRotatePages
        ' 
        txtRotatePages.Location = New System.Drawing.Point(159, 92)
        txtRotatePages.Name = "txtRotatePages"
        txtRotatePages.PlaceholderText = "All"
        txtRotatePages.Size = New System.Drawing.Size(240, 27)
        txtRotatePages.TabIndex = 3
        ' 
        ' lblRotateHint
        ' 
        lblRotateHint.AutoSize = True
        lblRotateHint.Location = New System.Drawing.Point(412, 95)
        lblRotateHint.Name = "lblRotateHint"
        lblRotateHint.Size = New System.Drawing.Size(421, 20)
        lblRotateHint.TabIndex = 4
        lblRotateHint.Tag = "muted"
        lblRotateHint.Text = "Leave empty for every page, or type 1-3, 5, 8-end, odd or even"
        '
        ' lnkRotateRangeHelp
        '
        lnkRotateRangeHelp.AutoSize = True
        lnkRotateRangeHelp.Location = New System.Drawing.Point(850, 95)
        lnkRotateRangeHelp.Name = "lnkRotateRangeHelp"
        lnkRotateRangeHelp.TabIndex = 5
        lnkRotateRangeHelp.TabStop = True
        lnkRotateRangeHelp.Text = "Page range help"
        ' 
        ' tabStamp
        ' 
        tabStamp.Controls.Add(lblStampInfo)
        tabStamp.Controls.Add(pnlStampMode)
        tabStamp.Controls.Add(lblStampFile)
        tabStamp.Controls.Add(txtStampFile)
        tabStamp.Controls.Add(btnStampBrowse)
        tabStamp.Controls.Add(lblStampUse)
        tabStamp.Controls.Add(numStampPage)
        tabStamp.Controls.Add(lblBackgroundFile)
        tabStamp.Controls.Add(txtBackgroundFile)
        tabStamp.Controls.Add(btnBackgroundBrowse)
        tabStamp.Controls.Add(lblBackgroundUse)
        tabStamp.Controls.Add(numBackgroundPage)
        tabStamp.Controls.Add(lblStampPages)
        tabStamp.Controls.Add(txtStampPages)
        tabStamp.Controls.Add(lnkStampRangeHelp)
        tabStamp.Location = New System.Drawing.Point(4, 29)
        tabStamp.Name = "tabStamp"
        tabStamp.Padding = New System.Windows.Forms.Padding(3)
        tabStamp.Size = New System.Drawing.Size(1224, 189)
        tabStamp.TabIndex = 5
        tabStamp.Text = "Stamp & Background"
        ' 
        ' lblStampInfo
        ' 
        lblStampInfo.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblStampInfo.Location = New System.Drawing.Point(13, 11)
        lblStampInfo.Name = "lblStampInfo"
        lblStampInfo.Size = New System.Drawing.Size(1047, 44)
        lblStampInfo.TabIndex = 0
        lblStampInfo.Text = resources.GetString("lblStampInfo.Text")
        ' 
        ' pnlStampMode
        ' 
        pnlStampMode.Controls.Add(lblStampAs)
        pnlStampMode.Controls.Add(rbStampOver)
        pnlStampMode.Controls.Add(rbStampUnder)
        pnlStampMode.Controls.Add(rbStampBoth)
        pnlStampMode.Location = New System.Drawing.Point(13, 57)
        pnlStampMode.Name = "pnlStampMode"
        pnlStampMode.Size = New System.Drawing.Size(982, 31)
        pnlStampMode.TabIndex = 1
        ' 
        ' lblStampAs
        ' 
        lblStampAs.AutoSize = True
        lblStampAs.Location = New System.Drawing.Point(0, 5)
        lblStampAs.Name = "lblStampAs"
        lblStampAs.Size = New System.Drawing.Size(37, 20)
        lblStampAs.TabIndex = 0
        lblStampAs.Text = "Add:"
        ' 
        ' rbStampOver
        ' 
        rbStampOver.AutoSize = True
        rbStampOver.Checked = True
        rbStampOver.Location = New System.Drawing.Point(194, 3)
        rbStampOver.Name = "rbStampOver"
        rbStampOver.Size = New System.Drawing.Size(165, 24)
        rbStampOver.TabIndex = 1
        rbStampOver.TabStop = True
        rbStampOver.Text = "A stamp only, on top"
        rbStampOver.UseVisualStyleBackColor = True
        ' 
        ' rbStampUnder
        ' 
        rbStampUnder.AutoSize = True
        rbStampUnder.Location = New System.Drawing.Point(400, 3)
        rbStampUnder.Name = "rbStampUnder"
        rbStampUnder.Size = New System.Drawing.Size(210, 24)
        rbStampUnder.TabIndex = 2
        rbStampUnder.Text = "A background only, behind"
        rbStampUnder.UseVisualStyleBackColor = True
        ' 
        ' rbStampBoth
        ' 
        rbStampBoth.AutoSize = True
        rbStampBoth.Location = New System.Drawing.Point(650, 3)
        rbStampBoth.Name = "rbStampBoth"
        rbStampBoth.Size = New System.Drawing.Size(250, 24)
        rbStampBoth.TabIndex = 3
        rbStampBoth.Text = "Both a stamp and a background"
        rbStampBoth.UseVisualStyleBackColor = True
        ' 
        ' lblStampFile
        ' 
        lblStampFile.AutoSize = True
        lblStampFile.Location = New System.Drawing.Point(13, 99)
        lblStampFile.Name = "lblStampFile"
        lblStampFile.Size = New System.Drawing.Size(82, 20)
        lblStampFile.TabIndex = 2
        lblStampFile.Text = "Stamp PDF:"
        ' 
        ' txtStampFile
        ' 
        txtStampFile.Location = New System.Drawing.Point(207, 96)
        txtStampFile.Name = "txtStampFile"
        txtStampFile.PlaceholderText = "Choose a PDF with the stamp on it (logo, watermark, mark)"
        txtStampFile.ReadOnly = True
        txtStampFile.Size = New System.Drawing.Size(480, 27)
        txtStampFile.TabIndex = 3
        ' 
        ' btnStampBrowse
        ' 
        btnStampBrowse.AutoSize = True
        btnStampBrowse.Location = New System.Drawing.Point(698, 91)
        btnStampBrowse.MinimumSize = New System.Drawing.Size(129, 37)
        btnStampBrowse.Name = "btnStampBrowse"
        btnStampBrowse.Size = New System.Drawing.Size(129, 37)
        btnStampBrowse.TabIndex = 4
        btnStampBrowse.Text = "Choose…"
        btnStampBrowse.UseVisualStyleBackColor = True
        ' 
        ' lblStampUse
        ' 
        lblStampUse.AutoSize = True
        lblStampUse.Location = New System.Drawing.Point(845, 99)
        lblStampUse.Name = "lblStampUse"
        lblStampUse.Size = New System.Drawing.Size(70, 20)
        lblStampUse.TabIndex = 5
        lblStampUse.Text = "use page"
        ' 
        ' numStampPage
        ' 
        numStampPage.Location = New System.Drawing.Point(925, 96)
        numStampPage.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numStampPage.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numStampPage.Name = "numStampPage"
        numStampPage.Size = New System.Drawing.Size(65, 27)
        numStampPage.TabIndex = 6
        numStampPage.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' lblBackgroundFile
        ' 
        lblBackgroundFile.AutoSize = True
        lblBackgroundFile.Location = New System.Drawing.Point(13, 141)
        lblBackgroundFile.Name = "lblBackgroundFile"
        lblBackgroundFile.Size = New System.Drawing.Size(118, 20)
        lblBackgroundFile.TabIndex = 7
        lblBackgroundFile.Text = "Background PDF:"
        ' 
        ' txtBackgroundFile
        ' 
        txtBackgroundFile.Location = New System.Drawing.Point(207, 138)
        txtBackgroundFile.Name = "txtBackgroundFile"
        txtBackgroundFile.PlaceholderText = "Choose a PDF with the background on it (letterhead, design)"
        txtBackgroundFile.ReadOnly = True
        txtBackgroundFile.Size = New System.Drawing.Size(480, 27)
        txtBackgroundFile.TabIndex = 8
        ' 
        ' btnBackgroundBrowse
        ' 
        btnBackgroundBrowse.AutoSize = True
        btnBackgroundBrowse.Location = New System.Drawing.Point(698, 133)
        btnBackgroundBrowse.MinimumSize = New System.Drawing.Size(129, 37)
        btnBackgroundBrowse.Name = "btnBackgroundBrowse"
        btnBackgroundBrowse.Size = New System.Drawing.Size(129, 37)
        btnBackgroundBrowse.TabIndex = 9
        btnBackgroundBrowse.Text = "Choose…"
        btnBackgroundBrowse.UseVisualStyleBackColor = True
        ' 
        ' lblBackgroundUse
        ' 
        lblBackgroundUse.AutoSize = True
        lblBackgroundUse.Location = New System.Drawing.Point(845, 141)
        lblBackgroundUse.Name = "lblBackgroundUse"
        lblBackgroundUse.Size = New System.Drawing.Size(70, 20)
        lblBackgroundUse.TabIndex = 10
        lblBackgroundUse.Text = "use page"
        ' 
        ' numBackgroundPage
        ' 
        numBackgroundPage.Location = New System.Drawing.Point(925, 138)
        numBackgroundPage.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numBackgroundPage.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numBackgroundPage.Name = "numBackgroundPage"
        numBackgroundPage.Size = New System.Drawing.Size(65, 27)
        numBackgroundPage.TabIndex = 11
        numBackgroundPage.Value = New Decimal(New Integer() {1, 0, 0, 0})
        ' 
        ' lblStampPages
        ' 
        lblStampPages.AutoSize = True
        lblStampPages.Location = New System.Drawing.Point(13, 183)
        lblStampPages.Name = "lblStampPages"
        lblStampPages.Size = New System.Drawing.Size(135, 20)
        lblStampPages.TabIndex = 12
        lblStampPages.Text = "Put them on pages:"
        ' 
        ' txtStampPages
        ' 
        txtStampPages.Location = New System.Drawing.Point(207, 180)
        txtStampPages.Name = "txtStampPages"
        txtStampPages.PlaceholderText = "All"
        txtStampPages.Size = New System.Drawing.Size(163, 27)
        txtStampPages.TabIndex = 13
        '
        ' lnkStampRangeHelp
        '
        lnkStampRangeHelp.AutoSize = True
        lnkStampRangeHelp.Location = New System.Drawing.Point(384, 183)
        lnkStampRangeHelp.Name = "lnkStampRangeHelp"
        lnkStampRangeHelp.TabIndex = 14
        lnkStampRangeHelp.TabStop = True
        lnkStampRangeHelp.Text = "Page range help"
        ' 
        ' tabCompress
        ' 
        tabCompress.Controls.Add(lblCompressInfo)
        tabCompress.Controls.Add(pnlCompressLevel)
        tabCompress.Location = New System.Drawing.Point(4, 29)
        tabCompress.Name = "tabCompress"
        tabCompress.Padding = New System.Windows.Forms.Padding(3)
        tabCompress.Size = New System.Drawing.Size(1224, 148)
        tabCompress.TabIndex = 6
        tabCompress.Text = "Reduce Size"
        ' 
        ' lblCompressInfo
        ' 
        lblCompressInfo.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblCompressInfo.Location = New System.Drawing.Point(13, 11)
        lblCompressInfo.Name = "lblCompressInfo"
        lblCompressInfo.Size = New System.Drawing.Size(1047, 24)
        lblCompressInfo.TabIndex = 0
        lblCompressInfo.Text = "Makes a smaller copy of every PDF in the list. Text and page layout stay the same; stronger levels save photos and scans at lower quality."
        ' 
        ' pnlCompressLevel
        ' 
        pnlCompressLevel.Controls.Add(rbCompressNormal)
        pnlCompressLevel.Controls.Add(rbCompressMedium)
        pnlCompressLevel.Controls.Add(rbCompressHigh)
        pnlCompressLevel.Controls.Add(rbCompressExtreme)
        pnlCompressLevel.Location = New System.Drawing.Point(13, 39)
        pnlCompressLevel.Name = "pnlCompressLevel"
        pnlCompressLevel.Size = New System.Drawing.Size(1047, 113)
        pnlCompressLevel.TabIndex = 1
        ' 
        ' rbCompressNormal
        ' 
        rbCompressNormal.AutoSize = True
        rbCompressNormal.Location = New System.Drawing.Point(0, 2)
        rbCompressNormal.Name = "rbCompressNormal"
        rbCompressNormal.Size = New System.Drawing.Size(357, 24)
        rbCompressNormal.TabIndex = 0
        rbCompressNormal.Text = "Normal: repacks the file with no change in quality"
        rbCompressNormal.UseVisualStyleBackColor = True
        ' 
        ' rbCompressMedium
        ' 
        rbCompressMedium.AutoSize = True
        rbCompressMedium.Checked = True
        rbCompressMedium.Location = New System.Drawing.Point(0, 29)
        rbCompressMedium.Name = "rbCompressMedium"
        rbCompressMedium.Size = New System.Drawing.Size(388, 24)
        rbCompressMedium.TabIndex = 1
        rbCompressMedium.TabStop = True
        rbCompressMedium.Text = "Medium: photos kept at good quality (recommended)"
        rbCompressMedium.UseVisualStyleBackColor = True
        ' 
        ' rbCompressHigh
        ' 
        rbCompressHigh.AutoSize = True
        rbCompressHigh.Location = New System.Drawing.Point(0, 57)
        rbCompressHigh.Name = "rbCompressHigh"
        rbCompressHigh.Size = New System.Drawing.Size(334, 24)
        rbCompressHigh.TabIndex = 2
        rbCompressHigh.Text = "High: photos at lower quality for a smaller file"
        rbCompressHigh.UseVisualStyleBackColor = True
        ' 
        ' rbCompressExtreme
        ' 
        rbCompressExtreme.AutoSize = True
        rbCompressExtreme.Location = New System.Drawing.Point(0, 84)
        rbCompressExtreme.Name = "rbCompressExtreme"
        rbCompressExtreme.Size = New System.Drawing.Size(810, 24)
        rbCompressExtreme.TabIndex = 3
        rbCompressExtreme.Text = "Extreme: smallest file; photos look visibly softer, and document details, page labels and accessibility tags are removed"
        rbCompressExtreme.UseVisualStyleBackColor = True
        ' 
        ' rootLayout
        ' 
        rootLayout.ColumnCount = 1
        rootLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rootLayout.Controls.Add(pnlQpdfMissing, 0, 0)
        rootLayout.Controls.Add(lblStep1, 0, 1)
        rootLayout.Controls.Add(flowToolbar, 0, 2)
        rootLayout.Controls.Add(grid, 0, 3)
        rootLayout.Controls.Add(lblStep2, 0, 4)
        rootLayout.Controls.Add(flowOperations, 0, 5)
        rootLayout.Controls.Add(tabOperations, 0, 6)
        rootLayout.Controls.Add(lblStep3, 0, 7)
        rootLayout.Controls.Add(tblOutput, 0, 8)
        rootLayout.Controls.Add(tblActions, 0, 9)
        rootLayout.Controls.Add(txtLog, 0, 10)
        rootLayout.Dock = System.Windows.Forms.DockStyle.Fill
        rootLayout.Location = New System.Drawing.Point(0, 70)
        rootLayout.Name = "rootLayout"
        rootLayout.Padding = New System.Windows.Forms.Padding(15, 4, 15, 9)
        rootLayout.RowCount = 11
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F))
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.RowStyles.Add(New System.Windows.Forms.RowStyle())
        rootLayout.Size = New System.Drawing.Size(1268, 733)
        rootLayout.TabIndex = 1
        ' 
        ' pnlQpdfMissing
        ' 
        pnlQpdfMissing.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        pnlQpdfMissing.AutoSize = True
        pnlQpdfMissing.BackColor = Drawing.Color.FromArgb(CByte(255), CByte(244), CByte(206))
        pnlQpdfMissing.Controls.Add(lblQpdfMissing)
        pnlQpdfMissing.Controls.Add(btnLocateQpdf)
        pnlQpdfMissing.Controls.Add(lnkDownloadQpdf)
        pnlQpdfMissing.Location = New System.Drawing.Point(18, 7)
        pnlQpdfMissing.Name = "pnlQpdfMissing"
        pnlQpdfMissing.Padding = New System.Windows.Forms.Padding(7, 3, 7, 3)
        pnlQpdfMissing.Size = New System.Drawing.Size(1232, 56)
        pnlQpdfMissing.TabIndex = 0
        pnlQpdfMissing.Visible = False
        ' 
        ' lblQpdfMissing
        ' 
        lblQpdfMissing.AutoSize = True
        lblQpdfMissing.Location = New System.Drawing.Point(10, 14)
        lblQpdfMissing.Margin = New System.Windows.Forms.Padding(3, 11, 13, 3)
        lblQpdfMissing.Name = "lblQpdfMissing"
        lblQpdfMissing.Size = New System.Drawing.Size(378, 20)
        lblQpdfMissing.TabIndex = 0
        lblQpdfMissing.Text = "QPDF wasn't found. This app needs it to work with PDFs."
        ' 
        ' btnLocateQpdf
        ' 
        btnLocateQpdf.AutoSize = True
        btnLocateQpdf.Location = New System.Drawing.Point(404, 6)
        btnLocateQpdf.MinimumSize = New System.Drawing.Size(142, 44)
        btnLocateQpdf.Name = "btnLocateQpdf"
        btnLocateQpdf.Size = New System.Drawing.Size(142, 44)
        btnLocateQpdf.TabIndex = 1
        btnLocateQpdf.Text = "Find QPDF…"
        btnLocateQpdf.UseVisualStyleBackColor = True
        ' 
        ' lnkDownloadQpdf
        ' 
        lnkDownloadQpdf.AutoSize = True
        lnkDownloadQpdf.Location = New System.Drawing.Point(559, 14)
        lnkDownloadQpdf.Margin = New System.Windows.Forms.Padding(10, 11, 3, 3)
        lnkDownloadQpdf.Name = "lnkDownloadQpdf"
        lnkDownloadQpdf.Size = New System.Drawing.Size(119, 20)
        lnkDownloadQpdf.TabIndex = 2
        lnkDownloadQpdf.TabStop = True
        lnkDownloadQpdf.Text = "Download QPDF"
        ' 
        ' lblStep1
        ' 
        lblStep1.AutoSize = True
        lblStep1.Font = New System.Drawing.Font("Segoe UI Semibold", 11F)
        lblStep1.Location = New System.Drawing.Point(18, 70)
        lblStep1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 2)
        lblStep1.Name = "lblStep1"
        lblStep1.Size = New System.Drawing.Size(157, 25)
        lblStep1.TabIndex = 1
        lblStep1.Text = "1. Add your PDFs"
        ' 
        ' flowToolbar
        ' 
        flowToolbar.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        flowToolbar.AutoSize = True
        flowToolbar.Controls.Add(btnAddPdf)
        flowToolbar.Controls.Add(btnAddFolder)
        flowToolbar.Controls.Add(chkSubfolders)
        flowToolbar.Controls.Add(btnMoveUp)
        flowToolbar.Controls.Add(btnMoveDown)
        flowToolbar.Controls.Add(btnSort)
        flowToolbar.Controls.Add(btnRemove)
        flowToolbar.Controls.Add(btnClear)
        flowToolbar.Location = New System.Drawing.Point(15, 97)
        flowToolbar.Margin = New System.Windows.Forms.Padding(0)
        flowToolbar.Name = "flowToolbar"
        flowToolbar.Size = New System.Drawing.Size(1238, 50)
        flowToolbar.TabIndex = 2
        ' 
        ' btnAddPdf
        ' 
        btnAddPdf.AutoSize = True
        btnAddPdf.Location = New System.Drawing.Point(3, 3)
        btnAddPdf.MinimumSize = New System.Drawing.Size(135, 44)
        btnAddPdf.Name = "btnAddPdf"
        btnAddPdf.Size = New System.Drawing.Size(135, 44)
        btnAddPdf.TabIndex = 0
        btnAddPdf.Text = "&Add PDF…"
        btnAddPdf.UseVisualStyleBackColor = True
        ' 
        ' btnAddFolder
        ' 
        btnAddFolder.AutoSize = True
        btnAddFolder.Location = New System.Drawing.Point(144, 3)
        btnAddFolder.MinimumSize = New System.Drawing.Size(135, 44)
        btnAddFolder.Name = "btnAddFolder"
        btnAddFolder.Size = New System.Drawing.Size(135, 44)
        btnAddFolder.TabIndex = 1
        btnAddFolder.Text = "Add &Folder…"
        btnAddFolder.UseVisualStyleBackColor = True
        ' 
        ' chkSubfolders
        ' 
        chkSubfolders.AutoSize = True
        chkSubfolders.Location = New System.Drawing.Point(289, 10)
        chkSubfolders.Margin = New System.Windows.Forms.Padding(7, 10, 20, 3)
        chkSubfolders.Name = "chkSubfolders"
        chkSubfolders.Size = New System.Drawing.Size(152, 24)
        chkSubfolders.TabIndex = 2
        chkSubfolders.Text = "Include subfolders"
        chkSubfolders.UseVisualStyleBackColor = True
        ' 
        ' btnMoveUp
        ' 
        btnMoveUp.AutoSize = True
        btnMoveUp.Location = New System.Drawing.Point(464, 3)
        btnMoveUp.MinimumSize = New System.Drawing.Size(129, 44)
        btnMoveUp.Name = "btnMoveUp"
        btnMoveUp.Size = New System.Drawing.Size(129, 44)
        btnMoveUp.TabIndex = 3
        btnMoveUp.Text = "Move &Up"
        btnMoveUp.UseVisualStyleBackColor = True
        ' 
        ' btnMoveDown
        ' 
        btnMoveDown.AutoSize = True
        btnMoveDown.Location = New System.Drawing.Point(599, 3)
        btnMoveDown.MinimumSize = New System.Drawing.Size(129, 44)
        btnMoveDown.Name = "btnMoveDown"
        btnMoveDown.Size = New System.Drawing.Size(129, 44)
        btnMoveDown.TabIndex = 4
        btnMoveDown.Text = "Move &Down"
        btnMoveDown.UseVisualStyleBackColor = True
        ' 
        ' btnSort
        ' 
        btnSort.AutoSize = True
        btnSort.Location = New System.Drawing.Point(734, 3)
        btnSort.MinimumSize = New System.Drawing.Size(118, 44)
        btnSort.Name = "btnSort"
        btnSort.Size = New System.Drawing.Size(118, 44)
        btnSort.TabIndex = 5
        btnSort.Text = "Sort A–Z"
        btnSort.UseVisualStyleBackColor = True
        ' 
        ' btnRemove
        ' 
        btnRemove.AutoSize = True
        btnRemove.Location = New System.Drawing.Point(858, 3)
        btnRemove.MinimumSize = New System.Drawing.Size(118, 44)
        btnRemove.Name = "btnRemove"
        btnRemove.Size = New System.Drawing.Size(118, 44)
        btnRemove.TabIndex = 6
        btnRemove.Text = "&Remove"
        btnRemove.UseVisualStyleBackColor = True
        ' 
        ' btnClear
        ' 
        btnClear.AutoSize = True
        btnClear.Location = New System.Drawing.Point(982, 3)
        btnClear.MinimumSize = New System.Drawing.Size(118, 44)
        btnClear.Name = "btnClear"
        btnClear.Size = New System.Drawing.Size(118, 44)
        btnClear.TabIndex = 7
        btnClear.Text = "Clear All"
        btnClear.UseVisualStyleBackColor = True
        ' 
        ' grid
        ' 
        grid.AllowDrop = True
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.AllowUserToResizeRows = False
        grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        grid.BackgroundColor = Drawing.Color.White
        grid.ColumnHeadersHeight = 26
        grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        grid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {colUse, colPosition, colFileName, colPageCount, colPagesToUse, colSize, colStatus, colFolder})
        grid.Dock = System.Windows.Forms.DockStyle.Fill
        grid.Location = New System.Drawing.Point(18, 150)
        grid.MinimumSize = New System.Drawing.Size(0, 120)
        grid.Name = "grid"
        grid.RowHeadersVisible = False
        grid.RowHeadersWidth = 47
        grid.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        grid.Size = New System.Drawing.Size(1232, 120)
        grid.TabIndex = 3
        ' 
        ' colUse
        ' 
        colUse.DataPropertyName = "Included"
        colUse.FillWeight = 5F
        colUse.HeaderText = "Use"
        colUse.MinimumWidth = 40
        colUse.Name = "colUse"
        colUse.Visible = False
        ' 
        ' colPosition
        ' 
        colPosition.DataPropertyName = "Position"
        colPosition.FillWeight = 4F
        colPosition.HeaderText = "#"
        colPosition.MinimumWidth = 34
        colPosition.Name = "colPosition"
        colPosition.ReadOnly = True
        colPosition.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        ' 
        ' colFileName
        ' 
        colFileName.DataPropertyName = "FileName"
        colFileName.FillWeight = 24F
        colFileName.HeaderText = "File Name"
        colFileName.MinimumWidth = 120
        colFileName.Name = "colFileName"
        colFileName.ReadOnly = True
        colFileName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        ' 
        ' colPageCount
        ' 
        colPageCount.DataPropertyName = "PageCountText"
        colPageCount.FillWeight = 6F
        colPageCount.HeaderText = "Pages"
        colPageCount.MinimumWidth = 50
        colPageCount.Name = "colPageCount"
        colPageCount.ReadOnly = True
        colPageCount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        ' 
        ' colPagesToUse
        ' 
        colPagesToUse.DataPropertyName = "PagesText"
        colPagesToUse.FillWeight = 16F
        colPagesToUse.HeaderText = "Pages to Use"
        colPagesToUse.MinimumWidth = 100
        colPagesToUse.Name = "colPagesToUse"
        colPagesToUse.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        colPagesToUse.Visible = False
        ' 
        ' colSize
        ' 
        colSize.DataPropertyName = "SizeText"
        colSize.FillWeight = 7F
        colSize.HeaderText = "Size"
        colSize.MinimumWidth = 60
        colSize.Name = "colSize"
        colSize.ReadOnly = True
        colSize.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        ' 
        ' colStatus
        ' 
        colStatus.DataPropertyName = "StatusText"
        colStatus.FillWeight = 12F
        colStatus.HeaderText = "Status"
        colStatus.MinimumWidth = 90
        colStatus.Name = "colStatus"
        colStatus.ReadOnly = True
        colStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        ' 
        ' colFolder
        ' 
        colFolder.DataPropertyName = "Folder"
        colFolder.FillWeight = 25F
        colFolder.HeaderText = "Folder"
        colFolder.MinimumWidth = 80
        colFolder.Name = "colFolder"
        colFolder.ReadOnly = True
        colFolder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        ' 
        ' lblStep2
        ' 
        lblStep2.AutoSize = True
        lblStep2.Font = New System.Drawing.Font("Segoe UI Semibold", 11F)
        lblStep2.Location = New System.Drawing.Point(18, 90)
        lblStep2.Margin = New System.Windows.Forms.Padding(3, 9, 3, 2)
        lblStep2.Name = "lblStep2"
        lblStep2.Size = New System.Drawing.Size(193, 25)
        lblStep2.TabIndex = 4
        lblStep2.Text = "2. Choose what to do"
        ' 
        ' tabOperations
        ' 
        tabOperations.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        tabOperations.Controls.Add(tabCombine)
        tabOperations.Controls.Add(tabMerge)
        tabOperations.Controls.Add(tabExtract)
        tabOperations.Controls.Add(tabEncrypt)
        tabOperations.Controls.Add(tabRotate)
        tabOperations.Controls.Add(tabStamp)
        tabOperations.Controls.Add(tabCompress)
        tabOperations.Controls.Add(tabArrange)
        tabOperations.Location = New System.Drawing.Point(18, 213)
        tabOperations.Name = "tabOperations"
        tabOperations.SelectedIndex = 0
        tabOperations.Size = New System.Drawing.Size(1232, 222)
        tabOperations.TabIndex = 5
        ' 
        ' tabCombine
        ' 
        tabCombine.Controls.Add(lblCombineInfo)
        tabCombine.Controls.Add(chkKeepDetails)
        tabCombine.Location = New System.Drawing.Point(4, 29)
        tabCombine.Name = "tabCombine"
        tabCombine.Padding = New System.Windows.Forms.Padding(3)
        tabCombine.Size = New System.Drawing.Size(1224, 148)
        tabCombine.TabIndex = 0
        tabCombine.Text = "Combine"
        tabCombine.UseVisualStyleBackColor = True
        ' 
        ' lblCombineInfo
        ' 
        lblCombineInfo.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblCombineInfo.Location = New System.Drawing.Point(13, 13)
        lblCombineInfo.Name = "lblCombineInfo"
        lblCombineInfo.Size = New System.Drawing.Size(1199, 44)
        lblCombineInfo.TabIndex = 0
        lblCombineInfo.Text = "Joins all the pages of every PDF in the list into one new PDF, in the order shown above. Use Move Up and Move Down to change the order."
        ' 
        ' chkKeepDetails
        ' 
        chkKeepDetails.AutoSize = True
        chkKeepDetails.Location = New System.Drawing.Point(13, 63)
        chkKeepDetails.Name = "chkKeepDetails"
        chkKeepDetails.Size = New System.Drawing.Size(473, 24)
        chkKeepDetails.TabIndex = 1
        chkKeepDetails.Text = "Keep the first PDF's bookmarks and document details (title, author)"
        chkKeepDetails.UseVisualStyleBackColor = True
        ' 
        ' tabMerge
        ' 
        tabMerge.Controls.Add(lblMergeInfo)
        tabMerge.Controls.Add(btnUseAll)
        tabMerge.Controls.Add(btnUseNone)
        tabMerge.Controls.Add(lblMergeSummary)
        tabMerge.Controls.Add(lnkMergeRangeHelp)
        tabMerge.Location = New System.Drawing.Point(4, 29)
        tabMerge.Name = "tabMerge"
        tabMerge.Padding = New System.Windows.Forms.Padding(3)
        tabMerge.Size = New System.Drawing.Size(1224, 148)
        tabMerge.TabIndex = 1
        tabMerge.Text = "Merge Pages"
        tabMerge.UseVisualStyleBackColor = True
        ' 
        ' lblMergeInfo
        ' 
        lblMergeInfo.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblMergeInfo.Location = New System.Drawing.Point(13, 13)
        lblMergeInfo.Name = "lblMergeInfo"
        lblMergeInfo.Size = New System.Drawing.Size(1047, 61)
        lblMergeInfo.TabIndex = 0
        lblMergeInfo.Text = resources.GetString("lblMergeInfo.Text")
        ' 
        ' btnUseAll
        ' 
        btnUseAll.AutoSize = True
        btnUseAll.Location = New System.Drawing.Point(13, 83)
        btnUseAll.MinimumSize = New System.Drawing.Size(129, 44)
        btnUseAll.Name = "btnUseAll"
        btnUseAll.Size = New System.Drawing.Size(129, 44)
        btnUseAll.TabIndex = 1
        btnUseAll.Text = "Tick All"
        btnUseAll.UseVisualStyleBackColor = True
        ' 
        ' btnUseNone
        ' 
        btnUseNone.AutoSize = True
        btnUseNone.Location = New System.Drawing.Point(153, 83)
        btnUseNone.MinimumSize = New System.Drawing.Size(129, 44)
        btnUseNone.Name = "btnUseNone"
        btnUseNone.Size = New System.Drawing.Size(129, 44)
        btnUseNone.TabIndex = 2
        btnUseNone.Text = "Untick All"
        btnUseNone.UseVisualStyleBackColor = True
        ' 
        ' lblMergeSummary
        ' 
        lblMergeSummary.AutoSize = True
        lblMergeSummary.Location = New System.Drawing.Point(297, 95)
        lblMergeSummary.Name = "lblMergeSummary"
        lblMergeSummary.Size = New System.Drawing.Size(0, 20)
        lblMergeSummary.TabIndex = 3
        '
        ' lnkMergeRangeHelp
        '
        lnkMergeRangeHelp.AutoSize = True
        lnkMergeRangeHelp.Location = New System.Drawing.Point(500, 95)
        lnkMergeRangeHelp.Name = "lnkMergeRangeHelp"
        lnkMergeRangeHelp.TabIndex = 4
        lnkMergeRangeHelp.TabStop = True
        lnkMergeRangeHelp.Text = "Page range help"
        ' 
        ' tabExtract
        ' 
        tabExtract.Controls.Add(lblExtractPages)
        tabExtract.Controls.Add(txtExtractPages)
        tabExtract.Controls.Add(lblExtractHint)
        tabExtract.Controls.Add(lnkExtractRangeHelp)
        tabExtract.Controls.Add(pnlExtractFrom)
        tabExtract.Controls.Add(pnlExtractOutput)
        tabExtract.Location = New System.Drawing.Point(4, 29)
        tabExtract.Name = "tabExtract"
        tabExtract.Padding = New System.Windows.Forms.Padding(3)
        tabExtract.Size = New System.Drawing.Size(1224, 148)
        tabExtract.TabIndex = 2
        tabExtract.Text = "Extract Pages"
        tabExtract.UseVisualStyleBackColor = True
        ' 
        ' lblExtractPages
        ' 
        lblExtractPages.AutoSize = True
        lblExtractPages.Location = New System.Drawing.Point(13, 16)
        lblExtractPages.Name = "lblExtractPages"
        lblExtractPages.Size = New System.Drawing.Size(117, 20)
        lblExtractPages.TabIndex = 0
        lblExtractPages.Text = "&Pages to extract:"
        ' 
        ' txtExtractPages
        ' 
        txtExtractPages.Location = New System.Drawing.Point(159, 13)
        txtExtractPages.Name = "txtExtractPages"
        txtExtractPages.PlaceholderText = "1-3, 5, 8-end"
        txtExtractPages.Size = New System.Drawing.Size(240, 27)
        txtExtractPages.TabIndex = 1
        ' 
        ' lblExtractHint
        ' 
        lblExtractHint.AutoSize = True
        lblExtractHint.Location = New System.Drawing.Point(412, 16)
        lblExtractHint.Name = "lblExtractHint"
        lblExtractHint.Size = New System.Drawing.Size(283, 20)
        lblExtractHint.TabIndex = 2
        lblExtractHint.Tag = "muted"
        lblExtractHint.Text = "For example 1-3, 5, 8-end, or odd or even"
        '
        ' lnkExtractRangeHelp
        '
        lnkExtractRangeHelp.AutoSize = True
        lnkExtractRangeHelp.Location = New System.Drawing.Point(715, 16)
        lnkExtractRangeHelp.Name = "lnkExtractRangeHelp"
        lnkExtractRangeHelp.TabIndex = 5
        lnkExtractRangeHelp.TabStop = True
        lnkExtractRangeHelp.Text = "Page range help"
        ' 
        ' pnlExtractFrom
        ' 
        pnlExtractFrom.Controls.Add(lblExtractFrom)
        pnlExtractFrom.Controls.Add(rbExtractSelected)
        pnlExtractFrom.Controls.Add(rbExtractAll)
        pnlExtractFrom.Location = New System.Drawing.Point(13, 50)
        pnlExtractFrom.Name = "pnlExtractFrom"
        pnlExtractFrom.Size = New System.Drawing.Size(873, 31)
        pnlExtractFrom.TabIndex = 3
        ' 
        ' lblExtractFrom
        ' 
        lblExtractFrom.AutoSize = True
        lblExtractFrom.Location = New System.Drawing.Point(0, 5)
        lblExtractFrom.Name = "lblExtractFrom"
        lblExtractFrom.Size = New System.Drawing.Size(115, 20)
        lblExtractFrom.TabIndex = 0
        lblExtractFrom.Text = "Take them from:"
        ' 
        ' rbExtractSelected
        ' 
        rbExtractSelected.AutoSize = True
        rbExtractSelected.Checked = True
        rbExtractSelected.Location = New System.Drawing.Point(129, 3)
        rbExtractSelected.Name = "rbExtractSelected"
        rbExtractSelected.Size = New System.Drawing.Size(207, 24)
        rbExtractSelected.TabIndex = 1
        rbExtractSelected.TabStop = True
        rbExtractSelected.Text = "The PDF selected in the list"
        rbExtractSelected.UseVisualStyleBackColor = True
        ' 
        ' rbExtractAll
        ' 
        rbExtractAll.AutoSize = True
        rbExtractAll.Location = New System.Drawing.Point(360, 3)
        rbExtractAll.Name = "rbExtractAll"
        rbExtractAll.Size = New System.Drawing.Size(159, 24)
        rbExtractAll.TabIndex = 2
        rbExtractAll.Text = "Every PDF in the list"
        rbExtractAll.UseVisualStyleBackColor = True
        ' 
        ' pnlExtractOutput
        ' 
        pnlExtractOutput.Controls.Add(lblExtractSave)
        pnlExtractOutput.Controls.Add(rbExtractOne)
        pnlExtractOutput.Controls.Add(rbExtractEach)
        pnlExtractOutput.Controls.Add(rbExtractEvery)
        pnlExtractOutput.Controls.Add(numSplitEvery)
        pnlExtractOutput.Controls.Add(lblSplitPages)
        pnlExtractOutput.Location = New System.Drawing.Point(13, 85)
        pnlExtractOutput.Name = "pnlExtractOutput"
        pnlExtractOutput.Size = New System.Drawing.Size(873, 35)
        pnlExtractOutput.TabIndex = 4
        ' 
        ' lblExtractSave
        ' 
        lblExtractSave.AutoSize = True
        lblExtractSave.Location = New System.Drawing.Point(0, 8)
        lblExtractSave.Name = "lblExtractSave"
        lblExtractSave.Size = New System.Drawing.Size(99, 20)
        lblExtractSave.TabIndex = 0
        lblExtractSave.Text = "Save them as:"
        ' 
        ' rbExtractOne
        ' 
        rbExtractOne.AutoSize = True
        rbExtractOne.Checked = True
        rbExtractOne.Location = New System.Drawing.Point(129, 5)
        rbExtractOne.Name = "rbExtractOne"
        rbExtractOne.Size = New System.Drawing.Size(87, 24)
        rbExtractOne.TabIndex = 1
        rbExtractOne.TabStop = True
        rbExtractOne.Text = "One PDF"
        rbExtractOne.UseVisualStyleBackColor = True
        ' 
        ' rbExtractEach
        ' 
        rbExtractEach.AutoSize = True
        rbExtractEach.Location = New System.Drawing.Point(229, 5)
        rbExtractEach.Name = "rbExtractEach"
        rbExtractEach.Size = New System.Drawing.Size(151, 24)
        rbExtractEach.TabIndex = 2
        rbExtractEach.Text = "One PDF per page"
        rbExtractEach.UseVisualStyleBackColor = True
        ' 
        ' rbExtractEvery
        ' 
        rbExtractEvery.AutoSize = True
        rbExtractEvery.Location = New System.Drawing.Point(406, 5)
        rbExtractEvery.Name = "rbExtractEvery"
        rbExtractEvery.Size = New System.Drawing.Size(140, 24)
        rbExtractEvery.TabIndex = 3
        rbExtractEvery.Text = "A new PDF every"
        rbExtractEvery.UseVisualStyleBackColor = True
        ' 
        ' numSplitEvery
        ' 
        numSplitEvery.Enabled = False
        numSplitEvery.Location = New System.Drawing.Point(545, 4)
        numSplitEvery.Maximum = New Decimal(New Integer() {9999, 0, 0, 0})
        numSplitEvery.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numSplitEvery.Name = "numSplitEvery"
        numSplitEvery.Size = New System.Drawing.Size(65, 27)
        numSplitEvery.TabIndex = 4
        numSplitEvery.Value = New Decimal(New Integer() {2, 0, 0, 0})
        ' 
        ' lblSplitPages
        ' 
        lblSplitPages.AutoSize = True
        lblSplitPages.Location = New System.Drawing.Point(617, 8)
        lblSplitPages.Name = "lblSplitPages"
        lblSplitPages.Size = New System.Drawing.Size(49, 20)
        lblSplitPages.TabIndex = 5
        lblSplitPages.Text = "pages"
        ' 
        ' tabEncrypt
        ' 
        tabEncrypt.Controls.Add(pnlEncryptMode)
        tabEncrypt.Controls.Add(lblOpenPassword)
        tabEncrypt.Controls.Add(txtOpenPassword)
        tabEncrypt.Controls.Add(lblConfirmPassword)
        tabEncrypt.Controls.Add(txtConfirmPassword)
        tabEncrypt.Controls.Add(chkShowPasswords)
        tabEncrypt.Controls.Add(lblPermissionsPassword)
        tabEncrypt.Controls.Add(txtPermissionsPassword)
        tabEncrypt.Controls.Add(lblAllow)
        tabEncrypt.Controls.Add(chkAllowPrinting)
        tabEncrypt.Controls.Add(chkAllowCopying)
        tabEncrypt.Controls.Add(chkAllowEditing)
        tabEncrypt.Controls.Add(lblEncryptInfo)
        tabEncrypt.Location = New System.Drawing.Point(4, 29)
        tabEncrypt.Name = "tabEncrypt"
        tabEncrypt.Padding = New System.Windows.Forms.Padding(3)
        tabEncrypt.Size = New System.Drawing.Size(1224, 148)
        tabEncrypt.TabIndex = 3
        tabEncrypt.Text = "Encrypt"
        tabEncrypt.UseVisualStyleBackColor = True
        ' 
        ' lblOpenPassword
        ' 
        lblOpenPassword.AutoSize = True
        lblOpenPassword.Location = New System.Drawing.Point(13, 56)
        lblOpenPassword.Name = "lblOpenPassword"
        lblOpenPassword.Size = New System.Drawing.Size(129, 20)
        lblOpenPassword.TabIndex = 0
        lblOpenPassword.Text = "Password to &open:"
        ' 
        ' txtOpenPassword
        ' 
        txtOpenPassword.Location = New System.Drawing.Point(159, 53)
        txtOpenPassword.Name = "txtOpenPassword"
        txtOpenPassword.Size = New System.Drawing.Size(218, 27)
        txtOpenPassword.TabIndex = 1
        txtOpenPassword.UseSystemPasswordChar = True
        ' 
        ' lblConfirmPassword
        ' 
        lblConfirmPassword.AutoSize = True
        lblConfirmPassword.Location = New System.Drawing.Point(13, 88)
        lblConfirmPassword.Name = "lblConfirmPassword"
        lblConfirmPassword.Size = New System.Drawing.Size(97, 20)
        lblConfirmPassword.TabIndex = 2
        lblConfirmPassword.Text = "Type it again:"
        ' 
        ' txtConfirmPassword
        ' 
        txtConfirmPassword.Location = New System.Drawing.Point(159, 85)
        txtConfirmPassword.Name = "txtConfirmPassword"
        txtConfirmPassword.Size = New System.Drawing.Size(218, 27)
        txtConfirmPassword.TabIndex = 3
        txtConfirmPassword.UseSystemPasswordChar = True
        ' 
        ' chkShowPasswords
        ' 
        chkShowPasswords.AutoSize = True
        chkShowPasswords.Location = New System.Drawing.Point(159, 116)
        chkShowPasswords.Name = "chkShowPasswords"
        chkShowPasswords.Size = New System.Drawing.Size(140, 24)
        chkShowPasswords.TabIndex = 4
        chkShowPasswords.Text = "Show passwords"
        chkShowPasswords.UseVisualStyleBackColor = True
        ' 
        ' lblPermissionsPassword
        ' 
        lblPermissionsPassword.AutoSize = True
        lblPermissionsPassword.Location = New System.Drawing.Point(415, 56)
        lblPermissionsPassword.Name = "lblPermissionsPassword"
        lblPermissionsPassword.Size = New System.Drawing.Size(225, 20)
        lblPermissionsPassword.TabIndex = 5
        lblPermissionsPassword.Text = "Permissions password (optional):"
        ' 
        ' txtPermissionsPassword
        ' 
        txtPermissionsPassword.Location = New System.Drawing.Point(655, 53)
        txtPermissionsPassword.Name = "txtPermissionsPassword"
        txtPermissionsPassword.Size = New System.Drawing.Size(218, 27)
        txtPermissionsPassword.TabIndex = 6
        txtPermissionsPassword.UseSystemPasswordChar = True
        ' 
        ' lblAllow
        ' 
        lblAllow.AutoSize = True
        lblAllow.Location = New System.Drawing.Point(415, 88)
        lblAllow.Name = "lblAllow"
        lblAllow.Size = New System.Drawing.Size(172, 20)
        lblAllow.TabIndex = 7
        lblAllow.Text = "People who open it may:"
        ' 
        ' chkAllowPrinting
        ' 
        chkAllowPrinting.AutoSize = True
        chkAllowPrinting.Checked = True
        chkAllowPrinting.CheckState = System.Windows.Forms.CheckState.Checked
        chkAllowPrinting.Location = New System.Drawing.Point(655, 87)
        chkAllowPrinting.Name = "chkAllowPrinting"
        chkAllowPrinting.Size = New System.Drawing.Size(61, 24)
        chkAllowPrinting.TabIndex = 8
        chkAllowPrinting.Text = "Print"
        chkAllowPrinting.UseVisualStyleBackColor = True
        ' 
        ' chkAllowCopying
        ' 
        chkAllowCopying.AutoSize = True
        chkAllowCopying.Checked = True
        chkAllowCopying.CheckState = System.Windows.Forms.CheckState.Checked
        chkAllowCopying.Location = New System.Drawing.Point(724, 87)
        chkAllowCopying.Name = "chkAllowCopying"
        chkAllowCopying.Size = New System.Drawing.Size(94, 24)
        chkAllowCopying.TabIndex = 9
        chkAllowCopying.Text = "Copy text"
        chkAllowCopying.UseVisualStyleBackColor = True
        ' 
        ' chkAllowEditing
        ' 
        chkAllowEditing.AutoSize = True
        chkAllowEditing.Checked = True
        chkAllowEditing.CheckState = System.Windows.Forms.CheckState.Checked
        chkAllowEditing.Location = New System.Drawing.Point(823, 87)
        chkAllowEditing.Name = "chkAllowEditing"
        chkAllowEditing.Size = New System.Drawing.Size(57, 24)
        chkAllowEditing.TabIndex = 10
        chkAllowEditing.Text = "Edit"
        chkAllowEditing.UseVisualStyleBackColor = True
        ' 
        ' lblEncryptInfo
        ' 
        lblEncryptInfo.Anchor = System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblEncryptInfo.Location = New System.Drawing.Point(13, 147)
        lblEncryptInfo.Name = "lblEncryptInfo"
        lblEncryptInfo.Size = New System.Drawing.Size(1199, 48)
        lblEncryptInfo.TabIndex = 11
        lblEncryptInfo.Text = "Every PDF in the list gets its own protected copy, using strong AES 256-bit encryption. Your original files aren't changed. A forgotten password can't be recovered."
        ' 
        ' lblStep3
        ' 
        lblStep3.AutoSize = True
        lblStep3.Font = New System.Drawing.Font("Segoe UI Semibold", 11F)
        lblStep3.Location = New System.Drawing.Point(18, 406)
        lblStep3.Margin = New System.Windows.Forms.Padding(3, 9, 3, 2)
        lblStep3.Name = "lblStep3"
        lblStep3.Size = New System.Drawing.Size(219, 25)
        lblStep3.TabIndex = 6
        lblStep3.Text = "3. Choose where to save"
        ' 
        ' tblOutput
        ' 
        tblOutput.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        tblOutput.AutoSize = True
        tblOutput.ColumnCount = 3
        tblOutput.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblOutput.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        tblOutput.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblOutput.Controls.Add(lblOutputFolder, 0, 0)
        tblOutput.Controls.Add(txtOutputFolder, 1, 0)
        tblOutput.Controls.Add(btnBrowse, 2, 0)
        tblOutput.Controls.Add(lblOutputName, 0, 1)
        tblOutput.Controls.Add(txtOutputName, 1, 1)
        tblOutput.Controls.Add(lblPdfExtension, 2, 1)
        tblOutput.Controls.Add(lblPreview, 1, 2)
        tblOutput.Controls.Add(lblExtras, 0, 3)
        tblOutput.Controls.Add(flowExtras, 1, 3)
        tblOutput.SetColumnSpan(flowExtras, 2)
        tblOutput.Location = New System.Drawing.Point(15, 433)
        tblOutput.Margin = New System.Windows.Forms.Padding(0)
        tblOutput.Name = "tblOutput"
        tblOutput.RowCount = 4
        tblOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblOutput.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblOutput.Size = New System.Drawing.Size(1238, 103)
        tblOutput.TabIndex = 7
        ' 
        ' lblOutputFolder
        ' 
        lblOutputFolder.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblOutputFolder.AutoSize = True
        lblOutputFolder.Location = New System.Drawing.Point(3, 15)
        lblOutputFolder.Name = "lblOutputFolder"
        lblOutputFolder.Size = New System.Drawing.Size(102, 20)
        lblOutputFolder.TabIndex = 0
        lblOutputFolder.Text = "Output f&older:"
        ' 
        ' txtOutputFolder
        ' 
        txtOutputFolder.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        txtOutputFolder.Location = New System.Drawing.Point(133, 11)
        txtOutputFolder.Name = "txtOutputFolder"
        txtOutputFolder.PlaceholderText = "Choose the folder to save in"
        txtOutputFolder.Size = New System.Drawing.Size(961, 27)
        txtOutputFolder.TabIndex = 1
        ' 
        ' btnBrowse
        ' 
        btnBrowse.AutoSize = True
        btnBrowse.Location = New System.Drawing.Point(1100, 3)
        btnBrowse.MinimumSize = New System.Drawing.Size(135, 44)
        btnBrowse.Name = "btnBrowse"
        btnBrowse.Size = New System.Drawing.Size(135, 44)
        btnBrowse.TabIndex = 2
        btnBrowse.Text = "&Browse…"
        btnBrowse.UseVisualStyleBackColor = True
        ' 
        ' lblOutputName
        ' 
        lblOutputName.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblOutputName.AutoSize = True
        lblOutputName.Location = New System.Drawing.Point(3, 56)
        lblOutputName.Name = "lblOutputName"
        lblOutputName.Size = New System.Drawing.Size(124, 20)
        lblOutputName.TabIndex = 3
        lblOutputName.Text = "Output file &name:"
        ' 
        ' txtOutputName
        ' 
        txtOutputName.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        txtOutputName.Location = New System.Drawing.Point(133, 53)
        txtOutputName.Name = "txtOutputName"
        txtOutputName.Size = New System.Drawing.Size(961, 27)
        txtOutputName.TabIndex = 4
        ' 
        ' lblPdfExtension
        ' 
        lblPdfExtension.Anchor = System.Windows.Forms.AnchorStyles.Left
        lblPdfExtension.AutoSize = True
        lblPdfExtension.Location = New System.Drawing.Point(1100, 56)
        lblPdfExtension.Name = "lblPdfExtension"
        lblPdfExtension.Size = New System.Drawing.Size(35, 20)
        lblPdfExtension.TabIndex = 5
        lblPdfExtension.Text = ".pdf"
        ' 
        ' lblPreview
        ' 
        lblPreview.AutoSize = True
        tblOutput.SetColumnSpan(lblPreview, 2)
        lblPreview.ForeColor = Drawing.Color.FromArgb(CByte(85), CByte(85), CByte(85))
        lblPreview.Location = New System.Drawing.Point(133, 83)
        lblPreview.Name = "lblPreview"
        lblPreview.Size = New System.Drawing.Size(0, 20)
        lblPreview.TabIndex = 6
        lblPreview.Tag = "muted"
        ' 
        ' tblActions
        ' 
        tblActions.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        tblActions.AutoSize = True
        tblActions.ColumnCount = 7
        tblActions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F))
        tblActions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblActions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblActions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblActions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblActions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblActions.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        tblActions.Controls.Add(lblStatus, 0, 0)
        tblActions.Controls.Add(lnkDetails, 1, 0)
        tblActions.Controls.Add(progressBar, 2, 0)
        tblActions.Controls.Add(btnOpenFile, 3, 0)
        tblActions.Controls.Add(btnOpenFolder, 4, 0)
        tblActions.Controls.Add(btnCancel, 5, 0)
        tblActions.Controls.Add(btnRun, 6, 0)
        tblActions.Location = New System.Drawing.Point(15, 540)
        tblActions.Margin = New System.Windows.Forms.Padding(0, 4, 0, 2)
        tblActions.Name = "tblActions"
        tblActions.RowCount = 1
        tblActions.RowStyles.Add(New System.Windows.Forms.RowStyle())
        tblActions.Size = New System.Drawing.Size(1238, 56)
        tblActions.TabIndex = 8
        ' 
        ' lblStatus
        ' 
        lblStatus.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        lblStatus.AutoSize = True
        lblStatus.Location = New System.Drawing.Point(3, 18)
        lblStatus.Name = "lblStatus"
        lblStatus.Size = New System.Drawing.Size(335, 20)
        lblStatus.TabIndex = 0
        ' 
        ' lnkDetails
        ' 
        lnkDetails.Anchor = System.Windows.Forms.AnchorStyles.Left
        lnkDetails.AutoSize = True
        lnkDetails.Location = New System.Drawing.Point(351, 18)
        lnkDetails.Margin = New System.Windows.Forms.Padding(10, 0, 10, 0)
        lnkDetails.Name = "lnkDetails"
        lnkDetails.Size = New System.Drawing.Size(93, 20)
        lnkDetails.TabIndex = 1
        lnkDetails.TabStop = True
        lnkDetails.Text = "Show details"
        ' 
        ' progressBar
        ' 
        progressBar.Anchor = System.Windows.Forms.AnchorStyles.Left
        progressBar.Location = New System.Drawing.Point(457, 18)
        progressBar.Name = "progressBar"
        progressBar.Size = New System.Drawing.Size(131, 20)
        progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee
        progressBar.TabIndex = 2
        progressBar.Visible = False
        ' 
        ' btnOpenFile
        ' 
        btnOpenFile.AutoSize = True
        btnOpenFile.Enabled = False
        btnOpenFile.Location = New System.Drawing.Point(594, 3)
        btnOpenFile.MinimumSize = New System.Drawing.Size(135, 50)
        btnOpenFile.Name = "btnOpenFile"
        btnOpenFile.Size = New System.Drawing.Size(135, 50)
        btnOpenFile.TabIndex = 3
        btnOpenFile.Text = "Open File"
        btnOpenFile.UseVisualStyleBackColor = True
        ' 
        ' btnOpenFolder
        ' 
        btnOpenFolder.AutoSize = True
        btnOpenFolder.Enabled = False
        btnOpenFolder.Location = New System.Drawing.Point(735, 3)
        btnOpenFolder.MinimumSize = New System.Drawing.Size(135, 50)
        btnOpenFolder.Name = "btnOpenFolder"
        btnOpenFolder.Size = New System.Drawing.Size(135, 50)
        btnOpenFolder.TabIndex = 4
        btnOpenFolder.Text = "Open Folder"
        btnOpenFolder.UseVisualStyleBackColor = True
        ' 
        ' btnCancel
        ' 
        btnCancel.AutoSize = True
        btnCancel.Location = New System.Drawing.Point(876, 3)
        btnCancel.MinimumSize = New System.Drawing.Size(135, 50)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New System.Drawing.Size(135, 50)
        btnCancel.TabIndex = 5
        btnCancel.Text = "Cancel"
        btnCancel.UseVisualStyleBackColor = True
        btnCancel.Visible = False
        ' 
        ' btnRun
        ' 
        btnRun.AutoSize = True
        btnRun.BackColor = Drawing.Color.FromArgb(CByte(11), CByte(92), CByte(173))
        btnRun.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        btnRun.Font = New System.Drawing.Font("Segoe UI Semibold", 11F)
        btnRun.ForeColor = Drawing.Color.White
        btnRun.Location = New System.Drawing.Point(1017, 3)
        btnRun.MinimumSize = New System.Drawing.Size(218, 50)
        btnRun.Name = "btnRun"
        btnRun.Size = New System.Drawing.Size(218, 50)
        btnRun.TabIndex = 6
        btnRun.Tag = "primary"
        btnRun.Text = "Combine PDFs"
        btnRun.UseVisualStyleBackColor = False
        ' 
        ' txtLog
        ' 
        txtLog.Anchor = System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right
        txtLog.BackColor = Drawing.Color.FromArgb(CByte(247), CByte(247), CByte(247))
        txtLog.Font = New System.Drawing.Font("Consolas", 9F)
        txtLog.Location = New System.Drawing.Point(18, 601)
        txtLog.Multiline = True
        txtLog.Name = "txtLog"
        txtLog.ReadOnly = True
        txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        txtLog.Size = New System.Drawing.Size(1232, 120)
        txtLog.TabIndex = 9
        txtLog.Visible = False
        ' 
        ' statusStrip
        ' 
        statusStrip.ImageScalingSize = New System.Drawing.Size(18, 18)
        statusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {lblQpdfStatus, lnkChangeQpdf})
        statusStrip.Location = New System.Drawing.Point(0, 803)
        statusStrip.Name = "statusStrip"
        statusStrip.Padding = New System.Windows.Forms.Padding(1, 0, 15, 0)
        statusStrip.Size = New System.Drawing.Size(1268, 26)
        statusStrip.TabIndex = 1
        ' 
        ' lblQpdfStatus
        ' 
        lblQpdfStatus.Name = "lblQpdfStatus"
        lblQpdfStatus.Size = New System.Drawing.Size(1094, 20)
        lblQpdfStatus.Spring = True
        lblQpdfStatus.Text = "Looking for QPDF…"
        lblQpdfStatus.TextAlign = Drawing.ContentAlignment.MiddleLeft
        ' 
        ' lnkChangeQpdf
        ' 
        lnkChangeQpdf.IsLink = True
        lnkChangeQpdf.Name = "lnkChangeQpdf"
        lnkChangeQpdf.Size = New System.Drawing.Size(158, 20)
        lnkChangeQpdf.Text = "Use a different QPDF…"
        ' 
        ' screenshotTimer
        ' 
        screenshotTimer.Interval = 16
        ' 
        ' MainForm
        ' 
        AllowDrop = True
        AutoScaleDimensions = New System.Drawing.SizeF(120F, 120F)
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        ClientSize = New System.Drawing.Size(1268, 829)
        Controls.Add(rootLayout)
        Controls.Add(pnlHeader)
        Controls.Add(statusStrip)
        Font = New System.Drawing.Font("Segoe UI", 9F)
        MinimumSize = New System.Drawing.Size(1286, 781)
        Name = "MainForm"
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Text = "PDF Manager"
        pnlHeader.ResumeLayout(False)
        pnlHeader.PerformLayout()
        CType(picLogo, ComponentModel.ISupportInitialize).EndInit()
        flowTheme.ResumeLayout(False)
        flowTheme.PerformLayout()
        flowOperations.ResumeLayout(False)
        tabRotate.ResumeLayout(False)
        tabRotate.PerformLayout()
        pnlRotateAngle.ResumeLayout(False)
        pnlRotateAngle.PerformLayout()
        tabStamp.ResumeLayout(False)
        tabStamp.PerformLayout()
        pnlStampMode.ResumeLayout(False)
        pnlStampMode.PerformLayout()
        CType(numStampPage, ComponentModel.ISupportInitialize).EndInit()
        CType(numBackgroundPage, ComponentModel.ISupportInitialize).EndInit()
        tabCompress.ResumeLayout(False)
        tabArrange.ResumeLayout(False)
        pnlCompressLevel.ResumeLayout(False)
        pnlCompressLevel.PerformLayout()
        rootLayout.ResumeLayout(False)
        rootLayout.PerformLayout()
        pnlQpdfMissing.ResumeLayout(False)
        pnlQpdfMissing.PerformLayout()
        flowToolbar.ResumeLayout(False)
        flowToolbar.PerformLayout()
        CType(grid, ComponentModel.ISupportInitialize).EndInit()
        tabOperations.ResumeLayout(False)
        tabCombine.ResumeLayout(False)
        tabCombine.PerformLayout()
        tabMerge.ResumeLayout(False)
        tabMerge.PerformLayout()
        tabExtract.ResumeLayout(False)
        tabExtract.PerformLayout()
        pnlExtractFrom.ResumeLayout(False)
        pnlExtractFrom.PerformLayout()
        pnlExtractOutput.ResumeLayout(False)
        pnlExtractOutput.PerformLayout()
        CType(numSplitEvery, ComponentModel.ISupportInitialize).EndInit()
        pnlEncryptMode.ResumeLayout(False)
        pnlEncryptMode.PerformLayout()
        tabEncrypt.ResumeLayout(False)
        tabEncrypt.PerformLayout()
        tblOutput.ResumeLayout(False)
        tblOutput.PerformLayout()
        tblActions.ResumeLayout(False)
        tblActions.PerformLayout()
        statusStrip.ResumeLayout(False)
        statusStrip.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub

    Friend WithEvents pnlHeader As System.Windows.Forms.Panel
    Friend WithEvents picLogo As System.Windows.Forms.PictureBox
    Friend WithEvents lblAppTitle As System.Windows.Forms.Label
    Friend WithEvents lblAppSubtitle As System.Windows.Forms.Label
    Friend WithEvents flowTheme As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblTheme As System.Windows.Forms.Label
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents lnkMergeRangeHelp As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkExtractRangeHelp As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkRotateRangeHelp As System.Windows.Forms.LinkLabel
    Friend WithEvents lnkStampRangeHelp As System.Windows.Forms.LinkLabel
    Friend WithEvents rbThemeLight As System.Windows.Forms.RadioButton
    Friend WithEvents rbThemeDark As System.Windows.Forms.RadioButton
    Friend WithEvents rbThemeAuto As System.Windows.Forms.RadioButton
    Friend WithEvents flowOperations As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents rbOpCombine As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpMerge As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpExtract As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpEncrypt As System.Windows.Forms.RadioButton
    Friend WithEvents rootLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents pnlQpdfMissing As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents lblQpdfMissing As System.Windows.Forms.Label
    Friend WithEvents btnLocateQpdf As System.Windows.Forms.Button
    Friend WithEvents lnkDownloadQpdf As System.Windows.Forms.LinkLabel
    Friend WithEvents lblStep1 As System.Windows.Forms.Label
    Friend WithEvents flowToolbar As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents btnAddPdf As System.Windows.Forms.Button
    Friend WithEvents btnAddFolder As System.Windows.Forms.Button
    Friend WithEvents chkSubfolders As System.Windows.Forms.CheckBox
    Friend WithEvents btnMoveUp As System.Windows.Forms.Button
    Friend WithEvents btnMoveDown As System.Windows.Forms.Button
    Friend WithEvents btnSort As System.Windows.Forms.Button
    Friend WithEvents btnRemove As System.Windows.Forms.Button
    Friend WithEvents btnClear As System.Windows.Forms.Button
    Friend WithEvents grid As System.Windows.Forms.DataGridView
    Friend WithEvents colUse As System.Windows.Forms.DataGridViewCheckBoxColumn
    Friend WithEvents colPosition As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colFileName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colPageCount As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colPagesToUse As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colSize As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colStatus As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents colFolder As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents lblStep2 As System.Windows.Forms.Label
    Friend WithEvents tabOperations As OptionsTabControl
    Friend WithEvents tabCombine As System.Windows.Forms.TabPage
    Friend WithEvents lblCombineInfo As System.Windows.Forms.Label
    Friend WithEvents chkKeepDetails As System.Windows.Forms.CheckBox
    Friend WithEvents tabMerge As System.Windows.Forms.TabPage
    Friend WithEvents lblMergeInfo As System.Windows.Forms.Label
    Friend WithEvents btnUseAll As System.Windows.Forms.Button
    Friend WithEvents btnUseNone As System.Windows.Forms.Button
    Friend WithEvents lblMergeSummary As System.Windows.Forms.Label
    Friend WithEvents tabExtract As System.Windows.Forms.TabPage
    Friend WithEvents lblExtractPages As System.Windows.Forms.Label
    Friend WithEvents txtExtractPages As System.Windows.Forms.TextBox
    Friend WithEvents lblExtractHint As System.Windows.Forms.Label
    Friend WithEvents pnlExtractFrom As System.Windows.Forms.Panel
    Friend WithEvents lblExtractFrom As System.Windows.Forms.Label
    Friend WithEvents rbExtractSelected As System.Windows.Forms.RadioButton
    Friend WithEvents rbExtractAll As System.Windows.Forms.RadioButton
    Friend WithEvents pnlExtractOutput As System.Windows.Forms.Panel
    Friend WithEvents lblExtractSave As System.Windows.Forms.Label
    Friend WithEvents rbExtractOne As System.Windows.Forms.RadioButton
    Friend WithEvents rbExtractEach As System.Windows.Forms.RadioButton
    Friend WithEvents rbExtractEvery As System.Windows.Forms.RadioButton
    Friend WithEvents numSplitEvery As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblSplitPages As System.Windows.Forms.Label
    Friend WithEvents tabEncrypt As System.Windows.Forms.TabPage
    Friend WithEvents lblOpenPassword As System.Windows.Forms.Label
    Friend WithEvents txtOpenPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblConfirmPassword As System.Windows.Forms.Label
    Friend WithEvents txtConfirmPassword As System.Windows.Forms.TextBox
    Friend WithEvents chkShowPasswords As System.Windows.Forms.CheckBox
    Friend WithEvents lblPermissionsPassword As System.Windows.Forms.Label
    Friend WithEvents txtPermissionsPassword As System.Windows.Forms.TextBox
    Friend WithEvents lblAllow As System.Windows.Forms.Label
    Friend WithEvents chkAllowPrinting As System.Windows.Forms.CheckBox
    Friend WithEvents chkAllowCopying As System.Windows.Forms.CheckBox
    Friend WithEvents chkAllowEditing As System.Windows.Forms.CheckBox
    Friend WithEvents lblEncryptInfo As System.Windows.Forms.Label
    Friend WithEvents rbOpRotate As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpStamp As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpCompress As System.Windows.Forms.RadioButton
    Friend WithEvents tabRotate As System.Windows.Forms.TabPage
    Friend WithEvents lblRotateInfo As System.Windows.Forms.Label
    Friend WithEvents pnlRotateAngle As System.Windows.Forms.Panel
    Friend WithEvents lblRotateTurn As System.Windows.Forms.Label
    Friend WithEvents rbRotateRight As System.Windows.Forms.RadioButton
    Friend WithEvents rbRotateLeft As System.Windows.Forms.RadioButton
    Friend WithEvents rbRotate180 As System.Windows.Forms.RadioButton
    Friend WithEvents lblRotatePages As System.Windows.Forms.Label
    Friend WithEvents txtRotatePages As System.Windows.Forms.TextBox
    Friend WithEvents lblRotateHint As System.Windows.Forms.Label
    Friend WithEvents tabStamp As System.Windows.Forms.TabPage
    Friend WithEvents lblStampInfo As System.Windows.Forms.Label
    Friend WithEvents pnlStampMode As System.Windows.Forms.Panel
    Friend WithEvents lblStampAs As System.Windows.Forms.Label
    Friend WithEvents rbStampOver As System.Windows.Forms.RadioButton
    Friend WithEvents rbStampUnder As System.Windows.Forms.RadioButton
    Friend WithEvents lblStampFile As System.Windows.Forms.Label
    Friend WithEvents txtStampFile As System.Windows.Forms.TextBox
    Friend WithEvents btnStampBrowse As System.Windows.Forms.Button
    Friend WithEvents lblStampPages As System.Windows.Forms.Label
    Friend WithEvents txtStampPages As System.Windows.Forms.TextBox
    Friend WithEvents lblStampUse As System.Windows.Forms.Label
    Friend WithEvents numStampPage As System.Windows.Forms.NumericUpDown
    Friend WithEvents rbStampBoth As System.Windows.Forms.RadioButton
    Friend WithEvents lblBackgroundFile As System.Windows.Forms.Label
    Friend WithEvents txtBackgroundFile As System.Windows.Forms.TextBox
    Friend WithEvents btnBackgroundBrowse As System.Windows.Forms.Button
    Friend WithEvents lblBackgroundUse As System.Windows.Forms.Label
    Friend WithEvents numBackgroundPage As System.Windows.Forms.NumericUpDown
    Friend WithEvents tabCompress As System.Windows.Forms.TabPage
    Friend WithEvents lblCompressInfo As System.Windows.Forms.Label
    Friend WithEvents pnlCompressLevel As System.Windows.Forms.Panel
    Friend WithEvents rbCompressNormal As System.Windows.Forms.RadioButton
    Friend WithEvents rbCompressMedium As System.Windows.Forms.RadioButton
    Friend WithEvents rbCompressHigh As System.Windows.Forms.RadioButton
    Friend WithEvents rbCompressExtreme As System.Windows.Forms.RadioButton
    Friend WithEvents rbOpArrange As System.Windows.Forms.RadioButton
    Friend WithEvents lblExtras As System.Windows.Forms.Label
    Friend WithEvents flowExtras As System.Windows.Forms.FlowLayoutPanel
    Friend WithEvents chkExtraPassword As System.Windows.Forms.CheckBox
    Friend WithEvents btnExtraPassword As System.Windows.Forms.Button
    Friend WithEvents chkExtraStamp As System.Windows.Forms.CheckBox
    Friend WithEvents btnExtraStamp As System.Windows.Forms.Button
    Friend WithEvents lblExtrasSummary As System.Windows.Forms.Label
    Friend WithEvents pnlEncryptMode As System.Windows.Forms.Panel
    Friend WithEvents lblEncryptMode As System.Windows.Forms.Label
    Friend WithEvents rbAddPassword As System.Windows.Forms.RadioButton
    Friend WithEvents rbRemovePassword As System.Windows.Forms.RadioButton
    Friend WithEvents tabArrange As System.Windows.Forms.TabPage
    Friend WithEvents lblArrangeInfo As System.Windows.Forms.Label
    Friend WithEvents btnShowPages As System.Windows.Forms.Button
    Friend WithEvents btnArrangeReset As System.Windows.Forms.Button
    Friend WithEvents lblArrangeSummary As System.Windows.Forms.Label
    Friend WithEvents lblStep3 As System.Windows.Forms.Label
    Friend WithEvents tblOutput As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblOutputFolder As System.Windows.Forms.Label
    Friend WithEvents txtOutputFolder As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowse As System.Windows.Forms.Button
    Friend WithEvents lblOutputName As System.Windows.Forms.Label
    Friend WithEvents txtOutputName As System.Windows.Forms.TextBox
    Friend WithEvents lblPdfExtension As System.Windows.Forms.Label
    Friend WithEvents lblPreview As System.Windows.Forms.Label
    Friend WithEvents tblActions As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents lnkDetails As System.Windows.Forms.LinkLabel
    Friend WithEvents progressBar As System.Windows.Forms.ProgressBar
    Friend WithEvents btnOpenFile As System.Windows.Forms.Button
    Friend WithEvents btnOpenFolder As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents statusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents lblQpdfStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lnkChangeQpdf As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents screenshotTimer As System.Windows.Forms.Timer
End Class
