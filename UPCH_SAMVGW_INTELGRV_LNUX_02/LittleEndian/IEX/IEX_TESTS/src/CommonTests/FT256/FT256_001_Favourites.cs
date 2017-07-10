/// <summary>
///  Script Name : 
///  Test Name   : 
///  TEST ID     : 
///  QC Version  :
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Ganpat S

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT256-favourites-services-numbering")]
public class FT256_0001 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static Helper helper = new Helper();
    private static string obtainedLogicalChName = "";
    private static string firstFocusedChName = "";
    private static string channelStatus = "";
    private static string reoredrChName = "";
    private static Boolean isSetAsFavorite;
    private static int numOfFailure = 0;
    //Variables which are used in different steps

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Unset all the favourites";
    private const string STEP1_DESCRIPTION = "Step 1: Set All the services as Favourites";
    private const string STEP2_DESCRIPTION = "Step 2: Unset few services from favourites";
    private const string STEP3_DESCRIPTION = "Step 3: Reorder two services of favourites and unset all services from favourites";


    private static class Constants
    {

        public const string currentid = "2"; //id of Service 2 before reordering. 
        public const string reorderid = "1"; //id of Service 2 after reordering. 
    }
    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        for (int j = 0; j < 120; j++)
        {
            this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
            this.AddStep(new Step1(), STEP1_DESCRIPTION);
            this.AddStep(new Step2(), STEP2_DESCRIPTION);
            this.AddStep(new Step3(), STEP3_DESCRIPTION);
        }
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

			obtainedLogicalChName = "";
			
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to UnsetAllFavChannels");
                numOfFailure++;
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

            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:SET FAVOURITES BY SETTINGS");

            CL.EA.UI.Utils.GetEpgInfo("chName", ref firstFocusedChName);

            while (obtainedLogicalChName != firstFocusedChName)
            {
                CL.EA.UI.Utils.SendIR("SELECT");
                CL.EA.UI.Utils.GetEpgInfo("key", ref channelStatus);

                isSetAsFavorite = String.Equals(channelStatus, "LockedChannel");

                if (!(isSetAsFavorite))
                {
                    LogCommentFailure(CL, "Failed to set Fav Channel");
                    numOfFailure++;
                }

                CL.EA.UI.Utils.SendIR("SELECT_DOWN");

                CL.EA.UI.Utils.GetEpgInfo("chName", ref obtainedLogicalChName);
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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SET FAVOURITES BY SETTINGS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Navigate Fav Settings");
                numOfFailure++;
            }

            for (int i = 0; i < 5; i++)
            {

                CL.EA.UI.Utils.SendIR("SELECT");
                CL.EA.UI.Utils.GetEpgInfo("key", ref channelStatus);

                isSetAsFavorite = String.Equals(channelStatus, "UnlockedChannel");

                if (!(isSetAsFavorite))
                {
                    LogCommentFailure(CL, "Failed to unset Fav Channel");
                    numOfFailure++;
                }

                CL.EA.UI.Utils.SendIR("SELECT_DOWN");

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

            //Reordering Serviec 2 to position 1
            if (!helper.ReorderFavourites())
            {
                LogCommentFailure(CL, "Failed to reorder the fav service");
                numOfFailure++;
            }

            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to UnsetAllFavChannels");
                numOfFailure++;
            }

            if (numOfFailure > 20)
            {

                FailStep(CL, "Failed to set/unset Favourites more than 20 times");
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
                    LogCommentFailure(CL, "Failed to Select RENUMBER FAVOURITES");
					numOfFailure++;
                }



                //get the current channel is

                CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);

                // select id to be swapped

                int i = 0;

                while (ChannelID != Constants.currentid || i == 15)
                {
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_NEXT_SELECTION"));
                    CL.IEX.Wait(4);
					CL.EA.UI.Utils.GetEpgInfo("title", ref reoredrChName);
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

                    if (ChannelID.Trim() == Constants.reorderid && ChannelName.Trim() == reoredrChName)
                    {
                        CL.IEX.LogComment("Reorder Favourites Passed");
                    }
                    else
                    {
                        numOfFailure++;
                    }

                }
                else
                {
                    numOfFailure++;
                }

                return true;

            }
            catch (Exception ex)
            {

                numOfFailure++;
                return false;
            }
        }
        #endregion
    }
}