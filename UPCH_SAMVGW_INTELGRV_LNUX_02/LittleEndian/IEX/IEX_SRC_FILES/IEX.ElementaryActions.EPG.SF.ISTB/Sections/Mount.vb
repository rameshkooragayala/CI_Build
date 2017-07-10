Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.ISTB.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    Public Overrides Function GetFlashCommand(ByVal IsGw As Boolean, Optional ByVal IsLastDelivery As Boolean = False) As String
        Dim FlashCommand As String = ""
        Dim Path As String = ""
        If IsLastDelivery = True Then
            Path = _UI.Utils.GetValueFromEnvironment("FlashCommand_LastBinary")
            FlashCommand = Path + "./flash_update.exe /host/bzImage emmc"
        Else
            FlashCommand = _UI.Utils.GetValueFromEnvironment("FlashCommand")
            FlashCommand += "./flash_update.exe /host/bzImage emmc"
        End If

        Return FlashCommand
    End Function
	
	 ''' <summary>
    '''   Waits For Standby Power Screen To Appear
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function WaitForStandbyPowerScreen() As Boolean
        Dim EpgText As String = ""

            _UI.Utils.StartHideFailures("Checking If Standby Power mode Screen Appears...")
			
            Try 
                EpgText = _UI.Utils.GetValueFromDictionary("DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_REJECT") 
            Catch ex1 As Exception 
                _UI.Utils.LogCommentFail("EPG Text For DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_REJECT Not Found In Dictionary") 
                Return False 
            End Try
        

        If _UI.Utils.VerifyDebugMessage("title", EpgText, 600, 2) Then 
            Return True 
        End If 
        _iex.ForceHideFailure() 

        Return False 


    End Function
End Class