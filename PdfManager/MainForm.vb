Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Windows.Forms

''' <summary>
''' The main window, in three steps from top to bottom: add PDFs, choose what to do with them,
''' and choose where the result goes. The PDF work itself is in Planner and OperationRunner.
''' </summary>
Public Class MainForm

    Private Const ScreenshotFrames As Integer = 120
    Private Const QpdfDownloadUrl As String = "https://github.com/qpdf/qpdf/releases/latest"

    Friend Enum StatusKind
        Normal
        Success
        Failure
    End Enum

    Private ReadOnly _items As New BindingList(Of PdfItem)
    Private _palette As Palette = Palette.LightPalette
    Private _themeChoice As ThemeChoice = ThemeChoice.Auto
    Private _settingTheme As Boolean
    Private _syncingOperation As Boolean
    ''' <summary>False while the designer code is still building the window.</summary>
    Private _ready As Boolean

    ''' <summary>What the last message under the buttons meant, for the tests.</summary>
    Friend LastStatusKind As StatusKind = StatusKind.Normal
    Private _settings As AppSettings
    Private _startup As StartupOptions
    Private _tool As QpdfTool
    Private _cts As CancellationTokenSource
    Private _busy As Boolean
    Private _closeAfterRun As Boolean
    Private _nameEdited As Boolean
    Private _settingName As Boolean
    Private _folderChosen As Boolean
    Private _settingFolder As Boolean
    Private _lastOutputs As New List(Of String)
    Private _screenshotPath As String
    Private _frames As Integer

    Friend Prompts As IUserPrompts
    Friend PersistSettings As Boolean = True
    Friend StartupTask As Task = Task.CompletedTask

    Public Sub New()
        InitializeComponent()
        Prompts = New DialogPrompts(Me)
        _settings = AppSettings.Load()
        chkSubfolders.Checked = _settings.IncludeSubfolders

        grid.AutoGenerateColumns = False
        grid.DataSource = _items
        LoadLogo()
        ApplyLook()
        _ready = True
        UpdateStampRows()
        UpdateEncryptRows()
        SetThemeChoice(Theme.ParseChoice(_settings.Theme))
        UpdateForOperation()
    End Sub

    ''' <summary>The window icon and the header logo, both built into the program.</summary>
    Private Sub LoadLogo()
        Dim assembly = GetType(MainForm).Assembly
        Using iconStream = assembly.GetManifestResourceStream("PdfManager.app.ico")
            If iconStream IsNot Nothing Then Icon = New Icon(iconStream)
        End Using
        Using logoStream = assembly.GetManifestResourceStream("PdfManager.logo.png")
            If logoStream IsNot Nothing Then picLogo.Image = New Bitmap(Image.FromStream(logoStream))
        End Using
    End Sub

    ''' <summary>Takes in what the program was started with. Call before the window is shown.</summary>
    Friend Sub ApplyStartup(options As StartupOptions)
        _startup = options
        _screenshotPath = options.ScreenshotPath
        _startupHelpTopic = options.HelpTopic
        If _screenshotPath IsNot Nothing Then
            PersistSettings = False
            ShowInTaskbar = False
        End If

        Select Case If(options.TabName, "").Trim().ToLowerInvariant()
            Case "merge" : tabOperations.SelectedTab = tabMerge
            Case "extract" : tabOperations.SelectedTab = tabExtract
            Case "encrypt" : tabOperations.SelectedTab = tabEncrypt
            Case "decrypt"
                tabOperations.SelectedTab = tabEncrypt
                rbRemovePassword.Checked = True
            Case "rotate" : tabOperations.SelectedTab = tabRotate
            Case "stamp" : tabOperations.SelectedTab = tabStamp
            Case "compress" : tabOperations.SelectedTab = tabCompress
            Case "arrange" : tabOperations.SelectedTab = tabArrange
        End Select
        If options.ShowPages Then tabOperations.SelectedTab = tabArrange
        SyncOperationButtons()
        UpdateForOperation()

        ' --theme=light|dark|auto changes the look for this run only.
        If Not String.IsNullOrWhiteSpace(options.ThemeName) Then
            Dim saved = _settings.Theme
            SetThemeChoice(Theme.ParseChoice(options.ThemeName))
            _settings.Theme = saved
        End If
    End Sub

    ''' <summary>Sizes and fonts that don't depend on the theme.</summary>
    Private Sub ApplyLook()
        grid.BorderStyle = BorderStyle.FixedSingle
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single
        grid.ColumnHeadersHeight = LogicalToDeviceUnits(32)
        grid.RowTemplate.Height = LogicalToDeviceUnits(30)

        ' Bigger, easier-to-hit buttons.
        Dim buttonFont As New Font("Segoe UI", 10.0F)
        For Each button In AllControls(Me).OfType(Of Button)()
            If button IsNot btnRun Then button.Font = buttonFont
        Next
        For Each toggle In {rbThemeLight, rbThemeDark, rbThemeAuto}
            toggle.Font = buttonFont
        Next

        ' Short columns keep a fixed width; the name and folder share the rest.
        Dim fixedWidths As New List(Of (Column As DataGridViewColumn, Width As Integer)) From {
            (colUse, 44), (colPosition, 36), (colPageCount, 60), (colPagesToUse, 130), (colSize, 72), (colStatus, 120)
        }
        For Each fixedColumn In fixedWidths
            fixedColumn.Column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            fixedColumn.Column.Width = LogicalToDeviceUnits(fixedColumn.Width)
        Next
        colFileName.FillWeight = 55
        colFolder.FillWeight = 45

    End Sub

    Private Shared Iterator Function AllControls(parent As Control) As IEnumerable(Of Control)
        For Each child As Control In parent.Controls
            Yield child
            For Each grandChild In AllControls(child)
                Yield grandChild
            Next
        Next
    End Function

    ' ---------------------------------------------------------------------
    ' Light and dark themes
    ' ---------------------------------------------------------------------

    Friend ReadOnly Property CurrentPalette As Palette
        Get
            Return _palette
        End Get
    End Property

    Friend Sub SetThemeChoice(choice As ThemeChoice)
        _themeChoice = choice
        _settings.Theme = choice.ToString()
        _settingTheme = True
        rbThemeLight.Checked = choice = ThemeChoice.Light
        rbThemeDark.Checked = choice = ThemeChoice.Dark
        rbThemeAuto.Checked = choice = ThemeChoice.Auto
        _settingTheme = False
        ApplyTheme(Theme.Resolve(choice))
    End Sub

    Private Sub ThemeToggle_CheckedChanged(sender As Object, e As EventArgs) Handles _
        rbThemeLight.CheckedChanged, rbThemeDark.CheckedChanged, rbThemeAuto.CheckedChanged
        If Not _ready OrElse _settingTheme OrElse Not DirectCast(sender, RadioButton).Checked Then Return
        If sender Is rbThemeLight Then
            SetThemeChoice(ThemeChoice.Light)
        ElseIf sender Is rbThemeDark Then
            SetThemeChoice(ThemeChoice.Dark)
        Else
            SetThemeChoice(ThemeChoice.Auto)
        End If
    End Sub

    ' "Same as Windows" follows Windows when it switches between light and dark.
    Private Sub OnWindowsPreferenceChanged(sender As Object, e As Microsoft.Win32.UserPreferenceChangedEventArgs)
        If _themeChoice = ThemeChoice.Auto AndAlso Not IsDisposed Then
            BeginInvoke(Sub() ApplyTheme(Theme.Resolve(ThemeChoice.Auto)))
        End If
    End Sub

    Private Sub ApplyTheme(p As Palette)
        _palette = p
        SuspendLayout()
        Theme.Apply(Me, p)

        pnlQpdfMissing.BackColor = p.WarningBack
        For Each child As Control In pnlQpdfMissing.Controls
            If TypeOf child Is Label AndAlso Not TypeOf child Is LinkLabel Then
                child.ForeColor = p.WarningText
                child.BackColor = p.WarningBack
            ElseIf TypeOf child Is LinkLabel Then
                child.BackColor = p.WarningBack
            End If
        Next

        colPagesToUse.DefaultCellStyle.BackColor = p.Editable
        colPagesToUse.DefaultCellStyle.SelectionBackColor = p.Editable
        colPagesToUse.DefaultCellStyle.SelectionForeColor = p.Text
        colFolder.DefaultCellStyle.ForeColor = p.Muted
        colFolder.DefaultCellStyle.SelectionForeColor = p.Muted
        txtLog.BackColor = p.Surface

        ShowStatus(lblStatus.Text, LastStatusKind)
        RefreshIcons()
        UpdateToggleBorders()
        Theme.UseDarkTitleBar(Me, p.IsDark)
        If _helpForm IsNot Nothing AndAlso Not _helpForm.IsDisposed Then _helpForm.ApplyPalette(p)
        ResumeLayout(True)
        Invalidate(True)
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        ' The title bar and scroll bars can only be themed once Windows has created them.
        If _ready Then ApplyTheme(_palette)
        AddHandler Microsoft.Win32.SystemEvents.UserPreferenceChanged, AddressOf OnWindowsPreferenceChanged
    End Sub

    Protected Overrides Sub OnHandleDestroyed(e As EventArgs)
        RemoveHandler Microsoft.Win32.SystemEvents.UserPreferenceChanged, AddressOf OnWindowsPreferenceChanged
        MyBase.OnHandleDestroyed(e)
    End Sub

    ''' <summary>Icons from Windows' own icon font, drawn in the current text colour.</summary>
    Private Sub RefreshIcons()
        Dim small = LogicalToDeviceUnits(18)
        Dim large = LogicalToDeviceUnits(24)
        Theme.SetIcon(btnAddPdf, ChrW(&HE8E5), small)
        Theme.SetIcon(btnAddFolder, ChrW(&HE8B7), small)
        Theme.SetIcon(btnMoveUp, ChrW(&HE74A), small)
        Theme.SetIcon(btnMoveDown, ChrW(&HE74B), small)
        Theme.SetIcon(btnSort, ChrW(&HE8CB), small)
        Theme.SetIcon(btnRemove, ChrW(&HE74D), small)
        Theme.SetIcon(btnClear, ChrW(&HE894), small)
        Theme.SetIcon(btnBrowse, ChrW(&HE838), small)
        Theme.SetIcon(btnOpenFile, ChrW(&HE8A5), small)
        Theme.SetIcon(btnOpenFolder, ChrW(&HE838), small)
        Theme.SetIcon(btnCancel, ChrW(&HE711), small)
        Theme.SetIcon(rbThemeLight, ChrW(&HE706), small)
        Theme.SetIcon(rbThemeDark, ChrW(&HE708), small)
        Theme.SetIcon(rbThemeAuto, ChrW(&HE7F4), small)
        Theme.SetIcon(rbOpCombine, ChrW(&HE8C8), large)
        Theme.SetIcon(rbOpMerge, ChrW(&HE762), large)
        Theme.SetIcon(rbOpExtract, ChrW(&HE8C6), large)
        Theme.SetIcon(rbOpEncrypt, ChrW(&HE72E), large)
        Theme.SetIcon(rbOpRotate, ChrW(&HE7AD), large)
        Theme.SetIcon(rbOpStamp, ChrW(&HE81E), large)
        Theme.SetIcon(rbOpCompress, ChrW(&HE73F), large)
        Theme.SetIcon(rbOpArrange, ChrW(&HE8A9), large)
        Theme.SetIcon(btnShowPages, ChrW(&HE91B), small)
        Theme.SetIcon(btnArrangeReset, ChrW(&HE72C), small)
        Theme.SetIcon(btnExtraPassword, ChrW(&HE72E), small)
        Theme.SetIcon(btnExtraStamp, ChrW(&HE81E), small)
        btnExtraPassword.TextImageRelation = TextImageRelation.ImageBeforeText
        btnExtraStamp.TextImageRelation = TextImageRelation.ImageBeforeText
        Theme.SetIcon(btnStampBrowse, ChrW(&HE8E5), small)
        Theme.SetIcon(btnBackgroundBrowse, ChrW(&HE8E5), small)
        Theme.SetIcon(btnHelp, ChrW(&HE897), small)
        For Each toggle In {rbThemeLight, rbThemeDark, rbThemeAuto}
            toggle.TextImageRelation = TextImageRelation.ImageBeforeText
        Next
        ' The operation buttons show their icon above the name, so seven fit in a row.
        For Each toggle In OperationButtons()
            toggle.TextImageRelation = TextImageRelation.ImageAboveText
        Next
    End Sub

    Private Function OperationButtons() As RadioButton()
        Return {rbOpCombine, rbOpMerge, rbOpExtract, rbOpEncrypt, rbOpRotate, rbOpStamp, rbOpCompress, rbOpArrange}
    End Function

    ''' <summary>The chosen operation and theme get an accent-coloured outline.</summary>
    Private Sub UpdateToggleBorders()
        For Each toggle In OperationButtons().Concat({rbThemeLight, rbThemeDark, rbThemeAuto})
            toggle.FlatAppearance.BorderColor = If(toggle.Checked, _palette.Accent, _palette.Border)
            toggle.FlatAppearance.BorderSize = If(toggle.Checked, 2, 1)
        Next
    End Sub

    ' ---------------------------------------------------------------------
    ' The big operation buttons choose which options are shown
    ' ---------------------------------------------------------------------

    Private Sub OperationToggle_CheckedChanged(sender As Object, e As EventArgs) Handles _
        rbOpCombine.CheckedChanged, rbOpMerge.CheckedChanged, rbOpExtract.CheckedChanged, rbOpEncrypt.CheckedChanged,
        rbOpRotate.CheckedChanged, rbOpStamp.CheckedChanged, rbOpCompress.CheckedChanged, rbOpArrange.CheckedChanged
        If Not _ready Then Return
        UpdateToggleBorders()
        If _syncingOperation OrElse Not DirectCast(sender, RadioButton).Checked Then Return
        _syncingOperation = True
        tabOperations.SelectedTab = PageFor(DirectCast(sender, RadioButton))
        _syncingOperation = False
    End Sub

    ''' <summary>Each big operation button and the options page it shows.</summary>
    Private Function PageFor(button As RadioButton) As TabPage
        Dim pages = {tabCombine, tabMerge, tabExtract, tabEncrypt, tabRotate, tabStamp, tabCompress, tabArrange}
        Return pages(Array.IndexOf(OperationButtons(), button))
    End Function

    Private Sub SyncOperationButtons()
        If _syncingOperation Then Return
        _syncingOperation = True
        For Each button In OperationButtons()
            button.Checked = tabOperations.SelectedTab Is PageFor(button)
        Next
        _syncingOperation = False
        UpdateToggleBorders()
    End Sub

    ' ---------------------------------------------------------------------
    ' Starting up and finding QPDF
    ' ---------------------------------------------------------------------

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        FitToScreen()
        SyncOperationButtons()
        StartupTask = StartUpAsync()
    End Sub

    ''' <summary>Opens roomy on a big screen, and never larger than the screen on a small one.</summary>
    Private Sub FitToScreen()
        If StartPosition = FormStartPosition.Manual Then Return
        Dim area = Screen.FromControl(Me).WorkingArea
        Dim width = Math.Min(LogicalToDeviceUnits(1180), area.Width - 24)
        Dim height = Math.Min(LogicalToDeviceUnits(900), area.Height - 24)
        Size = New Size(Math.Max(width, MinimumSize.Width), Math.Max(height, Math.Min(MinimumSize.Height, area.Height)))
        Location = New Point(area.Left + (area.Width - Width) \ 2, area.Top + (area.Height - Height) \ 2)
    End Sub

    Private Async Function StartUpAsync() As Task
        Await Task.Run(AddressOf QpdfTool.CleanUpLeftovers)
        Await UseQpdfAsync(QpdfTool.Locate(_settings.QpdfPath))
        If _startup IsNot Nothing AndAlso _startup.Paths.Count > 0 Then
            Await AddPathsAsync(_startup.Paths)
        End If
    End Function

    Private Async Function UseQpdfAsync(exePath As String) As Task(Of Boolean)
        Dim version As String = Nothing
        If exePath IsNot Nothing Then
            Dim candidate As New QpdfTool(exePath)
            version = Await candidate.GetVersionAsync()
            If version IsNot Nothing Then _tool = candidate
        End If

        If version Is Nothing Then
            If _tool Is Nothing Then
                pnlQpdfMissing.Visible = True
                lblQpdfStatus.Text = "QPDF not found"
            End If
            Return False
        End If

        pnlQpdfMissing.Visible = False
        lblQpdfStatus.Text = $"QPDF {version} is ready"
        lblQpdfStatus.ToolTipText = exePath

        ' Anything added before QPDF was available can be read now.
        Await InspectItemsAsync(_items.Where(Function(i) i.State = PdfState.Checking).ToList())
        Return True
    End Function

    Private Async Function ChooseQpdfAsync() As Task
        Using dialog As New OpenFileDialog With {
            .Title = "Find qpdf.exe",
            .Filter = "QPDF program (qpdf.exe)|qpdf.exe|Programs (*.exe)|*.exe",
            .CheckFileExists = True
        }
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            If Await UseQpdfAsync(dialog.FileName) Then
                _settings.QpdfPath = dialog.FileName
                ShowStatus($"Now using QPDF from {dialog.FileName}.", StatusKind.Normal)
            Else
                ShowStatus("That program isn't a working copy of QPDF.", StatusKind.Failure)
            End If
        End Using
    End Function

    Private Async Sub btnLocateQpdf_Click(sender As Object, e As EventArgs) Handles btnLocateQpdf.Click
        Await ChooseQpdfAsync()
    End Sub

    Private Async Sub lnkChangeQpdf_Click(sender As Object, e As EventArgs) Handles lnkChangeQpdf.Click
        If _busy Then Return
        Await ChooseQpdfAsync()
    End Sub

    Private Sub lnkDownloadQpdf_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkDownloadQpdf.LinkClicked
        OpenWithShell(QpdfDownloadUrl)
    End Sub

    ' ---------------------------------------------------------------------
    ' Step 1: the list of PDFs
    ' ---------------------------------------------------------------------

    Private Async Sub btnAddPdf_Click(sender As Object, e As EventArgs) Handles btnAddPdf.Click
        Using dialog As New OpenFileDialog With {
            .Title = "Add PDFs",
            .Filter = "PDF files (*.pdf)|*.pdf",
            .Multiselect = True,
            .CheckFileExists = True
        }
            If Directory.Exists(_settings.LastInputFolder) Then dialog.InitialDirectory = _settings.LastInputFolder
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            _settings.LastInputFolder = Path.GetDirectoryName(dialog.FileNames(0))
            Await AddPathsAsync(dialog.FileNames)
        End Using
    End Sub

    Private Async Sub btnAddFolder_Click(sender As Object, e As EventArgs) Handles btnAddFolder.Click
        Using dialog As New FolderBrowserDialog With {
            .Description = "Add every PDF in this folder",
            .UseDescriptionForTitle = True
        }
            If Directory.Exists(_settings.LastInputFolder) Then dialog.InitialDirectory = _settings.LastInputFolder
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return
            _settings.LastInputFolder = dialog.SelectedPath
            Await AddPathsAsync({dialog.SelectedPath})
        End Using
    End Sub

    ''' <summary>Adds PDF files and every PDF in any folders, then reads their details with QPDF.</summary>
    Friend Async Function AddPathsAsync(paths As IEnumerable(Of String)) As Task
        Dim includeSubfolders = chkSubfolders.Checked
        Dim requested = paths.ToList()
        Dim collected = Await Task.Run(Function() CollectPdfs(requested, includeSubfolders))

        Dim known As New HashSet(Of String)(_items.Select(Function(i) i.Path), StringComparer.OrdinalIgnoreCase)
        Dim added As New List(Of PdfItem)
        Dim duplicates = 0
        For Each filePath In collected.Files
            If known.Add(Path.GetFullPath(filePath)) Then
                Dim item As New PdfItem(filePath)
                _items.Add(item)
                added.Add(item)
            Else
                duplicates += 1
            End If
        Next

        RenumberItems()
        If added.Count > 0 AndAlso Not _folderChosen AndAlso String.IsNullOrWhiteSpace(txtOutputFolder.Text) Then
            SetOutputFolderAutomatically(added(0).Folder)
        End If
        UpdateForOperation()
        ReportAdded(added.Count, duplicates, collected.Ignored)

        Await InspectItemsAsync(added)
        UpdateForOperation()
    End Function

    Private Shared Function CollectPdfs(paths As List(Of String), includeSubfolders As Boolean) As (Files As List(Of String), Ignored As Integer)
        Dim files As New List(Of String)
        Dim ignored = 0
        Dim options As New EnumerationOptions With {
            .IgnoreInaccessible = True,
            .RecurseSubdirectories = includeSubfolders,
            .MatchCasing = MatchCasing.CaseInsensitive
        }
        For Each entry In paths
            If Directory.Exists(entry) Then
                files.AddRange(Directory.EnumerateFiles(entry, "*.pdf", options))
            ElseIf File.Exists(entry) AndAlso Path.GetExtension(entry).Equals(".pdf", StringComparison.OrdinalIgnoreCase) Then
                files.Add(entry)
            Else
                ignored += 1
            End If
        Next
        files.Sort(AddressOf NaturalCompare)
        Return (files, ignored)
    End Function

    Private Sub ReportAdded(added As Integer, duplicates As Integer, ignored As Integer)
        Dim parts As New List(Of String)
        If added > 0 Then parts.Add(If(added = 1, "Added 1 PDF.", $"Added {added} PDFs."))
        If added = 0 AndAlso duplicates = 0 AndAlso ignored = 0 Then parts.Add("No PDFs found there.")
        If duplicates > 0 Then parts.Add(If(duplicates = 1, "1 was already in the list.", $"{duplicates} were already in the list."))
        If ignored > 0 Then parts.Add(If(ignored = 1, "1 item wasn't a PDF.", $"{ignored} items weren't PDFs."))
        ShowStatus(String.Join(" ", parts), If(added > 0, StatusKind.Normal, StatusKind.Failure))
    End Sub

    Private Async Function InspectItemsAsync(items As IReadOnlyCollection(Of PdfItem)) As Task
        If _tool Is Nothing OrElse items.Count = 0 Then Return
        Dim tool = _tool
        Using gate As New SemaphoreSlim(4)
            Dim work = items.Select(
                Async Function(item)
                    Await gate.WaitAsync()
                    Try
                        Await PdfInspector.InspectAsync(tool, item)
                    Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is Win32Exception OrElse TypeOf ex Is UnauthorizedAccessException
                        item.Problem = ex.Message
                        item.State = PdfState.Unreadable
                    Finally
                        gate.Release()
                    End Try
                End Function).ToList()
            Await Task.WhenAll(work)
        End Using
    End Function

    ''' <summary>Asks for a locked file's password until it opens or the person gives up.</summary>
    Private Async Function UnlockAsync(item As PdfItem) As Task(Of Boolean)
        Dim wasWrong = False
        While True
            Dim password = Prompts.AskPassword(item.FileName, wasWrong)
            If password Is Nothing Then Return False

            item.Password = password
            Await PdfInspector.InspectAsync(_tool, item)
            If item.State = PdfState.Ready Then Return True
            If item.State = PdfState.Unreadable Then Return False
            wasWrong = True
        End While
        Return False
    End Function

    Private Sub RenumberItems()
        For i = 0 To _items.Count - 1
            _items(i).Position = i + 1
        Next
        grid.Invalidate()
    End Sub

    Private Sub AfterListChanged()
        RenumberItems()
        If _items.Count = 0 AndAlso Not _folderChosen Then SetOutputFolderAutomatically("")
        UpdateForOperation()
    End Sub

    Private Function SelectedIndexes() As List(Of Integer)
        Return grid.SelectedRows.Cast(Of DataGridViewRow)().
            Select(Function(r) r.Index).
            OrderBy(Function(i) i).
            ToList()
    End Function

    Private Sub MoveSelected(delta As Integer)
        Dim indexes = SelectedIndexes()
        If indexes.Count = 0 Then Return
        grid.EndEdit()

        ' Work from the end being moved towards, so a selected block keeps its shape.
        If delta > 0 Then indexes.Reverse()
        Dim newSelection As New HashSet(Of Integer)
        _items.RaiseListChangedEvents = False
        For Each index In indexes
            Dim target = index + delta
            If target < 0 OrElse target >= _items.Count OrElse newSelection.Contains(target) Then
                newSelection.Add(index)
            Else
                Dim moved = _items(index)
                _items(index) = _items(target)
                _items(target) = moved
                newSelection.Add(target)
            End If
        Next
        _items.RaiseListChangedEvents = True
        _items.ResetBindings()

        AfterListChanged()
        SelectRows(newSelection)
    End Sub

    Private Sub SelectRows(indexes As IEnumerable(Of Integer))
        Dim list = indexes.OrderBy(Function(i) i).ToList()
        If list.Count = 0 Then Return
        grid.CurrentCell = grid.Rows(list(0)).Cells(colFileName.Index)
        grid.ClearSelection()
        For Each index In list
            grid.Rows(index).Selected = True
        Next
    End Sub

    Private Sub btnMoveUp_Click(sender As Object, e As EventArgs) Handles btnMoveUp.Click
        MoveSelected(-1)
    End Sub

    Private Sub btnMoveDown_Click(sender As Object, e As EventArgs) Handles btnMoveDown.Click
        MoveSelected(1)
    End Sub

    Private Sub btnSort_Click(sender As Object, e As EventArgs) Handles btnSort.Click
        grid.EndEdit()
        Dim sorted = _items.ToList()
        sorted.Sort(Function(a, b) NaturalCompare(a.FileName, b.FileName))
        _items.RaiseListChangedEvents = False
        _items.Clear()
        For Each item In sorted
            _items.Add(item)
        Next
        _items.RaiseListChangedEvents = True
        _items.ResetBindings()
        AfterListChanged()
    End Sub

    Private Sub RemoveSelected()
        Dim indexes = SelectedIndexes()
        If indexes.Count = 0 Then Return
        grid.EndEdit()
        For Each index In Enumerable.Reverse(indexes)
            _items.RemoveAt(index)
        Next
        AfterListChanged()
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        RemoveSelected()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        grid.EndEdit()
        _items.Clear()
        AfterListChanged()
    End Sub

    Private Sub grid_KeyDown(sender As Object, e As KeyEventArgs) Handles grid.KeyDown
        If e.KeyCode = Keys.Delete AndAlso Not grid.IsCurrentCellInEditMode AndAlso Not _busy Then
            RemoveSelected()
            e.Handled = True
        End If
    End Sub

    Private Async Sub grid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellDoubleClick
        If _busy OrElse e.RowIndex < 0 OrElse e.RowIndex >= _items.Count Then Return
        Dim item = _items(e.RowIndex)
        If item.State = PdfState.PasswordNeeded Then Await UnlockAsync(item)
    End Sub

    ' A single click starts typing in the Pages to Use column.
    Private Sub grid_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellClick
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = colPagesToUse.Index AndAlso colPagesToUse.Visible Then
            grid.BeginEdit(True)
        End If
    End Sub

    ' A tick counts straight away, not only when moving to another cell.
    Private Sub grid_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles grid.CurrentCellDirtyStateChanged
        If grid.IsCurrentCellDirty AndAlso TypeOf grid.CurrentCell Is DataGridViewCheckBoxCell Then
            grid.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub grid_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellValueChanged
        If e.RowIndex >= 0 AndAlso e.ColumnIndex = colUse.Index Then UpdateForOperation()
    End Sub

    Private Sub grid_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles grid.CellFormatting
        If e.RowIndex < 0 OrElse e.RowIndex >= _items.Count Then Return
        Dim item = _items(e.RowIndex)
        Dim cell = grid.Rows(e.RowIndex).Cells(e.ColumnIndex)

        If e.ColumnIndex = colStatus.Index Then
            cell.ToolTipText = If(item.Problem, "")
            If item.State = PdfState.Unreadable OrElse item.State = PdfState.PasswordNeeded Then
                e.CellStyle.ForeColor = _palette.ErrorText
                e.CellStyle.SelectionForeColor = _palette.ErrorText
            End If
        ElseIf e.ColumnIndex = colPagesToUse.Index Then
            Dim range = PageRange.Parse(item.PagesText, item.PageCount)
            cell.ToolTipText = If(range.IsValid, "", range.ErrorMessage)
            If Not range.IsValid Then
                e.CellStyle.ForeColor = _palette.ErrorText
                e.CellStyle.SelectionForeColor = _palette.ErrorText
            End If
        End If
    End Sub

    Private Sub grid_SelectionChanged(sender As Object, e As EventArgs) Handles grid.SelectionChanged
        UpdateOutputDefaults()
    End Sub

    Private Sub grid_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) Handles grid.DataError
        e.ThrowException = False
    End Sub

    Private Sub grid_Paint(sender As Object, e As PaintEventArgs) Handles grid.Paint
        If _items.Count > 0 Then Return
        Dim area = grid.ClientRectangle
        area.Y += grid.ColumnHeadersHeight
        area.Height -= grid.ColumnHeadersHeight
        TextRenderer.DrawText(e.Graphics, "Drag PDFs or folders here, or use Add PDF and Add Folder above.",
                              New Font("Segoe UI", 10.5F), area, _palette.Muted,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.WordBreak)
    End Sub

    Private Sub AnyDragEnter(sender As Object, e As DragEventArgs) Handles grid.DragEnter, Me.DragEnter
        e.Effect = If(Not _busy AndAlso e.Data.GetDataPresent(DataFormats.FileDrop), DragDropEffects.Copy, DragDropEffects.None)
    End Sub

    Private Async Sub AnyDragDrop(sender As Object, e As DragEventArgs) Handles grid.DragDrop, Me.DragDrop
        Dim dropped = TryCast(e.Data.GetData(DataFormats.FileDrop), String())
        If dropped IsNot Nothing AndAlso Not _busy Then Await AddPathsAsync(dropped)
    End Sub

    ' ---------------------------------------------------------------------
    ' Step 2: the operation
    ' ---------------------------------------------------------------------

    Friend ReadOnly Property CurrentOperation As PdfOperation
        Get
            If tabOperations.SelectedTab Is tabMerge Then Return PdfOperation.MergePages
            If tabOperations.SelectedTab Is tabExtract Then Return PdfOperation.ExtractPages
            If tabOperations.SelectedTab Is tabEncrypt Then Return If(rbRemovePassword.Checked, PdfOperation.Decrypt, PdfOperation.Encrypt)
            If tabOperations.SelectedTab Is tabRotate Then Return PdfOperation.Rotate
            If tabOperations.SelectedTab Is tabStamp Then Return PdfOperation.Stamp
            If tabOperations.SelectedTab Is tabCompress Then Return PdfOperation.Compress
            If tabOperations.SelectedTab Is tabArrange Then Return PdfOperation.ArrangePages
            Return PdfOperation.Combine
        End Get
    End Property

    Private Sub tabOperations_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabOperations.SelectedIndexChanged
        grid.EndEdit()
        SyncOperationButtons()
        UpdateForOperation()
    End Sub

    Private Sub UpdateForOperation()
        Dim isMerge = CurrentOperation = PdfOperation.MergePages
        colUse.Visible = isMerge
        colPagesToUse.Visible = isMerge

        Select Case CurrentOperation
            Case PdfOperation.Combine : btnRun.Text = "Combine PDFs"
            Case PdfOperation.MergePages : btnRun.Text = "Merge Pages"
            Case PdfOperation.ExtractPages : btnRun.Text = "Extract Pages"
            Case PdfOperation.Encrypt : btnRun.Text = "Encrypt PDFs"
            Case PdfOperation.Decrypt : btnRun.Text = "Remove Passwords"
            Case PdfOperation.Rotate : btnRun.Text = "Rotate PDFs"
            Case PdfOperation.Stamp
                Select Case ChosenStampMode()
                    Case StampMode.BackgroundOnly : btnRun.Text = "Add Background"
                    Case StampMode.Both : btnRun.Text = "Add Stamp && Background"
                    Case Else : btnRun.Text = "Add Stamp"
                End Select
            Case PdfOperation.Compress : btnRun.Text = "Reduce Size"
            Case PdfOperation.ArrangePages : btnRun.Text = "Save Pages"
        End Select

        Dim used = _items.Where(Function(i) i.Included).Count()
        lblMergeSummary.Text = If(_items.Count = 0, "", $"Using {used} of {_items.Count} PDFs")
        ReconcileArrangement()
        UpdateArrangeSummary()
        UpdateExtras()
        UpdateOutputDefaults()
    End Sub

    Private Sub btnUseAll_Click(sender As Object, e As EventArgs) Handles btnUseAll.Click
        SetAllIncluded(True)
    End Sub

    Private Sub btnUseNone_Click(sender As Object, e As EventArgs) Handles btnUseNone.Click
        SetAllIncluded(False)
    End Sub

    Private Sub SetAllIncluded(value As Boolean)
        grid.EndEdit()
        For Each item In _items
            item.Included = value
        Next
        UpdateForOperation()
    End Sub

    Private Sub ExtractChoiceChanged(sender As Object, e As EventArgs) Handles _
        rbExtractSelected.CheckedChanged, rbExtractAll.CheckedChanged,
        rbExtractOne.CheckedChanged, rbExtractEach.CheckedChanged, rbExtractEvery.CheckedChanged
        numSplitEvery.Enabled = rbExtractEvery.Checked
        UpdateOutputDefaults()
    End Sub

    Private Sub EncryptModeChanged(sender As Object, e As EventArgs) Handles rbAddPassword.CheckedChanged, rbRemovePassword.CheckedChanged
        If Not _ready OrElse Not DirectCast(sender, RadioButton).Checked Then Return
        UpdateEncryptRows()
        UpdateForOperation()
    End Sub

    ''' <summary>Removing a password needs none of the boxes for a new one, so they're greyed out.</summary>
    Private Sub UpdateEncryptRows()
        Dim adding = Not rbRemovePassword.Checked
        For Each control In New Control() {lblOpenPassword, txtOpenPassword, lblConfirmPassword, txtConfirmPassword, chkShowPasswords,
                                           lblPermissionsPassword, txtPermissionsPassword, lblAllow, chkAllowPrinting, chkAllowCopying, chkAllowEditing}
            control.Enabled = adding
        Next
        lblEncryptInfo.Text = If(adding,
            "Every PDF in the list gets its own protected copy, using strong AES 256-bit encryption. Your original files aren't changed. A forgotten password can't be recovered.",
            "Every locked PDF in the list gets an unlocked copy, with no password and no limits on printing, copying or editing. You're asked for any password not typed yet. Originals aren't changed; PDFs without a password are skipped.")
    End Sub

    Private Sub chkShowPasswords_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPasswords.CheckedChanged
        Dim hide = Not chkShowPasswords.Checked
        txtOpenPassword.UseSystemPasswordChar = hide
        txtConfirmPassword.UseSystemPasswordChar = hide
        txtPermissionsPassword.UseSystemPasswordChar = hide
    End Sub

    ' ---------------------------------------------------------------------
    ' Step 3: where the result goes
    ' ---------------------------------------------------------------------

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Using dialog As New FolderBrowserDialog With {
            .Description = "Choose where to save",
            .UseDescriptionForTitle = True
        }
            If Directory.Exists(txtOutputFolder.Text) Then dialog.InitialDirectory = txtOutputFolder.Text
            If dialog.ShowDialog(Me) = DialogResult.OK Then txtOutputFolder.Text = dialog.SelectedPath
        End Using
    End Sub

    Private Sub SetOutputFolderAutomatically(folder As String)
        _settingFolder = True
        txtOutputFolder.Text = folder
        _settingFolder = False
    End Sub

    Private Sub txtOutputFolder_TextChanged(sender As Object, e As EventArgs) Handles txtOutputFolder.TextChanged
        If Not _settingFolder Then _folderChosen = txtOutputFolder.Text.Trim().Length > 0
    End Sub

    Private Sub txtOutputName_TextChanged(sender As Object, e As EventArgs) Handles txtOutputName.TextChanged
        If Not _settingName Then _nameEdited = txtOutputName.Text.Trim().Length > 0
        UpdatePreview()
    End Sub

    Private Sub UpdateOutputDefaults()
        If Not _nameEdited Then
            _settingName = True
            txtOutputName.Text = DefaultOutputName()
            _settingName = False
        End If
        UpdatePreview()
    End Sub

    Private Function DefaultOutputName() As String
        Dim items = ItemsForOperation()
        Select Case CurrentOperation
            Case PdfOperation.Combine
                Return "combined"
            Case PdfOperation.MergePages
                Return "merged-pages"
            Case PdfOperation.ExtractPages
                Return If(items.Count = 1, $"{items(0).BaseName}-pages", "extracted-pages")
            Case PdfOperation.Rotate
                Return If(items.Count = 1, $"{items(0).BaseName}-rotated", "rotated")
            Case PdfOperation.Stamp
                Dim ending = If(ChosenStampMode() = StampMode.BackgroundOnly, "with-background", "stamped")
                Return If(items.Count = 1, $"{items(0).BaseName}-{ending}", ending)
            Case PdfOperation.Compress
                Return If(items.Count = 1, $"{items(0).BaseName}-smaller", "smaller")
            Case PdfOperation.ArrangePages
                Return If(items.Count = 1, $"{items(0).BaseName}-arranged", "arranged-pages")
            Case PdfOperation.Decrypt
                Return If(items.Count = 1, $"{items(0).BaseName}-unlocked", "unlocked")
            Case Else
                Return If(items.Count = 1, $"{items(0).BaseName}-protected", "protected")
        End Select
    End Function

    ' ---------------------------------------------------------------------
    ' Rotate, Stamp & Background and Reduce Size options
    ' ---------------------------------------------------------------------

    Private _stampItem As PdfItem
    Private _backgroundItem As PdfItem

    Private Function ChosenStampMode() As StampMode
        If rbStampUnder.Checked Then Return StampMode.BackgroundOnly
        If rbStampBoth.Checked Then Return StampMode.Both
        Return StampMode.StampOnly
    End Function

    Private Sub StampModeChanged(sender As Object, e As EventArgs) Handles _
        rbStampOver.CheckedChanged, rbStampUnder.CheckedChanged, rbStampBoth.CheckedChanged
        If Not _ready Then Return
        UpdateStampRows()
        UpdateForOperation()
    End Sub

    ''' <summary>Only the rows for what's being added can be used; the other is greyed out.</summary>
    Private Sub UpdateStampRows()
        Dim mode = ChosenStampMode()
        Dim stampOn = mode <> StampMode.BackgroundOnly
        Dim backgroundOn = mode <> StampMode.StampOnly
        For Each control In New Control() {lblStampFile, txtStampFile, btnStampBrowse, lblStampUse, numStampPage}
            control.Enabled = stampOn
        Next
        For Each control In New Control() {lblBackgroundFile, txtBackgroundFile, btnBackgroundBrowse, lblBackgroundUse, numBackgroundPage}
            control.Enabled = backgroundOn
        Next
    End Sub

    Private Async Sub btnStampBrowse_Click(sender As Object, e As EventArgs) Handles btnStampBrowse.Click
        Dim chosen = ChoosePdf("Choose the stamp PDF")
        If chosen IsNot Nothing Then Await SetStampFileAsync(chosen)
    End Sub

    Private Async Sub btnBackgroundBrowse_Click(sender As Object, e As EventArgs) Handles btnBackgroundBrowse.Click
        Dim chosen = ChoosePdf("Choose the background PDF")
        If chosen IsNot Nothing Then Await SetBackgroundFileAsync(chosen)
    End Sub

    Private Function ChoosePdf(title As String) As String
        Using dialog As New OpenFileDialog With {
            .Title = title,
            .Filter = "PDF files (*.pdf)|*.pdf",
            .CheckFileExists = True
        }
            If Directory.Exists(_settings.LastInputFolder) Then dialog.InitialDirectory = _settings.LastInputFolder
            Return If(dialog.ShowDialog(Me) = DialogResult.OK, dialog.FileName, Nothing)
        End Using
    End Function

    ''' <summary>Chooses the PDF to stamp with, reads its page count and asks for its password if it has one.</summary>
    Friend Async Function SetStampFileAsync(filePath As String) As Task
        _stampItem = New PdfItem(filePath)
        txtStampFile.Text = _stampItem.Path
        Await ReadLayerAsync(_stampItem, numStampPage)
    End Function

    ''' <summary>Chooses the PDF to use as a background, the same way.</summary>
    Friend Async Function SetBackgroundFileAsync(filePath As String) As Task
        _backgroundItem = New PdfItem(filePath)
        txtBackgroundFile.Text = _backgroundItem.Path
        Await ReadLayerAsync(_backgroundItem, numBackgroundPage)
    End Function

    Private Async Function ReadLayerAsync(layer As PdfItem, pageBox As NumericUpDown) As Task
        If _tool Is Nothing Then Return
        Await PdfInspector.InspectAsync(_tool, layer)
        If layer.State = PdfState.PasswordNeeded Then Await UnlockAsync(layer)

        Select Case layer.State
            Case PdfState.Ready
                pageBox.Maximum = Math.Max(1, layer.PageCount.GetValueOrDefault(1))
                Dim pages = If(layer.PageCount = 1, "1 page", $"{layer.PageCount} pages")
                ShowStatus($"{layer.FileName} has {pages}.", StatusKind.Normal)
            Case PdfState.PasswordNeeded
                ShowStatus($"{layer.FileName} needs its password before it can be used.", StatusKind.Failure)
            Case Else
                ShowStatus($"{layer.FileName} can't be read: {layer.Problem}", StatusKind.Failure)
        End Select
    End Function

    Private Function ChosenCompressLevel() As CompressLevel
        If rbCompressNormal.Checked Then Return CompressLevel.Normal
        If rbCompressHigh.Checked Then Return CompressLevel.High
        If rbCompressExtreme.Checked Then Return CompressLevel.Extreme
        Return CompressLevel.Medium
    End Function

    ' ---------------------------------------------------------------------
    ' Arrange Pages: the page thumbnail view
    ' ---------------------------------------------------------------------

    ''' <summary>The pages as arranged in the page view, or Nothing for every page in list order.</summary>
    Private _arrangement As List(Of PageRef)
    ''' <summary>The PDFs the arrangement already knows about; PDFs added later join it at the end.</summary>
    Private ReadOnly _arrangedFiles As New HashSet(Of PdfItem)
    Private _pageView As PageViewForm

    Friend ReadOnly Property Arrangement As List(Of PageRef)
        Get
            Return _arrangement
        End Get
    End Property

    ''' <summary>The PDFs whose pages can be shown, in list order.</summary>
    Private Function ReadyItems() As List(Of PdfItem)
        Return _items.Where(Function(i) i.State = PdfState.Ready AndAlso i.PageCount.GetValueOrDefault(0) > 0).ToList()
    End Function

    ''' <summary>Keeps the arrangement in step with the list: removed PDFs leave it, new PDFs join at the end.</summary>
    Private Sub ReconcileArrangement()
        If _arrangement Is Nothing Then Return
        Dim inList As New HashSet(Of PdfItem)(_items)
        _arrangement.RemoveAll(Function(p) Not inList.Contains(p.Item) OrElse
                                           (p.Item.PageCount.HasValue AndAlso p.PageNumber > p.Item.PageCount.Value))
        _arrangedFiles.IntersectWith(inList)
        For Each item In ReadyItems()
            If _arrangedFiles.Add(item) Then
                _arrangement.AddRange(Enumerable.Range(1, item.PageCount.Value).Select(Function(n) New PageRef(item, n)))
            End If
        Next
    End Sub

    Private Sub UpdateArrangeSummary()
        btnArrangeReset.Enabled = _arrangement IsNot Nothing
        If _items.Count = 0 Then
            lblArrangeSummary.Text = "Add PDFs in step 1 first."
        ElseIf _arrangement Is Nothing Then
            Dim total = ReadyItems().Sum(Function(i) i.PageCount.Value)
            lblArrangeSummary.Text = $"Not arranged yet: all {If(total = 1, "1 page", $"{total} pages")}, in list order."
        Else
            lblArrangeSummary.Text = PageViewForm.Describe(_arrangement, ReadyItems())
        End If
    End Sub

    ''' <summary>Takes in the pages chosen in the page view. The same pages in list order means "not arranged".</summary>
    Friend Sub UseArrangement(pages As IEnumerable(Of PageRef))
        Dim chosen = pages.ToList()
        Dim files = ReadyItems()
        Dim untouched = OperationRequest.AllPages(files)
        Dim same = chosen.Count = untouched.Count AndAlso
                   chosen.Zip(untouched).All(Function(pair) pair.First.Item Is pair.Second.Item AndAlso pair.First.PageNumber = pair.Second.PageNumber)
        _arrangement = If(same, Nothing, chosen)
        _arrangedFiles.Clear()
        _arrangedFiles.UnionWith(files)
        UpdateForOperation()
    End Sub

    ''' <summary>Reads any PDFs still being read and asks for passwords, then makes the page view. Nothing when there's nothing to show.</summary>
    Friend Async Function CreatePageViewAsync() As Task(Of PageViewForm)
        If _tool Is Nothing Then
            ShowStatus("QPDF wasn't found. Use Find QPDF at the top of the window.", StatusKind.Failure)
            Return Nothing
        End If
        Await InspectItemsAsync(_items.Where(Function(i) i.State = PdfState.Checking).ToList())
        For Each item In _items.Where(Function(i) i.State = PdfState.PasswordNeeded).ToList()
            Await UnlockAsync(item)
        Next
        UpdateForOperation()

        Dim files = ReadyItems()
        If files.Count = 0 Then
            ShowStatus(If(_items.Count = 0, "Add PDFs first, then show their pages.", "None of the PDFs in the list can be read."), StatusKind.Failure)
            FocusField(RequestField.FileList)
            Return Nothing
        End If
        Dim skipped = _items.Count - files.Count
        ShowStatus(If(skipped > 0, $"{skipped} PDF(s) that can't be read are left out of the page view.", ""), StatusKind.Normal)

        Dim view As New PageViewForm(files, _arrangement, _tool) With {
            .Icon = Icon,
            .QuietMode = QuietMode OrElse _screenshotPath IsNot Nothing
        }
        view.ApplyPalette(_palette)
        Return view
    End Function

    Private Async Sub btnShowPages_Click(sender As Object, e As EventArgs) Handles btnShowPages.Click
        If _busy Then Return
        Dim view = Await CreatePageViewAsync()
        If view Is Nothing Then Return
        Using view
            _pageView = view
            If view.ShowDialog(Me) = DialogResult.OK Then
                UseArrangement(view.Arrangement)
                ShowStatus("Pages arranged. Choose where to save, then press Save Pages.", StatusKind.Normal)
            End If
            _pageView = Nothing
        End Using
    End Sub

    Private Sub btnArrangeReset_Click(sender As Object, e As EventArgs) Handles btnArrangeReset.Click
        _arrangement = Nothing
        _arrangedFiles.Clear()
        UpdateForOperation()
        ShowStatus("Back to every page, in list order.", StatusKind.Normal)
    End Sub

    ''' <summary>--show-pages: the page view opens by itself once the PDFs are read (for pictures of it).</summary>
    Private Async Sub ShowPagesAtStart()
        Await StartupTask
        Dim view = Await CreatePageViewAsync()
        If view Is Nothing Then Return
        _pageView = view
        view.StartPosition = FormStartPosition.Manual
        view.Location = Location
        view.Show()
        If _screenshotPath IsNot Nothing Then
            SetWindowPos(view.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE Or SWP_NOACTIVATE)
        End If
    End Sub

    Private Sub UpdatePreview()
        If _items.Count = 0 Then
            lblPreview.Text = ""
            Return
        End If
        Dim request = ReadRequest()
        Dim text = Planner.DescribeOutputs(request)
        Dim extras As New List(Of String)
        If request.UsesExtraLayers Then extras.Add(DescribeLayers(request.ExtraLayers))
        If request.UsesExtraProtection Then extras.Add("protected with a password")
        If text.Length > 0 AndAlso extras.Count > 0 Then text &= "  ·  " & String.Join(", ", extras)
        lblPreview.Text = text
    End Sub

    ' ---------------------------------------------------------------------
    ' Step 3 extras: a password and/or a stamp / background on every new PDF
    ' ---------------------------------------------------------------------

    Private _extraProtection As ProtectionSettings
    Private _extraLayers As LayerSettings
    Private _settingExtras As Boolean

    ''' <summary>Encrypt / Decrypt has its own password settings and Stamp &amp; Background its own layers, so those extras are greyed out there.</summary>
    Private Sub UpdateExtras()
        Dim op = CurrentOperation
        Dim passwordOk = op <> PdfOperation.Encrypt AndAlso op <> PdfOperation.Decrypt
        Dim stampOk = op <> PdfOperation.Stamp
        chkExtraPassword.Enabled = passwordOk
        btnExtraPassword.Enabled = passwordOk
        chkExtraStamp.Enabled = stampOk
        btnExtraStamp.Enabled = stampOk

        Dim parts As New List(Of String)
        If chkExtraPassword.Checked AndAlso passwordOk AndAlso _extraProtection IsNot Nothing Then
            Dim p = _extraProtection
            parts.Add(If(p.OpenPassword.Length > 0, "Password set", "Limits set") &
                      If(p.AllowPrinting AndAlso p.AllowCopying AndAlso p.AllowEditing, "", " (with limits)"))
        End If
        If chkExtraStamp.Checked AndAlso stampOk AndAlso _extraLayers IsNot Nothing Then parts.Add(DescribeLayers(_extraLayers))
        If Not passwordOk AndAlso chkExtraPassword.Checked Then parts.Add("Encrypt / Decrypt uses its own password settings")
        If Not stampOk AndAlso chkExtraStamp.Checked Then parts.Add("Stamp & Background uses its own settings above")
        lblExtrasSummary.Text = String.Join("  ·  ", parts)
    End Sub

    Private Shared Function DescribeLayers(layers As LayerSettings) As String
        Dim what As New List(Of String)
        If layers.UsesStamp AndAlso layers.StampItem IsNot Nothing Then what.Add($"stamp {layers.StampItem.FileName}")
        If layers.UsesBackground AndAlso layers.BackgroundItem IsNot Nothing Then what.Add($"background {layers.BackgroundItem.FileName}")
        Dim pages = If(PageRange.IsAll(layers.Pages), "all pages", $"pages {layers.Pages.Trim()}")
        Return $"with {String.Join(" and ", what)} on {pages}"
    End Function

    ''' <summary>Opens the password settings; True when the person pressed OK.</summary>
    Friend Function EditExtraPassword() As Boolean
        Using dialog As New ProtectDialog(_extraProtection) With {.Icon = Icon}
            dialog.ApplyPalette(_palette)
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return False
            _extraProtection = dialog.Settings
        End Using
        Return True
    End Function

    ''' <summary>Opens the stamp / background settings; True when the person pressed OK.</summary>
    Friend Function EditExtraLayers() As Boolean
        Using dialog As New LayersDialog(_extraLayers, AddressOf ReadExtraLayerAsync) With {.Icon = Icon, .StartFolder = _settings.LastInputFolder}
            dialog.ApplyPalette(_palette)
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return False
            _extraLayers = dialog.Settings
        End Using
        Return True
    End Function

    ''' <summary>Sets the extras directly (for the tests).</summary>
    Friend Sub SetExtras(protection As ProtectionSettings, layers As LayerSettings)
        _extraProtection = protection
        _extraLayers = layers
        _settingExtras = True
        chkExtraPassword.Checked = protection IsNot Nothing
        chkExtraStamp.Checked = layers IsNot Nothing
        _settingExtras = False
        UpdateForOperation()
    End Sub

    ''' <summary>Reads a stamp or background PDF chosen in step 3's settings, asking for its password if it has one.</summary>
    Friend Async Function ReadExtraLayerAsync(filePath As String) As Task(Of PdfItem)
        Dim item As New PdfItem(filePath)
        If _tool Is Nothing Then Return item
        Await PdfInspector.InspectAsync(_tool, item)
        If item.State = PdfState.PasswordNeeded Then Await UnlockAsync(item)
        Return item
    End Function

    ' Ticking an extra for the first time asks for its settings; cancelling leaves it unticked.
    Private Sub chkExtraPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkExtraPassword.CheckedChanged
        If Not _ready OrElse _settingExtras Then Return
        If chkExtraPassword.Checked AndAlso _extraProtection Is Nothing AndAlso Not EditExtraPassword() Then
            _settingExtras = True
            chkExtraPassword.Checked = False
            _settingExtras = False
        End If
        UpdateForOperation()
    End Sub

    Private Sub chkExtraStamp_CheckedChanged(sender As Object, e As EventArgs) Handles chkExtraStamp.CheckedChanged
        If Not _ready OrElse _settingExtras Then Return
        If chkExtraStamp.Checked AndAlso _extraLayers Is Nothing AndAlso Not EditExtraLayers() Then
            _settingExtras = True
            chkExtraStamp.Checked = False
            _settingExtras = False
        End If
        UpdateForOperation()
    End Sub

    Private Sub btnExtraPassword_Click(sender As Object, e As EventArgs) Handles btnExtraPassword.Click
        If EditExtraPassword() Then
            _settingExtras = True
            chkExtraPassword.Checked = True
            _settingExtras = False
        End If
        UpdateForOperation()
    End Sub

    Private Sub btnExtraStamp_Click(sender As Object, e As EventArgs) Handles btnExtraStamp.Click
        If EditExtraLayers() Then
            _settingExtras = True
            chkExtraStamp.Checked = True
            _settingExtras = False
        End If
        UpdateForOperation()
    End Sub

    ' ---------------------------------------------------------------------
    ' Running it
    ' ---------------------------------------------------------------------

    ''' <summary>The PDFs this operation will work on, in list order.</summary>
    Private Function ItemsForOperation() As List(Of PdfItem)
        Select Case CurrentOperation
            Case PdfOperation.MergePages
                Return _items.Where(Function(i) i.Included).ToList()
            Case PdfOperation.ExtractPages
                If rbExtractAll.Checked Then Return _items.ToList()
                Return SelectedIndexes().Where(Function(i) i < _items.Count).Select(Function(i) _items(i)).ToList()
            Case PdfOperation.Decrypt
                ' PDFs known to have no password or limits are left out.
                Return _items.Where(Function(i) i.IsEncrypted.GetValueOrDefault(True)).ToList()
            Case Else
                Return _items.ToList()
        End Select
    End Function

    Friend Function ReadRequest() As OperationRequest
        Return New OperationRequest With {
            .Operation = CurrentOperation,
            .Items = ItemsForOperation(),
            .OutputFolder = txtOutputFolder.Text,
            .OutputName = txtOutputName.Text,
            .KeepFirstFileDetails = chkKeepDetails.Checked,
            .ExtractPages = txtExtractPages.Text,
            .ExtractOutput = If(rbExtractEach.Checked, ExtractOutput.EachPage,
                                If(rbExtractEvery.Checked, ExtractOutput.EveryNPages, ExtractOutput.OnePdf)),
            .SplitEvery = CInt(numSplitEvery.Value),
            .OpenPassword = txtOpenPassword.Text,
            .ConfirmPassword = txtConfirmPassword.Text,
            .PermissionsPassword = txtPermissionsPassword.Text,
            .AllowPrinting = chkAllowPrinting.Checked,
            .AllowCopying = chkAllowCopying.Checked,
            .AllowEditing = chkAllowEditing.Checked,
            .RotateAngle = If(rbRotateLeft.Checked, -90, If(rbRotate180.Checked, 180, 90)),
            .RotatePages = txtRotatePages.Text,
            .StampMode = ChosenStampMode(),
            .StampItem = _stampItem,
            .BackgroundItem = _backgroundItem,
            .BackgroundPageNumber = CInt(numBackgroundPage.Value),
            .StampPages = txtStampPages.Text,
            .StampPageNumber = CInt(numStampPage.Value),
            .CompressLevel = ChosenCompressLevel(),
            .ArrangedPages = _arrangement,
            .ExtraProtection = If(chkExtraPassword.Checked, _extraProtection, Nothing),
            .ExtraLayers = If(chkExtraStamp.Checked, _extraLayers, Nothing)
        }
    End Function

    Private Async Sub btnRun_Click(sender As Object, e As EventArgs) Handles btnRun.Click
        Try
            Await RunOperationAsync()
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is Win32Exception OrElse TypeOf ex Is UnauthorizedAccessException
            SetBusy(False)
            AddToLog(ex.ToString())
            ShowStatus("Something went wrong: " & ex.Message, StatusKind.Failure)
            ShowDetails(True)
        End Try
    End Sub

    Friend Async Function RunOperationAsync() As Task
        If _busy Then Return
        grid.EndEdit()
        ShowStatus("", StatusKind.Normal)

        If _tool Is Nothing Then
            ShowStatus("QPDF wasn't found. Use Find QPDF at the top of the window.", StatusKind.Failure)
            Return
        End If

        ' Remove the password: first find out which PDFs have one (or limits) at all.
        Dim skipped = 0
        If CurrentOperation = PdfOperation.Decrypt Then
            For Each item In _items.Where(Function(i) i.State = PdfState.PasswordNeeded)
                item.IsEncrypted = True
            Next
            Dim unknown = _items.Where(Function(i) i.State = PdfState.Ready AndAlso Not i.IsEncrypted.HasValue).ToList()
            If unknown.Count > 0 Then
                ShowStatus("Checking which PDFs have a password…", StatusKind.Normal)
                Await Task.WhenAll(unknown.Select(Function(i) PdfInspector.CheckEncryptionAsync(_tool, i)))
            End If
            skipped = _items.Where(Function(i) i.IsEncrypted.HasValue AndAlso Not i.IsEncrypted.Value).Count()
            If _items.Count > 0 AndAlso skipped = _items.Count Then
                ShowStatus(If(skipped = 1, "This PDF has no password or limits to remove.",
                              "None of these PDFs has a password or limits to remove."), StatusKind.Failure)
                FocusField(RequestField.FileList)
                UpdateForOperation()
                Return
            End If
            UpdateForOperation()
        End If

        Dim request = ReadRequest()
        For Each item In request.Items.Where(Function(i) i.State = PdfState.PasswordNeeded).ToList()
            Await UnlockAsync(item)
        Next
        If request.Operation = PdfOperation.Stamp Then
            Dim layers As New List(Of PdfItem)
            If request.UsesStamp AndAlso request.StampItem IsNot Nothing Then layers.Add(request.StampItem)
            If request.UsesBackground AndAlso request.BackgroundItem IsNot Nothing Then layers.Add(request.BackgroundItem)
            For Each layer In layers
                If layer.State = PdfState.Checking Then Await PdfInspector.InspectAsync(_tool, layer)
                If layer.State = PdfState.PasswordNeeded Then Await UnlockAsync(layer)
            Next
        End If

        Dim issue = Planner.Validate(request)
        If issue IsNot Nothing Then
            ShowStatus(issue.Message, StatusKind.Failure)
            FocusField(issue.Field)
            Return
        End If

        Dim conflicts = Planner.FindConflicts(request)
        If conflicts.Count > 0 AndAlso Not Prompts.ConfirmReplace(conflicts) Then
            ShowStatus("Nothing was changed. Choose another name or folder.", StatusKind.Normal)
            FocusField(RequestField.OutputName)
            Return
        End If

        txtLog.Clear()
        _lastOutputs = New List(Of String)
        Dim runner As New OperationRunner(_tool, AddressOf AddToLog, AddressOf ShowProgress)
        Dim plan = Planner.Plan(request)

        Using cts As New CancellationTokenSource()
            _cts = cts
            SetBusy(True)
            ShowStatus("Working…", StatusKind.Normal)
            Dim result As OperationResult
            Try
                result = Await runner.RunAsync(plan, cts.Token)
            Finally
                _cts = Nothing
                SetBusy(False)
            End Try
            If skipped > 0 AndAlso result.Outcome = RunOutcome.Succeeded Then
                result.Message &= If(skipped = 1, " 1 PDF had no password and was skipped.", $" {skipped} PDFs had no password and were skipped.")
            End If
            ShowResult(result)
        End Using

        If _closeAfterRun Then Close()
    End Function

    Private Sub ShowResult(result As OperationResult)
        Select Case result.Outcome
            Case RunOutcome.Succeeded
                _lastOutputs = result.OutputFiles
                ShowStatus(result.Message, StatusKind.Success)
                If result.HadWarnings Then ShowDetails(True)
            Case RunOutcome.Cancelled
                ShowStatus(result.Message, StatusKind.Normal)
            Case Else
                ShowStatus(result.Message, StatusKind.Failure)
                ShowDetails(True)
        End Select
        btnOpenFile.Enabled = _lastOutputs.Count > 0
        btnOpenFolder.Enabled = _lastOutputs.Count > 0
    End Sub

    Private Sub SetBusy(busy As Boolean)
        _busy = busy
        flowToolbar.Enabled = Not busy
        grid.Enabled = Not busy
        tabOperations.Enabled = Not busy
        tblOutput.Enabled = Not busy
        progressBar.Visible = busy
        btnCancel.Visible = busy
        btnCancel.Enabled = True
        btnRun.Visible = Not busy
        If busy Then
            btnOpenFile.Enabled = False
            btnOpenFolder.Enabled = False
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        btnCancel.Enabled = False
        ShowStatus("Cancelling…", StatusKind.Normal)
        _cts?.Cancel()
    End Sub

    Private Sub FocusField(field As RequestField)
        Select Case field
            Case RequestField.FileList : grid.Focus()
            Case RequestField.OutputFolder : txtOutputFolder.Focus()
            Case RequestField.OutputName : txtOutputName.Focus()
            Case RequestField.ExtractPages : txtExtractPages.Focus()
            Case RequestField.OpenPassword : txtOpenPassword.Focus()
            Case RequestField.ConfirmPassword : txtConfirmPassword.Focus()
            Case RequestField.PermissionsPassword : txtPermissionsPassword.Focus()
            Case RequestField.RotatePages : txtRotatePages.Focus()
            Case RequestField.StampFile : btnStampBrowse.Focus()
            Case RequestField.BackgroundFile : btnBackgroundBrowse.Focus()
            Case RequestField.StampPages : txtStampPages.Focus()
            Case RequestField.ExtraPassword : btnExtraPassword.Focus()
            Case RequestField.ExtraStamp : btnExtraStamp.Focus()
        End Select
    End Sub

    Private Sub ShowStatus(message As String, kind As StatusKind)
        LastStatusKind = kind
        lblStatus.Text = message
        lblStatus.ForeColor = If(kind = StatusKind.Success, _palette.SuccessText,
                                 If(kind = StatusKind.Failure, _palette.ErrorText, _palette.Text))
        lblStatus.Font = New Font("Segoe UI", 10.0F, If(kind = StatusKind.Normal, FontStyle.Regular, FontStyle.Bold))
    End Sub

    ''' <summary>While a batch runs: how many of its files QPDF has finished.</summary>
    Private Sub ShowProgress(finished As Integer, total As Integer)
        If IsDisposed Then Return
        If InvokeRequired Then
            BeginInvoke(New Action(Of Integer, Integer)(AddressOf ShowProgress), finished, total)
            Return
        End If
        If Not _busy OrElse total < 2 Then Return
        If btnCancel.Enabled Then ShowStatus($"Working… {finished} of {total} done", StatusKind.Normal)
    End Sub

    Private Sub AddToLog(text As String)
        If IsDisposed Then Return
        If InvokeRequired Then
            BeginInvoke(New Action(Of String)(AddressOf AddToLog), text)
            Return
        End If
        txtLog.AppendText(text & Environment.NewLine)
    End Sub

    Private Sub ShowDetails(visible As Boolean)
        txtLog.Visible = visible
        lnkDetails.Text = If(visible, "Hide details", "Show details")
    End Sub

    Private Sub lnkDetails_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles lnkDetails.LinkClicked
        ShowDetails(Not txtLog.Visible)
    End Sub

    Private Sub btnOpenFile_Click(sender As Object, e As EventArgs) Handles btnOpenFile.Click
        If _lastOutputs.Count > 0 Then OpenWithShell(_lastOutputs(0))
    End Sub

    Private Sub btnOpenFolder_Click(sender As Object, e As EventArgs) Handles btnOpenFolder.Click
        If _lastOutputs.Count > 0 Then OpenWithShell(Path.GetDirectoryName(_lastOutputs(0)))
    End Sub

    Private Sub OpenWithShell(target As String)
        Try
            Process.Start(New ProcessStartInfo(target) With {.UseShellExecute = True})?.Dispose()
        Catch ex As Exception When TypeOf ex Is Win32Exception OrElse TypeOf ex Is InvalidOperationException OrElse TypeOf ex Is FileNotFoundException
            ShowStatus($"Couldn't open {target}: {ex.Message}", StatusKind.Failure)
        End Try
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If _busy Then
            ' Stop QPDF first; the window closes as soon as it has.
            e.Cancel = True
            _closeAfterRun = True
            _cts?.Cancel()
            Return
        End If
        If PersistSettings Then
            _settings.IncludeSubfolders = chkSubfolders.Checked
            _settings.Save()
        End If
    End Sub

    ' ---------------------------------------------------------------------
    ' Sorting names the way people expect: page2 before page10
    ' ---------------------------------------------------------------------

    <DllImport("shlwapi.dll", CharSet:=CharSet.Unicode)>
    Private Shared Function StrCmpLogicalW(a As String, b As String) As Integer
    End Function

    Private Shared Function NaturalCompare(a As String, b As String) As Integer
        Return StrCmpLogicalW(a, b)
    End Function

    ' ---------------------------------------------------------------------
    ' The --screenshot option: open quietly behind other windows, save a picture, exit
    ' ---------------------------------------------------------------------

    Private Const WS_EX_NOACTIVATE As Integer = &H8000000
    Private Const SWP_NOSIZE As UInteger = &H1
    Private Const SWP_NOMOVE As UInteger = &H2
    Private Const SWP_NOACTIVATE As UInteger = &H10
    Private Const PW_RENDERFULLCONTENT As UInteger = &H2
    Private Shared ReadOnly HWND_BOTTOM As New IntPtr(1)

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowPos(hWnd As IntPtr, insertAfter As IntPtr, x As Integer, y As Integer, cx As Integer, cy As Integer, flags As UInteger) As Boolean
    End Function

    <DllImport("user32.dll")>
    Private Shared Function PrintWindow(hWnd As IntPtr, hdc As IntPtr, flags As UInteger) As Boolean
    End Function

    ''' <summary>Opens without taking focus. Used by the screenshot option and by the tests.</summary>
    Friend QuietMode As Boolean

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return QuietMode OrElse _screenshotPath IsNot Nothing
        End Get
    End Property

    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim params = MyBase.CreateParams
            If QuietMode OrElse _screenshotPath IsNot Nothing Then params.ExStyle = params.ExStyle Or WS_EX_NOACTIVATE
            Return params
        End Get
    End Property

    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        If _screenshotPath IsNot Nothing Then
            SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE Or SWP_NOACTIVATE)
            screenshotTimer.Start()
        End If
        If _startupHelpTopic IsNot Nothing Then
            ShowHelp(If(_startupHelpTopic.Length = 0, Nothing, _startupHelpTopic))
        End If
        If _startup IsNot Nothing AndAlso _startup.ShowPages Then ShowPagesAtStart()
    End Sub

    ' ---------------------------------------------------------------------
    ' Help
    ' ---------------------------------------------------------------------

    Private _helpForm As HelpForm
    Private _startupHelpTopic As String

    ''' <summary>Opens the Help window (or brings it forward) on a topic, or on the last topic shown.</summary>
    Friend Function ShowHelp(Optional topic As String = Nothing) As HelpForm
        If _helpForm Is Nothing OrElse _helpForm.IsDisposed Then
            _helpForm = New HelpForm(Function() lblQpdfStatus.Text) With {
                .QuietMode = QuietMode OrElse _screenshotPath IsNot Nothing,
                .Icon = Icon
            }
            _helpForm.ApplyPalette(_palette)
            If _screenshotPath IsNot Nothing Then
                ' A picture is being taken: keep the window out of the way, like the main one.
                _helpForm.StartPosition = FormStartPosition.Manual
                _helpForm.Location = Location
                _helpForm.Show()
                SetWindowPos(_helpForm.Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE Or SWP_NOSIZE Or SWP_NOACTIVATE)
            Else
                _helpForm.Show(Me)
            End If
        ElseIf Not QuietMode AndAlso _screenshotPath Is Nothing Then
            _helpForm.Activate()
        End If
        If topic IsNot Nothing Then _helpForm.ShowTopic(topic)
        Return _helpForm
    End Function

    ''' <summary>The Help topic about the operation on show.</summary>
    Private Function TopicForCurrentOperation() As String
        Select Case CurrentOperation
            Case PdfOperation.MergePages : Return HelpForm.TopicMerge
            Case PdfOperation.ExtractPages : Return HelpForm.TopicExtract
            Case PdfOperation.Encrypt, PdfOperation.Decrypt : Return HelpForm.TopicEncrypt
            Case PdfOperation.Rotate : Return HelpForm.TopicRotate
            Case PdfOperation.Stamp : Return HelpForm.TopicStamp
            Case PdfOperation.Compress : Return HelpForm.TopicCompress
            Case PdfOperation.ArrangePages : Return HelpForm.TopicArrange
            Case Else : Return HelpForm.TopicCombine
        End Select
    End Function

    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        ShowHelp()
    End Sub

    Private Sub PageRangeHelp_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles _
        lnkMergeRangeHelp.LinkClicked, lnkExtractRangeHelp.LinkClicked, lnkRotateRangeHelp.LinkClicked, lnkStampRangeHelp.LinkClicked
        ShowHelp(HelpForm.TopicPageRanges)
    End Sub

    ' F1 opens Help on the operation being used, from anywhere in the window.
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        If keyData = Keys.F1 Then
            ShowHelp(TopicForCurrentOperation())
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub screenshotTimer_Tick(sender As Object, e As EventArgs) Handles screenshotTimer.Tick
        _frames += 1
        Invalidate(True)
        If _frames < ScreenshotFrames Then Return
        ' With --show-pages, wait (up to about 10 seconds more) for the page view and its pictures.
        If _startup IsNot Nothing AndAlso _startup.ShowPages AndAlso _frames < ScreenshotFrames * 5 Then
            If _pageView Is Nothing OrElse Not _pageView.Grid.VisiblePicturesReady Then Return
        End If

        screenshotTimer.Stop()
        Try
            SaveScreenshot(_screenshotPath)
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is ExternalException OrElse TypeOf ex Is ArgumentException
            Console.Error.WriteLine("Couldn't save the screenshot: " & ex.Message)
            Environment.ExitCode = 1
        End Try
        Close()
    End Sub

    Private Sub SaveScreenshot(filePath As String)
        Dim fullPath = Path.GetFullPath(filePath)
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath))
        ' With --show-help the picture is of the Help window.
        Dim target As Form = Me
        If _helpForm IsNot Nothing AndAlso Not _helpForm.IsDisposed AndAlso _helpForm.Visible Then target = _helpForm
        ' With --show-pages it is of the page view.
        If _pageView IsNot Nothing AndAlso Not _pageView.IsDisposed AndAlso _pageView.Visible Then target = _pageView
        target.Update()
        Using picture As New Bitmap(target.Width, target.Height, PixelFormat.Format32bppArgb)
            Using canvas = Graphics.FromImage(picture)
                Dim hdc = canvas.GetHdc()
                Try
                    PrintWindow(target.Handle, hdc, PW_RENDERFULLCONTENT)
                Finally
                    canvas.ReleaseHdc(hdc)
                End Try
            End Using
            picture.Save(fullPath, ImageFormat.Png)
        End Using
    End Sub

End Class
