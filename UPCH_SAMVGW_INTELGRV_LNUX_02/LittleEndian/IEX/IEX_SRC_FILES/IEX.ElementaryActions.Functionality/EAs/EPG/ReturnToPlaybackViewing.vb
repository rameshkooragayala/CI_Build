Namespace EAImplementation

    ''' <summary>
    '''   Return To Playback Viewing
    ''' </summary>
    Public Class ReturnToPlaybackViewing
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _CheckForVideo As Boolean

        ''' <param name="CheckForVideo">Optional Parameter Default = False. If True Checks For Video After Returnning To Playback Viewing</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>334 - VideoNotPresent</para> 
        ''' </remarks>
        Sub New(ByVal CheckForVideo As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._CheckForVideo = CheckForVideo
        End Sub

        Protected Overrides Sub Execute()
            EPG.Utils.ReturnToPlaybackViewing(_CheckForVideo)
        End Sub

    End Class

End Namespace