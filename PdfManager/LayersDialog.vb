Imports System.IO
Imports System.Windows.Forms

''' <summary>Step 3's "Add a stamp / background": what every new PDF gets on top and/or behind its pages.</summary>
Public Class LayersDialog

    Private ReadOnly _readPdf As Func(Of String, Task(Of PdfItem))
    Private _stamp As PdfItem
    Private _background As PdfItem
    Private _palette As Palette = Palette.LightPalette

    ''' <summary>Where Choose… starts looking.</summary>
    <System.ComponentModel.Browsable(False), System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)>
    Friend Property StartFolder As String

    Public Sub New()
        Me.New(Nothing, Nothing)
    End Sub

    ''' <param name="current">The settings to start from, or Nothing.</param>
    ''' <param name="readPdf">Reads a chosen PDF with QPDF (and asks for its password), giving it back ready to use.</param>
    Public Sub New(current As LayerSettings, readPdf As Func(Of String, Task(Of PdfItem)))
        InitializeComponent()
        _readPdf = readPdf
        If current IsNot Nothing Then
            rbBackgroundOnly.Checked = current.Mode = StampMode.BackgroundOnly
            rbBoth.Checked = current.Mode = StampMode.Both
            rbStampOnly.Checked = current.Mode = StampMode.StampOnly
            SetStamp(current.StampItem)
            SetBackground(current.BackgroundItem)
            numStampPage.Value = Math.Clamp(current.StampPageNumber, 1, CInt(numStampPage.Maximum))
            numBackgroundPage.Value = Math.Clamp(current.BackgroundPageNumber, 1, CInt(numBackgroundPage.Maximum))
            txtPages.Text = If(PageRange.IsAll(current.Pages), "", current.Pages)
        End If
        UpdateRows()
    End Sub

    ''' <summary>What was chosen.</summary>
    Public ReadOnly Property Settings As LayerSettings
        Get
            Return New LayerSettings With {
                .Mode = If(rbBoth.Checked, StampMode.Both, If(rbBackgroundOnly.Checked, StampMode.BackgroundOnly, StampMode.StampOnly)),
                .StampItem = _stamp, .StampPageNumber = CInt(numStampPage.Value),
                .BackgroundItem = _background, .BackgroundPageNumber = CInt(numBackgroundPage.Value),
                .Pages = txtPages.Text}
        End Get
    End Property

    Friend Sub ApplyPalette(p As Palette)
        _palette = p
        Theme.Apply(Me, p)
        lblProblem.ForeColor = p.ErrorText
    End Sub

    Protected Overrides Sub OnHandleCreated(e As EventArgs)
        MyBase.OnHandleCreated(e)
        Theme.UseDarkTitleBar(Me, _palette.IsDark)
    End Sub

    Private Sub ModeChanged(sender As Object, e As EventArgs) Handles rbStampOnly.CheckedChanged, rbBackgroundOnly.CheckedChanged, rbBoth.CheckedChanged
        UpdateRows()
    End Sub

    ''' <summary>The row that isn't needed is greyed out.</summary>
    Private Sub UpdateRows()
        Dim stampOn = Not rbBackgroundOnly.Checked
        Dim backgroundOn = Not rbStampOnly.Checked
        For Each c As Control In New Control() {lblStamp, txtStamp, btnStamp, lblStampPage, numStampPage}
            c.Enabled = stampOn
        Next
        For Each c As Control In New Control() {lblBackground, txtBackground, btnBackground, lblBackgroundPage, numBackgroundPage}
            c.Enabled = backgroundOn
        Next
    End Sub

    Friend Sub SetStamp(item As PdfItem)
        _stamp = item
        txtStamp.Text = If(item?.Path, "")
        If item?.PageCount IsNot Nothing Then numStampPage.Maximum = Math.Max(1, item.PageCount.Value)
    End Sub

    Friend Sub SetBackground(item As PdfItem)
        _background = item
        txtBackground.Text = If(item?.Path, "")
        If item?.PageCount IsNot Nothing Then numBackgroundPage.Maximum = Math.Max(1, item.PageCount.Value)
    End Sub

    Private Async Sub btnStamp_Click(sender As Object, e As EventArgs) Handles btnStamp.Click
        Dim item = Await ChooseAsync("Choose the stamp PDF")
        If item IsNot Nothing Then SetStamp(item)
    End Sub

    Private Async Sub btnBackground_Click(sender As Object, e As EventArgs) Handles btnBackground.Click
        Dim item = Await ChooseAsync("Choose the background PDF")
        If item IsNot Nothing Then SetBackground(item)
    End Sub

    Private Async Function ChooseAsync(title As String) As Task(Of PdfItem)
        Dim chosen As String
        Using dialog As New OpenFileDialog With {.Title = title, .Filter = "PDF files (*.pdf)|*.pdf", .CheckFileExists = True}
            If Directory.Exists(StartFolder) Then dialog.InitialDirectory = StartFolder
            If dialog.ShowDialog(Me) <> DialogResult.OK Then Return Nothing
            chosen = dialog.FileName
        End Using
        lblProblem.Text = ""
        If _readPdf Is Nothing Then Return New PdfItem(chosen)
        Dim item = Await _readPdf(chosen)
        If item IsNot Nothing AndAlso item.State <> PdfState.Ready Then
            lblProblem.Text = If(item.State = PdfState.PasswordNeeded,
                                 $"{item.FileName} needs its password before it can be used.",
                                 $"{item.FileName} can't be read: {item.Problem}")
            Return Nothing
        End If
        Return item
    End Function

    ''' <summary>Only closes with settings that will work.</summary>
    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Dim issue = Planner.ValidateLayers(Settings)
        If issue IsNot Nothing Then
            lblProblem.Text = issue.Message.Replace("Add a stamp / background: ", "")
            Return
        End If
        DialogResult = DialogResult.OK
    End Sub

End Class
