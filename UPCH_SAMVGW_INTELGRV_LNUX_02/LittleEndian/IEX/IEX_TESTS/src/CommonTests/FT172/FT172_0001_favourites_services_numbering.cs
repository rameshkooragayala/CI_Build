/// <summary>
///  Script Name : FT172_0001_favourites_services_numbering.cs
///  Test Name   : FT172-0001-favourites-services-numbering.cs
///  TEST ID     : 74290
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar,Madhu R
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT172-0001-favourites-services-numbering")]
public class FT172_0001 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    private static string FavChannelList = string.Empty; //Channel to be set as Fav
    private static Service favService1;
    private static Service favService2;
    private static string ConfirmedFavChannelList = string.Empty;
    private static string ConfirmedFavouriteChannelNumber = string.Empty;
    private static string obtainedFavouriteChNum = "";
    private static string obtainedLogicalChNum = "";
    private static string obtainedFavouriteChNumOnGrid = "";
    //Variables which are used in different steps

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch list of Service from Test.INI file & Set the list of channels as Favouites through Navigation SETTING";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to service s1 and Logical channel Number displayed";
    private const string STEP2_DESCRIPTION = "Step 2: Enable the Favourite Mode and check the Favourite channel Number displayed or not on the Channel Bar & Zap to channel s2,FAV channel Number displayed Channel Bar ";
    private const string STEP3_DESCRIPTION = "Step 3: Navigate to TV Guide, Fast Channel List and check for Favourite channel Number";


    private static class Constants
    {
        public const int totalDurationInMin = 10;
    }
    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

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

            //Get List of Fav Channel from TEST ini File
            FavChannelList = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FavChannelList");

            if (string.IsNullOrEmpty(FavChannelList))
            {
                FailStep(CL, res, "Unable to fetch the ChannelList from test ini file");
            }

            string[] FavouriteChannelList = FavChannelList.Split(',');

            //Get Values From xml File
            favService1 = CL.EA.GetServiceFromContentXML("LCN=" + FavouriteChannelList[0].Trim());
            if (favService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }

            favService2 = CL.EA.GetServiceFromContentXML("LCN=" + FavouriteChannelList[1].Trim(), null);
            if (favService2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }

            //Unset all the current favourites channel 
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove all Favorite channel");

            }

            //Set the channel as Favourite List of fav channel
            res = CL.EA.STBSettings.SetFavoriteChannelNameList("" + favService1.Name + ", " + favService2.Name + "", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set SetFavoriteChannelNumList");
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

            //Tune to First service s1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, favService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + favService1.LCN);
            }

            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedLogicalChNum);
            if (obtainedLogicalChNum == null)
            {
                FailStep(CL, res, "obtained Logical channel Number on First service is Null");
            }

            if (obtainedLogicalChNum == favService1.LCN)
            {
                LogCommentImportant(CL, "obtained Logical channel number :" + obtainedLogicalChNum);

            }
            else
            {
                FailStep(CL, "Received Different Logical ChNum: (" + obtainedLogicalChNum + ") Than Expected ChNum:(" + favService1.LCN + ") on First service");

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

            //if DISABLE FAVOURITE MODE is present on the ACTION BAR,then no need to navigate to "STATE:ENABLE FAVOURITE MODE"
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
            if (res.CommandSucceeded)
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem("DISABLE FAVOURITE MODE");
                if (res.CommandSucceeded)
                {
                    CL.IEX.LogComment("DISABLE FAVOURITE MODE is present on the ACTION BAR");
                }
                else
                {
                    res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ENABLE FAVOURITE MODE");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Failed to Set FAVOURITE mode");
                    }
                }
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Launch Channel Bar on  First service Channel
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to CHANNEL BAR");
            }

            //Get the Favourite Channel Number on First Channel 
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedFavouriteChNum);
            if (obtainedFavouriteChNum == null)
            {
                FailStep(CL, res, "obtained Favourite channel Number is Null");
            }
            else
            {
                LogCommentInfo(CL, "Obtained Favourite on  first channel service is:" + obtainedFavouriteChNum);

            }
            if ((obtainedFavouriteChNum != "1"))
            {
                FailStep(CL, "Received Different Favourite ChNum: (" + obtainedFavouriteChNum + ") Than Expected ChNum: 1 on service s1");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Zap to SECOND SERVICE through channel bar
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Channel Lineup from channel Bar");
            }

            //Get the Favourite Channel Number on SecondChannel
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedFavouriteChNum);
            LogCommentInfo(CL, "Obtained chNum on Second Channel" + obtainedFavouriteChNum);
            if (obtainedFavouriteChNum == null)
            {
                FailStep(CL, res, "obtained Favourite channel Number on SecondChannelNumber is Null");
            }

            if ((obtainedFavouriteChNum != "2"))
            {
                FailStep(CL, "Received Different Favourite ChNum: (" + obtainedFavouriteChNum + ") Than Expected ChNum: 2 on s2");
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

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate To TV GUIDE
            LogCommentInfo(CL, "Navigate To TV GUIDE");
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }
			
            CL.IEX.Wait(10);
			
            //Obtaining focussed chNum on Grid State
            LogCommentInfo(CL, "Obtaining chNum on Grid State");
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out obtainedFavouriteChNumOnGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }

            if (obtainedFavouriteChNumOnGrid != "2")
            {
                FailStep(CL, "Received Different Favourite ChNum:(" + obtainedFavouriteChNumOnGrid + ") Than Expected ChNum: 2 on guide");
            }

      
            string NextChannelNum = "";
            res = CL.IEX.SendIRCommand("SELECT_UP", -1, ref NextChannelNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR command Select_UP");
            }

            CL.IEX.Wait(10);
            //Obtaining the next chNum on Grid State
            LogCommentInfo(CL, "Obtaining next chNum on Grid State");
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out obtainedFavouriteChNumOnGrid);

            if (obtainedFavouriteChNumOnGrid != "1")
            {
                FailStep(CL, "Received Different Favourite ChNum: (" + obtainedFavouriteChNumOnGrid + ") Than Expected ChNum:1 on guide");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Launch Fast channel List through the 'channel bar'
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Launch Channel line up
            CL.EA.UI.ChannelLineup.Navigate();

            //Verify CHANNEL LINEUP state
            if (!CL.EA.UI.Utils.VerifyState("CHANNEL LINEUP", 10))
            {
                FailStep(CL, res, "Failed to verify Channel Lineup state");
            }

            //Obtaining chNum on Fast Channel List
            LogCommentInfo(CL, "Obtaining chNum on Fast Channel List");

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out obtainedFavouriteChNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }

            //if ConfirmedFavouriteChannelNumber should contains the chNum on Fast Channel List
            if (obtainedFavouriteChNum != "2" && obtainedFavouriteChNum!="1")
            {
                FailStep(CL, "Received Different Favourite ChNum: (" + obtainedFavouriteChNum + ") Than Expected ChNum:1 and 2 on Fast Channel List");
            }

            res = CL.IEX.SendIRCommand("SELECT_UP", -1, ref NextChannelNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR command Select_UP");
            }
            CL.IEX.Wait(10);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out obtainedFavouriteChNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }

            //if ConfirmedFavouriteChannelNumber should contains the chNum on Fast Channel List
            if (obtainedFavouriteChNum != "2" && obtainedFavouriteChNum!="1")
            {
                FailStep(CL, "Received Different Favourite ChNum: (" + obtainedFavouriteChNum + ") Than Expected chNum:1 and 2 on Fast Channel List");
            }

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

        //Restore default settings
        IEXGateway._IEXResult res;

        CL.IEX.Wait(5);

        res = CL.EA.STBSettings.UnsetAllFavChannels();
        if (!res.CommandSucceeded)
        {
            LogCommentInfo(CL, "Failed to UnsetAllFavChannels");
        }
        CL.IEX.LogComment("Removed Favourite List");
    }
    #endregion
}