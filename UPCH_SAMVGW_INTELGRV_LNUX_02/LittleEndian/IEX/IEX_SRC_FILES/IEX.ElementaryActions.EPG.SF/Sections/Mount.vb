Imports FailuresHandler

Public Class Mount
    Inherits IEX.ElementaryActions.EPG.Mount

    Dim _UI As UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Wait For Client To Load By Verifying DebugTextInitialize Milestones
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function WaitForClientToLoad() As Boolean
        Dim Res As IEXGateway.IEXResult = Nothing
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList

        _Utils.LogCommentInfo("Waiting For Debug Text Initialize Milestone To Appear...")

        Milestones = _Utils.GetValueFromMilestones("DebugTextInitialize")

        _Utils.BeginWaitForDebugMessages(Milestones, 300)

        If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
            _Utils.LogCommentFail("Failed To Verify DebugTextInitialize Milestones")
            Return False
        End If

        Return True
    End Function

    ''' <summary>
    '''   Waiting For First Screen To Appear On EPG
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Overrides Function WaitForFirstScreen() As Boolean
        Dim ActualLines As New ArrayList

        _Utils.LogCommentInfo("Waiting For title To Appear...")

        '*********************** For OSD In COGECO
        _Utils.BeginWaitForDebugMessages("title", 600)

        If Not _Utils.EndWaitForDebugMessages("title", ActualLines) Then
            _Utils.LogCommentFail("Failed To Verify title Arrived")
            Return False
        End If

        Return True
    End Function

    Overrides Sub PressSelect()
        _Utils.SendIR("SELECT")
    End Sub

    ''' <summary>
    '''   Handling New PIN Screens (N/R By Default So No Implementation Below)
    ''' </summary>
    ''' <remarks></remarks>
    Overrides Sub HandlePinScreens()
        Exit Sub
    End Sub

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

        If CheckLanguageSelection And _IsGw Then

            If Not _UI.Mount.WaitForFirstScreen() Then
                _UI.Utils.LogCommentFail("Failed To Verify First Screen Arrived")
                Return False
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

    ''' <summary>
    ''' Tune to a default channel after Mount. If not handled to tune from this method, it should return false else true.
    ''' </summary>
    ''' <param name="ChNumber">Channel Number of the service where it needs to tune to</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>As it is handle to tune to channel in Functionality.MountGW.vb, so returning False here</remarks>
    Public Overrides Function TuneToDefaultChannel(ByVal ChNumber As String) As Boolean
        Return False
    End Function

End Class