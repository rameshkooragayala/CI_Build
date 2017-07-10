Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''    Canceling Booking From Guide
    ''' </summary>
    Public Class CancelBookingFromGuide
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
		Private _IsSeries As Boolean
        Private _VerifyCanceledInPCAT As Boolean
        Private _ReturnToLive As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="VerifyCanceledInPCAT">Optional Parameter Default = True : If True Verifies Event Canceled In PCAT</param>
        ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para>  
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>342 - CancelEventFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' <para>355 - TuneToChannelFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal VerifyCanceledInPCAT As Boolean, ByVal ReturnToLive As Boolean, ByVal IsSeries As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._VerifyCanceledInPCAT = VerifyCanceledInPCAT
            Me._ReturnToLive = ReturnToLive
            Me._manager = pManager
			Me._IsSeries = IsSeries
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim EventName As String = ""
            Dim EventTime As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exist On Collection"))
                End Try
            End If

            Try
                EventTime = EPG.Utils.FormatEventTime(EPG.Events(_EventKeyName).StartTime.ToString, EPG.Events(_EventKeyName).EndTime.ToString)
                EPG.Utils.LogCommentInfo("Formatted Event Time: " & EventTime)
                Dim ExpectedEventTimeLength As Byte = EPG.Utils.GetExpectedEventTimeLength()
                EPG.Utils.LogCommentInfo("Expected Event Time Length: " & ExpectedEventTimeLength)
                If EventTime.Length <> ExpectedEventTimeLength Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Get Event Full Time (Format Mismatch - Event Time Length Expected=" & ExpectedEventTimeLength & " Actual=" & EventTime.Length & ")"))
                End If
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.GetEventInfoFailure, "Failed To Get Event Full Time : " + ex.Message))
            End Try

            If Not EPG.Guide.IsGuide Then
                EPG.Guide.Navigate()
            End If

            EPG.Guide.NavigateToChannel(EPG.Events(_EventKeyName).Channel)

            EPG.Guide.FindEventByTime(EventTime)

            EPG.Guide.SelectEvent()

            EPG.Banner.VerifyEventName(EventName)

            EPG.Banner.CancelBooking()

            If Me._VerifyCanceledInPCAT Then
                res = Me._manager.PCAT.VerifyEventDeleted(Me._EventKeyName, 2)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            End If

            If Me._ReturnToLive Then
                EPG.Utils.ReturnToLiveViewing()
            End If
        End Sub

    End Class

End Namespace
