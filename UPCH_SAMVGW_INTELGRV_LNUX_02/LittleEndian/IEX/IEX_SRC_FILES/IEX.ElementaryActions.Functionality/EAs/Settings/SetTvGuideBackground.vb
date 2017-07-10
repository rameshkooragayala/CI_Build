Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Setting The TV Guide Background (Solid/Transparent)
    ''' </summary>
    Public Class SetTvGuideBackground
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _TvGuideBackground As EnumTvGuideBackground

        ''' <param name="TvGuideBackground">TvGuideBackground - Solid/Transparent</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para>
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para>
        ''' </remarks>
        Sub New(ByVal TvGuideBackground As EnumTvGuideBackground, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._TvGuideBackground = TvGuideBackground
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            EPG.Settings.NavigateToTvGuideBackgroundSettings()

            Select Case _TvGuideBackground
                Case EnumTvGuideBackground.SOLID
                    EPG.Settings.SetTvGuideBackgroundAsSolid()
                Case EnumTvGuideBackground.TRANSPARENT
                    EPG.Settings.SetTvGuideBackgroundAsTransparent()
            End Select

            EPG.Utils.ReturnToLiveViewing()
        End Sub

    End Class

End Namespace