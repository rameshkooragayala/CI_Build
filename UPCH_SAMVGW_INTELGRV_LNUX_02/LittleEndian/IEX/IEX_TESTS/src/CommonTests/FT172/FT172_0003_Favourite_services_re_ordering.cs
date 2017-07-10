/// <summary>
///  Script Name : FT172_0003_Favourite_services_re_ordering.cs
///  Test Name   : FT172_0003_Favourite_services_re_ordering
///  TEST ID     : FT172-0003
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
/// Created By : Aswin
///  Modified by :
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

[Test("FT172_0003_Favourite_services_re_ordering")]
public class FT172_0003_Favourite_services_re_ordering : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static Helper helper = new Helper();
    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    private static string FavChannelList = string.Empty; //Channel to be set as Fav

    private static Service favService1;
    private static Service favService2;
    private static Service favService3;

   


    static string channelName = "";
    static string ChannelID = "";
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Fav Channel Numbers From ini File & Set Channels as Favourite";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to Serivce S2 & Reorder Favourite List - Reorder  Service 2 Position to Service 1 Position ";
    private const string STEP2_DESCRIPTION = "Step 2:Verify Reordering by pressing P +  & check the next channel is Service 1 ";

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

        //EA Unset all favourites

          res= CL.EA.STBSettings.UnsetAllFavChannels();
          if (!res.CommandSucceeded)
          {
              FailStep(CL, "Failed to remove all Favorite channels");

          }


            //Get List of Fav Channel from TEST ini File

           // FavChannelList = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FavChannelList");


            //if (string.IsNullOrEmpty(FavChannelList))
            //{
            //    FailStep(CL, res, "Unable to fetch the ChannelList from test ini file");
            //}

            //string[] str = FavChannelList.Split(',');

          
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

            
            //Set Service 1 , Service 2 & Service 3 as Favourite Channels
         
            res = CL.EA.STBSettings.SetFavoriteChannelNameList(favService1.Name + "," + favService2.Name + "," + favService3.Name, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set SetFavoriteChannelNumList");
            }

                res = CL.EA.ReturnToLiveViewing(false);

                if (!res.CommandSucceeded)
                {

                    CL.IEX.LogComment("Fail to return Live");
                }

                  //Enable Favourite Mode

                if (!helper.enableFavoriteMode())
                {
                    FailStep(CL, "Failed to Enable Favourite Mode");
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
           
            // surf to Service 2

          
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Constants.currentid);
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


            //Reordering Serviec 2 to position 1
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
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Surfing P+

            CL.EA.UI.Utils.SendIR(CL.EA.GetValueFromINI(EnumINIFile.Project,"FAVOURITES","NEXT_CHANNEL"));

            CL.IEX.Wait(5);

            CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName);

            if (channelName != favService1.Name)
            {
                FailStep(CL, "Fail to verify reorder Favourite list");
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
       public bool  enableFavoriteMode()
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


  
