/// <summary>
///  Script Name : FT172_0005_DCA_to_Fav_and_Non_fav_services_fav_mode.cs
///  Test Name   : FT172_0005_DCA_to_Fav_and_Non_fav_services_fav_mode
///  TEST ID     : 74267
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 18/06/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT172_0005")]
public class FT172_0005 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static List<string> favoriteChannelList;
    static Service nonFavChannel;
    static string lowestChannelNumber;
    static string highestChannelNumber;
    static string expFavModeExitMsg;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Set favorite channels and enable favorite mode";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to favorite channel 5";
    private const string STEP2_DESCRIPTION = "Step 2: Tune to favorite channel 1";
    private const string STEP3_DESCRIPTION = "Step 3: Tune to a non-favorite channel by entering its logical channel number";
    private const string STEP4_DESCRIPTION = "Step 4: Return to favorite mode and check the STB tunes to the 1st favorite channel";
    private const string STEP5_DESCRIPTION = "Step 5: Tune to a non-existing channel using a LCN lower than the smallest LCN in channel line-up";
    private const string STEP6_DESCRIPTION = "Step 6: Return to favorite mode and check the STB tunes to the 1st favorite channel";
    private const string STEP7_DESCRIPTION = "Step 7: Tune to a non-existing channel using a LCN greater than the highest LCN in channel line-up";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);
        this.AddStep(new Step6(), STEP6_DESCRIPTION);
        this.AddStep(new Step7(), STEP7_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion


    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Get list of channel to set as favorite (S1, S2, S3, S4, S5, S6, S7)
            string _favoriteChannelList = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FAVORITE_CHANNEL_LIST");
            if (string.IsNullOrEmpty(_favoriteChannelList))
            {
                FailStep(CL, "FAVORITE_CHANNEL_LIST is empty in Test.ini in section TEST_PARAMS");
            }
            favoriteChannelList = new List<string>(_favoriteChannelList.Split(new char[] { ',' }));
              
            // Fetch a non-favorite channel            
            nonFavChannel = CL.EA.GetServiceFromContentXML("", "Name=" + String.Join(",", favoriteChannelList));
            if (nonFavChannel == null)
            {
                FailStep(CL, "Failed to fetch a service different from the favorite channels");
            }

            lowestChannelNumber = CL.EA.GetValue("Lowest_Service_Number");
            if (string.IsNullOrEmpty(lowestChannelNumber))
            {
                FailStep(CL, "Lowest_Service_Number is empty in Channels.ini");
            }

            highestChannelNumber = CL.EA.GetValue("Highest_Service_Number");
            if (string.IsNullOrEmpty(highestChannelNumber))
            {
                FailStep(CL, "Highest_Service_Number is empty in Channels.ini");
            }

            expFavModeExitMsg = CL.EA.UI.Utils.GetValueFromDictionary("DIC_DEACTIVATE_FAVOURITE_MODE");
            if (string.IsNullOrEmpty(expFavModeExitMsg))
            {
                FailStep(CL, "DIC_DEACTIVATE_FAVOURITE_MODE is not found in dictionary");
            }

            // Set favorite channels
            res = CL.EA.STBSettings.SetFavoriteChannelNameList(_favoriteChannelList, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels " + _favoriteChannelList + " as favorites");
            }

            // Set favorite mode            
            if (!enableFavoriteMode())
            {
                FailStep(CL, "Failed to set favorite mode");
            }

            PassStep();
        }
    }
    #endregion

    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Tune to favorite channel 5
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "5");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to channel 5");
            }

            // Check channel name is equal to S5 in channel bar
            string channelName = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName))
            {
                FailStep(CL, "Failed to check channel name: Unable to read 'chname' milestone");
            }
            if (channelName != favoriteChannelList[4])
            {
                FailStep(CL, "Wrong channel name. Read: " + channelName + ", Expected: " + favoriteChannelList[4]);
            }

            PassStep();
        }
    }
    #endregion

    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Tune to favorite channel 1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to channel 1");
            }

            // Check channel name is equal to S1 in channel bar
            string channelName = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName))
            {
                FailStep(CL, "Failed to check channel name: Unable to read 'chname' milestone");
            }
            if (channelName != favoriteChannelList[0])
            {
                FailStep(CL, "Wrong channel name. Read: " + channelName + ", Expected: " + favoriteChannelList[0]);
            }

            PassStep();
        }
    }
    #endregion

    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.EA.UI.Utils.ClearEPGInfo();

            // Tune to a non-favorite channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, nonFavChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to channel " + nonFavChannel.LCN);
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

            // Check channel name
            string channelName = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName))
            {
                FailStep(CL, "Failed to check channel name: Unable to read 'chname' milestone");
            }
            if (channelName != nonFavChannel.Name)
            {
                FailStep(CL, "Wrong channel name. Read: " + channelName + ", Expected: " + nonFavChannel.Name);
            }

            PassStep();
        }
    }
    #endregion

    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Return to favorite mode
            if (!enableFavoriteMode())
            {
                FailStep(CL, "Failed to set favorite mode");
            }

            // Check channel number
            string channelNumber = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chNum", ref channelNumber))
            {
                FailStep(CL, "Failed to check channel number: Unable to read 'chNum' milestone");
            }
            if (channelNumber != "1")
            {
                FailStep(CL, "Wrong channel name. Read: " + channelNumber + ", Expected: 1");
            }

            PassStep();
        }
    }
    #endregion

    #region Step5
    [Step(5, STEP5_DESCRIPTION)]
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.EA.UI.Utils.ClearEPGInfo();

            // Enter a LCN lower than the lowest LCN in channel line-up
            CL.EA.ChannelSurf(EnumSurfIn.Live, (Int32.Parse(lowestChannelNumber)-1).ToString());

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

            // Read channel number after tuning
            string chNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out chNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check channel number");
            }

            // Verify that the current channel is the nearest higher channel
            if (chNumber != lowestChannelNumber)
            {
                FailStep(CL, "Wrong channel number. Read: " + chNumber + ", Expected: " + lowestChannelNumber);
            }

            PassStep();
        }
    }
    #endregion

    #region Step6
    [Step(6, STEP6_DESCRIPTION)]
    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Return to favorite mode
            if (!enableFavoriteMode())
            {
                FailStep(CL, "Failed to set favorite mode");
            }

            // Check channel number
            string channelNumber = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chNum", ref channelNumber))
            {
                FailStep(CL, "Failed to check channel number: Unable to read 'chNum' milestone");
            }
            if (channelNumber != "1")
            {
                FailStep(CL, "Wrong channel name. Read: " + channelNumber + ", Expected: 1");
            }

            PassStep();
        }
    }
    #endregion

    #region Step7
    [Step(7, STEP7_DESCRIPTION)]
    private class Step7 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.EA.UI.Utils.ClearEPGInfo();

            // Enter a LCN greater than the highest LCN in channel line-up
            CL.EA.ChannelSurf(EnumSurfIn.Live, (Int32.Parse(highestChannelNumber) + 1).ToString());

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

            // Read channel number after tuning
            string chNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out chNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check channel number");
            }

            // Verify that the current channel is the nearest higher channel
            if (chNumber != highestChannelNumber)
            {
                FailStep(CL, "Wrong channel number. Read: " + chNumber + ", Expected: " + highestChannelNumber);
            }

            PassStep();
        }
    }
    #endregion

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

    #endregion
}


