Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Stops Recording Event From Archive
    ''' </summary>
    Public Class StopRecordingFromArchive
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Navigate As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Archive</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>348 - StopRecordEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Navigate As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            EPG = Me._manager.UI
            Me._Navigate = Navigate
        End Sub

        Protected Overrides Sub Execute()
            Dim res As IEXGateway.IEXResult
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

            res = Me._manager.PVR.VerifyEventInArchive(Me._EventKeyName, Me._Navigate)
            If Not res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
            End If

            EPG.ArchiveRecordings.StopRecordingEvent()

        End Sub

    End Class

End Namespace