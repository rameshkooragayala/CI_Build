Imports FailuresHandler
Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Banner

    Dim _UI As IEX.ElementaryActions.EPG.SF.GET.UI


    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI

    End Sub


    ''' <summary>
    '''     Stopping Recording Event From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub CancelBooking(Optional ByVal IsSeries As Boolean = False, Optional ByVal IsComplete As Boolean = False)
        'EPG TEXT
        Dim EpgText As String = ""
        Dim Milestones As String = ""

        _UI.Utils.StartHideFailures("Canceling Booked Event From Action Bar")

        Try
            _UI.Utils.StartHideFailures("Trying To Navigate To CANCEL THIS EPISODE")
            Try
                Try
                    _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL THIS EPISODE")
                Catch ex2 As Exception
                    _iex.ForceHideFailure()
                    _UI.Utils.StartHideFailures("Trying To Navigate To CANCEL RECORDING")
                    _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL RECORDING")
                End Try
            Finally
                _iex.ForceHideFailure()
            End Try

            Dim ActualLines As New ArrayList

            'MILESTONES MESSAGES
            Milestones = _UI.Utils.GetValueFromMilestones("CancelBooking")

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 15)

            'Not Checking If Action Bar Disappeared Cause When Canceling Event From Guide Need To Check Change And Not Disappear
            _UI.Utils.SendIR("SELECT", 5000)

            _UI.Utils.SendIR("SELECT")
            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

	''' <summary>
    '''   Setting Event Keep From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>312 - SetEventKeepFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetEventKeep()

        _UI.Utils.StartHideFailures("Setting Event Keep")

        Try

            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            Milestones = _UI.Utils.GetValueFromMilestones("SetKeep")

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 30)

            _UI.Utils.EPG_Milestones_Navigate("A//V SETTINGS/PROTECT RECORDING")

            'MILESTONES MESSAGES
            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventKeepFailure, "Failed To Verify SetKeep Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Removing Event Keep From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>312 - SetEventKeepFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub RemoveEventKeep()

        _UI.Utils.StartHideFailures("Removing Event Keep")

        Try
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            Milestones = _UI.Utils.GetValueFromMilestones("SetUnKeep")

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 30)

            _UI.Utils.EPG_Milestones_Navigate("A//V SETTINGS/UNPROTECT RECORDING")

            'MILESTONES MESSAGES

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventKeepFailure, "Failed To Verify SetUnKeep Milestones : " + Milestones))
            End If

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


		 '****changes as per new EPG flow as stop recording from archive shifted to banner ******

            _UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR")
            If Not _UI.Utils.VerifyState("ACTION BAR", 10) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Navigate to ACTION BAR"))
            End If

            _UI.Utils.EPG_Milestones_Navigate("STOP RECORDING")

            _UI.Utils.LogCommentInfo("Verifying State 'CONFIRM RECORDING'")

            If Not _UI.Utils.VerifyState("CONFIRM RECORDING", 3) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify CONFIRMATION"))
            End If

            _UI.Utils.EPG_Milestones_SelectMenuItem("CONFIRM CANCEL RECORDING")

            'MILESTONES MESSAGES

            Milestones = _UI.Utils.GetValueFromMilestones("StopRecording")

            Dim ActualLines As New ArrayList

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 10)

            _UI.Utils.ClearEPGInfo()

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify StopRecording Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Sets Subtitles Language On Action Bar
    ''' </summary>
    ''' <param name="Language">Language Of Subtitles To Select</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetSubtitlesLanguage(ByVal Language As String, Optional ByVal expState As String = "LIVE")
        Dim Milestones As String = ""
        Dim State As String = ""
        Dim ActualLines As New ArrayList

        _UI.Utils.StartHideFailures("Setting Subtitles Language To " + Language + " From Action Bar")

        Try

            _UI.Utils.EPG_Milestones_SelectMenuItem("A//V SETTINGS")

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.VerifyState("A//V SETTINGS") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is A/V SETTINGS"))
            End If

            _UI.Utils.EPG_Milestones_SelectMenuItem("SUBTITLES")

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.VerifyState("SUBTITLES") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is SUBTITLES SELECTION"))
            End If

            _UI.Menu.SetActionBarSubAction(Language)

            Milestones = _UI.Utils.GetValueFromMilestones("SetSubtitlesLanguage")

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 60)

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.VerifyState(expState, 25) Then

                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify State Is " & expState))
            End If

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSubtitlesLanguageFailure, "Failed To Verify SetSubtitlesLanguage Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub AddToFavourites()
        _UI.Utils.StartHideFailures("Adding Channel To Favourites")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("A//V SETTINGS")

            _UI.Utils.SendIR("SELECT")

            _UI.Utils.EPG_Milestones_SelectMenuItem("ADD CHANNEL TO FAVORITES")

            _UI.Utils.SendIR("SELECT")

            _UI.Utils.VerifyState("RATE", 10, 10)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub RemoveFromFavourites()
        _UI.Utils.StartHideFailures("Removing Channel From Favourites")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("A//V SETTINGS")

            _UI.Utils.SendIR("SELECT")

            _UI.Utils.EPG_Milestones_SelectMenuItem("REMOVE CHANNEL FROM FAVORITES")

            _UI.Utils.SendIR("SELECT")

            _UI.Utils.VerifyState("RATE", 10, 10)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
