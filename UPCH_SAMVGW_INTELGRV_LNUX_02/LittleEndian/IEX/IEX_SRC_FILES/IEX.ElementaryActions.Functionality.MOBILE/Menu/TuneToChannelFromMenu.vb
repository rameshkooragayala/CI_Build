Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Tune To Channel
    ''' </summary>
    Public Class TuneToChannelFromMenu
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelName As String

        ''' <param name="ChannelName">ChannelName</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal ChannelName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelName = ChannelName
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            If _ChannelName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "ChannelNumber Can't Be Empty"))
            End If

            EPG.Menu.NavigateToTV()
            EPG.Menu.TuneToChannel(Me._ChannelName)

        End Sub

    End Class

End Namespace