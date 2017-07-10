Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Cancel Future Event Booking
    ''' </summary>
    Public Class CancelBookingFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _VerifyCancelInPCAT As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="VerifyCancelInPCAT">Optional Parameter Default = True : If True Verifies Event Cancelled In PCAT</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>322 - VerificationFailure</para> 	  
        ''' <para>328 - INIFailure</para> 	  
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>342 - CancelEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal VerifyCancelInPCAT As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._VerifyCancelInPCAT = VerifyCancelInPCAT
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim EventStartTime As String = ""
            Dim EventEndTime As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                    EventStartTime = EPG.Events(_EventKeyName).StartTime
                    EventEndTime = EPG.Events(_EventKeyName).EndTime
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            EPG.Banner.Navigate()

            Dim CurrentEventName As String = ""
            Dim BannerStartTime As String = ""
            Dim BannerEndTime As String = ""

            EPG.Banner.GetEventName(CurrentEventName)

            EPG.Banner.GetEventStartTime(BannerStartTime)

            EPG.Banner.GetEventEndTime(BannerEndTime)

            If EventName <> CurrentEventName OrElse BannerStartTime <> EventStartTime OrElse BannerEndTime <> EventEndTime Then

                EPG.Utils.ReturnToLiveViewing()

                EPG.ChannelBar.Navigate()

                EPG.ChannelBar.NextEvent(True)

                EPG.Banner.VerifyEventName(EventName)
            End If

            EPG.Banner.CancelBooking()

            'If Me._VerifyCancelInPCAT Then
            '    res = Me._manager.PCAT.VerifyEventDeleted(Me._EventKeyName, 2)
            '    If Not res.CommandSucceeded Then
            '        ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            '    End If
            'End If

        End Sub

    End Class

End Namespace
