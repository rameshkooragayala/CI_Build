Imports FailuresHandler

Public Class Menu
    Inherits IEX.ElementaryActions.EPG.SF.Menu

    Dim _UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''    Navigating To Menu By Pressing Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        _UI.Utils.StartHideFailures("Navigating To Menu")

        Try
            If IsMenu() Then
                Exit Sub
            End If

            Do
                _UI.Utils.Tap("BG", VerifyAnimation:=False, WaitAfterTap:=2000)
            Loop Until IsMenuButtonOnScreen()

            _UI.Utils.Tap("CanalDHomeButton", VerifyAnimation:=True, WaitAfterTap:=2000)

            If Not _UI.Utils.VerifyState("MainHub") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Main Menu Is On Screen"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''    Navigating To TV On Main Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToTV()
        _UI.Utils.StartHideFailures("Navigating To TV")

        Try

            If IsTV() Then
                Exit Sub
            End If

            Navigate()

            _UI.Utils.Tap("TV")

            If Not _UI.Utils.VerifyState("TvHub") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify TV Is On Screen"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checks If TV Is On The Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function IsTV() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If TV Is On The Screen")

        Try
            If _UI.Utils.VerifyState("TvHub", 2) Then
                Msg = "TV Is On Screen"
                Return True
            Else
                Msg = "TV Is Not On Screen"
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
    '''   Checking If Menu Button Is On Screen
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Private Function IsMenuButtonOnScreen() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If Menu Button Is On Screen")

        Try
            If DirectCast(_UI.Utils.StaticParam("EPG_Info"), Dictionary(Of String, String)).ContainsKey("CanalDHomeButton") Then
                Msg = "Menu Button Is On Screen"
                Return True
            Else
                Msg = "Menu Button Is Not On Screen"
                Return False
            End If
        Catch ex As Exception
            Msg = "Menu Button Is Not On Screen"
            Return False
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    ''' <summary>
    '''   Checks If The Menu Is On The Screen 
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function IsMenu() As Boolean
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Checking If Menu Is On The Screen")

        Try
            If _UI.Utils.VerifyState("MainHub", 2) Then
                Msg = "Menu Is On Screen"
                Return True
            Else
                Msg = "Menu Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    Public Overrides Sub TuneToChannel(ByVal ChannelName As String)
        Dim Coordinate As String = ""

        _UI.Utils.StartHideFailures("Tunning To " + ChannelName)

        Try
            _UI.Utils.FindInEvents("TV.Live.Events", ChannelName)
            _UI.Utils.Tap(ChannelName, "TV.Live.Events")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
