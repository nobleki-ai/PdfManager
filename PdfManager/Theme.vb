Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports Microsoft.Win32

Public Enum ThemeChoice
    Light
    Dark
    Auto
End Enum

''' <summary>The colours for one theme.</summary>
Public Class Palette
    Public Property IsDark As Boolean
    Public Property Back As Color
    Public Property Surface As Color
    Public Property Border As Color
    Public Property Text As Color
    Public Property Muted As Color
    Public Property Accent As Color
    Public Property AccentHover As Color
    Public Property AccentText As Color
    Public Property AccentSoft As Color
    Public Property ButtonBack As Color
    Public Property ButtonHover As Color
    Public Property InputBack As Color
    Public Property GridHeader As Color
    Public Property GridLine As Color
    Public Property Selection As Color
    Public Property Editable As Color
    Public Property ErrorText As Color
    Public Property SuccessText As Color
    Public Property WarningBack As Color
    Public Property WarningText As Color
    Public Property Link As Color

    Public Shared ReadOnly Property LightPalette As New Palette With {
        .IsDark = False,
        .Back = ColorTranslator.FromHtml("#F4F6F9"),
        .Surface = Color.White,
        .Border = ColorTranslator.FromHtml("#CBD2DC"),
        .Text = ColorTranslator.FromHtml("#1F2328"),
        .Muted = ColorTranslator.FromHtml("#56606E"),
        .Accent = ColorTranslator.FromHtml("#0B5CAD"),
        .AccentHover = ColorTranslator.FromHtml("#094C90"),
        .AccentText = Color.White,
        .AccentSoft = ColorTranslator.FromHtml("#DCEBFA"),
        .ButtonBack = Color.White,
        .ButtonHover = ColorTranslator.FromHtml("#EAF1FA"),
        .InputBack = Color.White,
        .GridHeader = ColorTranslator.FromHtml("#EEF1F5"),
        .GridLine = ColorTranslator.FromHtml("#E3E7ED"),
        .Selection = ColorTranslator.FromHtml("#D6E8FA"),
        .Editable = ColorTranslator.FromHtml("#FFF8DC"),
        .ErrorText = ColorTranslator.FromHtml("#B3261E"),
        .SuccessText = ColorTranslator.FromHtml("#16703A"),
        .WarningBack = ColorTranslator.FromHtml("#FFF4CE"),
        .WarningText = ColorTranslator.FromHtml("#3B2F00"),
        .Link = ColorTranslator.FromHtml("#0B5CAD")
    }

    Public Shared ReadOnly Property DarkPalette As New Palette With {
        .IsDark = True,
        .Back = ColorTranslator.FromHtml("#1C1D21"),
        .Surface = ColorTranslator.FromHtml("#26282D"),
        .Border = ColorTranslator.FromHtml("#474B53"),
        .Text = ColorTranslator.FromHtml("#E8E9EC"),
        .Muted = ColorTranslator.FromHtml("#A9AFB8"),
        .Accent = ColorTranslator.FromHtml("#1F6FC5"),
        .AccentHover = ColorTranslator.FromHtml("#2A7FD8"),
        .AccentText = Color.White,
        .AccentSoft = ColorTranslator.FromHtml("#21395C"),
        .ButtonBack = ColorTranslator.FromHtml("#30333A"),
        .ButtonHover = ColorTranslator.FromHtml("#3B3F47"),
        .InputBack = ColorTranslator.FromHtml("#202226"),
        .GridHeader = ColorTranslator.FromHtml("#2E3137"),
        .GridLine = ColorTranslator.FromHtml("#383B42"),
        .Selection = ColorTranslator.FromHtml("#2B4A70"),
        .Editable = ColorTranslator.FromHtml("#36321F"),
        .ErrorText = ColorTranslator.FromHtml("#FF8A80"),
        .SuccessText = ColorTranslator.FromHtml("#6FD08C"),
        .WarningBack = ColorTranslator.FromHtml("#4A3B10"),
        .WarningText = ColorTranslator.FromHtml("#F6E7B8"),
        .Link = ColorTranslator.FromHtml("#7DBBFF")
    }
End Class

''' <summary>Colours the window's controls for light or dark mode and draws the button icons.</summary>
Public Module Theme

    ''' <summary>Whether Windows is set to dark mode for apps.</summary>
    Public Function WindowsPrefersDark() As Boolean
        Try
            Using key = Registry.CurrentUser.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Themes\Personalize")
                Dim value = key?.GetValue("AppsUseLightTheme")
                Return TypeOf value Is Integer AndAlso CInt(value) = 0
            End Using
        Catch ex As Exception When TypeOf ex Is Security.SecurityException OrElse TypeOf ex Is UnauthorizedAccessException
            Return False
        End Try
    End Function

    Public Function ParseChoice(text As String) As ThemeChoice
        Dim choice As ThemeChoice
        If [Enum].TryParse(If(text, ""), True, choice) Then Return choice
        Return ThemeChoice.Auto
    End Function

    Public Function Resolve(choice As ThemeChoice) As Palette
        Dim dark = choice = ThemeChoice.Dark OrElse (choice = ThemeChoice.Auto AndAlso WindowsPrefersDark())
        Return If(dark, Palette.DarkPalette, Palette.LightPalette)
    End Function

    ''' <summary>Colours a control and everything inside it.</summary>
    Public Sub Apply(control As Control, p As Palette)
        Apply(control, p, p.Back)
    End Sub

    Private Sub Apply(control As Control, p As Palette, background As Color)
        Select Case True
            Case TypeOf control Is DataGridView
                StyleGrid(DirectCast(control, DataGridView), p)
            Case TypeOf control Is TextBox
                Dim box = DirectCast(control, TextBox)
                box.BackColor = If(box.ReadOnly, p.Surface, p.InputBack)
                box.ForeColor = p.Text
                box.BorderStyle = BorderStyle.FixedSingle
                UseDarkScrollbars(box, p.IsDark)
            Case TypeOf control Is NumericUpDown
                control.BackColor = p.InputBack
                control.ForeColor = p.Text
                DirectCast(control, NumericUpDown).BorderStyle = BorderStyle.FixedSingle
            Case TypeOf control Is ButtonBase AndAlso IsToggle(control)
                StyleToggle(DirectCast(control, ButtonBase), p)
            Case TypeOf control Is Button
                StyleButton(DirectCast(control, Button), p)
            Case TypeOf control Is RadioButton
                ' Windows' own round buttons show the chosen dot clearly in both themes.
                Dim radio = DirectCast(control, RadioButton)
                radio.FlatStyle = FlatStyle.Standard
                radio.BackColor = background
                radio.ForeColor = p.Text
            Case TypeOf control Is CheckBox
                Dim box = DirectCast(control, ButtonBase)
                box.FlatStyle = FlatStyle.Flat
                box.FlatAppearance.BorderColor = p.Border
                box.FlatAppearance.CheckedBackColor = p.InputBack
                box.BackColor = background
                box.ForeColor = p.Text
            Case TypeOf control Is LinkLabel
                Dim link = DirectCast(control, LinkLabel)
                link.LinkColor = p.Link
                link.ActiveLinkColor = p.Link
                link.VisitedLinkColor = p.Link
                link.BackColor = background
            Case TypeOf control Is StatusStrip
                Dim strip = DirectCast(control, StatusStrip)
                strip.RenderMode = ToolStripRenderMode.ManagerRenderMode
                strip.Renderer = New ToolStripProfessionalRenderer(New FlatColors(p))
                strip.BackColor = p.Surface
                strip.ForeColor = p.Text
                For Each item As ToolStripItem In strip.Items
                    item.ForeColor = p.Text
                    Dim label = TryCast(item, ToolStripStatusLabel)
                    If label IsNot Nothing AndAlso label.IsLink Then
                        label.LinkColor = p.Link
                        label.ActiveLinkColor = p.Link
                        label.VisitedLinkColor = p.Link
                    End If
                Next
            Case TypeOf control Is Label
                control.BackColor = background
                control.ForeColor = If(IsMuted(control), p.Muted, p.Text)
            Case TypeOf control Is TabPage, TypeOf control Is Panel AndAlso IsCard(control)
                ' Option cards sit on a slightly raised surface.
                control.BackColor = p.Surface
                background = p.Surface
                If TypeOf control Is TabPage Then DirectCast(control, TabPage).UseVisualStyleBackColor = False
            Case Else
                control.BackColor = background
                control.ForeColor = p.Text
        End Select

        For Each child As Control In control.Controls
            Apply(child, p, background)
        Next
    End Sub

    ''' <summary>Marks a control by its Tag: "muted", "card" or "toggle".</summary>
    Private Function HasTag(control As Control, word As String) As Boolean
        Return TypeOf control.Tag Is String AndAlso CStr(control.Tag).Split(" "c).Contains(word)
    End Function

    Private Function IsMuted(control As Control) As Boolean
        Return HasTag(control, "muted")
    End Function

    Private Function IsCard(control As Control) As Boolean
        Return HasTag(control, "card")
    End Function

    Private Function IsToggle(control As Control) As Boolean
        Return HasTag(control, "toggle")
    End Function

    Private Sub StyleButton(button As Button, p As Palette)
        button.FlatStyle = FlatStyle.Flat
        If HasTag(button, "primary") Then
            button.BackColor = p.Accent
            button.ForeColor = p.AccentText
            button.FlatAppearance.BorderSize = 0
            button.FlatAppearance.MouseOverBackColor = p.AccentHover
            button.FlatAppearance.MouseDownBackColor = p.AccentHover
        Else
            button.BackColor = p.ButtonBack
            button.ForeColor = p.Text
            button.FlatAppearance.BorderSize = 1
            button.FlatAppearance.BorderColor = p.Border
            button.FlatAppearance.MouseOverBackColor = p.ButtonHover
            button.FlatAppearance.MouseDownBackColor = p.AccentSoft
        End If
    End Sub

    ''' <summary>A radio button or check box drawn as a button that stays pressed when chosen.</summary>
    Private Sub StyleToggle(toggle As ButtonBase, p As Palette)
        toggle.FlatStyle = FlatStyle.Flat
        toggle.BackColor = p.ButtonBack
        toggle.ForeColor = p.Text
        toggle.FlatAppearance.BorderSize = 1
        toggle.FlatAppearance.MouseOverBackColor = p.ButtonHover
        toggle.FlatAppearance.CheckedBackColor = p.AccentSoft
        Dim chosen = (TypeOf toggle Is RadioButton AndAlso DirectCast(toggle, RadioButton).Checked) OrElse
                     (TypeOf toggle Is CheckBox AndAlso DirectCast(toggle, CheckBox).Checked)
        toggle.FlatAppearance.BorderColor = If(chosen, p.Accent, p.Border)
    End Sub

    Private Sub StyleGrid(grid As DataGridView, p As Palette)
        grid.BackgroundColor = p.Surface
        grid.GridColor = p.GridLine
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersDefaultCellStyle.BackColor = p.GridHeader
        grid.ColumnHeadersDefaultCellStyle.ForeColor = p.Text
        grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = p.GridHeader
        grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = p.Text
        grid.DefaultCellStyle.BackColor = p.Surface
        grid.DefaultCellStyle.ForeColor = p.Text
        grid.DefaultCellStyle.SelectionBackColor = p.Selection
        grid.DefaultCellStyle.SelectionForeColor = p.Text
        grid.RowsDefaultCellStyle.BackColor = p.Surface
        grid.AlternatingRowsDefaultCellStyle.BackColor = p.Surface
        For Each child As Control In grid.Controls
            If TypeOf child Is ScrollBar Then UseDarkScrollbars(child, p.IsDark)
        Next
    End Sub

    ' ---------------------------------------------------------------------
    ' Windows' own parts: the title bar and scroll bars
    ' ---------------------------------------------------------------------

    <DllImport("dwmapi.dll")>
    Private Function DwmSetWindowAttribute(hwnd As IntPtr, attribute As Integer, ByRef value As Integer, size As Integer) As Integer
    End Function

    <DllImport("uxtheme.dll", CharSet:=CharSet.Unicode)>
    Private Function SetWindowTheme(hwnd As IntPtr, appName As String, idList As String) As Integer
    End Function

    Public Sub UseDarkTitleBar(form As Form, dark As Boolean)
        If Not form.IsHandleCreated Then Return
        Dim value = If(dark, 1, 0)
        ' 20 is DWMWA_USE_IMMERSIVE_DARK_MODE on Windows 10 20H1 and later; 19 on earlier builds.
        If DwmSetWindowAttribute(form.Handle, 20, value, 4) <> 0 Then
            DwmSetWindowAttribute(form.Handle, 19, value, 4)
        End If
    End Sub

    Public Sub UseDarkScrollbars(control As Control, dark As Boolean)
        If control.IsHandleCreated Then
            SetWindowTheme(control.Handle, If(dark, "DarkMode_Explorer", "Explorer"), Nothing)
        End If
    End Sub

    ' ---------------------------------------------------------------------
    ' Icons drawn from Windows' icon font
    ' ---------------------------------------------------------------------

    Private ReadOnly IconFontName As String = FindIconFont()

    Private Function FindIconFont() As String
        Using fonts As New InstalledFontCollection()
            For Each name In {"Segoe Fluent Icons", "Segoe MDL2 Assets"}
                If fonts.Families.Any(Function(f) f.Name = name) Then Return name
            Next
        End Using
        Return Nothing
    End Function

    ''' <summary>A small picture of one icon-font character, or Nothing if the font isn't installed.</summary>
    Public Function Glyph(character As Char, color As Color, pixels As Integer) As Bitmap
        If IconFontName Is Nothing Then Return Nothing
        Dim image As New Bitmap(pixels, pixels)
        Using g = Graphics.FromImage(image),
              font As New Font(IconFontName, pixels * 0.78F, GraphicsUnit.Pixel),
              brush As New SolidBrush(color),
              format As New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center}
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
            g.SmoothingMode = SmoothingMode.AntiAlias
            g.DrawString(character.ToString(), font, brush, New RectangleF(0, 0, pixels, pixels), format)
        End Using
        Return image
    End Function

    ''' <summary>Puts an icon before a button's text, in the button's own text colour.</summary>
    Public Sub SetIcon(button As ButtonBase, character As Char, pixels As Integer)
        Dim old = button.Image
        button.Image = Glyph(character, button.ForeColor, pixels)
        old?.Dispose()
        If button.Image IsNot Nothing Then
            button.TextImageRelation = TextImageRelation.ImageBeforeText
            button.ImageAlign = ContentAlignment.MiddleCenter
        End If
    End Sub

    ''' <summary>Flat colours for the status bar, instead of the default gradients.</summary>
    Private Class FlatColors
        Inherits ProfessionalColorTable

        Private ReadOnly _p As Palette

        Public Sub New(p As Palette)
            _p = p
            UseSystemColors = False
        End Sub

        Public Overrides ReadOnly Property StatusStripGradientBegin As Color
            Get
                Return _p.Surface
            End Get
        End Property

        Public Overrides ReadOnly Property StatusStripGradientEnd As Color
            Get
                Return _p.Surface
            End Get
        End Property

        Public Overrides ReadOnly Property StatusStripBorder As Color
            Get
                Return _p.Border
            End Get
        End Property

        Public Overrides ReadOnly Property ToolStripBorder As Color
            Get
                Return _p.Border
            End Get
        End Property

        Public Overrides ReadOnly Property GripDark As Color
            Get
                Return _p.Border
            End Get
        End Property

        Public Overrides ReadOnly Property GripLight As Color
            Get
                Return _p.Surface
            End Get
        End Property
    End Class

End Module
