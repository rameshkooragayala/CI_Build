Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''  Sets Reminder Notification
    ''' </summary>
    Public Class SetReminderNotifications
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _SetRemindersOn As Boolean

        ''' <param name="SetRemindersOn">If True Sets To ON Else To OFF</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal SetRemindersOn As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._SetRemindersOn = SetRemindersOn
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            If Me._SetRemindersOn Then
                EPG.Settings.SetReminderNotifications(True)
            Else
                EPG.Settings.SetReminderNotifications(False)
            End If

            EPG.Utils.ReturnToLiveViewing()

        End Sub

    End Class

End Namespace