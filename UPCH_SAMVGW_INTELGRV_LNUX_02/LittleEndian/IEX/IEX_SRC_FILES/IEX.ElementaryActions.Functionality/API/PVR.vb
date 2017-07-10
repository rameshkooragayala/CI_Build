Imports System.Runtime.InteropServices
Imports IEX.ElementaryActions.EPG

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class PVR
    Protected _iex As IEXGateway.IEX
    Protected _Manager As IEX.ElementaryActions.Functionality.Manager

    Sub New(ByVal pIEX As IEXGateway.IEX, ByVal Manager As IEX.ElementaryActions.Manager)
        _iex = pIEX
        _Manager = Manager
    End Sub

#Region "RECORDINGS"

    ''' <summary>
    '''    Record Manual Recording From Current Event
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Number</param>
    ''' <param name="DurationInMin">Optional Parameter Default = -1 : Duration In Minutes</param>
    ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <param name="NoEIT">Optional Parameter Default = False : If True Navigation to Manual Recording Is Different</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>309 - GetEpgTimeFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para>    
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function RecordManualFromCurrent(ByVal EventKeyName As String, ByVal ChannelNumber As String, Optional ByVal DurationInMin As Integer = -1, Optional ByVal Frequency As EnumFrequency = EnumFrequency.ONE_TIME, _
                                            Optional ByVal VerifyBookingInPCAT As Boolean = True, Optional ByVal IsConflict As Boolean = False, Optional ByVal NoEIT As Boolean = False) As IEXGateway._IEXResult

        Return _Manager.Invoke("RecordManual", EventKeyName, "", Integer.Parse(ChannelNumber), -1, -1, "", DurationInMin, Frequency, VerifyBookingInPCAT, IsConflict, True, NoEIT, _Manager)
    End Function

    ''' <summary>
    '''   Record Manual Recording From Planner
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelName">Channel Name</param>
    ''' <param name="DaysDelay">Optional Parameter Default = -1 : Adds Days From Current Time</param>
    ''' <param name="MinutesDelayUntilBegining">Optional Parameter Default = -1 : Minutes Delay Until Beginning</param>
    ''' <param name="DurationInMin">Optional Parameter Default = 1 : Duration Of Recording</param>
    ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para>    
    ''' <para>310 - GetEpgDateFailure</para>    
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para>  
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para>  
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para>  
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function RecordManualFromPlanner(ByVal EventKeyName As String, ByVal ChannelName As String, Optional ByVal DaysDelay As Integer = 1, Optional ByVal MinutesDelayUntilBegining As Integer = -1, _
                                            Optional ByVal DurationInMin As Integer = -1, Optional ByVal Frequency As EnumFrequency = EnumFrequency.ONE_TIME, Optional ByVal VerifyBookingInPCAT As Boolean = True, _
                                            Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("RecordManual", EventKeyName, ChannelName, -1, DaysDelay, MinutesDelayUntilBegining, "", DurationInMin, Frequency, VerifyBookingInPCAT, IsConflict, False, False, _Manager)
    End Function

    ''' <summary>
    '''   Record Manual Recording From Planner
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Number</param>
    ''' <param name="DaysDelay">Optional Parameter Default = -1 : Adds Days From Current Time</param>
    ''' <param name="MinutesDelayUntilBegining">Optional Parameter Default = -1 : Minutes Delay Until Beginning</param>
    ''' <param name="DurationInMin">Optional Parameter Default = 1 : Duration Of Recording</param>
    ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para>    
    ''' <para>310 - GetEpgDateFailure</para>    
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para>  
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para>  
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para>  
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function RecordManualFromPlanner(ByVal EventKeyName As String, ByVal ChannelNumber As Integer, Optional ByVal DaysDelay As Integer = 1, Optional ByVal MinutesDelayUntilBegining As Integer = -1, _
                                           Optional ByVal DurationInMin As Integer = -1, Optional ByVal Frequency As EnumFrequency = EnumFrequency.ONE_TIME, Optional ByVal VerifyBookingInPCAT As Boolean = True, _
                                           Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("RecordManual", EventKeyName, "", ChannelNumber, DaysDelay, MinutesDelayUntilBegining, "", DurationInMin, Frequency, VerifyBookingInPCAT, IsConflict, False, False, _Manager)
    End Function

    ''' <summary>
    '''   Record Manual Recording From Planner
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelName">Channel Name</param>
    ''' <param name="StartTime">Start Time For The Manual Recording</param>
    ''' <param name="DurationInMin">Duration Of Recording</param>
    ''' <param name="DaysDelay">Optional Parameter Default = -1 : Adds Days From Current Time</param>
    ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para>    
    ''' <para>310 - GetEpgDateFailure</para>    
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para>  
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para>  
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para>  
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function RecordManualFromPlanner(ByVal EventKeyName As String, ByVal ChannelName As String, ByVal StartTime As String, ByVal DurationInMin As Integer, _
                                            Optional ByVal DaysDelay As Integer = 1, Optional ByVal Frequency As EnumFrequency = EnumFrequency.ONE_TIME, _
                                            Optional ByVal VerifyBookingInPCAT As Boolean = True, Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("RecordManual", EventKeyName, ChannelName, -1, DaysDelay, -1, StartTime, DurationInMin, Frequency, VerifyBookingInPCAT, IsConflict, False, False, _Manager)
    End Function

    ''' <summary>
    '''   Record Manual Recording From Planner
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Number</param>
    ''' <param name="StartTime">Start Time For The Manual Recording</param>
    ''' <param name="DurationInMin">Duration Of Recording</param>
    ''' <param name="DaysDelay">Optional Parameter Default = -1 : Adds Days From Current Time</param>
    ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para>    
    ''' <para>310 - GetEpgDateFailure</para>    
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para>  
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para>  
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para>  
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function RecordManualFromPlanner(ByVal EventKeyName As String, ByVal ChannelNumber As Integer, ByVal StartTime As String, ByVal DurationInMin As Integer, _
                                            Optional ByVal DaysDelay As Integer = 1, Optional ByVal Frequency As EnumFrequency = EnumFrequency.ONE_TIME, _
                                            Optional ByVal VerifyBookingInPCAT As Boolean = True, Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("RecordManual", EventKeyName, "", ChannelNumber, DaysDelay, -1, StartTime, DurationInMin, Frequency, VerifyBookingInPCAT, IsConflict, False, False, _Manager)
    End Function

    ''' <summary>
    '''   Recording Current Event From Action Bar
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="MinTimeBeforeEvEnd">Optional Parameter Default = -1 : Minutes Required Until End Of Event</param>
    ''' <param name="IsResuming">Optional Parameter Default = False : If True Checks Resume Recording Milestones</param>
    ''' <param name="VerifyIsRecordingInPCAT">Optional Parameter Default = True : If True Verify Is Recording In PCAT</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>337 - ParseEventTimeFailure</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function RecordCurrentEventFromBanner(ByVal EventKeyName As String, Optional ByVal MinTimeBeforeEvEnd As Integer = -1, Optional ByVal IsResuming As Boolean = False, _
                                                 Optional ByVal VerifyIsRecordingInPCAT As Boolean = True, Optional ByVal IsConflict As Boolean = False, Optional ByVal IsPastEvent As Boolean = False, Optional ByVal IsSeries As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("RecordCurrentEventFromBanner", EventKeyName, MinTimeBeforeEvEnd, IsResuming, VerifyIsRecordingInPCAT, IsConflict, IsPastEvent, IsSeries, _Manager)
    End Function

    ''' <summary>
    '''    Booking Future Event From Action Bar 
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="NumOfPresses">Optional Parameter Default = -1 : Number Of Left Presses From Current</param>
    ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 1 : Minimum Time Right For The Event To Start ( EXAMPLE : For Guard Time )</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verify Is Booked In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Return To Live</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>305 - PCATFailure</para>
    ''' <para>323 - VerifyStateFailure</para>
    ''' <para>328 - INIFailure</para>
    ''' <para>330 - TelnetFailure</para>
    ''' <para>331 - CopyFileFailure</para>
    ''' <para>332 - NoValidParameters</para>
    ''' <para>334 - VideoNotPresent</para>
    ''' <para>337 - ParseEventTimeFailure</para>
    ''' <para>338 - EventNotExistsFailure</para>
    ''' <para>339 - RecordEventFailure</para>
    ''' <para>344 - ConflictFailure</para>
    ''' <para>347 - SelectEventFailure</para>
    ''' <para>349 - ReturnToLiveFailure</para>
    ''' <para>350 - ParsingFailure </para>
    ''' </remarks>
    Public Function BookFutureEventFromBanner(ByVal EventKeyName As String, Optional ByVal NumOfPresses As Integer = 1, _
                                              Optional ByVal MinTimeBeforeEvStart As Integer = 1, Optional ByVal VerifyBookingInPCAT As Boolean = True, _
                                              Optional ByVal ReturnToLive As Boolean = True, Optional ByVal IsConflict As Boolean = False, Optional ByVal IsSeries As Boolean = False) As IEXGateway._IEXResult

        Return _Manager.Invoke("BookFutureEventFromBanner", EventKeyName, NumOfPresses, MinTimeBeforeEvStart, VerifyBookingInPCAT, ReturnToLive, IsConflict, IsSeries, _Manager)
    End Function

    ''' <summary>
    '''   Recording Current Event From Guide
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
    ''' <param name="MinTimeBeforeEvEnd">Optional Parameter Default = -1 : Minutes Required Until End Of Event</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 	
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function RecordCurrentEventFromGuide(ByVal EventKeyName As String, ByVal ChannelNumber As String, Optional ByVal MinTimeBeforeEvEnd As Integer = 1, Optional ByVal VerifyBookingInPCAT As Boolean = True, Optional ByVal ReturnToLive As Boolean = True, Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("RecordCurrentEventFromGuide", EventKeyName, ChannelNumber, MinTimeBeforeEvEnd, VerifyBookingInPCAT, ReturnToLive, IsConflict, _Manager)
    End Function

    ''' <summary>
    '''   Recording Using REC Key
    ''' </summary>
    ''' <param name="RecordIn">Can Be : Live, Guide, ChannelBar Or Action Bar</param>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
    ''' <param name="MinTimeBeforeEvEnd">Optional Parameter Default = -1 : Minutes Required Until End Of Event</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <param name="IsCurrent">Optional Parameter Default = False : If true then book future event</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 	
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function RecordUsingRECkey(ByVal RecordIn As EnumRecordIn, ByVal EventKeyName As String, ByVal ChannelNumber As String, Optional ByVal MinTimeBeforeEvEnd As Integer = 1, Optional ByVal VerifyBookingInPCAT As Boolean = True, Optional ByVal ReturnToLive As Boolean = True, Optional ByVal IsConflict As Boolean = False, Optional ByVal IsCurrent As Boolean = True, Optional ByVal IsSeries As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("RecordUsingRECkey", RecordIn, EventKeyName, ChannelNumber, MinTimeBeforeEvEnd, VerifyBookingInPCAT, ReturnToLive, IsConflict, IsCurrent, IsSeries, _Manager)
    End Function
    ''' <summary>
    '''  Stop Recording Using STOP Key
    ''' </summary>
    ''' <param name="RecordIn">Can Be : Live, Guide, ChannelBar Or Action Bar</param>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
    ''' <param name="IsCurrent">Optional Parameter Default = False : If true then book future event</param>
    ''' <param name="IsSeries">Optional Parameter default = False : If true then cancel the Entire Series</param>
    ''' <param name="IsStopRecord">Optional Parameter default = True : If False then select NO and recording will not stop </param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 	
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function StopRecordUsingStopKey(ByVal RecordIn As EnumRecordIn, ByVal EventKeyName As String, ByVal ChannelNumber As String, Optional ByVal IsCurrent As Boolean = True, Optional ByVal IsSeries As Boolean = False, Optional ByVal IsStopRecord As Boolean = True, Optional ByVal IsTBR As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("StopRecordUsingStopKey", RecordIn, EventKeyName, ChannelNumber, IsCurrent, IsSeries, IsStopRecord, IsTBR, _Manager)
    End Function

    ''' <summary>
    '''   Booking Future Event From Guide
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
    ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Right Presses From Current Event</param>
    ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 1 : Minimum Time Right For The Event To Start ( EXAMPLE : For Guard Time )</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <param name="IsConflict">Optional Parameter Default = True : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para>    
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para>   
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para>   
    ''' <para>355 - TuneToChannelFailure</para>  
    ''' </remarks>
    Public Function BookFutureEventFromGuide(ByVal EventKeyName As String, ByVal ChannelNumber As String, Optional ByVal NumberOfPresses As Integer = 1, _
                                             Optional ByVal MinTimeBeforeEvStart As Integer = 1, Optional ByVal VerifyBookingInPCAT As Boolean = True, _
                                             Optional ByVal ReturnToLive As Boolean = True, Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult

        Return _Manager.Invoke("BookFutureEventFromGuide", EventKeyName, ChannelNumber, "", "", 0, -1, 0, NumberOfPresses, MinTimeBeforeEvStart, VerifyBookingInPCAT, ReturnToLive, IsConflict, _Manager)

    End Function

    ''' <summary>
    '''   Booking Future Event From Guide
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
    ''' <param name="StartTime">Requested Exact Event Start Time</param>
    ''' <param name="EndTime">Requested Exact Event Start Time</param>
    ''' <param name="DaysDelay">Days Delay On Guide - Not Yet Supported</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <param name="IsConflict">Optional Parameter Default = True : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para>    
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para>   
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 
    ''' <para>346 - FindEventFailure</para>
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para>   
    ''' <para>355 - TuneToChannelFailure</para>  
    ''' </remarks>
    Public Function BookFutureEventFromGuide(ByVal EventKeyName As String, ByVal ChannelNumber As String, ByVal StartTime As String, ByVal EndTime As String, _
                                             Optional ByVal DaysDelay As Integer = 0, Optional ByVal VerifyBookingInPCAT As Boolean = True, Optional ByVal ReturnToLive As Boolean = True, _
                                             Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult

        Return _Manager.Invoke("BookFutureEventFromGuide", EventKeyName, ChannelNumber, StartTime, EndTime, 0, -1, DaysDelay, 1, 1, VerifyBookingInPCAT, ReturnToLive, IsConflict, _Manager)

    End Function


    ''' <summary>
    '''   Booking Future Event From Guide
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Recorded</param>
    ''' <param name="StartTime">Requested Exact Event Start Time</param>
    ''' <param name="MinEventLength">Minimum Event Length</param>
    ''' <param name="MaxEventLength">Max Event Length</param>
    ''' <param name="DaysDelay">Days Delay On Guide - Not Yet Supported</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <param name="IsConflict">Optional Parameter Default = True : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para>    
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para>   
    ''' <para>323 - VerifyStateFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 
    ''' <para>346 - FindEventFailure</para>
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para>   
    ''' <para>355 - TuneToChannelFailure</para>  
    ''' </remarks>
    Public Function BookFutureEventFromGuide(ByVal EventKeyName As String, ByVal ChannelNumber As String, ByVal StartTime As String, _
                                             ByVal MinEventLength As Integer, Optional ByVal MaxEventLength As Integer = -1, Optional ByVal DaysDelay As Integer = 0, _
                                             Optional ByVal VerifyBookingInPCAT As Boolean = True, Optional ByVal ReturnToLive As Boolean = True, _
                                             Optional ByVal IsConflict As Boolean = False) As IEXGateway._IEXResult

        Return _Manager.Invoke("BookFutureEventFromGuide", EventKeyName, ChannelNumber, StartTime, "", MinEventLength, MaxEventLength, DaysDelay, 1, 1, VerifyBookingInPCAT, ReturnToLive, IsConflict, _Manager)

    End Function

    ''' <summary>
    '''    Modify Manual Recording From Planner
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="StartTime">Channel Name</param>
    ''' <param name="EndTime">Channel Number If Entered Doing DCA</param>
    ''' <param name="ChannelName">Optional Parameter Default = "" : The Channel Name</param>
    ''' <param name="Days">Optional Parameter Default = 0 : Adds Or Substructing Days From Current Date</param>
    ''' <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para>    
    ''' <para>310 - GetEpgDateFailure</para>    
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para>  
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para>  
    ''' <para>339 - RecordEventFailure</para>  
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function ModifyManualRecording(ByVal EventKeyName As String, Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "", _
                                         Optional ByVal ChannelName As String = "", Optional ByVal Days As Integer = 0, Optional ByVal Frequency As EnumFrequency = EnumFrequency.NO_CHANGE, Optional ByVal IsFirstTime As Boolean = False) As IEXGateway._IEXResult

        Return _Manager.Invoke("ModifyManualRecording", EventKeyName, StartTime, EndTime, ChannelName, Days, Frequency, IsFirstTime, _Manager)
    End Function

#End Region

    ''' <summary>
    '''  Set Keep Or Un Keep On Planner Or Archive As Requested
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="SetKeep">If True Then Set To Keep If False Set To Un Keep</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>312 - SetEventKeepFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function SetKeepFlag(ByVal EventKeyName As String, ByVal SetKeep As Boolean, Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetKeepFlag", EventKeyName, SetKeep, ReturnToLive, _Manager)
    End Function

    ''' <summary>
    '''   Stop Recording From Action Bar
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function StopRecordingFromBanner(ByVal EventKeyName As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("StopRecordingFromBanner", EventKeyName, _Manager)
    End Function

    ''' <summary>
    '''   Cancel Future Event Booking
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="VerifyCancelInPCAT">Optional Parameter Default = True : If True Verifies Event Cancelled In PCAT</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function CancelBookingFromBanner(ByVal EventKeyName As String, Optional ByVal VerifyCancelInPCAT As Boolean = True, Optional ByVal IsSeries As Boolean = False, Optional ByVal IsComplete As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("CancelBookingFromBanner", EventKeyName, VerifyCancelInPCAT, IsSeries, IsComplete, _Manager)
    End Function

    ''' <summary>
    '''   Cancel Booking From Planner Screen
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="VerifyCancelInPCAT">Optional Parameter Default = True : If True Verifies Event Cancelled In PCAT</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function CancelBookingFromPlanner(ByVal EventKeyName As String, Optional ByVal VerifyCancelInPCAT As Boolean = True, Optional ByVal IsSeries As Boolean = False, Optional ByVal IsComplete As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("CancelBookingFromPlanner", EventKeyName, -1, VerifyCancelInPCAT, IsSeries, IsComplete, _Manager)
    End Function

    ''' <summary>
    '''   Cancel Ocurrence Booking From Planner Screen
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="OccurrenceNumber">The Occurrence To Cancel From 1 To 14</param>
    ''' <param name="VerifyCancelInPCAT">Optional Parameter Default = True : If True Verifies Event Cancelled In PCAT</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function CancelBookingFromPlanner(ByVal EventKeyName As String, ByVal OccurrenceNumber As Integer, Optional ByVal VerifyCancelInPCAT As Boolean = True, Optional ByVal IsSeries As Boolean = False, Optional ByVal IsComplete As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("CancelBookingFromPlanner", EventKeyName, OccurrenceNumber, VerifyCancelInPCAT, IsSeries, IsComplete, _Manager)
    End Function

    ''' <summary>
    '''   Delete All Bookings In Planner
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>342 - CancelEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function CancelAllBookingsFromPlanner() As IEXGateway._IEXResult
        Return _Manager.Invoke("CancelAllBookingsFromPlanner", _Manager)
    End Function

    ''' <summary>
    '''   Verify Event On Archive Screen
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Archive</param>
    ''' <param name="SupposedToFindEvent">Optional Parameter Default = True : If True Tries To Find Event On Archive Else Tries Not To Find It</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function VerifyEventInArchive(ByVal EventKeyName As String, Optional ByVal Navigate As Boolean = True, Optional ByVal SupposedToFindEvent As Boolean = True, _
                                         Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "", Optional ByVal EvDate As String = "") As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventInArchive", EventKeyName, Navigate, SupposedToFindEvent, StartTime, EndTime, EvDate, _Manager)
    End Function

    ''' <summary>
    '''    Verify Sorting On Archive Screen
    ''' </summary>
    ''' <param name="SortBy">Enum Can Be SortBy Time Or A-Z</param>
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
    Public Function VerifySortingInArchive(ByVal SortBy As EnumSortBy) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifySortingInArchive", SortBy, _Manager)
    End Function

    ''' <summary>
    '''    Verify Sorting On Planner Screen
    ''' </summary>
    ''' <param name="SortBy">Enum Can Be SortBy Time Or A-Z</param>
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
    Public Function VerifySortingInPlanner(ByVal SortBy As EnumSortBy) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifySortingInPlanner", SortBy, _Manager)
    End Function

    ''' <summary>
    '''   Verify Event On Planner Screen
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Planner</param>
    ''' <param name="SupposedToFindEvent">Optional Parameter Default = True : If True Tries To Find Event On Planner Else Tries Not To Find It</param>
    ''' <param name="StartTime">Optional Parameter Default = "" : If Entered Tries To Find Event On Planner By StartTime As Well</param>
    ''' <param name="EndTime">Optional Parameter Default = "" : If Entered Tries To Find Event On Planner By EndTime As Well</param>
    ''' <param name="EvDate">Optional Parameter Default = "" : If Entered Tries To Find Event On Planner By Date As Well</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function VerifyEventInPlanner(ByVal EventKeyName As String, Optional ByVal Navigate As Boolean = True, Optional ByVal SupposedToFindEvent As Boolean = True, _
                                         Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "", Optional ByVal EvDate As String = "") As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventInPlanner", EventKeyName, Navigate, SupposedToFindEvent, StartTime, EndTime, EvDate, _Manager)
    End Function

    ''' <summary>
    '''    Verifying Event Recurring Events On Planner By Name,Date,StartTime And EndTime
    ''' </summary>
    ''' <param name="EventKeyName">The Key Of The Event</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function VerifyRecurringBookingInPlanner(ByVal EventKeyName As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyRecurringBookingInPlanner", EventKeyName, _Manager)
    End Function

    ''' <summary>
    '''   Verify Event On Planner Screen
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="RecordError">Type of Error</param>
    ''' <param name="StartGuardTime">Optional Parameter Default = "" : If Entered Tries To Find Event On Planner By StartTime with Guard Time As Well</param>
    ''' <param name="EndGuardTime">Optional Parameter Default = "" : If Entered Tries To Find Event On Planner By EndTime with Guard Time As Well</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function VerifyRecordErrorInfo(ByVal EventKeyName As String, ByVal RecordError As EnumRecordErr, Optional ByVal StartGuardTime As String = "", Optional ByVal EndGuardTime As String = "", Optional ByVal VerifyInPCAT As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyRecordErrorInfo", EventKeyName, RecordError, StartGuardTime, EndGuardTime, VerifyInPCAT, _Manager)
    End Function
    ''' <summary>
    '''   Deletes Event From Archive
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="InReviewBuffer">Optional Parameter Default = True : True If The Event Is In The Persistent Review Buffer</param>
    ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Archive</param>
    ''' <param name="VerifyDeletedInPCAT">Optional Parameter Default = True : If True Verifies Event Deleted In PCAT</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function DeleteRecordFromArchive(ByVal EventKeyName As String, Optional ByVal InReviewBuffer As Boolean = False, Optional ByVal Navigate As Boolean = True, Optional ByVal VerifyDeletedInPCAT As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("DeleteRecordFromArchive", EventKeyName, InReviewBuffer, Navigate, VerifyDeletedInPCAT, _Manager)
    End Function

    ''' <summary>
    '''   Deletes All Events From Archive
    ''' </summary>
    ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Archive</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>345 - DeleteEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function DeleteAllRecordsFromArchive(Optional ByVal Navigate As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("DeleteAllRecordsFromArchive", Navigate, _Manager)
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="VerifyDeletedInPCAT">Optional Parameter Default = True : If True Verifies Event Deleted In PCAT</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    '''Possible Error Codes:
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
    ''' <para>347 - SelectEventFailure</para></remarks>
    Public Function DeleteFailedRecordedEvent(ByVal EventKeyName As String, Optional ByVal VerifyDeletedInPCAT As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("DeleteFailedRecordedEvent", EventKeyName, VerifyDeletedInPCAT, _Manager)
    End Function

    ''' <summary>
    '''   Lock Event From Archive
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Function LockEventFromArchive(ByVal EventKeyName As String, Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("LockEventFromArchive", EventKeyName, ReturnToLive, _Manager)
    End Function

    ''' <summary>
    '''   Lock Event From Planner
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Function LockEventFromPlanner(ByVal EventKeyName As String, Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("LockEventFromPlanner", EventKeyName, ReturnToLive, _Manager)
    End Function

    ''' <summary>
    '''   UnLock Event From Archive
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para>  
    ''' <para>349 - ReturnToLiveFailure</para> 	
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Function UnLockEventFromArchive(ByVal EventKeyName As String, Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("UnLockEventFromArchive", EventKeyName, ReturnToLive, _Manager)
    End Function

    ''' <summary>
    '''   UnLock Event From Planner
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>359 - LockUnlockFailure</para> 
    ''' </remarks>
    Public Function UnLockEventFromPlanner(ByVal EventKeyName As String, Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("UnLockEventFromPlanner", EventKeyName, ReturnToLive, _Manager)
    End Function

    ''' <summary>
    '''   Stops Recording Event From Archive
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="Navigate">Optional Parameter Default = True : If True Navigates To Archive</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function StopRecordingFromArchive(ByVal EventKeyName As String, Optional ByVal Navigate As Boolean = True) As IEXGateway._IEXResult
        Return _Manager.Invoke("StopRecordingFromArchive", EventKeyName, Navigate, _Manager)
    End Function

    ''' <summary>
    '''   Playback Event From Archive 
    '''   If SecToPlay=0 And VerifyEOF=False Then Plays The Event And Exit The EA
    '''   If SecToPlay=0 And VerifyEOF=True Then Plays The Event Until End And Verifies EOF Then Exit EA 
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="SecToPlay">Seconds To Play If 0 And VerifyEOF Is True Playes Until End Else If Verify EOF Is False Exit The EA.If SecToPlay Is Not Equal To 0 Then Plays The Event SecToPlay And StopPlayback</param>
    ''' <param name="FromBeginning">If Already Played To Play From The Beginning Or Last Viewed</param>
    ''' <param name="VerifyEOF">If True Verify Playback Until End</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>320 - VerifyEofBofFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>340 - PlayEventFailure</para> 
    ''' <para>343 - StopPlayEventFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overloads Function PlaybackRecFromArchive(ByVal EventKeyName As String, ByVal SecToPlay As Integer, ByVal FromBeginning As Boolean, ByVal VerifyEOF As Boolean, _
                                           Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "", Optional ByVal EvDate As String = "") As IEXGateway._IEXResult
        Return _Manager.Invoke("PlaybackRecFromArchive", EventKeyName, SecToPlay, FromBeginning, VerifyEOF, False, StartTime, EndTime, EvDate, _Manager)
    End Function

    ''' <summary>
    '''   Playback Event From Archive With Entering PIN
    '''   If SecToPlay=0 And VerifyEOF=False Then Plays The Event And Exit The EA
    '''   If SecToPlay=0 And VerifyEOF=True Then Plays The Event Until End And Verifies EOF Then Exit EA 
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="SecToPlay">Seconds To Play If 0 And VerifyEOF Is True Playes Until End Else If Verify EOF Is False Exit The EA.If SecToPlay Is Not Equal To 0 Then Plays The Event SecToPlay And StopPlayback</param>
    ''' <param name="FromBeginning">If Already Played To Play From The Beginning Or Last Viewed</param>
    ''' <param name="VerifyEOF">If True Verify Playback Until End</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>320 - VerifyEofBofFailure</para> 
    ''' <para>321 - VerifyChannelAttributeFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>340 - PlayEventFailure</para> 
    ''' <para>343 - StopPlayEventFailure</para> 
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Overloads Function PlaybackRecFromArchive_LockedEvent(ByVal EventKeyName As String, ByVal SecToPlay As Integer, ByVal FromBeginning As Boolean, ByVal VerifyEOF As Boolean, _
                                                       Optional ByVal StartTime As String = "", Optional ByVal EndTime As String = "", Optional ByVal EvDate As String = "") As IEXGateway._IEXResult
        Return _Manager.Invoke("PlaybackRecFromArchive", EventKeyName, SecToPlay, FromBeginning, VerifyEOF, True, StartTime, EndTime, EvDate, _Manager)
    End Function

    ''' <summary>
    '''    Stop Current Playback
    ''' </summary>
    ''' <param name="IsReviewBuffer">Optional Parameter Default = False : If True Stop From Review Buffer Else From Playback</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>343 - StopPlayEventFailure</para> 
    ''' </remarks>
    Public Function StopPlayback(Optional ByVal IsReviewBuffer As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("StopPlayback", IsReviewBuffer, _Manager)
    End Function

    ''' <summary>
    '''   Setting Trickmode Speed And Checkes BOF or EOF
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event - "RB" For Review Buffer Or Event Key Name For Regular Playback</param>
    ''' <param name="Speed">For Example : 1 For Play, 0 For Pause,0.5,2,6,12,30</param>
    ''' <param name="Verify_EOF_BOF">If True Verifies EOF Or BOF According To Direction</param>
    ''' <param name="IsReviewBufferFull">Optional Parameter Default = False : If True Review Buffer Full</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para>   
    ''' <para>305 - PCATFailure</para> 
    ''' <para>307 - GetStreamInfoFailure</para> 
    ''' <para>313 - SetTrickModeSpeedFailure</para> 
    ''' <para>320 - VerifyEofBofFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' </remarks>
    Public Function SetTrickModeSpeed(ByVal EventKeyName As String, ByVal Speed As Double, ByVal Verify_EOF_BOF As Boolean, Optional ByVal IsReviewBufferFull As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("SetTrickModeSpeed", EventKeyName, Speed, Verify_EOF_BOF, IsReviewBufferFull, _Manager)
    End Function

    ''' <summary>
    '''  Verifies EOF/BOF Of Playback Event That Is Currently Playing 
    ''' </summary>
    ''' <param name="DurationInSec">Duration Of The Event Or Played Duration</param>
    ''' <param name="Speed">Speed Of Trickmode</param>
    ''' <param name="EOF">Optional Parameter Default = False : If True EOF Else BOF</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>320 - VerifyEofBofFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function VerifyPlaybackEOFBOF(ByVal DurationInSec As Long, ByVal Speed As Double, Optional ByVal EOF As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyPlaybackEOFBOF", DurationInSec, Speed, EOF, _Manager)
    End Function

    ''' <summary>
    '''   Select And Pause From Action Bar Menu
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Function PauseFromActionBar() As IEXGateway._IEXResult
        Return _Manager.Invoke("PauseFromActionBar", _Manager)
    End Function

    ''' <summary>
    ''' Doing skip on trickmode bar
    ''' </summary>
    ''' <param name="direction">Direction - Madatory - direction of skip -  True if Forward else False. No default value</param>
    ''' <param name="PlaybackContext">PlaybackContext - Mandatory - whether RB or Playback  </param>
    ''' <param name="NumOfSkipPoints">NumofSkipPoints - optional - Number of skips to be done. Default value = 0 (check if you can set nothing as default value)</param>
    ''' <param name="SkipDurationSetting">SkipDurationSetting -  Current setting value for skip. Default value will be fetched from project ini (this includes the value - "Last Event Boundary")</param>
    ''' <returns></returns>
    ''' <remarks>  
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' Possible Error Codes:
    ''' <para>303 - FasVerificationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>313 - SetTrickModeSpeedFailure</para> 
    ''' <para>318 - SetSkipFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function Skip(ByVal direction As Boolean, ByVal PlaybackContext As Boolean, ByVal SkipDurationSetting As EnumVideoSkip, Optional ByVal NumOfSkipPoints As Integer = Nothing) As IEXGateway._IEXResult
        Return _Manager.Invoke("Skip", direction, PlaybackContext, SkipDurationSetting, NumOfSkipPoints, _Manager)
    End Function

    ''' <summary>
    '''    Canceling Booking From Guide
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="VerifyCanceledInPCAT">Optional Parameter Default = True : If True Verifies Event Canceled In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
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
    Public Function CancelBookingFromGuide(ByVal EventKeyName As String, Optional ByVal VerifyCanceledInPCAT As Boolean = True, Optional ByVal ReturnToLive As Boolean = True, Optional ByVal IsSeries As Boolean = False) As IEXGateway._IEXResult
        Return _Manager.Invoke("CancelBookingFromGuide", EventKeyName, VerifyCanceledInPCAT, ReturnToLive, IsSeries, _Manager)
    End Function

    ''' <summary>
    '''  Resolves recording and booking conflicts
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ResolveOption">Use: DEFER, AUTOMATICALLY, MANUALLY, CANCEL BOOKING</param>
    ''' <param name="VerifyInPCAT">If True Verifies Event Booked or Canceled In PCAT</param>
    ''' <param name="EventToCancel">Optional Parameter Default = 1 : Not Implemented</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' </remarks>
    Public Function ResolveConflict(ByVal EventKeyName As String, ByVal ResolveOption As String, ByVal VerifyInPCAT As Boolean, Optional ByVal EventToCancel As Integer = 1) As IEXGateway._IEXResult
        Return _Manager.Invoke("ResolveConflict", EventKeyName, ResolveOption, VerifyInPCAT, EventToCancel, _Manager)
    End Function

    ''' <summary>
    '''   Compares Current EPG Time And Given Event Times
    ''' </summary>
    ''' <param name="EventOccures">Can Be : Past,Current Or Future</param>
    ''' <param name="EventTimes">Event Time In Format HH:MM - HH:MM</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>309 - GetEpgTimeFailure</para> 
    ''' <para>350 - ParsingFailure</para>  
    ''' <para>362 - TimeFailure </para>        
    ''' </remarks>
    Public Function VerifyEventSchedule(ByVal EventOccures As EnumEventOccures, ByVal EventTimes As String) As IEXGateway._IEXResult
        Return _Manager.Invoke("VerifyEventSchedule", EventOccures, EventTimes, _Manager)
    End Function

    ''' <summary>
    '''   Navigates To Archive 
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function NavigateToArchive() As IEXGateway._IEXResult
        Return _Manager.Invoke("NavigateToArchive", _Manager)
    End Function

    ''' <summary>
    '''   Navigates To Planner
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function NavigateToPlanner() As IEXGateway._IEXResult
        Return _Manager.Invoke("NavigateToPlanner", _Manager)
    End Function

    

End Class
