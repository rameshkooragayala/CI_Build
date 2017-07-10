Namespace EAImplementation

    Public Class Dummy
        Inherits IEX.ElementaryActions.BaseCommand
        Private _toSet As Boolean
        Private _keyname As String
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        Sub New(ByVal keyname As String, ByVal toSet As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._toSet = toSet
            Me._keyname = keyname
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            If _toSet Then
                EPG.Events.Add(_keyname, New EPG.EpgEvent(EPG.Utils))
                EPG.Events(_keyname).Name = "koko"
            Else

                MsgBox(EPG.Events(_keyname).Name)

            End If


        End Sub

    End Class

End Namespace