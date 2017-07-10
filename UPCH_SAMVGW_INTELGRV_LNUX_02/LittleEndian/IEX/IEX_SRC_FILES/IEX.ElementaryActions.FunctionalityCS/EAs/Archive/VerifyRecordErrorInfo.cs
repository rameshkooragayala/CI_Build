using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /**
     * Information about the verify failed event.
     *
     * @author Avinoba
     * @date 09-Oct-13
     */

    public class VerifyRecordErrorInfo : IEX.ElementaryActions.BaseCommand
    {
        IEXGateway._IEXResult res;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private string _EventKeyName;
        EnumRecordErr _RecordError;
        private bool _VerifyInPCAT;
        private string _StartGuardTime;
        private string _EndGuardTime;


        /// <summary>
        /// Verified the Record Error Information for Failed and Partial recording
        /// </summary>
        /// <param name="EventKeyName">The Key of the Event</param>
        /// <param name="RecordError">The type of Record</param>
        /// <param name="StartGuardTime">Optional Parameter. Default="". search for the start time with the SGT if not empty</param>
        /// <param name="EndGuardTime">Optional Parameter. Default="". search for the end time with the EGT if not empty</param>
        /// <param name="VerifyInPCAT">Verifies in PCAT</param>
        /// <param name="pManager">Manager</param>
        public VerifyRecordErrorInfo(string EventKeyName, EnumRecordErr RecordError, string StartGuardTime, string EndGuardTime, bool VerifyInPCAT, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this._EventKeyName = EventKeyName;
            this._RecordError = RecordError;
            this._VerifyInPCAT = VerifyInPCAT;
            this._StartGuardTime = StartGuardTime;
            this._EndGuardTime = EndGuardTime;
            EPG = this._manager.UI;
        }

        /// <summary>
        ///   EA Execution
        /// </summary>
        protected override void Execute()
        {
            string eventName = "";
            string recordIcon = "";
            bool isFailedRecord = false;
            string evtDate = "";
            string recordErrDescription = "";
            string eventStartTime = "";
            string eventEndTime = "";
            string startTime = "";
            string endTime = "";


            try
            {
                eventName = this._manager.GetEventInfo(_EventKeyName, EnumEventInfo.EventName);
            }
            catch
            {

                ExceptionUtils.ThrowEx(new EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + this._EventKeyName + " Does Not Exists On Collection"));
            }

            if (_StartGuardTime != "")
            {
                try
                {
                    eventStartTime = _manager.GetEventInfo(_EventKeyName, EnumEventInfo.EventStartTime);
                    startTime = TimeSpan.Parse(eventStartTime).Subtract(TimeSpan.Parse("00:" + _StartGuardTime)).ToString("hh\\:mm");
                }
                catch
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.ParsingFailure, "Failed to Get Start Time"));
                }
            }

            if (_EndGuardTime != "")
            {
                try
                {
                    eventEndTime = _manager.GetEventInfo(_EventKeyName, EnumEventInfo.EventEndTime);
                    endTime = TimeSpan.Parse(eventEndTime).Add(TimeSpan.Parse("00:" + _EndGuardTime)).ToString("hh\\:mm");
                }
                catch
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.ParsingFailure, "Failed to Get Start Time"));
                }
            }

            if (_RecordError.ToString().ToLower().Contains("failed"))
            {
                isFailedRecord = true;
            }


            if (isFailedRecord)
            {
                recordIcon = "failed";

                EPG.PlannerBase.NavigateToFailedEventScreen();

                if (EPG.FutureRecordings.IsEmpty())
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.FindEventFailure, "Failed Event Screen Is Empty"));
                }

                EPG.PlannerBase.FindFailedRecordedEvent(eventName, evtDate, startTime, endTime);

                EPG.PlannerBase.SelectEvent();
            }
            else
            {
                recordIcon = "partial";

                EPG.ArchiveRecordings.Navigate();

                if (EPG.ArchiveRecordings.IsEmpty())
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.FindEventFailure, "Failed Event Screen Is Empty"));
                }

                EPG.ArchiveRecordings.FindEvent(eventName, evtDate, startTime, endTime);

                EPG.ArchiveRecordings.SelectEvent();
            }

            recordErrDescription = this.GetDictValueForRecordStatus();

            EPG.PlannerBase.VerifyErrorInfo(recordIcon, recordErrDescription);


            if (_VerifyInPCAT)
            {
                if (isFailedRecord)
                {
                    res = this._manager.PCAT.VerifyEventStatus(_EventKeyName, EnumPCATtables.FromBookings, "PARTIAL_CONTENT_STATUS", "NONE", true);
                }
                else
                {
                    res = this._manager.PCAT.VerifyEventPartialStatus(_EventKeyName, "PARTIAL");
                }

                if (!res.CommandSucceeded)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.PCATFailure, "Failed To Verify Record Error Status in PCAT "));
                }

            }

        }

        private string GetDictValueForRecordStatus()
        {
            string ReturnedString = "";


            switch (_RecordError)
            {
                case EnumRecordErr.Failed_PowerFailure:

                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_4010_PHRASE_2");
                    break;

                case EnumRecordErr.Failed_DiscSpace:
                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_4011_PHRASE_2");
                    break;

                case EnumRecordErr.Partial_UserStopped:
                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_4001_PHRASE_2");
                    break;

                case EnumRecordErr.Partial_SignalLoss:
                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_4021_PHRASE_2");
                    break;

                case EnumRecordErr.Partial_RecordedAfterEvtStart:
                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_4003_PHRASE_2");
                    break;

                case EnumRecordErr.Partial_ResumedRecording:
                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_4002_PHRASE_2");
                    break;

                case EnumRecordErr.Failed_UnSubscribedChannel:
                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_3030_PHRASE_2");
                    break;

                case EnumRecordErr.Partial_PowerFailure:
                    ReturnedString = EPG.Utils.GetValueFromDictionary("DIC_TECH_ERROR_4010_PHRASE_2");
                    break;
                default:
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, _RecordError + " Is Not A Valid Parameter"));
                    break;

            }
            return ReturnedString;
        }

    }
}
