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
    public class OtaDownload : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private String _VersionID;
        private String _UsageID;
        private String _NITTable;
        private String _RFFeed;
        private String _Project;
		private bool _isLive;
        private IEXGateway._IEXResult res;
        private bool _IsLastDelivery;
        public OtaDownload(String VersionID, String UsageID, String _NITTable, bool IsLastDelivery, String RFFeed, bool isLive, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._VersionID = VersionID;
            this._manager = pManager;
            this._UsageID = UsageID;
            this._NITTable = _NITTable;
            this._IsLastDelivery = IsLastDelivery;
            this._RFFeed = RFFeed;
			this._isLive = isLive;
            EPG = this._manager.UI;
        }

        /// <summary>
        /// EA Execution
        /// </summary>
        protected override void Execute()
        {
            _Project = _manager.GetValueFromINI(EnumINIFile.Environment, "IEX", "Project");
            if (string.IsNullOrEmpty(_Project))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.INIFailure, "Failed to fetch Project from Environment.ini"));
            }
            else
            {
                EPG.Utils.LogCommentInfo("Project fetched is: " + _Project);
            }
            if (_IsLastDelivery)
            {
                res = _manager.MountGw(EnumMountAs.NOFORMAT, IsLastDelivery: _IsLastDelivery);
                if (!res.CommandSucceeded)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.MountFailure, "Failed to Mount the last delivery image"));
                }
                _iex.Wait(120);
                if (_isLive)
                {
                    res = _manager.StandBy(true);
                    if (!res.CommandSucceeded)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.StandByFailure, "Failed to come out of stand by after mouting last delivery binary"));
                    }
                    _iex.Wait(10);
                }
            }
            //Copy binary
            EPG.OTA.CopyBinary();

            //Modify Imgae version
            EPG.OTA.ModifyImageVersion(_VersionID);

            if(_Project != ("IPC"))
            {
                //Create carousel
                EPG.OTA.Create_Carousel(_VersionID, _UsageID, _RFFeed);

                //Broadcast and rest carousel
                EPG.OTA.BroadcastCarousel(_RFFeed);
            }
            //Broadcast NIT
            EPG.OTA.NITBraodcast(_NITTable);

        }
    }
}
