Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Deletes Event From Archive
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DeleteRecordFromArchive
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Navigate As Boolean
        Private _InReviewBuffer As Boolean
        Private _VerifyDeletedInPCAT As Boolean

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="InReviewBuffer">Optional Parameter Default = True : True If The Event Is In The Persistent Review Buffer</param>
        ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Archive</param>
        ''' <param name="VerifyDeletedInPCAT">Optional Parameter Default = True : If True Verifies Event Deleted In PCAT</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>305 - PCATFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>330 - TelnetFailure</para> 
        ''' <para>331 - CopyFileFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>345 - DeleteEventFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal InReviewBuffer As Boolean, ByVal Navigate As Boolean, ByVal VerifyDeletedInPCAT As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            Me._InReviewBuffer = InReviewBuffer
            Me._VerifyDeletedInPCAT = VerifyDeletedInPCAT
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

            If _Navigate Then
                res = Me._manager.PVR.VerifyEventInArchive(Me._EventKeyName)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            End If

            EPG.ArchiveRecordings.SelectEvent(EventName)

            EPG.ArchiveRecordings.DeleteEvent(Me._InReviewBuffer)

            If Me._VerifyDeletedInPCAT Then
                res = Me._manager.PCAT.VerifyEventDeleted(Me._EventKeyName, 2)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            End If

        End Sub

    End Class

End Namespace