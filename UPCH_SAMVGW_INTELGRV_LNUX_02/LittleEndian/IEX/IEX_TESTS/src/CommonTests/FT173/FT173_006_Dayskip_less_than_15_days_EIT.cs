/// <summary>
/// Script Name :  FT173_006_Dayskip_less_than_15_days_EIT.cs
///  Test Name   : FT173_006
///  TEST ID     : 25396
///  QC Version  : 4
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by :  Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT173_006_Dayskip_less_than_15_days_EIT")]
public class FT173_006 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;
    static bool isPass=false;
    //Shared members between steps
    static string FTA_Channel;
    static string mode;
    static string condition;
    static int EIT;
    static EnumGuideViews guideView;
    static string guideviewFromTestIni;
    static EnumSurfIn enumSurfIn;
    static Helper _helper = new Helper();
    static string channel1;
    static string channel2;
    static string evtName;
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel,Guide,EIT Avalilable,condtion & Launch Guide";
    private const string STEP1_DESCRIPTION = "Step 1:Perform  Day skip till EIT is available and check event information ";
    private const string STEP2_DESCRIPTION = "Step 2:Perform  Day Skip and check event information";
    

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

            try
            {
                

                //Get Service from tEST ini

                channel1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel1");
                channel2 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel2");

                // Fetch Guide View from Test.INI
                guideviewFromTestIni = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "GuideView");
                if (guideviewFromTestIni == "")
                {
                    FailStep(CL, "Fail to fetch GuideView from Test INI");
                }

                //Fetch Mode from Test INI
                mode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Mode");
                if (mode == "")
                {
                    FailStep(CL, "Fail to fetch Mode from Test INI");
                }

                ////////////////change test params

               //fetch number of days EIT available from Test INI
                EIT = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EIT_AVAILABLE"));

                /// assign value to EnumGuideView as per value from Test INI


                guideView = (EnumGuideViews)Enum.Parse(typeof(EnumGuideViews), guideviewFromTestIni, true);
               


                // Set Mode as per Test INI

                if (mode == "FAVOURITE")
                {
                       res = CL.EA.STBSettings.SetFavoriteChannelNumList("" + channel1 + "," + channel2 + "", EnumFavouriteIn.Settings);

                    if (!res.CommandSucceeded)
                    {
                        LogComment(CL, "Failed to set channels as favourite");
                    }


                    res = CL.EA.ReturnToLiveViewing(false);
                    if (!res.CommandSucceeded)
                    {
                        LogComment(CL, "Failed to return to live");
                    }

                    if (_helper.enableFavoriteMode())
                    {
                        LogComment(CL, "Enabled Favourite Mode");

                    }
                    else
                    {
                        FailStep(CL, "Failed to Enable Favourite Mode");
                    }

                }
                else if (mode == "MINITV")
                {
                    res = CL.EA.STBSettings.SetTvGuideBackgroundAsSolid();
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Failed to Enable MINITV Mode");
                    }

                }

                if (mode == "FAVOURITE") // if favaouriter surf to "2" else to "720"
                {
                    res = CL.EA.ChannelSurf(EnumSurfIn.Live, "1");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Fail to Tune to service s1");
                    }
                }
                else
                {
                    res = CL.EA.ChannelSurf(EnumSurfIn.Live, channel1);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Fail to Tune to service s1");
                    }
                }
            }
            catch (Exception ex)
            {
                FailStep(CL, "Fail in precondition. Reason: "+ex.Message);
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
            // evt Name is available till EIT exists
            res = CL.EA.DaySkipInGuide(guideView, true, EIT-1, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform Day Skip");
            }

            CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
            CL.IEX.Wait(1);

            if (evtName != "No programme information available")
            {
                LogComment(CL, "Verified event information available");
            }
            else
            {
                res = CL.EA.DaySkipInGuide(guideView, false,1, false, false);  
                // Head End  has given 4 days EIT and 4th day last event on 1:50 AM. if test is running after 1:50 the evtname will be "No Program available and it will fail the test to overcome the same we are moving to previous day event.
                EIT = EIT - 1; // changing EIT data available

                CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName); // fetch event name
                CL.IEX.Wait(1);
                if (evtName != "No programme information available")
                {
                    LogComment(CL, "Verified event information available");
                }
                else
                {
                    FailStep(CL, "Failed - Event information is not available");
                }
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

            switch (guideView)
            {
                case EnumGuideViews.ALL_CHANNELS:
                    {
                        if (!_helper.dayskipAllchannels_AdjTimeline())
                        {
                            FailStep(CL, "Failed to Perform  Day Skip and check event information");
                        }
                        break;
                    }
               
                case EnumGuideViews.ADJUST_TIMELINE:
                    {
                        if (!_helper.dayskipAllchannels_AdjTimeline())
                        {
                            FailStep(CL, "Failed to Perform  Day Skip and check event information");
                        }

                        break;
                    }
                case EnumGuideViews.SINGLE_CHANNEL:
                    {
                        if (!_helper.dayskipSingleChannel())
                        {
                            FailStep(CL, "Failed to Perform  Day Skip and check event information");
                        }
                        
                        break;
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
        // reverting all settings
        if (mode == "FAVOURITE")
        {
            // unset assignes favourite channels and disable favourite mode
            CL.EA.STBSettings.UnsetFavoriteChannelNumList("" + channel1 + "," + channel2 + "", EnumFavouriteIn.Settings);
            if (_helper.enableFavoriteMode())
            {
                LogComment(CL, "Enabled Favourite Mode");

            }
            else
            {
                LogComment(CL, "failed Favourite Mode");
            }

        }
        else if (mode == "MINITV")
        {
            CL.EA.STBSettings.SetTvGuideBackgroundAsTransparent(); // back to Transparent which is default value

        }

    }
    #endregion

    public class Helper : _Step
    {

        public override void Execute() { }
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

        public bool  dayskipAllchannels_AdjTimeline() //or checking grid is showing "No Program information"  if no EIT 
        {
            isPass = true;
            for (int i = 0; i < (14 - EIT); i++)
            {

                res = CL.EA.DaySkipInGuide(guideView, true, 2, false, true);
               
                CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);

                if (evtName == "No programme information available")
                {

                    LogCommentInfo(CL, "Verified No information available");
                    isPass = true;
                }
                else
                {
                    LogCommentFailure(CL,"Failed. Event information is  available");
                    isPass = false;
                    break;
                }
            }
            return isPass;
        }

        public bool dayskipSingleChannel() // for checking grid is not skipping to next day if no EIT 
        {
            isPass = true;
            string selectionDate = "";
            string selectionDateAfterSkip = "";
          res= CL.IEX.MilestonesEPG.GetEPGInfo("selection date", out selectionDate);
          CL.IEX.Wait(2);
          if (!res.CommandSucceeded)
          {
              LogCommentFailure(CL, "Fail to get select date milestone");
              isPass = false;
          }
           res = CL.EA.DaySkipInGuide(guideView, true, 1, false, false);

           res = CL.IEX.MilestonesEPG.GetEPGInfo("selection date", out selectionDateAfterSkip);
           CL.IEX.Wait(2);
          if (!res.CommandSucceeded)
          {
              LogCommentFailure(CL, "Fail to get select date milestone after day skip");
              isPass = false;
          }
          if (selectionDate.Trim() == selectionDateAfterSkip.Trim())
          {
              LogCommentInfo(CL, "Verified grid is not skipping to next day if EIT is not available");
          }
          else
          {
              LogCommentFailure(CL, "Failed to verify grid is not skipping to next day if EIT is not available");
              isPass = false;
          }

          return isPass;
             
            
        }
    }
}

