/// <summary>
///  Script Name : FT172_0007_DCA_in_fast_channel_list_fav_mode.cs
///  Test Name   : FT172_0007_DCA_in_fast_channel_list_fav_mode
///  TEST ID     : 74266
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

[Test("FT172_0007")]
public class FT172_0007 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static List<Favourite> expFavorites;
    static Service nonFavChannel;
    static string highestChannelNumber;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Set favorite channels and enable favorite mode";
    private const string STEP1_DESCRIPTION = "Step 1: Launch fast channel list and check the list of channels";
    private const string STEP2_DESCRIPTION = "Step 2: Focus channel 1";
    private const string STEP3_DESCRIPTION = "Step 3: Focus channel 4";
    private const string STEP4_DESCRIPTION = "Step 4: Enter LCN of an existing channel in channel line-up";
    private const string STEP5_DESCRIPTION = "Step 5: Enter LCN of a non-existing channel in channel line-up (greater than the highest LCN in channel line-up)";

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
            // Split and store the expected favourites in a list
            string[] favoriteChannelList = _favoriteChannelList.Split(new char[] { ',' });
            expFavorites = new List<Favourite>();
            for (int i=0; i<favoriteChannelList.Length; i++)
            {
                expFavorites.Add(new Favourite() { channelName = favoriteChannelList[i], channelNumber = i+1 });
            }

            // Fetch a non-favorite channel            
            nonFavChannel = CL.EA.GetServiceFromContentXML("", "Name=" + String.Join(",", favoriteChannelList));
            if (nonFavChannel == null)
            {
                FailStep(CL, "Failed to fetch a service different from the favorite channels");
            }
            
            highestChannelNumber = CL.EA.GetValue("Highest_Service_Number");
            if (string.IsNullOrEmpty(highestChannelNumber))
            {
                FailStep(CL, "Highest_Service_Number is empty in Channels.ini");
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

            // Launch ChannelLineUP
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelLineup, "", false, -1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Channel Lineup From Live");
            }

            // check available channels and order
            verifyFavoriteOrder(expFavorites, this);

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

            // Focus favorite channel 1
            CL.EA.UI.Utils.SendIR("KEY_1");            

            // Check channel name is equal to S1 in channel bar
            string channelName = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName))
            {
                FailStep(CL, "Failed to check channel name: Unable to read 'chname' milestone");
            }
            if (channelName != expFavorites[0].channelName)
            {
                FailStep(CL, "Wrong channel name. Read: " + channelName + ", Expected: " + expFavorites[0].channelName);
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

            // Focus favorite channel 4
            CL.EA.UI.Utils.SendIR("KEY_4");

            // Check channel name is equal to S4 in channel bar
            string channelName = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelName))
            {
                FailStep(CL, "Failed to check channel name: Unable to read 'chname' milestone");
            }
            if (channelName != expFavorites[3].channelName)
            {
                FailStep(CL, "Wrong channel name. Read: " + channelName + ", Expected: " + expFavorites[3].channelName);
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

            // Enter LCN of an existing channel in channel line-up
            CL.EA.UI.Utils.SendChannelAsIRSequence(nonFavChannel.LCN);

            // Check channel number is equal to the highest favorite channel number
            string channelNumber = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chNum", ref channelNumber))
            {
                FailStep(CL, "Failed to check channel number: Unable to read 'chNum' milestone");
            }
            if (channelNumber != expFavorites.Count.ToString())
            {
                FailStep(CL, "Wrong channel number. Read: " + channelNumber + ", Expected: " + expFavorites.Count.ToString());
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

            // Enter LCN of a non-existing channel in channel line-up (greater than the highest LCN in channel line-up)
            CL.EA.UI.Utils.SendChannelAsIRSequence(highestChannelNumber);

            // Check channel number is equal to the highest favorite channel number
            string channelNumber = "";
            if (!CL.EA.UI.Utils.GetEpgInfo("chNum", ref channelNumber))
            {
                FailStep(CL, "Failed to check channel number: Unable to read 'chNum' milestone");
            }
            if (channelNumber != expFavorites.Count.ToString())
            {
                FailStep(CL, "Wrong channel number. Read: " + channelNumber + ", Expected: " + expFavorites.Count.ToString());
            }

            PassStep();
        }
    }
    #endregion

    // Read the favorites available in fast channel list and compare with the given list
    private static void verifyFavoriteOrder(List<Favourite> expFavorites, _Step step)
    {
        // Read all listed favorites
        List<Favourite> readFavorites = new List<Favourite>();
        string channelTitle = "";
        string channelNumber = "";

        // Read the focused channel name
        if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelTitle))
        {
            step.FailStep(CL, "Failed to verify favorite order: Unable to get 'chname' milestone when parsing favorite list");
        }

        // Read the focused channel number
        if (!CL.EA.UI.Utils.GetEpgInfo("chNum", ref channelNumber))
        {
            step.FailStep(CL, "Failed to verify favorite order: Unable to get 'chNum' milestone when parsing favorite list");
        }

        do
        {
            // Store the favorite channel name/number
            readFavorites.Add(new Favourite() { channelName = channelTitle, channelNumber = Int32.Parse(channelNumber) });

            CL.EA.UI.Utils.ClearEPGInfo();

            // Navigate to next favorite by pressing DOWN key
            CL.EA.UI.Utils.SendIR("SELECT_DOWN");

            // Read the focused channel name
            if (!CL.EA.UI.Utils.GetEpgInfo("chname", ref channelTitle))
            {
                step.FailStep(CL, "Failed to verify favorite order: Unable to get 'chname' milestone when parsing favorite list");
            }

            // Read the focused channel number
            if (!CL.EA.UI.Utils.GetEpgInfo("chNum", ref channelNumber))
            {
                step.FailStep(CL, "Failed to verify favorite order: Unable to get 'chNum' milestone when parsing favorite list");
            }

        } while (channelTitle != readFavorites[0].channelName);

        // Sort the read favorites by channel number
        readFavorites.Sort();

        // compare with the expected list of favorite
        if (!readFavorites.SequenceEqual(expFavorites))
        {
            step.FailStep(CL, "Wrong favorite order\nRead: " + String.Join(", ", readFavorites) + "\nExpected: " + String.Join(", ", expFavorites));
        }
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
}



