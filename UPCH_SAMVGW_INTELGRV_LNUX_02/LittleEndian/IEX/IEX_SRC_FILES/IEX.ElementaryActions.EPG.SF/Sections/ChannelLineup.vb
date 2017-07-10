Imports FailuresHandler

Public Class ChannelLineup
    Inherits IEX.ElementaryActions.EPG.ChannelLineup

    Dim _UI As UI
    Private Shadows _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Channel Lineup
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _Utils.StartHideFailures("Navigating To Channel Lineup")

        Try
            _Utils.EPG_Milestones_Navigate("CHANNEL LINEUP")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Checks If The EPG Is On Channel Lineup
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsChannelLineUp() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Channel Lineup Is On The Screen")

        Try
            If _Utils.VerifyState("CHANNEL LINEUP", 2) Then
                Msg = "Channel Lineup Is On Screen"
                Return True
            Else
                Msg = "Channel Lineup Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

    ''' <summary>
    '''   Surfing Down On Channel Lineup
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelDown()
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.StartHideFailures("Surfing Down On Channel Lineup")

        Try
            Milestones = _Utils.GetValueFromMilestones("ChannelLineupSurf")

            _Utils.BeginWaitForDebugMessages(Milestones, 15)

            'Fetch the channel surf up key
            Dim chSurfDownKey As String = GetSurfKey(False)
            Dim chSurfTimeoutInMSec As Integer = GetChannelSurfTimeout()
            _Utils.SendIR(chSurfDownKey, chSurfTimeoutInMSec)

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify ChannelLineupSurf Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Surfing Up On Channel Lineup
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelUp()
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.StartHideFailures("Surfing Up On Channel Lineup")

        Try
            Milestones = _Utils.GetValueFromMilestones("ChannelLineupSurf")

            _Utils.BeginWaitForDebugMessages(Milestones, 15)

            'Fetch the channel surf up key
            Dim chSurfUpKey As String = GetSurfKey()
            Dim chSurfTimeoutInMSec As Integer = GetChannelSurfTimeout()

            _Utils.SendIR(chSurfUpKey, chSurfTimeoutInMSec)
            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify ChannelLineupSurf Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Get the channel surf key    
    ''' </summary>
    ''' <param name="surfUp">Optional. The surf direction.</param>
    ''' <returns>The surf key from the project configuration or the default value from the code</returns>
    Overrides Function GetSurfKey(Optional ByVal surfUp As Boolean = True) As String
        Dim surfUpKeyDefault As String = "SELECT_UP"
        Dim surfDownKeyDefault As String = "SELECT_DOWN"
        Dim surfKey As String = ""
        Dim surfDirection As String = ""

        'Checking surf direction
        If surfUp Then
            surfDirection = "UP"
        Else
            surfDirection = "DOWN"
        End If

        Try
            'Get the key from Project ini
            surfKey = _Utils.GetValueFromProject("CHANNEL_LINEUP", "CHANNEL_SURF_" + surfDirection + "_KEY")
        Catch
            'Taking default value
            _Utils.LogCommentWarning("CHANNEL_SURF_" + surfDirection + "_KEY in section CHANNEL_LINEUP is missing in your project configuration file. Please add it if it deviates from the default value!")

            If surfUp Then
                surfKey = surfUpKeyDefault
            Else
                surfKey = surfDownKeyDefault
            End If

            _Utils.LogCommentWarning("Taking default value from code instead - " + surfKey)
        End Try

        Return surfKey
    End Function

    ''' <summary>
    ''' Get the channel surf timeout required
    ''' </summary>
    ''' <returns>Timeout in milliseconds as integer</returns>
    ''' <remarks></remarks>
    Overrides Function GetChannelSurfTimeout() As Integer
        Dim chSurfTimeout As String = "5"
        Try
            chSurfTimeout = _Utils.GetValueFromProject("CHANNEL_LINEUP", "CHANNEL_SURF_TIMEOUT_SEC")
        Catch
            _Utils.LogCommentWarning("CHANNEL_SURF_TIMEOUT_SEC in section CHANNEL_LINEUP is missing in your project configuration file. Please add it if it deviates from the default value!")
            _Utils.LogCommentWarning("Taking default value from code instead - " + chSurfTimeout)
        End Try
        Return CInt(chSurfTimeout) * 1000
    End Function

    ''' <summary>
    '''   Select Channel From Channel Line-up
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectChannel()
        Dim ActualLines As New ArrayList
        Dim EventSessionStopResult As Boolean
        Dim LiveSurfNotPredictedResult As Boolean
        Dim EventSessionStop_Milestones As String = ""
        Dim LiveSurfNotPredicted_Milestones As String = ""

        _UI.Utils.StartHideFailures("Selecting Channel On Channel Lineup")

        Try
            _UI.Utils.StartHideFailures("Checking LiveSurfPredicted Milestones")

            Try
                EventSessionStop_Milestones = _Utils.GetValueFromMilestones("EventSessionStop")
                LiveSurfNotPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfNotPredicted")

                _UI.Utils.BeginWaitForDebugMessages(EventSessionStop_Milestones, 10)
                _UI.Utils.BeginWaitForDebugMessages(LiveSurfNotPredicted_Milestones, 10)

                _UI.Utils.SendIR("SELECT")

                If _UI.Utils.EndWaitForDebugMessages(EventSessionStop_Milestones, ActualLines) Then
                    EventSessionStopResult = True
                Else
                    EventSessionStopResult = False
                End If
            Finally
                _iex.ForceHideFailure()
            End Try

            _UI.Utils.StartHideFailures("Checking LiveSurfNotPredicted Milestones")

            Try
                If _UI.Utils.EndWaitForDebugMessages(LiveSurfNotPredicted_Milestones, ActualLines) Then
                    LiveSurfNotPredictedResult = True
                Else
                    LiveSurfNotPredictedResult = False
                End If

            Finally
                _iex.ForceHideFailure()
            End Try

            If EventSessionStopResult = False And LiveSurfNotPredictedResult = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify Both EventSessionStop Milestones : " + EventSessionStop_Milestones + " And LiveSurfNotPredicted Milestones : " + LiveSurfNotPredicted_Milestones + " After Tunning"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class
