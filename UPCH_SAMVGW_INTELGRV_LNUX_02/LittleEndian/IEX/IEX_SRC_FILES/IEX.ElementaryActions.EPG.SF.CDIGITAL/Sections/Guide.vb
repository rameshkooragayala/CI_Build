Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Guide

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To Guide
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _UI.Utils.StartHideFailures("Navigating To Guide")

        Try
            _UI.Utils.EPG_Milestones_NavigateByName("STATE:TV GUIDE")

            


        Finally
            _iex.ForceHideFailure()
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
        Dim Res As IEXGateway._IEXResult

        _UI.Utils.StartHideFailures("Selecting Current Event On Guide")

        Try
			_iex.Wait(20)
            Select Case Type
                Case "WithoutPIP"

                    GuideTuneWithoutPIP_Milestones = _UI.Utils.GetValueFromMilestones("GuideTuneToCurrentEventWithoutPIP")
                    _UI.Utils.BeginWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, 15)

                Case "WithPIP", "Predicted"

                    LiveSurfPredicted_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfPredicted")
                    LiveSurfSlowZapping_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfSlowZapping")

                    _UI.Utils.BeginWaitForDebugMessages(LiveSurfPredicted_Milestones, 15)
                    _UI.Utils.BeginWaitForDebugMessages(LiveSurfSlowZapping_Milestones, 15)

                Case "Default"
                    GuideSurfDefault = _UI.Utils.GetValueFromMilestones("GuideSurfDefault")
                    _UI.Utils.BeginWaitForDebugMessages(GuideSurfDefault, 15)

                Case "Ignore"
                    _UI.Utils.LogCommentWarning("Skipping validation as type is Ignore!")
            End Select

            Res = _iex.IR.SendLongIR("SELECT", "", 3000, 2000)
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If

            Select Case Type
                Case "WithoutPIP"
                    If Not _UI.Utils.EndWaitForDebugMessages(GuideTuneWithoutPIP_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideTuneToCurrentEventWithoutPIP Milestones : " + GuideTuneWithoutPIP_Milestones))
                    End If
                Case "WithPIP", "Predicted"
                    If Not _UI.Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify LiveSurfPredicted Milestones : " + LiveSurfPredicted_Milestones))
                    End If

                    If _UI.Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Verified LiveSurfSlowZapping Milestones When Shouldn't : " + LiveSurfSlowZapping_Milestones))
                    End If
                Case "Default"
                    If Not _UI.Utils.EndWaitForDebugMessages(GuideSurfDefault, ActualLines) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.SelectEventFailure, "Failed To Verify GuideSurfDefault Milestones : " + GuideSurfDefault))
                    End If
                Case "Ignore"
            End Select

            _UI.Utils.VerifyLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
    Public Overrides Sub GetEventTimeLeftToStart(ByRef TimeLeft As String)
        Dim StartTime As String = ""
        Dim EPGDateTime As String = ""
        Dim ReturnedEpgTime As String = ""
        Dim Msg As String = ""
        _UI.Utils.StartHideFailures("Getting Event Time Left To Start")
        Try
            _UI.Utils.GetEpgInfo("date", EPGDateTime)
            _UI.Utils.ParseEPGTime(EPGDateTime, ReturnedEpgTime)
            GetSelectedEventStartTime(StartTime)
            TimeLeft = _UI.Utils._DateTime.Subtract(CDate(StartTime), CDate(ReturnedEpgTime))
            If TimeLeft = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Calculate Event Time Left"))
            End If
            Msg = "Time Left To Start : " + TimeLeft.ToString
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub
    Public Overrides Sub GetSelectedEventTime(ByRef EventTime As String)
        Dim Msg As String = ""
        Dim evStartTime As String = ""
        Dim evEndTime As String = ""
        _UI.Utils.StartHideFailures("Getting Selected Event Time From Guide")
        Try
            GetSelectedEventStartTime(evStartTime)
            GetSelectedEventEndTime(evEndTime)
            EventTime = evStartTime + " - " + evEndTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
            _iex.Wait(2)
        End Try
    End Sub
    Overrides Sub GetSelectedEventStartTime(ByRef StartTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""
        Dim EvBothTimes As String()
        _UI.Utils.StartHideFailures("Getting Event Start Time From Guide")
        Try
            _UI.Utils.GetEpgInfo("evttime", EvTime)
            StartTime = Trim(EvTime.Split("-")(0))
            If EvTime.Contains("PM") Then
                EvBothTimes = StartTime.Split(":")
                StartTime = Trim((CInt(EvBothTimes(0)) + 12).ToString + ":" + EvBothTimes(1))
                Msg = "Event Start Time : " + StartTime.ToString
                Exit Sub
            End If
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub
    Overrides Sub GetSelectedEventEndTime(ByRef EndTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""
        Dim EvBothTimes As String()
        _UI.Utils.StartHideFailures("Getting Event Start Time From Guide")
        Try
            _UI.Utils.GetEpgInfo("evttime", EvTime)
            EndTime = Trim(EvTime.Split("-")(1))
            If EvTime.Contains("AM") Then
                EndTime = EndTime.Split("AM")(0)
                Msg = "Event End Time : " + EndTime.ToString
                Exit Sub
            ElseIf EvTime.Contains("PM") Then
                EndTime = EndTime.Split("PM")(0)
                EvBothTimes = EndTime.Split(":")
                EndTime = Trim((CInt(EvBothTimes(0)) + 12).ToString + ":" + EvBothTimes(1))
                Msg = "Event End Time : " + EndTime.ToString
                Exit Sub
            End If
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
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
	
	''' <summary>
    '''   Setting Reminder From Guide
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>311 - SetEventReminderFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetEventReminder()

        _UI.Utils.StartHideFailures("Setting Reminder From Guide")

        Try
            _UI.Banner.SetReminder()

            If Not _UI.Utils.VerifyState("GUIDE", 20) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetEventReminderFailure, "Failed To Verify State Is TV GUIDE"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub	
End Class
