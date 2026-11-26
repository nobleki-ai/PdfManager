Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' The page thumbnail view for Arrange Pages: every page of every PDF in the list as a small
''' picture, to drag into a new order or remove. Nothing is saved here; "Use These Pages" hands
''' the new order back to the main window, and its big blue button saves it with QPDF.
''' </summary>
Public Class PageViewForm

    Private ReadOnly _files As List(Of PdfItem)
    Private ReadOnly _renderer As PageRenderer
    Private _palette As Palette = Palette.LightPalette

    ''' <summary>For the form designer only.</summary>
    Public Sub New()
        Me.New(New List(Of PdfItem), Nothing, Nothing)
    End Sub

    ''' <param name="files">The PDFs in the list, in list order.</param>
    ''' <param name="current">The pages as last arranged, or Nothing for every page in list order.</param>
    ''' <param name="tool">QPDF, so locked PDFs can be shown too.</param>
    Public Sub New(files As IEnumerable(Of PdfItem), current As IEnumerable(Of PageRef), tool As QpdfTool)
        InitializeComponent()
        _renderer = New PageRenderer(tool)
        _files = files.ToList()
        pageGrid.Renderer = _renderer
        pageGrid.SetPages(If(current, OperationRequest.AllPages(_files)), _files)
        trkSize.Value = Math.Clamp(pageGrid.ThumbWidth, trkSize.Minimum, trkSize.Maximum)
        UpdateState()
    End Sub

    ''' <summary>Opens without taking focus. Used by the screenshot option and by the tests.</summary>
    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Friend Property QuietMode As Boolean

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return QuietMode
        End Get
    End Property

    ''' <summary>The pages to keep, in their new order.</summary>
    Public ReadOnly Property Arrangement As List(Of PageRef)
        Get
            Return pageGrid.Pages.ToList()
        End Get
    End Property

    ''' <summary>The grid, for the tests.</summary>
    Friend ReadOnly Property Grid As PageGrid
        Get
            Return pageGrid
        End Get
    End Property

    Friend Sub ApplyPalette(p As Palette)
        _palette = p
        Theme.Apply(Me, p)
        pageGrid.ThemePalette = p
        Dim small = LogicalToDeviceUnits(18)
        Theme.SetIcon(btnRemove, ChrW(&HE74D), small)
        Theme.SetIcon(btnSelectAll, ChrW(&HE8B3), small)
        Theme.SetIcon(btnMoveFirst, ChrW(&HE892), small)
        Theme.SetIcon(btnMoveLast, ChrW(&HE893), small)
        Theme.SetIcon(btnReset, ChrW(&HE72C), small)
        Theme.UseDarkTitleBar(Me, p.IsDark)
        UpdateState()
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        Theme.UseDarkTitleBar(Me, _palette.IsDark)
    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)
        ' Roomy on a big screen, never bigger than the screen.
        Dim area = Screen.FromControl(If(Owner, CType(Me, Control))).WorkingArea
        Size = New Size(Math.Min(LogicalToDeviceUnits(1080), area.Width - 40), Math.Min(LogicalToDeviceUnits(780), area.Height - 40))
        If StartPosition = FormStartPosition.CenterParent AndAlso Owner IsNot Nothing Then
            Location = New Point(Owner.Left + (Owner.Width - Width) \ 2, Owner.Top + (Owner.Height - Height) \ 2)
        End If
        lblHint.MaximumSize = New Size(ClientSize.Width - LogicalToDeviceUnits(40), 0)
        pageGrid.Focus()
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If lblHint IsNot Nothing Then lblHint.MaximumSize = New Size(Math.Max(200, ClientSize.Width - LogicalToDeviceUnits(40)), 0)
    End Sub

    ' ---------------------------------------------------------------------
    ' Buttons
    ' ---------------------------------------------------------------------

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        pageGrid.RemoveSelected()
        pageGrid.Focus()
    End Sub

    Private Sub btnSelectAll_Click(sender As Object, e As EventArgs) Handles btnSelectAll.Click
        pageGrid.SelectAll()
        pageGrid.Focus()
    End Sub

    Private Sub btnMoveFirst_Click(sender As Object, e As EventArgs) Handles btnMoveFirst.Click
        pageGrid.MoveSelectedTo(0)
        pageGrid.Focus()
    End Sub

    Private Sub btnMoveLast_Click(sender As Object, e As EventArgs) Handles btnMoveLast.Click
        pageGrid.MoveSelectedTo(pageGrid.Pages.Count)
        pageGrid.Focus()
    End Sub

    ''' <summary>Every page back, in list order.</summary>
    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        pageGrid.SetPages(OperationRequest.AllPages(_files), _files)
        UpdateState()
        pageGrid.Focus()
    End Sub

    Private Sub trkSize_ValueChanged(sender As Object, e As EventArgs) Handles trkSize.ValueChanged
        pageGrid.ThumbWidth = trkSize.Value
    End Sub

    Private Sub btnDone_Click(sender As Object, e As EventArgs) Handles btnDone.Click
        If pageGrid.Pages.Count = 0 Then
            lblSummary.Text = "Keep at least one page, or use Start Over to bring them all back."
            lblSummary.ForeColor = _palette.ErrorText
            Return
        End If
        DialogResult = DialogResult.OK
        If Not Modal Then Close()
    End Sub

    Private Sub pageGrid_Changed(sender As Object, e As EventArgs) Handles pageGrid.PagesChanged, pageGrid.SelectionChanged
        UpdateState()
    End Sub

    ''' <summary>The summary line and which buttons can be used.</summary>
    Private Sub UpdateState()
        Dim pages = pageGrid.Pages
        Dim selected = pageGrid.SelectedPages.Count
        btnRemove.Enabled = selected > 0
        btnMoveFirst.Enabled = selected > 0
        btnMoveLast.Enabled = selected > 0
        btnSelectAll.Enabled = pages.Count > 0
        If trkSize.Value <> pageGrid.ThumbWidth Then trkSize.Value = Math.Clamp(pageGrid.ThumbWidth, trkSize.Minimum, trkSize.Maximum)

        lblSummary.ForeColor = _palette.Text
        lblSummary.Text = Describe(pages, _files) & If(selected > 0, $"  ·  {selected} selected", "")
    End Sub

    ''' <summary>"12 pages from 3 PDFs · 2 left out · new order", for here and the main window.</summary>
    Public Shared Function Describe(pages As IReadOnlyCollection(Of PageRef), files As IEnumerable(Of PdfItem)) As String
        Dim all = OperationRequest.AllPages(files)
        Dim fileCount = pages.Select(Function(p) p.Item).Distinct().Count()
        Dim text = If(pages.Count = 1, "1 page", $"{pages.Count} pages") &
                   If(fileCount = 1, " from 1 PDF", $" from {fileCount} PDFs")
        Dim leftOut = all.Count - pages.Count
        If leftOut > 0 Then text &= $"  ·  {leftOut} left out"
        Dim keptSet As New HashSet(Of (PdfItem, Integer))(pages.Select(Function(p) (p.Item, p.PageNumber)))
        Dim kept = all.Where(Function(a) keptSet.Contains((a.Item, a.PageNumber))).ToList()
        Dim reordered = Not kept.Select(Function(p) (p.Item, p.PageNumber)).SequenceEqual(pages.Select(Function(p) (p.Item, p.PageNumber)))
        If reordered Then text &= "  ·  new order"
        Return text
    End Function

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        MyBase.OnFormClosed(e)
        _renderer.Dispose()
    End Sub

End Class
