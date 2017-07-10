Public Class Mount
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Mount

    Dim _UI As IEX.ElementaryActions.EPG.SF.IPC.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.IPC.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Gets The Mount Command And Adding Values Needed To Mount
    ''' </summary>
    ''' <param name="IsFormat">If True Adds FORMAT FORMAT_FLASH FOUR_K</param>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public Overrides Function GetMountCommand(ByVal IsFormat As Boolean) As String
        Dim MountCommand As String = ""

        MountCommand = _UI.Utils.GetValueFromEnvironment("MountCommand")
        MountCommand = MountCommand + " " + IIf(IsFormat, "FORMAT", "NOCLEAN")

        Return MountCommand
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
                EpgText = _UI.Utils.GetValueFromDictionary("DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_ACCEPT") 
            Catch ex1 As Exception 
                _UI.Utils.LogCommentFail("EPG Text For DIC_FIRST_INSTALL_LEGAL_DISCLAIMER_ACCEPT Not Found In Dictionary") 
                Return False 
            End Try
        

        If _UI.Utils.VerifyDebugMessage("title", EpgText, 600, 2) Then 
            Return True 
        End If 
        _iex.ForceHideFailure() 

        Return False 


    End Function
    ''' <summary>
    '''   Handles All The Screens Before LIVE/MENU Arrive
    ''' </summary>
    ''' <param name="_IsGw">If True Handles GW First Screens Else The Client</param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function HandleFirstScreens(ByVal _IsGw As Boolean) As Boolean
        Dim CheckLanguageSelection As Boolean = True
        Dim CheckLegalDisclaimer As Boolean = False

        Try
            CheckLanguageSelection = CBool(_UI.Utils.GetValueFromEnvironment("CheckLanguageSelection"))
        Catch ex As Exception
            CheckLanguageSelection = True
        End Try

        Try
            CheckLegalDisclaimer = CBool(_UI.Utils.GetValueFromEnvironment("CheckLegalDisclaimer"))
        Catch ex As Exception
            CheckLegalDisclaimer = True
        End Try

        If CheckLanguageSelection Then

            If Not _UI.Mount.WaitForFirstScreen() Then
                _UI.Utils.LogCommentFail("Failed To Verify First Screen Arrived")
                Return False
            End If

            If Not _IsGw Then
                Try
                    _UI.Menu.SetCountry("NETHERLANDS")
                Catch ex As Exception
                    _UI.Utils.LogCommentFail("Failed To Verify Country Is NETHERLANDS : " + ex.Message)
                    Return False
                End Try

                _UI.Utils.SendIR("SELECT")
            End If

            Try
                _UI.Menu.SetLanguage("English")
            Catch ex As Exception
                _UI.Utils.LogCommentFail("Failed To Verify Language Is English : " + ex.Message)
                Return False
            End Try

            _iex.Wait(5)
            _UI.Utils.SendIR("SELECT")

            _UI.Mount.HandlePinScreens()

            If CheckLegalDisclaimer Then
                If WaitForLegalDisclaimer() Then
                    _UI.Utils.SendIR("SELECT")
                End If
            End If

        End If

        If WaitForStandbyPowerScreen() Then
            _UI.Utils.SendIR("SELECT")
        Else
            _UI.Utils.LogCommentInfo("Sending IR To Restore The Menu") 'Waited For Standby Power Screen Unsuccessfully, So Meanwhile Menu Is Down, Therefore Need To Get It Up Again
            _UI.Utils.SendIR("MENU")
        End If

        Return True

    End Function
End Class