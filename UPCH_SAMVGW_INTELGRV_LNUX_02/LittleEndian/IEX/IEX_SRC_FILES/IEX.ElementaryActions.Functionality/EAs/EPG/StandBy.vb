Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Entering Or Existing Standby
    ''' </summary>
    Public Class StandBy
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _IsOn As Boolean

        ''' <param name="IsOn">If True Exiting Standby Else Entering</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>328 - INIFailure</para> 
        ''' <para>358 - StandByFailure</para> 
        ''' </remarks>
        Sub New(ByVal IsOn As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            Me._IsOn = IsOn
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            EPG.Utils.LogCommentInfo("StandBy EA :" + IIf(Me._IsOn, "Exiting ", "Entering ") + "Standby")

            EPG.Utils.Standby(Me._IsOn)

        End Sub
    End Class

End Namespace
