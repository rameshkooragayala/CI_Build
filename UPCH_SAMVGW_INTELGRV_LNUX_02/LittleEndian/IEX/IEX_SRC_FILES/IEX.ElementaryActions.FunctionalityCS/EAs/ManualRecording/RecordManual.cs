using System;
using System.Collections.Generic;
using System.Globalization;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace IEX.ElementaryActions.FunctionalityCS.EAs.ManualRecording
{
    class RecordManual :IEX.ElementaryActions.BaseCommand
    {

        private IEX.ElementaryActions.EPG.SF.UI EPG ; 
        private IEX.ElementaryActions.Functionality.Manager _manager ;
        private String _EventKeyName ;
        private String _ChannelName ;
        private int _ChannelNumber ;
        private int _DaysDelay ;
        private int _MinutesDelayUntilBegining ;
        private int _DurationInMin ;
        private EnumFrequency _Frequency ;
        private Boolean _VerifyBookingInPCAT  ;
        private Boolean _IsConflict ;
        private Boolean _IsCurrent ;
        private Boolean _NoEIT ;
        private String _StartTime;



        // <param name="EventKeyName">Key Of The Event</param>
        // <param name="ChannelName">Channel Name</param>
        // <param name="ChannelNumber">Channel Number If Entered Doing DCA</param>
        // <param name="DaysDelay">Optional Parameter Default = -1 : Adds Days From Current Time</param>
        // <param name="MinutesDelayUntilBegining">Optional Parameter Default = -1 : Minutes Delay Until Beginning</param>
        // <param name="DurationInMin">Optional Parameter Default = 1 : Duration Of Recording</param>
        // <param name="Frequency">Optional Parameter Default = ONE_TIME</param>
        // <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Booking In PCAT</param>
        // <param name="IsConflict">Optional Parameter Default = False : If True Verify Conflict Appeared Before Recording Is Confirmed</param>
        // <param name="pManager">Manager</param>
        // <remarks>
        // Possible Error Codes:
        // <para>300 - NavigationFailure</para> 
        // <para>301 - DictionaryFailure</para> 
        // <para>302 - EmptyEpgInfoFailure</para> 
        // <para>304 - IRVerificationFailure</para> 
        // <para>305 - PCATFailure</para> 
        // <para>309 - GetEpgTimeFailure</para>    
        // <para>310 - GetEpgDateFailure</para>    
        // <para>322 - VerificationFailure</para> 
        // <para>328 - INIFailure</para>  
        // <para>330 - TelnetFailure</para> 
        // <para>331 - CopyFileFailure</para> 
        // <para>332 - NoValidParameters</para> 
        // <para>334 - VideoNotPresent</para>  
        // <para>339 - RecordEventFailure</para> 
        // <para>344 - ConflictFailure</para>  
        // <para>349 - ReturnToLiveFailure</para> 
        // <para>350 - ParsingFailure</para> 
        // </remarks>


        public RecordManual(String EventKeyName, String ChannelName,int ChannelNumber,int DaysDelay, int MinutesDelayUntilBegining,
                String StartTime, int DurationInMin, EnumFrequency Frequency, Boolean VerifyBookingInPCAT,
                Boolean IsConflict,Boolean IsCurrent,Boolean NoEIT, IEX.ElementaryActions.Functionality.Manager pManager )
        {
            _EventKeyName = EventKeyName;
            _ChannelName = ChannelName;
            _ChannelNumber = ChannelNumber;
            _DaysDelay = DaysDelay;
            _MinutesDelayUntilBegining = MinutesDelayUntilBegining;
            _DurationInMin = DurationInMin;
            _Frequency = Frequency;
            _VerifyBookingInPCAT = VerifyBookingInPCAT;
            _IsConflict = IsConflict;
            _StartTime = StartTime;
            _IsCurrent = IsCurrent;
            _NoEIT = NoEIT;
            _manager = pManager;
            EPG = _manager.UI;
        }
        

        protected override void Execute()
        {
            String frequency = "";
            int occurences =-1;
            String EPGTime = "";
            String EPGDate = "";
            String startTimeToSet = "";
            String endTimeToSet = "";
            String EPGEventdate = "";
            String PCATEventDate = "";
            String EPGDateFormat = "";
            String PCATDateFormat = "";
            String tuneToService ="";
            String milestoneDateFormat = "";
            Boolean isCurrentRecording = false;
            String EventSource = "";
            String EventName = "";
            
            IEXGateway._IEXResult res;

            tuneToService = EPG.Utils.GetValueFromProject("MANUAL_RECORDING", "TUNE_TO_SERVICE_BEFORE_RECORD");
            milestoneDateFormat = EPG.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT");    
            EPGDateFormat = EPG.Utils.GetEpgDateFormat();
            PCATDateFormat = EPG.Utils.GetDateFormatForEventDictionary();

            // Validate input parameters
            if(String.IsNullOrEmpty(_EventKeyName))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"));
            }
            
            if(String.IsNullOrEmpty(_ChannelName) &&  _ChannelNumber == -1)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Channel Argument Can't Be Empty"));
            }

            if(_DurationInMin <= 0)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "DurationInMin Can't Be Equal Or Less Than 0"));
            }

            if(_StartTime != "" && _DaysDelay < 1)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "DaysDelay Can't Be Less Then 1 In Case StartTime Entered"));
            }

            // Set validation params for frequency and occurences accordingly
            switch(_Frequency)
            {
                case EnumFrequency.ONE_TIME: 
                    frequency = EPG.Utils.GetValueFromDictionary("DIC_ONE_TIME");
                    break;
                case EnumFrequency.DAILY:
                    frequency = EPG.Utils.GetValueFromDictionary("DIC_DAILY");
                    break;
                case EnumFrequency.WEEKLY:
                    frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKLY");
                    break;
                case EnumFrequency.WEEKDAY:
                    frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKDAY");
                    break;
                case EnumFrequency.WEEKEND:
                    frequency = EPG.Utils.GetValueFromDictionary("DIC_WEEKEND");
                    break;
                default:
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "No Valid Frequency Has Been Entered"));
                    break;
             }

            // In case from Planner, keep 2 min buffer for delay until beginning to account for time taken up for scheduling the recording
            if (!_IsCurrent)
            {
                if (_MinutesDelayUntilBegining < 0)
                {
                    _MinutesDelayUntilBegining = 2;
                }
                else
                {
                    _MinutesDelayUntilBegining += 2;
                }
            }
            else
            {
                _MinutesDelayUntilBegining = 0;
            }

            // Tune to service if required
            String channelName = "";
            int channelNum = -1;

            if (_IsCurrent || tuneToService.ToUpper() == "TRUE")
            {
                if (_ChannelNumber == -1)
                {
                    try
                    {
                        _ChannelNumber = int.Parse(_manager.GetServiceFromContentXML("Name=" + _ChannelName).LCN);
                    }
                    catch (Exception ex)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.GetChannelFailure,ex.Message));
                    }

                }
                res = _manager.ChannelSurf(EnumSurfIn.Live, _ChannelNumber.ToString());
                if (!res.CommandSucceeded)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.TuneToChannelFailure, res.FailureReason));
                }

            }
            else
            {
                channelName = _ChannelName;
                channelNum = _ChannelNumber;
            }

            // Set the event Name
            EventName = _ChannelName;
            if (_ChannelName == "")
            {
                EventName = _manager.GetServiceFromContentXML("LCN=" + _ChannelNumber).Name;
				channelName = EventName;
            }

            // Calculate start Time to be set
            EPG.Live.GetEpgTime(ref EPGTime);
            if (_StartTime == "")
            {
                // Handle the case if Minutes delay passed is more than a day
                if (_MinutesDelayUntilBegining > ((24 * 60) - ((DateTime.Parse(EPGTime).Hour * 60) + DateTime.Parse(EPGTime).Minute)))
                {
                    if (_DaysDelay == -1)
                    {
                        _DaysDelay = 0;
                        _DaysDelay = _DaysDelay + _MinutesDelayUntilBegining / (24 * 60) + 1;
                    }
                    else
                        _DaysDelay = _DaysDelay + _MinutesDelayUntilBegining / (24 * 60);

                    _MinutesDelayUntilBegining = _MinutesDelayUntilBegining % (24 * 60);

                }

                startTimeToSet = DateTime.Parse(EPGTime).AddMinutes(_MinutesDelayUntilBegining).ToString("HH:mm");

            }
            else
            {
                startTimeToSet = _StartTime;
            }

            // Set Event Source
            if (_IsCurrent)
            {
                isCurrentRecording = true;
                EventSource = "ManualCurrent";
            }
            else
            {
                EventSource = "ManualFuture";
            }

            // Calculate end time to be set
            endTimeToSet = (DateTime.Parse(startTimeToSet).AddMinutes(_DurationInMin)).ToString("HH:mm");

            // Set proper days delay param
            if (_DaysDelay < 0)
            {
                _DaysDelay = 0;
            }

            // Calculate the date to be set
            EPG.Live.GetEpgDate(ref EPGDate);

            EPGEventdate = DateTime.ParseExact(EPGDate, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(_DaysDelay).ToString(EPGDateFormat);
            PCATEventDate = DateTime.ParseExact(EPGDate, milestoneDateFormat, CultureInfo.InvariantCulture).AddDays(_DaysDelay).ToString(PCATDateFormat);

            // Get the expected number of occurences
            EPG.ManualRecording.GetOccurences(frequency, _DaysDelay, ref occurences);

            // Navigate to Manual Recording screen from Planner
            EPG.ManualRecording.Navigate(_IsCurrent, _NoEIT);

            // Set params for manual recording
            if (_IsCurrent)
            {
                EPG.ManualRecording.SetManualRecordingParams("", "", "",-1, endTimeToSet, frequency);
            }
            else
            {
                EPG.ManualRecording.SetManualRecordingParams(EPGEventdate, startTimeToSet, channelName, channelNum, endTimeToSet, frequency);
            }

            // Save and verify
            if(_IsConflict)
            {

                EPG.Utils.InsertEventToCollection(_EventKeyName, EventName, EventSource, startTimeToSet, endTimeToSet, _ChannelName, _DurationInMin * 60, _DurationInMin * 60, PCATEventDate, frequency, occurences, EPGEventdate, false);
                try
                {
				    EPG.ManualRecording.NavigateToRecord(false);
                    EPG.Menu.SelectToConflict();
                }
                catch(Exception ex)
                {
                    EPG.Events.Remove(_EventKeyName);
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.ConflictFailure, "Failed To Create Conflict By Recording Event From Planner:"+ex.Message));
                }
            }
            else
            {
                EPG.ManualRecording.SaveAndEnd(isCurrentRecording);

                try
                {
                    EPG.ManualRecording.VerifySaveAndEndFinished(isCurrentRecording);
                }
                catch(Exception ex)
                {
                    EPG.Events.Remove(_EventKeyName);
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.DeleteEventFailure, ex.Message));
                }

                EPG.Utils.InsertEventToCollection(_EventKeyName, EventName, EventSource, startTimeToSet, endTimeToSet, _ChannelName, _DurationInMin * 60, _DurationInMin * 60, PCATEventDate, frequency, occurences, EPGEventdate,false);

                if(_VerifyBookingInPCAT)
                {
                    res = _manager.PCAT.VerifyEventBooked(_EventKeyName);
                    if(!res.CommandSucceeded)
                    {
                        EPG.Events.Remove(_EventKeyName);
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, res.FailureReason));
                    }
                }

                EPG.Utils.ReturnToLiveViewing();
            }
        
        }
    }
}
