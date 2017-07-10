/// <summary>
///  Script Name : FT172_0014_Event_Info_Grid.cs
///  Test Name   : FT172-0014-Event-Info-in-GRID
///  TEST ID     : 
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Madhu Kumar k
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using System.Collections.Generic;


public class FT172_0014 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service FvrtService_1;
    private static Service FvrtService_2;
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static string timeStamp;
    private static string sgtFriendlyName;
    private static string egtFriendlyName;
    private static string namedNavigationGrid;
    private static string stateAfterEnetringPin;
   static Helper helper = new Helper();
   private static Dictionary<string, string> infoDictionary = new Dictionary<string, string>();
    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml and ini File, and set two service as Favourite");
        this.AddStep(new Step1(), "Step1: Enable Favourite mode and launch TV Guide on Favourite Service S1");
        this.AddStep(new Step2(), "Step2: Launch Action Bar and verify EPG info");
        this.AddStep(new Step3(), "Step3: Disable Favourite Mode, Launch TV Guide and Navigate to Future event in S1");
        this.AddStep(new Step4(), "Step4: Launch Action Bar and Verify EPG info");
        this.AddStep(new Step5(), "Step5: Tune to a locked Service in Guide and verify the EPG info on Action bar after entering the Pin");
        this.AddStep(new Step6(), "Step6: Tune to service s2 in guide and verify the EPG info status including the Ongoing Record Status");
        this.AddStep(new Step7(), "Step7: Browse to the future event and verify the  EPG info status including the Future Record Status");
        this.AddStep(new Step8(), "Step8: Tune to favourite Service FS2 and verify the  EPG info status including the ongoing Record Status");
        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            FvrtService_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;HasChannelLogo=True", "ParentalRating=High");
            if (FvrtService_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + FvrtService_1.LCN);
            }

            FvrtService_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;HasChannelLogo=False", "ParentalRating=High");
            if (FvrtService_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + FvrtService_2.LCN);
            }


            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + FvrtService_1.LCN + "," + FvrtService_2.LCN);
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_1.LCN);
            }


            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;Resolution=HD", "ParentalRating=High;LCN=" + FvrtService_1.LCN + "," + FvrtService_2.LCN + "," + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_2.LCN);
            }

            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;ParentalRating=High", "");
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_3.LCN);
            }
            //Fetch the Grid Named Navigation and State after enterin PIN from Test ini
            namedNavigationGrid = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NAMED_NAVIGATION_GRID");
            if (namedNavigationGrid == "")
            {
                FailStep(CL, "NAMED_NAVIGATION_GRID is not defined in the Test ini");
            }
            else
            {
                LogCommentImportant(CL, "NAMED_NAVIGATION_GRID fetched from Test ini is "+namedNavigationGrid);
            }

            stateAfterEnetringPin = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "STATE_AFTER_PIN");
            if (stateAfterEnetringPin == "")
            {
                FailStep(CL, "STATE_AFTER_PIN fetched from Test ini is null");
            }
            else
            {
                LogCommentImportant(CL, "STATE_AFTER_PIN fetched from test ini is "+stateAfterEnetringPin);
            }
            //Unsetting all the favourite services
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to UnsetAllFavChannels");
            }
            //Setting s1 and s2 as Favourite services
            res = CL.EA.STBSettings.SetFavoriteChannelNameList(FvrtService_1.Name + "," + FvrtService_2.Name, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Service : " + FvrtService_1.LCN + "," + FvrtService_2.LCN + " as Favourite from Settings");
            }

            sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "MIN");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, "sgtFriendlyName is not defined in the projrct ini file");
            }

            egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "MIN");
            if (sgtFriendlyName == "")
            {
                FailStep(CL, "egtFriendlyName is not defined in the projrct ini file");
            }

            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }

            res = CL.EA.STBSettings.SetGuardTime(false, egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
            }
            //Recording time based from planner on Favourite service 2
            res = CL.EA.PVR.RecordManualFromPlanner("TimeBasedRecording", FvrtService_2.Name, DaysDelay: -1, MinutesDelayUntilBegining: 3, DurationInMin: 20, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record manual from planner on " + FvrtService_2.Name);
            }

            res = CL.EA.WaitUntilEventStarts("TimeBasedRecording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until event ends");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_2.LCN);
            }
            //recording current event and future event on banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventBasedRecording", MinTimeBeforeEvEnd: 5, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book the future event from banner on " + Service_2.LCN);
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("EventBasedRecording1", VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book the future event from banner on " + Service_2.LCN);
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Enable Favourite mode
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ENABLE FAVOURITE MODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable the favourite mode");
            }
            //Tune to service 1 in Favourite mode
            CL.EA.ChannelSurf(EnumSurfIn.Live, "1");

            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }

            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.IEX.MilestonesEPG.ClearEPGInfo();
            //Launch Action bar on guide 
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Bar");
            }

            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            //Adding the Event details to the dictionary to verify in the EPG Info
            infoDictionary.Add("channel_logo", FvrtService_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", "1");
            infoDictionary.Add("evtname", "");
            infoDictionary.Add("evttime", "");
            infoDictionary.Add("progressbartime", "");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL,"Please check the above failures");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {

        public override void Execute()
        {
            StartStep();
            //Disable the favourite mode
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISABLE FAVOURITE MODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable the favourite mode");
            }
            //Launch GRID
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }
            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            //Depending on the GRID select down or left to the future event
            if (stateAfterEnetringPin == "ONE CHANNEL")
            {
                res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref  timeStamp);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to send IR select down");
                }
                res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref  timeStamp);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to send IR select down");
                }
            }
            else
            {
                res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPress: -1, timestamp: ref  timeStamp);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to send IR SELECT RIGHT");
                }
                res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPress: -1, timestamp: ref  timeStamp);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to send IR SELECT RIGHT");
                }
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
           
            PassStep();
        }
    }

    #endregion Step3
    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Bar");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", FvrtService_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", FvrtService_1.LCN);
            infoDictionary.Add("evtname", "");
            infoDictionary.Add("evttime", "");
            infoDictionary.Add("ADD REMINDER","");
            infoDictionary.Add("RECORD", "");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }

            PassStep();
        }
    }


    #endregion Step4

    #region Step5

    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.EA.UI.Utils.SendChannelAsIRSequence(Service_3.LCN);
            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Bar");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            res = CL.EA.EnterDeafultPIN(stateAfterEnetringPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Default pin");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Bar");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }

            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_3.ChannelLogo);
            infoDictionary.Add("chNum", Service_3.LCN);
            infoDictionary.Add("evtname", "");
            infoDictionary.Add("evttime", "");
            infoDictionary.Add("progressbartime", "");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }

            PassStep();
        }
    }


    #endregion Step5
    #region Step6

    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.EA.UI.Utils.SendChannelAsIRSequence(Service_2.LCN);

            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Bar");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_2.ChannelLogo);
            infoDictionary.Add("chNum", Service_2.LCN);
            infoDictionary.Add("evtname", "");
            infoDictionary.Add("evttime", "");
            infoDictionary.Add("RecordingStatus", "Ongoing Recording State");
            infoDictionary.Add("progressbartime", "");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }

            PassStep();
        }
    }


    #endregion Step6
    #region Step7

    private class Step7 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.EA.UI.Utils.SendChannelAsIRSequence(Service_2.LCN);

            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }

            if (stateAfterEnetringPin == "ONE CHANNEL")
            {
                res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref  timeStamp);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to send IR select down");
                }
            }
            else
            {
                res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPress: -1, timestamp: ref  timeStamp);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to send IR SELECT RIGHT");
                }
            }

            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Bar");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_2.ChannelLogo);
            infoDictionary.Add("chNum", Service_2.LCN);
            infoDictionary.Add("evtname", "");
            infoDictionary.Add("evttime", "");
            infoDictionary.Add("RecordingStatus", "Future Recording State");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }
        }
    }


    #endregion Step7
    #region Step8

    private class Step8 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.EA.UI.Utils.SendChannelAsIRSequence(FvrtService_2.LCN);

            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            CL.IEX.MilestonesEPG.ClearEPGInfo();
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Bar");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            infoDictionary.Clear();
            infoDictionary.Add("chName", FvrtService_2.Name);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", FvrtService_2.LCN);
            infoDictionary.Add("evtname", "");
            infoDictionary.Add("evttime", "");
            infoDictionary.Add("RecordingStatus", "Ongoing Recording State");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }
        }
    }


    #endregion Step8

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.STBSettings.UnsetAllFavChannels();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to UnsetAllFavChannels");
        }
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete all the records from Archive");
        }

    }

    #endregion PostExecute
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Verifies the Expected info with the obtained EPG info
        /// </summary>
        /// <returns>bool</returns>
        public bool VerifyEpgInfo(Dictionary<string,string> infoDictionary)
        {
            IEXGateway._IEXResult res;
            string obtainedValue = "";
            bool isPass = true;
            foreach (KeyValuePair<string, string> keyValue in infoDictionary)
            {
                switch (keyValue.Key)
                {
                    case "channel_logo":
                        string obtaionedChannelLogo = CL.EA.GetChannelLogo();
                        if (obtaionedChannelLogo != keyValue.Value)
                        {
                            LogCommentFailure(CL, "Channel logo fetched is different " + obtaionedChannelLogo + " from the expected" + keyValue.Value);
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Channel logo fetched " + obtaionedChannelLogo + " is same as expected" + keyValue.Value);
                        }
                        break;
                    case "evtname":
                    case "evttime":
                    case "progressbartime":
                        CL.IEX.MilestonesEPG.GetEPGInfo(keyValue.Key, out obtainedValue);
                        if (string.IsNullOrEmpty(obtainedValue))
                        {
                            LogCommentFailure(CL, keyValue.Key + " fetched from EPG is null");
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, keyValue.Key + " fetched from EPG is not null "+obtainedValue);
                        }
                        break;
                    case "ADD REMINDER":
                    case "RECORD":
                        res = CL.IEX.MilestonesEPG.SelectMenuItem(keyValue.Key);
                        if (!res.CommandSucceeded)
                        {
                            LogCommentFailure(CL, "Failed to highlight " + keyValue.Key + " in Action bar");
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Highlighted option" + keyValue.Key + " in Action bar");
                        }
                        break;
                    default:
                        CL.IEX.MilestonesEPG.GetEPGInfo(keyValue.Key, out obtainedValue);
                        if (keyValue.Value != obtainedValue)
                        {
                            LogCommentFailure(CL, keyValue.Key + " fetched from EPG is different from " + obtainedValue);
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, keyValue.Key + " fetched from EPG"+keyValue.Value+" is same as expected " + obtainedValue);
                        }

                        break;
                }
            }
            return isPass;
        }
    }
    #endregion
}
