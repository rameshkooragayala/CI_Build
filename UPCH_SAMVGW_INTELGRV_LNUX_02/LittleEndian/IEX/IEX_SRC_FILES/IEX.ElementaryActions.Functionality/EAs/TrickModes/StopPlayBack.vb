Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''    Stop Current Playback
    ''' </summary>
    Public Class StopPlayback
        Inherits IEX.ElementaryActions.BaseCommand
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _IsReviewBuffer As Boolean

        ''' <param name="IsReviewBuffer">Optional Parameter Default = False : If True Stop From Review Buffer Else From Playback</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>343 - StopPlayEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal IsReviewBuffer As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._IsReviewBuffer = IsReviewBuffer
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            Dim res As New IEXGateway.IEXResult

            EPG.TrickModes.RaiseTrickMode()

            EPG.TrickModes.StopPlayEvent(_IsReviewBuffer)

        End Sub

    End Class
End Namespace