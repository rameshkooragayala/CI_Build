using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Deletes the Failed Recorded Event
    /// </summary>
    public class DeleteFailedRecordedEvent : IEX.ElementaryActions.BaseCommand
    {
        IEXGateway._IEXResult res;
        private IEX.ElementaryActions.EPG.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private string _EventKeyName;
        private bool _VerifyDeleteInPcat;

        /// <param name="EventKeyName">Key of the Event</param>
        /// <param name="VerifyDeleteInPCAT">Optional Parameter Dafault=true. Verifies Event has deleted</param>
        /// <param name="pManager">manager</param>
        public DeleteFailedRecordedEvent(string EventKeyName, bool VerifyDeleteInPCAT, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            this._EventKeyName = EventKeyName;
            this._VerifyDeleteInPcat = VerifyDeleteInPCAT;
            EPG = this._manager.UI;
        }

        /// <summary>
        ///   EA Execute
        /// </summary>
        protected override void Execute()
        {

            string eventName = "";

            try
            {
                eventName = this._manager.GetEventInfo(_EventKeyName, EnumEventInfo.EventName);
            }
            catch
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + this._EventKeyName + " Does Not Exists On Collection"));
            }


            EPG.PlannerBase.NavigateToFailedEventScreen();

            EPG.PlannerBase.FindFailedRecordedEvent(eventName, "", "", "");

            EPG.FutureRecordings.SelectEvent();

            EPG.FutureRecordings.DeleteEvent();

            if (this._VerifyDeleteInPcat)
            {
                res = this._manager.PCAT.VerifyEventDeleted(_EventKeyName, EnumPCATtables.FromBookings);
                if (!res.CommandSucceeded)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.PCATFailure, res.FailureReason));
                }
            }

        }
    }
}
