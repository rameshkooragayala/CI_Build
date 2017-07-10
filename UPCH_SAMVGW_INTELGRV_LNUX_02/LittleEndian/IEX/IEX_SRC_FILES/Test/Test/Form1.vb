Imports System.Threading
Imports FailuresHandler
Imports IEX.ElementaryActions.Functionality

Public Class Form1

    Dim EA As _EA

    Public Class _EA
        Inherits IEX.ElementaryActions.Functionality.MOBILE.Manager

        Private _EA As IEX.ElementaryActions.Functionality.MOBILE.Manager
        'Inherits IEX.ElementaryActions.Functionality.Manager
        'Private _EA As IEX.ElementaryActions.Functionality.Manager
        Dim iex3 As New IEXGateway.IEX
        Dim res As IEXGateway._IEXResult

        Public Sub New()
            'res = iex3.Connect("127.0.0.1", 2)
            iex3.Connect("127.0.0.1", 4)
            Dim eerror As String = ""
            Dim passed As Boolean = Me.Init(iex3, "MOBILE", eerror)
            'Dim passed As Boolean = Me.Init(iex3, "COGECO", eerror)
            If Not passed Then
                MsgBox("Failed To Init EA")
                End
            End If
        End Sub
    End Class

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim res As IEXGateway._IEXResult
        Dim eventKeyName As String = "Event1"
        Dim speed As Double = -30

        res = EA.SignIn()
        If Not res.CommandSucceeded Then
            MsgBox("fail")
        End If

        'res = EA.PVR.BookFutureEventFromBanner(eventKeyName, VerifyBookingInPCAT:=False)
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        'End If

        'EA.UI.Menu.Navigate()



        'res = EA.PVR.VerifyRecordErrorInfo("fff", EnumRecordErr.Failed_PowerFailure, "", "", False)

        ''res = EA.FirstInstall()
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        'End If

        'res = EA.ChannelBarSurfFuture(True)
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        '    'Exit For
        'End If

        'EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME")

        'EA.TestBuildingBlocks()

        'res = EA.StandBy(True)
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        'Else

        'res = EA.PVR.SetTrickModeSpeed("RB", 0, False, False)
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        '    'Exit For
        'End If

        'res = EA.ChannelSurf(EnumSurfIn.Live, "101")
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        'End If

        'res = EA.PVR.BookFutureEventFromBanner("Event2", 1, 3, False, False)
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        'End If

        'res = EA.PVR.VerifyEventInPlanner("Event2")
        'If Not res.CommandSucceeded Then
        '    MsgBox("fail")
        'End If
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        EA = New _EA
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Button2.Enabled = False
        EA.MountGw()
        Button2.Enabled = True
    End Sub
End Class