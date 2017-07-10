Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Setting Trickmode Speed And Checkes BOF or EOF
    ''' </summary>
    Public Class SetTrickModeSpeed
        Inherits IEX.ElementaryActions.BaseCommand

        Private _Speed As Double
        Private _EventKeyName As String
        Private _Verify_EOF_BOF As Boolean
        Private _IsReviewBufferFull As Boolean
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="EventKeyName">Key Of The Event - "RB" For Review Buffer Or Event Key Name For Regular Playback</param>
        ''' <param name="Speed">For Example : 1 For Play, 0 For Pause,0.5,2,6,12,30</param>
        ''' <param name="Verify_EOF_BOF">If True Verifies EOF Or BOF According To Direction</param>
        ''' <param name="IsReviewBufferFull">Optional Parameter Default = False : If True Review Buffer Full</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>303 - FasVerificationFailure</para> 
        ''' <para>304 - IRVerificationFailure</para>   
        ''' <para>305 - PCATFailure</para> 
        ''' <para>307 - GetStreamInfoFailure</para> 
        ''' <para>313 - SetTrickModeSpeedFailure</para> 
        ''' <para>320 - VerifyEofBofFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Speed As Double, ByVal Verify_EOF_BOF As Boolean, ByVal IsReviewBufferFull As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _Speed = Speed
            _EventKeyName = EventKeyName
            _IsReviewBufferFull = IsReviewBufferFull
            _Verify_EOF_BOF = Verify_EOF_BOF
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim EventDuration As Integer = 0
            Dim IsReviewBuffer As Boolean
            Dim DurationFromBeginning As Integer = 0
            Dim EventName As String = ""
            Dim Duration As String = ""
            Dim Eof As Boolean
            Dim RaiseTMBar As Boolean

            Try
                RaiseTMBar = Convert.ToBoolean(EPG.Utils.GetValueFromProject("TRICKMODES", "RAISE_TM_BAR_BEFORE_SETTING_SPEED"))
            Catch
                EPG.Utils.LogCommentWarning("RAISE_TM_BAR_BEFORE_SETTING_SPEED value not present in Project.ini, taking default value instead!")
                RaiseTMBar = False
            End Try
            If _Speed <> 0 And _Speed <> 0.5 And _Speed <> 1 And _Speed <> 2 And _Speed <> 4 And _Speed <> 6 And _Speed <> 8 And _Speed <> 12 And _Speed <> 30 And _Speed <> 32 And _
              _Speed <> -2 And _Speed <> -4 And _Speed <> -6 And _Speed <> -8 And _Speed <> -12 And _Speed <> -30 And _Speed <> -32 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Speed " + _Speed.ToString + " Not Supproted"))
            End If

            If _EventKeyName.ToLower = "rb" Then
                IsReviewBuffer = True
            End If

            If (_Speed = 1 Or _Speed = 0.5) And (IsReviewBuffer And _Verify_EOF_BOF = True) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Can't Verify EOF In Speed 1 Or 0.5"))
            End If

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            ElseIf Me._EventKeyName.ToLower <> "rb" Then
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            EPG.Utils.LogCommentInfo("Setting TrickMode Speed To : " + Me._Speed.ToString)

            If _Speed <> 0 AndAlso _Speed <> 1 Then
                EPG.TrickModes.RaiseTrickMode()
            ElseIf Not _manager.Project.IsEPGLikeCogeco Or RaiseTMBar Then
                EPG.TrickModes.RaiseTrickMode()
            End If

            EPG.TrickModes.SetSpeed(_Speed)

            If _Verify_EOF_BOF Then
                If _Speed = 0 Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Can't Verify EOF BOF With Speed 0"))
                End If

                If _Speed.ToString.Contains("-") Then
                    Eof = False
                Else
                    Eof = True
                End If

                Dim res As IEXGateway.IEXResult

                If IsReviewBuffer Then
                    EPG.TrickModes.GetCurrentPlaybackDuration(EventDuration)

                ElseIf EPG.Events(_EventKeyName).Source.Contains("Manual") Then
                    'Bug In PCAT StartTime So Getting Duration From PCAT Failes Taking Given Duration From User
                    EPG.Utils.LogCommentWarning("WORKAROUND : BUG IN PCAT STARTTIME SO GETTING DURATION FROM PCAT FAILES TAKING GIVEN DURATION FROM USER")
                    EventDuration = EPG.Events(_EventKeyName).Duration
                ElseIf EPG.Events(_EventKeyName).Source = "VOD" Then
                    EventDuration = EPG.Events(_EventKeyName).Duration
                Else
                    res = Me._manager.PCAT.GetEventDuration(_EventKeyName, EnumPCATtables.FromRecordings, Duration)
                    If Not res.CommandSucceeded Then
                        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                    End If

                    EventDuration = EPG.Events(_EventKeyName).Duration
                End If

                EPG.TrickModes.GetStreamPosition(DurationFromBeginning)

                If Eof Then
                    EventDuration = EventDuration - DurationFromBeginning
                Else 'BOF
                    EventDuration = DurationFromBeginning
                End If

                EPG.TrickModes.VerifyEofBof(EventDuration, _Speed, IsReviewBuffer, _IsReviewBufferFull, Eof)

            End If
        End Sub
    End Class

End Namespace