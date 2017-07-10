/// <summary>
///  Script Name        : FT172_0010_RB_Playback.cs
///  Test Name          : FT172-0010-RB-Playback
///  TEST ID            : 74319
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 4th July, 2014
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;



[Test("FT172_0010")]
public class FT172_0010 : _Test
{

    [ThreadStatic]
    private static _Platform CL;


    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;

    static Helper helper = new Helper();
    private static Dictionary<string, string> infoDictionary = new Dictionary<string, string>();

    private static class Constants
    {
        public const int RB_Initial_Depth = 120;//In seconds
    }
    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition : Get values from Ini files & Sync ");
        this.AddStep(new Step1(), "Step 1: Press Pause on Service S1 and Wait for 2 min in RB in Favourite mode ");
        this.AddStep(new Step2(), "Step 2: Navigate to channel bar and verify the EPG info ");
        this.AddStep(new Step3(), "Step 3: Tune to service S3 press pause and wait for 2 min ");
        this.AddStep(new Step4(), "Step 4: Press play, Launch channel bar and verify the EPG info ");
        this.AddStep(new Step5(), "Step 5: Stop Playback from RB and come out of favourite mode ");
        this.AddStep(new Step6(), "Step 6: Rewind to Service S3, Launch channel bar and verify the EPG info ");
        this.AddStep(new Step7(), "Step 7: Rewind to the beginning of file to Service S1 ");
        this.AddStep(new Step8(), "Step 8: Launch Channel bar and verify the EPG info ");
        this.AddStep(new Step9(), "Step 9: Tune to service S2(Non-Favourite Service) press pause and wait for 2 min");
        this.AddStep(new Step10(), "Step 10: Press play, Launch channel bar and verify the EPG info ");

        CL = GetClient();
    }
    #endregion

    #region PreCondition
    private class PreCondition : _Step
    {

        // Enable favorite mode in action menu
        private static bool enableFavoriteMode()
        {
            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ENABLE FAVOURITE MODE");
                return true;
            }
            catch (Exception)
            {
                try
                {
                    CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR");
                    if (CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("DISABLE FAVOURITE MODE"))
                    {
                        CL.EA.UI.Utils.SendIR("RETOUR");
                        return true;
                    }
                }
                catch (Exception) { }
            }
            return false;
        }

        public override void Execute()
        {
            StartStep();

            //Get Values From xml File
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched from content xml " + Service_1.LCN);
            }
            //Get Values From xml File
            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched from content xml " + Service_2.LCN);
            }
            //Get Values From xml File
            Service_3 = CL.EA.GetServiceFromContentXML("IsRecordable=True;HasChannelLogo=False", "IsDefault=True;ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched from content xml " + Service_3.LCN);
            }

            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unset all the favourite channels");
            }
            // Set favorite channels

            res = CL.EA.STBSettings.SetFavoriteChannelNameList("" + Service_1.Name + ", " + Service_3.Name + "", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels" + Service_1.LCN + ", " + Service_3.LCN + " as favorites");
            }
            // enable favorite mode 
            bool result;
            result = enableFavoriteMode();
            if (false == result)
            {
                FailStep(CL, "Failed to enable favorite mode .");
            }
            PassStep();
        }
    }
    #endregion

    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live,ChannelNumber: "1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA 1");
            }
            res = CL.EA.FlushRB();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to flush the RB");
            }
            //press pause and wait for two minute 
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to live viewing");
            }
            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed: 0,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }

            CL.IEX.Wait(Constants.RB_Initial_Depth);
            // Step 2 start play back 
            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed: 1,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }

            // start channel bar
            CL.IEX.Wait(5);

            PassStep();
        }


    }
    #endregion

    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to clear the EPG info");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to channel bar ");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", "1");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Step 3 start play back 
            res = CL.EA.ChannelSurf(EnumSurfIn.Live,ChannelNumber: "2");

            res = CL.IEX.Wait(10);
            //Create play form RB (press pause, wait for timeshifting to be activated);
            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed: 0,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }

            CL.IEX.Wait(Constants.RB_Initial_Depth);
            PassStep();
        }
    }
    #endregion

    #region Step4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed: 1,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }


            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to clear the EPG info");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to channel bar ");
            }
            infoDictionary.Clear();
            infoDictionary.Add("chname", Service_3.Name);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", "2");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }

            PassStep();
        }
    }
    #endregion
    #region Step5
    private class Step5 : _Step
    {
        private static bool disableFavoriteMode()
        {
            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:DISABLE FAVOURITE MODE");
                return true;
            }
            catch (Exception)
            {
                try
                {
                    CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR");
                    if (CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("ENABLE FAVOURITE MODE"))
                    {
                        CL.EA.UI.Utils.SendIR("RETOUR");
                        return true;
                    }
                }
                catch (Exception) { }
            }
            return false;
        }
        public override void Execute()
        {
            StartStep();
            CL.IEX.Wait(10);
            res = CL.EA.PVR.StopPlayback(IsReviewBuffer:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Playback of RB");
            }
            // Desable Favorite mode
            bool result;
            result = disableFavoriteMode();
            if (result == false)
            {
                FailStep(CL, res, "Failed to disable favorite mode");
            }


            PassStep();
        }
    }
    #endregion
    #region Step6
    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Step 6 rewind for 30 second'Rew the RB till the BOF
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: -2, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Rewind RB to BOF");
            }
            CL.IEX.Wait(30);

            // start play back 
            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed: 1,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to clear the EPG info");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to channel bar ");
            }
            infoDictionary.Clear();
            infoDictionary.Add("chname", Service_3.Name);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", Service_3.LCN);
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }




            PassStep();
        }
    }
    #endregion
    #region Step7
    private class Step7 : _Step
    {
        public override void Execute()
        {
            StartStep();
            CL.IEX.Wait(10);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: -2, Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Rewind RB to BOF");
            }

            PassStep();
        }
    }
    #endregion
    #region Step8
    private class Step8 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to clear the EPG info");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to channel bar ");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", Service_1.LCN);
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }

            PassStep();
        }
    }
    #endregion
    #region Step9
    private class Step9 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }
            //Create play form RB (press pause, wait for timeshifting to be activated);
            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed: 0,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }

            CL.IEX.Wait(Constants.RB_Initial_Depth);
            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed: 1,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }

            PassStep();
        }
    }
    #endregion
    #region Step10
    private class Step10 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to clear the EPG info");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to channel bar ");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_2.ChannelLogo);
            infoDictionary.Add("chNum", Service_2.LCN);
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }


            PassStep();
        }
    }
    #endregion
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Verifies the Expected info with the obtained EPG info
        /// </summary>
        /// <returns>bool</returns>
        public bool VerifyEpgInfo(Dictionary<string, string> infoDictionary)
        {
            string obtainedValue = "";
            bool isPass = true;
            foreach (KeyValuePair<string, string> keyValue in infoDictionary)
            {
                switch (keyValue.Key)
                {
                    case "channel_logo":
                        string obtainedChannelLogo = CL.EA.GetChannelLogo();
                        if (obtainedChannelLogo != keyValue.Value)
                        {
                            LogCommentFailure(CL, "Channel logo fetched is different from the expected");
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Channel logo fetched is " + obtainedChannelLogo + " same as expected " + keyValue.Value);
                        }
                        break;
                    default:
                        CL.IEX.MilestonesEPG.GetEPGInfo(keyValue.Key, out obtainedValue);
                        if (keyValue.Value != obtainedValue)
                        {
                            LogCommentFailure(CL, "Obtained info " + obtainedValue + " is different from expected " + keyValue.Value);
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Obtained info " + obtainedValue + " is same as expected " + keyValue.Value);
                        }
                        break;
                }
            }
            return isPass;
        }
    }
    #endregion
}



