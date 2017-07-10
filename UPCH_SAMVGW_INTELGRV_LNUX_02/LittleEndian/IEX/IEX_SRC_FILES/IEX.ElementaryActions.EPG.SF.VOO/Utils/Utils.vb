Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.Utils

    Dim _UI As IEX.ElementaryActions.EPG.SF.VOO.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub

    ''' <summary>
    '''    Retrieving The Event Time Separator To Be Inserted Between Start Time And End Time  
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetEventTimeSeparator() As String
        Return "-"
    End Function
    ''' <summary>
    '''   Entering PIN
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub EnterPin(ByVal PIN As String)
        Dim EpgText As String = ""
        Dim _PIN As String = ""

        If PIN = "" Then
            _PIN = GetValueFromEnvironment("DefaultPIN")

            If _PIN.Length <> 4 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "PIN Length Was Less Than 4 Digits Please Check DefaultPIN In Your Environment.ini"))
            End If
        Else
            _PIN = PIN
        End If

        StartHideFailures("Entering PIN : " + _PIN)

        Try

            TypeKeys(_PIN)

            '' ''Commented for LS240 and FS1501

            'EpgText = GetValueFromDictionary("DIC_CONFIRM")

            '_UI.Menu.SetConfirmationMenu(EpgText)

            '_UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try
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
    '''   Return To Live Viewing
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter FALSE.If TRUE Checks For Video</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Overrides Sub ReturnToLiveViewing(Optional ByVal CheckForVideo As Boolean = False)
        Dim res As IEXGateway._IEXResult
        Dim EpgText As String = ""
        Dim State As String = ""
        Dim Msg As String = ""

        StartHideFailures("Checking If Already In Live")

        Try
            If VerifyState("LIVE", 5) Then
                Msg = "Already On Live"
                Exit Sub
            End If

            Msg = "NOT Already On Live"

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                LogCommentInfo(Msg)
            End If
        End Try

        StartHideFailures("Returning To Live Viewing")
        Try

            Dim ChannelType As String = ""
            Try
                GetEPGInfo("chtype", ChannelType)
            Catch ex As Exception
                LogCommentWarning("Channel Type returned Empty therefore Assuming as TV")
                ChannelType = "TV"
            End Try


            If ChannelType.ToUpper <> "RADIO" Then
                _UI.Menu.Navigate()

                _UI.Utils.SendIR("RETOUR")

                If Not VerifyState("LIVE", 30) Then
                    _UI.Utils.SendIR("EXIT")
                End If
                If Not VerifyState("LIVE", 30) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ReturnToLiveFailure, "Failed To Verify State Is LIVE"))
                End If

                If CheckForVideo Then
                    StartHideFailures("Checking Video Is On The Screen")
                    Try
                        res = _iex.CheckForVideo(VideoCords, True, 15)
                        If Not res.CommandSucceeded Then
                            res = _iex.CheckForVideo(VideoCords(IsFullArea:=False, Area:=2), True, 15)
                            If Not res.CommandSucceeded Then
                                res = _iex.CheckForVideo(VideoCords(IsFullArea:=False, Area:=3), True, 15)
                                If Not res.CommandSucceeded And res.CommandExecutionFailed Then
                                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VideoNotPresent, "Failed To Check Video Exists"))
                                End If
                            End If
                        End If
                    Finally
                        _iex.ForceHideFailure()
                    End Try
                End If
            ElseIf ChannelType.ToUpper.Contains("RADIO") Then
                _UI.Live.VerifyRadioStateReached()
            Else
                If CheckForVideo Then
                    LogCommentWarning("WARNING : Video Was Not Checked Since The Content Is Locked")
                End If
            End If

            _UI.Live.WaitAfterLiveReached()

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Sends IR Key
    ''' </summary>
    ''' <param name="IRKey">The Key To Send</param>
    ''' <param name="WaitAfterIR">Optional Parameter Default 2000 : Wait After Sending</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SendIR(ByVal IRKey As String, Optional ByVal WaitAfterIR As Integer = 2000)
        Dim res As New IEXGateway.IEXResult
        Dim ActualLines As New ArrayList

        StartHideFailures("Sending IR : " + IRKey + " And Waiting After : " + WaitAfterIR.ToString)

        Try
            WaitAfterIR = WaitAfterIR * WaitAfterIRFactor

            For i As Integer = 0 To 2

                BeginWaitForDebugMessages("IEX IR key", 5)

                res = _iex.IR.SendIR(IRKey)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If

                Try
                    If EndWaitForDebugMessages("IEX IR key", ActualLines) Then
                        If WaitAfterIR > 0 Then
                            _iex.Wait(WaitAfterIR / 1000)
                        End If
                        Exit Sub
                    End If

                Catch ex As EAException
                    If i = 2 Then 'If Reached Last Iteration
                        ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, "Failed To Verify Send IR " + IRKey.ToString + " : " + ex.ShortMessage))
                    End If
                Catch ex As IEXException
                    If i = 2 Then 'If Reached Last Iteration
                        ExceptionUtils.ThrowEx(New EAException(ex.ExitCode, ex.Message))
                    End If
                End Try
            Next

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Verify Send IR " + IRKey.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Translate Languages
    ''' </summary>
    ''' <param name="Language">Language To Translate</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Overrides Function TranslateLanguage(ByVal language As String, Optional ByVal fromEnglish As Boolean = True) As String
        Select Case language
            Case "Dutch"
                Return "Nederlands"
            Case "English"
                Return "english"
            Case "French"
                Return "français"
            Case "German"
                Return "DEUTSCH"
            Case "Nederlands"
                Return "nederlands"
            Case "Off"
                Return "OFF"
            Case Else
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Language " + language.ToString + " Is Not Valid"))
        End Select

        Return ""
    End Function
End Class


