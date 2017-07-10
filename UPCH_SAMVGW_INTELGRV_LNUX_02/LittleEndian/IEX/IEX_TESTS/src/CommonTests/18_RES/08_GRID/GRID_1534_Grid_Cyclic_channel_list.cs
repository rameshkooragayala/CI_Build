/// <summary>
///  Script Name : GRID_1534_Grid_Cyclic_channel_list.cs
///  Test Name   : GRID-1534-Grid-Cyclic-channel-list
///  TEST ID     : 11298
///  QC Version  : 10
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("GRID_1534_Grid_Cyclic_channel_list")]
public class GRID_1534 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static Helper _helper=new Helper();
    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static bool isInGuide;
    static string AdjTimelineDuration;
    static string crumbTextAdjTImeline;
    static string category;
    static string lowestChannelNumber="";
    static string higestChannelNumber="";
    static string lowestChannelNumberFrmGrid="";
    static string higestChannelNumberFrmGrid="";
    static string channelNum="";
    static string channelName="";
    static string timestamp ="";
    static string channelCount;
    static string mode;
    public const double timeToPressKey = -1;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch Mode & First and Last Channel Numbers in Channel line up";
    private const string STEP1_DESCRIPTION = "Step 1:Launch All Channels Guide and Verify Grid is Cyclic";
    private const string STEP2_DESCRIPTION = "Step 2:Launch By Channel Guide and Verify Grid is Cyclic";
    private const string STEP3_DESCRIPTION = "Step 3:Launch Adjust Timeline Guide and Verify Grid is Cyclic";
   
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

            //Get Values From ini File
            FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");
            // fetch duration for adjust timeline
            AdjTimelineDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DURATION");

            category = CL.EA.UI.Utils.GetValueFromTestIni("TEST PARAMS", "CATEGORY");

            // fetch total number of channels
            channelCount = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "CHANNEL_COUNT");
            // fetch total number of channels
            mode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MODE");

            // Fetch First channel in channel line up
            lowestChannelNumber = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Lowest_Service_Number");
            if (string.IsNullOrEmpty(lowestChannelNumber))
            {
                lowestChannelNumber = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "Lowest_Service_Number");
            }

            higestChannelNumber = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Highest_Service_Number");
            if (string.IsNullOrEmpty(higestChannelNumber))
            {
                higestChannelNumber = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "Highest_Service_Number");
            }

            if (mode == "FAVOURITE")
            {
                Service  channel1 = CL.EA.GetServiceFromContentXML("Type=Video", "");
                Service channel2 = CL.EA.GetServiceFromContentXML("Type=Video", "LCN="+ channel1.LCN);
                Service channel3 = CL.EA.GetServiceFromContentXML("Type=Video", "LCN=" + channel1.LCN+","+channel2.LCN);

                CL.IEX.Wait(2);
                res = CL.EA.STBSettings.SetFavoriteChannelNumList(channel1.LCN + "," + channel2.LCN + "," + channel3.LCN, EnumFavouriteIn.Settings);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Faile to set channels as Favourite");
                }
                CL.IEX.Wait(2);
                _helper.enableFavoriteMode();

                lowestChannelNumber = "1";
                higestChannelNumber = "3";

               res= CL.EA.ReturnToLiveViewing();
               if (!res.CommandSucceeded)
               {
                   FailStep(CL, "Fail to return to live");
               }

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
             //navigate to All channel

            CL.EA.UI.Guide.Navigate();
            isInGuide = CL.EA.UI.Guide.IsGuide();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch All channels Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified All Channels guide launched");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, lowestChannelNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to tune to first channel");
            }

            _helper.CheckCyclic("SELECT_UP", "SELECT_DOWN");
            _helper.SurfAllChannels("SELECT_DOWN");


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
           //  navigate to single channel

            CL.EA.UI.Guide.NavigateToGuideSingleChannel();

            isInGuide = CL.EA.UI.Guide.IsGuideSingleChannel();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch single channel Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified single Channels guide launched");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.GuideSingleChannel, lowestChannelNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to tune to first channel");
            }

            _helper.CheckCyclic("SELECT_LEFT", "SELECT_RIGHT");

            _helper.SurfAllChannels("SELECT_RIGHT");
            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            // navigate to Adjust TimeLine

            CL.EA.UI.Guide.NavigateToGuideAdjustTimeline(AdjTimelineDuration);
            CL.IEX.Wait(2);
            CL.IEX.MilestonesEPG.GetEPGInfo("crumbtext", out crumbTextAdjTImeline);
            if (crumbTextAdjTImeline.ToUpper() != "ADJUST TIMELINE  ALL CHANNELS")
            {
                FailStep(CL, "Failed to launch Adjust Timeline Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified Adjust Timeline guide launched");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.GuideAdjustTimeline,ChannelNumber:lowestChannelNumber,GuideTimeline:AdjTimelineDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to tune to first channel");
            }

            _helper.CheckCyclic("SELECT_UP", "SELECT_DOWN");

            PassStep();
        }
    }
    #endregion



    #endregion

        #region PostExecute
        [PostExecute()]
        public override void PostExecute()
        {
            if(mode=="FAVOURITE")
            CL.EA.STBSettings.UnsetAllFavChannels();
        }
        #endregion

    public class Helper : _Step
    {

        public override void Execute() { }
        // Enable favorite mode in action menu
        public void CheckCyclic(string keyup,string keydown)
        {
            // verifiing grid is cyclic by pressing up button and match with higesh channel number

            res = CL.IEX.SendIRCommand(keyup, timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to select previous channel");
            }

            CL.IEX.Wait(2);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out higestChannelNumberFrmGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to fetch higest channel number from EPG Info");
            }
            CL.IEX.Wait(5);

            if (higestChannelNumberFrmGrid.Trim() == higestChannelNumber.Trim())
            {
                LogCommentInfo(CL, "Verified grid is cyclic from lowest channel to highest channel");
            }
            else
            {
                FailStep(CL, "Fail to verify grid is cyclic from lowest channel to highest channel");
            }

            // verifiing grid is cyclic by pressing down button and match with lowest channel number

            res = CL.IEX.SendIRCommand(keydown, timeToPressKey, ref timestamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to select next channel");
            }

            CL.IEX.Wait(2);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out lowestChannelNumberFrmGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to fetch lowest channel number from EPG Info");
            }
            CL.IEX.Wait(5);

            if (lowestChannelNumberFrmGrid.Trim() == lowestChannelNumber.Trim())
            {
                LogCommentInfo(CL, "Verified grid is cyclic from higest channel to lowest channel");
            }
            else
            {
                FailStep(CL, "Fail to verify grid is cyclic from higest channel to lowest channel");
            }


        }

        public void SurfAllChannels(string nextchannel)
        {
           
          
            for (int i = 0; i <= Convert.ToInt32(channelCount); i++)
            {
                CL.IEX.Wait(2);
                res = CL.IEX.SendIRCommand(nextchannel, timeToPressKey, ref timestamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to select next channel");
                }

                CL.IEX.Wait(2);

                res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNum);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to fetch Channel Number from EPG Info");
                }
                CL.IEX.Wait(2);

               
                LogComment(CL, "Channel : " + channelNum);

                if (channelNum == lowestChannelNumber)
                {
                    LogCommentInfo(CL, "Scroll the entire channel line- up successful");
                    break;
                }

                else if (i == 60)
                {
                    FailStep(CL, "Fail to Scroll the entire channel line- up");
                    break;
                }

            }
         }

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

        // Disable favorite mode in action menu
        public bool disableFavoriteMode()
        {
            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR");
                if (CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("DISABLE FAVOURITE MODE"))
                {
                    CL.EA.UI.Utils.SendIR("SELECT");
                    CL.IEX.Wait(1);
                    CL.EA.UI.Utils.SendIR("RETOUR");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    
    }
}
