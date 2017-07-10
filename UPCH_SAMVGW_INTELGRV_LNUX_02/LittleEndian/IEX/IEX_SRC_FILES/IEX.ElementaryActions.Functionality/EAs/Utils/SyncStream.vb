Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''    Sync The Stream By Given Time
    ''' </summary>
    Public Class SyncStream
        Inherits IEX.ElementaryActions.BaseCommand
        Private _StreamStartTime As String
        Private _StreamEndTime As String
        Private _MinutesFromStreamEndToReboot As Integer
        Private _TestDurationInMin As Integer
        Private _TestStartTime As String
        Private _IsThroughTelnet As Boolean
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="StreamStartTime">The Stream Start Time</param>
        ''' <param name="StreamEndTime">The Stream End Time</param>
        ''' <param name="MinutesFromStreamEndToReboot">Minutes Before Stream End Time To Perform Power Cycle</param>
        ''' <param name="TestDurationInMin">Optional Parameter Default = -1 : On Default TestStartTime Is Checked Else The Test Duration In Minutes Is Checked</param>
        ''' <param name="TestStartTime">Optional Parameter Default = "" : The Test Requested Start Time</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>309 - GetEpgTimeFailure</para> 
        ''' <para>324 - MountFailure</para> 
        ''' <para>327 - SyncFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal StreamStartTime As String, ByVal StreamEndTime As String, ByVal MinutesFromStreamEndToReboot As Integer, ByVal TestDurationInMin As Integer, ByVal TestStartTime As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._StreamStartTime = StreamStartTime
            Me._StreamEndTime = StreamEndTime
            Me._MinutesFromStreamEndToReboot = MinutesFromStreamEndToReboot
            Me._TestDurationInMin = TestDurationInMin
            Me._TestStartTime = TestStartTime
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim Pass As Boolean = False
            Dim Reboot As Boolean = False
            Dim CurrentEpgTime As String = ""
            Dim TestEndTime As String = ""
            Dim TestStartTime As String = ""
            Dim TimeStart As Integer
            Dim TimeEnd As Integer
            Dim TimeToWait As Double
            Dim DateToWait As Date
            Dim Indx As Integer = 0

            EPG.Utils.LogCommentImportant("Syncing Stream...")

            If Me._StreamStartTime = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Stream Start Time Is EMPTY Not Valid Parameter"))
            End If

            If Me._StreamEndTime = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Stream End Time Is EMPTY Not Valid Parameter"))
            End If

            _IsThroughTelnet = CBool(EPG.Utils.GetValueFromEnvironment("MountThroughTelnet"))

            Try
                EPG.Utils.ClearEPGInfo()
            Catch ex As Exception
                EPG.Utils.LogCommentFail("Failed To Clear EPG Info : " + ex.Message)
            End Try

            If _IsThroughTelnet Then
                res = _manager.MountTelnetStb()
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            Else
                res = _manager.MountSerialStb()
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            End If

            Do Until Pass = True Or Indx = 3
                Indx += 1

                For i = 1 To 3
                    EPG.Utils.StartHideFailures("Trying To Navigate To Menu Try : " + i.ToString)
                    Try
                        EPG.Menu.Navigate()
                        Try
                            EPG.Live.GetEpgTime(CurrentEpgTime)
                            _iex.ForceHideFailure()
                            Exit For
                        Catch ex As Exception
                            _iex.ForceHideFailure()
                            _iex.Debug.EndLogging()
                            _iex.Wait(2)
                            If _IsThroughTelnet Then
                                res = _manager.MountTelnetStb(True)
                                If Not res.CommandSucceeded Then
                                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                                End If
                            Else
                                res = _manager.MountSerialStb(True)
                                If Not res.CommandSucceeded Then
                                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                                End If
                            End If
                        End Try
                    Catch ex As Exception
                        _iex.ForceHideFailure()
                        _iex.Debug.EndLogging()
                        _iex.Wait(2)
                        If _IsThroughTelnet Then
                            res = _manager.MountTelnetStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                            End If
                        Else
                            res = _manager.MountSerialStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                            End If
                        End If
                    End Try
                Next

                If CurrentEpgTime = "" Then
                    EPG.Utils.LogCommentFail("Failed To Get EPG Time")
                End If

                Try
                    TimeStart = EPG.Utils._DateTime.SubtractInSec(CDate(CurrentEpgTime), CDate(Me._StreamStartTime))
                    TimeEnd = EPG.Utils._DateTime.SubtractInSec(CDate(Me._StreamEndTime), CDate(CurrentEpgTime))

                    If TimeStart < 0 Then
                        Reboot = True
                        EPG.Utils.LogCommentInfo("Current EPG Time --> " + CurrentEpgTime.ToString + " Is Smaller Then StreamStartTime --> " + Me._StreamStartTime.ToString)
                    Else
                        Reboot = False
                    End If

                    If Reboot = False And TimeEnd < 0 Then
                        Reboot = True
                        EPG.Utils.LogCommentInfo("Current EPG Time --> " + CurrentEpgTime.ToString + " Is Over The StreamEndTime --> " + Me._StreamEndTime.ToString)
                    End If

                    If Reboot Then
                        _iex.ForceHideFailure()
                        _iex.Debug.EndLogging()
                        _iex.Wait(2)
                        If _IsThroughTelnet Then
                            res = _manager.MountTelnetStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                            End If
                        Else
                            res = _manager.MountSerialStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                            End If
                        End If

                    Else
                        EPG.Utils.LogCommentImportant("Current EPG Time --> " + CurrentEpgTime.ToString + " Is Between StreamStartTime --> " + Me._StreamStartTime.ToString + " And StreamEndTime --> " + Me._StreamEndTime.ToString)
                        Pass = True
                    End If
                Catch ex As Exception
                    Pass = False
                    EPG.Utils.LogCommentFail("Failed To Get EPG Time Exception : " + ex.Message)
                End Try
            Loop

            If Pass = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SyncFailure, "Failed To Verify EPG Time Is Within Stream Bounds"))
            End If
            Pass = False
            Dim ErrorMsg As String = ""
            If Me._TestDurationInMin <> -1 Then
                ErrorMsg = "Failed To Verify Enough Time Left For Running The Test"

                For i As Integer = 1 To 2

                    EPG.Menu.Navigate()

                    EPG.Live.GetEpgTime(CurrentEpgTime)

                    TestEndTime = CDate(CurrentEpgTime).AddMinutes(Me._TestDurationInMin).ToString("HH:mm")
                    TimeEnd = EPG.Utils._DateTime.SubtractInSec(CDate(Me._StreamEndTime), CDate(TestEndTime))

                    If TimeEnd < 0 Then

                        DateToWait = CDate(Me._StreamEndTime).AddMinutes((-1) * Me._MinutesFromStreamEndToReboot)
                        TimeToWait = EPG.Utils._DateTime.SubtractInSec(CDate(DateToWait), CDate(CurrentEpgTime))
                        If TimeToWait > 0 Then
                            EPG.Utils.LogCommentInfo("Test Duration Is -> " + Me._TestDurationInMin.ToString + " Minutes")
                            _iex.Wait(TimeToWait)
                        End If

                        _iex.ForceHideFailure()
                        _iex.Debug.EndLogging()
                        _iex.Wait(2)

                        If _IsThroughTelnet Then
                            res = _manager.MountTelnetStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                            End If
                        Else
                            res = _manager.MountSerialStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                            End If
                        End If

                        If i = 2 Then
                            EPG.Menu.Navigate()

                            EPG.Live.GetEpgTime(CurrentEpgTime)

                            TestEndTime = CDate(CurrentEpgTime).AddMinutes(Me._TestDurationInMin).ToString("HH:mm")
                            TimeEnd = EPG.Utils._DateTime.SubtractInSec(CDate(Me._StreamEndTime), CDate(TestEndTime))

                            If TimeEnd >= 0 Then
                                Pass = True
                            End If
                        End If
                    Else
                        Pass = True
                        Exit For
                    End If
                Next

            Else
                ErrorMsg = "Failed To Verify Correct Time For Test Start Is Reached"
                For i As Integer = 1 To 2

                    EPG.Menu.Navigate()

                    EPG.Live.GetEpgTime(CurrentEpgTime)

                    TimeStart = EPG.Utils._DateTime.SubtractInSec(CDate(Me._TestStartTime), CDate(CurrentEpgTime))

                    If TimeStart < 0 Then

                        DateToWait = CDate(Me._StreamEndTime).AddMinutes((-1) * Me._MinutesFromStreamEndToReboot)
                        TimeToWait = EPG.Utils._DateTime.SubtractInSec(CDate(DateToWait), CDate(CurrentEpgTime))
                        If TimeToWait > 0 Then
                            _iex.Wait(TimeToWait)
                        End If

                        _iex.ForceHideFailure()
                        _iex.Debug.EndLogging()
                        _iex.Wait(2)

                        If _IsThroughTelnet Then
                            res = _manager.MountTelnetStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountFailure, "Failed To Mount STB"))
                            End If
                        Else
                            res = _manager.MountSerialStb(True)
                            If Not res.CommandSucceeded Then
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.MountFailure, "Failed To Mount STB"))
                            End If
                        End If

                        If i = 2 Then

                            EPG.Menu.Navigate()

                            EPG.Live.GetEpgTime(CurrentEpgTime)

                            TimeStart = EPG.Utils._DateTime.SubtractInSec(CDate(Me._TestStartTime), CDate(CurrentEpgTime))

                            If TimeStart >= 0 Then
                                Pass = True
                                Exit For
                            End If
                        End If
                    Else
                        EPG.Utils.LogCommentInfo("Waiting " + (TimeStart).ToString + " Seconds Until Test Start Time : " + Me._TestStartTime)
                        _iex.Wait(TimeStart)
                        Pass = True
                        Exit For
                    End If
                Next
            End If

            If Pass = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SyncFailure, ErrorMsg))
            Else
                res = _manager.ReturnToLiveViewing(False)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ReturnToLiveFailure, "Failed To Return To Live Viewing"))
                End If
            End If

        End Sub
    End Class

End Namespace