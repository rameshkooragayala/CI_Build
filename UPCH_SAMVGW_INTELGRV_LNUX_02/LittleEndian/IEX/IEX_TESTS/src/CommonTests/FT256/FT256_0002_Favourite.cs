/// <summary>
///  Script Name : FT256_0002_Favourite
///  Test Name   : FT256_0002_Favourite
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
/// Created By : Ganpat Singh
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using System.Collections;

[Test("FT256_0002_Favourite")]
public class FT256_0002_Favourite : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static Helper helper = new Helper();

    //Shared members between steps
    private static string FavChannelList = string.Empty; //Channel to be set as Fav
    private static string focusedTitle = "";
    private static string expectedTitle = "";
    private static Service favService1;
    private static Service favService2;
    private static Service favService3;
    static string channelName = "";
    static string ChannelID = "";
    static string channelNameBeforeReorder = "";
    static string ChannelIDBeforeReorder = "";
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Fav Channel Numbers From ini File & Set Channels as Favourite";
    private const string STEP1_DESCRIPTION = "Step 1:Verify that CLEAR LIST option is not available if no fvrt is set and set few services as fvrt ";
    private const string STEP2_DESCRIPTION = "Step 2:Verify Reordering and default order behaviour ";

     private static class Constants
    {
        
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
     
            //Get Values From xml File
            favService1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True","ParentalRating=High");
            if (favService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }

            favService2 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True","ParentalRating=High;LCN="+favService1.LCN+"");
            if (favService2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }

            favService3 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True;IsEITAvailable=True", "LCN=" + favService1.LCN + "," + favService2.LCN + ";ParentalRating=High");
            if (favService3 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }

            //EA Unset all favourites
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove all Favorite channels");

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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SET FAVOURITES BY SETTINGS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate SetFavoriteChannelNumList");
            }

            CL.EA.UI.Utils.GetEpgInfo("title", ref expectedTitle);
            if (focusedTitle == null)
            {
                FailStep(CL, "Failed to get focused title on set fvrt screen");
            }


            CL.EA.UI.Utils.SendIR("SELECT_LEFT");

            CL.EA.UI.Utils.GetEpgInfo("title", ref focusedTitle);
            if (focusedTitle == null)
            {
                FailStep(CL, "Failed to get focused title on set fvrt screen");
            }

            if (focusedTitle.Contains("CLEAR LIST"))
            {
                FailStep(CL, "Clear List option is present, even after no service is set as fvrt");
            }

            //Set Service 1 , Service 2 & Service 3 as Favourite Channels

            res = CL.EA.STBSettings.SetFavoriteChannelNumList(favService1.LCN + "," + favService2.LCN + "," + favService3.LCN, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set SetFavoriteChannelNumList");
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

            // Surfing P+

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, favService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to surf Service2");
            }

            CL.IEX.Wait(5);

            CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName);

            // checking the current service is same as Service 2
            if (channelName != favService2.Name)
            {
                FailStep(CL, "Failed to Verify surf Service2");
            }

            // Navigate to Renumber Favorites
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:RENUMBER FAVOURITES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Select RENUMBER FAVOURITES");
            }


            CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelIDBeforeReorder);
            CL.EA.UI.Utils.GetEpgInfo("title", ref channelNameBeforeReorder);


            //Reordering Serviec 2 to position 1
            if (!helper.ReorderFavourites())
            {
                FailStep(CL, "Failed to Reorder Services");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:RENUMBER FAVOURITES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Select RENUMBER FAVOURITES");
            }

            CL.EA.UI.Utils.SendIR("SELECT_LEFT");
            //CL.EA.UI.Utils.GetEpgInfo("title", ref focusedTitle);
            //if (focusedTitle == null)
            //{
            //    FailStep(CL, "Failed to get focused title on set fvrt screen");
            //}

            //if (!(focusedTitle.Contains("")))
            //{

            //    FailStep(CL, "");
            //}

            CL.EA.UI.Utils.SendIR("SELECT");

            CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
            CL.EA.UI.Utils.GetEpgInfo("title", ref channelName);

            if ((channelName.Contains(channelNameBeforeReorder)) && (ChannelID.Contains(ChannelIDBeforeReorder)))
            {
                LogCommentImportant(CL, "Fvrt order is set to default order");
            }
            else
            {
                FailStep(CL, "Fvrt order is not set to default order");
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

        CL.EA.STBSettings.UnsetAllFavChannels();

          
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

                CL.IEX.Wait(4);

                if (EpgText.Trim() == "RENUMBER FAVOURITES")
                {
                    // Confirm reorder by checking id is 1 and chname is service 2 channel name
                    CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project, "FAVOURITES", "REORDER_SELECT"));

                    CL.EA.UI.Utils.GetEpgInfo("id", ref ChannelID);
                    CL.EA.UI.Utils.GetEpgInfo("title", ref ChannelName);

                    CL.IEX.Wait(4);
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


    }
}


  
