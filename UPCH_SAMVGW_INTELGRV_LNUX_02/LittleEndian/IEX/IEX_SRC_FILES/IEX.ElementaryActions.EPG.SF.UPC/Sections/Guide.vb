Imports FailuresHandler

Public Class Guide
    Inherits IEX.ElementaryActions.EPG.SF.Guide

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''   Navigating To RECORD On Action Bar From Guide By Pressing RECORD
    ''' </summary>
    ''' <param name="IsCurrent">Optional Parameter Default = True : If True Pressing RED Else Pressing Select For Future Events</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
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

                _UI.Utils.SendIR("RECORD")

                If Not _UI.Utils.VerifyState("CONFIRM RECORDING", 30) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify State Is CONFIRM RECORDING"))
                End If

                EpgText = _UI.Utils.GetValueFromDictionary("DIC_ACTION_LIST_RECORD_SERIE_EVENT")

                _UI.Utils.StartHideFailures("Trying To Navigate To " + EpgText)
                Try
                    Try
                        _UI.Menu.SetActionBarSubAction(EpgText)
                    Catch ex As Exception

                        _UI.Utils.LogCommentFail("Failed To Set " + EpgText + " On Action Bar")
                    End Try

                Finally
                    _iex.ForceHideFailure()
                End Try

            Else
                SelectEvent()
                _UI.Banner.PreRecordEvent()

            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

End Class
