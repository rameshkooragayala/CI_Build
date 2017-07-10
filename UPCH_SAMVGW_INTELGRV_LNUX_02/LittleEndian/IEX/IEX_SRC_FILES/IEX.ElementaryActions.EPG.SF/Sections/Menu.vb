Imports FailuresHandler

Public Class Menu
    Inherits IEX.ElementaryActions.EPG.Menu

    Dim _UI As UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

#Region "Menu"
    ''' <summary>
    '''    Navigating To Menu By Pressing Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        _Utils.StartHideFailures("Navigating To Menu")

        Try
            If IsMenu() Then
                Exit Sub
            End If

            _Utils.EPG_Milestones_Navigate("MAIN MENU")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checks If The Menu Is On The Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function IsMenu() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Menu Is On The Screen")

        Try
            If _Utils.VerifyState("MAIN MENU", 2) Then
                Msg = "Menu Is On Screen"
                Return True
            Else
                Msg = "Menu Is Not On Screen"
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
    '''  Gets Channel Name From MENU
    ''' </summary>
    ''' <param name="ChannelName">Byref Returns The Channel Name</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Overrides Sub GetMenuChannelName(ByRef ChannelName As String)
        Dim Msg As String = ""

        _Utils.StartHideFailures("Getting Channel Name From Menu")

        Try
            _Utils.GetEpgInfo("chname", ChannelName)
            Msg = "Menu Channel Name : " + ChannelName
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Gets Channel Number From EPG Milestones Only When Activating The MENU
    ''' </summary>
    ''' <param name="ChannelNumber">The Returned Channel Number</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Overrides Sub GetChannelNumber(ByRef ChannelNumber As String)
        Dim Msg As String = ""
        _Utils.StartHideFailures("Getting Channel Number From Menu")

        Try
            Navigate()

            _Utils.GetEpgInfo("chnum", ChannelNumber)

            Msg = "Menu Channel Number : " + ChannelNumber

			_iex.SendIRCommand("MENU")
			 
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub


    ''' <summary>
    '''   Pressing SELECT And Waiting For Conflict Menu To Appear
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>344 - ConflictFailure</para> 
    ''' </remarks>
    Public Overrides Sub SelectToConflict()
        Dim Msg As String = ""

        _Utils.StartHideFailures("Trying To Raise Conflict")

        Try
            Dim ReturnedValue As String = ""

            _UI.Utils.SendIR("SELECT")

            _UI.Utils.GetEpgInfo("state", ReturnedValue)

            If ReturnedValue = "PvrWarningOptionState" Then
                Msg = "Verified Conflict Is On Screen"
                Exit Sub
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ConflictFailure, "Failed To Raise Conflict"))
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

#End Region

#Region "Settings Menu"

    ''' <summary>
    '''   Sets Settings Menu Item 
    ''' </summary>
    ''' <param name="Action">Requested Action</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetSettingsMenuAction(ByVal Action As String)

        _Utils.StartHideFailures("Set Setting Menu Action To -> " + Action.ToString)

        Try
            Dim CurSettings As String = ""
            Dim CheckedSettings As String = "Empty"
            Dim FirstSettings As String = ""
            Dim SameItemTimes As Integer = 3

            _Utils.GetEpgInfo("title", FirstSettings)

            If FirstSettings = Action Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstSettings = CheckedSettings And (SameItemTimes = 2 Or SameItemTimes = 0)

                _Utils.GetEpgInfo("title", CurSettings)

                _Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", CheckedSettings)

                If CurSettings = CheckedSettings Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN SETTINGS ACTION SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedSettings = Action Then
                    Exit Sub
                End If

            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Setting Menu Action To : " + Action.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

#End Region

#Region "ActionBar Menu"

    ''' <summary>
    '''   Sets Action Bar Sub Menu Action
    ''' </summary>
    ''' <param name="Action">Requested Action</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Overrides Sub SetActionBarSubAction(ByVal Action As String)
        Dim CurrentAction As String = ""
        Dim IR As String = ""
        Dim Times As Integer = 0

        _Utils.StartHideFailures("Setting Action Bar Sub Menu Action To -> " + Action.ToString)

        Try
            Dim CurrentItem As String = "Empty"
            Dim FirstItem As String = ""
            Dim LastItem As String = ""

            _Utils.GetEpgInfo("title", FirstItem)

            If FirstItem = Action Then
                _iex.Wait(2)
                Exit Sub
            End If

            Times = 0

            Do Until (CurrentItem = FirstItem) Or (CurrentItem = LastItem) Or Times = 15

                LastItem = CurrentItem

                _Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", CurrentItem)

                If CurrentItem = Action Then
                    Exit Sub
                End If

                Times += 1
            Loop

            FirstItem = CurrentItem
            CurrentItem = "Empty"
            LastItem = ""
            Times = 0

            Do Until (CurrentItem = FirstItem) Or (CurrentItem = LastItem) Or Times = 15

                LastItem = CurrentItem

                _Utils.SendIR("SELECT_UP")

                _Utils.GetEpgInfo("title", CurrentItem)

                If CurrentItem = Action Then
                    Exit Sub
                End If

                Times += 1
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Action Bar Sub Menu Action To : " + Action.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

#End Region

#Region "Library Menu"

    ''' <summary>
    '''   Checks If Library No Content Menu Is On Screen
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsLibraryNoContent() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Library No Content Menu Is On The Screen")

        Try
            If _Utils.VerifyState("LIBRARY ERROR", 10) Then
                Msg = "Library No Content Is On Screen"
                Return True
            Else
                Msg = "Library No Content Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

#End Region

#Region "ManualRecording Menu"

    ''' <summary>
    '''   Sets Manual Recording Date On Date List
    ''' </summary>
    ''' <param name="tDate">Requested Date</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetManualRecordingDate(ByVal tDate As String)

        _Utils.StartHideFailures("Setting Manual Recording Date To -> " + tDate.ToString)

        Try
            Dim CurrentDate As String = ""
            Dim CheckedDate As String = "Empty"
            Dim FirstDate As String = ""
            Dim SameItemTimes As Integer = 3

            _Utils.GetEpgInfo("title", FirstDate)

            If FirstDate = tDate Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstDate = CheckedDate And (SameItemTimes = 2 Or SameItemTimes = 0)
                _Utils.GetEpgInfo("title", CurrentDate)

                _Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", CheckedDate)

                If CurrentDate = CheckedDate Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN DATE SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedDate = tDate Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Date Is : " + tDate))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''  Sets Manual Recording Channel On Channels List
    ''' </summary>
    ''' <param name="Channel">Requested Channel</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Overrides Sub SetManualRecordingChannel(ByVal Channel As String)
        _Utils.StartHideFailures("Setting Manual Recording Channel To -> " + Channel.ToString)

        Try
            Dim CurrentChannel As String = ""
            Dim CheckedChannel As String = "Empty"
            Dim FirstChannel As String = ""
            Dim SameItemTimes As Integer = 3

            _Utils.GetEpgInfo("title", FirstChannel)

            If FirstChannel = Channel Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstChannel = CheckedChannel And (SameItemTimes = 2 Or SameItemTimes = 0)

                _Utils.GetEpgInfo("title", CurrentChannel)

                _UI.Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", CheckedChannel)

                If CurrentChannel = CheckedChannel Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN CHANNEL SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedChannel = Channel Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Manual Recording Channel To : " + Channel))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub
#End Region

#Region "Conflict Menu"

    ''' <summary>
    '''   Sets Action On Conflict Menu
    ''' </summary>
    ''' <param name="Action">Requested Action</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Overrides Sub SetConflictAction(ByVal Action As String)

        _Utils.StartHideFailures("Setting Conflict Menu Action To -> " + Action.ToString)

        Try
            Dim CurrentAction As String = ""
            Dim CheckedAction As String = "Empty"
            Dim FirstAction As String = ""
            Dim SameItemTimes As Integer = 3

            _Utils.GetEpgInfo("title", FirstAction)

            If FirstAction = Action Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstAction = CheckedAction And (SameItemTimes = 2 Or SameItemTimes = 0)

                _Utils.GetEpgInfo("title", CurrentAction)

                _Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", CheckedAction)

                If CurrentAction = CheckedAction Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN CONFLICT ACTION SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedAction = Action Then
                    Exit Sub
                End If
            Loop


            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Conflict Menu Action To : " + Action.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


#End Region

#Region "Language Menu"

    ''' <summary>
    '''   Sets Language On EPG
    ''' </summary>
    ''' <param name="Language">Requested Language</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Overrides Sub SetLanguage(ByVal Language As String)

        _Utils.StartHideFailures("Setting EPG Language To -> " + Language.ToString)

        Try
            Dim CurrentLanguage As String = ""
            Dim CheckedLanguage As String = "Empty"
            Dim FirstLanguage As String = ""
            Dim SameItemTimes As Integer = 3

            _Utils.GetEpgInfo("title", FirstLanguage)

            If FirstLanguage.ToUpper = Language.ToUpper Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstLanguage = CheckedLanguage And (SameItemTimes = 2 Or SameItemTimes = 0)

                _Utils.GetEpgInfo("title", CurrentLanguage)

                _Utils.SendIR("SELECT_UP")

                _Utils.GetEpgInfo("title", CheckedLanguage)

                If CurrentLanguage.ToUpper = CheckedLanguage.ToUpper Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_UP LANGUAGE SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedLanguage.ToUpper = Language.ToUpper Then
                    Exit Sub
                End If

            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Language To -> " + Language.ToUpper))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


#End Region

#Region "Country Menu"

    ''' <summary>
    '''   Sets Country On EPG
    ''' </summary>
    ''' <param name="Country">Requested Country</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Overrides Sub SetCountry(ByVal Country As String)

        _Utils.StartHideFailures("Setting EPG Country To -> " + Country.ToString)

        Try
            Dim CurrentCountry As String = ""
            Dim CheckedCountry As String = "Empty"
            Dim FirstCountry As String = ""
            Dim SameItemTimes As Integer = 3

            _Utils.GetEpgInfo("title", FirstCountry)

            If FirstCountry.ToUpper = Country.ToUpper Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstCountry = CheckedCountry And (SameItemTimes = 2 Or SameItemTimes = 0)

                _Utils.GetEpgInfo("title", CurrentCountry)

                _Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", CheckedCountry)

                If CurrentCountry.ToUpper = CheckedCountry.ToUpper Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_UP COUNTRY SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedCountry.ToUpper = Country.ToUpper Then
                    Exit Sub
                End If

            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Country To -> " + Country.ToUpper))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


#End Region
#Region "Confirmation Menu"
    ''' <summary>
    '''   Sets Action On Confirmation Menu
    ''' </summary>
    ''' <param name="Action">Requested Action</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetConfirmationMenu(ByVal Action As String)

        _Utils.StartHideFailures("Setting Confirmation Menu To -> " + Action)

        Try
            Dim CurrentAction As String = ""
            Dim CheckedAction As String = "Empty"
            Dim FirstAction As String = ""
            Dim SameItemTimes As Integer = 3

            _Utils.GetEpgInfo("title", FirstAction)

            If FirstAction = Action Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstAction = CheckedAction And (SameItemTimes = 2 Or SameItemTimes = 0)

                _Utils.GetEpgInfo("title", CurrentAction)

                _Utils.SendIR("SELECT_DOWN")

                _Utils.GetEpgInfo("title", CheckedAction)

                If CurrentAction = CheckedAction Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(ExitCodes.IRVerificationFailure, "SELECT_DOWN Confirm Action Is Same As Before IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN CONFIRM ACTION SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedAction = Action Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Confirmation Menu Action To : " + Action.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

#End Region

End Class
