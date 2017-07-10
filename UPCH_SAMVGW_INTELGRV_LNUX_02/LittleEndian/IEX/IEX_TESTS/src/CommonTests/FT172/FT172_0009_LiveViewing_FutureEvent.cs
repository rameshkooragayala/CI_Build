/// <summary>
///  Script Name        : FT172_0009_LiveViewing_FutureEvent.cs
///  Test Name          : FT172-0009-Live-viewing-and-Future-event
///  TEST ID            : 74543
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 25th July, 2014
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;



[Test("FT172_0009")]
public class FT172_0009 : _Test
{

    [ThreadStatic]
    private static _Platform CL;

    
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    static string expFavModeExitMsg;
    static string timeStamp;

    static Helper helper = new Helper();
    private static Dictionary<string, string> infoDictionary = new Dictionary<string, string>();

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition:Get values from Ini files");
        this.AddStep(new Step1(), "Step 1: Go to service 1 and Verify EPF info in Favourite mode");
        this.AddStep(new Step2(), "Step 2: surf to next event and Verify EPF info in Favourite mode");
        this.AddStep(new Step3(), "Step 3: Go to service 2 and Verify EPF info in Favourite mode");
        this.AddStep(new Step4(), "Step 4: surf to next event and Verify EPF info in Favourite mode");
        this.AddStep(new Step5(), "Step 5: Go to service 3 LCN and Verify that the Favourite mode is deactivated");
        this.AddStep(new Step6(), "Step 6: Launch channel bar and verify the EPG info");
        this.AddStep(new Step7(), "Step 7: Navigate to Channel bar next and verify the EPG info");
        this.AddStep(new Step8(), "Step 8: Tune to Service 1 in Non-Favourite mode");
        this.AddStep(new Step9(), "Step 9: Launch channel bar and verify the EPG info in Non-Favourite mode");
        this.AddStep(new Step10(), "Step 10: Navigate to channel bar Next and verify the EPG info in Non-Favourite mode");
        //Get Client Platform
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
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True","ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            else
            {
                LogCommentInfo(CL,"Service number fetched from Content xml "+Service_1.LCN);
            }
            //Get Values From xml File
            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=False", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service number fetched from Content xml " + Service_2.LCN);
            }
            //Get Values From xml File
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video","ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service number fetched from Content xml " + Service_3.LCN);
            }
         
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to UnsetAllFavChannels");
            }
            // Set favorite channels
            res = CL.EA.STBSettings.SetFavoriteChannelNameList("" + Service_1.Name + ", " + Service_2.Name + "", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels " + Service_1.LCN + ", " + Service_2.LCN + " as favorites");
            }

            expFavModeExitMsg = CL.EA.UI.Utils.GetValueFromDictionary("DIC_DEACTIVATE_FAVOURITE_MODE");
            if (string.IsNullOrEmpty(expFavModeExitMsg))
            {
                FailStep(CL, "DIC_DEACTIVATE_FAVOURITE_MODE is not found in dictionary");
            }
            //enable favorite mode 
            enableFavoriteMode();
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

            //Step 1  tune to service 1 
             CL.EA.ChannelSurf(EnumSurfIn.Live, "1");

            // start channel bar 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");

            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_1.ChannelLogo);
            infoDictionary.Add("IsFavourite","True");
            infoDictionary.Add("chnum","1");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }

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
            //go to next event on the last channel 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPress: -1, timestamp: ref  timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to Channel bar next");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chnum", "1");
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
            //Step 2  tune to service 3 
             CL.EA.ChannelSurf(EnumSurfIn.Live, "2");

            // start channel bar 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");
            infoDictionary.Clear();
            infoDictionary.Add("chname", Service_2.Name);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chnum", "2");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }
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
            //go to next event on the last channel 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPress: -1, timestamp: ref  timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Channel bar next");
            }
            infoDictionary.Clear();
            infoDictionary.Add("chname", Service_2.Name);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chnum", "2");
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
        public override void Execute()
        {
            StartStep();
            //Step 2  tune to service 4 
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }
            // Check favorite mode exit message
            string favModeExitMsg = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("Deactivating_Message", ref favModeExitMsg))
            {
                FailStep(CL, "Failed to check favorite mode exit message: Unable to read 'Deactivating_Message' milestone");
            }
            if (favModeExitMsg != expFavModeExitMsg)
            {
                FailStep(CL, "Wrong message displayed. Read: " + favModeExitMsg + ", Expected: " + expFavModeExitMsg);
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
            // start channel bar 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");

            infoDictionary.Clear();
            infoDictionary.Add("chname", Service_3.Name);
            infoDictionary.Add("chnum", Service_3.LCN);
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

            //go to next event on the last channel 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPress: -1, timestamp: ref  timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Channel bar next");
            }
            infoDictionary.Clear();
            infoDictionary.Add("chname", Service_3.Name);
            infoDictionary.Add("chnum", Service_3.LCN);
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+Service_1.LCN);
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
            // start channel bar 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");

            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chnum", Service_1.LCN);
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
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
            // start channel bar 
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:CHANNEL BAR");
            res = CL.IEX.SendIRCommand("SELECT_RIGHT", timeToPress: -1, timestamp: ref  timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Channel bar next");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", Service_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chnum", Service_1.LCN);
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
                            LogCommentFailure(CL, "Channel logo fetched is different"+obtainedChannelLogo+" from the expected"+keyValue.Value);
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
    #region PostExecute

    public override void PostExecute()
    {
        //Unset all the current favourites channel 
        CL.EA.STBSettings.UnsetAllFavChannels();
    }

    #endregion PostExecute
}

