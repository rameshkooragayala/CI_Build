Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Sets Subtitles Language From Action Bar
    ''' </summary>
    Public Class SubtitlesLanguageChange
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Language As EnumLanguage
        Private subtitletype As String

        ''' <param name="Language">Language To Set To By Enum Language</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>319 - SetSubtitlesLanguageFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' </remarks>
        Sub New(ByVal Language As EnumLanguage, ByVal pManager As IEX.ElementaryActions.Functionality.Manager, Optional ByVal _Type As EnumSubtitleType = EnumSubtitleType.NORMAL)
            Me._manager = pManager
            EPG = Me._manager.UI
            subtitletype = _Type
            _Language = Language
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim Language As String

            Language = EPG.Utils.TranslateLanguage(_Language.ToString)

            If subtitletype = EnumSubtitleType.HARD_OF_HEARING Then

                Language = Language + "-" + "SUBTITLES FOR HARD OF HEARING"
            End If

            Dim state As String = ""
            EPG.Utils.GetActiveState(state)
            If state = "LIVE" Then
                EPG.Banner.Navigate(False)
            ElseIf state = "PLAYBACK" Then
                EPG.Banner.Navigate(True)
            End If

            EPG.Banner.SetSubtitlesLanguage(Language.ToUpper, state)

        End Sub

    End Class

End Namespace