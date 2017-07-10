Imports System.Runtime.InteropServices
Imports IEX.ElementaryActions.EPG

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class MEM
    Protected _iex As IEXGateway.IEX
    Protected _Manager As IEX.ElementaryActions.Functionality.Manager

    Sub New(ByVal pIEX As IEXGateway.IEX, ByVal Manager As IEX.ElementaryActions.Manager)
        _iex = pIEX
        _Manager = Manager
    End Sub

   ''' <summary>
    '''   Mount The STB Through Telnet Blindly Without Verification of the IEX For Release Build
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function MountTelnetStbBlind() As IEXGateway.IEXResult
        Return _Manager.Invoke("MountTelnetStbBlind", _Manager)
    End Function

    ''' <summary>
    '''   Copies All Csv Files On \NDS To Log Folder
    ''' </summary>
    ''' <returns>IEXGateway.IEXResult</returns>
    ''' <remarks></remarks>
    Public Function CopyCsvFiles() As IEXGateway.IEXResult
        Return _Manager.Invoke("CopyCsvFiles", _Manager)
    End Function

End Class