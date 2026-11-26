Imports System.Windows.Forms

''' <summary>
''' A tab control whose tabs and border are hidden while the app runs, so its pages show as one
''' plain panel and the big operation buttons above choose between them. In Visual Studio's
''' designer the tabs still show, so each page can be edited as usual.
''' </summary>
Public Class OptionsTabControl
    Inherits TabControl

    Private Const TCM_ADJUSTRECT As Integer = &H1328

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = TCM_ADJUSTRECT AndAlso Not DesignMode Then
            ' Let each page fill the whole control: no tab strip, no border.
            m.Result = New IntPtr(1)
            Return
        End If
        MyBase.WndProc(m)
    End Sub

End Class
