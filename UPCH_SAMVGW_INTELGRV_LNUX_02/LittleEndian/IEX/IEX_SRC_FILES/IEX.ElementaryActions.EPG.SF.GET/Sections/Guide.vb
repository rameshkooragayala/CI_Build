Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Guide

    Dim _UI As IEX.ElementaryActions.EPG.SF.GET.UI
    Dim _utils As IEX.ElementaryActions.EPG.SF.GET.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _utils = UI.Utils
    End Sub




    ''' <summary>
    '''   Moving UP/DOWN On Guide
    ''' </summary>
    ''' <param name="IsUp">If True Moves UP Else Moves Down</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub MoveChannelUpDown(ByVal IsUp As Boolean)
        Dim Direction As String = ""
        Dim CurChannel As String = ""
        Dim CheckedChannel As String = ""

        Try
            If IsUp Then
                _Utils.StartHideFailures("Moving Up On Guide")
                Direction = "UP"
            Else
                _Utils.StartHideFailures("Moving Down On Guide")
                Direction = "DOWN"
            End If

            GetSelectedChannelNumber(CurChannel)
            _utils.ClearEPGInfo()

            _UI.Utils.LogCommentWarning("Adding delay as PIP is slow")
            _iex.Wait(5)

            _Utils.SendIR("SELECT_" + Direction, 8000)

            GetSelectedChannelNumber(CheckedChannel)

            If CurChannel = CheckedChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Move " + Direction.ToString + " On Guide Channel Number Is The Same As Previous"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Surfing Up On Guide
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Surf Channel Up With PIP (UPC), "WithoutPIP" For Surf Channel Up Without PIP</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelUp(Optional ByVal Type As String = "")
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim CurEventChannel As String = ""
        Dim CheckedEventChannel As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Surfing Up On Guide")

        Try
            Milestones = GetMilestoneToValidate(Type)

            If Milestones <> "" Then
                _Utils.BeginWaitForDebugMessages(Milestones, 15)
            End If

            GetSelectedChannelNumber(CurEventChannel)

            'Fetch the channel surf up key
            'Fetch the channel surf up key
            Dim chSurfUpKey As String = GetSurfKey("UP")
            'Fetch the channel surf timeout
            Dim chSurfTimeoutInMSec As String = GetChannelSurfTimeout()

            _UI.Utils.LogCommentWarning("Adding delay as PIP is slow")
            _iex.Wait(5)

            _Utils.SendIR(chSurfUpKey, chSurfTimeoutInMSec)

            GetSelectedChannelNumber(CheckedEventChannel)

            If CurEventChannel = CheckedEventChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Surf Up On Guide CheckedEventChannel = " + CheckedEventChannel.ToString + " CurEventChannel = " + CurEventChannel.ToString))
            End If

            If Milestones <> "" Then
                If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify " + Type + " Milestones : " + Milestones))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Surfing Down On Guide
    ''' </summary>
    ''' <param name="Type">Can Be : "" For Surf Channel Down With PIP (UPC), "WithoutPIP" For Surf Channel Down Without PIP</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelDown(Optional ByVal Type As String = "")
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim CurEventChannel As String = ""
        Dim CheckedEventChannel As String = ""
        Dim Msg As String = ""

        _Utils.StartHideFailures("Surfing Down On Guide")

        Try
            Milestones = GetMilestoneToValidate(Type)

            If Milestones <> "" Then
                _Utils.BeginWaitForDebugMessages(Milestones, 15)
            End If

            GetSelectedChannelNumber(CurEventChannel)

            'Fetch the channel surf up key
            Dim chSurfDownKey As String = GetSurfKey("DOWN")
            'Fetch the channel surf timeout
            Dim chSurfTimeoutInMSec As Integer = GetChannelSurfTimeout()
            _UI.Utils.LogCommentWarning("Adding delay as PIP is slow")
            _iex.Wait(5)
            'Send the key
            _Utils.SendIR(chSurfDownKey, chSurfTimeoutInMSec)

            GetSelectedChannelNumber(CheckedEventChannel)

            If CurEventChannel = CheckedEventChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Surf Down On Guide CheckedEventChannel = " + CheckedEventChannel.ToString + " CurEventChannel = " + CurEventChannel.ToString))
            End If

            If Milestones <> "" Then
                If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Verify " + Type + " Milestones : " + Milestones))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Moving To Next Event X Times
    ''' </summary>
    ''' <param name="NumOfPresses">Optional Parameter Default = 1 : X Events To Move On Guide</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextEvent(Optional ByVal NumOfPresses As Integer = 1)
        'Must Be EventName Since No Time In No Information Availble
        Dim CurEventTime As String = ""
        Dim CheckedEventTime As String = ""

        _Utils.StartHideFailures("Navigating To NextEvent Event " + NumOfPresses.ToString + " Times")

        Try
            For RepeatIR As Integer = 1 To NumOfPresses

                GetSelectedEventTime(CurEventTime)
                _UI.Utils.LogCommentWarning("Adding delay as PIP is slow")
                _iex.Wait(5)
                _Utils.SendIR("SELECT_RIGHT", 10000)

                GetSelectedEventTime(CheckedEventTime)

                If CurEventTime = CheckedEventTime Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On Guide CheckedEventTime = " + CheckedEventTime.ToString + " CurEventTime = " + CurEventTime.ToString))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''    Gets Selected Event Time
    ''' </summary>
    ''' <param name="EventTime">ByRef Event Time As String</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedEventTime(ByRef EventTime As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Selected Event Time From Guide")

        Try
            _Utils.GetEpgInfo("evtTime", EventTime)
            Msg = "Selected Event Time : " + EventTime
            EventTime = Replace(EventTime, " ", "")
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
            _iex.Wait(2)
        End Try
    End Sub

    ''' <summary>
    '''   Selecting Current Event From Guide And Checking Tunning Fas Milestones
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectCurrentEvent(Optional ByVal Type As String = "")
        Dim LiveSurfPredicted_Milestones As String = ""
        Dim LiveSurfSlowZapping_Milestones As String = ""
        Dim GuideTuneWithoutPIP_Milestones As String = ""
        Dim GuideSurfDefault As String = ""
        Dim ActualLines As New ArrayList
        Dim ReturnedValue As String = ""

        _Utils.StartHideFailures("Selecting Current Event On Guide")

        Try
            Select Case Type
                Case "WithoutPIP"

                    GuideTuneWithoutPIP_Milestones = _Utils.GetValueFromMilestones("GuideTuneToCurrentEventWithoutPIP")
                    _Utils.BeginWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, 15)

                Case "WithPIP"

                    LiveSurfPredicted_Milestones = _Utils.GetValueFromMilestones("LiveSurfPredicted")
                    LiveSurfSlowZapping_Milestones = _Utils.GetValueFromMilestones("LiveSurfSlowZapping")

                    _Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                    _Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)

                Case "Default"

                    GuideSurfDefault = _Utils.GetValueFromMilestones("GuideSurfDefault")
                    _Utils.BeginWaitForDebugMessages(GuideSurfDefault, 15)

                Case "Ignore"

            End Select

            'Res = _iex.IR.SendLongIR("SELECT", "", 3000, 2000)
            _UI.Utils.SendIR("SELECT")
            'If Not Res.CommandSucceeded Then
            '    ExceptionUtils.ThrowEx(New IEXException(Res))
            'End If


            _utils.VerifyState("ACTION BAR")
            _UI.Utils.LogCommentInfo("Waiting for predicted zap to happen")
            _iex.Wait(2)
            _utils.EPG_Milestones_Navigate("WATCH")
            Select Case Type
                Case "WithoutPIP"
                    If Not _Utils.EndWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideTuneToCurrentEventWithoutPIP Milestones : " + GuideTuneWithoutPIP_Milestones))
                    End If
                Case "WithPIP"
                    If Not _Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
                    End If

                    If _Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Verified LiveSurfSlowZapping Milestones When Shouldn't : " + LiveSurfSlowZapping_Milestones))
                    End If

                Case "Default"
                    If Not _Utils.EndWaitForDebugMessages(GuideSurfDefault, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideSurfDefault Milestones : " + GuideSurfDefault))
                    End If
                Case "Ignore"

            End Select

            _Utils.VerifyLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
	
	    ''' <summary>
    '''   Typing Channel Number Keys And Checking Channel Surf Milestones
    ''' </summary>
    ''' <param name="ChannelNumber">The Requested Channel To Type</param>
    ''' <param name="VerifyFas">Optional Default = True : If True Verify ChannelSurf FAS Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub TypeKeys(ByVal ChannelNumber As String, Optional ByVal VerifyFas As Boolean = True)
        Dim ActualLines As New ArrayList
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Entering " + ChannelNumber.ToString)

        Try
            If VerifyFas Then
                'MILESTONES MESSAGES
                Milestones = _Utils.GetValueFromMilestones("ChannelSurf")

                _Utils.BeginWaitForDebugMessages(Milestones, 20)
            End If

			_UI.Utils.LogCommentInfo("Waiting before doing DCA")
            _iex.Wait(5)
			
            _Utils.SendChannelAsIRSequence(ChannelNumber, 500)

            If Not _Utils.VerifyDebugMessage("dca_number", ChannelNumber, 10, 10) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify Grid Is On Channel : " + ChannelNumber.ToString))
            End If
            If VerifyFas Then
                If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify ChannelSurf Milestones : " + Milestones))
                End If
            Else
                _UI.Live.VerifyChannelNumber(ChannelNumber)
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

''' <summary>
    '''   Navigating To RECORD On Action Bar From Guide By Pressing RED/SELECT
    ''' </summary>
    ''' <param name="IsCurrent">Optional Parameter Default = True : If True Pressing RED Else Pressing Select For Future Events</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    ''' 
    Public Overrides Sub NavigateToRecordEvent(Optional ByVal IsCurrent As Boolean = True)
        Dim EpgText As String = ""
        _UI.Utils.StartHideFailures("Navigating To RECORD On Guide")
        Try
            If IsCurrent Then
                SelectEvent()
                _UI.Banner.PreRecordEvent()
                _UI.Utils.StartHideFailures("Checking If PIN Is Requested")
                If _UI.Utils.VerifyDebugMessage("state", "NewPinState", 5, 1) Then
                    _UI.Utils.EnterPin("")
                    _UI.Utils.EPG_Milestones_SelectMenuItem("RECORD")
                End If
                _iex.ForceHideFailure()

                EpgText = _UI.Utils.GetValueFromDictionary("DIC_ACTION_LIST_RECORD_SERIE_EVENT")
                _UI.Utils.StartHideFailures("Trying To Navigate To " + EpgText)
                Try
                    Try
                        _UI.Menu.SetActionBarSubAction(EpgText)
                    Catch ex As EAException
                        _UI.Utils.LogCommentFail("Failed To Set " + EpgText + " On Action Bar")
                    End Try
                Finally
                    _iex.ForceHideFailure()
                End Try
            Else
                SelectEvent()
                _UI.Banner.PreRecordEvent()
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


End Class
