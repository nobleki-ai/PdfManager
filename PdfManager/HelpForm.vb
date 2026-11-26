Imports System.Drawing
Imports System.Windows.Forms

''' <summary>
''' The Help window: a list of topics on the left and the chosen topic on the right.
''' The Page ranges topic has a "Try it" box that shows exactly which pages a list picks.
''' </summary>
Public Class HelpForm

    Public Const TopicGettingStarted As String = "Getting started"
    Public Const TopicWhatsNew As String = "What's new"
    Public Const TopicPageRanges As String = "Page ranges"
    Public Const TopicCombine As String = "Combine"
    Public Const TopicMerge As String = "Merge Pages"
    Public Const TopicExtract As String = "Extract Pages"
    Public Const TopicEncrypt As String = "Encrypt / Decrypt"
    Public Const TopicRotate As String = "Rotate"
    Public Const TopicStamp As String = "Stamp & Background"
    Public Const TopicCompress As String = "Reduce Size"
    Public Const TopicArrange As String = "Arrange Pages"
    Public Const TopicPasswords As String = "Locked PDFs and passwords"
    Public Const TopicTips As String = "Tips and shortcuts"
    Public Const TopicAbout As String = "About"

    ''' <summary>The page lists shown on the Page ranges topic, with what each one does.</summary>
    Public Shared ReadOnly PageRangeExamples As (Text As String, Meaning As String)() = {
        ("All", "Every page. Leaving the box empty means the same, except in Extract Pages, where you type the pages you want."),
        ("5", "Just page 5."),
        ("1-10", "Pages 1 to 10."),
        ("3-end", "From page 3 to the last page. You can write last instead of end."),
        ("end", "Just the last page."),
        ("1-3, 5, 8-end", "Several pieces, separated by commas."),
        ("6, 1-2", "Pieces are used in the order you type them: page 6 first, then pages 1 and 2."),
        ("10-1", "Backwards: pages 10 down to 1."),
        ("end-1", "The whole PDF in reverse order."),
        ("odd", "Pages 1, 3, 5 and so on."),
        ("even", "Pages 2, 4, 6 and so on."),
        ("2, 2", "The same page twice, for example to print an extra copy.")
    }

    Private Const ExamplePages As Integer = 10

    Private ReadOnly _topics As New Dictionary(Of String, Action)
    Private _palette As Palette = Palette.LightPalette
    Private ReadOnly _qpdfStatus As Func(Of String)

    ''' <summary>For Visual Studio's designer.</summary>
    Public Sub New()
        Me.New(Function() "")
    End Sub

    ''' <param name="qpdfStatus">Tells the About topic which QPDF the app is using.</param>
    Public Sub New(qpdfStatus As Func(Of String))
        InitializeComponent()
        _qpdfStatus = qpdfStatus

        _topics.Add(TopicGettingStarted, AddressOf WriteGettingStarted)
        _topics.Add(TopicWhatsNew, AddressOf WriteWhatsNew)
        _topics.Add(TopicPageRanges, AddressOf WritePageRanges)
        _topics.Add(TopicCombine, AddressOf WriteCombine)
        _topics.Add(TopicMerge, AddressOf WriteMerge)
        _topics.Add(TopicExtract, AddressOf WriteExtract)
        _topics.Add(TopicEncrypt, AddressOf WriteEncrypt)
        _topics.Add(TopicRotate, AddressOf WriteRotate)
        _topics.Add(TopicStamp, AddressOf WriteStamp)
        _topics.Add(TopicCompress, AddressOf WriteCompress)
        _topics.Add(TopicArrange, AddressOf WriteArrange)
        _topics.Add(TopicPasswords, AddressOf WritePasswords)
        _topics.Add(TopicTips, AddressOf WriteTips)
        _topics.Add(TopicAbout, AddressOf WriteAbout)
        For Each title In _topics.Keys
            lstTopics.Items.Add(title)
        Next
        lstTopics.SelectedIndex = 0
    End Sub

    Friend ReadOnly Property TopicTitles As IReadOnlyCollection(Of String)
        Get
            Return _topics.Keys
        End Get
    End Property

    Friend ReadOnly Property CurrentTopic As String
        Get
            Return CStr(lstTopics.SelectedItem)
        End Get
    End Property

    ''' <summary>The text of the topic on show, for the tests.</summary>
    Friend ReadOnly Property TopicText As String
        Get
            Return rtbHelp.Text
        End Get
    End Property

    Friend Sub ShowTopic(title As String)
        Dim index = lstTopics.Items.IndexOf(title)
        If index >= 0 Then lstTopics.SelectedIndex = index
    End Sub

    Friend Sub ApplyPalette(p As Palette)
        _palette = p
        Theme.Apply(Me, p)
        lstTopics.BackColor = p.Surface
        lstTopics.ForeColor = p.Text
        rtbHelp.BackColor = p.Surface
        rootLayout.BackColor = p.Back
        Theme.UseDarkTitleBar(Me, p.IsDark)
        Theme.UseDarkScrollbars(rtbHelp, p.IsDark)
        Theme.UseDarkScrollbars(lstTopics, p.IsDark)
        RenderTopic()
        UpdateTryResult()
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        Theme.UseDarkTitleBar(Me, _palette.IsDark)
    End Sub

    ' Scroll bars can only be themed once Windows has created the text and list boxes.
    Private Sub HelpForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Theme.UseDarkScrollbars(rtbHelp, _palette.IsDark)
        Theme.UseDarkScrollbars(lstTopics, _palette.IsDark)
    End Sub

    ''' <summary>Opens without taking focus when the main window was started that way.</summary>
    Friend QuietMode As Boolean

    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return QuietMode
        End Get
    End Property

    Private Sub HelpForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Close()
            e.Handled = True
        End If
    End Sub

    ' Pictures are fitted to the width of the text, so a resized window draws the topic again.
    Private Sub HelpForm_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        RenderTopic()
    End Sub

    Private Sub lstTopics_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstTopics.SelectedIndexChanged
        RenderTopic()
    End Sub

    Private Sub RenderTopic()
        Dim title = CurrentTopic
        If title Is Nothing Then Return
        lblTopicTitle.Text = title
        pnlTry.Visible = title = TopicPageRanges

        rtbHelp.SuspendLayout()
        ' A read-only box refuses pictures, so it's only read-only while it's being read.
        rtbHelp.ReadOnly = False
        rtbHelp.Clear()
        rtbHelp.BackColor = _palette.Surface
        PictureCount = 0
        _topics(title).Invoke()
        rtbHelp.ReadOnly = True
        rtbHelp.BackColor = _palette.Surface
        rtbHelp.SelectionStart = 0
        rtbHelp.ScrollToCaret()
        rtbHelp.ResumeLayout()
    End Sub

    ' ---------------------------------------------------------------------
    ' The "Try it" box
    ' ---------------------------------------------------------------------

    Private Sub TryChanged(sender As Object, e As EventArgs) Handles txtTry.TextChanged, numTryPages.ValueChanged
        UpdateTryResult()
    End Sub

    Private Sub UpdateTryResult()
        Dim count = CInt(numTryPages.Value)
        Dim pages = PageRange.Expand(txtTry.Text, count)
        If pages Is Nothing Then
            lblTryResult.ForeColor = _palette.ErrorText
            lblTryResult.Text = PageRange.Parse(txtTry.Text, count).ErrorMessage
        Else
            lblTryResult.ForeColor = _palette.SuccessText
            lblTryResult.Text = $"Picks {Plural(pages.Count, "page")}: {ListPages(pages, 40)}"
        End If
    End Sub

    Friend ReadOnly Property TryResultText As String
        Get
            Return lblTryResult.Text
        End Get
    End Property

    Friend Sub TryRange(text As String, pageCount As Integer)
        numTryPages.Value = pageCount
        txtTry.Text = text
    End Sub

    Private Shared Function ListPages(pages As List(Of Integer), limit As Integer) As String
        Dim shown = String.Join(", ", pages.Take(limit))
        Return If(pages.Count > limit, shown & $" … ({pages.Count - limit} more)", shown)
    End Function

    Private Shared Function Plural(count As Integer, word As String) As String
        Return If(count = 1, $"1 {word}", $"{count} {word}s")
    End Function

    ' ---------------------------------------------------------------------
    ' Writing formatted text
    ' ---------------------------------------------------------------------

    Private Sub Add(text As String, size As Single, style As FontStyle, color As Color,
                    Optional indent As Integer = 0, Optional family As String = "Segoe UI")
        rtbHelp.SelectionStart = rtbHelp.TextLength
        rtbHelp.SelectionLength = 0
        rtbHelp.SelectionFont = New Font(family, size, style)
        rtbHelp.SelectionColor = color
        rtbHelp.SelectionIndent = LogicalToDeviceUnits(indent)
        rtbHelp.SelectionBullet = False
        rtbHelp.SelectedText = text
    End Sub

    Private Sub Heading(text As String)
        Add(text & vbLf, 11.5F, FontStyle.Bold, _palette.Text)
    End Sub

    Private Sub Para(text As String)
        Add(text & vbLf & vbLf, 10.5F, FontStyle.Regular, _palette.Text)
    End Sub

    Private Sub Bullet(text As String)
        Add("•  ", 10.5F, FontStyle.Regular, _palette.Accent, 8)
        Add(text & vbLf, 10.5F, FontStyle.Regular, _palette.Text, 8)
    End Sub

    Private Sub Gap()
        Add(vbLf, 6.0F, FontStyle.Regular, _palette.Text)
    End Sub

    ''' <summary>
    ''' A picture of the app, built into the program (Assets\help\help-name.png), shrunk to fit the
    ''' Help text, with an optional caption under it. Nothing happens if the picture is missing.
    ''' </summary>
    Private Sub Picture(name As String, Optional caption As String = Nothing)
        Dim png = HelpPicture(name)
        If png Is Nothing OrElse png.Length < 24 Then Return
        ' A PNG states its size in pixels at bytes 16-23.
        Dim width = (CInt(png(16)) << 24) Or (CInt(png(17)) << 16) Or (CInt(png(18)) << 8) Or png(19)
        Dim height = (CInt(png(20)) << 24) Or (CInt(png(21)) << 16) Or (CInt(png(22)) << 8) Or png(23)
        If width <= 0 OrElse height <= 0 Then Return

        ' Shown at most 640 wide (at 100% scale), and never wider than the text area.
        Dim room = CInt(rtbHelp.ClientSize.Width * 96.0 / DeviceDpi) - 40
        Dim shownWidth = Math.Max(240, Math.Min(Math.Min(640, width), room))
        Dim shownHeight = CInt(height * shownWidth / width)
        Dim hex As New System.Text.StringBuilder(png.Length * 2)
        For Each b In png
            hex.Append(b.ToString("x2"))
        Next
        ' Sizes in twips: 15 per pixel at 100% scale; Windows enlarges them on bigger scales.
        Dim rtf = $"{{\rtf1{{\pict\pngblip\picw{width}\pich{height}\picwgoal{shownWidth * 15}\pichgoal{shownHeight * 15} {hex}}}\par}}"
        rtbHelp.SelectionStart = rtbHelp.TextLength
        rtbHelp.SelectionLength = 0
        rtbHelp.SelectionIndent = 0
        rtbHelp.SelectedRtf = rtf
        If caption IsNot Nothing Then Add(caption & vbLf, 9.0F, FontStyle.Italic, _palette.Muted)
        Gap()
        PictureCount += 1
    End Sub

    ''' <summary>How many pictures the topic on show has, for the tests.</summary>
    Friend PictureCount As Integer

    Private Shared ReadOnly PictureCache As New Dictionary(Of String, Byte())

    Friend Shared Function HelpPicture(name As String) As Byte()
        Dim data As Byte() = Nothing
        If PictureCache.TryGetValue(name, data) Then Return data
        Using stream = GetType(HelpForm).Assembly.GetManifestResourceStream($"PdfManager.help.help-{name}.png")
            If stream IsNot Nothing Then
                Using memory As New IO.MemoryStream()
                    stream.CopyTo(memory)
                    data = memory.ToArray()
                End Using
            End If
        End Using
        PictureCache(name) = data
        Return data
    End Function

    ''' <summary>A label in bold followed by its explanation, e.g. "Normal: …".</summary>
    Private Sub Term(label As String, text As String)
        Add("•  ", 10.5F, FontStyle.Regular, _palette.Accent, 8)
        Add(label & ": ", 10.5F, FontStyle.Bold, _palette.Text, 8)
        Add(text & vbLf, 10.5F, FontStyle.Regular, _palette.Text, 8)
    End Sub

    ' ---------------------------------------------------------------------
    ' The topics
    ' ---------------------------------------------------------------------

    Private Sub WriteGettingStarted()
        Para("PDF Manager works in three steps, from the top of the window to the bottom.")
        Picture("overview", "The main window: 1. the list of PDFs, 2. what to do with them, 3. where to save the result.")
        Heading("1. Add your PDFs")
        Para("Click Add PDF to pick files, or Add Folder to add every PDF in a folder (tick Include subfolders to look in the folders inside it too). You can also drag files and folders onto the list. Move Up, Move Down and Sort A–Z set the order; select a row and click Remove, or press Delete, to take it off the list.")
        Heading("2. Choose what to do")
        Para("Click one of the big buttons: Combine, Merge Pages, Extract Pages, Arrange Pages, Encrypt / Decrypt, Rotate, Stamp & Background or Reduce Size. Its options appear underneath. Each one has its own topic in this Help.")
        Heading("3. Choose where to save")
        Para("Pick the output folder and type a name for the new file, then press the big blue button. The line under the name shows exactly which files will be created.")
        Heading("Also: a password, a stamp or a background")
        Picture("extras", "The Also row in step 3: extras for every new PDF, whatever you chose in step 2.")
        Para("Tick Protect with a password, Add a stamp / background, or both, to give every new PDF these too, in the same step. The first time you tick one, its settings open; Password… and Stamp & Background… change them later. The line under the name says what the new PDFs will get.")
        Bullet("Protect with a password: the same choices as Encrypt (a password to open, optional limits on printing, copying and editing). It replaces any password the originals had.")
        Bullet("Add a stamp / background: a page of another PDF on top, behind, or both, on all pages or the pages you type (counted in the new PDF). Split results get it on every piece.")
        Bullet("Encrypt / Decrypt has its own password settings, and Stamp & Background its own stamp settings, so the matching extra is greyed out there.")
        Heading("Your originals are safe")
        Bullet("Every operation makes new files. The PDFs you added are never changed.")
        Bullet("If a file with the same name already exists, you're asked before it's replaced.")
        Bullet("Nothing is saved unless QPDF finishes and the new PDF opens correctly.")
    End Sub

    Private Sub WriteWhatsNew()
        Para("Recent additions to PDF Manager:")
        Term("Arrange Pages", "see every page as a small picture, drag pages into a new order and remove the ones you don't want, then save them as one PDF. See the Arrange Pages topic.")
        Term("Remove the password (decrypt)", "the Encrypt button is now Encrypt / Decrypt. Choose Remove the password to make unlocked copies of locked PDFs, with no password and no limits on printing, copying or editing. See the Encrypt / Decrypt topic.")
        Term("Faster batches", "several PDFs are now worked on at the same time, so encrypting, rotating, stamping, shrinking or splitting many files is several times faster. The line under the buttons counts the files done, for example Working… 5 of 16 done.")
        Term("Stamp and background together", "Stamp & Background can now add a stamp on top and a background behind in one go.")
        Term("Also: password, stamp, background", "step 3 can now give every new PDF a password and/or a stamp and background, whatever operation you choose, in one go. See Getting started.")
        Term("Pictures in Help", "the topics now show pictures of the window, so it's easier to find each option.")
    End Sub

    Private Sub WritePageRanges()
        Para("Wherever you choose pages, you can type a page list: in the Pages to Use column (Merge Pages), Pages to extract (Extract Pages), Pages to turn (Rotate) and Put it on pages (Stamp & Background). Type a list in the Try it box above to see exactly which pages it picks.")
        Heading($"Examples, and what they pick in a {ExamplePages}-page PDF")
        Gap()
        For Each example In PageRangeExamples
            Add(example.Text, 11.0F, FontStyle.Bold, _palette.Link, 8, "Consolas")
            Add("   " & example.Meaning & vbLf, 10.5F, FontStyle.Regular, _palette.Text, 8)
            Dim pages = PageRange.Expand(example.Text, ExamplePages)
            Add($"Picks: {ListPages(pages, 40)}" & vbLf & vbLf, 9.5F, FontStyle.Regular, _palette.Muted, 28)
        Next
        Heading("Good to know")
        Bullet("Spaces don't matter, and you can use ; instead of a comma.")
        Bullet("Page numbers start at 1. If you ask for a page a PDF doesn't have, the app says so before anything runs.")
        Bullet("odd and even work on their own, not mixed with other pieces.")
        Bullet("In Merge Pages each PDF has its own list. In Extract Pages, Rotate and Stamp & Background, the same list is used for every PDF.")
    End Sub

    Private Sub WriteCombine()
        Para("Joins all the pages of every PDF in the list into one new PDF, in the order shown in the list.")
        Bullet("Use Move Up, Move Down or Sort A–Z to change the order before you start.")
        Bullet("Tick Keep the first PDF's bookmarks and document details to keep its bookmarks, title and author in the new file.")
        Bullet("You need at least two PDFs. The result is one file with the name you type.")
        Bullet("To use only some pages of each PDF, use Merge Pages instead.")
    End Sub

    Private Sub WriteMerge()
        Para("Builds one new PDF from just the pages you choose from each PDF.")
        Picture("merge", "Merge Pages adds the Use and Pages to Use columns to the list.")
        Bullet("Tick the PDFs to use in the Use column. Tick All and Untick All save time.")
        Bullet("Click a PDF's Pages to Use cell and type its pages. Leave it as All for every page.")
        Bullet("The chosen pages are joined in list order: all the chosen pages of the first ticked PDF, then the second, and so on.")
        Gap()
        Heading("Example")
        Para("To take pages 1–10 from the first PDF and page 5 from the second, tick both, type 1-10 in the first row's Pages to Use and 5 in the second row's.")
        Para("A page that doesn't exist shows in red in the list, and the app explains the problem before running. See Page ranges for everything you can type.")
    End Sub

    Private Sub WriteExtract()
        Para("Copies chosen pages into new PDFs, leaving the original as it is.")
        Picture("extract", "The Extract Pages options.")
        Bullet("Pages to extract: type the pages, for example 1-3, 5, 8-end. See Page ranges.")
        Bullet("Take them from: the PDF selected in the list, or every PDF in the list.")
        Gap()
        Heading("Save them as")
        Term("One PDF", "all the chosen pages in a single file.")
        Term("One PDF per page", "a separate file for each page, numbered in order: name-1.pdf, name-2.pdf… (name-01.pdf, name-02.pdf… when there are 10 or more).")
        Term("A new PDF every N pages", "splits the chosen pages into files of N pages each, named by the pages they hold, such as name-1-5.pdf and name-6-10.pdf.")
        Gap()
        Para("When taking pages from several PDFs into separate files, each file name includes the original PDF's name.")
    End Sub

    Private Sub WriteEncrypt()
        Para("The Encrypt / Decrypt button adds a password to your PDFs, or takes one off. Choose with What to do at the top of its options.")
        Heading("Add a password (encrypt)")
        Picture("encrypt", "Adding a password: type it twice, and optionally set limits with a permissions password.")
        Para("Makes a password-protected copy of every PDF in the list, using strong AES 256-bit encryption.")
        Term("Password to open", "needed to open the new PDF. Type it twice to avoid mistakes.")
        Term("Permissions password", "optional. With it you can stop people printing, copying text or editing. They can still open the file with the first password.")
        Term("People who open it may", "untick Print, Copy text or Edit to limit them. Limits need a permissions password that differs from the password to open.")
        Gap()
        Bullet("With only a permissions password, anyone can open the PDF but the limits apply.")
        Bullet("Each copy is named after its original, for example report-protected.pdf.")
        Bullet("A forgotten password can't be recovered, so keep it somewhere safe.")
        Bullet("To protect the result of another operation (for example a combined PDF), tick Protect with a password in step 3 instead; then it's done in the same step.")
        Gap()
        Heading("Remove the password (decrypt)")
        Picture("decrypt", "Removing a password: the boxes for a new password are greyed out, and the big button says Remove Passwords.")
        Para("Makes an unlocked copy of every locked PDF in the list, with no password to open it and no limits on printing, copying or editing.")
        Bullet("You're asked for any password you haven't typed yet; you must know it. Or double-click a locked PDF in the list first to type it.")
        Bullet("A PDF that opens without a password but blocks printing or copying is unlocked without needing its permissions password.")
        Bullet("PDFs with no password at all are skipped, and the result says how many.")
        Bullet("Copies are named after their originals, for example report-unlocked.pdf. The originals keep their passwords.")
    End Sub

    Private Sub WriteRotate()
        Para("Turns pages the right way up. Every PDF in the list gets its own rotated copy.")
        Picture("rotate", "The Rotate options.")
        Bullet("Turn the pages: 90° clockwise, 90° anticlockwise, or upside down (180°).")
        Bullet("Pages to turn: leave it empty for every page, or type a list such as 2, 4-6 or even. See Page ranges.")
        Bullet("Turning only changes how pages are shown. Nothing on the page is lost, and you can turn them back any time.")
    End Sub

    Private Sub WriteStamp()
        Para("Lays a page of another PDF over (a stamp) and/or under (a background) the pages of every PDF in the list.")
        Picture("stamp", "The Stamp & Background options: choose a stamp only, a background only, or both.")
        Term("A stamp only, on top", "for logos, watermarks, page footers or an ""Approved"" mark.")
        Term("A background only, behind", "for letterheads or paper designs.")
        Term("Both a stamp and a background", "both at once, in a single step: for example a letterhead behind the page and an ""Approved"" mark on top.")
        Gap()
        Bullet("Click Choose… on the Stamp PDF row, the Background PDF row, or both, depending on your choice. The row you don't need is greyed out.")
        Bullet("Use page picks which page of that PDF to use; the same page goes on every chosen page. The stamp and the background can come from different PDFs, or from different pages of the same PDF.")
        Bullet("Put them on pages: leave it empty for every page, or type a list such as 1 or 2-end. See Page ranges.")
        Bullet("To stamp the result of another operation (for example a combined or extracted PDF), tick Add a stamp / background in step 3 instead.")
        Bullet("Tip: design the stamp in Word or any program that can save as PDF. Leave the rest of the page empty, so only the stamp shows.")
        Bullet("A background only shows where pages are see-through. Scanned pages are solid pictures, so they hide it; use a stamp instead.")
    End Sub

    Private Sub WriteCompress()
        Para("Makes a smaller copy of every PDF in the list. Text and page layout stay the same.")
        Picture("compress", "The four Reduce Size levels.")
        Term("Normal", "repacks the file more efficiently with no change in quality. Helps most with PDFs that were saved inefficiently.")
        Term("Medium (recommended)", "also saves photos and scans at good JPEG quality. Usually the best balance.")
        Term("High", "photos at lower quality for a smaller file. Fine for reading on screen.")
        Term("Extreme", "the smallest file. Photos look visibly softer, and the document details, page labels and accessibility tags are removed.")
        Gap()
        Bullet("When it finishes, the app shows the size before and after, for example 2.1 MB → 0.8 MB, 62% smaller.")
        Bullet("A photo is only replaced when the new version is smaller, so a file never gets bigger because of its pictures.")
        Bullet("PDFs that are mostly text are already small and shrink less. Picture sizes (resolution) aren't reduced, only their quality.")
    End Sub

    Private Sub WriteArrange()
        Para("Shows every page of the PDFs in the list as a small picture, so you can see them, put them in a new order and leave out the ones you don't want. The kept pages are saved as one new PDF.")
        Picture("arrange", "The Arrange Pages options: Show Page Thumbnails opens the page view; the line beside it says what you've changed.")
        Bullet("Click Show Page Thumbnails. Pages appear in list order; a coloured strip under each page shows which PDF it comes from.")
        Picture("pages", "The page view. The big number under each page is its place in the new PDF; below it, the PDF and page it comes from.")
        Bullet("Drag a page to move it. A blue line shows where it will go. Drag near the top or bottom edge to scroll.")
        Bullet("Click a page's red ✕, or select pages and press Delete (or Remove Selected), to leave them out.")
        Bullet("Ctrl+click picks several pages, Shift+click a run of pages, Ctrl+A all of them. Dragging moves every selected page together.")
        Bullet("Move to Start and Move to End send the selected pages to the beginning or the end. Alt+Left and Alt+Right move them one place.")
        Bullet("Picture size, or Ctrl+mouse wheel, makes the pictures bigger or smaller. Start Over brings back every page in list order.")
        Bullet("Use These Pages keeps your choice; then choose where to save and press Save Pages. Nothing is saved before that.")
        Gap()
        Bullet("PDFs added to the list later join the end of your arrangement; PDFs removed from the list leave it.")
        Bullet("The pictures are drawn by Windows' own PDF viewer. If one can't be drawn, it shows the page number instead, and QPDF still saves the page.")
        Bullet("Locked PDFs ask for their password first. The new PDF has no password; use Encrypt afterwards to protect it.")
    End Sub

    Private Sub WritePasswords()
        Para("PDFs that need a password show Needs password in the list's Status column.")
        Bullet("Double-click the PDF in the list to type its password, or just start; the app asks for any passwords it needs.")
        Bullet("Passwords are only kept while the app is open. They're never saved to disk.")
        Bullet("Rotate, Stamp & Background and Reduce Size keep the original password on the new copy.")
        Bullet("Combine, Merge Pages, Extract Pages and Arrange Pages make unlocked files, unless you tick Protect with a password in step 3.")
        Bullet("To take a password off for good, use Encrypt / Decrypt and choose Remove the password (decrypt). You need to know the password.")
        Bullet("Locked PDFs can be shown in the Arrange Pages page view too, once their password is typed.")
        Bullet("The stamp or background PDF can have a password too. You're asked for it when you choose the file.")
    End Sub

    Private Sub WriteTips()
        Term("F1", "opens Help on the topic for the operation you're using.")
        Term("Delete", "removes the selected PDFs from the list. Hold Ctrl or Shift to select several.")
        Term("Drag and drop", "drop PDFs or whole folders onto the window to add them.")
        Term("Theme", "the buttons at the top right switch between Light, Dark and Same as Windows. Your choice is remembered.")
        Term("Show details", "shows exactly what QPDF was asked to do (passwords hidden) and anything it reported.")
        Term("Open File / Open Folder", "open the result as soon as it's ready.")
        Term("Cancel", "stops a long job. Nothing is saved, and existing files are untouched.")
        Term("Use a different QPDF…", "at the bottom right, picks another copy of QPDF if you ever need to.")
        Term("Working… 5 of 16 done", "while a batch runs, the line under the buttons counts the files finished. Several files are worked on at once.")
        Heading("In the page view (Arrange Pages)")
        Term("Drag", "moves the page, or every selected page. A blue line shows where they'll go.")
        Term("Ctrl+click / Shift+click / Ctrl+A", "select several pages, a run of pages, or all of them.")
        Term("Delete or the red ✕", "leaves pages out.")
        Term("Arrow keys, Home, End", "move between pages. Alt+Left and Alt+Right move the selected pages one place.")
        Term("Ctrl+mouse wheel", "makes the pictures bigger or smaller.")
        Term("Enter / Esc", "Use These Pages / Cancel.")
    End Sub

    Private Sub WriteAbout()
        Dim version = GetType(HelpForm).Assembly.GetName().Version
        Heading("PDF Manager")
        Para($"Version {version.Major}.{version.Minor}.{version.Build}")
        Para("Combine, merge, extract, arrange, protect and unlock, rotate, stamp and shrink PDFs. All the PDF work is done by QPDF, a free and widely used PDF tool. Page pictures are drawn by Windows' own PDF viewer.")
        Dim status = _qpdfStatus()
        If Not String.IsNullOrWhiteSpace(status) Then Para(status & ".")
        Heading("Credits")
        Para("QPDF is © Jay Berkenbilt and Manfred Holger, under the Apache License 2.0 (qpdf.sourceforge.io). Its licence files are in the qpdf folder next to the program.")
    End Sub

End Class
