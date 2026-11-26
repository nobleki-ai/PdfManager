Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Threading
Imports System.Windows.Forms

''' <summary>
''' A scrolling grid of page pictures. Click to select (Ctrl+click and Shift+click for several),
''' drag to move the selected pages, press Delete or click a page's ✕ to remove it.
''' </summary>
Public Class PageGrid
    Inherits ScrollableControl

    ''' <summary>A colour per PDF, shown as a strip under each page so pages from different files are easy to tell apart.</summary>
    Private Shared ReadOnly FileColors As Color() = {
        Color.FromArgb(47, 111, 222), Color.FromArgb(217, 130, 43), Color.FromArgb(46, 158, 91), Color.FromArgb(176, 74, 205),
        Color.FromArgb(201, 64, 90), Color.FromArgb(30, 154, 168), Color.FromArgb(138, 109, 59), Color.FromArgb(89, 101, 211)
    }

    Private _pages As New List(Of PageRef)
    Private ReadOnly _selected As New HashSet(Of PageRef)
    Private ReadOnly _noPicture As New HashSet(Of PageRef)
    Private ReadOnly _fileColor As New Dictionary(Of PdfItem, Color)
    Private _palette As Palette = Palette.LightPalette
    Private _thumbWidth As Integer = 130
    Private _anchor As Integer = -1
    Private _focus As Integer = -1
    Private _hover As Integer = -1

    Private _pressIndex As Integer = -1
    Private _pressPoint As Point
    Private _pressWasSelected As Boolean
    Private _dragging As Boolean
    Private _dropIndex As Integer = -1
    Private ReadOnly _scrollTimer As New System.Windows.Forms.Timer With {.Interval = 40}
    Private _scrollStep As Integer

    Private _renderer As PageRenderer
    Private _pumping As Boolean
    Private _cancel As New CancellationTokenSource()

    ''' <summary>The order or the set of pages changed.</summary>
    Public Event PagesChanged As EventHandler
    Public Event SelectionChanged As EventHandler

    Public Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint Or
                 ControlStyles.ResizeRedraw Or ControlStyles.Selectable, True)
        AutoScroll = True
        TabStop = True
        AddHandler _scrollTimer.Tick, AddressOf ScrollTimer_Tick
    End Sub

    ' ---------------------------------------------------------------------
    ' What it shows
    ' ---------------------------------------------------------------------

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public ReadOnly Property Pages As IReadOnlyList(Of PageRef)
        Get
            Return _pages
        End Get
    End Property

    ''' <summary>Shows these pages. The PDFs' order sets which colour each file gets.</summary>
    Public Sub SetPages(pages As IEnumerable(Of PageRef), files As IEnumerable(Of PdfItem))
        _pages = pages.ToList()
        _selected.Clear()
        _fileColor.Clear()
        For Each file In files
            If Not _fileColor.ContainsKey(file) Then _fileColor(file) = FileColors(_fileColor.Count Mod FileColors.Length)
        Next
        _anchor = -1
        _focus = If(_pages.Count > 0, 0, -1)
        UpdateScrollSize()
        Invalidate()
        RaiseEvent SelectionChanged(Me, EventArgs.Empty)
    End Sub

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Renderer As PageRenderer
        Get
            Return _renderer
        End Get
        Set(value As PageRenderer)
            _renderer = value
            Invalidate()
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ThemePalette As Palette
        Get
            Return _palette
        End Get
        Set(value As Palette)
            _palette = value
            BackColor = value.Surface
            Invalidate()
        End Set
    End Property

    ''' <summary>How wide each page picture is, in pixels at 96 DPI.</summary>
    <DefaultValue(130)>
    Public Property ThumbWidth As Integer
        Get
            Return _thumbWidth
        End Get
        Set(value As Integer)
            value = Math.Clamp(value, 80, 240)
            If value = _thumbWidth Then Return
            ' Keep the page in focus in view while the pictures change size.
            _thumbWidth = value
            UpdateScrollSize()
            If _focus >= 0 Then EnsureVisible(_focus)
            Invalidate()
        End Set
    End Property

    <Browsable(False)>
    Public ReadOnly Property SelectedPages As List(Of PageRef)
        Get
            Return _pages.Where(Function(p) _selected.Contains(p)).ToList()
        End Get
    End Property

    Public Function ColorOf(item As PdfItem) As Color
        Dim found As Color
        Return If(_fileColor.TryGetValue(item, found), found, _palette.Muted)
    End Function

    ' ---------------------------------------------------------------------
    ' Changing the pages
    ' ---------------------------------------------------------------------

    Public Sub SelectAll()
        _selected.Clear()
        _selected.UnionWith(_pages)
        Invalidate()
        RaiseEvent SelectionChanged(Me, EventArgs.Empty)
    End Sub

    Public Sub SelectIndexes(ParamArray indexes As Integer())
        _selected.Clear()
        For Each index In indexes
            If index >= 0 AndAlso index < _pages.Count Then _selected.Add(_pages(index))
        Next
        If indexes.Length > 0 Then
            _focus = indexes(0)
            _anchor = indexes(0)
        End If
        Invalidate()
        RaiseEvent SelectionChanged(Me, EventArgs.Empty)
    End Sub

    ''' <summary>Takes the selected pages out.</summary>
    Public Sub RemoveSelected()
        If _selected.Count = 0 Then Return
        Dim firstRemoved = _pages.FindIndex(Function(p) _selected.Contains(p))
        _pages.RemoveAll(Function(p) _selected.Contains(p))
        _selected.Clear()
        If _pages.Count > 0 Then
            _focus = Math.Min(firstRemoved, _pages.Count - 1)
            _selected.Add(_pages(_focus))
            _anchor = _focus
        Else
            _focus = -1
            _anchor = -1
        End If
        AfterChange()
    End Sub

    ''' <summary>Moves the selected pages, keeping their order, so they start where the page at insertIndex is now.</summary>
    Public Sub MoveSelectedTo(insertIndex As Integer)
        If _selected.Count = 0 Then Return
        insertIndex = Math.Clamp(insertIndex, 0, _pages.Count)
        Dim moving = SelectedPages
        Dim before = _pages.Take(insertIndex).Count(Function(p) _selected.Contains(p))
        Dim remaining = _pages.Where(Function(p) Not _selected.Contains(p)).ToList()
        Dim target = insertIndex - before
        Dim reordered = remaining.Take(target).Concat(moving).Concat(remaining.Skip(target)).ToList()
        If reordered.SequenceEqual(_pages) Then Return
        _pages = reordered
        _focus = target
        _anchor = target
        AfterChange()
        EnsureVisible(_focus)
    End Sub

    Private Sub AfterChange()
        UpdateScrollSize()
        Invalidate()
        RaiseEvent PagesChanged(Me, EventArgs.Empty)
        RaiseEvent SelectionChanged(Me, EventArgs.Empty)
    End Sub

    ' ---------------------------------------------------------------------
    ' Layout
    ' ---------------------------------------------------------------------

    Private ReadOnly Property Gap As Integer
        Get
            Return LogicalToDeviceUnits(14)
        End Get
    End Property

    Private ReadOnly Property PictureWidth As Integer
        Get
            Return LogicalToDeviceUnits(_thumbWidth)
        End Get
    End Property

    ''' <summary>Pages are drawn in a box of A4/Letter shape; other shapes fit inside it.</summary>
    Private ReadOnly Property PictureHeight As Integer
        Get
            Return CInt(PictureWidth * 1.32)
        End Get
    End Property

    Private ReadOnly Property CaptionHeight As Integer
        Get
            Return LogicalToDeviceUnits(42)
        End Get
    End Property

    Private ReadOnly Property CellSize As Size
        Get
            Dim pad = LogicalToDeviceUnits(8)
            Return New Size(PictureWidth + pad * 2, PictureHeight + CaptionHeight + pad * 2)
        End Get
    End Property

    Private ReadOnly Property Columns As Integer
        Get
            Return Math.Max(1, (ClientSize.Width - Gap) \ (CellSize.Width + Gap))
        End Get
    End Property

    Private Sub UpdateScrollSize()
        Dim rows = (_pages.Count + Columns - 1) \ Columns
        AutoScrollMinSize = New Size(0, Gap + rows * (CellSize.Height + Gap))
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        UpdateScrollSize()
    End Sub

    ''' <summary>Where page number index sits on screen (scrolling included).</summary>
    Friend Function CellBounds(index As Integer) As Rectangle
        Dim cell = CellSize
        Dim cols = Columns
        Dim used = cols * cell.Width + (cols - 1) * Gap
        Dim left = Math.Max(Gap, (ClientSize.Width - used) \ 2)
        Dim col = index Mod cols
        Dim row = index \ cols
        Return New Rectangle(left + col * (cell.Width + Gap) + AutoScrollPosition.X,
                             Gap + row * (cell.Height + Gap) + AutoScrollPosition.Y,
                             cell.Width, cell.Height)
    End Function

    Private Function PictureBox(cell As Rectangle) As Rectangle
        Dim pad = LogicalToDeviceUnits(8)
        Return New Rectangle(cell.X + pad, cell.Y + pad, PictureWidth, PictureHeight)
    End Function

    Private Function RemoveButton(cell As Rectangle) As Rectangle
        Dim box = PictureBox(cell)
        Dim size = LogicalToDeviceUnits(26)
        Return New Rectangle(box.Right - size - LogicalToDeviceUnits(4), box.Top + LogicalToDeviceUnits(4), size, size)
    End Function

    Private Function VisibleRange() As (First As Integer, Last As Integer)
        If _pages.Count = 0 Then Return (0, -1)
        Dim rowHeight = CellSize.Height + Gap
        Dim top = -AutoScrollPosition.Y
        Dim firstRow = Math.Max(0, (top - Gap) \ rowHeight)
        Dim lastRow = (top + ClientSize.Height) \ rowHeight
        Dim first = firstRow * Columns
        Dim last = Math.Min(_pages.Count - 1, (lastRow + 1) * Columns - 1)
        Return (first, last)
    End Function

    Private Function HitTest(point As Point) As Integer
        Dim range = VisibleRange()
        For i = range.First To range.Last
            If CellBounds(i).Contains(point) Then Return i
        Next
        Return -1
    End Function

    ''' <summary>Where dropped pages would go: before the page under the pointer, or after it on its right half.</summary>
    Private Function DropIndexAt(point As Point) As Integer
        If _pages.Count = 0 Then Return 0
        Dim cols = Columns
        Dim best = _pages.Count
        Dim range = VisibleRange()
        For i = range.First To range.Last
            Dim cell = CellBounds(i)
            Dim rowTop = cell.Top - Gap \ 2
            Dim rowBottom = cell.Bottom + Gap \ 2
            If point.Y < rowTop OrElse point.Y >= rowBottom Then Continue For
            Dim isRowEnd = (i Mod cols = cols - 1) OrElse i = _pages.Count - 1
            If point.X < cell.Left + cell.Width \ 2 Then Return i
            If isRowEnd Then Return i + 1
        Next
        If point.Y < Gap Then Return range.First
        Return best
    End Function

    Public Sub EnsureVisible(index As Integer)
        If index < 0 OrElse index >= _pages.Count Then Return
        Dim cell = CellBounds(index)
        Dim y = -AutoScrollPosition.Y
        If cell.Top < 0 Then
            AutoScrollPosition = New Point(0, y + cell.Top - Gap)
        ElseIf cell.Bottom > ClientSize.Height Then
            AutoScrollPosition = New Point(0, y + cell.Bottom - ClientSize.Height + Gap)
        End If
        Invalidate()
    End Sub

    ' ---------------------------------------------------------------------
    ' Drawing
    ' ---------------------------------------------------------------------

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim g = e.Graphics
        g.Clear(_palette.Surface)
        If _pages.Count = 0 Then
            TextRenderer.DrawText(g, "No pages. Use Start Over to bring them all back.", Font, ClientRectangle, _palette.Muted,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.WordBreak)
            Return
        End If

        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        Dim range = VisibleRange()
        Dim missing = False
        For i = range.First To range.Last
            Dim cell = CellBounds(i)
            If Not cell.IntersectsWith(e.ClipRectangle) Then Continue For
            If Not DrawCell(g, i, cell) Then missing = True
        Next

        If _dragging AndAlso _dropIndex >= 0 Then DrawDropMarker(g)
        If missing Then StartDrawingPictures()
    End Sub

    ''' <summary>Draws one page; returns False when its picture isn't ready yet.</summary>
    Private Function DrawCell(g As Graphics, index As Integer, cell As Rectangle) As Boolean
        Dim page = _pages(index)
        Dim selected = _selected.Contains(page)
        Dim box = PictureBox(cell)
        Dim ready = True

        If selected Then
            Using fill As New SolidBrush(_palette.AccentSoft)
                FillRounded(g, fill, cell, LogicalToDeviceUnits(6))
            End Using
        ElseIf index = _hover AndAlso Not _dragging Then
            Using fill As New SolidBrush(_palette.ButtonHover)
                FillRounded(g, fill, cell, LogicalToDeviceUnits(6))
            End Using
        End If

        ' The page itself, fitted into its box.
        Dim picture = PageRenderer.Cached(page)
        If picture IsNot Nothing Then
            Dim scale = Math.Min(box.Width / picture.Width, box.Height / picture.Height)
            Dim w = CInt(picture.Width * scale), h = CInt(picture.Height * scale)
            Dim shown As New Rectangle(box.X + (box.Width - w) \ 2, box.Y + (box.Height - h) \ 2, w, h)
            Using shadow As New SolidBrush(Color.FromArgb(40, 0, 0, 0))
                g.FillRectangle(shadow, shown.X + 2, shown.Y + 2, shown.Width, shown.Height)
            End Using
            g.DrawImage(picture, shown)
            Using edge As New Pen(_palette.Border)
                g.DrawRectangle(edge, shown)
            End Using
        Else
            Dim blank As New Rectangle(box.X + box.Width \ 10, box.Y, box.Width - box.Width \ 5, box.Height)
            Using fill As New SolidBrush(If(_palette.IsDark, Color.FromArgb(60, 60, 64), Color.White)), edge As New Pen(_palette.Border)
                g.FillRectangle(fill, blank)
                g.DrawRectangle(edge, blank)
            End Using
            Dim waiting = Not _noPicture.Contains(page)
            ready = Not waiting
            TextRenderer.DrawText(g, If(waiting, "Loading…", $"Page {page.PageNumber}{vbLf}(no preview)"), Font, blank, _palette.Muted,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.WordBreak)
        End If

        ' A strip in the file's colour, then the new page number and where the page comes from.
        Dim strip As New Rectangle(box.X, box.Bottom + LogicalToDeviceUnits(4), box.Width, LogicalToDeviceUnits(4))
        Using fill As New SolidBrush(ColorOf(page.Item))
            g.FillRectangle(fill, strip)
        End Using
        Dim line1 As New Rectangle(box.X, strip.Bottom + LogicalToDeviceUnits(2), box.Width, LogicalToDeviceUnits(18))
        Dim line2 As New Rectangle(box.X, line1.Bottom, box.Width, LogicalToDeviceUnits(18))
        Using bold As New Font(Font, FontStyle.Bold)
            TextRenderer.DrawText(g, (index + 1).ToString(), bold, line1, _palette.Text, TextFormatFlags.HorizontalCenter Or TextFormatFlags.EndEllipsis)
        End Using
        TextRenderer.DrawText(g, $"{page.Item.BaseName} · p{page.PageNumber}", Font, line2, _palette.Muted,
                              TextFormatFlags.HorizontalCenter Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPrefix)

        If selected Then
            Using edge As New Pen(_palette.Accent, LogicalToDeviceUnits(2))
                DrawRounded(g, edge, Rectangle.Inflate(cell, -1, -1), LogicalToDeviceUnits(6))
            End Using
        End If

        ' The remove button, on the page under the pointer.
        If index = _hover AndAlso Not _dragging Then
            Dim button = RemoveButton(cell)
            g.SmoothingMode = SmoothingMode.AntiAlias
            Using fill As New SolidBrush(Color.FromArgb(220, 196, 43, 28))
                g.FillEllipse(fill, button)
            End Using
            g.SmoothingMode = SmoothingMode.None
            Using cross As New Font("Segoe UI", 9.0F, FontStyle.Bold)
                TextRenderer.DrawText(g, "✕", cross, button, Color.White, TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or TextFormatFlags.NoPadding)
            End Using
        End If
        Return ready
    End Function

    Private Sub DrawDropMarker(g As Graphics)
        Dim width = LogicalToDeviceUnits(4)
        Dim cell As Rectangle
        Dim x As Integer
        If _dropIndex < _pages.Count Then
            cell = CellBounds(_dropIndex)
            x = cell.Left - Gap \ 2
        Else
            cell = CellBounds(_pages.Count - 1)
            x = cell.Right + Gap \ 2
        End If
        Using fill As New SolidBrush(_palette.Accent)
            g.FillRectangle(fill, x - width \ 2, cell.Top, width, cell.Height)
        End Using
    End Sub

    Private Shared Function Rounded(area As Rectangle, radius As Integer) As GraphicsPath
        Dim path As New GraphicsPath()
        Dim d = radius * 2
        path.AddArc(area.X, area.Y, d, d, 180, 90)
        path.AddArc(area.Right - d, area.Y, d, d, 270, 90)
        path.AddArc(area.Right - d, area.Bottom - d, d, d, 0, 90)
        path.AddArc(area.X, area.Bottom - d, d, d, 90, 90)
        path.CloseFigure()
        Return path
    End Function

    Private Shared Sub FillRounded(g As Graphics, brush As Brush, area As Rectangle, radius As Integer)
        Using path = Rounded(area, radius)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.FillPath(brush, path)
            g.SmoothingMode = SmoothingMode.None
        End Using
    End Sub

    Private Shared Sub DrawRounded(g As Graphics, pen As Pen, area As Rectangle, radius As Integer)
        Using path = Rounded(area, radius)
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.DrawPath(pen, path)
            g.SmoothingMode = SmoothingMode.None
        End Using
    End Sub

    ' ---------------------------------------------------------------------
    ' Drawing the pictures, visible pages first
    ' ---------------------------------------------------------------------

    Private Async Sub StartDrawingPictures()
        If _pumping OrElse _renderer Is Nothing Then Return
        _pumping = True
        Try
            While Not IsDisposed
                Dim range = VisibleRange()
                Dim nextPage As PageRef = Nothing
                For i = range.First To range.Last
                    Dim page = _pages(i)
                    If PageRenderer.Cached(page) Is Nothing AndAlso Not _noPicture.Contains(page) Then
                        nextPage = page
                        Exit For
                    End If
                Next
                If nextPage Is Nothing Then Exit While
                Dim picture = Await _renderer.RenderAsync(nextPage, _cancel.Token)
                If IsDisposed Then Exit While
                If picture Is Nothing Then _noPicture.Add(nextPage)
                Invalidate()
            End While
        Catch ex As OperationCanceledException
            ' The page view closed.
        Finally
            _pumping = False
        End Try
    End Sub

    ''' <summary>True once every page on screen has its picture (or is known to have none).</summary>
    Friend ReadOnly Property VisiblePicturesReady As Boolean
        Get
            Dim range = VisibleRange()
            For i = range.First To range.Last
                If PageRenderer.Cached(_pages(i)) Is Nothing AndAlso Not _noPicture.Contains(_pages(i)) Then Return False
            Next
            Return True
        End Get
    End Property

    ' ---------------------------------------------------------------------
    ' Mouse and keyboard
    ' ---------------------------------------------------------------------

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        Focus()
        If e.Button <> MouseButtons.Left Then Return
        Dim index = HitTest(e.Location)
        _pressIndex = index
        _pressPoint = e.Location
        If index < 0 Then
            If ModifierKeys = Keys.None Then SelectIndexes()
            Return
        End If

        If RemoveButton(CellBounds(index)).Contains(e.Location) Then
            _pressIndex = -1
            SelectIndexes(index)
            RemoveSelected()
            Return
        End If

        Dim page = _pages(index)
        _pressWasSelected = _selected.Contains(page)
        If (ModifierKeys And Keys.Control) = Keys.Control Then
            If Not _selected.Remove(page) Then _selected.Add(page)
            _anchor = index
            _pressIndex = If(_selected.Contains(page), index, -1)
        ElseIf (ModifierKeys And Keys.Shift) = Keys.Shift AndAlso _anchor >= 0 Then
            _selected.Clear()
            For i = Math.Min(_anchor, index) To Math.Max(_anchor, index)
                _selected.Add(_pages(i))
            Next
        ElseIf Not _pressWasSelected Then
            _selected.Clear()
            _selected.Add(page)
            _anchor = index
        End If
        _focus = index
        Invalidate()
        RaiseEvent SelectionChanged(Me, EventArgs.Empty)
    End Sub

    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If e.Button = MouseButtons.Left AndAlso _pressIndex >= 0 Then
            Dim drag = SystemInformation.DragSize
            If Not _dragging AndAlso (Math.Abs(e.X - _pressPoint.X) > drag.Width OrElse Math.Abs(e.Y - _pressPoint.Y) > drag.Height) Then
                _dragging = True
                Cursor = Cursors.SizeAll
            End If
            If _dragging Then
                Dim drop = DropIndexAt(e.Location)
                If drop <> _dropIndex Then
                    _dropIndex = drop
                    Invalidate()
                End If
                ' Near the top or bottom edge, scroll so pages can be dragged anywhere.
                Dim edge = LogicalToDeviceUnits(40)
                _scrollStep = If(e.Y < edge, -LogicalToDeviceUnits(18), If(e.Y > ClientSize.Height - edge, LogicalToDeviceUnits(18), 0))
                _scrollTimer.Enabled = _scrollStep <> 0
            End If
            Return
        End If

        Dim hover = HitTest(e.Location)
        If hover <> _hover Then
            _hover = hover
            Invalidate()
        End If
        Cursor = If(hover >= 0 AndAlso RemoveButton(CellBounds(hover)).Contains(e.Location), Cursors.Hand, Cursors.Default)
    End Sub

    Private Sub ScrollTimer_Tick(sender As Object, e As EventArgs)
        If Not _dragging Then
            _scrollTimer.Stop()
            Return
        End If
        AutoScrollPosition = New Point(0, -AutoScrollPosition.Y + _scrollStep)
        _dropIndex = DropIndexAt(PointToClient(MousePosition))
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        _scrollTimer.Stop()
        If _dragging Then
            _dragging = False
            Cursor = Cursors.Default
            Dim drop = _dropIndex
            _dropIndex = -1
            MoveSelectedTo(drop)
            Invalidate()
        ElseIf _pressIndex >= 0 AndAlso _pressWasSelected AndAlso ModifierKeys = Keys.None Then
            ' A plain click on a page that was already among several selected ones selects only it.
            SelectIndexes(_pressIndex)
        End If
        _pressIndex = -1
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        If _hover >= 0 Then
            _hover = -1
            Invalidate()
        End If
    End Sub

    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
        If (ModifierKeys And Keys.Control) = Keys.Control Then
            ' Ctrl + wheel makes the pictures bigger or smaller.
            ThumbWidth += If(e.Delta > 0, 20, -20)
            RaiseEvent SelectionChanged(Me, EventArgs.Empty)
            Return
        End If
        MyBase.OnMouseWheel(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnScroll(se As ScrollEventArgs)
        MyBase.OnScroll(se)
        Invalidate()
    End Sub

    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        Select Case keyData And Keys.KeyCode
            Case Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End
                Return True
        End Select
        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        If _pages.Count = 0 Then Return
        Select Case e.KeyCode
            Case Keys.Delete, Keys.Back
                RemoveSelected()
                e.Handled = True
            Case Keys.A
                If e.Control Then
                    SelectAll()
                    e.Handled = True
                End If
            Case Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Home, Keys.End
                Dim cols = Columns
                Dim current = Math.Max(0, _focus)
                Dim target = current
                Select Case e.KeyCode
                    Case Keys.Left : target -= 1
                    Case Keys.Right : target += 1
                    Case Keys.Up : target -= cols
                    Case Keys.Down : target += cols
                    Case Keys.Home : target = 0
                    Case Keys.End : target = _pages.Count - 1
                End Select
                target = Math.Clamp(target, 0, _pages.Count - 1)
                If e.Alt AndAlso _selected.Count > 0 Then
                    ' Alt + arrow moves the selected pages one place.
                    Dim first = _pages.FindIndex(Function(p) _selected.Contains(p))
                    Dim last = _pages.FindLastIndex(Function(p) _selected.Contains(p))
                    If e.KeyCode = Keys.Left Then MoveSelectedTo(Math.Max(0, first - 1))
                    If e.KeyCode = Keys.Right Then MoveSelectedTo(Math.Min(_pages.Count, last + 2))
                ElseIf e.Shift AndAlso _anchor >= 0 Then
                    _selected.Clear()
                    For i = Math.Min(_anchor, target) To Math.Max(_anchor, target)
                        _selected.Add(_pages(i))
                    Next
                    _focus = target
                    RaiseEvent SelectionChanged(Me, EventArgs.Empty)
                Else
                    SelectIndexes(target)
                End If
                EnsureVisible(_focus)
                e.Handled = True
        End Select
    End Sub

    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing Then
            _cancel.Cancel()
            _scrollTimer.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

End Class
