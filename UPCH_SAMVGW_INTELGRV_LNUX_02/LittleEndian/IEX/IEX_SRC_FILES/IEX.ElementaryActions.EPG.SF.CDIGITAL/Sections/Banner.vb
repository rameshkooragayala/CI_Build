Imports FailuresHandler
Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Banner

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI


    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    Public Overrides Sub CancelBooking(Optional ByVal IsSeries As Boolean = False, Optional ByVal IsComplete As Boolean = False)
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
            Milestones = _UI.Utils.GetValueFromMilestones("CancelBooking")
            _UI.Utils.BeginWaitForDebugMessages(Milestones, 15)
            _UI.Utils.SendIR("SELECT")
            If _UI.Utils.VerifyState("CONFIRM RECORDING", 5) Then
                _UI.Utils.SendIR("SELECT")
            End If
            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify CancelBooking Milestones : " + Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''    Getting Event Start Time From Action Bar
    ''' </summary>
    ''' <param name="StartTime">Returns Start Time From Action Bar</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventStartTime(ByRef StartTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""
        Dim EvBothTimes As String()
        _UI.Utils.StartHideFailures("Getting Event Start Time From Action Bar")
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

    ''' <summary>
    '''   Getting Event End Time From Action Bar
    ''' </summary>
    ''' <param name="ReturnedEndTime">Returns End Time From Action Bar</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventEndTime(ByRef ReturnedEndTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""
        Dim EvBothTimes As String()
        _UI.Utils.StartHideFailures("Getting Event End Time From Action Bar")
        Try
            _UI.Utils.GetEpgInfo("evttime", EvTime)
            ReturnedEndTime = Trim(EvTime.Split("-")(1))
            If EvTime.Contains("AM") Then
                ReturnedEndTime = ReturnedEndTime.Split("AM")(0)
                Msg = "Event End Time : " + ReturnedEndTime.ToString
                Exit Sub
            ElseIf EvTime.Contains("PM") Then
                ReturnedEndTime = ReturnedEndTime.Split("PM")(0)
                EvBothTimes = ReturnedEndTime.Split(":")
                ReturnedEndTime = Trim((CInt(EvBothTimes(0)) + 12).ToString + ":" + EvBothTimes(1))
                Msg = "Event End Time : " + ReturnedEndTime.ToString
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
    '''   Preparing Recording Event From Action Bar - Navigating To Confirm Record Without Confirming The Record
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>323 - VerifyStateFailure</para> 
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
    '''   Pausing Event From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub PauseEvent()

        _UI.Utils.StartHideFailures("Pausing Event From Action Bar")

        Try

            _UI.Utils.EPG_Milestones_SelectMenuItem("TRICK MODE ICON")

            Dim Milestones As String = ""

            Milestones = _UI.Utils.GetValueFromMilestones("TrickModeSpeed")
            Milestones += "0"

            _UI.Utils.VerifyFas("SELECT", Milestones, 10, False)

            _iex.Wait(2)

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
    ''' <summary>
    '''   Navigating To Action Bar
    ''' </summary>
    ''' <remarks></remarks>
    Overrides Sub Navigate(Optional ByVal FromPlayback As Boolean = False)

        _UI.Utils.StartHideFailures("Navigating To Action Bar")
        Try

            If IsActionBar() Then
                Exit Sub
            End If

            If FromPlayback Then
                Try
                    _UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION_BAR_PLAYBACK")
                Catch ex As Exception
                    _UI.Utils.LogCommentWarning("WorkAround put for retry of path")
                    _UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION_BAR_PLAYBACK")
                End Try
            Else
                _UI.Utils.EPG_Milestones_Navigate("MAIN MENU/LIVE/ACTION BAR")
            End If


        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class
