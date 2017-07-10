Imports System.Runtime.InteropServices
Imports IEX.ElementaryActions.EPG

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class PCAT
    Protected _iex As IEXGateway.IEX
    Protected _Manager As IEX.ElementaryActions.Functionality.Manager

    Sub New(ByVal pIEX As IEXGateway.IEX, ByVal Manager As IEX.ElementaryActions.Manager)
        _iex = pIEX
        _Manager = Manager
    End Sub


    ''' <summary>
    '''   Finds Event In PCAT.DB And Returnes The Index In The Excel
    ''' </summary>
    ''' <param name="PcatEvId">byRef The Returned PCAT ID Of The Event</param>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Table">In Which Table To Look In</param>
    ''' <param name="CopyPCAT">If True Copies The PCAT From STB</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Friend Function FindEvent(ByRef PcatEvId As String, ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByVal CopyPCAT As Boolean) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = _Manager.Invoke("FindEvent", EventKeyName, Table, CopyPCAT, _Manager)
        If _Manager.ReturnValues Is Nothing Then
            PcatEvId = ""
        Else
            PcatEvId = DirectCast(_Manager.ReturnValues(0), String)
        End If

        Return res

    End Function

    ''' <summary>
    '''   Get Event Duration From PCAT 
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Table">The Table To Look In</param>
    ''' <param name="Duration">ByRef The Returned Duration</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function GetEventDuration(ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByRef Duration As String) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = _Manager.Invoke("GetEventDuration", EventKeyName, Table, _Manager)

        If _Manager.ReturnValues Is Nothing Then
            Duration = ""
        Else
            Duration = DirectCast(_Manager.ReturnValues(0), String)
        End If

        Return res
    End Function

    ''' <summary>
    '''   Internal Function For Verifying Parameter Status In PCAT
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Table">Recordings Or Bookings</param>
    ''' <param name="Field">Field Of Status In PCAT Modifier</param>
    ''' <param name="ExpectedStatus">Expected String In Column</param>
    ''' <param name="CopyPCAT">If True Copies The PCAT From STB</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function VerifyEventStatus(ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByVal Field As String, ByVal ExpectedStatus As String, ByVal CopyPCAT As Boolean) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventStatus", EventKeyName, Table, Field, ExpectedStatus, CopyPCAT, _Manager)
    End Function

    ''' <summary>
    '''   Verify Event Partial Recording Staus
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Expected">ALL,PARTIAL Or NONE</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function VerifyEventPartialStatus(ByVal EventKeyName As String, ByVal Expected As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventPartialStatus", EventKeyName, Expected, _Manager)
    End Function

    ''' <summary>
    '''   Verify Event Exception Reason
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ExceptionReason">
    ''' <para>VIEWER_STOPPED</para>
    ''' <para>VIEWER_RESTARTED_AFTER_RECORDING_STOPPED</para>
    ''' <para>VIEWER_BOOKED_TOO_LATE</para>
    ''' <para>VIEWER_STOPPED_TUNER_IMMEDIATE_CONFLICT</para>
    ''' <para>VIEWER_STOPPED_TUNER_BOOKING_CONFLICT</para>
    ''' <para>VIEWER_STOPPED_DISK_CONFLICT</para>
    ''' <para>VIEWER_BOOKING_DELETED</para>
    ''' <para>POWER_FAILURE</para>
    ''' <para>INSUFFICIENT_DISK_SPACE</para>
    ''' <para>NO_EVENT_IN_SCHEDULE</para>
    ''' <para>NO_CARD_DETECTED</para>
    ''' <para>NO_TUNER_AVAILABLE</para>
    ''' <para>NO_CA_AUTHORISATION</para>
    ''' <para>RECORDED_CONTENT_MISSING</para>
    ''' <para>SIGNAL_LOSS</para>
    ''' <para>PMT_NOT_AVAILABLE</para>
    ''' <para>ES_NOT_AVAILABLE</para>
    ''' <para>RASP_OVERFLOW</para>
    ''' <para>MAX_FILE_SIZE_EXCEEDED</para>
    ''' <para>RECORDING_SUCCEEDED</para>
    ''' <para>BOOKING_EXPIRED</para>
    ''' <para>HEALTH_CHECK</para>
    ''' <para>SWDOWNLOAD</para>
    ''' <para>CONNECTION_LOST</para>
    ''' <para>HTTP Error</para>
    ''' <para>RESTARTED_AFTER_RECORDING_STOPPED</para>
    ''' <para>PASSIVE_STANDBY</para>
    ''' <para>NO_EXCEPTION</para>
    ''' <para>Unknown exception (100)</para>
    ''' </param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function VerifyEventExceptionReason(ByVal EventKeyName As String, ByVal ExceptionReason As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventExceptionReason", EventKeyName, ExceptionReason, _Manager)
    End Function

    ''' <summary>
    '''   Verify Event Is Deleted From PCAT
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Table">In Which Table To Look In</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function VerifyEventDeleted(ByVal EventKeyName As String, ByVal Table As EnumPCATtables) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventDeleted", EventKeyName, Table, _Manager)
    End Function

    ''' <summary>
    '''   Verify Event Is Recording In PCAT
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function VerifyEventIsRecording(ByVal EventKeyName As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventIsRecording", EventKeyName, _Manager)
    End Function

    ''' <summary>
    '''   Verify Event Is Booked From PCAT
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function VerifyEventBooked(ByVal EventKeyName As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventBooked", EventKeyName, _Manager)
    End Function

    ''' <summary>
    '''  Verify Event Keep Status In PCAT
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Keep">If True Search Status TRUE Else Search FALSE</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function VerifyKeep(ByVal EventKeyName As String, ByVal Keep As Boolean) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyKeep", EventKeyName, Keep, _Manager)
    End Function

    ''' <summary>
    '''   Gets Event Information From PCAT By Field Name In The PCAT Table
    ''' </summary>
    ''' <param name="EventKeyName">The Key Of The Event</param>
    ''' <param name="Table">In Which Table To Look In</param>
    ''' <param name="FieldName">The Field Name In The Table</param>
    ''' <param name="ReturnedValue">ByRef The Returned Value</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetEventInfo(ByVal EventKeyName As String, ByVal Table As EnumPCATtables, ByVal FieldName As String, ByRef ReturnedValue As String)
        Dim res As IEXGateway.IEXResult

        res = _Manager.Invoke("GetEventInfo", EventKeyName, Table, FieldName, _Manager)

        If _Manager.ReturnValues Is Nothing Then
            ReturnedValue = ""
        Else
            ReturnedValue = DirectCast(_Manager.ReturnValues(0), String)
        End If

        Return res
    End Function

    ''' <summary>
    '''   Copies PCAT.DB From STB By FTP
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' </remarks>
    Public Function CopyPCAT() As IEXGateway.IEXResult
        Return _Manager.Invoke("CopyPCAT", _Manager)
    End Function

End Class
