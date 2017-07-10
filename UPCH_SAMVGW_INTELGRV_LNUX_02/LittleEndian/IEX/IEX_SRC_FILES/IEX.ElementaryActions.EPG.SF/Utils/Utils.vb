Imports System.Threading.Tasks
Imports FailuresHandler
Imports System.ServiceProcess
Imports System.IO
Imports System.Text
Imports System.Xml

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.Utils

    Dim _UI As EPG.UI
    Private FullPALAreaCords As String = "99 1 676 569"
    Private FullNTSCAreaCords As String = "1 1 645 480"
    Private PartialPALAreaCords As String = "5 12 766 168"
    Private PartialNTSCAreaCords As String = "5 12 600 450"
    Public WaitAfterIRFactor As Integer = 1
    Public BeginWaitForMessageFactor As Integer = 1
    Protected ExpectedTimeLengthWithoutSeparator = 10 'Start Time Length + End Time Length, For Example 09:00 (5) 10:00 (5) => 5+5 = 10
    Private Const PathToIexPrefix As String = "C:\Program Files\IEX\IEX_"
    Public EPGStateMachine As StateMachine

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        MyBase.New(_pIex, pUI)
        EPGStateMachine = New StateMachine(Me)
        _UI = pUI
    End Sub

    Public ReadOnly Property VideoCords(Optional ByVal IsFullArea As Boolean = True, Optional ByVal Area As Integer = 1)
        Get
            Dim cords As String = GetVideoCords(IsFullArea)

            If IsFullArea Then
                Select Case Area
                    Case 6 'For MOBILE
                        Return "147 39 626 526"
                    Case Else
                        Return cords
                End Select
            Else
                Select Case Area
                    Case 1 'For CDIGITAL, COGECO And Default
                        Return cords
                    Case 2
                        Return "17 13 757 569"
                    Case 3
                        Return "123 137 630 289"
                    Case 4
                        Return "181 7 646 320"
                    Case 5
                        Return "164 52 760 220"
                    Case 6 'For MOBILE
                        Return "263 62 626 166"
                End Select
                Return cords
            End If
        End Get
    End Property

    ''' <summary>
    '''   Gets Value From Build Dictionary
    ''' </summary>
    ''' <param name="isFull">True if full area is to be checked, false if partial area to be checked</param>
    ''' <returns>String</returns>
    ''' <remarks>
    '''   Following values are to be added to your project configuration file unless the default value is ok -
    '''   [LIVE]
    '''   FULL_VIDEO_CORDS_FOR_PAL
    '''   FULL_VIDEO_CORDS_FOR_NTSC
    '''   PARTIAL_VIDEO_CORDS_FOR_PAL
    '''   PARTIAL_VIDEO_CORDS_FOR_NTSC
    ''' </remarks>
    Private Function GetVideoCords(ByVal isFull As Boolean) As String

        Dim videoCords As String = ""
        Dim prefixString As String = ""
        Dim videoStandard As String = ""

        If isFull Then
            prefixString = "FULL"
        Else
            prefixString = "PARTIAL"
        End If

        Try
            videoStandard = GetValueFromEnvironment("VideoFormat").ToString.ToUpper
            videoCords = GetValueFromProject("LIVE", prefixString + "_VIDEO_CORDS_FOR_" + videoStandard)
        Catch ex As Exception
            LogCommentWarning("Failed to get the value from Project ini - " + prefixString + "_VIDEO_CORDS_FOR_" + videoStandard + " . Fetching default value instead!!")
            If isFull Then
                Select Case videoStandard
                    Case "NTSC"
                        videoCords = FullNTSCAreaCords
                    Case "PAL"
                        videoCords = FullPALAreaCords
                End Select
            Else
                Select Case videoStandard
                    Case "NTSC"
                        videoCords = PartialNTSCAreaCords
                    Case "PAL"
                        videoCords = PartialPALAreaCords
                End Select
            End If
        End Try

        Return videoCords

    End Function

    ''' <summary>
    '''   Gets Value From Build Dictionary
    ''' </summary>
    ''' <param name="Key">The Key Of The Value Requested</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' </remarks>
    Public Function GetValueFromDictionary(ByVal Key As String) As String
        Dim ReturnedValue As String = ""

        If DirectCast(_UI.Utils.StaticParam("Dictionary"), Dictionary(Of String, String)).Count <= 0 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Problem With Dictionary ! Dictionary Did Not Load To EA"))
        End If

        Try
            ReturnedValue = DirectCast(_UI.Utils.StaticParam("Dictionary"), Dictionary(Of String, String))(Key)
            LogCommentInfo("Got " + ReturnedValue + " For Key " + Key + " From Dictionary")
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Key " + Key + " Was Not Found In Loaded Dictionary"))
        End Try

        If ReturnedValue = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Key " + Key + " Value Is Empty ! Check Dictionary"))
        End If

        Return ReturnedValue
    End Function

    ''' <summary>
    '''   Gets Value From Environment.ini
    ''' </summary>
    ''' <param name="Key">The Key Of The Value Requested</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function GetValueFromEnvironment(ByVal Key As String) As String
        Dim ReturnedValue As String = ""

        If DirectCast(_UI.Utils.StaticParam("Environment"), Dictionary(Of String, String)).Count <= 0 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Problem With Environment.ini ! Environment.ini Did Not Load To EA"))
        End If

        Try
            ReturnedValue = DirectCast(_UI.Utils.StaticParam("Environment"), Dictionary(Of String, String))(Key)
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Key " + Key + " Was Not Found In Loaded Environment.ini"))
        End Try

        If ReturnedValue = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Key " + Key + " Value Is Empty ! Check Environment.ini"))
        End If

        Return ReturnedValue
    End Function

    ''' <summary>
    '''   Gets Value From Milestones.ini
    ''' </summary>
    ''' <param name="Key">The Key Of The Value Requested</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function GetValueFromMilestones(ByVal Key As String) As String
        Dim ReturnedValue As String = ""

        If DirectCast(_UI.Utils.StaticParam("Milestones"), Dictionary(Of String, String)).Count <= 0 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Problem With Environment.ini ! Milestones.ini Did Not Load To EA"))
        End If

        Try
            ReturnedValue = DirectCast(_UI.Utils.StaticParam("Milestones"), Dictionary(Of String, String))(Key)
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Key " + Key + " Was Not Found In Loaded Milestones.ini"))
        End Try

        If ReturnedValue = "" Then
            LogCommentWarning("WARNING : Value Is Empty ! For Key " + Key + " Check Milestones.ini")
            ' ExceptionUtils.ThrowEx( New EAException(ExitCodes.DictionaryFailure, "Key " + Key + " Value Is Empty ! Check Milestones.ini"))
        End If

        Return ReturnedValue
    End Function

    ''' <summary>
    '''   Gets Value From Project.ini
    ''' </summary>
    ''' <param name="Key">The Key Of The Value Requested</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function GetValueFromProject(ByVal section As String, ByVal key As String) As String

        Dim returnedValue As String = ""
        Dim _iexNumber As String = _iex.IEXServerNumber

        Try
            Dim projectIni As New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Project.ini")
            returnedValue = projectIni.GetValue(section, key)
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed to get value from section " + section + " and for key " + key + " Project.ini with the following Exception : " + ex.Message))
        End Try

        If returnedValue = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Value is empty for key " + key + " in section " + section + ".Please check your Project.ini"))
        End If

        Return returnedValue

    End Function
  ''' <summary>
    '''     Gets Value From Browser.ini
    ''' </summary>
    ''' <param name="Key">The Key Of The Value Requested</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function GetValueFromBrowser(ByVal section As String, ByVal key As String) As String

        Dim returnedValue As String = ""
        Dim _iexNumber As String = _iex.IEXServerNumber

        Try
            Dim BrowserIni As New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Browser.ini")
            returnedValue = BrowserIni.GetValue(section, key)
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed to get value from section " + section + " and for key " + key + " Project.ini with the following Exception : " + ex.Message))
        End Try

        If returnedValue = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Value is empty for key " + key + " in section " + section + ".Please check your Project.ini"))
        End If

        Return returnedValue

    End Function
    ''' <summary>
    '''   Recording Event From different screens(Action Bar, Live, Guide and Channel Bar) using Hot key (REC key)
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>323 - VerifyStateFailure</para> 
    ''' </remarks>
    Public Overrides Sub PreRecordRECkey(ByVal isCurrent As Boolean, ByVal isConflict As Boolean, Optional ByVal IsSeries As Boolean = False)

        StartHideFailures("Preparing Recording Event From Action Bar")

        Try
            SendIR("REC")

            If Not VerifyState("CONFIRM RECORDING") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify State Is CONFIRM RECORDING"))
            End If

            If IsSeries Then
                Try
                    EPG_Milestones_SelectMenuItem("RECORD ALL EPISODES")
                Catch ex As Exception
				    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigateFailure, "Failed To select RECORD ALL EPISODES"))                   
                End Try
            Else
                Try
                    EPG_Milestones_SelectMenuItem("CONFIRM RECORDING")
                Catch ex As Exception
                    EPG_Milestones_SelectMenuItem("RECORD THIS EPISODE")
                End Try
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Recording Event From different screens(Action Bar, Live, Guide and Channel Bar) using Hot key (REC key)
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>323 - VerifyStateFailure</para> 
    ''' </remarks>
    Public Overrides Sub StopRecordingSTOPkey(Optional ByVal IsSeries As Boolean = False, Optional ByVal IsStopRecording As Boolean = True, Optional ByVal IsCurrent As Boolean = True, Optional ByVal IsTBR As Boolean = False)

        Dim EpgText As String = ""
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        StartHideFailures("Stop Recording Event Using STOP key")
        Try
            LogCommentInfo("StopRecording : Stopping Recording Event Using STOP key")

            SendIR("STOP")

            If Not VerifyState("CONFIRM DELETE") Then
                If Not VerifyState("CONFIRM RECORDING") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Verify State Is CONFIRM DELETE"))
                End If
            End If


            If IsStopRecording Then

                If IsSeries Then
                    EPG_Milestones_SelectMenuItem("RECORD ALL EPISODES")
                    Milestones = GetValueFromMilestones("CancelBooking")

                Else
                    'MILESTONES MESSAGES
                    If Not IsCurrent And Not IsTBR Then
                        Milestones = GetValueFromMilestones("CancelBooking")
                    Else
                        Milestones = GetValueFromMilestones("StopRecording")
                    End If
                    Try
                        EPG_Milestones_SelectMenuItem("YES")
                    Catch
                        Try
                            EPG_Milestones_SelectMenuItem("RECORD THIS EPISODE")
                        Catch
                            Try
                                EPG_Milestones_SelectMenuItem("CANCEL RECORDING")
                            Catch
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To highlight YES/RECORD THIS EPISODE"))
                            End Try
                        End Try
                    End Try
                End If
            Else
                Try
                    EPG_Milestones_SelectMenuItem("NO")
                Catch
                    Try
                        EPG_Milestones_SelectMenuItem("CANCEL RECORDING")
                    Catch
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To highlight NO/CANCEL RECORDING"))
                    End Try
                End Try
                Try
                    SendIR("SELECT")
                Finally
                    _iex.ForceHideFailure()
                End Try
                Exit Sub
            End If

            BeginWaitForDebugMessages(Milestones, 30)

            ClearEPGInfo()

            SendIR("SELECT")

            If Not EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopRecordEventFailure, "Failed To Verify StopRecording Milestones : " + Milestones))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checking If STB Crashed By Pressing Menu And BACK_UP And Checking Correct Behaviour
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsSTBCrash() As Boolean

        Dim State As String = ""

        _UI.Menu.Navigate()

        Try
            _UI.Utils.ReturnToLiveViewing(False)
        Catch ex As Exception
            _iex.GetSnapshot("Utils.IsSTBCrash : For Debugging...")
            _iex.ForceShowFailure()
            LogCommentFail("Utils.IsSTBCrash : Failed To Return To Live")
            Return True
        End Try

        Return False

    End Function

    ''' <summary>
    '''   Entering/Existing Standby
    ''' </summary>
    ''' <param name="IsOn">If True Existing Else Entering</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>358 - StandByFailure</para> 
    ''' </remarks>
    Public Overrides Sub Standby(ByVal IsOn As Boolean)
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim IsSupported As String = ""

        Try
            IsSupported = GetValueFromProject("STANDBY", "SUPPORTED")
        Catch ex As Exception
            LogCommentWarning("Value for STANDBY SUPPORTED is not defined in project file, so by default it is considered as True")
            IsSupported = True
        End Try

        If (Convert.ToBoolean(IsSupported)) Then

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
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.StandByFailure, "Failed To Verify StandBy Milestones : " + Milestones))
                End If
                _iex.Wait(5)

            Finally
                _iex.ForceHideFailure()
            End Try
        Else
            LogCommentWarning("StandBy Feature is not supported")
        End If
    End Sub

    ''' <summary>
    '''   Checking Review Buffer Max Depth Milestones To See If STB Ready After Standby
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>363 - STBReadyFailure</para> 
    ''' </remarks>
    Public Overrides Sub STBReady()
        'MILESTONES MESSAGES
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        StartHideFailures("Verifying STB Ready")

        Try
            Milestones = GetValueFromMilestones("ReviewBufferMaxDepth")

            BeginWaitForDebugMessages(Milestones, 20)

            If Not EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.STBReadyFailure, "Failed To Verify StandBy Milestones : " + Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Exit To Live By Pressing BACK_UP 1 Time And Verify Nothing Is On The Screen
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub ExitToLive()

        StartHideFailures("Exiting To Live")

        Try
            Dim State As String = ""

            SendIR("BACK_UP")

            If Not VerifyState("LIVE", 25) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify LIVE State Entered"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Return To Playback Viewing
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter FALSE.If TRUE Checks For Video</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>334 - VideoNotPresent</para> 
    ''' </remarks>
    Public Overrides Sub ReturnToPlaybackViewing(Optional ByVal CheckForVideo As Boolean = False)
        Dim res As IEXGateway._IEXResult
        Dim EpgText As String = ""
        Dim State As String = ""
        Dim Msg As String = ""

        StartHideFailures("Checking If Already In PLayback")

        Try
            If VerifyState("PLAYBACK", 5) Then
                Msg = "Already On Playback"
                Exit Sub
            End If

            Msg = "NOT Already On Playback"

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                LogCommentInfo(Msg)
            End If
        End Try

        StartHideFailures("Returning To Playback Viewing")
        Try

            EPG_Milestones_Navigate("MAIN MENU/PLAYBACK")

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

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Return To Live Viewing
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter FALSE.If TRUE Checks For Video</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Overrides Sub ReturnToLiveViewing(Optional ByVal CheckForVideo As Boolean = False)
        Dim res As IEXGateway._IEXResult
        Dim EpgText As String = ""
        Dim State As String = ""
        Dim Msg As String = ""

        StartHideFailures("Checking If Already In Live")

        Try
            If VerifyState("LIVE", 5) Then
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

            EpgText = GetValueFromDictionary("DIC_MAIN_MENU_ON_NOW")

            _UI.Menu.Navigate()

            res = _iex.MilestonesEPG.SelectMenuItem(EpgText)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(res))
            End If

            Dim ReturnedValue As String = ""

            _iex.IR.SendIR("SELECT", "", 4000)

            Dim LockedChannel As String = ""
            Dim LockedProgram As String = ""
            Dim Unsubscribed As String = ""


            LockedChannel = GetValueFromDictionary("DIC_LOCKED_CHANNEL")
            LockedProgram = GetValueFromDictionary("DIC_LOCKED_PROGRAM")
            Unsubscribed = GetValueFromDictionary("DIC_UNSUBSCRIBED_CHANNEL")

            Dim EventName As String = ""


              Try
                GetEpgInfo("evtName", EventName)
            Catch ex As Exception

            End Try

            If EventName <> LockedChannel And EventName <> LockedProgram And EventName <> Unsubscribed Then

                If Not VerifyState("LIVE", 30) Then
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
            Else
                If CheckForVideo Then
                    LogCommentWarning("WARNING : Video Was Not Checked Since The Content Is Locked")
                End If
            End If

            _UI.Live.WaitAfterLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Convert And Enter Any String To IEX Keys On Manual Recording
    ''' </summary>
    ''' <param name="StringToType">The String To Enter</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub TypeManualRecordingKeys(ByVal StringToType As String)

        StartHideFailures("Typing Manual Recording Keys " + StringToType.ToString)

        Try
            TypeKeys(StringToType)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Getting All EPG Texts From Dictionary
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' </remarks>
    Public Overridable Function GetTextsFromDictionary() As Boolean

        Dim _iexNumber As String = _iex.IEXServerNumber

        Dim Dictionary As New XML("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber, _iex, _UI)

        Dictionary.LoadDictionary()

        Return True
    End Function

    ''' <summary>
    '''   Getting All Milestones From Milestones.ini
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overridable Function GetMilestonesFromIni() As Boolean
        Dim _iexNumber As String = _iex.IEXServerNumber

        StartHideFailures("Loading MileStones...")
        Try
            Dim Milestones As New Dictionary(Of String, String)

            Dim a As New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Milestones.ini")
            Dim Entries As String() = a.GetEntryNames("MILESTONES")
            If Entries.Length > 0 Then
                LogCommentImportant(Entries.Length & " Milestones Were Found In C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Milestones.ini")
            End If

            For Each ent As String In Entries
                Milestones.Add(ent.ToString, a.GetValue("MILESTONES", ent.ToString).ToString)
            Next

            _UI.Utils.StaticParam("Milestones") = Milestones

            _iex.ForceHideFailure()
            LogCommentInfo("Milestones Were Loaded Succesfuly")

        Catch ex As Exception
            _iex.ForceHideFailure()
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed To Fill All Milestones From Milestones.ini Exception : " + ex.Message))
        End Try

        Return True
    End Function

    ''' <summary>
    '''   Getting All Values From Environment.ini
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overridable Function GetEnvironmentIni() As Boolean
        Dim _iexNumber As String = _iex.IEXServerNumber

        StartHideFailures("Loading Environment.ini ...")

        LogCommentInfo("GetEnvironmentIni : Loading Environment.ini ...")

        Try
            Dim Environment As New Dictionary(Of String, String)

            Dim iniFile As New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Environment.ini")
            Dim Entries As String() = iniFile.GetEntryNames("IEX" + _iexNumber)
            If Entries.Length > 0 Then
                LogCommentImportant(Entries.Length & " Keys Were Found In C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Environment.ini")
            End If

            For Each ent As String In Entries
                If ent.StartsWith("#") = False Then
                    LogCommentInfo("Adding Key : " + ent.ToString + " Value : " + iniFile.GetValue("IEX" + _iexNumber, ent.ToString).ToString)
                    Environment.Add(ent.ToString, iniFile.GetValue("IEX" + _iexNumber, ent.ToString).ToString)
                End If
            Next

            _UI.Utils.StaticParam("Environment") = Environment

            _iex.ForceHideFailure()
            LogCommentInfo("Environment.ini Was Loaded Succesfuly")

        Catch ex As Exception
            _iex.ForceHideFailure()
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Failed To Fill All Values From Environment.ini Exception: " + ex.Message))
        End Try

        Return True
    End Function

    ''' <summary>
    '''   Sends IR Key
    ''' </summary>
    ''' <param name="IRKey">The Key To Send</param>
    ''' <param name="WaitAfterIR">Optional Parameter Default 2000 : Wait After Sending</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SendIR(ByVal IRKey As String, Optional ByVal WaitAfterIR As Integer = 2000)
        Dim res As New IEXGateway.IEXResult
        Dim ActualLines As New ArrayList

        StartHideFailures("Sending IR : " + IRKey + " And Waiting After : " + WaitAfterIR.ToString)

        Try
            WaitAfterIR = WaitAfterIR * WaitAfterIRFactor

            For i As Integer = 0 To 2

                BeginWaitForDebugMessages("IEX IR key", 4)

                res = _iex.IR.SendIR(IRKey)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If

                Try
                    If EndWaitForDebugMessages("IEX IR key", ActualLines) Then
                        If WaitAfterIR > 0 Then
                            _iex.Wait(WaitAfterIR / 1000)
                        End If
                        Exit Sub
                    End If

                Catch ex As EAException
                    If i = 2 Then 'If Reached Last Iteration
                        ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, "Failed To Verify Send IR " + IRKey.ToString + " : " + ex.ShortMessage))
                    End If
                Catch ex As IEXException
                    If i = 2 Then 'If Reached Last Iteration
                        ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                    End If
                End Try
            Next

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Verify Send IR " + IRKey.ToString))

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
        Dim res As IEXGateway.IEXResult

        StartHideFailures("Sending Channel : " + ChannelNumber + " Waiting " + MsBetweenSending.ToString + " Ms Between IR")

        Try
            Dim digits As Char() = ChannelNumber.ToCharArray
            For i As Integer = 0 To digits.Length - 1
                cmdSeq += "KEY_" & digits(i) + ","
            Next

            cmdSeq = cmdSeq.Remove(cmdSeq.Length - 1, 1)

            res = _iex.IR.SendIrSequence(cmdSeq, msBetweenSending:=MsBetweenSending)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(res))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Entering Keys On Screens Like PIN
    ''' </summary>
    ''' <param name="StringToType">The Keys To Enter</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub TypeKeys(ByVal StringToType As String, Optional ByVal WaitAfter As Integer = 2000)

        StartHideFailures("Typing Keys : " + StringToType)

        Try
            Dim digits As Char() = StringToType.ToCharArray

            For Each digi As Char In digits
                SendIR("KEY_" + digi, 500)
            Next

            If WaitAfter > 0 Then
                _iex.Wait(WaitAfter / 1000)
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

            EpgText = GetValueFromDictionary("DIC_CONFIRM")

            _UI.Menu.SetConfirmationMenu(EpgText)

            _UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Translate Languages
    ''' </summary>
    ''' <param name="Language">Language To Translate</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Overrides Function TranslateLanguage(ByVal Language As String, Optional ByVal fromEnglish As Boolean = True) As String

        If fromEnglish Then
            Select Case Language
                Case "Arabic"
                    Return "ARABIC"
                Case "Bosnian"
                    Return "Bosanski"
                Case "Bulgarian"
                    Return "?????????"
                Case "Croatian"
                    Return "HRVATSKI"
                Case "Czech"
                    Return "CESKY"
                Case "Danish"
                    Return "DANSK"
                Case "Dutch"
                    Return "Nederlands"
                Case "English"
                    Return "ENGLISH"
                Case "Estonian"
                    Return "Eesti"
                Case "Finnish"
                    Return "Suomi"
                    'Case "French"
                    '    Return "Fran�ais"
                Case "French"
                    Return "Fran?ais"
                Case "German"
                    Return "DEUTSCH"
                Case "Greek"
                    Return "????????"
                Case "Hungarian"
                    Return "MAGYAR"
                Case "Irish"
                    Return "Gaeilge"
                Case "Italian"
                    Return "Italiano"
                Case "Latvian"
                    Return "Latvie�u"
                Case "Lithuanian"
                    Return "Lietuviu"
                Case "Maltese"
                    Return "Malti"
                Case "Norwegian"
                    Return "Nynorsk"
                Case "Polish"
                    Return "Polski"
                Case "Portuguese"
                    Return "PORTUGU�S"
                Case "Romanian"
                    Return "ROM�NA"
                Case "Russian"
                    Return "RUSSIAN"
                Case "Serbian"
                    Return "??????"
                Case "Slovak"
                    Return "Slovencina"
                Case "Slovenian"
                    Return "Sloven�cina"
                Case "Spanish"
                    Return "ESPA�OL"
                Case "Swedish"
                    Return "Svenska"
                Case "Turkish"
                    Return "T�RK�E"
                Case "Hebrew"
                    Return "HEBREW"
                Case "Norsk"
                    Return "NORSK"
                Case "Nederlands"
                    Return "NEDERLANDS"
                Case "Danskw"
                    Return "DANSKW"
                Case "Off"
                    Return "OFF"
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Language " + Language.ToString + " Is Not Valid"))
            End Select
        Else
            Select Case Language
                Case "ARABIC"
                    Return "Arabic"
                Case "Bosanski"
                    Return ("Bosnian")
                Case "?????????"
                    Return ("Bulgarian")
                Case "HRVATSKI"
                    Return ("Croatian")
                Case "CESKY"
                    Return ("Czech")
                Case "DANSK"
                    Return ("Danish")
                Case "Nederlands"
                    Return ("Dutch")
                Case "ENGLISH"
                    Return ("English")
                Case "Eesti"
                    Return ("Estonian")
                Case "Suomi"
                    Return ("Finnish")
                    'Case "Fran�ais"
                    '   Return ("French")
                Case "FRAN?AIS"
                    Return ("French")
                Case "DEUTSCH"
                    Return ("German")
                Case "????????"
                    Return ("Greek")
                Case "MAGYAR"
                    Return ("Hungarian")
                Case "Gaeilge"
                    Return ("Irish")
                Case "Italiano"
                    Return ("Italian")
                Case "Latvie�u"
                    Return ("Latvian")
                Case "Lietuviu"
                    Return ("Lithuanian")
                Case "Malti"
                    Return ("Maltese")
                Case "Nynorsk"
                    Return ("Norwegian")
                Case "Polski"
                    Return ("Polish")
                Case "PORTUGU�S"
                    Return ("Portuguese")
                Case "ROM�NA"
                    Return ("Romanian")
                Case "RUSSIAN"
                    Return ("Russian")
                Case "??????"
                    Return ("Serbian")
                Case "Slovencina"
                    Return ("Slovak")
                Case "Sloven�cina"
                    Return ("Slovenian")
                Case "ESPA�OL"
                    Return ("Spanish")
                Case "SVENSKA"
                    Return ("Swedish")
                Case "T�RK�E"
                    Return ("Turkish")
                Case "HEBREW"
                    Return ("Hebrew")
                Case "NORSK"
                    Return ("Norsk")
                Case "NEDERLANDS"
                    Return ("Nederlands")
                Case "DANSKW"
                    Return ("Danskw")
                Case "OFF"
                    Return ("Off")
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Language " + Language.ToString + " Is Not Valid"))
            End Select
        End If

        Return ""
    End Function

    ''' <summary>
    '''    Insert Event To Events Collection
    ''' </summary>
    ''' <param name="EventKeyName">The Key Of The Event</param>
    ''' <param name="EventName">Event Name</param>
    ''' <param name="EventSource">Event Source</param>
    ''' <param name="StartTime">Start Time</param>
    ''' <param name="EndTime">End Time</param>
    ''' <param name="Channel">Channel Number</param>
    ''' <param name="Duration">Event Duration</param>
    ''' <param name="tDate">Event Date</param>
    ''' <param name="Frequency">Event Frequency</param>
    ''' <param name="EventRecurrence">Event Recurrence Number</param>
    ''' <remarks></remarks>
    Public Overrides Sub InsertEventToCollection(ByVal EventKeyName As String, ByVal EventName As String, ByVal EventSource As String, ByVal StartTime As String, _
                                                 ByVal EndTime As String, ByVal Channel As String, ByVal Duration As Long, ByVal OriginalDuration As Long, ByVal tDate As String, ByVal Frequency As String, _
                                                 ByVal EventRecurrence As Integer, ByVal EventConvertedDate As String, Optional ByVal IsModify As Boolean = False, Optional ByVal IsSeries As Boolean = False)
        'Adding the event to collection
        If IsModify = False Then
            _UI.Events.Add(EventKeyName, New EPG.EpgEvent(Me))
        End If
        If Not String.IsNullOrEmpty(EventName) Then _UI.Events(EventKeyName).Name = EventName
        If Not String.IsNullOrEmpty(EventSource) Then _UI.Events(EventKeyName).Source = EventSource
        If Not String.IsNullOrEmpty(StartTime) Then _UI.Events(EventKeyName).StartTime = StartTime
        If Not String.IsNullOrEmpty(EndTime) Then _UI.Events(EventKeyName).EndTime = EndTime
        If Not String.IsNullOrEmpty(Channel) Then _UI.Events(EventKeyName).Channel = Channel
        If Not Duration = 0 Then _UI.Events(EventKeyName).Duration = Duration
        If Not OriginalDuration = 0 Then _UI.Events(EventKeyName).OriginalDuration = OriginalDuration
        If Not String.IsNullOrEmpty(tDate) Then _UI.Events(EventKeyName).EventDate = tDate
        If Not String.IsNullOrEmpty(Frequency) Then _UI.Events(EventKeyName).Frequency = Frequency
        _UI.Events(EventKeyName).Occurrences = EventRecurrence
        If Not String.IsNullOrEmpty(EventConvertedDate) Then _UI.Events(EventKeyName).ConvertedDate = EventConvertedDate
        _UI.Events(EventKeyName).IsSeries = IsSeries

        _UI.Utils.LogCommentImportant(IIf(EventSource.Contains("Manual"), "Manual Recording Event Key : ", "Event Key : ") + EventKeyName + vbCrLf + _
                                      "Name : " + EventName + vbCrLf + _
                                      "StartTime : " + StartTime + vbCrLf + _
                                      "EndTime : " + EndTime + _
                                      IIf(Channel <> "", vbCrLf + "ChannelNum : " + Channel, "") + _
                                      IIf(OriginalDuration <> 0, vbCrLf + "Original Duration : " + OriginalDuration.ToString, "") + _
                                      IIf(Duration <> 0, vbCrLf + "Duration : " + Duration.ToString, "") + _
                                      IIf(tDate <> "", vbCrLf + "Date : " + tDate, "") + _
                                      IIf(Frequency <> "", vbCrLf + "Frequency : " + Frequency, "") + _
                                      IIf(EventRecurrence <> 0, vbCrLf + "Recurrence : " + EventRecurrence.ToString, "") + _
                                      IIf(IsSeries, vbCrLf + "Is series : " + IsSeries.ToString, ""))
    End Sub


    ''' <summary>
    '''   Navigates With IEX Core Named Navigation
    ''' </summary>
    ''' <param name="Name">Named Navigation Criteria</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overridable Function EPG_Milestones_NavigateByName(ByVal Name As String) As Boolean
        Dim Res As IEXGateway.IEXResult
        Dim StopAtWarning As Boolean

        StopAtWarning = CBool(GetValueFromEnvironment("WarningAsError"))

        Res = _iex.MilestonesEPG.NavigateByName(Name)

        If Not Res.CommandSucceeded Then
            _iex.GetSnapshot("For Debugging....")
            ExceptionUtils.ThrowEx(New IEXException(Res))
        ElseIf Res.HasWarning And StopAtWarning = False Then
            LogCommentWarning("WARNING :" + Res.WarningReason)
        ElseIf Res.HasWarning And StopAtWarning Then
            ExceptionUtils.ThrowEx(New IEXWarnning(Res))
        End If

        Return True
    End Function

    ''' <summary>
    '''   Navigates With IEX Core Navigation To The Desired PATH
    ''' </summary>
    ''' <param name="Path">Navigation Criteria</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overridable Function EPG_Milestones_Navigate(ByVal Path As String) As Boolean
        Dim Res As IEXGateway.IEXResult
        Dim StopAtWarning As Boolean

        StopAtWarning = CBool(GetValueFromEnvironment("WarningAsError"))

        Res = _iex.MilestonesEPG.Navigate(Path)

        If Not Res.CommandSucceeded Then
            _iex.GetSnapshot("For Debugging....")
            ExceptionUtils.ThrowEx(New IEXException(Res))
        ElseIf Res.HasWarning And StopAtWarning = False Then
            LogCommentWarning("WARNING :" + Res.WarningReason)
        ElseIf Res.HasWarning And StopAtWarning Then
            ExceptionUtils.ThrowEx(New IEXWarnning(Res))
        End If

        Return True
    End Function

    ''' <summary>
    '''   Select Menu Item With IEX Core Command To The Desired PATH
    ''' </summary>
    ''' <param name="Item">Item Criteria</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overridable Function EPG_Milestones_SelectMenuItem(ByVal Item As String) As Boolean
        Dim Res As IEXGateway.IEXResult
        Dim StopAtWarning As Boolean

        StopAtWarning = CBool(GetValueFromEnvironment("WarningAsError"))

        Res = _iex.MilestonesEPG.SelectMenuItem(Item)

        If Not Res.CommandSucceeded Then
            ExceptionUtils.ThrowEx(New IEXException(Res))
        ElseIf Res.HasWarning And StopAtWarning = False Then
            LogCommentWarning(Res.WarningReason)
        ElseIf Res.HasWarning And StopAtWarning Then
            ExceptionUtils.ThrowEx(New IEXWarnning(Res))
        End If

        Return True
    End Function


#Region "Debug"

    ''' <summary>
    '''    Beginning Waiting For Debug Messages With Timeout
    ''' </summary>
    ''' <param name="Messages">Message Or Messages With , Delimiter</param>
    ''' <param name="Timeout">Timeout In Seconds</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function BeginWaitForDebugMessages(ByVal Messages As String, ByVal Timeout As Integer) As Boolean
        Dim res As IEXGateway.IEXResult
        Dim MessagesCol As String()

        Timeout = BeginWaitForMessageFactor * Timeout

        If Messages <> "" Then
            MessagesCol = Messages.Split(",")

            For Each msg As String In MessagesCol
                res = _iex.Debug.BeginWaitForMessage(msg, 0, Timeout, IEXGateway.DebugDevice.Udp)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
            Next
        End If

        Return True

    End Function

    ''' <summary>
    '''    End Waiting For Debug Messages With Timeout
    ''' </summary>
    ''' <param name="Messages">Message Or Messages With , Delimiter</param>
    ''' <param name="ActualLines">ByRef Actual Lines Returned In Debug</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function EndWaitForDebugMessages(ByVal Messages As String, ByRef ActualLines As ArrayList) As Boolean
        Dim res As IEXGateway.IEXResult = Nothing
        Dim Passed As Boolean = True
        Dim MessagesCol As String()
        Dim ActualLine As String = ""
        Dim LogicalError As Boolean = False
        Dim Fatal As Boolean = False
        Dim FatalRes As IEXGateway.IEXResult = Nothing
        ActualLines.Clear()

        If Messages <> "" Then
            MessagesCol = Messages.Split(",")

            For Each msg As String In MessagesCol
                res = _iex.Debug.EndWaitForMessage(msg, ActualLine, "", IEXGateway.DebugDevice.Udp)
                If Not res.CommandSucceeded Then
                    LogCommentFail("Failed To Verify Message : " + msg)
                    If res.FailureCode = 165 Then
                        LogicalError = True
                    Else
                        FatalRes = res
                        Fatal = True
                    End If
                    Passed = False
                Else
                    ActualLines.Add(ActualLine)
                    LogCommentInfo("Verified Message : " + msg)
                End If
            Next
        End If

        If Passed Then
            Return True
        ElseIf LogicalError And Fatal = False Then
            Return False
        ElseIf Fatal Then
            ExceptionUtils.ThrowEx(New IEXException(FatalRes))
        Else
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Messages Empty !!!"))
        End If

        Return False
    End Function

    ''' <summary>
    '''   Verifying FAS Milestones
    ''' </summary>
    ''' <param name="IR">IR To Send</param>
    ''' <param name="Messages">FAS Messages In FAS Milestones</param>
    ''' <param name="Timeout">Timeout In Seconds</param>
    ''' <param name="IsOrdered">If True Verify Messages In Order</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Function VerifyFas(ByVal IR As String, ByVal Messages As String, ByVal Timeout As Integer, Optional ByVal IsOrdered As Boolean = False) As Boolean
        Dim MessagesCol As String() = Nothing
        Dim ActualLines As New ArrayList
        Dim LastArrived As Double = 0
        Dim Succeed As Boolean = False

        StartHideFailures("Verifying FAS Milestones Messages...")

        BeginWaitForDebugMessages(Messages, Timeout)

        Try
            SendIR(IR)
        Catch ex As Exception
            StartHideFailures("Closing WaitForDebugMessages...")
            Try
                EndWaitForDebugMessages(Messages, ActualLines)
            Catch ex2 As Exception
            End Try
            _iex.ForceHideFailure()
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Send IR Command - " + IR.ToString))
        End Try

        Try
            Succeed = EndWaitForDebugMessages(Messages, ActualLines)
        Catch ex As Exception
            _iex.ForceHideFailure()
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify All FAS Milestones : " + Messages))
        End Try
		
		If Not Succeed Then
            _iex.ForceHideFailure()
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FasVerificationFailure, "Failed To Verify FAS Messages : " + Messages))
        End If

        _iex.ForceHideFailure()
        Return True

    End Function

    Public Function ConvertUTCToTime(ByVal UTCTime As String) As String
        Dim time As DateTime = New DateTime()
        Dim stime As DateTime = New DateTime()

        Return CDate("1.1.1970 00:00:00").AddSeconds(UTCTime).AddHours(1).ToString("HH:mm")

    End Function

    ''' <summary>
    '''   Clears IEX EPG Info
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub ClearEPGInfo()
        Dim res As IEXGateway.IEXResult

        res = _iex.MilestonesEPG.ClearEPGInfo
        If Not res.CommandSucceeded Then
            ExceptionUtils.ThrowEx(New IEXException(res))
        End If
    End Sub

    ''' <summary>
    '''    Sends IR And Verifies Key Or Value Or Both With Timeout And Delay After
    ''' </summary>
    ''' <param name="Key">The EPGInfo Key</param>
    ''' <param name="Value">The EPGInfo Value</param>
    ''' <param name="TimeoutInSec">Time Out In Seconds</param>
    ''' <param name="Delay">Delay In Seconds</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function VerifyDebugMessage(ByVal Key As String, ByVal Value As String, ByVal TimeoutInSec As Integer, Optional ByVal Delay As Integer = 2) As Boolean
        Dim Timeout As Date = Now.AddSeconds(TimeoutInSec)
        Dim Passed As Boolean = False
        Dim Result As String = ""
        Dim Message As String = ""
        Dim Margine As String = ""

        StartHideFailures("Utils.VerifyDebugMessage : Waiting " + IIf(Value = "", "For Key -> " + Key, "For Key -> " + Key + " With Value -> " + Value) + " Timout Requested : " + TimeoutInSec.ToString + " Timeout Until-> " + Timeout.ToString + " In EPG Milestones")

        Do While DateDiff(DateInterval.Second, Now, Timeout) > 0

            Try
                GetEpgInfo(Key, Result, True)
            Catch ex As IEXException
            End Try

            If Value = "" Then
                If Result <> "" Then
                    Passed = True
                    Exit Do
                End If
            Else
                If Result = Value Then
                    Passed = True
                    Exit Do
                End If
            End If

            'Don't want to use iex.wait since it will be written to the log
            System.Threading.Thread.Sleep(1000)
            System.Windows.Forms.Application.DoEvents()
        Loop
        _iex.ForceHideFailure()

        If Passed And Delay > 0 Then
            _iex.Wait(Delay)
        End If

        If Passed = False Then
            LogCommentFail("Utils.VerifyDebugMessage : Failed To Verify Key " + Key + " GOT --> " + Result)
        End If

        Return Passed
    End Function

    ''' <summary>
    '''   Gets A Value From EPG Info Dictionary
    ''' </summary>
    ''' <param name="Key">Key Of The Value</param>
    ''' <param name="Value">ByRef The Returned Value</param>
    ''' <param name="CalledFromVerify">If True Disregards Empty Values</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Function GetEpgInfo(ByVal Key As String, ByRef Value As String, Optional ByVal CalledFromVerify As Boolean = False) As Boolean
        Dim res As IEXGateway.IEXResult
        'Try
            res = _iex.MilestonesEPG.GetEPGInfo(Key.ToLower, Value)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(res))
            End If
       ' Catch ex As Exception
         '   LogCommentWarning("Failed to get the EPG Info for key " + Key.ToLower)
       ' End Try

        If Value = "" Then
            If CalledFromVerify = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.EmptyEpgInfoFailure, "Returned Nothing For Key - " + Key.ToLower.ToString + " From EPG Milestones"))
            End If
            LogCommentWarning("WARNING - Key : " + Key + " Returned Nothing")
        End If

        LogCommentInfo("Utils.GetEpgInfo: GOT -> " + Value.ToString + IIf(Key <> "", " For Key -> " + Key.ToLower.ToString, "") + " From EPG Milestones")
        Return True
    End Function

    ''' <summary>
    '''   Parsing EPG Time From EPG
    ''' </summary>
    ''' <param name="DateTime">EPG Date Time</param>
    ''' <param name="ReturnedTime">ByRef The Returned Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overridable Sub ParseEPGTime(ByVal DateTime As String, ByRef ReturnedTime As String)
        Try
            Dim DatePart As String() = DateTime.Split("_")
            Dim time As String = ""

            time = DatePart(3) + ":" + DatePart(4) + ":" + DatePart(5)

            ReturnedTime = time

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse EPG Time From " + DateTime + " : " + ex.Message))
        End Try
    End Sub

    ''' <summary>
    '''   Parsing EPG Date From EPG
    ''' </summary>
    ''' <param name="DateTime">EPG Date Time</param>
    ''' <param name="ReturnedDate">ByRef The Returned Date</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Sub ParseEPGDate(ByVal DateTime As String, ByRef ReturnedDate As String)
        Try
            Dim DatePart As String() = DateTime.Split("_")
            Dim sdate As String = ""

            sdate = DatePart(0) + "/" + DatePart(1) + "/" + DatePart(2)
            ReturnedDate = sdate

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse EPG Date From " + DateTime + " : " + ex.Message))
        End Try
    End Sub

    ''' <summary>
    '''   Parsing Event Time From Returned EPG Time
    ''' </summary>
    ''' <param name="ReturnedTime">The Returned Time After Parsing</param>
    ''' <param name="TimeString">Time String From EPG</param>
    ''' <param name="IsStartTime">If True Returnes Start Time Else Returns End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub ParseEventTime(ByRef ReturnedTime As String, ByVal TimeString As String, ByVal IsStartTime As Boolean)
        Dim StartTime As String = TimeString
        Dim EndTime As String = TimeString
        Dim EvBothTimes As String()

        Try
            If IsStartTime Then
                StartTime = StartTime.Replace(" > ", " ").Replace(">", " ").Replace(" - ", " ").Replace("-", " ")
                EvBothTimes = StartTime.Split(" ")

                If EvBothTimes.Count > 1 Then
                    ReturnedTime = EvBothTimes(0)
                    LogCommentInfo("Got -> " + ReturnedTime.ToString)
                    Exit Sub
                End If
            Else
                EndTime = EndTime.Replace(" > ", " ").Replace(">", " ").Replace(" - ", " ").Replace("-", " ")
                EvBothTimes = EndTime.Split(" ")

                If EvBothTimes.Count > 1 Then
                    ReturnedTime = EvBothTimes(1)
                    LogCommentInfo("Got -> " + ReturnedTime.ToString)
                    Exit Sub
                End If

            End If
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse Event " + IIf(IsStartTime, "Start ", "End ") + "Time From " + TimeString + " : " + ex.Message))
        End Try
    End Sub

    Public Sub ParseEventDate(ByRef ReturnedDate As String, ByVal DetailsString As String)
        Try
            Dim DetailsParts As String()

            ReturnedDate = DetailsString.Replace("Recorded on ", "")
            DetailsParts = ReturnedDate.Split(" ")

            ReturnedDate = DetailsParts(0).ToString + " " + DetailsParts(1).ToString + " " + DetailsParts(2).ToString

            LogCommentInfo("Got -> " + ReturnedDate.ToString)

            Exit Sub
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse Event Date From " + DetailsString + " : " + ex.Message))
        End Try
    End Sub

    ''' <summary>
    '''   Getting Active State From EPG By The State Machine
    ''' </summary>
    ''' <param name="State">ByRef The Returned State</param>
    ''' <remarks></remarks>
    Public Sub GetActiveState(ByRef State As String)
        Dim res As IEXGateway.IEXResult

        res = _iex.MilestonesEPG.GetActiveState(State)
        If Not res.CommandSucceeded Then
            ExceptionUtils.ThrowEx(New IEXException(res))
        End If
    End Sub

    ''' <summary>
    '''   Waiting For Given State To Arrive Until Timeout And Delaying After Found
    ''' </summary>
    ''' <param name="State">Requested State</param>
    ''' <param name="TimeOutInSec">Optional Parameter Default = 15 : Time Out In Seconds</param>
    ''' <param name="Delay">Optional Parameter Default = 0 : How Much Seconds To Delay </param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overridable Function VerifyState(ByVal State As String, Optional ByVal TimeOutInSec As Integer = 15, Optional ByVal Delay As Integer = 0) As Boolean
        Dim res As IEXGateway.IEXResult
        Dim CurrentState As String = ""
        Dim Timeout As Date = Now.AddSeconds(TimeOutInSec)

        StartHideFailures("Utils.VerifyState : Waiting For State " + State + " Timout Requested : " + TimeOutInSec.ToString + " Timeout Until-> " + Timeout.ToString)

        Try
            Do
                res = _iex.MilestonesEPG.GetActiveState(CurrentState)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
                If CurrentState = State Then
                    If Delay <> 0 Then
                        _iex.Wait(Delay)
                    End If
                    Return True
                End If

                Threading.Thread.Sleep(1000)
            Loop Until DateDiff(DateInterval.Second, Now, Timeout) < 0

            LogCommentFail("VerifyState : Failed To Verify Active State Is " + State.ToString)
            Return False
        Finally
            _iex.ForceHideFailure()
        End Try
    End Function

#End Region

    ''' <summary>
    '''   Verifies Live Reached
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyLiveReached()
        If Not VerifyState("LIVE", 25) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Live State Reached"))
        End If
    End Sub

    ''' <summary>
    '''   Finds Event In PCAT
    ''' </summary>
    ''' <param name="PCATEvID">ByRef The Returned PCATEvID</param>
    ''' <param name="Table">EnumTables Can Be RECORDINGS Or BOOKINGS</param>
    ''' <param name="EventName">The Event Name To Look For</param>
    ''' <param name="EvDateStart">The Event Start Time For Manual Recording Events</param>
    ''' <param name="EvDuration">The Event Duration For Manual Recording Events</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function FindEventInPCAT(ByRef PCATEvID As String, ByVal Table As EnumTables, ByVal EventName As String, Optional ByVal EvDateStart As String = "", Optional ByVal EvDuration As String = "") As Boolean
        Dim Msg As String = ""
        Dim PCAT_Handler As New PcatHandler.Reader
        Dim ds As New DataSet
        Dim deviationFlag As Boolean = False
        LogCommentInfo("Trying To Find Event : " + EventName + " In Table :" + Table.ToString + " EvDateStart : " + EvDateStart + " EvDuration : " + EvDuration + " In PCAT")

        Try
            Select Case Table
                Case EnumTables.RECORDINGS
                    ds = PCAT_Handler.GetRecordingsTable("c:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.db")
                Case EnumTables.BOOKINGS
                    ds = PCAT_Handler.GetBookingsTable("c:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.db")
            End Select

            LogCommentInfo("Number Of Rows In PCAT : " + ds.Tables(0).Rows.Count.ToString)

            LogCommentWarning("WORKAROUND : CHECKING THAT EVENT NAME CONTAINS THE EVENT NAME IN PCAT")

            For Each row As DataRow In ds.Tables(0).Rows
                LogCommentInfo("Checking If EventName : " + EventName + " Contains PCAT EventName : " + row("EVENT_NAME").ToString)

                If GetValueFromEnvironment("Project").ToUpper = "GET" Then
                    deviationFlag = EventName.Contains(row("EVENT_NAME").ToString.Trim.ToUpper)
                Else
                    deviationFlag = EventName.Contains(row("EVENT_NAME").ToString.Trim)
                End If
                If deviationFlag Then
                    If EvDateStart <> "" And EvDuration <> "" Then
                        LogCommentInfo("Checking If PCAT LOCAL_START_TIME : " + row("LOCAL_START_TIME").ToString.Trim.ToLower + " Contains EvDateStart : " + EvDateStart)
                        If row("LOCAL_START_TIME").ToString.Trim.ToLower.Contains(EvDateStart) Then
                            LogCommentInfo("Checking If PCAT DURATION : " + row("DURATION").ToString.ToLower + " Contains EvDuration : " + EvDuration)
                            If row("DURATION").ToString.Trim.ToLower.Contains(EvDuration) Then
                                PCATEvID = row("EVENT_ID").ToString
                                LogCommentImportant("Found Event " + EventName + " PCATEvID = " + PCATEvID)
                                Return True
                            End If
                        End If
                    Else
                        LogCommentInfo("Checking If EventName : " + EventName.ToLower.Replace(" ", "") + " Contains PCAT EventName : " + row("EVENT_NAME").ToString.Trim.ToLower.Replace(" ", ""))
                        If EventName.Trim.ToLower.Replace(" ", "").Contains(row("EVENT_NAME").ToString.Trim.ToLower.Replace(" ", "")) Then
                            PCATEvID = row("EVENT_ID").ToString.Trim
                            LogCommentImportant("Found Event " + EventName + " PCATEvID = " + PCATEvID)
                            Return True
                        Else
                            Return False
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            Msg = " With Exception : " + ex.Message
        Finally
            PCAT_Handler = Nothing
            ds.Dispose()
        End Try

        LogCommentFail("Failed To Find Event " + EventName + " In PCAT" + Msg)
        Return False

    End Function

    ''' <summary>
    '''   Verifi
    ''' </summary>
    ''' <param name="PCATEvID">The ID Found Earlier In FindInPCAT Function</param>
    ''' <param name="Table">EnumTables Can Be RECORDINGS Or BOOKINGS</param>
    ''' <param name="Field">The Verification Field</param>
    ''' <param name="EventInfo">The Verification Field To Verify</param>
    ''' <param name="ActualStatus">ByRef The Actual Info Found</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function VerifyEventInfo(ByVal PCATEvID As String, ByVal Table As EnumTables, ByVal Field As String, ByVal EventInfo As String, ByRef ActualStatus As String) As Boolean
        Dim PCAT_Handler As New PcatHandler.Reader
        Dim ds As New DataSet

        Try
            Select Case Table
                Case EnumTables.RECORDINGS
                    ds = PCAT_Handler.GetRecordingsTable("c:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.db")
                Case EnumTables.BOOKINGS
                    ds = PCAT_Handler.GetBookingsTable("c:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.db")
            End Select

            For Each row As DataRow In ds.Tables(0).Rows
                LogCommentInfo("Checking If PCAT Event ID : " + row("EVENT_ID").ToString + " Equals PCATEvID : " + PCATEvID)
                If row("EVENT_ID").ToString.Trim = PCATEvID Then
                    If row(Field).ToString.Trim = EventInfo Then
                        LogCommentImportant("Found Event Info : " + EventInfo)
                        Return True
                    Else
                        ActualStatus = row(Field).ToString.Trim
                        LogCommentFail("Failed To Find Event Info : " + EventInfo + " Actual Info Is : " + ActualStatus)
                        Return False
                    End If
                End If
            Next
        Finally
            PCAT_Handler = Nothing
            ds.Dispose()
        End Try

        LogCommentFail("Failed To Find Event Info : " + EventInfo + " Actual Info Is : " + ActualStatus)
        Return False
    End Function

    ''' <summary>
    '''    PCAT Get Event Info By PCATEvID  
    ''' </summary>
    ''' <param name="PCATEvID">The ID Found Earlier In FindInPCAT Function</param>
    ''' <param name="Table">EnumTables Can Be RECORDINGS Or BOOKINGS</param>
    ''' <param name="Field">The Searched Field</param>
    ''' <param name="Status">The Field Value</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function GetEventInfo(ByVal PCATEvID As String, ByVal Table As EnumTables, ByVal Field As String, ByRef Status As String) As Boolean

        Dim PCAT_Handler As New PcatHandler.Reader
        Dim ds As New DataSet

        LogCommentInfo("Trying To Get Event Info PCATEvID : " + PCATEvID + " In Table :" + Table.ToString + " Field : " + Field + " From PCAT")

        Try
            Select Case Table
                Case EnumTables.RECORDINGS
                    ds = PCAT_Handler.GetRecordingsTable("c:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.db")
                Case EnumTables.BOOKINGS
                    ds = PCAT_Handler.GetBookingsTable("c:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.db")
            End Select

            For Each row As DataRow In ds.Tables(0).Rows
                LogCommentInfo("Checking If PCAT Event ID : " + row("EVENT_ID").ToString + " Equals PCATEvID : " + PCATEvID)
                If row("EVENT_ID").ToString.Trim = PCATEvID Then
                    Status = row(Field).ToString.Trim
                    LogCommentImportant("Found Event Info : " + Field + "=" + Status)
                    Return True
                End If
            Next

            LogCommentFail("Failed To Get Event Info For PCATEvID : " + PCATEvID)
            Return False
        Finally
            PCAT_Handler = Nothing
            ds.Dispose()
        End Try

    End Function

    ''' <summary>
    '''   Checking If PCAT Is Valid
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Function IsPCATValid(ByRef PCATError As String) As Boolean
        Dim PCAT_Handler As New PcatHandler.Reader
        Dim ds As New DataSet

        Try
            ds = PCAT_Handler.GetRecordingsTable("c:\PCAT_Modifier\IEX" + _iex.IEXServerNumber.ToString + "\PCAT.db")
        Catch ex As Exception
            LogCommentFail("PCAT Is Not Valid Exception : " + ex.Message)
            PCATError = ex.Message
            Return False
        Finally
            PCAT_Handler = Nothing
            ds.Dispose()
        End Try

        Return True
    End Function

    ''' <summary>
    '''   Try Retrieving EPG Date Format From Project.ini Otherwise Default Value From Code
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetEpgDateFormat() As String

        Dim epgDateFormat As String = ""

        Try
            epgDateFormat = GetValueFromProject("EPG", "DATE_FORMAT")
        Catch ex As Exception
            LogCommentFail("Failed to Get EPG Date Format from Project.ini.Following exception - " + ex.Message)
            epgDateFormat = GetEpgDateFormatDefaultValue()
            LogCommentWarning("WARNING : Retrieving Default Value From Code Instead: " & epgDateFormat)
        End Try

        Return epgDateFormat

    End Function

    ''' <summary>
    '''   Try Retrieving Date Format For Event Dictionary From Project.ini Otherwise Default Value From Code
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetDateFormatForEventDictionary() As String

        Dim epgDateFormat As String = ""

        Try
            epgDateFormat = GetValueFromProject("EPG", "DATE_FORMAT_FOR_EVENT_DIC")
        Catch ex As Exception
            LogCommentFail("Failed to Get EPG Date Format from Project.ini.Following exception - " + ex.Message)
            epgDateFormat = GetDateFormatForEventDicDefaultValue()
            LogCommentWarning("WARNING : Retrieving Default Value From Code Instead: " & epgDateFormat)
        End Try

        Return epgDateFormat

    End Function

    ''' <summary>
    ''' Get the EPG Time delimiter from the project.ini otherwise we get default value from code
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetEpgTimeDelimiter() As String

        Dim epgTimeDelimiter As String = ""

        Try
            epgTimeDelimiter = GetValueFromProject("EPG", "TIME_DELIMITER")
        Catch ex As Exception
            LogCommentFail("Failed to Get EPG Time delimiter from Project.ini.Following exception - " + ex.Message)
            epgTimeDelimiter = GetEpgTimeDelimiterDefaultValue()
            LogCommentWarning("Retrieving Default Value From Code Instead: " & epgTimeDelimiter)
        End Try

        Return epgTimeDelimiter

    End Function

    ''' <summary>
    '''   Retrieving EPG Date Format Default Value
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetEpgDateFormatDefaultValue() As String

        Return "ddd d MMM"

    End Function

    ''' <summary>
    '''   Retrieving Date Format For Event Dictionary Default Value
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetDateFormatForEventDicDefaultValue() As String

        Return "dd/MM/yyyy"

    End Function

    ''' <summary>
    ''' Retrieving default value of Time delimiter
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetEpgTimeDelimiterDefaultValue() As String

        Return "-"

    End Function

    ''' <summary>
    '''    Formatting the Event Time Based On Start Time And End Time  
    ''' </summary>
    ''' <param name="sStartTime">The Start Time As String</param>
    ''' <param name="sEndTime">The End Time As String</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function FormatEventTime(sStartTime As String, sEndTime As String) As String
        Return sStartTime & GetEventTimeSeparator() & sEndTime
    End Function

    ''' <summary>
    '''    Retrieving The Event Time Separator To Be Inserted Between Start Time And End Time  
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetEventTimeSeparator() As String
        Return " - "
    End Function

    Overrides Function GetExpectedEventTimeLength() As Byte
        Return GetEventTimeSeparator().Length + ExpectedTimeLengthWithoutSeparator
    End Function

    Public Sub StartStopService(ByVal ServiceName As String)

        Dim service As ServiceController = New ServiceController(ServiceName)
        Dim _wait As Integer = 1
        If ((service.Status.Equals(ServiceControllerStatus.Stopped)) OrElse (service.Status.Equals(ServiceControllerStatus.StopPending))) Then
            service.Start()
        Else
            service.Stop()
            While (Not service.Status.Equals(ServiceControllerStatus.Stopped)) And (_wait < 10)
                _iex.Wait(1)
                _wait += 1
            End While
            LogCommentInfo("Took " + _wait.ToString + " seconds to stop the NFS service.")

            service.Start()
            While (Not service.Status.Equals(ServiceControllerStatus.Running)) And (_wait < 10)
                _iex.Wait(1)
                _wait += 1
            End While
            LogCommentInfo("Took " + _wait.ToString + " seconds to start the NFS service.")
        End If

    End Sub

    ''' <summary>
    '''    Retrieving Base Path Based On IEX Server Number
    ''' </summary>
    ''' <param name="iexServerNumber">IEX Server Number</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Private Function GetBasePathOfIexNumber(ByVal iexServerNumber As Short)
        Return PathToIexPrefix & iexServerNumber
    End Function

    ''' <summary>
    '''    Retrieving Full Path To iex.ini File
    ''' </summary>
    ''' <param name="iexServerNumber">IEX Server Number</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetFullPathToIexIniFile(ByVal iexServerNumber As Short) As String
        Return Path.Combine(GetBasePathOfIexNumber(iexServerNumber), "iex.ini")
    End Function

    ''' <summary>
    '''    Retrieving Full Path To iex.xml File
    ''' </summary>
    ''' <param name="iexServerNumber">IEX Server Number</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetFullPathToIexXmlFile(ByVal iexServerNumber As Short) As String
        Return Path.Combine(GetBasePathOfIexNumber(iexServerNumber), "iex.xml")
    End Function

    ''' <summary>
    '''    Retrieving Baud Rate From Config File (iex.xml If Existing, Otherwise From iex.ini)
    ''' </summary>
    ''' <param name="iexServerNumber">IEX Server Number</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetBaudRateFromConfigFile(ByVal iexServerNumber As Short) As String
        Dim baudrate As String
        Dim fullPathToIexXml As String = GetFullPathToIexXmlFile(iexServerNumber)

        If File.Exists(fullPathToIexXml) Then
            Dim xml As XML = New XML()
            baudrate = xml.GetSerialBaudRateFromIexXml(fullPathToIexXml)
        Else
            Dim fullPathToIexIni As String = GetFullPathToIexIniFile(iexServerNumber)
            Dim iniFile = New AMS.Profile.Ini(fullPathToIexIni)
            baudrate = iniFile.GetValue("DEBUG_OUTPUT", "BAUDRATE").ToString
        End If

        Return baudrate
    End Function

    ''' <summary>
    ''' Get the guard time Integer from friendly name passed
    ''' </summary>
    ''' <param name="friendlyName">The guard time friendly name from state machine as String</param>
    ''' <param name="isSGT">The Guard Time Type As String</param>
    ''' <returns>The Guard Time as Integer</returns>
    ''' <remarks></remarks>
    Public Function GetGuardTimeFromFriendlyName(ByVal friendlyName As String, Optional ByVal isSGT As Boolean = True) As Integer

        Dim guardTime As Integer
        Dim guardTimeType As String = ""

        If isSGT Then
            guardTimeType = "SGT"
        Else
            guardTimeType = "EGT"
        End If

        'Get the integer string from the friendly name
        friendlyName = friendlyName.Split(" ").First()

        If Not Integer.TryParse(friendlyName, guardTime) Then

            Select Case friendlyName
                Case "NONE"
                    guardTime = 0
                Case "AUTOMATIC"
                    Dim guardTimeString As String = GetValueFromProject(guardTimeType, "AUTOMATIC")
                    guardTime = Integer.Parse(guardTimeString)
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to parse given friendly name. Check State machine for more details!"))
            End Select

        End If

        Return guardTime

    End Function

    ''' <summary>
    ''' Get the minimum number of scrolls between two services based on their positions and the total number of services present in the stream
    ''' </summary>
    ''' <param name="baseChPos">The base channel position from which the scrolls have to be calculated</param>
    ''' <param name="targetChPos">The target channel position to which the scrolls have to be calculated</param>
    ''' <param name="totalCh">Total number of services present in the stream</param>
    ''' <returns>The direction of scroll as Boolean. True is returned if forward scroll is required, false is returned if backward scroll is required</returns>
    Public Overrides Function GetMinScrollsBetweenServices(ByVal baseChPos As Integer, ByVal targetChPos As Integer, ByVal totalCh As Integer, ByRef isNext As Boolean) As Integer
        isNext = True

        If baseChPos > totalCh Or targetChPos > totalCh Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Total number of services is lesser than given service positions! Please check your stream configuration!"))
        End If

        Dim numberOfScrolls As Integer = targetChPos - baseChPos
        If numberOfScrolls > 0 Then
            If numberOfScrolls > totalCh / 2 Then
                numberOfScrolls = totalCh - numberOfScrolls
                isNext = False
            End If
        ElseIf numberOfScrolls < 0 Then
            numberOfScrolls = Math.Abs(numberOfScrolls)
            isNext = False
            If numberOfScrolls > totalCh / 2 Then
                numberOfScrolls = totalCh - numberOfScrolls
                isNext = True
            End If
        End If

        Return numberOfScrolls
    End Function

#Region "EPG State Machine Parsing Methods"

    Public Class StateMachine

        Private utils As EPG.SF.Utils

        Sub New(ByRef utils As EPG.SF.Utils)
            Me.utils = utils
        End Sub

        ''' <summary>
        ''' Get State Machine Path (currently hardcoded)
        ''' </summary>
        ''' <returns>The state machine Path as String</returns>
        Public Function GetStateMachinePath() As String
            Return "C:\Program Files\IEX\Tests\TestsINI\IEX" + utils._iex.IEXServerNumber.ToString() + "\EpgStateMachineConfiguration.xml"
        End Function

        ''' <summary>
        ''' Get the state object from the state machine
        ''' </summary>
        ''' <param name="stateName">The name of the state</param>
        ''' <returns>The state object with all its information</returns>
        Public Function GetState(ByVal stateName As String) As EpgState
            Dim ns As XNamespace = "http://www.w3.org/2001/XMLSchema-instance"
            Dim matchedObj As New EpgState()
            Dim epgStateMachinePath As String = GetStateMachinePath()

            'Load XML Document
            Dim xmlFile As XDocument = LoadXMLFile(epgStateMachinePath)
            'Get the state XML object which matches the given state name
            Dim matchedXmlObj As XElement = (From obj In xmlFile.Descendants("State") Where obj.Attribute("name") = stateName).FirstOrDefault()

            If Not IsNothing(matchedXmlObj) Then
                'Remove the XML namespace type tag to prevent its resolution while deserialising
                Dim xAttrib As XAttribute = matchedXmlObj.Attribute(ns + "type")
                If Not IsNothing(xAttrib) Then
                    xAttrib.Remove()
                End If

                'Deserialise the object
                Try
                    Dim xmlSerializer = New System.Xml.Serialization.XmlSerializer(GetType(EpgState))
                    Using memoryStream As New MemoryStream(Encoding.ASCII.GetBytes(matchedXmlObj.ToString()))
                        matchedObj = DirectCast(xmlSerializer.Deserialize(memoryStream), EpgState)
                    End Using
                Catch ex As Exception
                    matchedObj = Nothing
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed to deserialize state object - " + stateName + " because of the reason - " + ex.Message))
                End Try
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "The state passed - " + stateName + " does not exist in the state machine file!"))
            End If

            Return matchedObj
        End Function

        ''' <summary>
        ''' Get Navigation Path from Named Navigation
        ''' </summary>
        ''' <param name="namedNavigation">The named navigation as string</param>
        ''' <returns>Complete Navigation path as string</returns>
        Public Function GetNavigationPath(ByVal namedNavigation As String) As String
            Dim epgStateMachinePath As String = GetStateMachinePath()
            Dim xmlFile As XDocument = LoadXMLFile(epgStateMachinePath)
            'Get the XML Element which matches the named navigation
            Dim matchedNavigation As XElement = (From obj In xmlFile.Descendants("NamedNavigation") Where obj.Attribute("name") = namedNavigation).FirstOrDefault()
            Dim navigationPath As String = ""

            If Not IsNothing(matchedNavigation) Then
                'Get the navigation path
                navigationPath = matchedNavigation.Attribute("path").Value
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "The named navigation - " + namedNavigation + " does not exist in the state machine file!"))
            End If

            Return navigationPath
        End Function

        ''' <summary>
        ''' Get dictionary value for a particular item
        ''' </summary>
        ''' <param name="state">The state in which item is present</param>
        ''' <param name="itemName">The item name whose dictionary value is needed</param>
        ''' <returns>The dictionary value as string</returns>
        Public Function GetDictionaryValueForItem(ByVal state As EpgState, ByVal itemName As String) As String
            Dim dictionaryValue As String = ""
            'Get the dictionary key from the state object for the passed item
            Dim dictionaryKey As String = state.GetDictionaryKey(itemName)

            If IsNothing(dictionaryKey) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to get dictionary key for item - " + itemName + ".Please check your state machine file if the item exists!"))
            End If

            dictionaryValue = utils.GetValueFromDictionary(dictionaryKey)

            Return dictionaryValue
        End Function

        ''' <summary>
        ''' Load an XML File as XDocument object
        ''' </summary>
        ''' <param name="xmlFilePath">The path of the XML File as string</param>
        ''' <returns>The XDocument</returns>
        Private Function LoadXMLFile(ByVal xmlFilePath As String) As XDocument
            Dim xmlFile As New XDocument

            Try
                xmlFile = XDocument.Load(xmlFilePath)
            Catch ex As Exception
                xmlFile = Nothing
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to load XML file at path - " + xmlFilePath + " because of the following reason - " + ex.Message))
            End Try

            Return xmlFile
        End Function

    End Class

#End Region

    ''' <summary>
    '''  Stream sync utility
    ''' </summary>
    ''' <param name="EPGTime">The Current time from EPG</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para></para> 
    ''' </remarks>
    Public Overrides Sub StreamSync(ByVal EPGTime As String)
        Dim TestDuration As Integer = 0
        Dim StreamEndTime As String = ""
        Dim leftTime As Integer = 0
        Dim avgMountTime As Integer = 0
        Dim avgTearDownTime As Integer = 0

        StreamEndTime = GetValueFromEnvironment("STREAM_END_TIME")
        avgMountTime = Convert.ToInt32(GetValueFromEnvironment("AVERAGE_MOUNT_TIME"))
        avgTearDownTime = Convert.ToInt32(GetValueFromEnvironment("AVERAGE_TEARDOWN_TIME"))
        Try
            TestDuration = Convert.ToInt32(GetValueFromTestIni("IEX", "duration"))
        Catch
            _UI.Utils.LogCommentInfo("Exit StreamSync:Test Duration not apecified in Test.ini")
            Exit Sub
        End Try

        ' Workaround
        If TestDuration >= 100 Then
            TestDuration = 30
            _UI.Utils.LogCommentInfo("Test Duration in Test.Ini is wrong - So hardcoding to :" + TestDuration.ToString)
        End If
        leftTime = _UI.Utils._DateTime.Subtract(CDate(StreamEndTime), CDate(EPGTime))
        _UI.Utils.LogCommentInfo("Current EPG Time:" + EPGTime + ",Stream End Time:" + StreamEndTime + ",Test Duration:" + TestDuration.ToString + ",Stream Left Time:" + leftTime.ToString)

        ' Test duration includes max 3 mount duration + Teardown. Need to subtract them.
        If leftTime < (TestDuration - ((3 * avgMountTime) + avgTearDownTime)) Then
            _UI.Utils.LogCommentInfo("Waiting for " + leftTime.ToString + " for stream wrap")
            _iex.Wait(leftTime * 60 + 300)
        Else
            _UI.Utils.LogCommentInfo("Stream left time sufficient for test execution")
        End If
    End Sub

    ''' <summary>
    '''   Gets Value From Test.ini
    ''' </summary>
    ''' <param name="Key">The Key Of The Value Requested</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function GetValueFromTestIni(ByVal section As String, ByVal key As String) As String
        Dim _iexNumber As String = _iex.IEXServerNumber
        Dim iniFile As AMS.Profile.Ini
        iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Test.ini")
        Dim value As String = iniFile.GetValue(section, key).ToString
        If value = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "Value is empty for key " + key + " in section " + section + ".Please check your Project.ini"))
        Else
            _UI.Utils.LogCommentInfo("Retrieved Value From ini File: " + key + " = " + value)
        End If
        Return value
    End Function

    ''' <summary>
    ''' Confluence Flow Diagram Link : http://confluence.fr.nds.com/display/FC/Flow+Diagram+for+the+HighlightOption+Iterator
    ''' Highlight on one option in the list based on the criterion in the dictionary
    ''' </summary>
    ''' <param name="stateInWhichOptionIsPresent">State in which option is present</param>
    ''' <param name="dictionary">Dictionary having the key-value pair of the options to be set</param>
    ''' <returns>The Guard Time as Integer</returns>
    ''' <remarks></remarks>
    Public Overrides Function HighlightOption(ByVal stateInWhichOptionIsPresent As EpgState, ByVal dictionary As Dictionary(Of EnumEpgKeys, String)) As Boolean
        ''Key value for the Next Navigation

        Dim KeysCount As Integer = dictionary.LongCount
        Dim NavNext As String = ""
        Dim NavPrev As String = ""
        Dim ReferenecMilestones() As String = {""}
        Dim ReferencesKeys() As String = {""}
        Dim EpgKeysArray(KeysCount - 1) As String
        Dim VerifiedMilestones(KeysCount - 1) As String
        Dim CurrentOptionValues(KeysCount - 1) As String
        Dim FirstItemInTheList(KeysCount - 1) As String
        Dim firstItemInTheFirstList(KeysCount - 1) As String
        Dim Milestones As String = ""
        Dim ExactMilestones As String = ""
        Dim itr As Integer = 0
        Dim DirectionKey As String = ""
        Dim TempArrLength As Integer = 0
        Dim ItemsVerified As Integer = 0
        Dim ActualLines As New ArrayList
        Dim FoundSuccess As Boolean = True
        Dim firstItemSet As Boolean = False
        Dim SameOptionFound As Boolean = False
        Dim LayoutType As String = ""
        Dim interLineNavigation As String = ""

        '''''   Converting the dictionary into two arrays of keys and the values to be verified.
        VerifiedMilestones = {""}

        ReferenecMilestones = dictionary.Values.ToArray()
        If Not stateInWhichOptionIsPresent.IsMultiLineMenu Then
            LayoutType = stateInWhichOptionIsPresent.GetMenuLayout()
        Else
            LayoutType = stateInWhichOptionIsPresent.GetMultiLineMenuLayout()
            interLineNavigation = stateInWhichOptionIsPresent.Menus.MultiLineMenu.GetInterLineNavigation()
        End If

        Dim pair As KeyValuePair(Of EnumEpgKeys, String)
        Dim i As Integer = 0
        For Each pair In dictionary
            EpgKeysArray(i) = pair.Key.ToString.ToLower
            i = i + 1
        Next

        If LayoutType.ToLower = "updown" Then
            NavNext = "SELECT_DOWN"
            NavPrev = "SELECT_UP"
        Else
            NavNext = "SELECT_RIGHT"
            NavPrev = "SELECT_LEFT"
        End If
        DirectionKey = NavPrev

        LogCommentInfo("Entering HighlightOption()")
        LogCommentInfo("Trying to Highlight on the option:" + (dictionary.Item(EnumEpgKeys.TITLE)).ToString)

        ''''' Create a Milestones list from the dictionary.

        If (EpgKeysArray.Length) = 1 Then
            Milestones = GetValueFromMilestones("EPGMilestones" + CStr(EpgKeysArray(0)))
        Else

            Milestones = GetValueFromMilestones("EPGMilestones" + EpgKeysArray(0))

            For i = 1 To (EpgKeysArray.Length) - 1
                Milestones = Milestones + "," + GetValueFromMilestones("EPGMilestones" + EpgKeysArray(i))
            Next
        End If


        Dim CurrentEpgOption As String = ""

        '''''' Verify the on-entry Milestones to know if the 
        '''''' first option itself is the Required one.
        '''''' Also store the values corresponding to the first item in the list.
            GetEpgInfo(dictionary.Keys(i).ToString, CurrentEpgOption)
        Do

            'Do interline navigation for the second line and onwards
            If firstItemSet Then
                DirectionKey = NavPrev
                SendIR(interLineNavigation)
            End If

            FoundSuccess = True

            'Check if option is present in the beginning itself
            For i = 0 To KeysCount - 1
                Dim OptionToHighlight As String = ""

                GetEPGInfo(dictionary.Keys(i).ToString, CurrentEpgOption)
                dictionary.TryGetValue(dictionary.Keys(i), OptionToHighlight)

                FirstItemInTheList(i) = CurrentEpgOption.Trim

                If CurrentEpgOption.Trim.ToUpper.Equals(OptionToHighlight.Trim.ToUpper) Then
                    If (i = KeysCount - 1) And FoundSuccess Then
                        LogCommentInfo("Option found in the beginning!! Exiting the function")
                        Return True
                    End If
                Else
                    FoundSuccess = False
                End If

            Next

            Array.Copy(FirstItemInTheList, CurrentOptionValues, FirstItemInTheList.Length)

            'Check if the first item is not set, if set then check if we've hit the first item in the first list
            If Not firstItemSet Then
                Array.Copy(FirstItemInTheList, firstItemInTheFirstList, FirstItemInTheList.Length)
                firstItemSet = True
            Else
                For itr = 0 To (KeysCount - 1)
                    If CurrentOptionValues(itr).ToUpper.Equals(firstItemInTheFirstList(itr).ToUpper) Then
                        If (itr = KeysCount - 1) And Not SameOptionFound Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Reached End of the list without finding the item!!"))
                        End If
                    Else
                        SameOptionFound = False
                        Exit For
                    End If
                Next
            End If

            ''''' Start searching for the option in the backward direction until you reach Beginning of the list
            ''''' once beginning of the list is found, start searching the requested item in the opposite direction
            Do
                itr = 0

                If Not BeginWaitForDebugMessages(Milestones, 15) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to Begin Wait for message!"))
                End If

                _iex.Wait(2)

                SendIR(DirectionKey)

                ''''' Verify the beginning/end of the Menu list. 
                ''''' If its beginning, change the direction and continue to navigate.
                If Not EndWaitForDebugMessages(Milestones, ActualLines) Then
                    LogCommentFail("Failed to End wait for Debug Messages::" + Milestones)
                    If Not DirectionKey.Equals(NavNext) Then
                        LogCommentInfo("Assuming you hit the beginning of the Non Circular list..!! changing the navigation direction")
                        DirectionKey = NavNext
                        Continue Do
                    Else
                        If stateInWhichOptionIsPresent.IsMultiLineMenu Then
                            Exit Do
                        Else
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Reached End of the list without finding the item!!"))
                        End If
                    End If
                End If

                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                '''''Verify if the required option is found after moving to the next option.
                TempArrLength = Split(ActualLines(itr), "@#$").Length
                For itr = 0 To (KeysCount - 1)
                    CurrentOptionValues(itr) = Split(ActualLines(itr), "@#$")(TempArrLength - 1).Trim
                Next

                FoundSuccess = True
                For itr = 0 To (KeysCount - 1)
                    If CurrentOptionValues(itr).ToUpper.Equals(ReferenecMilestones(itr).ToUpper) Then

                        If (itr = KeysCount - 1) And FoundSuccess Then
                            LogCommentInfo("Required Option found::Exiting the function")
                            Return True
                        End If
                    Else
                        FoundSuccess = False
                    End If
                Next

                ''''' Verify if after the navigation, option found is same as the previous option.
                For itr = 0 To (KeysCount - 1)
                    If CurrentOptionValues(itr).ToUpper.Equals(VerifiedMilestones(itr).ToUpper) Then
                        If (itr = KeysCount - 1) And Not SameOptionFound Then
                            LogCommentInfo("Same option as the previous option is found..!! changing the direction to::" + NavNext)
                            SameOptionFound = True
                        End If
                    Else
                        SameOptionFound = False
                        Exit For
                    End If
                Next

                For itr = 0 To (KeysCount - 1)
                    If CurrentOptionValues(itr).ToUpper.Equals(FirstItemInTheList(itr).ToUpper) Then
                        If (itr = KeysCount - 1) And Not SameOptionFound Then
                            If stateInWhichOptionIsPresent.IsMultiLineMenu Then
                                Exit Do
                            Else
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Reached End of the list without finding the item!!"))
                            End If
                        End If
                    Else
                        SameOptionFound = False
                        Exit For
                    End If
                Next

                ''''' Store the values found for the option in the Beginning of 
                ''''' the list and change the Navigation direction.
                If SameOptionFound And DirectionKey.Equals(NavPrev) Then
                    DirectionKey = NavNext
                End If

                Array.Copy(CurrentOptionValues, VerifiedMilestones, CurrentOptionValues.Length)

            Loop

        Loop

        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Unable to find the Item ..!!"))

    End Function

    ''' <summary>
    ''' Function takes multiple strings in array 
    ''' Call another Function Parse Milestone Time 
    ''' Store time values in a list of interger
    ''' </summary>
    ''' <param name="logArray"></param>
    ''' <returns>List of Integers</returns>
    ''' <remarks></remarks>
    Public Overrides Function ParseMileStoneTimeFromLogArray(ByVal logArray As ArrayList) As List(Of Double)
        Dim time As New List(Of Double)

        For Each log As String In logArray
            time.Add(ParseMileStoneTime(log))
        Next

        Return time
    End Function

    ''' <summary>
    ''' Function Takes String 
    ''' Checks String's Time Stamp 
    ''' Parse and Calculates Time in Micro Seconds
    ''' </summary>
    ''' <param name="SelectionItem"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides Function ParseMileStoneTime(ByVal SelectionItem As String) As Double

        Dim _irSelectionMilestones As String()
        Dim firstTimeMilestone As Double
        Dim secondTimeMilestone As Double
        Dim _totalTime As Double

        'check if logs have arrived or not
        If String.IsNullOrEmpty(SelectionItem) Then
            _UI.Utils.LogCommentBlack("Unable to get milestones please check udp log file for the time stamp!!")
        End If
        'Logs will come in format like 0000001089.108978 IEX IR key@#$36
        'Doing Split with IEX
        'taking firstTimeMilestone before (.) and secondTimeMilestone after (.)

        _irSelectionMilestones = Split(SelectionItem, "IEX")
        Dim TimeStampFromMilestone = Split(_irSelectionMilestones(0), " ")(0)

        firstTimeMilestone = Convert.ToInt32(Split(TimeStampFromMilestone, ".")(0))
        secondTimeMilestone = Convert.ToInt32(Split(TimeStampFromMilestone, ".")(1))

        'From IEX we get Logs in Format as 0000001089.108978 after doing END WAIT for a particular log,not as time stamp as we observe from udp log file
        'when observed after taking two logs as 15:07:18.138 16Dec 0000011295.404500 IEX IR key@#$40 , 15:07:18.355 16Dec 0000011295.621403 IEX_Predicted_Channel:102
        'and parsing as mentioned it was found to take 216.903ms which is almost same as observed from time stamp of Udp Log(15:07:18.355 - 15:07:18.138)
        firstTimeMilestone = firstTimeMilestone * 1000
        secondTimeMilestone = secondTimeMilestone / 1000

        _totalTime = firstTimeMilestone + secondTimeMilestone


        Dim Msg As String = ""
        Msg = _totalTime.ToString()

        _iex.ForceHideFailure()
        If Msg <> "" Then

            _UI.Utils.LogCommentInfo("Total Time taken in milliseconds for this particular log to arrive:" & Msg)
        End If
        Return (Msg)
    End Function

    '''<summary>
    '''  Creating and Writing data in XML file at a particular location
    ''' </summary>
    Public Overrides Sub CreateandWritePerformanceResultXML(ByVal obj As Performance)

        'creating Performance.xml file inside log folder
        Dim LogPath As String = Path.GetFullPath(_iex.LogFileName)

        _UI.Utils.LogCommentInfo("Current Log Path is: " & LogPath)

        'create XML Writer Settings
        Dim settings As XmlWriterSettings = New XmlWriterSettings()
        settings.Indent = True

        'create XML writer
        Try
            Using writer As XmlWriter = XmlWriter.Create(LogPath + "Performance.xml", settings)

                'Begin Writing Element and Document
                writer.WriteStartDocument()

                writer.WriteStartElement("PerformanceData")
                writer.WriteStartElement("PerformanceType")
                writer.WriteElementString("Name", "AverageValue")
                writer.WriteElementString("Threshold", obj.Thresholdvalue)
                writer.WriteElementString("Value", obj.AverageTime)
                writer.WriteEndElement()
                writer.WriteStartElement("PerformanceType")
                writer.WriteElementString("Name", "MinimumValue")
                writer.WriteElementString("Threshold", obj.Thresholdvalue)
                writer.WriteElementString("Value", obj.MinimumTime)
                writer.WriteEndElement()
                writer.WriteStartElement("PerformanceType")
                writer.WriteElementString("Name", "MaximumValue")
                writer.WriteElementString("Threshold", obj.Thresholdvalue)
                writer.WriteElementString("Value", obj.MaximumTime)
                writer.WriteEndElement()
                writer.WriteStartElement("PerformanceType")
                writer.WriteElementString("Name", "IterationCount")
                writer.WriteElementString("Value", obj.RunningNumberOfIteration)
                writer.WriteEndElement()
                writer.WriteStartElement("PerformanceType")
                writer.WriteElementString("Name", "FirstTimeLaunchValue")
                writer.WriteElementString("Value", obj.TimeForFirstLaunchTime)
                writer.WriteEndElement()

                ' End Writing Element and Document.

                writer.WriteEndElement()
                writer.WriteEndDocument()

            End Using
        Catch ex As Exception

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Unable to create xml file or write in xml file!!"))

		End Try

    End Sub


    Private isCompleteIEXNavigation As Boolean = True


    Public Sub NavigateAndHighlight(ByVal navigationPath As String, Optional ByVal dictionary As Dictionary(Of EnumEpgKeys, String) = Nothing)

        Dim namedNavigation As String
        Dim navigationStatesArray As String()
        Dim navigationItem As String

        namedNavigation = navigationPath
        isCompleteIEXNavigation = True

        Dim state As New EpgState()
        Dim prevState As New EpgState()
        Dim sameStateAsPrevious As Boolean = False
        Dim namedNavigationPrefix As String = "STATE:"
        Try
            If namedNavigation.Contains(namedNavigationPrefix) Then
                navigationPath = EPGStateMachine.GetNavigationPath(namedNavigation)
            Else
                navigationPath = namedNavigation
            End If
        Catch
            LogCommentFail("Failed to get the full path for the Named Navigation: " & namedNavigation)
        End Try
        'split the given Navigationpath with a delimitter '/'. 

        navigationStatesArray = TrimAndReplaceNavPath(navigationPath)

        Dim navigationIndex As Integer = 0

        ' For each state in the navigation path, check if its possible to perform IEX navigation. 
        '            * if yes, continue fetching the next in the Navigation-list. 
        '            * Else, Navigate till the last possible path and for the remaining part, 
        '            * call the Highlightoption() API. 

        'navigationItem can be local - it holds different values according to context !!!

        For numberOfPossibleNavigations As Integer = 0 To navigationStatesArray.Length - 1
            navigationItem = navigationStatesArray(numberOfPossibleNavigations)

            'Check wehther the current navigation item is a state or not

            Try
                state = EPGStateMachine.GetState(navigationItem)
            Catch
                LogCommentInfo("Exception:: state is not defined for the entry:" & navigationItem)
                state = Nothing
            End Try

            If state Is Nothing Then
                'If its not a state, make sure that the item navigation is possible
                '                     * else, We need to call HighlightOption Method with 
                '                     * its parent Menu Layout type  

                Dim isIEXNavigation As Boolean = True

                Try
                    isIEXNavigation = prevState.Menu.GetNavigationType(navigationStatesArray(numberOfPossibleNavigations))
                Catch
                    LogCommentInfo("StandardNavigation is not defined for the item" + navigationItem + " in the EPGStateMachine.Assuming IEX Navigation is True in this case.")
                    isIEXNavigation = True
                End Try

                If ((Not isIEXNavigation) Or sameStateAsPrevious Or prevState.IsMultiLineMenu()) Then
                    NavigateConstructedPath(navigationIndex, numberOfPossibleNavigations, navigationStatesArray)

                    ' Fetch the layout of the current option from the previous screen 
                    ' and pass it to the HighlightOption() 
                    HighlightBasedOnTitle(prevState, navigationStatesArray(numberOfPossibleNavigations))

                    If numberOfPossibleNavigations <> navigationStatesArray.Length - 1 Then
                        'Send select to enter the state
                        SendIR("SELECT")
                    End If

                    'Increment the StartIndex value by one to make it to move to the next navigation in the list
                    navigationIndex = numberOfPossibleNavigations + 1
                End If
            ElseIf state.IsActivationCriteriaSame(prevState) Then
                sameStateAsPrevious = True

                'Navigate the part before
                NavigateConstructedPath(navigationIndex, numberOfPossibleNavigations, navigationStatesArray)

                'Highlight the required state entry and enter it
                HighlightBasedOnTitle(prevState, navigationStatesArray(numberOfPossibleNavigations))

                If numberOfPossibleNavigations <> navigationStatesArray.Length - 1 Then
                    'Send select to enter the state
                    SendIR("SELECT")
                End If

                'Increment the StartIndex value by one to make it to move to the next navigation in the list
                navigationIndex = numberOfPossibleNavigations + 1
            End If

            prevState = state
        Next

        If isCompleteIEXNavigation Then
            EPG_Milestones_Navigate(navigationPath)
        End If

        'TODO:: Add validation dictionary later if required

        LogCommentInfo("Exiting the NavigateAndHighlight() EA")

    End Sub

    Private Sub NavigateConstructedPath(ByVal navigationIndex As Integer, ByVal numberOfPossibleNavigations As Integer, ByVal navigationStatesArray As String())
        isCompleteIEXNavigation = False

        ' Constructing the Navigation path to pass it to EPG_Milestones_Navigate() API.
        Dim navigationItem As String = navigationStatesArray(navigationIndex)

        'The first entry is store before the loop so that it is easy to append slash
        Dim newNavigationPath As String = navigationItem

        'Constructing IEX navigating path for the items that will use IEX navigate
        For j As Integer = navigationIndex + 1 To numberOfPossibleNavigations - 1
            navigationItem = navigationStatesArray(j)

            newNavigationPath = newNavigationPath & "/" & navigationItem
        Next

        'EDGE CASE - Navigate using IEX Navigate call only if there is at 
        '             * least one Navigation possible in the list of Navigations

        If navigationIndex <> numberOfPossibleNavigations Then
            EPG_Milestones_Navigate(newNavigationPath)
        End If
    End Sub

    Private Sub HighlightBasedOnTitle(ByVal state As EpgState, ByVal navigationState As String)
        Dim dictionaryValue As [String] = ""
        Dim parentScrnDictionary As New Dictionary(Of EnumEpgKeys, String)()

        Try
            dictionaryValue = EPGStateMachine.GetDictionaryValueForItem(state, navigationState)
        Catch
            LogCommentWarning("Dictionary value is not defined for the " & navigationState & ". Taking item name instead.")
            dictionaryValue = navigationState
        End Try

        parentScrnDictionary.Clear()
        parentScrnDictionary.Add(EnumEpgKeys.TITLE, dictionaryValue)

        HighlightOption(state, parentScrnDictionary)
    End Sub

    Private Function TrimAndReplaceNavPath(ByVal navigationPath As String) As String()
        Dim navigationPathArray As String() = {""}

        LogCommentInfo("Entering the TrimAndReplaceNavPath() function. Input string::" & navigationPath)

        If navigationPath.Contains("//") Then
            navigationPath = navigationPath.Replace("//", "$")
        End If

        navigationPathArray = navigationPath.Split("/"c)

        For itr As Integer = 0 To navigationPathArray.Length - 1
            If navigationPathArray(itr).Contains("$") Then
                navigationPathArray(itr) = navigationPathArray(itr).Replace("$", "//")
            End If
        Next

        LogCommentInfo("Exiting the TrimAndReplaceNavPath() function")
        Return navigationPathArray
    End Function
    Public Sub NavigateToDiagnostics()

        Dim title As String = ""
        _iex.SendIRCommand("MENU")
        _iex.Wait(1)
        For itr1 As Integer = 0 To 6
            _iex.SendIRCommand("SELECT_LEFT")
            _iex.Wait(2)
            GetEpgInfo("title", title)
            If title.ToUpper().StartsWith("PREFERENCES") Then
                Exit For
            End If

        Next

        For itr As Integer = 0 To 10
            _iex.SendIRCommand("SELECT_DOWN")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().Contains("DIAGNOSTICS") Then
                _iex.SendIRCommand("SELECT")
                _iex.Wait(5)
                Exit For
            End If
        Next

    End Sub
	
	 Public Sub NavigateToFactoryReset()
        Dim title As String = ""
        _iex.SendIRCommand("MENU")
        _iex.Wait(1)

        For itr1 As Integer = 0 To 6
            _iex.SendIRCommand("SELECT_LEFT")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().StartsWith("PREFERENCES") Then
                Exit For
            End If

        Next
        For itr As Integer = 0 To 10
            _iex.SendIRCommand("SELECT_DOWN")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().Contains("ADVANCED SETTINGS") Then
                _iex.SendIRCommand("SELECT")
                _iex.Wait(5)
                Exit For
            End If
        Next

        For itr2 As Integer = 0 To 5
            _iex.SendIRCommand("SELECT_DOWN")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().Contains("HORIZON BOX") Then
                _iex.SendIRCommand("SELECT")
                _iex.Wait(5)
                Exit For
            End If
        Next

        For itr3 As Integer = 0 To 10
            _iex.SendIRCommand("SELECT_DOWN")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().Contains("FACTORY RESET") Then
                _iex.SendIRCommand("SELECT")
                _iex.Wait(5)
                Exit For
            End If
        Next

    End Sub
	
	 Public Sub NavigateToCheckForUpdates()

        Dim title As String = ""
        _iex.SendIRCommand("MENU")
        _iex.Wait(1)
        For itr1 As Integer = 0 To 6
            _iex.SendIRCommand("SELECT_LEFT")
            _iex.Wait(2)
            GetEpgInfo("title", title)
            If title.ToUpper().StartsWith("PREFERENCES") Then
                Exit For
            End If

        Next

        For itr As Integer = 0 To 10
            _iex.SendIRCommand("SELECT_DOWN")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().Contains("ADVANCED SETTINGS") Then
                _iex.SendIRCommand("SELECT")
                _iex.Wait(5)
                Exit For
            End If
        Next

        For itr2 As Integer = 0 To 5
            _iex.SendIRCommand("SELECT_DOWN")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().Contains("HORIZON BOX") Then
                _iex.SendIRCommand("SELECT")
                _iex.Wait(5)
                Exit For
            End If
        Next

        For itr3 As Integer = 0 To 10
            _iex.SendIRCommand("SELECT_DOWN")
            _iex.Wait(1)
            GetEpgInfo("title", title)
            If title.ToUpper().Contains("CHECK FOR UPDATES") Then
                _iex.SendIRCommand("SELECT")
                _iex.Wait(5)
                Exit For
            End If
        Next

    End Sub
	''' <summary>
    '''   sets and Unsets the PersonalizedRecommendationActivation Flag
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function SetPersonalizedRecommendationActivation(ByVal setoption As String) As Boolean
        Dim title As String = ""
        Try
            _iex.MilestonesEPG.NavigateByName("STATE:SUGGESTED SETTINGS")
            EnterPin(PIN:="")
            _iex.Wait(2)
             For itr As Integer = 0 To 2

                GetEpgInfo("title", title)
                If (title = setoption) Then
                    _iex.SendIRCommand("SELECT")
                    _iex.Wait(5)
                    Exit For
                End If
                _iex.SendIRCommand("SELECT_DOWN")
                _iex.Wait(1)
            Next
			
            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function
	
	 ''' <summary>
    '''   Navigates To Customer Care FAQ
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function NavigateToCustomerCareFAQ(ByVal customerCareOption As String) As String
        Dim res As IEXGateway._IEXResult
        Dim expectedFAQString As String = ""
        Dim FAQDictionaryFile As String = ""
        Dim _iexNumber As String = _iex.IEXServerNumber
        Try
            Dim RFFeed As String = ""
            RFFeed = GetValueFromTestIni("IEX", "rf_port")
            If (RFFeed.ToUpper = "NL") Then
                FAQDictionaryFile = "gw_help_faq_eng-nld.xml"
            Else
                FAQDictionaryFile = "gw_help_faq_eng-deu.xml"
            End If
            Dim HelpXML = XDocument.Load("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Dictionary\" + FAQDictionaryFile)
            Dim HelpXMLDoc As New XmlDocument()
            Using xmlReader = HelpXML.CreateReader()
                HelpXMLDoc.Load(xmlReader)
            End Using
            Dim innerXML As String = HelpXMLDoc.SelectSingleNode("root/category[@name='" + customerCareOption + "']").InnerXml
            res = _iex.MilestonesEPG.Navigate(customerCareOption)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(res))
            End If
            Dim ishelpitem As Boolean = False
            While ishelpitem = False
                Dim mydocument As XDocument = XDocument.Parse(innerXML)
                Dim InnerHelpXML As New XmlDocument()
                Using xmlReader = mydocument.CreateReader()
                    InnerHelpXML.Load(xmlReader)
                End Using
                If innerXML.StartsWith("<category") Then
                    ishelpitem = False

                    Dim Category As String = InnerHelpXML.SelectSingleNode("category/@name").Value

                    innerXML = InnerHelpXML.SelectSingleNode((Convert.ToString("category[@name='") & Category) + "']").InnerXml
                    Dim obtainedTitle As String = ""
                    GetEpgInfo("title", obtainedTitle)
                    If (obtainedTitle = Category) Then
                        SendIR("SELECT")
                        _iex.Wait(2)
                    Else
                        LogCommentFail("Failed to Verify obtained title from EPG " + obtainedTitle + " is same as expected from the FAQ Dictionary" + Category)
                        Return ""
                        Exit Function
                    End If
                Else
                    ishelpitem = True
                    Dim helpItem As String = InnerHelpXML.SelectSingleNode("help_item/@name").Value
                    Dim obtainedTitle As String = ""
                    GetEpgInfo("title", obtainedTitle)
                    If (obtainedTitle = helpItem) Then
                        SendIR("SELECT")
                        _iex.Wait(2)
                    Else
                        LogCommentFail("Failed to Verify obtained title from EPG " + obtainedTitle + " is same as expected from the FAQ Dictionary" + helpItem)
                        Return ""
                        Exit Function
                    End If
                    expectedFAQString = InnerHelpXML.SelectSingleNode((Convert.ToString("help_item[@name='") & helpItem) + "']").InnerText
                End If
            End While
            Return expectedFAQString
        Catch ex As Exception
            LogCommentFail("Error Occurred while Navigating to the FAQ in " + customerCareOption)
            Return ""
        End Try

    End Function

'''' <summary>
    ''''   Verify HDD Usage Indicator in my recording & My Planner
    '''' </summary>
    ''''  <returns>String</returns>
    '''' <remarks>
    '''' Possible Error Codes:
    ''''   <par> Epginfo null </para> 
    '''' </remarks>
    ''' 
    Overrides Sub VerifyHDDIndicator(ByVal isVisible As Boolean)
Try
        Dim HDDIndicator As String = ""
        ' getting HDD indicator is enabled or not & verify indicator color
        GetEpgInfo("disk usage enabled", HDDIndicator)
        _iex.Wait(1)
        If HDDIndicator.ToUpper() = isVisible.ToString().ToUpper() Then

            LogCommentInfo("Verified HDD Indicator is " & isVisible.ToString())
        Else
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify HDD Indicator is " & isVisible.ToString() & ". The disk usage enabled milestone value is " & HDDIndicator))
        End If
       
 If HDDIndicator.ToUpper() = "TRUE" Then
                Dim hddIndicatorColor As String = ""
                GetEpgInfo("color", hddIndicatorColor)
                _iex.Wait(1)

               

                Dim hddUsageText As String

                GetEpgInfo("hdd usage text", hddUsageText)
                _iex.Wait(1)
                If hddUsageText.Trim().ToUpper() = "FULL" Then
                    LogCommentInfo("Verified HDD Usage Text")
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify HDD Indicator Text"))
                End If

               
            End If
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, ex.Message))

        End Try



    End Sub
    '''' <summary>
    ''''   Get  HDD Usage Indicator  percentage in my recording & My Planner
    '''' </summary>
    ''''  <returns>String</returns>
    '''' <remarks>
    '''' Possible Error Codes:
    ''''   <par> Epginfo null </para> 
    '''' </remarks>
    ''' 
    Overrides Function GetHDDUsagePercentage(Optional ByVal isClearEPG As Boolean = True) As Integer
Try

            Dim hddUsagePercent As String = ""
        If isClearEPG = True Then
                ClearEPGInfo()
                _iex.Wait(4)
            End If
        GetEpgInfo("Occupied disk space", hddUsagePercent)
        _iex.Wait(2)
        Return Convert.ToInt32(hddUsagePercent)
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, ex.Message))
            Return 0
        End Try
    End Function


End Class


