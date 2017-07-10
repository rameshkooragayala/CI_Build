using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.Collections;
using System.Data;
using System.Diagnostics;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;
using System.Globalization;
using System.IO;
using IEXGateway;


namespace EAImplementation
{
    public class RecordUsingRECkey : IEX.ElementaryActions.BaseCommand
    {

        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private IEXGateway._IEXResult res;
        private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
        private EnumRecordIn _RecordIn;
        private String _EventKeyName;
        private String _ChannelNumber;
        private int _MinTimeBeforeEvEnd;
        private bool _VerifyBookingInPCAT;
        private bool _ReturnToLive;
        private bool _IsConflict;
        private bool _IsCurrent;
        private bool _IsSeries;

        public RecordUsingRECkey(EnumRecordIn RecordIn, String EventKeyName, String ChannelNumber, int MinTimeBeforeEvEnd, bool VerifyBookingInPCAT, bool ReturnToLive, bool IsConflict, bool IsCurrent, bool IsSeries, IEX.ElementaryActions.Functionality.Manager pManager)
        {
          
			this._manager = pManager;
            EPG = this._manager.UI;
            this._RecordIn = RecordIn;
            this._EventKeyName = EventKeyName;
            this._ChannelNumber = ChannelNumber;
            this._MinTimeBeforeEvEnd = MinTimeBeforeEvEnd;
            this._VerifyBookingInPCAT = VerifyBookingInPCAT;
            this._ReturnToLive = ReturnToLive;
            this._IsConflict = IsConflict;
            this._IsCurrent = IsCurrent;
            this._IsSeries = IsSeries;
        }
        protected override void Execute()
        {
            IEXGateway._IEXResult res = default(IEXGateway._IEXResult);
            string EventName = "";
            string StartTime = "";
            string EndTime = "";
            int OriginalDuration = 0;
            int Duration = 0;
            string milestoneDateFormat = "";
            string EventDate = "";
            int TimeLeft = 0;

            milestoneDateFormat = EPG.Utils.GetValueFromProject("EPG", "MILESTONE_DATE_FORMAT");
            if (string.IsNullOrEmpty(_EventKeyName))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "EventKeyName Is Empty"));
            }

            if (Convert.ToInt32(_ChannelNumber) <= 0)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "ChannelNumber Can't Be Equal Or Less Than 0"));
            }

            res = _manager.ChannelSurf(EnumSurfIn.Live, _ChannelNumber);

            if (this._MinTimeBeforeEvEnd > 0)
            {
                EPG.Banner.GetEventTimeLeft(ref TimeLeft);

                if (TimeLeft <= _MinTimeBeforeEvEnd * 60)
                {
                    EPG.Utils.LogCommentImportant("Current Event Time Left Was Less Than " + Convert.ToString(_MinTimeBeforeEvEnd) + " Minutes Waiting " + (TimeLeft + 5).ToString() + " Seconds");
                    EPG.Utils.ReturnToLiveViewing();
                    _iex.Wait(TimeLeft + 5);

                    EPG.Utils.EPG_Milestones_NavigateByName("STATE:LIVE");
                }
            }


            switch (_RecordIn)
            {

                case EnumRecordIn.Guide:
                    {
                        EPG.Guide.Navigate();
                        EPG.Guide.NavigateToChannel(_ChannelNumber, true);
                        if (!(_IsCurrent))
                        {
                            EPG.Guide.NextEvent();
                        }
                        break;
                    }

                case EnumRecordIn.ChannelBar:
                    {
                        if (_IsCurrent)
                        {
                            EPG.ChannelBar.Navigate();
                        }

                        else
                        {
                            EPG.ChannelBar.NextEvent(false);
                        }
                        break;
                    }

                case EnumRecordIn.ActionBar:
                    {
                        EPG.Banner.Navigate();
                        break;
                    }

                case EnumRecordIn.Live:
                    {
                        EPG.Utils.ReturnToLiveViewing(true);
                        break;
                    }
            }

            EPG.Banner.GetEventName(ref EventName);

            if (string.IsNullOrEmpty(EventName) | EventName == "null")
            {
                SetWarning("Event Name Is Empty Will Fail Test If Tryied To Use");
            }

            EPG.Banner.GetEventStartTime(ref StartTime);

            EPG.Banner.GetEventEndTime(ref EndTime);
            EPG.Live.GetEpgDate(ref EventDate);
            EventDate = DateTime.ParseExact(EventDate, milestoneDateFormat, CultureInfo.InvariantCulture).ToString(EPG.Utils.GetEpgDateFormat());

            OriginalDuration = EPG.Utils._DateTime.SubtractInSec(Convert.ToDateTime(EndTime), Convert.ToDateTime(StartTime));

            EPG.Banner.GetEventTimeLeft(ref Duration);

            EPG.Utils.InsertEventToCollection(_EventKeyName, EventName, "GuideCurrent", StartTime, EndTime, Convert.ToString(_ChannelNumber), Duration, OriginalDuration, "", "",
            0, EventDate);

            if (_IsConflict)
            {
                try
                {
                    EPG.Menu.SelectToConflict();
                }
                catch
                {
                    EPG.Events.Remove(_EventKeyName);
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.SelectEventFailure, "Failed to select event " + _EventKeyName + "'"));
                }
            }
            else
            {
                try
                {
                    EPG.Utils.PreRecordRECkey(_IsCurrent, _IsConflict, _IsSeries);
                    _iex.Wait(2);
                    EPG.Banner.RecordEvent(_IsCurrent, false, _IsConflict, false, _IsSeries);
                }
                catch
                {
                    EPG.Events.Remove(_EventKeyName);
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.RecordEventFailure, "Failed to Record event " + _EventKeyName + "'"));
                }
                if (_IsCurrent)
                {
                    if (this._VerifyBookingInPCAT)
                    {
                        res = this._manager.PCAT.VerifyEventBooked(this._EventKeyName);
                        if (!res.CommandSucceeded)
                        {
                            EPG.Events.Remove(_EventKeyName);
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.PCATFailure, res.FailureReason));
                        }
                    }
                }
                else
                {
                    if (this._VerifyBookingInPCAT)
                    {
                        res = this._manager.PCAT.VerifyEventBooked(this._EventKeyName);
                        if (!res.CommandSucceeded)
                        {
                            EPG.Events.Remove(_EventKeyName);
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.PCATFailure, res.FailureReason));
                        }
                        res = this._manager.PCAT.VerifyEventStatus(this._EventKeyName, EnumPCATtables.FromBookings, "BOOKING_TYPE", "RECORD", false);
                        if (!res.CommandSucceeded)
                        {
                            EPG.Events.Remove(_EventKeyName);
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.PCATFailure, res.FailureReason));
                        }
                    }
                }

                if (this._ReturnToLive)
                {
                    EPG.Utils.ReturnToLiveViewing();
                }
           }

        }  

    }
}
