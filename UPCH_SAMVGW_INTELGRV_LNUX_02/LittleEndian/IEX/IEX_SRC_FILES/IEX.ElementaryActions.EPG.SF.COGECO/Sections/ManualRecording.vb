Imports FailuresHandler

Public Class ManualRecording
    Inherits IEX.ElementaryActions.EPG.SF.ManualRecording

    Dim _UI As UI
    Private CurrentMenuOnManual As String = ""
    Private SelectedChannel As String = ""
    Private SelectedDate As String = ""
    Private SelectedFrequency As String = ""
    Private SelectedStartTime As String = ""
    Private SelectedEndTime As String = ""

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Getting Selected Channel Name From Manual Recording 
    ''' </summary>
    ''' <param name="ChannelName">Returns The Selected Channel Name</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetSelectedChannelName(ByRef ChannelName As String)
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Selected Channel Name")

        Try

            Dim ChName As String = ""


            _UI.Utils.GetEpgInfo("chName", ChName)

            ChannelName = ChName

            Msg = "Selected Channel Name : " + ChannelName

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Setting Date In Date List
    ''' </summary>
    ''' <param name="tDate">Requested Date To Set</param>
    ''' <param name="DefaultValue">If True Only For Logging Purposes Writes Default Value</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetDate(ByVal tDate As String, ByVal DefaultValue As Boolean)
        Dim EpgText As String = ""

        _UI.Utils.StartHideFailures("Setting Date To " + tDate.ToUpper.ToString)

        Try

            _UI.Menu.SetManualRecordingDate(tDate.ToUpper)

            Dim stateid As String = ""

            _UI.Utils.SendIR("SELECT", 4000)

            _UI.Utils.GetEpgInfo("stateid", stateid)

            If stateid <> "51" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify StateId Is 51"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try


    End Sub

    ''' <summary>
    '''   Verifying After Manual Recording Booked : If Current Verifies Action Bar Is Not On Screen Else Verifies Got Back To Planner Menu
    ''' </summary>
    ''' <param name="IsFromCurrent">If True Means Manual Recording From Banner Else From Planner</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifySaveAndEndFinished(ByVal IsFromCurrent As Boolean)
        Dim LibErrVerified As Boolean = False
        Dim ArchiveVerified As Boolean = False
        Dim PlannerVerified As Boolean = False

        _UI.Utils.StartHideFailures("Verifying Manual Record Is Finished Booking IsFromCurrent=" + IsFromCurrent.ToString)

        Try
            If IsFromCurrent Then
                If Not _UI.Utils.VerifyState("ACTION BAR", 2) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is ACTION BAR"))
                End If
            Else
                Dim Status As String = ""


                If Not _UI.Utils.VerifyState("LIBRARY ERROR") Then
                    LibErrVerified = False
                Else
                    LibErrVerified = True
                End If

                If Not _UI.Utils.VerifyState("MY PLANNER") Then
                    PlannerVerified = False
                Else
                    PlannerVerified = True
                End If

                If Not _UI.Utils.VerifyState("MY RECORDINGS") Then
                    ArchiveVerified = False
                Else
                    ArchiveVerified = True
                End If

                If ArchiveVerified = False And LibErrVerified = False And PlannerVerified = False Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Manual Recording Confirmation Menu Is NOT On Screen"))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Manual Recording
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate(Optional ByVal FromCurrent As Boolean = False, Optional ByVal NoEIT As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To Manual Recording From Banner")

        Try
            If FromCurrent Then
                If NoEIT Then
                    _UI.Utils.EPG_Milestones_Navigate("ACTION BAR/RECORD")
                Else
                    _UI.Utils.EPG_Milestones_NavigateByName("STATE:ADD EXTRA TIME")
                End If
            Else
                _UI.Utils.EPG_Milestones_NavigateByName("STATE:MANUAL RECORDING")
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
    ''' <summary>
    '''   Navigating To Manual Recording From Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateFromPlanner()

        _UI.Utils.StartHideFailures("Navigating To Manual Recording From Planner")

        Try
            _UI.Utils.EPG_Milestones_NavigateByName("STATE:MANUAL RECORDING")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To RECORD After Entering All Fields On Manual Recording
    ''' </summary>
    ''' <param name="IsFromCurrent">For UPC: If True Means Manual Recording From Banner Or Modify Manual Else False</param>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToRecord(ByVal IsFromCurrent As Boolean)
        _UI.Utils.StartHideFailures("Navigating To CONFIRM On Manual Recording")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("CONFIRM")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Navigating To CHANNELS On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToChannel(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To CHANNEL On Manual Recording Menu")

        Try
            If isModify Then
                _UI.Utils.EPG_Milestones_Navigate("CHANNEL")
            Else
                VerifyStateId("54")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To DATE On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToDate(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To DATE On Manual Recording Menu")

        Try
            If isModify Then
                _UI.Utils.EPG_Milestones_Navigate("DATE")
            Else
                VerifyStateId("50")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To START TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToStartTime(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To START TIME On Manual Recording Menu")

        Try
            If isModify Then
                _UI.Utils.EPG_Milestones_Navigate("START TIME")
            Else
                VerifyStateId("51")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To END TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToEndTime(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To END TIME On Manual Recording Menu")

        Try
            If isModify Then
                _UI.Utils.EPG_Milestones_Navigate("END TIME")
            Else
                VerifyStateId("52")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To FREQUENCY On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToFrequency(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To FREQUENCY On Manual Recording Menu")

        Try
            If isModify Then
                _UI.Utils.EPG_Milestones_Navigate("FREQUENCY")
            Else
                VerifyStateId("53")
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Setting Start Time In Start Time Field
    ''' </summary>
    ''' <param name="StartTime">Requested Start Time To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetStartTime(ByVal StartTime As String)

        _UI.Utils.StartHideFailures("Setting Start Time To " + StartTime.ToString)
        Try
            Dim _startTime As DateTime = Convert.ToDateTime(StartTime)
            SetHour(_startTime)
            SetMinutes(_startTime.Minute)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting End Time In End Time Field
    ''' </summary>
    ''' <param name="EndTime">Requested End Time To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetEndTime(ByVal EndTime As String)

        _UI.Utils.StartHideFailures("Setting End Time To " + EndTime.ToString)

        Try
            Dim _endTime As DateTime = Convert.ToDateTime(EndTime)
            SetHour(_endTime)
            SetMinutes(_endTime.Minute)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Sets Manual Recording Hour On List
    ''' </summary>
    ''' <param name="startTime">Requested Hour</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>315 - SetManualRecordingParamFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetHour(ByVal StartTime As DateTime)

        _UI.Utils.StartHideFailures("Setting Manual Recording Hour To -> " + StartTime.ToString("hh"))

        Try
            Dim CurrentHour As String = ""
            Dim CheckedHour As String = "Empty"
            Dim FirstHour As String = ""
            Dim Passed As Boolean = False
            Dim Hour As Integer = Convert.ToInt32(StartTime.ToString("hh"))
            Dim HourFormatToSet As String = StartTime.ToString("tt")
            Dim FirstHourFormat As String = ""
            Dim CurrentHourFormat As String = ""
            Dim CheckedHourFormat As String = "Empty"
            Dim TimeDiff As Integer
            Dim IrKey As String = ""

            _UI.Utils.GetEpgInfo("selected hour", FirstHour)
            _UI.Utils.GetEpgInfo("hourFormat", FirstHourFormat)
            
            If FirstHour = Hour.ToString And FirstHourFormat.Trim.ToUpper().Equals(HourFormatToSet) Then
                _iex.Wait(2)
                _iex.ForceHideFailure()
                Passed = True
            End If

            TimeDiff = Convert.ToInt32(StartTime.ToString("HH")) - (IIf(FirstHourFormat.Trim.ToUpper().Equals("PM"), Convert.ToInt32(FirstHour) + 12, Convert.ToInt32(FirstHour)))

            If Math.Abs(TimeDiff) > 12 Then
                IrKey = (IIf(TimeDiff > 0, "SELECT_UP", "SELECT_DOWN"))
            Else
                IrKey = (IIf(TimeDiff > 0, "SELECT_DOWN", "SELECT_UP"))
            End If


            If Passed = False Then
                Dim StopAtWarning As Boolean

                StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

                'The For Loop is just for workarround. for eg: If the last "hourformat" milestone was PM and then navigate back from MENU,if first hour format is AM in screen, the milestone receive for "houFormat" is PM.
                ' This For loop will handle this issue to check for 2 time for the desired time
                For i As Integer = 0 To 1
                    CheckedHour = ""
                    CheckedHourFormat = ""
                    Do Until FirstHour = CheckedHour And FirstHourFormat.Trim.ToUpper().Equals(CheckedHourFormat.Trim.ToUpper())

                    _UI.Utils.GetEpgInfo("selected hour", CurrentHour)

                        _UI.Utils.SendIR(IrKey, 1000)

                    _UI.Utils.GetEpgInfo("selected hour", CheckedHour)
                    _UI.Utils.GetEpgInfo("hourFormat", CheckedHourFormat)

                        If CurrentHour = CheckedHour Then
                            If StopAtWarning Then
                                ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                            End If
                        End If
                        If CheckedHour = Hour.ToString And CheckedHourFormat.Trim.ToUpper().Equals(HourFormatToSet) Then
                            _iex.ForceHideFailure()
                            Passed = True
                            Exit Do
                        End If
                    Loop
                    If Passed Then
                        Exit For
                    End If
                Next
            End If


            If Passed Then
                _UI.Utils.SendIR("SELECT")
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetManualRecordingParamFailure, "Failed To Set Manual Recording Hour To : " + Hour))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Sets Manual Recording Minutes On List
    ''' </summary>
    ''' <param name="Minutes">Requested Minutes</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>315 - SetManualRecordingParamFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetMinutes(ByVal Minutes As String)
        _UI.Utils.StartHideFailures("Setting Manual Recording Minutes To -> " + Minutes.ToString)

        Try
            Dim CurrentMinutes As String = ""
            Dim CheckedMinutes As String = "Empty"
            Dim FirstMinutes As String = ""
            Dim Passed As Boolean = False
            Dim TimeDiff As Integer
            Dim IrKey As String = ""

            _UI.Utils.GetEpgInfo("selected minute", FirstMinutes)

            If FirstMinutes = Minutes Then
                _iex.Wait(2)
                _iex.ForceHideFailure()
                Passed = True
            End If

            TimeDiff = Convert.ToInt32(Minutes) - Convert.ToInt32(FirstMinutes)

            If Math.Abs(TimeDiff) > 30 Then
                IrKey = (IIf(TimeDiff > 0, "SELECT_UP", "SELECT_DOWN"))
            Else
                IrKey = (IIf(TimeDiff > 0, "SELECT_DOWN", "SELECT_UP"))
            End If

            If Passed = False Then
                Dim StopAtWarning As Boolean

                StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

                Do Until FirstMinutes = CheckedMinutes

                    _UI.Utils.GetEpgInfo("selected minute", CurrentMinutes)

                    _UI.Utils.SendIR(IrKey, 1000)

                    _UI.Utils.GetEpgInfo("selected minute", CheckedMinutes)

                    If CurrentMinutes = CheckedMinutes Then
                        If StopAtWarning Then
                            ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                        End If
                    End If
                    If CheckedMinutes = Minutes Then
                        _iex.ForceHideFailure()
                        Passed = True
                        Exit Do
                    End If
                Loop
            End If

            If Passed Then
                _UI.Utils.SendIR("SELECT")
                Exit Sub
            End If


            ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetManualRecordingParamFailure, "Failed To Set Manual Recording Minutes To : " + Minutes))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Setting Channel In Channels List
    ''' </summary>
    ''' <param name="ChannelName">Requested Channel Name To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetChannel(ByVal ChannelName As String)

        _UI.Utils.StartHideFailures("Setting Channel To " + ChannelName.ToString)

        Try
            _UI.Menu.SetManualRecordingChannel(ChannelName)

            Dim data As String = ""

            _UI.Utils.SendIR("SELECT", 4000)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting Frequency In Frequency List
    ''' </summary>
    ''' <param name="Frequency">Requested Frequency To Set : One Time,Daily,Weekday,Weekly,Saturday-Sun</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetFrequency(ByVal Frequency As String)
        Dim FirstTitle As String = ""

        _UI.Utils.StartHideFailures("Setting Frequency To " + Frequency.ToString)

        Try
            FirstTitle = _UI.Utils.GetValueFromDictionary("DIC_ONE_TIME")

            Dim tFrequency As String = ""

            _UI.Utils.GetEpgInfo("title", tFrequency)

            If tFrequency <> FirstTitle Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Manual Recording Frequency Menu On Screen"))
            End If

            _UI.Menu.SetSettingsMenuAction(Frequency)

            _UI.Utils.SendIR("SELECT", 4000)

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verify State On Manual Recording
    ''' </summary>
    ''' <param name="State">ByRef State</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>323 - VerifyStateFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyStateId(ByVal State As String)
        Dim TmpState As String = ""

        _UI.Utils.StartHideFailures("Verifying StateId Is : " + State)

        Try
            _UI.Utils.GetEpgInfo("stateid", TmpState)

            If State = TmpState Then
                Exit Sub
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify StateId Is : " + State))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
End Class
