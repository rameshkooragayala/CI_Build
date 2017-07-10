Namespace EAImplementation
    ''' <summary>
    '''   Replace The TearDown Function Of The UserLib
    ''' </summary>
    Public Class TearDown
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="pManager">Manager</param>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway._IEXResult

            res = _manager.StillAlive()

            If _manager.TelnetLogIn(Telnet1:=False, LoginToGw:=False) Then
                res = _manager.CopyLogTelnet()
            Else
                EPG.Utils.LogCommentFail("Failed To Do Telnet Login Trying To Copy Logs Through Serial")
                _manager.CopyLogSerial()
            End If

            EPG.Utils.StartHideFailures("Trying To Close All Connections")
            _iex.Debug.EndLogging()
            _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet)
            _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet2)
            _iex.ForceHideFailure()

        End Sub

    End Class

End Namespace