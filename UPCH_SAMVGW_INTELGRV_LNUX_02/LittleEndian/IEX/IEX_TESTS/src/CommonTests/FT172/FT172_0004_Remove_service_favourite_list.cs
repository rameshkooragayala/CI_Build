/// <summary>
///  Script Name : FT172_0004_Remove_service_favourite_list.cs
///  Test Name   : FT172_0004_Remove_service_favourite_list
///  TEST ID     : 74268
///  QC Version  : 1
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Frederic Luu
///  Modified Date : 17/06/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT172_0004")]
public class FT172_0004 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static List<string> favoriteChannelList;
    static List<string> favoriteChannelNumList;
    const int NB_FAVORITES = 7;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Set favorite channels and enable favorite mode";
    private const string STEP1_DESCRIPTION = "Step 1: Remove S2 from favorites using settings and check re-ordering";
    private const string STEP2_DESCRIPTION = "Step 2: Tune to 4 and check channel name is equal to S5 in channel bar";
    private const string STEP3_DESCRIPTION = "Step 3: Remove S5 from favorites using action menu then check automatic tuning to S1 and new ordering";
    private const string STEP4_DESCRIPTION = "Step 4: Remove S4 from favorites using guide then check re-ordering ";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);

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
            List<Service> serviceList = CL.EA.GetServiceListFromContentXML("Encryption=Clear");            
            if (serviceList.Count < NB_FAVORITES)
            {
                FailStep(CL, "Failed to get " + NB_FAVORITES + " clear services from Content.xml");
            }   
            favoriteChannelList = new List<string>();
            favoriteChannelNumList = new List<string>();
            for (int i = 0; i < NB_FAVORITES; i++)
            { 
                favoriteChannelList.Add(serviceList[i].Name);
                favoriteChannelNumList.Add(serviceList[i].LCN);
            }
            string _favoriteChannelList = String.Join(",", favoriteChannelList);
            string _favoriteChannelNumList = String.Join(",", favoriteChannelNumList);
            //Unset all the current favourites channel 
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove all Favorite channel");
            }

            // Set favorite channels
            //res = CL.EA.STBSettings.SetFavoriteChannelNameList(_favoriteChannelList, EnumFavouriteIn.Settings);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, "Failed to set channels " + _favoriteChannelList + " as favorites");
            //}

            res = CL.EA.STBSettings.SetFavoriteChannelNumList(_favoriteChannelNumList, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels " + _favoriteChannelNumList + " as favorites");
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
            
            // Remove S2 from favorite list using settings
            res = CL.EA.STBSettings.UnsetFavoriteChannelNameList(favoriteChannelList[1], EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove channel 2 (" + favoriteChannelNumList[1] + ") from favorites");
            }
            favoriteChannelList.RemoveAt(1);

            // Check favorite re-ordering
            verifyFavoriteOrder(favoriteChannelList, this);

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
            
            // Return to live
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to return to live");
            }

            // Tune to 4
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "4");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to channel 4");
            }

            // Check channel name is equal to S5 in channel bar
            string channelName = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName))
            {
                FailStep(CL, "Failed to check channel name: Unable to read 'chname' milestone");
            }
            if (channelName != favoriteChannelList[3])
            {
                FailStep(CL, "Wrong channel name. Read: " + channelName + ", Expected: " + favoriteChannelList[3]);
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

            // Remove S5 from favorite list using action menu
            res = CL.EA.STBSettings.UnsetFavoriteChannelNumList("4", EnumFavouriteIn.ActionBar);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove channel 4 from favorites");
            }
            favoriteChannelList.RemoveAt(3);
            CL.IEX.Wait(5);
            
            // Check that CPE tuned to S1 (first favorite channel)
            string channelName = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName))
            {
                FailStep(CL, "Failed to check channel name: Unable to read 'chname' milestone");
            }
            if (channelName != favoriteChannelList[0])
            {
                FailStep(CL, "Wrong channel name after S5 removal. Read: " + channelName + ", Expected: " + favoriteChannelList[0]);
            }

            // Check favorite re-ordering
            verifyFavoriteOrder(favoriteChannelList, this);          

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

            // Remove S4 from favorite list using guide
            res = CL.EA.STBSettings.UnsetFavoriteChannelNumList("3", EnumFavouriteIn.Guide);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to remove channel 3 from favorites");
            }
            favoriteChannelList.RemoveAt(2);
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to return to live viewing");
            }
            // Check favorite re-ordering
            verifyFavoriteOrder(favoriteChannelList, this);

            PassStep();
        }
    }
    #endregion

    // Verify the favorites order in re-ordering menu and compare with the given list
    private static void verifyFavoriteOrder(List<string> expFavoriteChannelList, _Step step)
    {
        // Navigate to Re-ordering menu
        if (!CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:RENUMBER FAVOURITES"))
        {
            step.FailStep(CL, "Failed to verify favorite order: Unable to navigate to re-ordering menu");
        }  

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

            // Navigate to next favorite by pressing DOWN key
            CL.EA.UI.Utils.SendIR("SELECT_DOWN");

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

        } while (channelTitle != readFavorites[0].channelName);

        // Sort the read favorites by channel number
        readFavorites.Sort();

        // compare with the expected list of favorite
        for (int i = 0; i < expFavoriteChannelList.Count; i++)
        {
            // compare favorite channel numbers and channel names
            if ((readFavorites[i].channelNumber != (i + 1)) || (readFavorites[i].channelName != expFavoriteChannelList[i]))
            {
                step.FailStep(CL, "Wrong favorite order\nRead: " + String.Join(", ", readFavorites) + "\nExpected: " + String.Join(", ", expFavoriteChannelList));
            }
        }

        CL.EA.ReturnToLiveViewing();
    }    

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
    
    #endregion

    #region PostExecute

    public override void PostExecute()
    {
        //Unset all the current favourites channel 
        CL.EA.STBSettings.UnsetAllFavChannels();        
    }

    #endregion PostExecute
}

