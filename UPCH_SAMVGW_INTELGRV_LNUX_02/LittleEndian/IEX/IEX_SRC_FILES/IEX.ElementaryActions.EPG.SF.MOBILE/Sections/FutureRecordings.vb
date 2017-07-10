Imports FailuresHandler

Public Class FutureRecordings
    Inherits IEX.ElementaryActions.EPG.SF.FutureRecordings

    Dim _uUI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.MOBILE.UI)
        MyBase.New(_pIex, pUI)
        _uUI = pUI
    End Sub

     ''' <summary>
    '''   Navigating To Planner
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()
        Try
            _uUI.Utils.StartHideFailures("Navigating To Planner")

            If isPlanner() Then
                Exit Sub
            End If

            _uUI.Menu.Navigate()
            _uUI.Utils.Tap("LIBRARY")
            _uUI.Utils.ClearEPGInfo()
            _uUI.Utils.Tap("FUTURE RECORDING")

            If Not _uUI.Utils.VerifyState("FullContentFUTURE RECORDING") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Verify Planner Is On Screen"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Function isPlanner() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Planner Is On Screen")

        Try
            If _uUI.Utils.VerifyState("FullContentFUTURE RECORDING", 2) Then
                Msg = "Planner Is On Screen"
                Return True
            Else
                Msg = "Planner Is Not On Screen"
                Return False
            End If

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    ''' <summary>
    '''   Checks If Planner Has No Events
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function isEmpty() As Boolean
        Dim Msg As String = ""

        _uUI.Utils.StartHideFailures("Checking If Planner Is Empty")

        Try
            Dim EventName As String = ""

            Try
                _uUI.Utils.GetEpgInfo("evtNa", EventName)
                Msg = "Planner Is Not Empty"
                Return False
            Catch ex As EAException
                Msg = "Planner Is Empty !!!"
                Return True
            End Try
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _uUI.Utils.LogCommentInfo(Msg)
            End If
        End Try

    End Function

    Public Overrides Sub SelectEvent(Optional EventName As String = "", Optional IsSeries As Boolean = False)
        _uUI.Utils.FindInEvents("Planner.Events", EventName)
        _uUI.Utils.Tap("Planner.Events", EventName)
    End Sub

    Public Overrides Sub FindEvent(ByVal EventName As String, Optional ByVal EventDate As String = "", Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "")
        _uUI.Utils.FindInEvents("Planner.Events", EventName)
    End Sub

    Public Overrides Sub CancelEvent(Optional shouldSucceed As Boolean = True, Optional IsSeriesEvent As Boolean = False, Optional IsComplete As Boolean = False)
        Try
            _uUI.Banner.CancelBooking()
        Catch ex As Exception
            If shouldSucceed Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.CancelEventFailure, "Failed To Cancel Event From Planner"))
            End If
        End Try

    End Sub
End Class

