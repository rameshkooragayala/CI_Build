Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Renting Current PPV Event From Banner
    ''' </summary>
    Public Class RentCurrentPPVEventFromBanner
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

            EPG.ChannelBar.Navigate()

            EPG.ChannelBar.SelectEvent()

            EPG.Banner.RentPpvEvent(True)

        End Sub

    End Class

End Namespace
