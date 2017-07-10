Namespace EAImplementation

    ''' <summary>
    '''   Return To Live Viewing By Pressing MENU And Then SELECT
    ''' </summary>
    Public Class ReturnToLiveViewing
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _CheckForVideo As Boolean

        ''' <param name="CheckForVideo">Optional Parameter Default = False. If True Checks For Video After Returnning To Live Viewing</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal CheckForVideo As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._CheckForVideo = CheckForVideo
        End Sub

        Protected Overrides Sub Execute()

            EPG.Utils.ReturnToLiveViewing(_CheckForVideo)

        End Sub

    End Class

End Namespace