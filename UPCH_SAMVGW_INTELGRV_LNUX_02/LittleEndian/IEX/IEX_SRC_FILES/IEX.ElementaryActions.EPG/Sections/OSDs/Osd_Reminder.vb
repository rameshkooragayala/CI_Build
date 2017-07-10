Public Class OSD_Reminder
    Protected _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim _UI As EPG.UI

    Sub New(ByVal pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
    End Sub

    Overridable Function IsReminder() As Boolean
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement localy in project.")
        Return False
    End Function

    Overridable Sub VerifyEventName(ByVal EventName As String)
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub AcceptReminder()
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub RejectReminder()
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub VerifyReminderDismissed()
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Sub VerifyReminderAppeared()
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement localy in project.")
    End Sub

    Overridable Function VerifyIsOsd() As Boolean
        Return _UI.OSD.VerifyIsOsd
    End Function

    Overridable Function VerifyTextOnOsd(ByVal Text As String) As Boolean
        Return _UI.OSD.VerifyTextOnOsd(Text)
    End Function

    Overridable Function VerifyNumberOsd() As Boolean
        Return _UI.OSD.VerifyNumberOsd
    End Function

    Overridable Function ConfirmOsd() As Boolean
        Return _UI.OSD.ConfirmOsd
    End Function

    Overridable Function CancelOsd() As Boolean
        Return _UI.OSD.CancelOsd

    End Function
    Overridable Sub TuneValidationOnReminder(ByVal CurrentChannel As String, ByVal EventChannel As String)
        _iex.LogComment("This UI._Osd function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
   
End Class
