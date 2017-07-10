Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''    Do STB Health Check By Checking Tune To Channel,Checking For Video,Navigate And Get Event Name From Guide And Lunching The Action Bar
    ''' </summary>
    Public Class HealthCheck
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _ChannelNumber As String

        ''' <param name="ChannelNumber">Channel Number To Tune To</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks></remarks>
        Sub New(ByVal ChannelNumber As String, ByVal pManager As IEX.ElementaryActions.Manager)
            Me._ChannelNumber = ChannelNumber
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EPG As EPG.UI = _manager.UI
            Dim eaFailed As Boolean = False

            '1. Tune To Channel
            '------------------
            EPG.Utils.StartHideFailures("Checking 'Tune To Channel'")

            res = _manager.ChannelSurf(EnumSurfIn.Live, Me._ChannelNumber)
            If Not res.CommandSucceeded Then
                _iex.ForceHideFailure()
                EPG.Utils.LogCommentFail("Failures Found While Checking 'Tune to Channel' : " + res.FailureReason)
                eaFailed = True
            Else
                _iex.ForceHideFailure()
            End If

            '2. Check For Video 
            '----------------------------
            EPG.Utils.StartHideFailures("Checking 'Video'")

            res = _manager.CheckForVideo(True, True, 20)
            If Not res.CommandSucceeded Then
                _iex.ForceHideFailure()
                EPG.Utils.LogCommentFail("Failures Found While Checking 'Video' : " + res.FailureReason)
                eaFailed = True
            Else
                _iex.ForceHideFailure()
            End If

            '4. Check Action Bar
            '-------------
            EPG.Utils.StartHideFailures("Checking 'Banner Information'")
            Try
                EPG.Banner.Navigate()
            Catch ex As Exception
                _iex.ForceHideFailure()
                EPG.Utils.LogCommentFail("Failures Found While Checking 'Banner Information' : " + res.FailureReason)
                eaFailed = True
            End Try

            '3. Check Guide
            '--------------
            Dim EventName As String = ""
            EPG.Utils.StartHideFailures("Checking 'Grid Information'")
            Try
                EPG.Guide.Navigate()
            Catch ex As EAException
                _iex.ForceHideFailure()
                EPG.Utils.LogCommentFail("Failures Found While Checking 'Grid Information' : " + ex.Message)
                eaFailed = True
            End Try

            EPG.Guide.GetSelectedEventName(EventName)

            EPG.Utils.StartHideFailures("Return To Live Viewing")
            res = _manager.ReturnToLiveViewing(False)
            If Not res.CommandSucceeded Then
                _iex.ForceHideFailure()
                EPG.Utils.LogCommentFail("Failed To Return To Live Viewing : " + res.FailureReason)
                eaFailed = True
            Else
                _iex.ForceHideFailure()
            End If

            If eaFailed Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.HealthCheckFailure, "Please Check Logs."))
            End If

        End Sub

    End Class
End Namespace
