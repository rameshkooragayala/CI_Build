Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.Guide

    Dim _UI As IEX.ElementaryActions.EPG.SF.TN.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub


    ''' <summary>
    '''   Navigating To RECORD On Action Bar From Guide By Pressing RED/SELECT
    ''' </summary>
    ''' <param name="IsCurrent">Optional Parameter Default = True : If True Pressing RED Else Pressing Select For Future Events</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToRecordEvent(Optional ByVal IsCurrent As Boolean = True)
        'EPG TEXT
        Dim EpgText As String = ""

        _UI.Utils.StartHideFailures("Navigating To RECORD On Guide")

        Try
            If IsCurrent Then

                _UI.Utils.SendIR("SELECT")

                _UI.Utils.StartHideFailures("Checking If PIN Is Requested")
                If _UI.Utils.VerifyDebugMessage("state", "NewPinState", 5, 1) Then
                    _UI.Utils.EnterPin("")
                    _UI.Utils.SendIR("SELECT")
                End If
                _iex.ForceHideFailure()

                If Not _UI.Utils.VerifyState("ACTION BAR", 30) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify State Is ACTION BAR"))
                End If

                _UI.Banner.PreRecordEvent()

            Else
                SelectEvent()
                _UI.Banner.PreRecordEvent()

            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Tunning To Channel On Guide
    ''' </summary>
    ''' <param name="ChannelNumber">The Requested Channel To Tune To</param>
    ''' <param name="VerifyFas">Optional Default = True : If True Verify ChannelSurf FAS Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Overrides Sub NavigateToChannel(ByVal ChannelNumber As String, Optional ByVal VerifyFas As Boolean = True)
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Tuning To Channel " + ChannelNumber.ToString + " On Guide")
        Try

            TypeKeys(ChannelNumber, VerifyFas)

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _UI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Sub

End Class
