Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''  Sets Subtitles On Stb Settings
    ''' </summary>
    Public Class SetSubtitlesPrefs
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private AreSubtitlesOn As Boolean
        Private LanguageToSet As EnumLanguage

        ''' <param name="AreSubtitlesOn">If True Sets To ON Else To OFF</param>
        ''' <param name="LanguageToSet">If Empty Default Else Sets Language</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal AreSubtitlesOn As Boolean, ByVal LanguageToSet As EnumLanguage, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me.AreSubtitlesOn = AreSubtitlesOn
            Me.LanguageToSet = LanguageToSet
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim Language As String

            Language = EPG.Utils.TranslateLanguage(LanguageToSet.ToString)

            EPG.Settings.SetSubtitles(AreSubtitlesOn, Language)

            EPG.Utils.ReturnToLiveViewing()

        End Sub

    End Class

End Namespace