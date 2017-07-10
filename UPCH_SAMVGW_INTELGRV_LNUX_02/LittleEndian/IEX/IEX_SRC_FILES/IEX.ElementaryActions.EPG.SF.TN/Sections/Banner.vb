Imports FailuresHandler

Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Banner

    Dim _UI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To Action Bar
    ''' </summary>
    ''' <remarks></remarks>

    Overrides Sub Navigate(Optional ByVal FromPlayback As Boolean = False)

        _UI.Utils.StartHideFailures("Navigating To Action Bar")
        Try
            _iex.Wait(4)

            If FromPlayback Then

                _UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR")
            Else
                _UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR")
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Preparing Recording Event From Action Bar - Navigating To Confirm Record Without Confirming The Record
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para></para> 
    ''' </remarks>
    Public Overrides Sub PreRecordEvent(Optional ByVal IsSeries As Boolean = False)

        _UI.Utils.StartHideFailures("Preparing Recording Event From Action Bar")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("RECORD")

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Stopping Recording Event From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>348 - StopRecordEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub StopRecording()
        'EPG TEXT
        Dim EpgText As String = ""
        Dim Milestones As String = ""

        _UI.Utils.StartHideFailures("Stopping Recording Event From Action Bar")

        Try
            _UI.Utils.LogCommentInfo("Banner.StopRecording : Stopping Recording Event From Action Bar")

            _UI.Utils.EPG_Milestones_SelectMenuItem("STOP RECORDING")

            _UI.Utils.ClearEPGInfo()

            'MILESTONES MESSAGES

            Milestones = _UI.Utils.GetValueFromMilestones("StopRecording")

            Dim ActualLines As New ArrayList

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 10)

            _UI.Utils.ClearEPGInfo()

            _UI.Utils.SendIR("SELECT")

            _UI.Utils.EPG_Milestones_SelectMenuItem("YES")

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify StopRecording Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class
