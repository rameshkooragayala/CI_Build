Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Stop Recording From Action Bar
    ''' </summary>
    Public Class StopRecordingFromBanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>348 - StopRecordEventFailure</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._EventKeyName = EventKeyName
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            Dim EventName As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            EPG.Banner.Navigate()

            EPG.Utils.StartHideFailures("Trying To Find Event : " + EventName.ToString)

            Try
                EPG.Banner.VerifyEventName(EventName)
            Catch ex As Exception
                EPG.Utils.LogCommentInfo("Didn't Find Event On Current Trying To Find On Next Event")

                EPG.Utils.ReturnToLiveViewing()
                EPG.ChannelBar.NextEvent(True)
            End Try

            _iex.ForceHideFailure()

            EPG.Banner.VerifyEventName(EventName)
            EPG.Banner.StopRecording()
        End Sub

    End Class

End Namespace