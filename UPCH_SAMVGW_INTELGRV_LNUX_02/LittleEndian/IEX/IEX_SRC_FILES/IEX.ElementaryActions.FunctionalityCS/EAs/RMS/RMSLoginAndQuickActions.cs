using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;

namespace IEX.ElementaryActions.FunctionalityCS.EAs.RMS
{
    class RMSLoginAndQuickActions: IEX.ElementaryActions.BaseCommand
    {
         private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private FirefoxDriver _driver;

        public string _quickActionControlId;
        public string _quickActionConfirmId;
        public string  _cpeId;
        
        public RMSLoginAndQuickActions(FirefoxDriver driver, string cpeId, string quickActionControlId,string quickActionConfirmId, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._driver=driver;
            this._manager = pManager;
            this._cpeId = cpeId;
            this._quickActionControlId = quickActionControlId;
            this._quickActionConfirmId = quickActionConfirmId;
            EPG = this._manager.UI;
        }

        /// <summary>
        /// EA Execution
        /// </summary>
        protected override void Execute()
        {
//Login to the panorama page
           EPG.RMS.RmsLoginValidation(_driver);
            //enters cpeid and search 
           EPG.RMS.EnterCpeId(_driver, _cpeId);
            //Select Quick Action and Performs Action
           EPG.RMS.QuickActionSelect(_driver, _quickActionControlId, _quickActionConfirmId);
        }
    }
}
