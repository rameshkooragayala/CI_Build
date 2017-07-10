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
    public class GetAndVerifySoftVersion : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private IEXGateway._IEXResult res;
        private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
        private String _SoftVersion = "";
        private String _OldSoftVersion = "";
        private String _UsageID = "";
        private Boolean _IsVerify = false;

        public GetAndVerifySoftVersion(bool IsVerify, String OldSoftVersion, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._IsVerify = IsVerify;
            this._OldSoftVersion = OldSoftVersion;
			this._manager = pManager;
            EPG = this._manager.UI;
        }
        protected override void Execute()
        {
            String SoftVersion = "";
            String UsageID = "";

			EPG.Utils.NavigateToDiagnostics();
            res = _iex.MilestonesEPG.GetEPGInfo("software version", out _SoftVersion);
            if (!res.CommandSucceeded)
            {
                EPG.Utils.LogCommentFail("Failed to get software version");
            }

            res = _iex.MilestonesEPG.GetEPGInfo("CPE USAGE ID", out _UsageID);
            if (!res.CommandSucceeded)
            {
                EPG.Utils.LogCommentFail("Failed to get Usage ID");
            }
        
            SoftVersion = _SoftVersion.Split('_').Last();
            UsageID = _UsageID;
            _iex.LogComment("Version ID is : " + SoftVersion);

            if (_IsVerify)
            {
                try
                {
                    EPG.OTA.VerifySoftVersion(SoftVersion, _OldSoftVersion);
                }
                catch
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.MountFailure, "Failed to verify software version is updated after download"));
                }
            }

            SetReturnValues(new String[] { SoftVersion, UsageID });
        }
    }
}
