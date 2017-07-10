Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Renting Future PPV Event From Guide
    ''' </summary>
    Public Class RentFuturePPVEventFromGuide
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumber As Integer
        Private _NumberOfPresses As Integer

        ''' <param name="ChannelNumber">Channel Of The Event To Be Rented</param>
        ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Right Presses From Current Event</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para>
        ''' <para>302 - EmptyEpgInfoFailure</para>
        ''' <para>304 - IRVerificationFailure</para>
        ''' <para>328 - INIFailure</para> 
        ''' <para>347 - SelectEventFailure</para>
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' <para>369 - RentPPVEventFailure</para>
        ''' </remarks>

        Sub New(ByVal ChannelNumber As String, ByVal NumberOfPresses As Integer, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNumber = ChannelNumber
            Me._NumberOfPresses = NumberOfPresses
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            If _NumberOfPresses < 1 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Number Of Presses Must Be 1 Or More"))
            End If

            Try
                If CInt(_ChannelNumber) <= 0 Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Number Must Be 1 Or More"))
                End If
            Catch ex As Exception
                If _ChannelNumber <> "" Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Channel Number Can Contain Only Digits"))
                End If
            End Try

            If Not EPG.Guide.IsGuide Then
                EPG.Guide.Navigate()
            End If

            EPG.Guide.NavigateToChannel(Me._ChannelNumber)

            EPG.Guide.NextEvent(Me._NumberOfPresses)

            EPG.Guide.SelectEvent()

            EPG.Banner.RentPpvEvent(False)

        End Sub

    End Class

End Namespace
