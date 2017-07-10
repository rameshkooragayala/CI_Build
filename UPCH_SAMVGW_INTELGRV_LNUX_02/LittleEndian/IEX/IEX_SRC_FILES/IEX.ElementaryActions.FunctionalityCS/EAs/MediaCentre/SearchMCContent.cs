using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FailuresHandler;

namespace IEX.ElementaryActions.FunctionalityCS.EAs.MediaCentre
{
    public class SearchMCContent : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private MediaContent _Content;
        private Boolean _Navigate;
        private Boolean _fromDeviceNavigator;


        // <param name="content">Media Content to navigate to</param>
        // <param name="navigate">Optional Parameter navigate Default = True</param>
        // <param name="fromDeviceNavigator">Optional Parameter fromDeviceNavigator Default = True</param>
        // <param name="pManager">Manager</param>
        // <remarks>
        // Possible Error Codes:
        // <para>300 - NavigationFailure</para> 
        // <para>301 - DictionaryFailure</para> 
        // <para>302 - EmptyEpgInfoFailure</para> 
        // <para>304 - IRVerificationFailure</para> 
        // <para>322 - VerificationFailure</para> 
        // <para>328 - INIFailure</para>    
        // <para>332 - NoValidParameters</para> 
        // </remarks>

        public SearchMCContent(MediaContent content, Boolean navigate,Boolean fromDeviceNavigator, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._Content = content;
            this._Navigate = navigate;
            this._fromDeviceNavigator = fromDeviceNavigator;
            this._manager = pManager;
            EPG = this._manager.UI;

        }
        protected override void Execute()
        {
            // Validate input parameters
            if (_Content.Equals(null))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Content object is null!"));
            }

            if (_Navigate)
            {
                EPG.MediaCentre.Navigate(_fromDeviceNavigator, _Content.Type);
            }

            EPG.MediaCentre.NavigateToContent(_Content);

        }

    }
}
