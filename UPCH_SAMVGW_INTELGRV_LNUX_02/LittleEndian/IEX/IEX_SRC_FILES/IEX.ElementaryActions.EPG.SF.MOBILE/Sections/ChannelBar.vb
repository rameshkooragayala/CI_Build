Imports FailuresHandler

Public Class ChannelBar
    Inherits IEX.ElementaryActions.EPG.SF.ChannelBar

    Dim _UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To Channel List
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _UI.Utils.StartHideFailures("Navigating To Channel Bar")

        Try

            If IsChannelBar() Then
                Exit Sub
            End If

            _UI.Utils.SwipeHorizontal(Source:="", IsSwipeRight:=True, WaitAfterSwipe:=2000, VerifyAnimation:=False)

            If Not IsChannelBar() Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To Channel Bar"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Checking If Channel Bar Is Already On Screen
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsChannelBar() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If Channel Bar Is On Screen")

        Try
            If _UI.Utils.VerifyState("ProgramBarView", 2) Then
                Msg = "Channel Is On Screen"
                Return True
            Else
                Msg = "Channel Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

    Public Overrides Sub SurfToChannel(ByVal ChannelNumber As String)
        Dim CurrentChNum As String = ""
        Dim CheckedChNum As String = "Empty"

        _UI.Utils.StartHideFailures("Surfing To Channel : " + ChannelNumber)

        Try
            GetChannelNumber(CurrentChNum)

            Do Until CurrentChNum = CheckedChNum
                _UI.Utils.SwipeVertical(Source:="ChannelBar.Events", IsSwipeUp:=True, WaitAfterSwipe:=1500, VerifyAnimation:=False)

                GetChannelNumber(CheckedChNum)

                If CheckedChNum = ChannelNumber Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Failed To Surf To Channel : " + ChannelNumber))
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Surfing Up On Channel List
    ''' </summary> 
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelUp(Optional ByVal Type As String = "", Optional ByVal WithSubtitles As Boolean = False)
        Dim CurrentChNum As String = ""
        Dim CheckedChNum As String = ""

        _UI.Utils.StartHideFailures("Surfing Up On Channel Bar")

        Try
            GetChannelNumber(CurrentChNum)

            _UI.Utils.SwipeVertical(Source:="ChannelBar.Events", IsSwipeUp:=True, WaitAfterSwipe:=1500, VerifyAnimation:=False)

            GetChannelNumber(CheckedChNum)

            If CurrentChNum = CheckedChNum Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Channel Number Is The Same As Previous Failed To Surf Channel Up"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''    Surfing Down On Channel List
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SurfChannelDown(Optional ByVal Type As String = "", Optional ByVal WithSubtitles As Boolean = False)
        Dim Messages As String = ""
        Dim CurrentChNum As String = ""
        Dim CheckedChNum As String = ""

        _UI.Utils.StartHideFailures("Surfing Down On Channel Bar")

        Try
            GetChannelNumber(CurrentChNum)

            _UI.Utils.SwipeVertical(Source:="ChannelBar.Events", IsSwipeUp:=False, WaitAfterSwipe:=1500, VerifyAnimation:=False)

            GetChannelNumber(CheckedChNum)

            If CurrentChNum = CheckedChNum Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Channel Number Is The Same As Previous Failed To Surf Channel Down"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Tunning To Channel From Channel Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub DoTune()
        _UI.Utils.StartHideFailures("Tunning To Channel From Channel Bar")

        Try
            _UI.Utils.Tap("NowEvent", "ChannelBar.Events", False)

            _UI.Utils.VerifyLiveReached()
            
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub GetEventName(ByRef EventName As String, ByVal IsNext As Boolean)
        Dim returnedValue As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Event Name From Channel Bar")

        Try
            If IsNext Then
                EventName = _UI.Utils.GetMobileEventInfo("NextEvent", "evtNa", "ChannelBar.Events")
            Else
                EventName = _UI.Utils.GetMobileEventInfo("NowEvent", "evtNa", "ChannelBar.Events")
            End If

            Msg = "Event Name : " + EventName

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    Public Overrides Sub GetEventStartTime(ByRef StartTime As String, ByVal IsNext As Boolean)
        Dim returnedValue As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Event Start Time From Channel Bar")

        Try
            If IsNext Then
                StartTime = _UI.Utils.GetMobileEventInfo("NextEvent", "evtStartTime", "ChannelBar.Events")
            Else
                StartTime = _UI.Utils.GetMobileEventInfo("NowEvent", "evtStartTime", "ChannelBar.Events")
            End If

            Msg = "Event StartTime : " + StartTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    Public Overrides Sub GetEventEndTime(ByRef EndTime As String, ByVal IsNext As Boolean)
        Dim returnedValue As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Event End Time From Channel Bar")

        Try
            If IsNext Then
                EndTime = _UI.Utils.GetMobileEventInfo("NextEvent", "evtEndTime", "ChannelBar.Events")
            Else
                EndTime = _UI.Utils.GetMobileEventInfo("NowEvent", "evtEndTime", "ChannelBar.Events")
            End If

            Msg = EndTime

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''   Getting Channel Number From Channel Bar
    ''' </summary>
    ''' <param name="ChannelNumber">ByRef The Returned Channel Number</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetChannelNumber(ByRef ChannelNumber As String)
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Channel Number From Channel Bar")

        Try
            Dim ReturnedValue As String = ""

            _UI.Utils.GetEpgInfo("chnum", ChannelNumber)

            Msg = "Channel Number : " + ChannelNumber
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Sub

    Public Overrides Sub NextEvent(Optional ByVal SelectEvent As Boolean = True)
        If SelectEvent Then
            _UI.Utils.Tap("NextEvent", "ChannelBar.Events")
        End If
    End Sub

    ''' <summary>
    '''   Preparing Recording Event From Channel Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>323 - VerifyStateFailure</para> 
    ''' </remarks>
    Public Overrides Sub PreRecordEvent()

        _UI.Utils.StartHideFailures("Preparing Recording Event From Channel Bar")

        Try

            _UI.Utils.Tap("NextEvent", "ChannelBar.Events")

            If Not _UI.Utils.VerifyState("ActionMenuHubStackView", 5) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerifyStateFailure, "Failed To Verify State Is ActionMenuHubStackView"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verifies Channel Number
    ''' </summary>
    ''' <param name="ChannelNumber">The Channel Number To Verify</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyChannelNumber(ByVal ChannelNumber As String)
        Dim CurrentChannel As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Verifying Channel Number Is : " + ChannelNumber)

        Try
            _UI.Utils.GetEpgInfo("chnum", CurrentChannel)

            If CurrentChannel = ChannelNumber Then
                Msg = "Verified Channel Is : " + ChannelNumber
                Exit Sub
            End If

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is : " + ChannelNumber))

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

End Class
