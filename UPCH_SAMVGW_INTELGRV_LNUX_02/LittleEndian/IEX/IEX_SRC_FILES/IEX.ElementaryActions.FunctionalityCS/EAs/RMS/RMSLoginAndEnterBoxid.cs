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
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;



namespace RMSEAImplementation
{
    class RMSLoginAndEnterBoxid : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private FirefoxDriver _driver;
        
        
        public string  _CpeId;
        public string _BrowserTabControlId;
        //static IEXGateway._IEXResult res;
        //static bool _IsLastDelivery;
        public RMSLoginAndEnterBoxid(FirefoxDriver driver, string CpeId, string BrowserTabControlId, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._driver=driver;
            this._manager = pManager;
            this._CpeId = CpeId;
            this._BrowserTabControlId = BrowserTabControlId;
            EPG = this._manager.UI;
        }

        /// <summary>
        /// EA Execution
        /// </summary>
        protected override void Execute()
        {
           EPG.RMS.RmsLoginValidation(_driver);
           EPG.RMS.EnterCpeId(_driver, _CpeId);
           EPG.RMS.SelectTab(_driver, _BrowserTabControlId);

        }
     }
}
