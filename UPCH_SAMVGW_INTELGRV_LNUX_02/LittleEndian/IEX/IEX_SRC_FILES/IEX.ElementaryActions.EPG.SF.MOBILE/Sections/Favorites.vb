Imports FailuresHandler

Public Class Favorites
    Inherits IEX.ElementaryActions.EPG.SF.Favorites

    Dim _UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        Me._iex = _pIex
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To Favorites Settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _UI.Utils.StartHideFailures("Navigating To Favorites Settings")

        Try
            _UI.Banner.Navigate()

            _UI.Utils.Tap("SETTINGS")

            If Not _UI.Utils.VerifyState("CDSettingsMainView") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Settings Is On Screen"))
            End If

            _UI.Utils.Tap("PROFILES")

            If Not _UI.Utils.VerifyState("CDSettingsProfileMenu") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Settings Is On Screen"))
            End If

            _UI.Utils.Tap("FAVOURITE CHANNELS")

            If Not _UI.Utils.VerifyState("CDSettingsChannelsMainMenuView") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify PIN Request Is On Screen"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Setting A List Of Channel Names To Favorite/Non-Favorite
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Overrides Sub SetChannelList(ByVal listChannelName As List(Of String), ByVal SetAsFavorites As Boolean)

        Dim ChannelName As String = ""

        Dim msgHideFailures As String = If(SetAsFavorites, "Trying To Set Channel As Favorite", "Trying To Unset Channel From Being Favorite")

        _UI.Utils.StartHideFailures(msgHideFailures)

        Try
            For Each channel As String In listChannelName
                _UI.Utils.Tap(channel, "ChannelList", False)
            Next

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class