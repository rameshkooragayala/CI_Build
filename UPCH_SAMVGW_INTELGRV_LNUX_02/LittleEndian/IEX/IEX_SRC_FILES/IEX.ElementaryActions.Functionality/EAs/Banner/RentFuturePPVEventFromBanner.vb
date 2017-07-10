Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Renting Future PPV Event From Banner
    ''' </summary>
    Public Class RentFuturePPVEventFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>304 - IRVerificationFailure</para>
        ''' <para>328 - INIFailure</para>
        ''' <para>347 - SelectEventFailure</para>
        ''' <para>369 - RentPPVEventFailure</para>
        ''' </remarks>

        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            EPG.ChannelBar.NextEvent(True)

            EPG.Banner.RentPpvEvent(False)

        End Sub

    End Class

End Namespace
