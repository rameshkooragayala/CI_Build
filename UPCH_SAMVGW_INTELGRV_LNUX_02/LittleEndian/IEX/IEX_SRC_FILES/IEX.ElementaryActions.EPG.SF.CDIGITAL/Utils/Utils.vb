Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Utils

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub

    ''' <summary>
    '''   Return To Live Viewing
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter FALSE.If TRUE Checks For Video</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' </remarks>
    Public Overrides Sub ReturnToLiveViewing(Optional ByVal CheckForVideo As Boolean = False)
        Dim res As IEXGateway._IEXResult
        Dim EpgText As String = ""
        Dim State As String = ""
        Dim Msg As String = ""

        StartHideFailures("Checking If Already In Live")

        Try
            If VerifyState("LIVE", 2) Then
                Msg = "Already On Live"
                Exit Sub
            End If

            Msg = "NOT Already On Live"

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                LogCommentInfo(Msg)
            End If
        End Try

        StartHideFailures("Returning To Live Viewing")

        Try

            StartHideFailures("Returning To Live Viewing")


            _iex.MilestonesEPG.NavigateByName("STATE:LIVE")

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

	''' <summary>
    '''   Enter Or Exist Standby
    ''' </summary>
    ''' <param name="IsOn">If True Exit Standby Else Enter Standby</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub Standby(ByVal IsOn As Boolean)
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        StartHideFailures(IIf(IsOn, "Exiting ", "Entering ") + "Standby")

        Try

            If IsOn Then
                Milestones = GetValueFromMilestones("ExitStandby")
            Else
                Milestones = GetValueFromMilestones("EnterStandBy")
            End If

            BeginWaitForDebugMessages(Milestones, 90)

            SendIR("ON_OFF")

            If Not EndWaitForDebugMessages(Milestones, ActualLines) Then
                SendIR("ON_OFF")
                _iex.Wait(10)
                SendIR("ON_OFF")
            End If
			
			LogCommentInfo("Doing Retour to handle the Smartcard authorisation OSD")
            If (IsOn) Then
                SendIR("RETOUR")
            End If
            _iex.Wait(5)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Sub LoadArchiveAndDeleteAllEvents(ByVal fromPlanner As Boolean)
        _UI.Utils.StartHideFailures("LoadArchiveAndDeleteAllEvents")

        Dim exit_status As Boolean = False
        Dim Temp_EventName As String = ""

        Try
            Dim EventName As String = ""

            '_UI.Utils.SendIR("SELECT")

            While exit_status = False

                If Not _UI.Utils.VerifyState("MY RECORDINGS", 5) Then

                    '************************************* WORKAROUND DONE AS AFTER PLAYBACK WE ARE NOT ABLE TO LIST EVENT****************************************
                    _UI.Utils.ClearEPGInfo()
                    Dim i As Integer = 0

                    If _UI.Utils.VerifyState("LIBRARY ERROR", 2) Or _UI.Utils.VerifyState("LIBRARY", 2) Then
                        For i = 0 To 5
                            'wait is added so that we are at MY RECORDINGS/BY DATE to start again
                            _iex.Wait(3)
                            _UI.Utils.SendIR("SELECT")

                            If Not _UI.Utils.VerifyState("MY RECORDINGS", 5) Then
                                _UI.Utils.LogCommentBlack("LIBRARY ERROR occurred again. Going to Retry")

                                If i = 5 Then
                                    exit_status = True
                                End If
                            Else
                                'Now STB is in MY RECORDINGS screen - Check if any event is present
                                _UI.Utils.GetEpgInfo("evtName", Temp_EventName, CalledFromVerify:=True)

                                If Temp_EventName IsNot String.Empty Then

                                    'Assuming the event is highlighted already
                                    _UI.Utils.SendIR("SELECT")
                                    If fromPlanner Then
                                        _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL")
                                    Else
                                        _UI.Utils.EPG_Milestones_SelectMenuItem("DELETE")
                                    End If
                                    _UI.Utils.SendIR("SELECT")
                                    _iex.Wait(2)
                                    _UI.Utils.EPG_Milestones_SelectMenuItem("YES")
                                    _UI.Utils.SendIR("SELECT")

                                    exit_status = False
                                Else
                                    If i = 5 Then
                                        exit_status = True
                                    End If
                                End If

                                Exit For
                            End If


                        Next

                    End If
                    '**********************************************************************************************************************************************
                Else

                    'Now STB is in MY RECORDINGS screen - Check if any event is present
                    _UI.Utils.GetEpgInfo("evtName", Temp_EventName, CalledFromVerify:=True)

                    If Temp_EventName IsNot String.Empty Then

                        'Assuming the event is highlighted already
                        _UI.Utils.SendIR("SELECT")

                        If fromPlanner Then
                            _UI.Utils.EPG_Milestones_SelectMenuItem("CANCEL")
                        Else
                            _UI.Utils.EPG_Milestones_SelectMenuItem("DELETE")
                        End If
                        _UI.Utils.SendIR("SELECT")
                        _iex.Wait(2)
                        _UI.Utils.EPG_Milestones_SelectMenuItem("YES")
                        _UI.Utils.SendIR("SELECT")

                        exit_status = False
                    Else
                        exit_status = True
                    End If

                End If
            End While
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    ''' Retrieving The Event Time Separator To Be Inserted Between Start Time And End Time  
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetEventTimeSeparator() As String
        Return " - "
    End Function
	
	
    ''' <summary>
    '''  SendChannelAsIRSequence is overriden which will send keys when rety mechanism is called from Live section
    ''' </summary>
    Public Overrides Sub SendChannelAsIRSequence(ByVal ChannelNumber As String, Optional ByVal MsBetweenSending As Integer = 500, Optional ByVal Type As String = "Ignore")
        Dim cmdSeq As String = ""
        Dim res As IEXGateway.IEXResult
        Dim sw As System.Diagnostics.Stopwatch

        StartHideFailures("Sending Channel : " + ChannelNumber + " Waiting " + MsBetweenSending.ToString + " Ms Between IR")

        Try
            Dim digits As Char() = ChannelNumber.ToCharArray
            For i As Integer = 0 To digits.Length - 1

                sw = System.Diagnostics.Stopwatch.StartNew()
                res = _iex.SendIRCommand("KEY_" & digits(i))
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
                sw.Stop()

                _UI.Utils.LogCommentInfo("Time taken by SendIr: " + sw.ElapsedMilliseconds.ToString + "ms")

                'Sleep for remaining milliseconds
                Threading.Thread.Sleep(sw.ElapsedMilliseconds)
            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
End Class


