using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;



namespace EAImplementation
{
    /// <summary>
    ///  Parses the RB depth milestones and calculate the RB depth
    /// </summary>
    public class FlushRB : IEX.ElementaryActions.BaseCommand
    {
        private double _waitInStby;
        private Manager _manager;

        /// <param name="waitInStby">time to wait in standby in secs</param>
        /// <param name="manager">the manager</param>
        /// <remarks>
        /// Possible Error Codes:
        /// <para>304 - IRVerificationFailure</para> 
        /// <para>322 - VerificationFailure</para> 
        /// <para>332 - NoValidParameters</para> 
        /// <para>350 - ParsingFailure</para> 
        /// </remarks>

        public FlushRB(double waitInStby, Manager manager)
        {

            _waitInStby = waitInStby;
            _manager = manager;
        }

        /// <summary>
        ///  EA Execution
        /// </summary>
        protected override void Execute()
        {
            IEXGateway._IEXResult res;

            res = _manager.StandBy(false);
            if (!res.CommandSucceeded)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Failed to put the STB into standby"));
            }

            _iex.Wait(_waitInStby);

            res = _manager.StandBy(true);
            if (!res.CommandSucceeded)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Failed to wakeup the STB from standby"));

            }

        }

    }
}