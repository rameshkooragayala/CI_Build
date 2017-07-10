Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Verify Event On Archive Screen
    ''' </summary>
    Public Class VerifyEventInArchive
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _EventKeyName As String
        Private _Navigate As Boolean
        Private _SupposedToFindEvent As Boolean
        Private _StartTime As String
        Private _EndTime As String
        Private _Date As String

        ''' <param name="EventKeyName">Key Of The Event</param>
        ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Archive</param>
        ''' <param name="SupposedToFindEvent">Optional Parameter Default = True : If True Tries To Find Event On Archive Else Tries Not To Find It</param>
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
        ''' </remarks>
        Sub New(ByVal EventKeyName As String, ByVal Navigate As Boolean, ByVal SupposedToFindEvent As Boolean, ByVal StartTime As String, ByVal EndTime As String, ByVal EvDate As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            Me._EventKeyName = EventKeyName
            Me._SupposedToFindEvent = SupposedToFindEvent
            Me._StartTime = StartTime
            Me._EndTime = EndTime
            Me._Date = EvDate
            EPG = Me._manager.UI
            Me._Navigate = Navigate
        End Sub

        Protected Overrides Sub Execute()
            Dim EventName As String = ""
            Dim IsSeries As Boolean = False
            Dim IsEmpty As Boolean = False
            Dim EventTime As String = ""

            If Me._EventKeyName = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"))
            Else
                Try
                    EventName = EPG.Events(_EventKeyName).Name
                    IsSeries = EPG.Events(_EventKeyName).IsSeries
                Catch ex As Exception
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + Me._EventKeyName + " Does Not Exists On Collection"))
                End Try
            End If

            If _StartTime <> "" AndAlso _StartTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Start Time Should Be Entered With Format : HH:MM"))
            End If

            If _EndTime <> "" AndAlso _EndTime.Contains(":") = False Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "End Time Should Be Entered With Format : HH:MM"))
            End If

            If Me._Navigate Then
                EPG.ArchiveRecordings.Navigate()
            End If

            IsEmpty = EPG.ArchiveRecordings.IsEmpty()

            If Me._SupposedToFindEvent And IsEmpty = False Then
                EPG.ArchiveRecordings.FindEvent(EventName, _Date, _StartTime, _EndTime)
            ElseIf Me._SupposedToFindEvent = False And IsEmpty Then
                Exit Sub
            ElseIf Me._SupposedToFindEvent And IsEmpty Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Archive Is Empty !!!"))
            ElseIf Me._SupposedToFindEvent = False And IsEmpty = False Then
                Try
                    EPG.Utils.StartHideFailures("Verifing Event : " + EventName + " Is Not On Archive")
                    EPG.ArchiveRecordings.FindEvent(EventName, _Date, _StartTime, _EndTime)
                Catch ex As EAException
                _iex.ForceHideFailure()
                    Exit Sub ' Event Was Not Found As Expected
                End Try
                _iex.ForceHideFailure()

                If _StartTime <> "" OrElse _EndTime <> "" OrElse _Date <> "" Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Not Find Event : Event " + EventName + " With Start Time - " + _StartTime + " End Time - " + _EndTime + " Date - " + Me._Date + " Is On Archive"))
                Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Not Find Event : Event " + EventName + " Is On Archive"))
                End If
            End If

            'Verify series events
            If IsSeries Then
                EPG.PlannerBase.VerifySeriesEvent()
                EPG.PlannerBase.ExpandSeries()
            End If

        End Sub

    End Class

End Namespace