Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Surfing To A Channel In Guide
    ''' </summary>
    Public Class SurfToChannelInGuide
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumber As String

        ''' <param name="ChannelNumber">Channel Number To Tune To</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>a
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' </remarks>
        Sub New(ByVal ChannelNumber As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNumber = ChannelNumber
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            If _ChannelNumber = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "ChannelNumber Can't Be Empty"))
            End If

            If Not EPG.Guide.IsGuide Then
                EPG.Guide.Navigate()
            End If

            EPG.Utils.TypeKeys(_ChannelNumber, 10000)

            EPG.Live.VerifyChannelNumber(Me._ChannelNumber)

        End Sub

    End Class

End Namespace