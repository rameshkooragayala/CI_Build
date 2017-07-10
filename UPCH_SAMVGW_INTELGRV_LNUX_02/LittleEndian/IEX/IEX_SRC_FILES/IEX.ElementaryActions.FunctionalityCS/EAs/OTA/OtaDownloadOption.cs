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
    public class OtaDownloadOption : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private EnumOTADownloadOption  _Option;
        private bool _Selection;
        private IEXGateway._IEXResult res;

        public OtaDownloadOption(EnumOTADownloadOption dwnloadOption, bool IsDownload, IEX.ElementaryActions.Functionality.Manager pManager)
        {

            this._Selection = IsDownload;
            this._Option = dwnloadOption;
			this._manager = pManager;
            EPG = this._manager.UI;
        }

        /// <summary>
        /// EA Execution
        /// </summary>
        protected override void Execute()
        {
            switch (_Option)
            {

                case EnumOTADownloadOption.AUTOMATIC:
                    {
                        EPG.OTA.OTADownloadOption("AUTOMATIC",_Selection);
                        break;
                    }
                    
                case EnumOTADownloadOption.FORCED:
                    {
                        EPG.OTA.OTADownloadOption("FORCED");
                        break;
                    }
                case EnumOTADownloadOption.MANUAL:
                    {
                        break;
                    }

            }
            



             
          
        }
        
    }
}
