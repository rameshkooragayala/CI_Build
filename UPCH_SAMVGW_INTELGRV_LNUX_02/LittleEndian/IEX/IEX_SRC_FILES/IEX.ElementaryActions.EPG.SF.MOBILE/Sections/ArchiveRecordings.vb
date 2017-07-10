Imports FailuresHandler

Public Class ArchiveRecordings
    Inherits IEX.ElementaryActions.EPG.SF.ArchiveRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
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

            If isArchive() Then
                Exit Sub
            End If

            _uUI.Menu.Navigate()
            _uUI.Utils.Tap("LIBRARY")
            _uUI.Utils.ClearEPGInfo()
            _uUI.Utils.Tap("MY RECORDINGS")

            If Not _uUI.Utils.VerifyState("FullContentMY RECORDINGS") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Archive Is On Screen"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Function isArchive() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Archive Is On Screen")

        Try
            If _uUI.Utils.VerifyState("FullContentMY RECORDINGS", 2) Then
                Msg = "Archive Is On Screen"
                Return True
            Else
                Msg = "Archive Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    ''' <summary>
    '''   Checks If Archive Has No Events
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isEmpty() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Archive Is Empty")

        Try
            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("evtNa", EventName)
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

    Public Overrides Sub FindEvent(ByVal EventName As String, Optional ByVal EventDate As String = "", Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "")
        _uUI.Utils.FindInEvents("Archive.Events", EventName)
    End Sub
End Class