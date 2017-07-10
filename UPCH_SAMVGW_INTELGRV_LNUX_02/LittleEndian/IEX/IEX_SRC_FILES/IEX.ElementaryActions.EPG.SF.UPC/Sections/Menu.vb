Imports FailuresHandler

Public Class Menu
    Inherits IEX.ElementaryActions.EPG.SF.Menu

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''  Sets Manual Recording Channel On Channels List
    ''' </summary>
    ''' <param name="Channel">Requested Channel</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Overrides Sub SetManualRecordingChannel(ByVal Channel As String)
        _UI.Utils.StartHideFailures("Setting Manual Recording Channel To -> " + Channel.ToString)

        Try
            Dim CurrentChannel As String = ""
            Dim CheckedChannel As String = "Empty"
            Dim FirstChannel As String = ""
            Dim SameItemTimes As Integer = 3

            _UI.Utils.GetEpgInfo("title", FirstChannel)

            If FirstChannel = Channel Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstChannel = CheckedChannel And (SameItemTimes = 2 Or SameItemTimes = 0)

                _UI.Utils.GetEpgInfo("title", CurrentChannel)

                _UI.Utils.SendIR("SELECT_DOWN")

                _UI.Utils.GetEpgInfo("title", CheckedChannel)

                If CurrentChannel = CheckedChannel Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _UI.Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN CHANNEL SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedChannel = Channel Then
                    Exit Sub
                End If
            Loop


            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Manual Recording Channel To : " + Channel))

        Finally
            _iex.ForceHideFailure()
        End Try
     
    End Sub

End Class
