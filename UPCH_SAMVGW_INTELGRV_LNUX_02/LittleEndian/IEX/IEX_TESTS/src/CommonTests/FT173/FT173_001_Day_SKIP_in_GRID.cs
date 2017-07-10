/// <summary>
///  Script Name : FT173_001_Day_SKIP_in_GRID.cs
///  Test Name   : FT173_001_Day_SKIP_in_GRID,
///  TEST ID     : 24700,24701,24702, 25400
///  QC Version  : 4
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : |Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using System.Globalization;

[Test("FT173_001_Day_SKIP_in_GRID")]
public class FT173_001 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static string currentDate;
    static string focusedEventDate;
    static string dateAfter15Days;
    static string mode;
    static EnumGuideViews guideView;
    static string guideviewFromTestIni;
    static EnumSurfIn enumSurfIn ;
    static Helper _helper =new Helper();
    //Test Duration
    static int testDuration = 0;
    static string timestamp;
    static string futureEventKey;
    static string reverseGrid;
    static int maxEITinReverseGrid;

    //Shared members between steps

    static string channel1;
    static string channel2;
    static string FTA_Channel;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File , Launch Service having 15 days EIT & Set mode as per Test.INI ";
    private const string STEP1_DESCRIPTION = "Step 1:Launch Grid as per View in Test.INI & Day skip to Next Day. Verify DaySkip Icon and 24 Hrs Skip has done  ";
    private const string STEP2_DESCRIPTION = "Step 2:Day Skip forward till 15th day and Verify Day Skip is Happening for all 15 Days  ";
    private const string STEP3_DESCRIPTION = "Step 3:Day Skip forward to one more day and Verify Grid is not Cyclic ";
    private const string STEP4_DESCRIPTION = "Step 4:Day Skip Rewind for 14 Days and verify grid in Day 1";
    private const string STEP5_DESCRIPTION = "Step 5:Day Skip Rewind for one more Day and verify grid is not cyclic";
    private const string STEP6_DESCRIPTION = "Step 6:Go to Service S2 and Select future event , Day Skip Rewind for one more Day & verify grid remains in same day";

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
        this.AddStep(new Step6(), STEP6_DESCRIPTION);

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
            try
            {

              
                FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");

                //Get Service from content xml 

                channel1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel1");

                channel2 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel2");

                maxEITinReverseGrid = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "MAX_DAYS_PAST_EVT"));

                // Fetch Mode from Test.INI
                guideviewFromTestIni = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "GuideView");
                mode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Mode");
                /// assign value to EnumGuideView as per value from Test INI

                switch (guideviewFromTestIni)
                {

                    case "ALL_CHANNELS":
                        {
                            guideView = EnumGuideViews.ALL_CHANNELS;
                            enumSurfIn = EnumSurfIn.Guide;
                            futureEventKey = "SELECT_RIGHT";
                            break;
                        }
                    case "ADJUST_TIMELINE":
                        {
                            guideView = EnumGuideViews.ADJUST_TIMELINE;
                            enumSurfIn = EnumSurfIn.GuideAdjustTimeline;
  							futureEventKey = "SELECT_RIGHT";
                            break;
                        }
                    case "BY_GENRE":
                        {
                            guideView = EnumGuideViews.BY_GENRE;
							futureEventKey = "SELECT_DOWN";
                            break;
                        }
                    case "SINGLE_CHANNEL":
                        {
                            guideView = EnumGuideViews.SINGLE_CHANNEL;
                            enumSurfIn = EnumSurfIn.GuideSingleChannel;
							futureEventKey = "SELECT_DOWN";
                            break;

                        }

                }

              

                // Set Mode as per Test INI

                if (mode == "FAVOURITE")
                {
                   // CL.EA.STBSettings.SetFavoriteChannelNumList("" + s1.LCN + "," + s2.LCN + "", EnumFavouriteIn.Settings);
                    res= CL.EA.STBSettings.SetFavoriteChannelNumList("" + channel1 + "," +channel2 +"", EnumFavouriteIn.Settings);

                   if (!res.CommandSucceeded)
                   {
                       LogComment(CL, "Failed to set channels as favourite");
                   }


                  res= CL.EA.ReturnToLiveViewing(false);
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
                
                res = CL.IEX.MilestonesEPG.GetEPGInfo("date", out currentDate);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to get current date from EPG Info");
                }

                // change currentdate format to selection date format

                currentDate = currentDate.Substring(0, 10).Replace("_", ".");

                // getting date after 15 days

                dateAfter15Days = DateTime.ParseExact(currentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).AddDays(14).ToString("dd.MM.yyyy");
            }
            catch(Exception ex)
            {
             FailStep(CL,"Failed in Precondition Reason is: " +ex.Message);
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

            // surf to s1 where 15 days EIT is available
            // surf to s2 from guide
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

            // verifing 24 hrs EIT
            res = CL.EA.DaySkipInGuide(guideView, true, 1, true, true);
            if(!res.CommandSucceeded)
            {
                FailStep(CL,"Failed to Verify DaySkip Icon and 24 Hrs Skip has done");
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
            // checking dayskip is happening for all 15 days
            res = CL.EA.DaySkipInGuide(guideView, true, 13, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Verify DaySkip Icon and 15 Days Skip is happening");
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

            //  checking grid is not moving to Day 1 after 15th day.
            res = CL.EA.DaySkipInGuide(guideView, true, 1, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Verify DaySkip is not cyclic");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // back to day 1 and checking reverse skip

            res = CL.EA.DaySkipInGuide(guideView, false, 14, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Verify DaySkip Icon and grid in Day1");
            }
            CL.IEX.MilestonesEPG.GetEPGInfo("selection date", out focusedEventDate); // getting selection date in grid
            if (currentDate == focusedEventDate)
            {
                LogComment(CL, "Verified Grid is in Day 1");
            }
            else
            {
                FailStep(CL, "failed to verify Grid is in Day 1");
            }


            PassStep();
        }
    }
    #endregion
    #region Step5
    [Step(5, STEP5_DESCRIPTION)]
    public class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //CL.EA.UI.Utils.GetEpgInfo("reversegrid", ref reverseGrid);
            reverseGrid = "Enabled";

            if (reverseGrid.Contains("Enabled") && (guideviewFromTestIni.Contains("ALL_CHANNELS") || guideviewFromTestIni.Contains("ADJUST_TIMELINE")))
            {
                res = CL.EA.DaySkipInGuide(EnumGuideViews.ALL_CHANNELS, false, maxEITinReverseGrid + 1, true, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail to perform day skip till last day for verifying GRID is cyclic or not");
                }

                LogCommentImportant(CL, "Verified that grid is cyclic");

            }
            else
            {
                res = CL.EA.DaySkipInGuide(guideView, false, 1, false, true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Verify DaySkip Icon and grid is not cyclic");
                }
                CL.IEX.MilestonesEPG.GetEPGInfo("selection date", out focusedEventDate); // getting selection date in grid
                if (currentDate == focusedEventDate)
                {
                    LogComment(CL, "Verified Grid is not cyclic");
                }
                else
                {
                    FailStep(CL, "Failed to verify Grid is not cyclic");
                }

            }

            PassStep();
        }
    }
    #endregion
    #region Step6
    [Step(6, STEP6_DESCRIPTION)]
    public class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //CL.EA.UI.Utils.GetEpgInfo("reversegrid", ref reverseGrid);
            reverseGrid = "Enabled";

            if (reverseGrid.Contains("Enabled") && (guideviewFromTestIni.Contains("ALL_CHANNELS")|| guideviewFromTestIni.Contains("ADJUST_TIMELINE")))
            {

                LogCommentImportant(CL, "Reverse Grid is enabled, hence skipping this step");
            }

            else
            {

                // surf to s2 from guide
                if (mode == "FAVOURITE") // if favaouriter surf to "2" else to "720"
                {
                    res = CL.EA.ChannelSurf(enumSurfIn, "2", GuideTimeline: "30 MINUTES");
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Fail to Tune to service s2");
                    }
                }
                else
                {
                    if (guideView != EnumGuideViews.BY_GENRE)
                    {
                        res = CL.EA.ChannelSurf(enumSurfIn, channel2, GuideTimeline: "30 MINUTES");
                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, "Fail to Tune to service s2");
                        }
                    }
                }

                // movin to future event
                CL.IEX.Wait(1);


                res = CL.IEX.IR.SendIR(futureEventKey, out timestamp, 2);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Fail select future event");
                }

                // performing dayskip rewind from future event.
                res = CL.EA.DaySkipInGuide(guideView, false, 1, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Verify DaySkip Icon and grid is not cyclic in Service 2");
                }

                // checking focused date and current date is same to verify grid is not cyclic
                CL.IEX.MilestonesEPG.GetEPGInfo("selection date", out focusedEventDate); // getting selection date in grid
                if (currentDate == focusedEventDate)
                {
                    LogComment(CL, "Verified Grid is not cyclic");
                }
                else
                {
                    FailStep(CL, "Failed to verify Grid is not cyclic");
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
    }
}