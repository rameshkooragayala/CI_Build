Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Utils

    Dim _UI As IEX.ElementaryActions.EPG.SF.GET.UI

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
		Dim DefaultChannel As String = ""
        Dim ChannelName As String = ""
        Dim BlockedChannel As String = ""
        Dim ChannelNum As String = ""

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

            Try
                BlockedChannel = _UI.Utils.GetValueFromEnvironment("BlockedChannel")
            Catch ex As Exception
                BlockedChannel = "15"
            End Try



            Try
                _UI.Utils.GetEpgInfo("Chname", ChannelName)

                _UI.Utils.GetEpgInfo("Chnum", ChannelNum)


                If ChannelName.Contains("DUMMY") Or ChannelNum.Contains(BlockedChannel) Or ChannelName.Contains("MHSI") Then

                    Try
                        DefaultChannel = _UI.Utils.GetValueFromEnvironment("DefaultChannel")
                    Catch ex As Exception
                        DefaultChannel = "10"
                    End Try


                    _UI.Utils.LogCommentInfo("Tuning to default channel and waiting for tune")
                    _UI.Utils.SendChannelAsIRSequence(DefaultChannel)
                    _iex.Wait(20)
                End If

            Catch ex As Exception
                _UI.Utils.LogCommentWarning("channel number check failed with the following exception - " + ex.Message)
            End Try

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
    '''   Sends Channel As IR Sequence
    ''' </summary>
    ''' <param name="ChannelNumber">The Channel Number</param>
    ''' <param name="MsBetweenSending">Miliseconds To Wait Between IR</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SendChannelAsIRSequence(ByVal ChannelNumber As String, Optional ByVal MsBetweenSending As Integer = 500, Optional ByVal Type As String = "Ignore")
        Dim cmdSeq As String = ""
        Static res As IEXGateway.IEXResult
        Dim ActualLine As String = ""
        Dim status As Boolean
        status = False
        Dim DCA_Miss As Boolean
        DCA_Miss = False
        Dim NumOfTries = 0
        Dim LiveSurfPredicted_Milestones As String = ""
        Dim LiveSurfSlowZapping_Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim LiveSurfNotPredicted_Milestones As String = ""
        Dim Radio_Milestones As String = ""
        Dim LiveSurfDefault As String = ""

        StartHideFailures("Sending Channel : " + ChannelNumber + " Waiting " + MsBetweenSending.ToString + " Ms Between IR Type:" + Type)


        Select Case Type
            Case ""
                _UI.Utils.LogCommentInfo("For Ignore No milestone to be checked")
            Case "Predicted"
                LiveSurfPredicted_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfPredicted")
                LiveSurfSlowZapping_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfSlowZapping")

            Case "Not Predicted"
                LiveSurfNotPredicted_Milestones = _UI.Utils.GetValueFromMilestones("LiveSurfNotPredicted")
            Case "Radio"
                Radio_Milestones = _UI.Utils.GetValueFromMilestones("TuneToRadio")
            Case "Default"
                LiveSurfDefault = _UI.Utils.GetValueFromMilestones("LiveSurfDefault")
        End Select








        Try
            Dim digits As Char() = ChannelNumber.ToCharArray

            For NumOfTries = 1 To 3
                _UI.Utils.LogCommentInfo("Sending DCA" + NumOfTries.ToString + "-Time")

                For i As Integer = 0 To digits.Length - 1
                    res = _iex.Debug.BeginWaitForMessage("IR key", 0, 3, IEXGateway.DebugDevice.Udp)
                    If Not res.CommandSucceeded Then
                        _UI.Utils.LogCommentFail("Failed To Begin Wait For Message")
                        ExceptionUtils.ThrowEx(New IEXException(res))
                    End If

                    _iex.SendIRCommand("KEY_" + digits(i))

                    res = _iex.Debug.EndWaitForMessage("IR key", ActualLine, "", IEXGateway.DebugDevice.Udp)
                    If Not res.CommandSucceeded Then
                        DCA_Miss = True
                        status = False
                        Exit For
                    Else
                        status = True
                        DCA_Miss = False
                    End If

                Next

                If DCA_Miss Then
                    _UI.Utils.LogCommentInfo("DCA Failed -Waiting for default tune before resending DCA")
                    _iex.Wait(20)

                    If Type.Equals("Predicted") Then
                        _UI.Utils.EndWaitForDebugMessages(LiveSurfPredicted_Milestones, ActualLines)
                        _UI.Utils.EndWaitForDebugMessages(LiveSurfSlowZapping_Milestones, ActualLines)
                    ElseIf Type.Equals("Not Predicted") Then
                        _UI.Utils.EndWaitForDebugMessages(LiveSurfNotPredicted_Milestones, ActualLines)
                    ElseIf Type.Equals("Default") Then
                        _UI.Utils.EndWaitForDebugMessages(LiveSurfDefault, ActualLines)
                    Else
                        _UI.Utils.LogCommentInfo("For Ignore No milestone to be checked")
                    End If

                    _UI.Live.SetTuneToChannelVerification(Type)


                Else

                    Exit For

                End If
            Next

            If status Then

                _UI.Utils.LogCommentInfo("DCA successful")
            Else

                _UI.Utils.LogCommentFail("Failed to do DCA after 3 tries")
                ExceptionUtils.ThrowEx(New IEXException(res))

            End If



        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Entering PIN
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub EnterPin(ByVal PIN As String)
        Dim State As String = ""
        Dim EpgText As String = ""

        Dim _PIN As String = ""

        If PIN = "" Then

            _PIN = GetValueFromEnvironment("DefaultPIN")

            If _PIN.Length <> 4 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "PIN Length Was Less Than 4 Digits Please Check DefaultPIN In Your Environment.ini"))
            End If
        Else
            _PIN = PIN
        End If

        StartHideFailures("Entering PIN : " + _PIN)

        Try
            TypeKeys(_PIN)
            _iex.Wait(1)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
	
    ''' <summary>
    '''    Retrieving The Event Time Separator To Be Inserted Between Start Time And End Time  
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetEventTimeSeparator() As String
        Return "-"
    End Function
End Class


