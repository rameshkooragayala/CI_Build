Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Utils

    Dim _UI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub


    Public Overrides Sub ReturnToLiveViewing(Optional ByVal CheckForVideo As Boolean = False)
        Dim res As IEXGateway._IEXResult
        Dim EpgText As String = ""
        Dim State As String = ""
        Dim Msg As String = ""

        StartHideFailures("Checking If Already In Live")

        Try
            If VerifyState("LIVE", 5) Then
                _UI.Utils.LogCommentInfo("Already On Live")
                Exit Sub
            End If
            _UI.Utils.LogCommentInfo("Not already On Live")

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                LogCommentInfo(Msg)
            End If
        End Try

        StartHideFailures("Returning To Live Viewing")

        Try
            _UI.Menu.Navigate()

            res = _iex.MilestonesEPG.NavigateByName("STATE:LIVE")
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(res))
            End If

            If Not VerifyState("LIVE", 25) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ReturnToLiveFailure, "Failed To Verify State Is LIVE"))
            End If

            If CheckForVideo Then
                StartHideFailures("Checking Video Is On The Screen")
                Try
                    res = _iex.CheckForVideo(VideoCords, True, 15)
                    If Not res.CommandSucceeded Then
                        res = _iex.CheckForVideo(VideoCords(IsFullArea:=False, Area:=2), True, 15)
                        If Not res.CommandSucceeded Then
                            res = _iex.CheckForVideo(VideoCords(IsFullArea:=False, Area:=3), True, 15)
                            If Not res.CommandSucceeded And res.CommandExecutionFailed Then
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VideoNotPresent, "Failed To Check Video Exists"))
                            End If
                        End If
                    End If
                Finally
                    _iex.ForceHideFailure()
                End Try

            End If

            _UI.Live.WaitAfterLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class


