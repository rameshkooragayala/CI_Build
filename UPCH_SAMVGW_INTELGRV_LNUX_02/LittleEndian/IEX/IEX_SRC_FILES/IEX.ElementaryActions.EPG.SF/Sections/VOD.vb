Imports FailuresHandler
Imports System.Text.RegularExpressions

Public Class VOD
    Inherits IEX.ElementaryActions.EPG.VOD

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigates To VOD Search Screen
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        _UI.Utils.StartHideFailures("Navigating To VOD Search")

        Try
            _UI.Utils.EPG_Milestones_NavigateByName("STATE:VOD SEARCH")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To Event Name On VOD Search
    ''' </summary>
    ''' <param name="EventName">The Request Event Name</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToEventName(ByVal EventName As String)
        Dim CharsArray As Char() = EventName.ToCharArray
        Dim CurrentLetter As String = "Empty"
        Dim FoundEvent As String = ""
        Dim OriginalEventName As String = EventName.ToUpper
        Dim FirstLetter As String = ""
        Dim FirstLoop As Boolean = True

        _UI.Utils.StartHideFailures("Navigating To " + EventName.ToUpper)

        Try
            EventName = EventName.Remove(EventName.Length - 1, 1).ToUpper

            For Each ch As Char In CharsArray

                _UI.Utils.GetEpgInfo("title", FirstLetter)

                Do While CurrentLetter <> ch.ToString
                    If FirstLetter = CurrentLetter Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Completed A Cycle Without Finding Letter " + ch.ToString + " On VOD"))
                    End If

                    _UI.Utils.GetEpgInfo("title", CurrentLetter)

                    If FirstLoop Then
                        CurrentLetter = "Empty"
                        FirstLoop = False
                    End If

                    If CurrentLetter = ch.ToString Then
                        _UI.Utils.SendIR("SELECT")
                        CurrentLetter = "Empty"
                        FirstLoop = True
                        Exit Do
                    End If

                    NextLetter()
                Loop

            Next

            _UI.Utils.SendIR("SELECT_DOWN")

            _UI.Utils.GetEpgInfo("keyWord", FoundEvent)

            If FoundEvent.ToUpper = OriginalEventName.ToUpper Then
                _UI.Utils.SendIR("SELECT")
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Search Event " + OriginalEventName + " On VOD"))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub NextLetter()
        Dim CurrentLetter As String = ""
        Dim CheckedLetter As String = ""

        _UI.Utils.StartHideFailures("Navigating To NextLetter")

        Try
            _UI.Utils.GetEpgInfo("title", CurrentLetter)

            _UI.Utils.SendIR("SELECT_RIGHT")

            _UI.Utils.GetEpgInfo("title", CheckedLetter)

            If CurrentLetter = CheckedLetter Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Letter On VOD Search CheckedLetter = " + CheckedLetter.ToString + " CurrentLetter = " + CurrentLetter.ToString))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''    Verifying Event Name On VOD
    ''' </summary>
    ''' <param name="EventName">The Event Name To Verify</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyEventName(ByVal EventName As String)
        Dim FoundEvent As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Verifying Event Name On VOD = " + EventName)

        Try

            _UI.Utils.GetEpgInfo("keyword", FoundEvent)

            If FoundEvent.ToUpper = EventName.ToUpper Then
                Msg = "Verified Event " + EventName + " On VOD"
                Exit Sub
            End If


            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Event " + EventName + " On VOD"))
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Moving To Next Asset X Times
    ''' </summary>
    ''' <param name="NumOfPresses">Optional Parameter Default = 1 : X Events To Move On VOD</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NextAsset(Optional ByVal NumOfPresses As Integer = 1)
        'Must Be EventName Since No Time In No Information Availble
        Dim CurEventName As String = ""
        Dim CheckedEventName As String = ""

        _UI.Utils.StartHideFailures("Navigating To NextEvent Event " + NumOfPresses.ToString + " Times")

        Try
            For RepeatIR As Integer = 1 To NumOfPresses

                GetSelectedEventName(CurEventName)

                _UI.Utils.SendIR("SELECT_RIGHT", 2000)

                GetSelectedEventName(CheckedEventName)

                If CurEventName = CheckedEventName Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Event On VOD CheckedEventName = " + CheckedEventName.ToString + " CurEventName = " + CurEventName.ToString))
                End If
            Next
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Moving To next category
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Public Overrides Sub NextCategory()

        _UI.Utils.StartHideFailures("Navigating to category")

        Try
            _UI.Utils.SendIR("SELECT_RIGHT", 2000)

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Gets The Selected Event Name From VOD
    ''' </summary>
    ''' <param name="EventName">Returns The Selected Event Name</param>
    ''' <remarks></remarks>
    Public Overrides Sub GetSelectedEventName(ByRef EventName As String)
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Get Selected EventName From VOD")

        Try
            _UI.Utils.GetEpgInfo("title", EventName)
            Msg = "Got -> " + EventName.ToString
            _iex.Wait(1)
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    ''' <summary>
    '''   Navigate to CatchUp menu
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToCatchUp()
        ' Navigate to Catchup menu
        If Not _UI.Utils.EPG_Milestones_NavigateByName("STATE:CATCHUP") Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed navigate to CatchUp menu"))
        End If
    End Sub

    ''' <summary>
    '''   Play an asset
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>340 - PlayEventFailure</para> 
    ''' 
    ''' </remarks>
    Public Overrides Sub Play(Optional ByVal fromStart As Boolean = True, _
                              Optional ByVal parentalControl As Boolean = False, _
                              Optional ByVal purchaseProtection As Boolean = True)
        Dim playbackMilestone As String
        Dim ActualLines As New ArrayList
		Dim waitingMsgMilestone As String

        _UI.Utils.StartHideFailures("Play asset")

        Try
            If (fromStart) Then
                ' try to select "PLAY" item
                Try
                    _UI.Utils.EPG_Milestones_SelectMenuItem("PLAY")
                Catch ex As Exception
                    ' try to select "PLAY FROM START" item
                    Try
                        _UI.Utils.EPG_Milestones_SelectMenuItem("PLAY FROM BEGINNING")
                    Catch _ex As Exception
                        ' try to buy the asset (in case asset is not purchased yet)
                        Buy(purchaseProtection, parentalControl)
                        Exit Sub
                    End Try
                End Try
            Else
                ' try to select "RESUME" item
                Try
                    _UI.Utils.EPG_Milestones_SelectMenuItem("PLAY FROM LAST VIEWED")
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed to select RESUME option"))
                End Try
            End If

            playbackMilestone = "TrickmodeState," & _UI.Utils.GetValueFromMilestones("PlayEvent")
			waitingMsgMilestone = _UI.Utils.GetValueFromDictionary("DIC_CONTENT_LOADING_MSG")
            playbackMilestone = playbackMilestone & "," & waitingMsgMilestone
			
            _UI.Utils.BeginWaitForDebugMessages(playbackMilestone, 90)

            ' Press SELECT
            _UI.Utils.SendIR("SELECT", 3000)

            ' Enter PIN for parental rating
            If parentalControl Then

                ' Check PIN is asked
                If Not _UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL", 5) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify PIN entry State"))
                End If
                ' Enter PIN
                Dim pin As String = _UI.Utils.GetValueFromEnvironment("YouthPIN")
                _UI.Utils.EnterPin(pin)

            End If

            ' Wait for playback state
            If Not _UI.Utils.VerifyState("PLAYBACK", 25) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PLAYBACK State Entered"))
            End If

            ' Check Trickmode bar state and playback FAS milestones
            If Not _UI.Utils.EndWaitForDebugMessages(playbackMilestone, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PlayEvent Milestone : " + playbackMilestone))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Play trailer
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>340 - PlayEventFailure</para> 
    ''' 
    ''' </remarks>
    Public Overrides Sub PlayTrailer(Optional ByVal parentalControl As Boolean = False)
        Dim playbackMilestone As String
        Dim ActualLines As New ArrayList

        _UI.Utils.StartHideFailures("Play trailer")

        Try
            ' try to select "WATCH TRAILER" item
            Try
                _UI.Utils.EPG_Milestones_SelectMenuItem("WATCH TRAILER")
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To select 'WATCH TRAILER' item"))
            End Try

            playbackMilestone = "TrickmodeState," & _UI.Utils.GetValueFromMilestones("PlayEvent")
            _UI.Utils.BeginWaitForDebugMessages(playbackMilestone, 90)

            ' Press SELECT
            _UI.Utils.SendIR("SELECT", 0)

            ' Enter PIN for parental rating
            If parentalControl Then

                ' Check PIN is asked
                If Not _UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL", 5) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify PIN entry State"))
                End If
                ' Enter PIN
                Dim pin As String = _UI.Utils.GetValueFromEnvironment("YouthPIN")
                _UI.Utils.EnterPin(pin)

            End If

            ' Wait for playback state
            'If Not _UI.Utils.VerifyState("PLAYBACK", 25) Then
            'ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PLAYBACK State Entered"))
            'End If

            ' Check Trickmode bar state and playback FAS milestones
            If Not _UI.Utils.EndWaitForDebugMessages(playbackMilestone, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PlayEvent Milestone : " + playbackMilestone))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Buy an asset
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>340 - PlayEventFailure</para> 
    ''' <returns>True if succeeded, False otherwise</returns>
    ''' </remarks>
    Public Overrides Sub Buy(Optional ByVal purchaseProtection As Boolean = True, _
                                  Optional ByVal parentalControl As Boolean = False)
        Dim devicePin As String
        Dim youthPin As String
        Dim playbackMilestone As String
        Dim ActualLines As New ArrayList
		Dim waitingMsgMilestone As String

        _UI.Utils.StartHideFailures("Buy asset")

        Try
            ' Get PINs
            devicePin = _UI.Utils.GetValueFromEnvironment("DefaultPIN")
            youthPin = _UI.Utils.GetValueFromEnvironment("YouthPIN")

            ' Select BUY item
            Try
                _UI.Utils.EPG_Milestones_SelectMenuItem("BUY")
            Catch ex As Exception
                _UI.Utils.LogCommentInfo("Failed to select BUY item")
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed select BUY item"))
            End Try

            playbackMilestone = "TrickmodeState," & _UI.Utils.GetValueFromMilestones("PlayEvent")
			waitingMsgMilestone = _UI.Utils.GetValueFromDictionary("DIC_CONTENT_LOADING_MSG")
            playbackMilestone = playbackMilestone & "," & waitingMsgMilestone
			
            _UI.Utils.BeginWaitForDebugMessages(playbackMilestone, 90)

            ' Press SELECT
            _UI.Utils.SendIR("SELECT", 3000)

            ' Enter PIN for purchase
            If purchaseProtection Then

                ' Check PIN is asked
                If Not _UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL", 5) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify PIN entry State"))
                End If
                _UI.Utils.EnterPin(devicePin)

            End If

            ' Enter PIN for parental rating
            If parentalControl Then

                ' Check PIN is asked
                If Not _UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL", 5) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify PIN entry State"))
                End If
                _iex.Wait(1)
                _UI.Utils.EnterPin(youthPin)
            End If

            ' Wait for playback state
            If Not _UI.Utils.VerifyState("PLAYBACK", 25) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PLAYBACK State Entered"))
            End If

            ' Check Trickmode bar state and playback FAS milestones
            If Not _UI.Utils.EndWaitForDebugMessages(playbackMilestone, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.PlayEventFailure, "Failed To Verify PlayEvent Milestone : " + playbackMilestone))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Stop a playback
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>343 - StopPlayEventFailure</para>
    ''' <para></para> 
    ''' 
    ''' </remarks>
    Public Overrides Sub StopPlayback()
        Dim stopKey As String
        Dim activeState As String = ""
        Dim stopPBMilestone As String
        Dim ActualLines As New ArrayList

        _UI.Utils.StartHideFailures("Stop asset playback")

        Try
            ' Exit if current state <> PLAYBACK
            _UI.Utils.GetActiveState(activeState)
            If (activeState <> "PLAYBACK") And (activeState <> "TRICKMODE") And (activeState <> "TRICKMODE BAR") Then
                Exit Sub
            End If

            stopPBMilestone = _UI.Utils.GetValueFromMilestones("TrickModeStopNotInReviewBuffer")
            _UI.Utils.BeginWaitForDebugMessages(stopPBMilestone, 60)

            ' Press STOP
            stopKey = _UI.Utils.GetValueFromProject("KEY_MAPPING", "STOP_KEY")
            _UI.Utils.SendIR(stopKey)

            ' Check FAS milestone
            If Not _UI.Utils.EndWaitForDebugMessages(stopPBMilestone, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed To Verify TrickModeStopNotInReviewBuffer Milestones : " + stopPBMilestone))
            End If

            ' Check state is different from PLAYBACK
            _iex.Wait(5)
            If _UI.Utils.VerifyState("PLAYBACK", 5) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.StopPlayEventFailure, "Failed to stop playback. Current state is still PLAYBACK"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Subscribe a SVOD asset
    ''' </summary>

    Public Overrides Sub Subscribe()

        Dim selectKey As String

        _UI.Utils.StartHideFailures("Subscribe asset")

        Try
            ' Select SUBSCRIBE item
            Try
                _UI.Utils.EPG_Milestones_SelectMenuItem("SUBSCRIBE")
            Catch
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed to find SUBSCRIBE item"))
            End Try

            selectKey = _UI.Utils.GetValueFromProject("KEY_MAPPING", "SELECT_KEY")
            _UI.Utils.SendIR(selectKey)

            ' Verify state
            ' ### check information message --> Not possible currently: CQ 2020793    

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigate to purchased assets in My Library
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToPurchasedAssets()

        _UI.Utils.StartHideFailures("Navigate to purchased items in My Library")

        Try
            If Not _UI.Utils.EPG_Milestones_NavigateByName("STATE:PURCHASED ON DEMAND") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To navigate to My Library/Purchased ON DEMAND"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub NavigateToAsset(ByVal vodAsset As VODAsset, Optional ByVal doSelect As Boolean = True)

        Dim activeState As String = ""

        _UI.Utils.StartHideFailures("Navigate to an asset")

        Try
            ' get the navigation path
            Dim path As String = _UI.Utils.EPGStateMachine.GetNavigationPath(vodAsset.NamedNavigation)

            If vodAsset.Genre.ToUpper <> "ADULT" Then
                ' *** General case : the asset is not an adult asset (normal navigation) ***
                _UI.Utils.NavigateAndHighlight(path)

                If doSelect Then
                    Me.DoSelect()
                End If
            Else
                ' *** the asset is an adult asset (2 cases) ***
                ' 1) the adult is not located inside an adult category (asset title is hidden, PIN is requested when selecting the adult asset)
                ' 2) the asset is located inside an adult category (adult category name = "Adult", PIN is requested when entering the adult category)

                Dim adultAssetRemplacementTitle As String = _UI.Utils.GetValueFromDictionary("DIC_STORE_ADULT_CONTENT")
                Dim pathItems As String() = path.Split(New [Char]() {"/"c})
                If pathItems(pathItems.Length - 1) = adultAssetRemplacementTitle Then

                    ' case 1: the adult is not located inside an adult category
                    ' access the asset through normal navigation
                    _UI.Utils.NavigateAndHighlight(path)
                    ' if selection is requested, PIN is automatically entered
                    If doSelect Then
                        Me.DoSelect()
                        _UI.Utils.EnterPin(_UI.Utils.GetValueFromEnvironment("AdultPIN"))
                    End If

                ElseIf pathItems(pathItems.Length - 2) = "Adult" Then

                    ' case 2: the asset is located in an adult category (we need to split the navigation)
                    ' first, navigate to the adult category
                    _UI.Utils.NavigateAndHighlight(path.Substring(0, InStrRev(path, "/") - 1))
                    Me.DoSelect()
                    ' enter PIN
                    _UI.Utils.EnterPin(_UI.Utils.GetValueFromEnvironment("AdultPIN"))
                    ' then focus the asset
                    _UI.Utils.GetActiveState(activeState)
                    _UI.Utils.NavigateAndHighlight(activeState + "/" + pathItems(pathItems.Length - 1))
                    ' select the asset if requested (no PIN is asked)
                    If doSelect Then
                        Me.DoSelect()
                    End If
                End If
            End If

            ' check PIN
            'If Not _UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL") Then
            'ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed to verify that PIN is asked when selecting the adult asset"))
            'End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Highlight next purchased item in My Library
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NextPurchasedAsset()

        _UI.Utils.StartHideFailures("Highlight next purchased item in My Library")

        Try
            _UI.Utils.SendIR("SELECT_DOWN")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Highlight next season
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NextSeason()

        _UI.Utils.StartHideFailures("Highlight next season")

        Try
            _UI.Utils.SendIR("SELECT_DOWN")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Highlight next episode
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NextEpisode()

        _UI.Utils.StartHideFailures("Highlight next episode")

        Try
            _UI.Utils.SendIR("SELECT_DOWN")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Return price of the asset currently selected
    ''' </summary>
    ''' <returns>price of the asset currently selected, -1 in case of error</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetPrice() As Double
        Dim displayedString As String = ""
        Dim state As String = ""
        Dim pattern As String = ""
        Dim match As Match

        _UI.Utils.StartHideFailures("Return asset price")

        Try
            GetAssetPrice = -1

            ' Get current state
            _UI.Utils.GetActiveState(state)

            If (state = "ON DEMAND" Or state = "STORE_LEAF_CATEGORY") Then
                ' Format is "€0,99 - 1h / Drama" (when focusing an asset)
                _UI.Utils.GetEPGInfo("description", displayedString)
            ElseIf state = "ASSET DETAILS" Then
                ' Format is "€0,99 - 1h" (when asset is selected: asset details page)
                _UI.Utils.GetEPGInfo("evttime", displayedString)
            End If

            If displayedString.Contains("FREE") Then
                GetAssetPrice = 0
            Else
                pattern = "\?[0-9]+(,)?[0-9]?[0-9]? - .*"
                match = Regex.Match(displayedString, pattern)
                If match.Success Then
                    GetAssetPrice = CDbl(displayedString.Substring(displayedString.IndexOf("?") + 1, displayedString.IndexOf(" - ") - displayedString.IndexOf("?") - 1).Replace(",", "."))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the rental duration (in minute) of the asset currently selected
    ''' </summary>
    ''' <returns>rental duration (in minute) of the asset currently selected, 0 in case the asset is free, -1 in case of error</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetRentalDuration() As Integer
        Dim displayedString As String = ""
        Dim pattern As String
        Dim nbOfMin As Integer = 0
        Dim nbOfHour As Integer = 0
        Dim nbOfDay As Integer = 0
        Dim match As Match
        Dim state As String = ""

        _UI.Utils.StartHideFailures("Return rental duration")

        Try
            GetAssetRentalDuration = -1

            ' Get current state
            _UI.Utils.GetActiveState(state)

            If state = "ON DEMAND" Or state = "STORE_LEAF_CATEGORY" Then
                _UI.Utils.GetEpgInfo("description", displayedString)
            ElseIf state = "ASSET DETAILS" Then
                _UI.Utils.GetEPGInfo("evttime", displayedString)
            End If

            If displayedString.Contains("FREE") Then
                GetAssetRentalDuration = 0
            Else
                pattern = "[0-9]+day(s)?"
                match = Regex.Match(displayedString, pattern)
                If match.Success Then
                    nbOfDay = Int32.Parse(match.Value.Replace("day", "").Replace("s", ""))
                End If
                pattern = "[0-2]?[0-9]h"
                match = Regex.Match(displayedString, pattern)
                If match.Success Then
                    nbOfHour = Int32.Parse(match.Value.Replace("h", ""))
                End If
                pattern = "[0-2]?[0-9]min(s)?"
                match = Regex.Match(displayedString, pattern)
                If match.Success Then
                    nbOfMin = Int32.Parse(match.Value.Replace("min", "").Replace("s", ""))
                End If
				pattern = "[0-2]?[0-9]{2}min(s)?"
                match = Regex.Match(displayedString, pattern)
                If match.Success Then
                    nbOfMin = Int32.Parse(match.Value.Replace("min", "").Replace("s", ""))
                End If
                GetAssetRentalDuration = nbOfDay * 1440 + nbOfHour * 60 + nbOfMin
                If GetAssetRentalDuration = 0 Then
                    GetAssetRentalDuration = -1
                End If
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the remaining rental duration (in minute) of the asset currently selected
    ''' </summary>
    ''' <returns>remaining rental duration (in minute) of the asset currently selected, -1 in case of error</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetRemainingRentalDuration() As Integer
        Dim remainingRental As String = ""
        Dim pattern As String
        Dim nbOfMin As Integer = 0
        Dim nbOfHour As Integer = 0
        Dim nbOfDay As Integer = 0
        Dim match As Match
        Dim state As String = ""

        _UI.Utils.StartHideFailures("Return remaining rental duration")

        Try
            GetAssetRemainingRentalDuration = -1

            ' Get current state
            _UI.Utils.GetActiveState(state)

            If state = "ON DEMAND" Then
                ' Format is "REMAINING RENTAL - 57min / Drama" ['description' milestone] (when focusing an asset)
                _UI.Utils.GetEPGInfo("description", remainingRental)
            ElseIf state = "ASSET DETAILS" Then
                ' Format is "REMAINING RENTAL - 55min" ['evttime' milestone] (when asset is selected: asset details page)
                _UI.Utils.GetEPGInfo("evttime", remainingRental)
            End If

            If remainingRental.StartsWith("?") Then
                GetAssetRemainingRentalDuration = 0
            ElseIf remainingRental.StartsWith("REMAINING RENTAL - ") Then
                pattern = "[0-9]+day(s)?"
                match = Regex.Match(remainingRental, pattern)
                If match.Success Then
                    nbOfDay = Int32.Parse(match.Value.Replace("day", "").Replace("s", ""))
                End If
                pattern = "[0-9]+h"
                match = Regex.Match(remainingRental, pattern)
                If match.Success Then
                    nbOfHour = Int32.Parse(match.Value.Replace("h", ""))
                End If
                pattern = "[0-9]+min(s)?"
                match = Regex.Match(remainingRental, pattern)
                If match.Success Then
                    nbOfMin = Int32.Parse(match.Value.Replace("min", "").Replace("s", ""))
                End If
                GetAssetRemainingRentalDuration = nbOfDay * 1440 + nbOfHour * 60 + nbOfMin
                If GetAssetRemainingRentalDuration = 0 Then
                    GetAssetRemainingRentalDuration = -1
                End If
            Else
                GetAssetRemainingRentalDuration = -1
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the genre of the asset currently selected
    ''' </summary>
    ''' <returns>Genre of the asset currently selected, Nothing if the displayed string is not formatted as expected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetGenre() As String
        Dim displayedString As String = ""
        Dim state As String = ""
        Dim pattern As String = ""

        _UI.Utils.StartHideFailures("Return genre")

        Try
            GetAssetGenre = Nothing

            ' Get displayed string
            _UI.Utils.GetEPGInfo("description", displayedString)

            ' Get current state
            _UI.Utils.GetActiveState(state)

            If state = "ON DEMAND" Or state = "STORE_LEAF_CATEGORY" Then
                ' Format is "€2,99 - 5min / Adventure" (when focusing an asset)
                GetAssetGenre = Split(displayedString, "/")(1).Trim()

            ElseIf state = "ASSET DETAILS" Then
                ' Format is "Adventure / 2min" (when asset is selected: asset details page)
                GetAssetGenre = Split(displayedString, "/")(0).Trim()
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Function


    ''' <summary>
    '''   Return the synopsis of the asset currently selected
    ''' </summary>
    ''' <returns>Synopsis of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetSynopsis() As String
        Dim synopsis As String = ""

        _UI.Utils.StartHideFailures("Return synopsis")

        Try
            _UI.Utils.GetEPGInfo("synopsis", synopsis)
            GetAssetSynopsis = synopsis
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the duration (in minute) of the asset currently selected
    ''' </summary>
    ''' <returns>Duration (in minute) of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetDuration() As Integer
        Dim duration As String = ""
        Dim pattern As String
        Dim nbOfMin As Integer = 0
        Dim nbOfHour As Integer = 0
        Dim match As Match

        _UI.Utils.StartHideFailures("Return duration")

        Try
            _UI.Utils.GetEPGInfo("duration", duration)
            pattern = "[0-9]+hour(s)?"
            match = Regex.Match(duration, pattern)
            If match.Success Then
                nbOfHour = Int32.Parse(match.Value.Replace("hour", "").Replace("s", ""))
            End If
            pattern = "[0-9]+min(s)?"
            match = Regex.Match(duration, pattern)
            If match.Success Then
                nbOfMin = Int32.Parse(match.Value.Replace("min", "").Replace("s", ""))
            End If
            GetAssetDuration = nbOfHour * 60 + nbOfMin
            If GetAssetDuration = 0 Then
                GetAssetDuration = -1
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the title of the asset currently selected
    ''' </summary>
    ''' <returns>Title of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetTitle() As String
        Dim title As String = ""

        _UI.Utils.StartHideFailures("Return title")

        Try
            _UI.Utils.GetEPGInfo("evtname", title)
            GetAssetTitle = title
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the broadcast date and time of the catchup asset currently selected
    ''' </summary>
    ''' <returns>Title of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetBroadcastDateTime() As Date
        Dim state As String = ""
        Dim displayedString As String = ""
        Dim pattern As String
        Dim match As Match
        Dim day As Integer, month As Integer, hour As Integer, minute As Integer
        Dim _array

        _UI.Utils.StartHideFailures("Return broadcast date/time")

        Try
            GetAssetBroadcastDateTime = Nothing

            ' Get current state
            _UI.Utils.GetActiveState(state)

            If state = "STORE_LEAF_CATEGORY" Then
                ' Format is "Fri 20 Jun / 11:00-11:00 / €1,00 - 1h / Talk Show" (when focusing the asset in store)
                _UI.Utils.GetEpgInfo("description", displayedString)
            ElseIf state = "ASSET DETAILS" Then
                ' Format is "Fri 20 Jun / 11:00-11:00 / €1,00 - 1h" (in asset action menu)
                _UI.Utils.GetEPGInfo("evttime", displayedString)
            End If

            ' check whole format
            pattern = "^(Mon|Tue|Wed|Thu|Fri|Sat|Sun) [0-9]+ (Jan|Feb|Mar|Apr|Mai|Jun|Jul|Aug|Sep|Oct|Nov|Dec) / [0-9]+:[0-9]+-[0-9]+:[0-9]+ /*"
            match = Regex.Match(displayedString, pattern)
            If Not match.Success Then
                Exit Function
            End If

            ' get day
            _array = Split(match.Value, " ")
            day = Int32.Parse(_array(1))

            ' get month
            pattern = "Jan|Feb|Mar|Apr|Mai|Jun|Jul|Aug|Sep|Oct|Nov|Dec"
            match = Regex.Match(displayedString, pattern)
            If match.Success Then
                Select Case match.Value
                    Case "Jan"
                        month = 1
                    Case "Feb"
                        month = 2
                    Case "Mar"
                        month = 3
                    Case "Apr"
                        month = 4
                    Case "Mai"
                        month = 5
                    Case "Jun"
                        month = 6
                    Case "Jul"
                        month = 7
                    Case "Aug"
                        month = 8
                    Case "Sep"
                        month = 9
                    Case "Oct"
                        month = 10
                    Case "Nov"
                        month = 11
                    Case "Dec"
                        month = 12
                End Select

            End If

            ' get hour & minute
            pattern = "[0-9]+:[0-9]+"
            match = Regex.Match(displayedString, pattern)
            If match.Success Then
                _array = Split(match.Value, ":")
                hour = Int32.Parse(_array(0))
                minute = Int32.Parse(_array(1))
            End If

            ' create the Date object
            GetAssetBroadcastDateTime = New Date(Date.Now.Year, month, day, hour, minute, 0)
            If GetAssetBroadcastDateTime.CompareTo(Date.Now) >= 0 Then
                GetAssetBroadcastDateTime = New Date(Date.Now.Year - 1, month, day, hour, minute, 0)
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the subscription status of the asset currently selected
    ''' </summary>
    ''' <returns>Subscription status of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetSubscriptionStatus() As EnumSubscriptionStatus

        Dim state As String = ""
        Dim displayedString As String = ""
        Dim freeAssetText As String
        Dim subscribedAssetText As String
        Dim nonSubscribedAssetText As String

        _UI.Utils.StartHideFailures("Return subscription status")

        Try
            GetAssetSubscriptionStatus = EnumSubscriptionStatus.UNKNOWN

            ' Get current state
            _UI.Utils.GetActiveState(state)

            ' get displayed string
            If state = "ON DEMAND" Then
                _UI.Utils.GetEPGInfo("description", displayedString)
            ElseIf state = "ASSET DETAILS" Then
                _UI.Utils.GetEPGInfo("evttime", displayedString)
            End If

            freeAssetText = _UI.Utils.GetValueFromDictionary("DIC_STORE_FREE_ASSET")
            nonSubscribedAssetText = _UI.Utils.GetValueFromDictionary("DIC_MONTHLY_SUBSCRIPTION")
            subscribedAssetText = _UI.Utils.GetValueFromDictionary("DIC_STORE_SUBSCRIBED")

            If (displayedString.StartsWith(freeAssetText)) Then
                GetAssetSubscriptionStatus = EnumSubscriptionStatus.FREE
            ElseIf (displayedString.StartsWith(subscribedAssetText)) Then
                GetAssetSubscriptionStatus = EnumSubscriptionStatus.SUBSCRIBED
            ElseIf (displayedString.StartsWith(nonSubscribedAssetText)) Then
                GetAssetSubscriptionStatus = EnumSubscriptionStatus.NOT_SUBSCRIBED
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the rental status of the asset currently selected
    ''' </summary>
    ''' <returns>rental status of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetRentalStatus() As EnumRentalStatus

        _UI.Utils.StartHideFailures("Return rental status")

        Try
            GetAssetRentalStatus = EnumRentalStatus.UNKNOWN

            If GetAssetRentalDuration() > 0 Then
                GetAssetRentalStatus = EnumRentalStatus.NOT_RENTED
            ElseIf GetAssetRentalDuration() = 0 Then
                GetAssetRentalStatus = EnumRentalStatus.FREE
            ElseIf GetAssetRemainingRentalDuration() > 0 Then
                GetAssetRentalStatus = EnumRentalStatus.RENTED
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the director information of the current selected asset. Extended information page must be displayed. 
    ''' </summary>
    ''' <returns>Director info of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetDirector() As String

        _UI.Utils.StartHideFailures("Return director info")

        Try
            GetAssetDirector = Nothing

            ' get displayed string
            Dim director As String = ""
            _UI.Utils.GetEpgInfo("directors", director)

            GetAssetDirector = director
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the cast information of the current selected asset. Extended information page must be displayed. 
    ''' </summary>
    ''' <returns>Cast info of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetCast() As String

        _UI.Utils.StartHideFailures("Return cast info")

        Try
            GetAssetCast = Nothing

            ' get displayed string
            Dim cast As String = ""
            _UI.Utils.GetEpgInfo("actors", cast)

            GetAssetCast = cast
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the year of production information of the current selected asset. Extended information page must be displayed. 
    ''' </summary>
    ''' <returns>Year of production of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetYearOfProduction() As String

        _UI.Utils.StartHideFailures("Return year of production")

        Try
            GetAssetYearOfProduction = Nothing

            ' get displayed string
            Dim yearOfProduction As String = ""
            _UI.Utils.GetEpgInfo("yearofproduction", yearOfProduction)

            GetAssetYearOfProduction = yearOfProduction
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the star rating information of the current selected asset. Extended information page must be displayed. 
    ''' </summary>
    ''' <returns>Star rating of the asset currently selected</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetAssetStarRating() As String

        _UI.Utils.StartHideFailures("Return star rating")

        Try
            GetAssetStarRating = Nothing

            ' get displayed string
            Dim starRating As String = ""
            _UI.Utils.GetEpgInfo("rating", starRating)

            GetAssetStarRating = starRating
        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Return the current classification. 
    ''' </summary>
    ''' <returns>Current classification</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetCurrentClassification() As String

        _UI.Utils.StartHideFailures("Return current classification")

        Try
            GetCurrentClassification = Nothing

            ' get crumbtext
            Dim crumbtext As String = ""
            _UI.Utils.GetEpgInfo("crumbtext", crumbtext)

            ' return the current classification (last item in the crumbtext)
            Dim classificationList As String() = crumbtext.Split(New [Char]() {" "c})
            GetCurrentClassification = classificationList(classificationList.Length - 1)

        Finally
            _iex.ForceHideFailure()
        End Try

    End Function

    ''' <summary>
    '''   Press SELECT
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub DoSelect()

        Dim selectKey As String

        ' Send SELECT IR
        selectKey = _UI.Utils.GetValueFromProject("KEY_MAPPING", "SELECT_KEY")
        _UI.Utils.SendIR(selectKey)

    End Sub

    ''' <summary>
    '''   Select an asset in the list of purchased assets in My Library
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub SelectPurchasedAsset(ByVal assetTitle As String)

        Dim title As String = ""
        Dim initialTitle As String = ""
        Dim previousTitle As String = ""

        ' Navigate to the list of purchased assets
        NavigateToPurchasedAssets()

        ' Check if purchased list is empty or not
        If (_UI.Utils.VerifyState("LIBRARY ERROR")) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed to select '" & assetTitle & "' asset in purchased list. Purchased list is empty!"))
        End If

        ' Search for the asset in the list
        _UI.Utils.GetEPGInfo("evtname", title)
        If (title <> assetTitle) Then
            initialTitle = title
            previousTitle = title
            Do
                _UI.Vod.NextPurchasedAsset()
                _UI.Utils.GetEPGInfo("evtname", title)
                If (title = assetTitle) Then
                    Exit Do
                ElseIf ((title = initialTitle) Or (title = previousTitle)) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed to select '" & assetTitle & "' asset in purchased list"))
                End If
                previousTitle = title
            Loop While True
        End If

        ' Select the asset
        DoSelect()

    End Sub

    ''' <summary>
    '''   Select extended info of the asset currently selected (current state must be 'ASSET DETAILS' page)
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub DisplayExtendedInfo()

        ' Navigate to the list of purchased assets
        If Not _UI.Utils.EPG_Milestones_Navigate("INFO") Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed to select INFO item"))
        End If

        ' Check INFO state
        If Not _UI.Utils.VerifyState("INFO") Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed to select to check INFO state"))
        End If

    End Sub

End Class
