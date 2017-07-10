/// <summary>
///  Script Name : GRID_2011_2036_Pgrm_Grid_All_Single_Ch.cs
///  Test Name   : GRID-2011-2036-Pgrm-Grid-All-Single-Ch
///  TEST ID     : 24604,24605
///  QC Version  : 7
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

[Test("GRID_2011_2036_Pgrm_Grid_All_Single_Ch")]
public class GRID_2011_2036 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static Helper _helper = new Helper();
    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static Service S1;
    static Service S2;
    static Service S3;
    static Service S4;
    static bool isInGuide;
    static bool result;
    static string guideView;
    static string evtTime="";
    static string evtTimeFrmGrid="";
    static string timestamp = "";
    public const double timeToPressKey = -1;
    static string channelNumber="";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Service S1,S2,S3,s4 FROM CONTENT XML & set S1 & S4 as favourite";
    private const string STEP1_DESCRIPTION = "Step 1:Launch guide & Verify focus in on current event running background ";
    private const string STEP2_DESCRIPTION = "Step 2:Enable favourite mode & verify favourites channels in channel lineup ";
    private const string STEP3_DESCRIPTION = "Step 3:Launch Service s2 & launch guide &Verify focus in on current event running background in favourite mode";
    

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
            // fetch service from content xml
            S1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            S2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + S1.LCN + "");
            S3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True","ParentalRating=High;LCN=" + S1.LCN + "," + S2.LCN + "");
            S4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + S1.LCN + "," + S2.LCN + "," + S3.LCN + "");

            guideView = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "GuideView");
            if (guideView.Trim() == "")
            {
                FailStep(CL, "Failed to fetch GuideView value from Test INI");
            }

            CL.IEX.Wait(2);
            // set s2 & s4 favourite

            res = CL.EA.STBSettings.SetFavoriteChannelNumList("" + S2.LCN + "," + S4.LCN + "", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels as favourite");
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

            _helper.LaunchGuideVerifyFocus();

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
            // enabling favourite mode from action bar
           result= _helper.enableFavoriteMode();
           if (!result)
           {
               FailStep(CL, "Fail to enable favourite mode");
           }
            // verifying favourite channels in channel line up
            CL.IEX.Wait(4);
            if (guideView == "ALL_CHANNELS")
            {

                CL.EA.UI.Guide.SurfChannelDown(Type: "Ignore");
            }
            else
            {
                CL.EA.UI.Guide.SurfChannelRight(Type: "Ignore");
            }
           
           
            CL.IEX.Wait(4);

            CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumber);

            CL.IEX.Wait(2);

            if (channelNumber == "2")
            {
                LogCommentInfo(CL, "Verified Favourite channel lineup in favourite mode");
            }
            else
            {
                FailStep(CL, "Failed to Verify Favourite channel lineup in favourite mode");
            }
            


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
            // Tunning to S2 Service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Tune to Service S2");
            }

            // launching guide and verify focus

            _helper.LaunchGuideVerifyFocus();
            
            

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

    }

    #endregion
    public class Helper : _Step
    {
        
        public override void Execute() { }

        public bool enableFavoriteMode()
        {
            try
            {
                CL.IEX.Wait(1);
                CL.IEX.SendIRCommand("SELECT", timeToPressKey, ref timestamp);
                CL.IEX.Wait(2);
                CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("ENABLE FAVOURITE MODE");
                CL.IEX.Wait(1);
                CL.IEX.SendIRCommand("SELECT", timeToPressKey, ref timestamp);
                CL.IEX.Wait(5);
                return true;
            }
            catch (Exception)
            {
                try
                {
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

        public void LaunchGuideVerifyFocus()
        {
            CL.IEX.Wait(10);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTime);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to get evtTime  Milestone");
                }
                CL.IEX.MilestonesEPG.ClearEPGInfo();
                CL.IEX.Wait(2);
              
            if (guideView == "ALL_CHANNELS")
            {
               
                // navigate to All Channel
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

            }
            else
            {
               
                //  navigate to single channel

                CL.EA.UI.Guide.NavigateToGuideSingleChannel();
				
				CL.EA.UI.Utils.ReturnToLiveViewing();

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
            }

            CL.IEX.Wait(10);

            // fetch evtName from grid

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out evtTimeFrmGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to get evtTime Milestone");
            }
          
            CL.IEX.Wait(7);

            if (evtTime == evtTimeFrmGrid)
            {
               LogCommentInfo(CL,"Verified focus is on current event after launching grid");
            }
            else
            {
              FailStep(CL,"Fail to verify focus is on current event after launching grid");
            }
        }
    }

}