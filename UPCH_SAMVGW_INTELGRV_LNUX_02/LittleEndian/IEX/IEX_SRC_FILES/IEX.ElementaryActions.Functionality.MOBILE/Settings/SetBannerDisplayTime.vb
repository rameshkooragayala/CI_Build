Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Sets Banner Display Time On Stb Settings
    ''' </summary>
    Public Class SetBannerDisplayTime
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _DisplayInSec As EnumChannelBarTimeout

        ''' <param name="DisplayInSec">Banner Display In Seconds : 5 , 7 or 10</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal DisplayInSec As EnumChannelBarTimeout, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._DisplayInSec = DisplayInSec
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Exit Sub
        End Sub

    End Class

End Namespace