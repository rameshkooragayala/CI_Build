Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Surfs Channel Up Down On Live,Guide,ChannelBar And Channel Lineup And Tune
    ''' </summary>
    Public Class ChannelSurf
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _SurfIn As EnumSurfIn
        Private _ChannelNumber As String
        Private _IsNext As Boolean
        Private _NumberOfPresses As Integer
        Private _IsPredicted As EnumPredicted
        Private _DoTune As Boolean
        Private _IsDCA As Boolean

        ''' <param name="SurfIn">Can Be : Live, Guide Or ChannelBar</param>
        ''' <param name="ChannelNumber">Optional Parameter Default = "" : Channel Number</param>
        ''' <param name="IsNext">Optional Parameter Default = True : If True Surfs To Next Channel Else To Previous</param>
        ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Channels To Surf Up OR Down</param>
        ''' <param name="IsPredicted">Optional Parameter Default = Ignore : If The Next Or Previous Channel Is Predicted</param>
        ''' <param name="DoTune">Optional Parameter Default = False : If True Tune To Surfed Channel</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal SurfIn As EnumSurfIn, ByVal ChannelNumber As String, ByVal IsNext As Boolean, ByVal NumberOfPresses As Integer, ByVal IsPredicted As EnumPredicted, ByVal DoTune As Boolean, ByVal IsDCA As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._SurfIn = SurfIn
            Me._ChannelNumber = ChannelNumber
            Me._IsNext = IsNext
            Me._NumberOfPresses = NumberOfPresses
            Me._IsPredicted = IsPredicted
            Me._DoTune = DoTune
            Me._IsDCA = IsDCA
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            If _NumberOfPresses = 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "NumberOfPresses Can't Be 0"))
            End If

            Select Case Me._SurfIn

                Case EnumSurfIn.Live

                    EPG.Utils.ReturnToLiveViewing()

                    If Me._ChannelNumber <> "" Then

                        EPG.Utils.StartHideFailures("Checking If Already Tunnned To " + Me._ChannelNumber)
                        Try
                            EPG.ChannelBar.Navigate()
                            EPG.ChannelBar.VerifyChannelNumber(Me._ChannelNumber)
                            EPG.Utils.ReturnToLiveViewing()
                            _iex.ForceHideFailure()
                            Exit Sub
                        Catch ex As IEXException
                            If ex.ExitCodeValue <> 112 Then
                                _iex.ForceHideFailure()
                                ExceptionUtils.ThrowEx(New IEXException(ex.ExitCodeValue, ex.ExitCodeName, "Failed To Check Channel Number In EpgInfo : " + ex.Message))
                            End If
                        Catch ex As EAException
                        End Try

                        _iex.ForceHideFailure()

                        EPG.Live.TuningToChannel(_ChannelNumber)
                    Else
                       ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Can't Surf On Live In MOBILE"))
                    End If

                    EPG.Utils.VerifyLiveReached()

                Case EnumSurfIn.ChannelLineup

                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SurfingFailure, "Can't Surf On Live In MOBILE"))

                Case EnumSurfIn.Guide

                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Surf In Guide Is Not Implemented"))
                    'If Not EPG.Guide.IsGuide Then
                    '    EPG.Guide.Navigate()
                    'End If

                    'If Me._ChannelNumber <> "" Then
                    '    EPG.Guide.NavigateToChannel(Me._ChannelNumber)
                    'Else
                    '    For i As Integer = 1 To Me._NumberOfPresses
                    '        If Me._IsNext Then
                    '            Select Case _IsPredicted
                    '                Case EnumPredicted.NotPredictedWithoutPIP, EnumPredicted.PredictedWithoutPIP
                    '                    EPG.Guide.SurfChannelDown("WithoutPIP")
                    '                Case Else
                    '                    EPG.Guide.SurfChannelDown()
                    '            End Select
                    '        Else
                    '            Select Case _IsPredicted
                    '                Case EnumPredicted.NotPredictedWithoutPIP, EnumPredicted.PredictedWithoutPIP
                    '                    EPG.Guide.SurfChannelUp("WithoutPIP")
                    '                Case Else
                    '                    EPG.Guide.SurfChannelUp()
                    '            End Select
                    '        End If
                    '    Next
                    'End If

                    'If Me._DoTune Then
                    '    Select Case _IsPredicted
                    '        Case EnumPredicted.NotPredictedWithoutPIP, EnumPredicted.PredictedWithoutPIP
                    '            EPG.Guide.SelectCurrentEvent("WithoutPIP")
                    '        Case Else
                    '            EPG.Guide.SelectCurrentEvent()
                    '    End Select

                    'End If

                Case EnumSurfIn.ChannelBar

                    EPG.Utils.ReturnToLiveViewing()

                    EPG.ChannelBar.Navigate()

                    For i As Integer = 1 To Me._NumberOfPresses
                        If Me._IsNext Then
                            EPG.ChannelBar.SurfChannelUp() 
                        Else
                            EPG.ChannelBar.SurfChannelDown()
                        End If
                    Next

                    If Me._DoTune Then
                        EPG.ChannelBar.DoTune()
                    End If

                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "SurfIn Param Not Valid"))
            End Select
        End Sub

    End Class

End Namespace