Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''    Checks If Video Is Present Or Not Present
    ''' </summary>
    Public Class CheckForVideo
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _Coordinates As String
        Private _IsPresent As Boolean
        Private _CheckFullArea As Boolean
        Private _Timeout As Integer
        Private _ISpip As Boolean

        ''' <param name="Coordinates">The Check For Video Coordinates In Format : xxx yyy www hhh</param>
        ''' <param name="IsPresent">If True Video Present Else No Video Present</param>
        ''' <param name="CheckFullArea">If True Checks As Much As Possible Of The Screen Else Checks Top Left Corner</param>
        ''' <param name="Timeout">Timeout To Check For Video Presence</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>333 - VideoPresent</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' </remarks>
        Sub New(ByVal Coordinates As String, ByVal IsPresent As Boolean, ByVal CheckFullArea As Boolean, ByVal Timeout As Integer, ByVal isPIP As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
            _IsPresent = IsPresent
            _Timeout = Timeout
            _Coordinates = Coordinates
            Me._CheckFullArea = CheckFullArea
            _ISpip = isPIP
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway.IEXResult
            Dim Area As Integer = _manager.Project.CheckForVideoArea
            Dim VideoDecoderStatPath As String
            Dim VideoDecoderValidationParam As String() = {"Started", "Buffer"}
            Dim VideoDecoderInfo As String() = {"", ""}
            Dim BufferValue As Integer() = {0, 0, 0, 0, 0}
            Dim VideoCheckCommand As String = ""
            Dim videoCheckPass As Boolean = False
            Dim timeToWait As Integer = 0

            Try
                If String.IsNullOrEmpty(_Coordinates) Then
                    If _CheckFullArea Then
                        _Coordinates = EPG.Utils.VideoCords
                    Else
                        _Coordinates = EPG.Utils.VideoCords(IsFullArea:=False, Area:=Area)
                    End If
                End If

                If _IsPresent Then
                    If _CheckFullArea Then
                        res = _iex.CheckForVideo(_Coordinates, _IsPresent, _Timeout)
                        If Not res.CommandSucceeded Then
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VideoNotPresent, "Failed To Check Video Exists On Coordinates : " + _Coordinates.ToString))
                        End If
                    Else
                        EPG.Utils.StartHideFailures("Checking Video Is On The Screen")
                        res = _iex.CheckForVideo(_Coordinates, _IsPresent, _Timeout)
                        If Not res.CommandSucceeded Then
                            res = _iex.CheckForVideo(_Coordinates, _IsPresent, _Timeout)
                            If Not res.CommandSucceeded Then
                                _iex.ForceHideFailure()
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VideoNotPresent, "Failed To Check Video Exists On Coordinates : " + _Coordinates.ToString))
                            End If
                        End If
                    End If
                    _iex.ForceHideFailure()
                End If

            Catch ex As Exception
                'Checking if video exists by verifying changing buffer values

                If Not _ISpip Then
                    Dim numOfTrials = 0
                    EPG.Utils.LogCommentWarning("Validating Video Decoder Status from procfs entries")

                    VideoDecoderStatPath = EPG.Utils.GetValueFromEnvironment("VideoDecoderStatPath")

                    If Not String.IsNullOrEmpty(VideoDecoderStatPath) Then
                        EPG.Utils.LogCommentInfo("Video Decoder status path: " + VideoDecoderStatPath)
                        VideoCheckCommand = "cat " + VideoDecoderStatPath
                        Try
                            numOfTrials = Convert.ToInt32(EPG.Utils.GetValueFromProject("VIDEO", "NUMBER_OF_VIDEO_CHECK_TRIALS"))
                        Catch ex3 As Exception
                            EPG.Utils.LogCommentInfo("NUMBER_OF_VIDEO_CHECK_TRIALS not defined so using default value")
                            numOfTrials = 3
                        End Try

                        If Me._manager.TelnetLogIn(False) Then
                            Try

                                ' Take maximum 5 samples of buffer values to check that video is changing 
                                For i As Integer = 0 To numOfTrials - 1
                                    Try
                                        Me._manager.SendCmd(VideoCheckCommand, False, VideoDecoderValidationParam, VideoDecoderInfo)
                                    Catch ex1 As Exception
                                        Me._manager.SendCmd(VideoCheckCommand, False, VideoDecoderValidationParam, VideoDecoderInfo)
                                    End Try
                                    'Sample buffer value - Buffer 2842:262144
                                    Dim bufferValueStr = Split(VideoDecoderInfo(1), VideoDecoderValidationParam(1))(1)
                                    BufferValue(i) = Convert.ToInt32(Trim(Split(bufferValueStr, ":")(1)))

                                    ' Compare buffer value with previous to check if its changing
                                    If i > 0 Then
                                        If BufferValue(i) <> BufferValue(i - 1) Then
                                            videoCheckPass = True

                                            timeToWait = _Timeout / numOfTrials
                                            If (timeToWait > 0) Then
                                                _iex.Wait(timeToWait)
                                            Else
                                                _iex.Wait(1)
                                            End If
                                        Else
                                            videoCheckPass = False
                                        End If
                                    End If
                                Next
                                ' If videoCheck is true but video is not present then throw an exception 
                                If _IsPresent Then
                                    If Not videoCheckPass Then
                                        ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.VideoNotPresent, "Video Decoder Buffer values are not different! Video is not present!"))
                                    End If
                                Else
                                    ' If videoCheck if false but video is present then throw an exception
                                    If videoCheckPass Then
                                        ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.VideoPresent, "Video Decoder Buffer values are different! Video is present!"))
                                    End If
                                End If

                            Catch ex2 As Exception
                                If Not _IsPresent Then
                                    ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.VideoPresent, "Video Decoder Buffer values are different! Video is present!"))
                                ElseIf Not videoCheckPass Then
                                    ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.VideoNotPresent, "Video Decoder Buffer values are not different! Video is not present! "))
                                End If

                            Finally
                                Me._manager.TelnetDisconnect(False)
                            End Try

                        Else
                            ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.TelnetFailure, "Could not Telnet to the box!"))
                        End If

                    Else
                        ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.INIFailure, "Video Decoder path is not present in environment.ini! "))

                    End If

                Else
                    EPG.Utils.LogCommentInfo("Implementation to check for PIP is not yet available")
                End If

            End Try

            EPG.Utils.LogCommentInfo("Check for Video Passed")


        End Sub
    End Class


End Namespace
