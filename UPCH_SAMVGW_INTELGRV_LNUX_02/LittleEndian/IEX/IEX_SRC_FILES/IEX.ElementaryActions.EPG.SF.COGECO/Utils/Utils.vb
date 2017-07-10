Imports FailuresHandler

Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.Utils

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
    End Sub

    ''' <summary>
    '''   Sends IR Key
    ''' </summary>
    ''' <param name="IRKey">The Key To Send</param>
    ''' <param name="WaitAfterIR">Optional Parameter Default 2000 : Wait
    '''  After Sending</param>
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

                BeginWaitForDebugMessages("IEX IR key", 6)

                res = _iex.IR.SendIR(IRKey)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New IEXException(res))
                End If

                Try
                    If EndWaitForDebugMessages("IEX IR key", ActualLines) Then
                        _iex.Wait(WaitAfterIR / 1000)
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
    '''   Return To Live Viewing
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter FALSE.If TRUE Checks For Video</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' </remarks>
    Public Overrides Sub ReturnToLiveViewing(Optional ByVal CheckForVideo As Boolean = False)
        Dim res As IEXGateway._IEXResult
        Dim EpgText As String = ""
        Dim State As String = ""
        Dim Msg As String = ""

        StartHideFailures("Checking If Already In Live")

        Try
            If VerifyState("LIVE", 2) Then
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

            _UI.Menu.Navigate()

            res = _iex.MilestonesEPG.SelectMenuItem("CHANNELS")
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New IEXException(res))
            End If

            Dim ReturnedValue As String = ""

            _iex.IR.SendIR("SELECT", "", 2000)

            Dim LockedChannel As String = ""
            Dim LockedProgram As String = ""
            Dim Unsubscribed As String = ""


            LockedChannel = GetValueFromDictionary("DIC_LOCKED_CHANNEL")
            LockedProgram = GetValueFromDictionary("DIC_LOCKED_PROGRAM")
            Unsubscribed = GetValueFromDictionary("DIC_UNSUBSCRIBED_CHANNEL")


            Dim EventName As String = ""

            Try
                GetEpgInfo("evtName", EventName)
            Catch ex As IEXException
                _UI.ChannelBar.Navigate()
                GetEpgInfo("evtName", EventName)
            End Try


            If EventName <> LockedChannel And EventName <> LockedProgram And EventName <> Unsubscribed Then
                If Not VerifyState("LIVE", 25) Then
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

                _UI.Live.WaitAfterLiveReached()
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

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
        Dim State As String = ""
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
            _iex.Wait(2)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Retrieving EPG Date Format Default Value
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Overrides Function GetEpgDateFormatDefaultValue() As String

        Return "ddd MMM d"

    End Function
End Class


