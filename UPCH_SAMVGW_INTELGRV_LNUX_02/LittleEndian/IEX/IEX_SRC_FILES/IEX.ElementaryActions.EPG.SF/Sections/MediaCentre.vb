Imports FailuresHandler

Public Class MediaCentre
    Inherits IEX.ElementaryActions.EPG.MediaCentre

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    ''' Navigates To Library/My Devices Screen
    ''' </summary>
    ''' <param name="_fromDeviceNavigator">Optional Parameter Default = True, else navigate directly to Music/Pictures Navigator</param>
    ''' <param name="ContentType">Optional Parameter Default = "" - ContentType for direct Navigation</param>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate(Optional ByVal _fromDeviceNavigator As Boolean = True, Optional ByVal ContentType As String = "")
        _UI.Utils.StartHideFailures("Navigating to My Devices")

        Try
            If (_fromDeviceNavigator) Then
                _UI.Utils.EPG_Milestones_NavigateByName("STATE:MY DEVICES")
            Else
                If (ContentType.ToUpper = "MUSIC") Then
                    _UI.Utils.EPG_Milestones_NavigateByName("STATE:MY MUSIC")
                ElseIf (ContentType.ToUpper = "PICTURES") Then
                    _UI.Utils.EPG_Milestones_NavigateByName("STATE:MY PHOTOS")
                End If
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Navigates To the specified content in My devices
    ''' </summary>
    ''' <param name="content">Media Content object</param>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToContent(ByVal content As MediaContent)

        Dim dictionary As Dictionary(Of EnumEpgKeys, String) = New Dictionary(Of EnumEpgKeys, String)

        _UI.Utils.StartHideFailures("Navigating to Content: " + content.Name)

        Try

            ' navigate to the device
            dictionary.Add(EnumEpgKeys.TITLE, content.Device)
            Try
                _UI.Utils.HighlightOption(_UI.Utils.EPGStateMachine.GetState("MY DEVICES"), dictionary)
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, ex.Message))
            End Try
            _UI.Utils.SendIR("SELECT", 500)

            ' Navigate to the content type
            _UI.Utils.EPG_Milestones_SelectMenuItem(content.Type)
            _UI.Utils.SendIR("SELECT", 500)

            ' Navigate to the Content Folder
            dictionary.Item(EnumEpgKeys.TITLE) = content.Folder
            Dim MCBrowserState As EpgState = _UI.Utils.EPGStateMachine.GetState("MC BROWSER")
            Try
                _UI.Utils.HighlightOption(MCBrowserState, dictionary)
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, ex.Message))
            End Try
            _UI.Utils.SendIR("SELECT", 500)

            ' Navigate to the Content name
            dictionary.Item(EnumEpgKeys.TITLE) = content.Name
            Try
                _UI.Utils.HighlightOption(MCBrowserState, dictionary)
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, ex.Message))
            End Try

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Playback Audio Content
    ''' </summary>
    ''' <param name="content">Media Content Object</param>
    ''' <remarks></remarks>
    Public Overrides Sub PlayAudioContent(ByVal content As MediaContent)
        _UI.Utils.StartHideFailures("Playing back Audio Content: " + content.Name)
        Try
            _UI.Utils.SendIR("SELECT", 500)

            If Not _UI.Utils.VerifyState("MUSIC LIBRARY") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify AUdio Trackview state"))
            End If

            _UI.Utils.SendIR("SELECT", 500)
            If Not _UI.Utils.VerifyState("PLAYBACK") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify the playback state"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Playback Video Content
    ''' </summary>
    ''' <param name="content">Media Content Object</param>
    ''' <remarks></remarks>
    Public Overrides Sub PlayVideoContent(ByVal content As MediaContent)
        _UI.Utils.StartHideFailures("Playing back Video Content: " + content.Name)
        Try
            _UI.Utils.EPG_Milestones_Navigate("ACTION BAR")

            _UI.Utils.EPG_Milestones_SelectMenuItem("PLAY")
            _UI.Utils.SendIR("SELECT", 500)

            If Not _UI.Utils.VerifyState("TRICKMODE BAR") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify video playback state"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    ''' Playback Picture content
    ''' </summary>
    ''' <param name="content"> Media Content Object</param>
    ''' <param name="PlayMode">Content Playback Mode : PLAY,SLIDESHOW</param>
    ''' <param name="PlaybackSetting">Content Playback Setting : Slideshow Setting</param>
    ''' <remarks></remarks>
    Public Overrides Sub PlayPictureContent(ByVal content As MediaContent, ByVal PlayMode As EnumMCPlayMode, Optional ByVal PlaybackSetting As String = "")

        _UI.Utils.StartHideFailures("Playing back Picture Content: " + content.Name)

        Try
            If PlayMode = EnumMCPlayMode.PLAY Then
                _UI.Utils.EPG_Milestones_Navigate("ACTION BAR")
                _UI.Utils.EPG_Milestones_SelectMenuItem("VIEW")
                _UI.Utils.SendIR("SELECT", 500)

                If Not _UI.Utils.VerifyState("SLIDE SHOW") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify the playback state"))
                End If

            ElseIf PlayMode = EnumMCPlayMode.SLIDESHOW Then

                _UI.Utils.SendIR("SELECT", 500)
                If Not _UI.Utils.VerifyState("SLIDE SHOW") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify the playback state"))
                End If

                _UI.Utils.EPG_Milestones_Navigate("ACTION BAR")
                _UI.Utils.EPG_Milestones_SelectMenuItem("START SLIDESHOW")
                _UI.Utils.SendIR("SELECT", 500)
                SetSlideshowSettings(PlaybackSetting)

                If Not _UI.Utils.VerifyState("SLIDE SHOW") Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to verify the slideshow state"))
                End If
            End If

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    ''' Setting Slideshow Interval
    ''' </summary>
    ''' <param name="SlideshowSetting">Slideshow Interval Setting</param>
    ''' <remarks></remarks>

    Public Overrides Sub SetSlideshowSettings(ByVal SlideshowSetting As String)
        _UI.Utils.StartHideFailures("Setting Slideshow Interval to:" + SlideshowSetting)

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem(SlideshowSetting)
            _UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

End Class
