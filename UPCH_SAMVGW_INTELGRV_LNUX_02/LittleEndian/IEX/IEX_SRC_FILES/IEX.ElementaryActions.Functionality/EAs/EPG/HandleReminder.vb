Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Handles Reminder
    ''' </summary>
    Public Class HandleReminder
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Action As EnumReminderActions

        ''' <param name="EventKeyName">The Key Of The Event</param>
        ''' <param name="Action">Can Be : Accept,Reject,Ignore Or Wait</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>308 - GetChannelFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Action As EnumReminderActions, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._Action = Action
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim EventChannel As String = ""
            Dim CurrentChannel As String = ""
            Dim ReminderRemoval As Integer = 120

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            EventChannel = EPG.Events(_EventKeyName).Channel

            If Not EPG.OSD_Reminder.IsReminder() Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Reminder Is On Screen"))
            End If

            EPG.OSD_Reminder.VerifyEventName(EventName)

            Select Case _Action
                Case EnumReminderActions.Accept
                    EPG.OSD_Reminder.AcceptReminder()
                    EPG.OSD_Reminder.VerifyReminderDismissed()
                    EPG.Menu.GetChannelNumber(CurrentChannel)

                    If CurrentChannel <> EventChannel Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetChannelFailure, "Failed To Verify Current Channel : " + CurrentChannel.ToString + " Is The Same As Event Channel : " + EventChannel.ToString))
                    End If

                Case EnumReminderActions.Reject

                    EPG.OSD_Reminder.RejectReminder()
                    EPG.OSD_Reminder.VerifyReminderDismissed()
                    EPG.Menu.GetChannelNumber(CurrentChannel)

                    If CurrentChannel = EventChannel Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetChannelFailure, "Failed To Verify Current Channel : " + CurrentChannel.ToString + " Is Different From Event Channel : " + EventChannel.ToString))
                    End If

                Case EnumReminderActions.Wait
                    EPG.Utils.LogCommentImportant("Waiting For Reminder To Disappear (" + ReminderRemoval.ToString + " Sec)")
                    _iex.Wait(ReminderRemoval)

                    If EPG.OSD_Reminder.IsReminder() Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Reminder Disappeared"))
                    End If

                    EPG.Menu.GetChannelNumber(CurrentChannel)

                    EPG.OSD_Reminder.TuneValidationOnReminder(CurrentChannel, EventChannel)

                Case EnumReminderActions.Ignore
                    EPG.Utils.LogCommentImportant("Ignoring Reminder")

                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param Action Is Not Valid Please Check"))
            End Select

        End Sub

    End Class

End Namespace
