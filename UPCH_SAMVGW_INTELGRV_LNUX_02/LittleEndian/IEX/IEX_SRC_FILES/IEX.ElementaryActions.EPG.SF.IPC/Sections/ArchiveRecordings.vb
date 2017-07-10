Imports FailuresHandler
Imports System.Runtime.InteropServices

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.UPC.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.IPC.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

    ''' <summary>
    '''   Navigating To Archive Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        Try
            _uUI.Utils.StartHideFailures("Navigating To Archive")

            _uUI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY")

            _uUI.Utils.ClearEPGInfo()

            If _UI.Menu.IsLibraryNoContent Then
                _uUI.Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY RECORDINGS")
            Else
                _uUI.Utils.EPG_Milestones_Navigate("MY RECORDINGS")
            End If

            _uUI.Utils.LogCommentWarning("WORKAROUND (Till Enabled At State Machine Level) Waiting Extra Time Before Milestones")
            _iex.Wait(2)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checks If Archive Has No Events
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isEmpty() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Archive Is Empty")

        _uUI.Utils.LogCommentWarning("WORKAROUND (Till Enabled At State Machine Level) Waiting Extra Time Before Milestones")
        _iex.Wait(2)

        Try
            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("displayTitle", EventName)
                Msg = "Archive Is Not Empty"
                Return False
            Catch ex As EAException
                Msg = "Archive Is Empty !!!"
                Return True
            End Try
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

End Class