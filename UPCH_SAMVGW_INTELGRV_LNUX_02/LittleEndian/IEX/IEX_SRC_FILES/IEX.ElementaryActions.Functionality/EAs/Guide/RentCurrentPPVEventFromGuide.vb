Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Renting Current PPV Event From Guide
    ''' </summary>
    Public Class RentCurrentPPVEventFromGuide
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelNumber As Integer

        ''' <param name="ChannelNumber">Channel Of The PPV Event To Be Rented</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para>
        ''' <para>328 - INIFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para>
        ''' <para>347 - SelectEventFailure</para>
        ''' <para>369 - RentPPVEventFailure</para>
        ''' </remarks>

        Sub New(ByVal ChannelNumber As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelNumber = ChannelNumber
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

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

            EPG.Guide.SelectEvent()

            EPG.Banner.RentPpvEvent(True)

        End Sub

    End Class

End Namespace
