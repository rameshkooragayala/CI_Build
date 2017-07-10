Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.UPC.Guide

    Dim _UI As IEX.ElementaryActions.EPG.SF.IPC.UI

     Private _Utils As EPG.SF.IPC.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub
	
	Public Overrides Sub TypeKeys(ByVal ChannelNumber As String, Optional ByVal VerifyFas As Boolean = True)
        Dim ActualLines As New ArrayList
        Dim Milestones As String = ""

        _Utils.StartHideFailures("Entering " + ChannelNumber.ToString)

        Try
            If VerifyFas Then
                'MILESTONES MESSAGES
                Milestones = _Utils.GetValueFromMilestones("GuideSurfDefault")

                _Utils.BeginWaitForDebugMessages(Milestones, 20)
            End If

            _Utils.SendChannelAsIRSequence(ChannelNumber, 500)

            If Not _Utils.VerifyDebugMessage("dca_number", ChannelNumber, 20, 20) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify Grid Is On Channel : " + ChannelNumber.ToString))
            End If
            If VerifyFas Then
                If Not _Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.TuneToChannelFailure, "Failed To Verify ChannelSurf Milestones : " + Milestones))
                End If
            Else
                _UI.Live.VerifyChannelNumber(ChannelNumber)
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


End Class
