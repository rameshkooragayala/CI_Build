Imports FailuresHandler

Public Class Settings
    Inherits IEX.ElementaryActions.EPG.SF.Settings

    Dim _UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To Parental Control Lock/Unlock Channels Settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToParentalControlLockUnlock()
        _UI.Utils.StartHideFailures("Navigating To Parental Control Lock/Unlock Channels Settings")

        Try
            _UI.Banner.Navigate()

            _UI.Utils.Tap("SETTINGS")

            If Not _UI.Utils.VerifyState("CDSettingsMainView") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Settings Is On Screen"))
            End If

            _UI.Utils.Tap("THIS iPad")

            If Not _UI.Utils.VerifyState("CDSettingsIpadMenu") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Settings Is On Screen"))
            End If

            _UI.Utils.Tap("LOCKED CHANNELS")

            If Not _UI.Utils.VerifyState("CDPinCodeVerificationView") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify PIN Request Is On Screen"))
            End If

            _UI.Utils.EnterPin("")

            If Not _UI.Utils.VerifyState("CDSettingsChannelsMainMenuView") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Enter The Correct PIN"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Locking Channel In Parental Control Lock/Unlock Channels
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>357 - LockUnlockChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub LockChannel(Optional ByVal ChannelName As String = "")
        Dim Locked As String = ""
        Dim Confirm As String = ""
        Dim EpgText As String = ""
        Dim Title As String = ""
        Dim ActualLines As New ArrayList

        _UI.Utils.StartHideFailures("Locking Channel")

        Try
            _UI.Utils.BeginWaitForDebugMessages(ChannelName + ".*isLock@#$1", 5)

            _UI.Utils.Tap(ChannelName, "ChannelList")

            If Not _UI.Utils.EndWaitForDebugMessages(ChannelName + ".*isLock@#$1", ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Confirm Channel Locked"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   UnLocking Channel In Parental Control Lock/Unlock Channels
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>357 - LockUnlockChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub UnLockChannel(Optional ByVal ChannelName As String = "")
        Dim ActualLines As New ArrayList

        _UI.Utils.StartHideFailures("UnLocking Channel")

        Try
          
            _UI.Utils.BeginWaitForDebugMessages(ChannelName + ".*isLock@#$0", 5)

            _UI.Utils.Tap(ChannelName, "ChannelList")

            If Not _UI.Utils.EndWaitForDebugMessages(ChannelName + ".*isLock@#$0", ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Confirm Channel Locked"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

#Region "First Installation"

    Public Sub WaitForWelcomeScreen()
        _UI.Utils.StartHideFailures("Waiting For Welcome Screen To Appear")
        Try
            _UI.Utils.VerifyState("FirstInstallWelcomeHubStackView", 30)
            _iex.Wait(3)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Sub HandleWelcomeScreen()
        _UI.Utils.StartHideFailures("Clicking On CONTINUE")
        Try
            _UI.Utils.Tap("CONTINUE")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Sub HandleUserNameScreen()
        Dim Result As New ArrayList

        _UI.Utils.StartHideFailures("Waiting For User Name And Password Screen To Appear")
        Try
            _UI.Utils.VerifyState("CDHubStackView", 30)
        Finally
            _iex.ForceHideFailure()
        End Try

        _UI.Utils.StartHideFailures("Clicking ENTER")
        Try
            _UI.Utils.EnterString("automation" + vbLf, 2)
            _UI.Utils.Tap(427, 576, 2)
            _UI.Utils.EnterString("automation" + vbLf, 2)
        Finally
            _iex.ForceHideFailure()
        End Try

        Try
            _UI.Utils.BeginWaitForDebugMessages("anEnd", 30)
            If Not _UI.Utils.EndWaitForDebugMessages("anEnd", Result) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.AnimationFailure, "Failed To Verify Next Screen Animation End"))
            End If
            _iex.Wait(10)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Sub HandleSetTopBoxSelectionScreen()
        Dim Result As New ArrayList
        Dim title As String = ""

        _UI.Utils.GetEpgInfo("title", title)
        If title.ToUpper = "CONTINUE" Then
            _UI.Utils.StartHideFailures("Clicking On CONTINUE")
            Try
                Dim ContinueCords As String = ""
                _UI.Utils.Tap("CONTINUE")
            Finally
                _iex.ForceHideFailure()
            End Try
        Else
            _UI.Utils.StartHideFailures("Clicking On STB_0")
            Try
                _UI.Utils.Tap("STB_0", "", True, 0)
                'Dim ContinueCords As String = ""
                'Dim X As Integer = 0
                'Dim Y As Integer = 0

                '_UI.Utils.GetEpgInfo("cord", ContinueCords)
                '_UI.Utils.GetTapXY(ContinueCords, X, Y)
                '_UI.Utils.Tap(X, Y, 1000)

            Finally
                _iex.ForceHideFailure()
            End Try
        End If


    End Sub

    Public Sub HandleSetProfileScreen()
        Dim Result As New ArrayList

        '_UI.Utils.StartHideFailures("Waiting For Profile Selection Screen To Appear")
        'Try
        '    _UI.Utils.BeginWaitForDebugMessages("Please select a profile", 10)
        '    If Not _UI.Utils.EndWaitForDebugMessages("Please select a profile", Result) Then
        '        ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Set Top Box Selection Screen Appeared"))
        '    End If
        '    _iex.Wait(2)
        'Finally
        '    _iex.ForceHideFailure()
        'End Try

        'HARD CODED CORDINATIONS FOR SELECT A PROFILE
        _UI.Utils.Tap("SELECT A PROFILE", "", False)

        _UI.Utils.StartHideFailures("Clicking On HOME")
        Try
            _UI.Utils.Tap("HOME", "", True, 10000)
            'Dim ContinueCords As String = ""
            'Dim X As Integer = 0
            'Dim Y As Integer = 0

            '_UI.Utils.GetEpgInfo("cord", ContinueCords)
            '_UI.Utils.GetTapXY(ContinueCords, X, Y)
            '_UI.Utils.Tap(X, Y, 1000)

        Finally
            _iex.ForceHideFailure()
        End Try

        _UI.Utils.StartHideFailures("Waiting For Main Menu To Appear")
        Try
            _UI.Utils.VerifyState("MainHub", 10)
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Sub HandleTutorial()
        _UI.Utils.StartHideFailures("Exiting Tutorial")
        Try
            _UI.Utils.Tap("EXIT FULL SCREEN TUTORIAL", "", False)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

#End Region

End Class
