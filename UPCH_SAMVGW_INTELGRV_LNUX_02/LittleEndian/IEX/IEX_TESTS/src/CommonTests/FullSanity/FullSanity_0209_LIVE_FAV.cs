/// <summary>
///  Script Name :FullSanity_0209_LIVE_FAV.cs
///  Test Name   : FullSanity_0209_Live_FAV
///  TEST ID     : 74550
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FullSanity_0209_Live_FAV")]
public class FullSanity_0209_Live_FAV : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static Helper helper = new Helper();

    //Channels used by the test
    private static string FavChannelList = string.Empty; //Channel to be set as Fav
    private static Service favService1;
    private static Service favService2;
    static string channelName = "";
    static string ChannelID = "";
    private static string obtainedFavouriteChNum = "";
    
   
    //Variables which are used in different steps

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch list of Services from Test.INI file";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to service s1 & s2 and set as Favourite channel through Action BAR & Enable the Favourite Mode";
    private const string STEP2_DESCRIPTION = "Step 2: Enable the Favourite Mode and check the Favourite channel Number displayed or not on the Channel Bar";
    private const string STEP3_DESCRIPTION = "Step 3: Change the Mode from Favourite to Regular Mode and Channel Line up should contains the all the favourites channel.";


    private static class Constants
    {
        public const int totalDurationInMin = 10;
        public const string currentid = "2"; //id of Service 2 before reordering. 
        public const string reorderid = "1"; //id of Service 2 after reordering. 

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

            //Tune to service s1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, favService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + favService2.LCN);
            }

            //Set s1 as Favourite through Action BAR
            res = CL.EA.STBSettings.SetFavoriteChannelNumList("" + favService1.LCN + " ", EnumFavouriteIn.ActionBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set SetFavoriteChannelNumList");
            }

            CL.IEX.Wait(5);

            String IsFavourite = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("IsFavourite", out IsFavourite);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail on Future event from Action Menu");
            }

            if (IsFavourite == "True")
            {
                LogComment(CL, "Service " + favService1.LCN + "is set as favorite");

            }
            else
            {
                FailStep(CL, res, "Failed to display Favorite channel indication");
            }


            //Tune to service s2
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, favService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + favService2.LCN);
            }

            //Set s2 as Favourite through Action BAR
            res = CL.EA.STBSettings.SetFavoriteChannelNumList("" + favService2.LCN + " ", EnumFavouriteIn.ActionBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set SetFavoriteChannelNumList");
            }

            CL.IEX.Wait(5);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("IsFavourite", out IsFavourite);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display thumbnail on Future event from Action Menu");
            }

            if (IsFavourite == "True")
            {
                LogComment(CL, "Service " + favService2.LCN + "is set as favorite");

            }
            else
            {
                FailStep(CL, res, "Failed to display Favorite channel indication");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, favService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + favService2.LCN);
            }


           //Enable the Fav Mode : if DISABLE FAVOURITE MODE is present on the ACTION BAR,then no need to navigate to "STATE:ENABLE FAVOURITE MODE"
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
            
            CL.IEX.Wait(5);

            CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName);

            // checking the current service is same as Service 2

            if (channelName != favService2.Name)
            {
                FailStep(CL, "Failed to Verify surf Service2");
            }


            //Reordering Service 2 to position 1
            if (!helper.ReorderFavourites())
            {
                FailStep(CL, "Failed to Reorder Services");
            }



            res = CL.EA.ReturnToLiveViewing(false);

            if (!res.CommandSucceeded)
            {

                CL.IEX.LogComment("Fail to return Live");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + favService2.LCN);
            }

             //Disable the Fav Mode : if ENABLE FAVOURITE MODE is present on the ACTION BAR,then no need to navigate to "STATE:DISABLE FAVOURITE MODE"
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
            if (res.CommandSucceeded)
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem("ENABLE FAVOURITE MODE");
                if (res.CommandSucceeded)
                {
                    CL.IEX.LogComment("ENABLE FAVOURITE MODE is present on the ACTION BAR");
                }
                else
                {
                    res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISABLE FAVOURITE MODE");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Failed to Disable the FAVOURITE mode");
                    }
                }
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
            LogCommentInfo(CL, "Obtaining ChName on Fast Channel List");
           
            CL.IEX.Wait(10);

            string obtainedFavoriteChannelName = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("FavoriteChannel", out obtainedFavoriteChannelName);
          
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel Name in Fast channel Line up");
            }
            else
            {
                if (!((obtainedFavoriteChannelName == favService1.Name) || (obtainedFavoriteChannelName == favService2.Name)))
                {

                    FailStep(CL, "Received Different Favorite ChannelName: (" + obtainedFavoriteChannelName + ") Than Expected Channel Name on Fast Channel List");
                }
                else
                {
                    LogCommentInfo(CL, "Obtained Favourite channel in Fast Channel Line Up is:" + obtainedFavoriteChannelName);
                }

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
    public class Helper : _Step
    {

        static string ChannelID = "";
        static string ChannelName = "";

        public override void Execute() { }

        /// <summary>
        /// Tunes to Channel with subtitles and verifies the defualt subtitle language in Action bar.
        /// </summary>
        /// <returns>bool</returns>


        #region Reorder Favourites

        public bool ReorderFavourites()
        {



            string EpgText = "";

            try
            {
                // Navigate to Renumber Favorites
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:RENUMBER FAVOURITES");

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Select RENUMBER FAVOURITES");
                }



                //get the current channel is

                CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);

                // select id to be swapped

                int i = 0;

                while (ChannelID != Constants.currentid || i == 15)
                {
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_NEXT_SELECTION"));
                    CL.IEX.Wait(4);
                    CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                    i++;
                }

                // moving to new positopn


                CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_SELECT"));

                while (ChannelID != Constants.reorderid || i == 15)
                {
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_PREV_SELECTION"));
                    CL.IEX.Wait(4);
                    CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                    i++;
                }

                //moving to confirm
                string confirmTitle = "";
                CL.IEX.Wait(1);

                CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "CONFIRM_FAVOURITE"));

                CL.IEX.Wait(2);

                CL.EA.UI.Utils.GetEpgInfo("title", ref confirmTitle);

                if (confirmTitle.ToUpper() == "DEFAULT ORDER")
                {
                    CL.EA.UI.Utils.SendIR("BACK");
                   
                }
                else
                {
                    //send the "SELECT" key to confirm reorder Favourist List
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_SELECT"));
          
                }


                //Check whether it reached RENUMBER FAVOURITE Menu
                CL.IEX.Wait(1);
                CL.EA.UI.Utils.GetEpgInfo("title", ref EpgText);

                if (EpgText.Trim() == "RENUMBER FAVOURITES")
                {
                    // Confirm reorder by checking id is 1 and chname is service 2 channel name
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_SELECT"));

                    CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                    CL.EA.UI.Utils.GetEpgInfo("title", ref ChannelName);

                    if (ChannelID.Trim() == Constants.reorderid && ChannelName.Trim() == favService2.Name)
                    {
                        CL.IEX.LogComment("Reorder Favourites Passed");
                    }
                    else
                    {
                        FailStep(CL, "Fail to CONFIRM Reorder Favourite");
                    }

                }
                else
                {
                    FailStep(CL, "Fail to CONFIRM Reorder Favourite");
                }

                return true;

            }
            catch (Exception ex)
            {

                FailStep(CL, "Failed to  Reorder Favourites." + ex.Message);
                return false;
            }
        }

        #endregion

        #region Enable Favourite Mode

        // Enable favorite mode in action menu
        public bool enableFavoriteMode()
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
}