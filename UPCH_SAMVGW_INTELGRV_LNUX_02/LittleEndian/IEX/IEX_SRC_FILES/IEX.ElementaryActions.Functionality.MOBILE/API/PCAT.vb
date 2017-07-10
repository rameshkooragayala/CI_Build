Imports System.Runtime.InteropServices
Imports IEX.ElementaryActions.EPG

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class PCAT
    Inherits Functionality.PCAT

    Sub New(ByVal pIEX As IEXGateway.IEX, ByVal Manager As IEX.ElementaryActions.Manager)
        MyBase.New(pIEX, Manager)
    End Sub

End Class
