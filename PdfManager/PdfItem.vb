Imports System.ComponentModel
Imports System.IO

Public Enum PdfState
    Checking
    Ready
    PasswordNeeded
    Unreadable
End Enum

''' <summary>One PDF in the list. The list on screen is bound directly to these.</summary>
Public Class PdfItem
    Implements INotifyPropertyChanged

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Private _position As Integer
    Private _pageCount As Integer?
    Private _state As PdfState = PdfState.Checking
    Private _pagesText As String = "All"
    Private _problem As String
    Private _included As Boolean = True

    Public Sub New(filePath As String)
        Me.Path = System.IO.Path.GetFullPath(filePath)
        Try
            SizeBytes = New FileInfo(Me.Path).Length
        Catch ex As Exception When TypeOf ex Is IOException OrElse TypeOf ex Is UnauthorizedAccessException
            SizeBytes = 0
        End Try
    End Sub

    Public ReadOnly Property Path As String
    Public ReadOnly Property SizeBytes As Long

    ''' <summary>The password that opens this file. Kept in memory only, never saved.</summary>
    Public Property Password As String

    ''' <summary>
    ''' Whether the file has a password or limits on printing, copying or editing. Nothing until
    ''' asked (only Remove the password needs to know).
    ''' </summary>
    Public Property IsEncrypted As Boolean?

    Public ReadOnly Property FileName As String
        Get
            Return System.IO.Path.GetFileName(Path)
        End Get
    End Property

    Public ReadOnly Property BaseName As String
        Get
            Return System.IO.Path.GetFileNameWithoutExtension(Path)
        End Get
    End Property

    Public ReadOnly Property Folder As String
        Get
            Return System.IO.Path.GetDirectoryName(Path)
        End Get
    End Property

    Public ReadOnly Property SizeText As String
        Get
            If SizeBytes < 1024 Then Return $"{SizeBytes} B"
            If SizeBytes < 1024 * 1024 Then Return $"{SizeBytes / 1024.0:0} KB"
            If SizeBytes < 1024L * 1024 * 1024 Then Return $"{SizeBytes / (1024.0 * 1024):0.0} MB"
            Return $"{SizeBytes / (1024.0 * 1024 * 1024):0.00} GB"
        End Get
    End Property

    Public Property Position As Integer
        Get
            Return _position
        End Get
        Set(value As Integer)
            If _position <> value Then
                _position = value
                Notify(NameOf(Position))
            End If
        End Set
    End Property

    Public Property PageCount As Integer?
        Get
            Return _pageCount
        End Get
        Set(value As Integer?)
            _pageCount = value
            Notify(NameOf(PageCount))
            Notify(NameOf(PageCountText))
        End Set
    End Property

    Public ReadOnly Property PageCountText As String
        Get
            Return If(_pageCount.HasValue, _pageCount.Value.ToString(), "–")
        End Get
    End Property

    Public Property State As PdfState
        Get
            Return _state
        End Get
        Set(value As PdfState)
            _state = value
            Notify(NameOf(State))
            Notify(NameOf(StatusText))
        End Set
    End Property

    ''' <summary>Why a file can't be read, in QPDF's words.</summary>
    Public Property Problem As String
        Get
            Return _problem
        End Get
        Set(value As String)
            _problem = value
            Notify(NameOf(Problem))
        End Set
    End Property

    Public ReadOnly Property StatusText As String
        Get
            Select Case _state
                Case PdfState.Checking
                    Return "Reading…"
                Case PdfState.PasswordNeeded
                    Return "Needs password"
                Case PdfState.Unreadable
                    Return "Can't be read"
                Case Else
                    Return If(String.IsNullOrEmpty(Password), "Ready", "Unlocked")
            End Select
        End Get
    End Property

    ''' <summary>Whether Merge Pages uses this PDF: the tick in the Use column.</summary>
    Public Property Included As Boolean
        Get
            Return _included
        End Get
        Set(value As Boolean)
            If _included <> value Then
                _included = value
                Notify(NameOf(Included))
            End If
        End Set
    End Property

    ''' <summary>The pages typed in the list for Merge Pages, such as "1-3, 5". Blank or "All" means every page.</summary>
    Public Property PagesText As String
        Get
            Return _pagesText
        End Get
        Set(value As String)
            _pagesText = If(String.IsNullOrWhiteSpace(value), "All", value.Trim())
            Notify(NameOf(PagesText))
        End Set
    End Property

    Private Sub Notify(propertyName As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub

End Class
