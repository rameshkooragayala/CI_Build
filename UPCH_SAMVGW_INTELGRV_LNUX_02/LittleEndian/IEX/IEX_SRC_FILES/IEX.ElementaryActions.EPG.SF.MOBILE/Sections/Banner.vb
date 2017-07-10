Imports FailuresHandler

Public Class Banner
    Inherits IEX.ElementaryActions.EPG.SF.Banner

    Dim _UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI
    Dim PIN As String = "0000"

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
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

            _UI.Utils.ReturnToLiveViewing()

            _UI.Utils.Tap("BG")

            If Not _UI.Utils.VerifyState("ActionMenuHubStackView") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Action Bar Is On Screen"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Checking If Action Bar Is Already On Screen
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function IsActionBar() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If Action Bar Is On Screen")

        Try
            If _UI.Utils.VerifyState("ActionMenuHubStackView", 2) Then
                Msg = "ActionBar Is On Screen"
                Return True
            Else
                Msg = "ActionBar Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

    ''' <summary>
    '''   Getting Event Time Left To End
    ''' </summary>
    ''' <param name="TimeLeft">Returnes The Time Left Until End Of Event In Minutes</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>337 - ParseEventTimeFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub GetEventTimeLeft(ByRef TimeLeft As Integer)
        Dim EndTime As String = ""
        Dim EPGDateTime As String = ""
        Dim ReturnedEpgTime As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Event Time Left To End")

        Try
            _UI.Utils.GetEpgInfo("epgTime", EPGDateTime)

            _UI.Utils.ParseEPGTime(EPGDateTime, ReturnedEpgTime)

            GetEventEndTime(EndTime)

            Try
                TimeLeft = _UI.Utils._DateTime.SubtractInSec(CDate(EndTime), CDate(ReturnedEpgTime))
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParseEventTimeFailure, "Failed To Substract Time EndTime : " + EndTime + " EpgTime : " + ReturnedEpgTime))
            End Try

            Msg = "Time Left To End : " + TimeLeft.ToString
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
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

        _UI.Utils.StartHideFailures("Getting Event Start Time From Action Bar")

        Try
            _UI.Utils.GetEpgInfo("sttime", StartTime)
            _UI.Utils.ParseEventTime(StartTime, StartTime, True)
            Msg = "Event Start Time : " + StartTime
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
    ''' <param name="EndTime">Returns End Time From Action Bar</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overrides Sub GetEventEndTime(ByRef EndTime As String)
        Dim EvTime As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Event End Time From Action Bar")

        Try
            _UI.Utils.GetEpgInfo("endtime", EndTime)
            _UI.Utils.ParseEventTime(EndTime, EndTime, False)
            Msg = "Event End Time : " + EndTime
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    ''' <summary>
    '''    Getting Event Name From Action Bar
    ''' </summary>
    ''' <param name="EventName">Returns EventName From Action Bar</param>
    ''' <remarks></remarks>
    Overrides Sub GetEventName(ByRef EventName As String)
        Dim returnedValue As String = ""
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Getting Event Name From Action Bar")

        Try
            _UI.Utils.GetEpgInfo("evtNa", EventName)
            Msg = "Event Name : " + EventName
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

    Public Overrides Sub PreRecordEvent(Optional ByVal IsSeries As Boolean = False)
        Exit Sub
    End Sub

    Public Overrides Sub RecordEvent()
        _UI.Utils.Tap("RECORD", VerifyAnimation:=False, WaitAfterTap:=15000)
    End Sub

    Public Overrides Sub LockChannel(Optional ByVal EnterPIN As Boolean = True)
        _UI.Utils.Tap("LOCK CHANNEL", VerifyAnimation:=False)
        If Not _UI.Utils.VerifyState("CDPinCodeVerificationView") Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify PIN Request Is On Screen"))
        End If
        If EnterPIN Then
            _UI.Utils.EnterPin("")
            If Not IsLocked() Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockChannelFailure, "Failed To Verify Channel Is Locked"))
            End If
        End If
    End Sub

    Public Overrides Sub UnLockChannel()
        _UI.Utils.Tap("UNLOCK CHANNEL", VerifyAnimation:=False)
        If Not _UI.Utils.VerifyState("CDPinCodeVerificationView") Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify PIN Request Is On Screen"))
        End If
        _UI.Utils.EnterPin("")
        If IsLocked() Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.LockUnlockChannelFailure, "Failed To Verify Channel Is UnLocked"))
        End If
    End Sub

    Public Overrides Sub AddToFavourites()
        _UI.Utils.StartHideFailures("Adding Channel To Favorites")

        Try
            _UI.Utils.Tap("ADD TO FAVORITE CHANNELS", VerifyAnimation:=False, WaitAfterTap:=4000)
            If Not IsFavorite() Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is Favorite"))
            End If

            _UI.Utils.ReturnToLiveViewing()
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub RemoveFromFavourites()
        _UI.Utils.StartHideFailures("Removing Channel From Favorites")

        Try
            _UI.Utils.Tap("REMOVE FROM FAVORITES", VerifyAnimation:=False)
            If IsFavorite() Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Channel Is Not Favorite"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''     Stopping Recording Event From Action Bar
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub CancelBooking(Optional IsSeries As Boolean = False, Optional IsComplete As Boolean = False)
        _UI.Utils.StartHideFailures("Canceling Booked Event From Action Bar")

        Try
            _UI.Utils.Tap("CANCEL RECORDING", VerifyAnimation:=False)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overrides Sub IsRecording()
        _UI.Utils.StartHideFailures("Verifying Event Is Recording")

        Try
            _UI.Utils.GetMobileEPGInfo("CANCEL RECORDING")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Function IsLocked() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If Channel Is Locked")

        Try
            _UI.Utils.GetMobileEPGInfo("UNLOCK CHANNEL")
            Msg = "Channel Is Locked"
            Return True
        Catch
            Msg = "Channel Is Not Locked"
            Return False
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    Public Overrides Function IsFavorite() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If Channel Is Favorite")

        Try
            _UI.Utils.GetMobileEPGInfo("REMOVE FROM FAVORITES")
            Msg = "Channel Is Favorite"
            Return True
        Catch
            Msg = "Channel Is Not Favorite"
            Return False
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function
End Class
