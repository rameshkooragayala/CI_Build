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
    public class StopRecordUsingStopKey : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;
        private IEXGateway._IEXResult res;
        private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
        private String _EventKeyName;
        private bool _IsCurrent;
        private bool _IsSeries;
        private EnumRecordIn _RecordIn;
        private bool _isStopRecording;
        private bool _isTBR;
        private String chName = "";


        public StopRecordUsingStopKey(EnumRecordIn RecordIn, String EventKeyName,String ChannelNumber,bool IsCurrent, bool IsSeries, bool IsStopRecord, bool IsTBR, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._manager = pManager;
            EPG = this._manager.UI;
            this._EventKeyName = EventKeyName;
            this._IsCurrent = IsCurrent;
            this._IsSeries = IsSeries;
            this._RecordIn = RecordIn;
            this._isStopRecording = IsStopRecord;
            this._isTBR = IsTBR;
        }

        protected override void Execute()
        {
            IEXGateway.IEXResult res = new IEXGateway.IEXResult();

            string EventName = "";

            if (string.IsNullOrEmpty(this._EventKeyName))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.NoValidParameters, "Param EventKeyName Is Empty"));
            }
            else
            {
                try
                {
                    EventName = EPG.Events[_EventKeyName].Name;
                }
                catch (Exception ex)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.EventNotExistsFailure, "Event With Key Name - " + this._EventKeyName + " Does Not Exists On Collection"));
                }
            }

            switch (_RecordIn)
            {

                case EnumRecordIn.Guide:
                    {
                        EPG.Guide.Navigate();
                        //EPG.Guide.NavigateToChannel(_ChannelNumber, true);
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

            if (!(_isTBR))
            {
                EPG.Utils.StartHideFailures("Trying To Find Event : " + EventName);

                try
                {
                    EPG.Banner.VerifyEventName(EventName);
                }
                catch (Exception ex)
                {
                    EPG.Utils.LogCommentInfo("Didn't Find Event On Current Trying To Find On Next Event");

                    EPG.Utils.ReturnToLiveViewing();
                    EPG.ChannelBar.NextEvent(false);
                }
            }

            _iex.ForceHideFailure();

            EPG.Utils.StopRecordingSTOPkey(_IsSeries, _isStopRecording, _IsCurrent, _isTBR);

            }
     }

}

