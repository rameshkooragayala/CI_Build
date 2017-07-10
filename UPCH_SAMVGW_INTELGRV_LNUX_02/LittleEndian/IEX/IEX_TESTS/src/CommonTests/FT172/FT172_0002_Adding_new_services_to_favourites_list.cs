/// <summary>
///  Script Name : FT172_0002_Adding_new_services_to_favourites_list.cs
///  Test Name   : FT172-0002-Adding-new-services-to-favourites-list
///  TEST ID     : 74291
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by :Mithlesh , Madhu R
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT172-0002-Adding-new-services-to-favourites-list")]
public class FT172_0002 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    private static Service Service1;
    private static Service Service3;
    private static Service Service5;
    private static Service Service10;
    private static Service Service9;
    private static Service Service6;
    private static Service Service2;

    private static string obtainedFavouriteChNum = "";
    private static string obtainedLogicalChNum = "";
    static List<Favourite> expFavorites;
    //Variables which are used in different steps

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch list of Service from Test.INI file to be Set As Favourite & Set some Few channel as Favourite Channel";
    private const string STEP1_DESCRIPTION = "Step 1: Add service s10 ,s9 to Fav list, Tune to service s5 & Press P+ so that channel tuned to s10, AGAIN Press P+ so that channel tuned to S9 ";
    private const string STEP2_DESCRIPTION = "Step 2: Perform DCA to s6 , add s6 to Fav List & LAUNCH Guide and make s2 as Favourite & tune to s6 channel again ";
    private const string STEP3_DESCRIPTION = "Step 3: Launch Channel Bar on LIve on s6,ENABLE FAVOURITE MODE, launch channel bar on s6 & Press p+ so that CPE tuned to s2 & Navigate to RENUMBER FAVOURITES to Check favorite re-ordering ";


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
            
            //Unset all the current favourites channel 
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove all Favorite channel");

            }

            // Fetch the services listed in content.xml
            List<Service> allServicesList = new List<Service>();
            allServicesList = CL.EA.GetServiceListFromContentXML("Type=Video;IsRecordable=True;IsEITAvailable=True;Encryption=Clear", "ParentalRating=High");

            if (allServicesList.Count >= 7)
            {
                Service1 = allServicesList[0];
                LogCommentInfo(CL, "Service1  ChName : " + Service1.Name + " and ChNum: " + Service1.LCN + "");

                Service3 = allServicesList[1];
                LogCommentInfo(CL, "Service3  ChName : " + Service3.Name + " and ChNum: " + Service3.LCN + "");

                Service5 = allServicesList[2];
                LogCommentInfo(CL, "Service5  ChName : " + Service5.Name + " and ChNum: " + Service5.LCN + "");

                Service10 = allServicesList[3];
                LogCommentInfo(CL, "Service10 ChName : " + Service10.Name + " and ChNum: " + Service10.LCN + "");

                Service9 = allServicesList[4];
                LogCommentInfo(CL, "Service9  ChName : " + Service9.Name + " and ChNum: " + Service9.LCN + "");

                Service6 = allServicesList[5];
                LogCommentInfo(CL, "Service6  ChName : " + Service6.Name + " and ChNum: " + Service6.LCN + "");

                Service2 = allServicesList[6];
                LogCommentInfo(CL, "Service2  ChName : " + Service2.Name + " and ChNum: " + Service2.LCN + "");

                string NonFavChannelNameList = " " + Service1.Name + "," + Service3.Name + "," + Service5.Name + "," + Service10.Name + "," + Service9.Name + "," + Service6.Name + "," + Service2.Name + " ";
                string[] NonfavoriteChannelLists = NonFavChannelNameList.Split(new char[] { ',' });

                //expFavorites will store all the 7 Channel retrevied from content XML with sequence ID' to be set as FAV channel
                expFavorites = new List<Favourite>();
                for (int i = 0; i < NonfavoriteChannelLists.Length; i++)
                {
                    expFavorites.Add(new Favourite() { channelName = NonfavoriteChannelLists[i], channelNumber = i + 1 });
                }
            }
            else
            {

                FailStep(CL, "Failed to fetch value from content.xml ");
            }

            LogCommentInfo(CL, "Adding  Service1 , Service3 , Service5 as FAV channel");
            res = CL.EA.STBSettings.SetFavoriteChannelNameList("" + Service1.Name + ", " + Service3.Name + "," + Service5.Name + "", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set SetFavoriteChannelNumList");
            }

            //FAVOURITE MODE should be Enabled as pre-condition. 
            //CHECK : if DISABLE FAVOURITE MODE is present on the ACTION BAR,then no need to navigate to "STATE:ENABLE FAVOURITE MODE" bcoz FAV MODE will be alreday enabled.
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
            else
            {
                FailStep(CL, "Failed to Navigate to ACTION BAR");

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

            LogCommentInfo(CL, "Add  Service10 and  Service9 to Favourites and ChNum=4, ChNum=5 should be assigned");
            res = CL.EA.STBSettings.SetFavoriteChannelNameList("" + Service10.Name + ", " + Service9.Name + "", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Both the channel As Favorite");
            }

            LogCommentInfo(CL, "Tuning to channel  Service5 : Favourite chNum = 3");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service channel  Service5 : Fav chNum=3 ");
            }


            LogCommentInfo(CL, "Do P+ : Tuning to  Service10, Favourite chNum:4 should be dispalyed on CHANNERBAR");
            CL.EA.UI.Utils.SendIR("CH_PLUS");

            CL.IEX.Wait(10);
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedFavouriteChNum);
            if (obtainedFavouriteChNum != "4")
            {
                FailStep(CL, "Received Different FAV ChNum: (" + obtainedFavouriteChNum + ") Than Expected ChNum:4");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            LogCommentInfo(CL, "Again Do the P+ :Tuning to  Service9, Favourite chNum:5 should be dispalyed on CHANNELBAR");
            CL.EA.UI.Utils.SendIR("CH_PLUS");

            CL.IEX.Wait(10);
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedFavouriteChNum);
            if (obtainedFavouriteChNum != "5")
            {
                FailStep(CL, "Received Different FAV ChNum: (" + obtainedFavouriteChNum + ") Than Expected ChNum:5");
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

            //******---NOW EXITING THE FAVOURITE MODE HERE-----********

            LogCommentInfo(CL, "Perform DCA TO  Service6 ,CPE should exit Fav Mode and logical Channel Number should be dispalyed.");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service6.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + Service6.LCN);
            }
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedLogicalChNum);
           
            if (obtainedLogicalChNum != Service6.LCN)
            {
                FailStep(CL, "Received Different Logical ChNum: (" + obtainedLogicalChNum + ") Than Expected ChNum:(" + Service6.LCN + ")");
              
            }

            //Launch ACTION BAR & add  Service6 to FAV List
            res = CL.EA.STBSettings.SetFavoriteChannelNumList(Service6.LCN, EnumFavouriteIn.ActionBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Make  Service6 As FAV by ACTIONBAR ");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + Service6.LCN);
            }

            CL.IEX.Wait(10);
            //Launch Guide and Navigate to  Service2 to Make FAV 
            LogCommentInfo(CL, "Launch Guide and Navigate to  Service2 to Make FAV :601");
            res = CL.EA.STBSettings.SetFavoriteChannelNumList(Service2.LCN, EnumFavouriteIn.Guide);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Make  Service2 as FAV by Guide ");
            }

            CL.IEX.Wait(10);
            // Tune to  Service6 channel again
            LogCommentInfo(CL, "Tune to  Service6 channel again : 726");
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service6.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service:" + Service6.LCN);
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

            //Launch Channel Bar on LIve on  Service6
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to CHANNEL BAR");
            }

            LogCommentInfo(CL, "Obtaining the logical Channel Number on  Service6");
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedLogicalChNum);
            
            if (obtainedLogicalChNum != Service6.LCN)
            {
                FailStep(CL, "Received Different Logical ChNum: (" + obtainedLogicalChNum + ") Than Expected ChNum:(" + Service6.LCN + ")");
            }
            
            //******---NOW ENTERING INTO FAVOURITE MODE HERE-----********

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
            else
            {
                FailStep(CL, "Failed to Navigate to ACTION BAR");

            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to navigate to CHANNEL BAR");
            }

            LogCommentInfo(CL, "Obtaining Favourite Channel Number on service6");
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedFavouriteChNum);
           
            if (obtainedFavouriteChNum != "6")
            {
                FailStep(CL, "Received Different FAV ChNum: (" + obtainedFavouriteChNum + ") Than Expected ChNum:6");
            }


            LogCommentInfo(CL, "Do P+ : Tuning to  Service2, Favourite chNum = 7 should be dispalyed on CHANNEL BAR");
            CL.EA.UI.Utils.SendIR("CH_PLUS");

            CL.IEX.Wait(10);
            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedFavouriteChNum);
           
            if (obtainedFavouriteChNum != "7")
            {
                FailStep(CL, "Received Different FAV ChNum: (" + obtainedFavouriteChNum + ") Than Expected ChNum:7");
            }       

            // Check favorite re-ordering 
            LogCommentInfo(CL, "Navigate to RENUMBER FAVOURITES & Check favorite re-ordering");
            verifyFavoriteOrder(expFavorites, this);

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

    // Verify the favorites order in re-ordering menu and compare with the given list
    private static void verifyFavoriteOrder(List<Favourite> expFavorites, _Step step)
    {
        //Restore default settings
        IEXGateway._IEXResult res;

        // Navigate to Re-ordering menu
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:RENUMBER FAVOURITES");
        if (!res.CommandSucceeded)
        {
            step.FailStep(CL, "Failed to verify favorite order: Unable to navigate to re-ordering menu");
        }

        CL.IEX.Wait(10);
        // Read all listed favorites
        List<Favourite> readFavorites = new List<Favourite>();
        string channelTitle = "";
        string channelNumber = "";

        // Read the focused channel name
        if (!CL.EA.UI.Utils.GetEpgInfo("title", ref channelTitle))
        {
            step.FailStep(CL, "Failed to verify favorite order: Unable to get 'title' milestone when parsing favorite list");
        }

        // Read the focused channel number
        if (!CL.EA.UI.Utils.GetEpgInfo("id", ref channelNumber))
        {
            step.FailStep(CL, "Failed to verify favorite order: Unable to get 'id' milestone when parsing favorite list");
        }

        do
        {
            // Store the favorite channel name/number
            readFavorites.Add(new Favourite() { channelName = channelTitle, channelNumber = Int32.Parse(channelNumber) });

            CL.EA.UI.Utils.ClearEPGInfo();
           
            CL.IEX.Wait(5);

            // Navigate to next favorite by pressing DOWN key
            CL.EA.UI.Utils.SendIR("SELECT_DOWN",5000);
            
            // Read the focused channel name
            if (!CL.EA.UI.Utils.GetEpgInfo("title", ref channelTitle))
            {
                step.FailStep(CL, "Failed to verify favorite order: Unable to get 'title' milestone when parsing favorite list");
            }

            // Read the focused channel number
            if (!CL.EA.UI.Utils.GetEpgInfo("id", ref channelNumber))
            {
                step.FailStep(CL, "Failed to verify favorite order: Unable to get 'chNum' milestone when parsing favorite list");
            }

            CL.IEX.Wait(5);

        } while (channelTitle != readFavorites[0].channelName);

        // Sort the read favorites by channel number
        readFavorites.Sort();

        // compare with the expected list of favorite
        if (!readFavorites.SequenceEqual(expFavorites))
        {
            step.FailStep(CL, "Wrong favorite order\nRead: " + String.Join(", ", readFavorites) + "\nExpected: " + String.Join(", ", expFavorites));
        }

        CL.EA.ReturnToLiveViewing();


    }

    public class Favourite : IEquatable<Favourite>, IComparable<Favourite>
    {
        public int channelNumber { get; set; }
        public string channelName { get; set; }

        public override string ToString()
        {
            return channelNumber + " - " + channelName;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Favourite objAsFav = obj as Favourite;
            if (objAsFav == null) return false;
            else return Equals(objAsFav);
        }

        // Default comparer for Favourite type.
        public int CompareTo(Favourite compareFavourite)
        {
            // A null value means that this object is greater.
            if (compareFavourite == null)
                return 1;

            else
                return this.channelNumber.CompareTo(compareFavourite.channelNumber);
        }
        public override int GetHashCode()
        {
            return channelNumber;
        }
        public bool Equals(Favourite other)
        {
            if (other == null) return false;
            return (this.channelNumber.Equals(other.channelNumber));
        }
    }


}