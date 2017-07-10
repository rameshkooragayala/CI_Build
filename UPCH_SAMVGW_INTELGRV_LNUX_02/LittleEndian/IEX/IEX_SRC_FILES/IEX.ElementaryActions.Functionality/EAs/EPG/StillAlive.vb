Imports System.IO
Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''  Checkes If STB Is Stuck Or Crashed 
    ''' </summary>
    Public Class StillAlive
        Inherits IEX.ElementaryActions.BaseCommand

        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Dim IEX As IEXGateway.IEX

        ''' <param name="pManager">Manager</param>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim LogPath As String = Path.GetFullPath(_iex.LogFileName)

            If EPG.Utils.IsSTBCrash Then
                EPG.Utils.LogCommentFail("STB Crashed")
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.STBCrashFailure, "Failed To Verify STB Is Alive"))
            Else
                EPG.Utils.LogCommentInfo("STB Alive")
            End If

        End Sub

    End Class

End Namespace