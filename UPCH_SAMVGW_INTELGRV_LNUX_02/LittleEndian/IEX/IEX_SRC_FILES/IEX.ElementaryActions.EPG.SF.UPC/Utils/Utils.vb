Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.Utils

    Dim _UI As EPG.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub

    Public Overrides Sub SwitchRF()
        Dim RFValue As String = ""
        Dim res As IEXGateway._IEXResult

        Try
            RFValue = GetValueFromEnvironment("RFPort")
        Catch ex As Exception
            Exit Sub
        End Try

        _UI.Utils.StartHideFailures("Switching RF To Stream = " + RFValue)
        Try
            If RFValue.ToLower = "um" Then
                LogCommentImportant("RF Port Connected To UM Signal By Connecting To B Instance 1")
                res = _iex.RF.ConnectToB("1")
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
            ElseIf RFValue.ToLower = "nl" Then
                LogCommentImportant("RF Port Connected To NL Signal By Connecting To A Instance 1")
                res = _iex.RF.ConnectToA("1")
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If
            Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Stream " + RFValue + " Not Supported Please Check Your Environment.ini"))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
	
	 ''' <summary>
    '''   Parsing Event Time From Returned EPG Time
    ''' </summary>
    ''' <param name="ReturnedTime">The Returned Time After Parsing</param>
    ''' <param name="TimeString">Time String From EPG</param>
    ''' <param name="IsStartTime">If True Returnes Start Time Else Returns End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub ParseEventTime(ByRef ReturnedTime As String, ByVal TimeString As String, ByVal IsStartTime As Boolean)
        Dim StartTime As String = TimeString
        Dim EndTime As String = TimeString
        Dim EvBothTimes As String()
        Dim EvtCmpltInfo As String()

        Try
            If IsStartTime Then
                EvtCmpltInfo = StartTime.Split("/")
                Try
                    EvtCmpltInfo(0) = EvtCmpltInfo(1).Replace(" > ", " ").Replace(">", " ").Replace(" - ", " ").Replace("-", " ")
                Catch
                    EvtCmpltInfo(0) = EvtCmpltInfo(0).Replace(" > ", " ").Replace(">", " ").Replace(" - ", " ").Replace("-", " ")
                End Try
                EvtCmpltInfo(0) = EvtCmpltInfo(0).Trim()
                EvBothTimes = EvtCmpltInfo(0).Split(" ")
                If EvBothTimes.Count > 1 Then
                    ReturnedTime = EvBothTimes(0).Trim()
                    LogCommentInfo("Got -> " + ReturnedTime.ToString)
                    Exit Sub
                End If
            Else
                EvtCmpltInfo = StartTime.Split("/")
                Try
                    EvtCmpltInfo(0) = EvtCmpltInfo(1).Replace(" > ", " ").Replace(">", " ").Replace(" - ", " ").Replace("-", " ")
                Catch
                    EvtCmpltInfo(0) = EvtCmpltInfo(0).Replace(" > ", " ").Replace(">", " ").Replace(" - ", " ").Replace("-", " ")
                End Try
                EvtCmpltInfo(0) = EvtCmpltInfo(0).Trim()
                EvBothTimes = EvtCmpltInfo(0).Split(" ")
                If EvBothTimes.Count > 1 Then
                    ReturnedTime = EvBothTimes(1).Trim()
                    LogCommentInfo("Got -> " + ReturnedTime.ToString)
                    Exit Sub
                End If

            End If
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse Event " + IIf(IsStartTime, "Start ", "End ") + "Time From " + TimeString + " : " + ex.Message))
        End Try
    End Sub
	
	 ''' <summary>
    '''   Gets The Selected Event Start Time From Archive
    ''' </summary>
    ''' <param name="EventDetails">Returns The Selected Event Start Time</param>
    ''' <remarks></remarks>
    Public Sub GetSelectedEventStartTime(ByRef EventDetails As String)
        Dim Msg As String = ""
        Dim ArrEventDetails As Array
        StartHideFailures("Get Selected Event Time From Archive")

        Try
            GetEpgInfo("evtdetails", EventDetails)
            Msg = "Got -> " + EventDetails.ToString
            _iex.Wait(1)
            ArrEventDetails = EventDetails.Split("/")
            EventDetails = ArrEventDetails(1).ToString.Trim()
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                LogCommentInfo(Msg)
            End If
        End Try
    End Sub
End Class


