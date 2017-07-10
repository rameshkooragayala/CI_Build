Imports FailuresHandler

Public Class PowerManagement
    Inherits EPG.PowerManagement

    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _Utils = pUI.Utils
    End Sub

    'Method to check projects support Maintainance 
    Public Overrides Function IsPwrMgmtMaintainanceSupported() As Boolean

        Dim maintananceSupport As String = ""
        Dim returnValue As Boolean = False
        maintananceSupport = _Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_SUPPORT")
        If String.IsNullOrEmpty(maintananceSupport) Then
            _Utils.LogCommentWarning("Unable to fetch the maintenance Support value from project ini")
        ElseIf (maintananceSupport.ToUpper().Equals("FALSE")) Then
            returnValue = False
        Else
            returnValue = True
        End If

        Return returnValue
    End Function

    'Method to verify MP Milestones if the project support maintenance

    Public Overrides Sub VerifyPmMilesStone(jobPresent As Object, startTime As Object, endTime As Object, currEPGTime As Object)
        'default maintenance duration fetched from project ini
        Dim maintenanceCompletionDuration As String = _Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DURATION")
        Dim defaultmaintenanceCompletionDuration As Int32
        If String.IsNullOrEmpty(maintenanceCompletionDuration) Then
            _Utils.LogCommentFail("Unable to fetch the maintenance Duration value from project ini")

        Else
            defaultmaintenanceCompletionDuration = Int32.Parse(maintenanceCompletionDuration)
            defaultmaintenanceCompletionDuration = defaultmaintenanceCompletionDuration * 60 'in secs
            _Utils.LogCommentInfo("defaultmaintenanceCompletionDuration is " & defaultmaintenanceCompletionDuration)
        End If

        Dim mpStartMilestones As String = ""
        Dim mpCompleteMilestones As String = ""
        Dim list As New ArrayList

        _Utils.LogCommentInfo("Inside the method VerifyMPMilesStones")

        ' calling method calculateWaitForMP to calculate wait for maintenance
        Dim maintenanceDelay As Integer = CalculateWaitForMP(jobPresent, startTime, endTime, currEPGTime)

        'get values from milestones ini
        mpStartMilestones = _Utils.GetValueFromMilestones("MaintenanceStart")
        mpCompleteMilestones = _Utils.GetValueFromMilestones("MaintenanceComplete")

        'verify for UTM start milestones.
        _Utils.BeginWaitForDebugMessages(mpStartMilestones, maintenanceDelay)
        If Not _Utils.EndWaitForDebugMessages(mpStartMilestones, list) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to recieve Maintenence start milestones"))
        End If

        'Verify for UTM completed milestones.
        _Utils.BeginWaitForDebugMessages(mpCompleteMilestones, defaultmaintenanceCompletionDuration)

        If Not _Utils.EndWaitForDebugMessages(mpCompleteMilestones, list) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to recieve Maintenence completed milestones"))
        End If
    End Sub

    'Method to calculate wait time before MP start
    Private Function CalculateWaitForMP(ByVal jobPresent As Boolean, ByVal StartTime As String, ByVal EndTime As String, ByVal CurrEPGTime As String) As Integer

        _Utils.LogCommentInfo("Inside the method CalculateWaitForMP")

        'Fetch Default maintenance duration from project ini.
        Dim maintenancedelay As String = _Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_DEALY")
        Dim defaultMaintenancedelay As Int32
        If String.IsNullOrEmpty(maintenancedelay) Then
            _Utils.LogCommentFail("Unable to fetch the Maintenancedelay value from project ini")

        Else
            defaultMaintenancedelay = Int32.Parse(maintenancedelay)
            defaultMaintenancedelay = defaultMaintenancedelay * 60 'in secs
            _Utils.LogCommentInfo("Time after which maintenance should start" & defaultMaintenancedelay)
        End If


        'Max time to check for scheduled records
        Dim maintenanceCheckTime As String = _Utils.GetValueFromProject("POWER_MANAGEMENT", "MAINTENANCE_CHECK_TIME")
        Dim defaultMaintenanceCheckTime As Int32
        If (String.IsNullOrEmpty(maintenanceCheckTime)) Then
            _Utils.LogCommentFail("Failed to get maintenanceCheckTime from project ini ")
        Else
            defaultMaintenanceCheckTime = Int32.Parse(maintenanceCheckTime)
            defaultMaintenanceCheckTime = defaultMaintenanceCheckTime * 60 * 60 'in secs
            _Utils.LogCommentInfo("Max time to check for scheduled records" & defaultMaintenanceCheckTime)
        End If



        'No recording present return default maintenance delay
        If Not jobPresent Then
            Return defaultMaintenancedelay
        Else
            'Recordings are ongoing/scheduled
            Dim startTimeAfterParse As DateTime = DateTime.Parse(StartTime)

            Dim endTimeAfterParse As DateTime
            Dim strArr() As String
            strArr = EndTime.Split(":")
            If strArr(0) = "00" Then
                endTimeAfterParse = (DateTime.Parse(EndTime)).AddDays("1")
            Else
                endTimeAfterParse = (DateTime.Parse(EndTime))
            End If

            Dim currEPGTimeAfterParse As DateTime = DateTime.Parse(CurrEPGTime)
            Dim recordDuration As Int32

            'Current recording
            Dim waitTime As Int32
            If StartTime <= CurrEPGTime Then

                recordDuration = (endTimeAfterParse - currEPGTimeAfterParse).TotalSeconds()
                waitTime = Convert.ToInt32(recordDuration) + (defaultMaintenancedelay)
                Return waitTime
            Else
                'Near future recording(If recording scheduled within maintenence check time)
                recordDuration = (endTimeAfterParse - currEPGTimeAfterParse).TotalSeconds()
                If (Convert.ToInt32(recordDuration)) <= defaultMaintenanceCheckTime Then
                    waitTime = Convert.ToInt32(recordDuration) + defaultMaintenancedelay
                    Return waitTime
                Else
                    'Far future reocrding(if recording is scheduled after maintenence check time)
                    Return defaultMaintenancedelay

                End If
            End If
        End If


    End Function


#Region "Perle RPC"

    Public Overrides Function IsPerleRpc() As Boolean
        _Utils.StartHideFailures("Checking If RPC Is Perle")
        Try

            If CBool(_Utils.GetValueFromEnvironment("PerleRpc")) Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        Finally
            _iex.ForceHideFailure()
        End Try
    End Function

    Public Overrides Sub PerleRpcRestart()
        _Utils.StartHideFailures("Restarting RPC")
        Try
            SendCommandToPerleRpc(powerUp:=False)
            _iex.Wait(2)
            SendCommandToPerleRpc(powerUp:=True)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Overrides Sub SendCommandToPerleRpc(powerUp As Boolean)
        Dim Res As IEXGateway._IEXResult
        Dim expectedResponseShouldInclude As String

        _Utils.StartHideFailures("Sending " + IIf(powerUp, " ON", " OFF") + " Command To RPC")
        Try
            Dim outletNumber As String = _Utils.GetValueFromEnvironment("PowerOutletNumber")
            Dim RpcIp As String = _Utils.GetValueFromEnvironment("PowerDeviceIP")
            expectedResponseShouldInclude = " " & outletNumber.ToString().PadRight(4, " "c) & "\|\D*\| \(undefined\) \|"

            If powerUp Then
                expectedResponseShouldInclude = expectedResponseShouldInclude & "   ON   \|\s+"
            Else
                expectedResponseShouldInclude = expectedResponseShouldInclude & "   OFF  \|\s+"
            End If



            Try
                For i As Integer = 1 To 10
                    ConnectToPerleRpc()
                    _Utils.LogCommentInfo("Attempting to Connect to RPC " & "Attempt " & i)
                    _iex.Wait(3)
                    _iex.Debug.WriteLine("/J " & outletNumber.ToString(), IEXGateway.DebugDevice.Telnet2)
                    _iex.Debug.WriteLine("/J " & outletNumber.ToString(), IEXGateway.DebugDevice.Telnet2)
                    If powerUp Then
                        _iex.Debug.WriteLine("/ON " & outletNumber.ToString(), IEXGateway.DebugDevice.Telnet2)
                    Else
                        _iex.Debug.WriteLine("/OFF " & outletNumber.ToString(), IEXGateway.DebugDevice.Telnet2)
                    End If

                    _iex.Wait(4)

                    Dim actual As String = ""
                    Dim marginLines As String = ""

                    _iex.Debug.BeginWaitForMessage(expectedResponseShouldInclude, 0, 8, IEXGateway.DebugDevice.Telnet2)

                    _iex.Debug.WriteLine("/S", IEXGateway.DebugDevice.Telnet2)

                    Res = _iex.Debug.EndWaitForMessage(expectedResponseShouldInclude, actual, marginLines, IEXGateway.DebugDevice.Telnet2)

                    If Not Res.CommandSucceeded Then
                        If i = 10 Then
                            ExceptionUtils.ThrowEx(New IEXException(Res))
                        End If
                    Else
                        Exit For
                    End If
                    _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet2)
                    _iex.Wait(60 + (Rnd() * 60))
                Next

            Finally
                _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet2)
            End Try
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Private Sub ConnectToPerleRpc()
        Dim Res As IEXGateway._IEXResult
        Dim RpcIp As String = ""
        Dim failed As Boolean = False

        _Utils.StartHideFailures("Connecting To RPC")
        Try
            RpcIp = _Utils.GetValueFromEnvironment("PowerDeviceIP")

            For i As Integer = 1 To 5

                Res = _iex.Debug.ConnectTo(RpcIp, IEXGateway.DebugDevice.Telnet2)
                If Not Res.CommandSucceeded Then
                    _Utils.LogCommentInfo("Attempting to Connect to RPC again after 30 - 90 seconds")
                    _iex.Wait(30 + (Rnd() * 60))
                Else
                    Exit For
                End If
                If i = 5 Then
                    failed = True
                End If
            Next

            If failed Then
                ExceptionUtils.ThrowEx(New IEXException(Res))
            End If


        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


#End Region

End Class
