Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Surfs Channel Up Down On Live,Guide,ChannelBar And Channel Lineup And Tune
    ''' </summary>
    Public Class ChannelSurf
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _SurfIn As EnumSurfIn
        Private _ChannelNumber As String
        Private _IsNext As Boolean
        Private _NumberOfPresses As Integer
        Private _IsPredicted As EnumPredicted
        Private _DoTune As Boolean
        Private _IsDCA As Boolean
        Private _GuideTimeLine As String
        Private _IsPastEvent As Boolean

        ''' <param name="SurfIn">Can Be : Live, Guide Or ChannelBar</param>
        ''' <param name="ChannelNumber">Optional Parameter Default = "" : Channel Number - Not Supported For Current Channel</param>
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
        Sub New(ByVal SurfIn As EnumSurfIn, ByVal ChannelNumber As String, ByVal IsNext As Boolean, ByVal NumberOfPresses As Integer, ByVal IsPredicted As EnumPredicted, ByVal DoTune As Boolean, ByVal IsDCA As Boolean, ByVal GuideTimeline As String, ByVal IsPastEvent As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._SurfIn = SurfIn
            Me._ChannelNumber = ChannelNumber
            Me._IsNext = IsNext
            Me._NumberOfPresses = NumberOfPresses
            Me._IsPredicted = IsPredicted
            Me._DoTune = DoTune
            Me._IsDCA = IsDCA
            Me._GuideTimeLine = GuideTimeline
            Me._IsPastEvent = IsPastEvent
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim type As String = ""
            Dim totalNumberOfServices As Integer = 0
            Dim currentChNum As String = ""
            Dim currentCh As New Service
            Dim targetCh As New Service
            Dim istargetChRadio As Boolean = False
            Dim channelType As String = ""

            If _NumberOfPresses = 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "NumberOfPresses Can't Be 0"))
            End If

            Select Case _SurfIn
                Case EnumSurfIn.ChannelBar, EnumSurfIn.ChannelLineup
                    _IsDCA = False
            End Select

            Select Case _IsPredicted
                Case EnumPredicted.PredictedWithoutPIP, EnumPredicted.NotPredictedWithoutPIP
                    _IsPredicted = EnumPredicted.WithoutPIP
            End Select
            If Me._ChannelNumber <> "" Then
                'Get Target Channel from content xml and check whether the Type is Radio
                targetCh = Me._manager.GetServiceFromContentXML("LCN=" + _ChannelNumber)
                'Check for Target Channel Empty will not check for Channel Type
                If Not IsNothing(targetCh) Then
                    If targetCh.Type.ToLower = "radio" Then
                        istargetChRadio = True
                    End If
                End If
            End If

            If Me._ChannelNumber <> "" And _NumberOfPresses = -1 And Not _IsDCA Then

                totalNumberOfServices = CInt(Me._manager.GetAttributeFromContentXML("Services", "Total"))

                Try
                    EPG.Banner.GetChannelNumber(currentChNum)
                Catch
                    EPG.Banner.Navigate()
                    EPG.Banner.GetChannelNumber(currentChNum)
                    EPG.Utils.ReturnToLiveViewing()
                End Try

                currentCh = Me._manager.GetServiceFromContentXML("LCN=" + currentChNum)

                If IsNothing(currentCh) Or IsNothing(targetCh) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Failed to fetch services from stream configuration."))
                End If

                If currentCh.LCN = targetCh.LCN Then
                    EPG.Utils.LogCommentWarning("The service you are trying to tune to is same as the service you are currently in!")
                    Return
                End If

                'Get the minimum scrolls required between current service and target service
                _NumberOfPresses = EPG.Utils.GetMinScrollsBetweenServices(CInt(currentCh.PositionOnList), CInt(targetCh.PositionOnList), totalNumberOfServices, _IsNext)

            End If

            Select Case Me._SurfIn

                Case EnumSurfIn.Live

                    Select Case _IsPredicted
                        Case EnumPredicted.Predicted
                            type = "Predicted"
                        Case EnumPredicted.NotPredicted
                            type = "Not Predicted"
                        Case EnumPredicted.Ignore
                            type = "Ignore"
                        Case EnumPredicted.Default
                            type = "Default"
                        Case Else
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Invalid EnumPredicted passed"))
                    End Select


                    EPG.Utils.ReturnToLiveViewing()

                    If Me._ChannelNumber <> "" And _IsDCA Then
                        'Check if whether it is on same channel
                        Try
                            EPG.Live.VerifyChannelNumber(Me._ChannelNumber)
                            _iex.ForceHideFailure()
                            Exit Sub
                        Catch ex As IEXException
                            If ex.ExitCodeValue <> 112 Then
                                _iex.ForceHideFailure()
                                ExceptionUtils.ThrowEx(New IEXException(ex.ExitCodeValue, ex.ExitCodeName, "Failed To Check Channel Number In EpgInfo : " + ex.Message))
                            End If
                        Catch ex As EAException
                        End Try

                        'Tune to channel
                        EPG.Live.TuningToChannel(_ChannelNumber, type)
                        EPG.Live.VerifyTuneToChannel(type)
                        EPG.Live.VerifyChannelNumber(_ChannelNumber)
                    Else
                        For i = 1 To Me._NumberOfPresses
                            If _IsNext Then
                                EPG.Live.SurfChannelUp(type)
                            Else
                                EPG.Live.SurfChannelDown(type)
                            End If

                            EPG.Live.VerifyTuneToChannel(type)
                        Next
                    End If
                    'Tuning to target Radio Channel

                    Try
                        EPG.Utils.GetEpgInfo("chtype", channelType) 'Fetching the Chtype TV/Radio on Channel Tune and checking the Target State reached.
                    Catch ex As Exception
                        EPG.Utils.LogCommentWarning("Channel Type returned Empty therefore Assuming as TV")
                        channelType = "TV"
                    End Try
                    If istargetChRadio Or channelType.ToLower = "radio" Then
                        EPG.Live.VerifyRadioStateReached()
                    Else
                        EPG.Utils.VerifyLiveReached()
                    End If

                Case EnumSurfIn.ChannelLineup

                    EPG.Utils.ReturnToLiveViewing()

                    If Not EPG.ChannelLineup.IsChannelLineUp Then
                        EPG.ChannelLineup.Navigate()
                    End If

                    For i As Integer = 1 To Me._NumberOfPresses
                        If Me._IsNext Then
                            EPG.ChannelLineup.SurfChannelUp()
                        Else
                            EPG.ChannelLineup.SurfChannelDown()
                        End If
                    Next

                    If Me._DoTune Then
                        EPG.ChannelLineup.SelectChannel()
                    End If

                Case EnumSurfIn.Guide

                    If Not EPG.Guide.IsGuide Then
                        EPG.Guide.Navigate()
                    End If

                    If Me._ChannelNumber <> "" And _IsDCA Then
                        Select Case _IsPredicted
                            Case EnumPredicted.Ignore
                                EPG.Guide.NavigateToChannel(Me._ChannelNumber, False)
                            Case Else
                                EPG.Guide.NavigateToChannel(Me._ChannelNumber)
                                EPG.Guide.VerifySelectedEventChannel(_ChannelNumber)
                        End Select
                        If Me._IsPastEvent Then
                            If Not EPG.Guide.VerifyIsPastEvent() Then
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Focused event is not the past event after channel surf"))
                            End If
                        End If
                    Else
                        Select Case _IsPredicted
                            Case EnumPredicted.Predicted
                                type = "Predicted"
                            Case EnumPredicted.WithPIP
                                type = "WithPIP"
                            Case EnumPredicted.WithoutPIP
                                type = "WithoutPIP"
                            Case EnumPredicted.Default, EnumPredicted.NotPredicted
                                type = "Default"
                            Case EnumPredicted.Ignore
                                type = "Ignore"
                            Case Else
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Invalid EnumPredicted passed"))
                        End Select

                        For i As Integer = 1 To Me._NumberOfPresses
                            If Me._IsNext Then
                                EPG.Guide.SurfChannelDown(type)
                            Else
                                EPG.Guide.SurfChannelUp(type)
                            End If
                            If Me._IsPastEvent Then
                                If Not EPG.Guide.VerifyIsPastEvent() Then
                                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Focused event is not the past event after channel surf"))
                                End If
                            End If
                        Next
                    End If

                    If Me._DoTune Then
                        EPG.Guide.SelectCurrentEvent(type)
                    End If

                Case EnumSurfIn.GuideSingleChannel

                    If Not EPG.Guide.IsGuideSingleChannel Then
                        EPG.Guide.NavigateToGuideSingleChannel()
                    End If

                    If Me._ChannelNumber <> "" And _IsDCA Then
                        EPG.Guide.NavigateToChannel(Me._ChannelNumber) 'works for ONE CHANNEL as well
                        EPG.Guide.VerifySelectedEventChannel(_ChannelNumber)
                    Else
                        Select Case _IsPredicted
                            Case EnumPredicted.Predicted
                                type = "Predicted"
                            Case EnumPredicted.WithPIP
                                type = "WithPIP"
                            Case EnumPredicted.WithoutPIP
                                type = "WithoutPIP"
                            Case EnumPredicted.Default, EnumPredicted.NotPredicted
                                type = "Default"
                            Case EnumPredicted.Ignore
                                type = "Ignore"
                            Case Else
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Invalid EnumPredicted passed"))
                        End Select

                        For i As Integer = 1 To Me._NumberOfPresses
                            If Me._IsNext Then
                                EPG.Guide.SurfChannelRight(type)
                            Else 'Surf Previous Channel For SingleChannel (BB sending SELECT_LEFT instead of SELECT_UP)
                                EPG.Guide.SurfChannelLeft(type)
                            End If
                        Next
                    End If

                    If Me._DoTune Then 'works for ONE CHANNEL as well
                        EPG.Guide.SelectCurrentEvent(type)
                    End If

                Case EnumSurfIn.ChannelBar

                    EPG.Utils.ReturnToLiveViewing()

                    EPG.ChannelBar.Navigate()

                    Select Case _IsPredicted
                        Case EnumPredicted.WithPIP
                            type = "WithPIP"
                        Case EnumPredicted.WithoutPIP
                            type = "WithoutPIP"
                        Case EnumPredicted.Default, EnumPredicted.Predicted
                            type = "Default"
                        Case EnumPredicted.Ignore
                            type = "Ignore"
                        Case Else
                            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Invalid EnumPredicted passed"))
                    End Select

                    For i As Integer = 1 To Me._NumberOfPresses
                        If Me._IsNext Then
                            EPG.ChannelBar.SurfChannelUp(type)
                            EPG.ChannelBar.VerifySurfChannel(type)
                        Else
                            EPG.ChannelBar.SurfChannelDown(type)
                            EPG.ChannelBar.VerifySurfChannel(type)
                        End If
                    Next

                    If Me._DoTune Then
                        EPG.ChannelBar.DoTune()
                    End If

                Case EnumSurfIn.GuideAdjustTimeline

                    EPG.Guide.NavigateToGuideAdjustTimeline(_GuideTimeLine)

                    If Me._ChannelNumber <> "" And _IsDCA Then
                        Select Case _IsPredicted
                            Case EnumPredicted.Ignore
                                EPG.Guide.NavigateToChannel(Me._ChannelNumber, False)
                            Case Else
                                EPG.Guide.NavigateToChannel(Me._ChannelNumber)
                                EPG.Guide.VerifySelectedEventChannel(_ChannelNumber)
                        End Select
                    Else
                        Select Case _IsPredicted
                            Case EnumPredicted.Predicted
                                type = "Predicted"
                            Case EnumPredicted.WithPIP
                                type = "WithPIP"
                            Case EnumPredicted.WithoutPIP
                                type = "WithoutPIP"
                            Case EnumPredicted.Default, EnumPredicted.NotPredicted
                                type = "Default"
                            Case EnumPredicted.Ignore
                                type = "Ignore"
                            Case Else
                                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Invalid EnumPredicted passed"))
                        End Select

                        For i As Integer = 1 To Me._NumberOfPresses
                            If Me._IsNext Then
                                EPG.Guide.SurfChannelDown(type)
                            Else
                                EPG.Guide.SurfChannelUp(type)
                            End If
                        Next
                    End If

                    If Me._DoTune Then
                        EPG.Guide.SelectCurrentEvent(type)
                    End If

                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "SurfIn Param Not Valid"))
            End Select

        End Sub

    End Class

End Namespace