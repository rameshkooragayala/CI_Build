Imports FailuresHandler

Public Class OSD_Reminder
    Inherits IEX.ElementaryActions.EPG.OSD_Reminder

    Dim _UI As IEX.ElementaryActions.EPG.SF.UI
    Dim res As IEXGateway._IEXResult

    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Checks If Reminder Is On Screen
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' </remarks>
    Public Overrides Function IsReminder() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Reminder Is On The Screen")

        Try
            Dim State As String = ""

            _Utils.GetEpgInfo("alarmpopupcntrlr", State)

            If State = "Launched" Then
                Msg = "Reminder Is On Screen"
                Return True
            Else
                Msg = "Reminder Is Not On Screen"
                Return False
            End If
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

    ''' <summary>
    '''   Pressing SELECT To Accept Reminder
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub AcceptReminder()
        _Utils.StartHideFailures("Accepting Reminder")

        Try
            _Utils.SendIR("SELECT", 4000)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''    Pressing BACK_UP To Reject Reminder
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub RejectReminder()

        _Utils.StartHideFailures("Rejecting Reminder")

        Try
            _Utils.SendIR("BACK_UP", 4000)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verifies The Reminder Dismissed
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>354 - ReminderFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyReminderDismissed()
        _Utils.StartHideFailures("Verifying The Reminder Dismissed")

        Try
            If Not _UI.Utils.VerifyDebugMessage("alarmpopupcntrlr", "Dismissed", 30, 2) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ReminderFailure, "Failed To Verify The Reminder Dismissed"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Verifies The Reminder Appeared
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>354 - ReminderFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyReminderAppeared()
        _Utils.StartHideFailures("Verifying The Reminder Appeared")

        Try
            If Not _UI.Utils.VerifyDebugMessage("alarmpopupcntrlr", "Launched", 90, 2) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.ReminderFailure, "Failed To Verify The Reminder Appeared"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
    ''' <summary>
    '''   Verifies The Event Name On Reminder
    ''' </summary>
    ''' <param name="EventName">The Requested Event Name To Verify</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyEventName(ByVal EventName As String)
        _Utils.StartHideFailures("Verifying Event Name On Reminder Is " + EventName.ToString)

        Try
            Dim EvName As String = ""

            _Utils.GetEpgInfo("evtname", EvName)

            If EvName <> EventName Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Event Name On Reminder"))
            End If

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Verifies The Channel Tuned after No Action taken during Reminder Notification
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub TuneValidationOnReminder(ByVal CurrentChannel As String, ByVal EventChannel As String)
        Dim Msg As String = ""
        Dim DoTuneOnReminder As String = ""
        'Fetch the ReminderTuning True/False from  Project.INI
        Try
            DoTuneOnReminder = _Utils.GetValueFromProject("REMINDER_NOTIFICATION_SETTINGS", "TUNE_ONREMINDER")

        Catch ex As EAException
            Msg = "TUNE_ONREMINDER returned Empty not defined in Project.INI,Assuming Value=False"
            _Utils.LogCommentInfo(Msg)
            DoTuneOnReminder = False
        End Try

        Dim DoTuneOnReminderFlag As Boolean = Convert.ToBoolean(DoTuneOnReminder)

        'If DoTuneOnReminder True then Tune automatically  to Reminder Channel on Reminder Timeout.
        If (DoTuneOnReminderFlag) Then
            If Not CurrentChannel = EventChannel Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetChannelFailure, "Failed To Verify Current Channel : " + CurrentChannel.ToString + " Is same as Event Channel : " + EventChannel.ToString))
            Else
                Msg = "Current channel is equal to Reminder Channel" + CurrentChannel + "=" + EventChannel
                _Utils.LogCommentInfo(Msg)
            End If
        ElseIf CurrentChannel = EventChannel Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetChannelFailure, "Failed To Verify Current Channel : " + CurrentChannel.ToString + " Is same as Event Channel : " + EventChannel.ToString))
        Else
            Msg = "Current channel is not equal to Reminder Channel" + CurrentChannel + "!=" + EventChannel
            _Utils.LogCommentInfo(Msg)
        End If

    End Sub
End Class
