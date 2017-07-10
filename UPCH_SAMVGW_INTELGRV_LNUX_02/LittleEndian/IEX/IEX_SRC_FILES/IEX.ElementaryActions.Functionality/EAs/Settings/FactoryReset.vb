Imports FailuresHandler

Namespace EAImplementation
    Public Class FactoryReset
        Inherits IEX.ElementaryActions.BaseCommand

        Private SaveRecordingsScreen As String
        Private KeepCurrentSettingsScreen As String
        Private ConfirmDeleteRecordingScreen As String
        Private ConfirmFactoryResetScreen As String
        Private PVRErrorScreen As String
        Private PopUpType As String
        Private _SaveRecordings As Boolean
        Private _KeepCurrentSettings As Boolean
        Private _PinCode As String
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI

        Sub New(ByVal SaveRecordings As Boolean, ByVal KeepCurrentSettings As Boolean, ByVal PinCode As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _SaveRecordings = SaveRecordings
            _KeepCurrentSettings = KeepCurrentSettings
            _PinCode = PinCode
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway._IEXResult

            SaveRecordingsScreen = EPG.Utils.GetValueFromProject("FACTORY_RESET", "LOG_SAVE_RECORD_SCREEN")
            KeepCurrentSettingsScreen = EPG.Utils.GetValueFromProject("FACTORY_RESET", "LOG_KEEP_SETTING_SCREEN")
            ConfirmDeleteRecordingScreen = EPG.Utils.GetValueFromProject("FACTORY_RESET", "LOG_CONFIRM_DELETE_REC_SCREEN")
            ConfirmFactoryResetScreen = EPG.Utils.GetValueFromProject("FACTORY_RESET", "LOG_CONFIRM_FACTORY_RESET_SCREEN")
            PVRErrorScreen = EPG.Utils.GetValueFromProject("FACTORY_RESET", "LOG_PVR_INFO_SCREEN")

            'Navigate to Factory Reset screen
            EPG.Utils.LogCommentInfo("Navigating To Factory Reset")

            If (EPG.Utils.GetValueFromEnvironment("Project").ToUpper() = "VOO" Or EPG.Utils.GetValueFromEnvironment("Project").ToUpper() = "GET") Then
                EPG.Utils.EPG_Milestones_NavigateByName("STATE:FACTORY RESET")
            Else
                EPG.Utils.NavigateToFactoryReset()
            End If

            EPG.Utils.ClearEPGInfo()

            'PIn Entry State
            res = _manager.EnterPIN(_PinCode, "YES_NO")
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(res))
            End If

            EPG.Utils.GetEpgInfo("PopUpType", PopUpType)

            ScreenNavigation(PopUpType)

        End Sub

        Private Function ScreenNavigation(ByVal PopUpType As String)

            Select Case PopUpType
                Case SaveRecordingsScreen
                    EPG.Utils.LogCommentInfo("SaveRecording screen is displayed")
                    If (_SaveRecordings) Then
                        SelectItem("YES", PopUpType)
                    Else
                        SelectItem("NO", PopUpType)
                    End If

                Case ConfirmDeleteRecordingScreen
                    EPG.Utils.LogCommentInfo("Confirm delete recording screen is displayed")
                    SelectItem("YES", PopUpType)

                Case KeepCurrentSettingsScreen
                    EPG.Utils.LogCommentInfo("Keep/Reset Settings screen is displayed")
                    If (_KeepCurrentSettings) Then
                        SelectItem("KEEP CURRENT", PopUpType)
                    Else
                        SelectItem("RESET TO DEFAULT", PopUpType)
                    End If

                Case ConfirmFactoryResetScreen
                    EPG.Utils.LogCommentInfo("Confirm Factory Reset screen is displayed")
                    SelectItem("YES", PopUpType)

                Case PVRErrorScreen
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FactoryResetPVRFailure, "Recording Is Ongoing"))

                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Box Has Not Rebooted. Also PopType Is Null Or Empty"))
                    Return False
            End Select

            Return True

        End Function

        Private Function SelectItem(ByVal Item As String, ByRef PopUpType As String)
            Dim Milestones As String = ""
            Dim ActualLines As New ArrayList

            Milestones = EPG.Utils.GetValueFromMilestones("FasShutdown")

            EPG.Utils.EPG_Milestones_SelectMenuItem(item)

            EPG.Utils.ClearEPGInfo()

            EPG.Utils.SendIR("SELECT")

            Try
                EPG.Utils.GetEpgInfo("PopUpType", PopUpType)
                EPG.Utils.LogCommentInfo("POP UP TYPE: " + PopUpType)
            Catch ex As Exception
                'Detect reboot
                EPG.Utils.BeginWaitForDebugMessages(Milestones, 320)
                If EPG.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                    EPG.Utils.LogCommentInfo("Box has rebooted. Factory reset completed successfully")
                    Return True
                End If
            End Try

            ScreenNavigation(PopUpType)

            Return True

        End Function

    End Class

End Namespace

