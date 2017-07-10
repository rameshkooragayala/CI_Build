Imports FailuresHandler
Imports System.Text.RegularExpressions
Public Class VOD
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.VOD

    Dim _UI As IEX.ElementaryActions.EPG.SF.GET.UI

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
            _UI.Utils.LogCommentBlack("Already in VOD Search Screen")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub HighLightAndCollectVODData(ByVal CatalogueName As String)

        'Align to a particular side L/R ,highlight and collect data
        Try
            Dim keyPress As String = _UI.Utils.GetValueFromProject("VOD", "KEY_PRESS")
            Dim nextKeyPress As String = _UI.Utils.GetValueFromProject("VOD", "NEXT_KEY_PRESS")
            Dim currentHighlight As String = ""
            Dim prevHighlight As String = ""
            Dim nextHighlight As String = ""
            'send key and ensure we are at one extreme end 
            'fetch current EPG highlight value
            _UI.Utils.LogCommentInfo("Fetching the current highlighted EPG menu item")
            _UI.Utils.GetEpgInfo("title", currentHighlight)
            _UI.Utils.LogCommentInfo("Current Highlight is " & currentHighlight)


            'this will allow us to reach one extreme end
            While Not prevHighlight = currentHighlight
                prevHighlight = currentHighlight
                _UI.Utils.SendIR(keyPress, 2000)
                _UI.Utils.GetEpgInfo("title", currentHighlight)
                _UI.Utils.LogCommentInfo("Current Highlight after key press is " & currentHighlight)

            End While
            _UI.Utils.LogCommentInfo("Reached One Extreme")
            nextHighlight = prevHighlight
            'now highlight and navigate on user define catalogue menu item
            While Not nextHighlight = CatalogueName
                nextHighlight = currentHighlight
                Dim counter As Integer = 0
                _UI.Utils.SendIR(nextKeyPress, 2000)
                counter = counter + 1
                If counter = 16 Then
                    _UI.Utils.GetEpgInfo("title", currentHighlight)
                    If currentHighlight <> CatalogueName Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigateFailure, "Failed To Navigate to Requested Path as Catalogue no  longer exists"))
                        Exit While
                    End If

                End If
                _UI.Utils.GetEpgInfo("title", currentHighlight)
                If currentHighlight = CatalogueName Then
                    _UI.Utils.LogCommentInfo("Highlight on proper item")
                    Exit While
                End If
                _UI.Utils.LogCommentInfo("Current Highlight after key press is " & currentHighlight)

            End While
            _UI.Utils.LogCommentInfo("Since the highlight is on proper user scenario, thus navigating inside of it")
            _UI.Utils.SendIR("SELECT", 2000)
        Catch ex As EAException
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigateFailure, "Failed To Navigate to Requested Path"))
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
    ''' 

    Public Overrides Sub NavigateToEventName(ByVal EventName As String)

        Dim CurrentLetter As String = "Empty"
        Dim FoundEvent As String = ""
        Dim OriginalEventName As String = EventName.ToUpper
        Dim FirstLetter As String = ""
        Dim FirstLoop As Boolean = True
        Dim Diff As Integer = 0
        EventName = EventName.ToUpper
        _UI.Utils.StartHideFailures("Navigating To " + EventName)
        Dim index As Integer = 0
        Dim lastIndex As Integer = 0
        Dim lst As List(Of Integer) = GetPosition(EventName)


        EventName = EventName.Replace(" ", "?")
        Dim CharsArray As Char() = EventName.ToCharArray
        Try

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
                        index = index + 1
                        FirstLoop = True
                        Exit Do
                    End If
                    If index = 0 Then
                        'If ch.ToString = "A" Then
                        '_UI.Utils.SendIR("SELECT")
                        'index = index + 1
                        'Else
                        Diff = (index - lst(index))
                        If Diff < 0 And Diff < -19 Then
                            lastIndex = -(Diff)
                            lastIndex = 37 - lastIndex
                            For i = 0 To lastIndex
                                _UI.Utils.SendIR("SELECT_LEFT")
                            Next
                            _UI.Utils.GetEpgInfo("title", CurrentLetter)
                            If CurrentLetter = ch.ToString() Then
                                _UI.Utils.SendIR("SELECT")
                                CurrentLetter = "Empty"
                                FirstLoop = True
                                index = index + 1
                                Exit Do

                            End If
                        ElseIf Diff < 0 And Diff >= -19 Then
                            lastIndex = -(Diff)
                            For i = 0 To lastIndex - 1
                                _UI.Utils.SendIR("SELECT_RIGHT")
                            Next
                            _UI.Utils.GetEpgInfo("title", CurrentLetter)
                            If CurrentLetter = ch.ToString() Then
                                _UI.Utils.SendIR("SELECT")
                                CurrentLetter = "Empty"
                                FirstLoop = True
                                index = index + 1
                                Exit Do
                            End If
                        End If
                    Else
                        Diff = lst(index - 1) - lst(index)
                        If Diff < 0 And Diff < -19 Then
                            lastIndex = -(Diff)
                            lastIndex = 37 - lastIndex
                            For i = 0 To lastIndex
                                _UI.Utils.SendIR("SELECT_LEFT")
                            Next
                            _UI.Utils.GetEpgInfo("title", CurrentLetter)
                            If CurrentLetter = ch.ToString() Then
                                _UI.Utils.SendIR("SELECT")
                                CurrentLetter = "Empty"
                                FirstLoop = True
                                index = index + 1
                                Exit Do
                            End If
                        ElseIf Diff < 0 And Diff >= -19 Then
                            lastIndex = -(Diff)
                            For i = 0 To lastIndex - 1
                                _UI.Utils.SendIR("SELECT_RIGHT")
                            Next
                            _UI.Utils.GetEpgInfo("title", CurrentLetter)
                            If CurrentLetter = ch.ToString() Then
                                _UI.Utils.SendIR("SELECT")
                                CurrentLetter = "Empty"
                                FirstLoop = True
                                index = index + 1
                                Exit Do
                            End If

                        ElseIf Diff > 0 And Diff > 19 Then
                            lastIndex = 37 - Diff
                            For i = 0 To lastIndex
                                _UI.Utils.SendIR("SELECT_RIGHT")
                            Next
                            _UI.Utils.GetEpgInfo("title", CurrentLetter)
                            If CurrentLetter = ch.ToString() Then
                                _UI.Utils.SendIR("SELECT")
                                CurrentLetter = "Empty"
                                FirstLoop = True
                                index = index + 1
                                Exit Do
                            End If
                        ElseIf Diff > 0 And Diff <= 19 Then
                            For i = 0 To Diff - 1
                                _UI.Utils.SendIR("SELECT_LEFT")
                            Next
                            _UI.Utils.GetEpgInfo("title", CurrentLetter)
                            If CurrentLetter = ch.ToString() Then
                                _UI.Utils.SendIR("SELECT")
                                CurrentLetter = "Empty"
                                FirstLoop = True
                                index = index + 1
                                Exit Do
                            End If
                        End If
                    End If

                    'NextLetter()
                Loop

            Next

            'EventName = EventName.Remove(EventName.Length - 1, 1).ToUpper

            'For Each ch As Char In CharsArray

            '    _UI.Utils.GetEpgInfo("title", FirstLetter)


            '    Do While CurrentLetter <> ch.ToString
            '        If FirstLetter = CurrentLetter Then
            '            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Completed A Cycle Without Finding Letter " + ch.ToString + " On VOD"))
            '        End If

            '        _UI.Utils.GetEpgInfo("title", CurrentLetter)

            '        If FirstLoop Then
            '            CurrentLetter = "Empty"
            '            FirstLoop = False
            '        End If

            '        If CurrentLetter = ch.ToString Then
            '            _UI.Utils.SendIR("SELECT")
            '            CurrentLetter = "Empty"
            '            FirstLoop = True
            '            Exit Do
            '        End If

            '        NextLetter()
            '    Loop

            'Next

            _UI.Utils.SendIR("SELECT_DOWN")

            _UI.Utils.GetEpgInfo("title", FoundEvent)



            'If FoundEvent.ToUpper = OriginalEventName.ToUpper Then
            '    _UI.Utils.SendIR("SELECT")
            '    Exit Sub
            'End If

            'ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Search Event " + OriginalEventName + " On VOD"))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Function GetPosition(ByVal eventName As String) As List(Of Integer)
        Dim lst As List(Of Integer) = New List(Of Integer)
        Dim CurrentLetter As String = ""
        Dim strData As Char() = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", "xx"}
        For Each ch As Char In eventName.ToCharArray
            For index = 0 To strData.Length - 1
                If strData(index) = ch Then lst.Add(index)
            Next
        Next
        Return lst
    End Function

   


    'Public Overrides Sub NextLetter()
    '    Dim CurrentLetter As String = ""
    '    Dim CheckedLetter As String = ""

    '    _UI.Utils.StartHideFailures("Navigating To NextLetter")

    '    Try
    '        _UI.Utils.GetEpgInfo("title", CurrentLetter)

    '        If IsNumeric(CurrentLetter) Then
    '            Asc(CurrentLetter)+43 - 
    '        Else

    '        End If


    '        _UI.Utils.SendIR("SELECT_RIGHT")

    '        _UI.Utils.GetEpgInfo("title", CheckedLetter)

    '        If CurrentLetter = CheckedLetter Then
    '            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Move To Next Letter On VOD Search CheckedLetter = " + CheckedLetter.ToString + " CurrentLetter = " + CurrentLetter.ToString))
    '        End If

    '    Finally
    '        _iex.ForceHideFailure()
    '    End Try
    'End Sub

    ''' <summary>
    ''' Playing Asset of VOD Asset
    ''' Applicable for GET Live Environment
    ''' </summary>
    Public Overrides Sub PlayModule()
        Try
            _UI.Utils.LogCommentWarning("Playing Navigated Asset")

            'Collecting Highlighted EPG Menu Item
            _UI.Utils.LogCommentInfo("Fetching the active state after selecting the asset")

            'Getting Active State
            Dim activeState As String = ""
            _UI.Utils.GetActiveState(activeState)
            _UI.Utils.LogCommentInfo("Active State after selecting asset is : " & activeState)

            'this is ensure if pin pop up menu is there else send key blindly
            Dim keySequence As String = "0000"
            Dim noPlayOption As Integer = 0
            'Checking condition before initiating play of asset
            If activeState = "VOD PLAY" Then

                'Getting Highlighted Menu Item
                Dim highlightedItem As String = ""
                _UI.Utils.GetEpgInfo("title", highlightedItem)


                If highlightedItem = "PLAY" Then
                    _UI.Utils.SendIR("SELECT", 2000)
                    'this part is breaking the flow before sending the key make sure pin is asked else just play

                    'adding a 10 sec wait here
                    _UI.Utils.LogCommentBlack("Adding a 10 sec safe delay to check whether playback started properly or not ")
                    _iex.Wait(10)
                    'get active state here
                    _UI.Utils.GetActiveState(activeState)
                    _UI.Utils.LogCommentImportant("Getting the active after 10 sec delay is :-  " & activeState)
                    If (activeState = "INSERT PIN") Then
                        _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                    End If
                    If (activeState = "RATE") Then
                        _UI.Utils.SendIR("SELECT", 2000)
                    End If
                    If (activeState <> "PLAYBACK") Then
                        _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                    End If
                    _UI.Utils.GetActiveState(activeState)
                    _UI.Utils.LogCommentImportant("Getting the active after sending PIN :-  " & activeState)
                    If (activeState <> "PLAYBACK") Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Playback of VOD Asset"))
                    End If
                    '******************************************************************
                ElseIf highlightedItem = "RESUME" Then
                    _UI.Utils.SendIR("SELECT", 2000)
                    _iex.Wait(10)
                    'get active state here
                    _UI.Utils.GetActiveState(activeState)
                    _UI.Utils.LogCommentImportant("Getting the active after 10 sec delay is :-  " & activeState)
                    If (activeState = "INSERT PIN") Then
                        _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                    End If
                    If (activeState = "RATE") Then
                        _UI.Utils.SendIR("SELECT", 2000)
                    End If

                    If (activeState <> "PLAYBACK") Then
                        _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                    End If
                    _UI.Utils.GetActiveState(activeState)
                    _UI.Utils.LogCommentImportant("Getting the active after sending PIN :-  " & activeState)
                    If (activeState <> "PLAYBACK") Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Playback of VOD Asset"))
                    End If


                ElseIf highlightedItem = "READ MORE" Then
                    Dim highlightedText As String = ""
                    ' check if play or resume option is present else try navigation to other asset
                    _UI.Utils.SendIR("SELECT_DOWN", 2000)
                    'Getting Highlighted Asset Name
                    _UI.Utils.GetEpgInfo("title", highlightedText)

                    If highlightedText = "PLAY" Then

                        _UI.Utils.SendIR("SELECT", 2000)
                        'adding a 10 sec wait here
                        _UI.Utils.LogCommentBlack("Adding a 10 sec safe delay to check whether playback started properly or not ")
                        _iex.Wait(10)
                        'get active state here
                        _UI.Utils.GetActiveState(activeState)
                        _UI.Utils.LogCommentImportant("Getting the active after 10 sec delay is :-  " & activeState)
                        If (activeState = "INSERT PIN") Then
                            _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                        End If
                        If (activeState = "RATE") Then
                            _UI.Utils.SendIR("SELECT", 2000)
                        End If
                        If (activeState <> "PLAYBACK") Then
                            _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                        End If
                        _UI.Utils.GetActiveState(activeState)
                        _UI.Utils.LogCommentImportant("Getting the active after sending PIN :-  " & activeState)
                        If (activeState <> "PLAYBACK") Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Playback of VOD Asset"))
                        End If
                    ElseIf highlightedText = "RESUME" Then
                        _UI.Utils.SendIR("SELECT", 2000)
                        'this part is breaking the flow before sending the key make sure pin is asked else just play

                        'adding a 10 sec wait here
                        _UI.Utils.LogCommentBlack("Adding a 10 sec safe delay to check whether playback started properly or not ")
                        _iex.Wait(10)
                        'get active state here
                        _UI.Utils.GetActiveState(activeState)
                        _UI.Utils.LogCommentImportant("Getting the active after 10 sec delay is :-  " & activeState)
                        If (activeState = "INSERT PIN") Then
                            _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                        End If
                        If (activeState = "RATE") Then
                            _UI.Utils.SendIR("SELECT", 2000)
                        End If
                        If (activeState <> "PLAYBACK") Then
                            _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
                        End If
                        _UI.Utils.GetActiveState(activeState)
                        _UI.Utils.LogCommentImportant("Getting the active after sending PIN :-  " & activeState)
                        If (activeState <> "PLAYBACK") Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Playback of VOD Asset"))
                        End If
                    Else
                        _UI.Utils.LogCommentBlack("PLAY/RESUME/RESTART Option is not highlighted on this asset")
                        _UI.Utils.LogCommentImportant("Trying on other asset")
                        ' Checking for other Assets before exiting test if play back is possible or not
                        Try

                            Dim focusedAsset As String = ""

                            _UI.Utils.LogCommentFail("Trying with other Asset for Play Option")
                            _UI.Utils.SendIR("RETOUR", 4000)
                            _UI.Utils.SendIR("SELECT_DOWN", 2000)

                            'Getting Highlighted Asset Name
                            _UI.Utils.GetEpgInfo("title", focusedAsset)
                            focusedAsset = ""
                            _UI.Utils.SendIR("SELECT", 2000)
                            _UI.Utils.GetEpgInfo("title", focusedAsset)
                            If focusedAsset = "READ MORE" Then
                                noPlayOption = noPlayOption + 1
                            End If
                            focusedAsset = ""
                            _UI.Utils.LogCommentFail("Play Option is not present , retrying again on other asset")
                            _UI.Utils.SendIR("RETOUR", 4000)
                            _UI.Utils.SendIR("SELECT_RIGHT", 2000)
                            'Getting Highlighted Asset Name
                            _UI.Utils.GetEpgInfo("title", focusedAsset)
                            focusedAsset = ""
                            _UI.Utils.SendIR("SELECT", 2000)
                            _UI.Utils.GetEpgInfo("title", focusedAsset)
                            If focusedAsset = "READ MORE" Then
                                noPlayOption = noPlayOption + 1
                            End If
                            _UI.Utils.LogCommentFail("No Play Option is :" & noPlayOption.ToString)

                            If noPlayOption = 2 Then
                                _UI.Utils.LogCommentFail("Call before Exiting test before catch block")
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Play Option continuously on Three Different Asset"))
                            End If
                        Catch ex As Exception
                            'Exiting the API and Tranferring Call to Script
                            _UI.Utils.LogCommentFail("Not Getting Enough Information from HE")
                            If noPlayOption = 2 Then
                                _UI.Utils.LogCommentFail("No Play Option is :" & noPlayOption.ToString)
                                _UI.Utils.LogCommentFail("Call before Exiting test inside catch block")
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Play Option continuously on Three Different Asset"))
                            Else
                                _UI.Utils.LogCommentImportant("Got Play Option and Script is executing properly")
                            End If
                        Finally
                            _iex.ForceHideFailure()
                        End Try
                    End If
                Else
                    _UI.Utils.SendIR("RETOUR", 4000)
                    _UI.Utils.SendIR("SELECT", 2000)
                    _iex.Wait(5)
                    _UI.Utils.SendIR("SELECT", 2000)
                End If


            ElseIf activeState = "INFO" Then
                _UI.Utils.LogCommentWarning("Active State is INFO so not performing any Operation")
            ElseIf activeState = "RATE" Then

                _UI.Utils.LogCommentImportant("Present State is Rate so performing No operation")
            Else
                _UI.Utils.SendChannelAsIRSequence(keySequence, 600)
            End If

            _UI.Utils.LogCommentInfo("Transferring Call Back to Layer Focus and Play API")
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Play Option continuously on Three Different Asset"))
        End Try
    End Sub

    ''' <summary>
    ''' Applying Trick Mode on playing VOD Asset
    ''' Applicable for GET Live Environment
    ''' </summary>
    Public Overrides Sub TrickSpeed()
        _UI.Utils.LogCommentInfo("Applying Trick Speeds in both forward and reverse direction")

        'Getting Milestones from Milestone.ini for Trickspeed
        Dim msg As Boolean = False
        Dim milestones As String = _UI.Utils.GetValueFromMilestones("TrickModeSpeed")
        Dim ActualLines As New ArrayList
        Try
            _UI.Utils.LogCommentInfo("First setting Forward TrickModes")

            'Raise the trickmode bar and Apply 4x forward speed
            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)

            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _iex.Wait(100)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("4x speed was properly set")
            End If

            'Stop the playback and change speed to -4x reverse speed

            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)
            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("REWIND", 2000)
            _iex.Wait(100)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("-4x speed was properly set")
            End If

            'Stop Play Back and Apply 8x forward speed

            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)

            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _iex.Wait(90)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("8x speed was properly set")
            End If

            'Stop Play Back and Apply -8x reverse speed

            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)
            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("REWIND", 2000)
            _UI.Utils.SendIR("REWIND", 2000)
            _iex.Wait(90)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("-8x speed was properly set")
            End If

            'Stop Play Back and Apply 16x forward speed

            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)

            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _iex.Wait(80)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("16x speed was properly set")
            End If

            'Stop PlayBack and Apply -16x reverse speed

            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)
            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("REWIND", 2000)
            _UI.Utils.SendIR("REWIND", 2000)
            _UI.Utils.SendIR("REWIND", 2000)
            _iex.Wait(80)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("-16x speed was properly set")
            End If

            'Stop PlayBack and Apply 32x Forward Speed
            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)
            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _UI.Utils.SendIR("FASTFORWARD", 2000)
            _iex.Wait(70)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("32x speed was properly set")
            End If
            'Stop PlayBack and Apply -32x Reverse Speed
            _UI.Utils.SendIR("PAUSE", 2000)
            _iex.Wait(5)
            _UI.Utils.BeginWaitForDebugMessages(milestones, 180)
            _UI.Utils.SendIR("REWIND", 2000)
            _UI.Utils.SendIR("REWIND", 2000)
            _UI.Utils.SendIR("REWIND", 2000)
            _UI.Utils.SendIR("REWIND", 2000)
            _iex.Wait(70)
            msg = _UI.Utils.EndWaitForDebugMessages(milestones, ActualLines)

            If msg = True Then
                _UI.Utils.LogCommentInfo("-32x speed was properly set")
            End If
        Catch ex As EAException
            _UI.Utils.LogCommentInfo("Something has gone wrong while setting trick speeds" & ex.Message)
            'adding exit condition from this API
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Something has gone wrong while setting trick speeds" & ex.Message))
        End Try
        _UI.Utils.LogCommentInfo("Tranfering Call back to Layer Focus and Play API")
    End Sub
End Class
