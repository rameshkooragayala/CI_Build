/// <summary>
///  Script Name : FT173_007_DaySkip_In_Specific_Condition.cs
///  Test Name   : FT173-007-DaySkip-In-Specific-Condition
///  TEST ID     : 24703
///  QC Version  : 6
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

[Test("FT173_007_DaySkip_In_Specific_Condition")]
public class FT173_007 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

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
    static Service S1;
    static string channel2;
    static string evtName;
    static string evtNameafterskip;
    static string timestamp;
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel,Mode,Condition from Test INI & Launch Channel";
    private const string STEP1_DESCRIPTION = "Step 1:Perform 1 day Day Skip and check event Name as per specific condition ";
    private const string STEP2_DESCRIPTION = "Step 2:Perform Day Skip till 15th day and check Event Name as per Specific Condition ";
    private const string STEP3_DESCRIPTION = "Step 3:Perform Navigation between Events and check Navgation is not happening ";
    private const string STEP4_DESCRIPTION = "Step 4:Perform 14 days reverse Day Skip and check Event Name in Day 1 ";

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

            try
            {
                FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel); // surfing to FTA Channel
                
                
                //Get Service from tEST ini

                channel1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel1");
                             

                channel2 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel2");

                S1 = CL.EA.GetServiceFromContentXML("LCN=" + channel1, ""); // to get channel name for set lock channel
                if (S1 == null)
                {
                    FailStep(CL, "Failed to fetch channel from Content.xml");
                }
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

                //Fetch CONDITION from Test INI
                condition = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "CONDITION");
                if (condition == "")
                {
                    FailStep(CL, "Fail to fetch Mode from Test INI");
                }

                ////////////////change test params

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
				
				 if (condition == "LOCKED") // if condition is locked make channel 1 as locked channel
                {
                   
                    res = CL.EA.STBSettings.SetLockChannel(S1.Name);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Fail to set Lock Channel");
                    }

                    CL.IEX.Wait(2);
                   
                }

                if (mode == "FAVOURITE") // if favaouriter surf to "2" else to "720"
                {
                    res = CL.EA.ChannelSurf(EnumSurfIn.Live, "2");
                    CL.IEX.Wait(2);
                    res = CL.EA.ChannelSurf(EnumSurfIn.Live, "1");
                     if (condition != "LOCKED")
                    {
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Fail to Tune to service s1");
                     }
                    }
                }
                else
                {
                     res = CL.EA.ChannelSurf(EnumSurfIn.Live, channel2);
                    CL.IEX.Wait(2);
                    res = CL.EA.ChannelSurf(EnumSurfIn.Live, channel1);
                    if(condition!="LOCKED")
                    {
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Fail to Tune to service s1");
                    }
                    }
                }

                // Set condition as per Test INI


              
            }
            catch (Exception ex)
            {
                FailStep(CL, "Fail in precondition. Reason: " + ex.Message);
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
            res = CL.EA.DaySkipInGuide(guideView, true,1, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform Day Skip");
            }

            CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
            CL.IEX.Wait(1);

            if (condition == "LOCKED") // if locked event name will be locked channel
            {
                if (evtName.ToUpper() == "LOCKED CHANNEL")
                {
                    LogCommentInfo(CL, "Verified Locked Channel");
                }
                else
                {
                    FailStep(CL, "Failed - Not a Locked Channel");

                }
            }
            else
            {

                if (evtName == "No programme information available") // if no eit event name will be No programme information available
                {
                    LogCommentInfo(CL, "Verified event not information available");
                }
                else
                {
                    FailStep(CL, "Failed - Event information is  available");

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
           
            _helper.dayskip(true,14); // doing 14 days forward dayskip
                      

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
            CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
            CL.IEX.Wait(1);
            res = CL.IEX.IR.SendIR("SELECT_RIGHT", out timestamp, 2); // moving to next event

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail select future event");
            }

            CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtNameafterskip);
            CL.IEX.Wait(1);

            if (evtName == evtNameafterskip) // checking event has not changed
            {
                LogCommentInfo(CL,"Verifed Event name is not changing after clicking right key");
            }
            else
            {
                 FailStep(CL, "Event name is  changing after clicking right key");
            }


            res = CL.IEX.IR.SendIR("SELECT_LEFT", out timestamp, 2); // moving to prev event

            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail select future event");
            }

            CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtNameafterskip);
            CL.IEX.Wait(1);

            if (evtName == evtNameafterskip) // checking evt name has not changed
            {
                LogCommentInfo(CL, "Verifed Event name is not changing after clicking left key");
            }
            else
            {
                FailStep(CL, "Event name is  changing after clicking left key");
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
            _helper.dayskip(false, 15); // doing reverse day skip  till day 1

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

        if (condition == "LOCKED")
        {
             CL.EA.STBSettings.SetUnLockChannel(S1.Name);
           
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

        public void dayskip(bool isForward,int noOfDays) //or checking grid is showing "No Program information"  if no EIT 
        {
            for (int i = 0; i < noOfDays; i++)
            {
                if (isForward)
                {
                    res = CL.EA.DaySkipInGuide(guideView, true, 1, false, true);
                }
                else
                {
                    res = CL.EA.DaySkipInGuide(guideView, false, 1, false, true);
                }

                CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);

				CL.IEX.Wait(4);
				 
                if (condition == "LOCKED")
                {
                    if (evtName.ToUpper() == "LOCKED CHANNEL")
                    {
                        LogCommentInfo(CL, "Verified Locked Channel");
                    }
                    else
                    {
                        FailStep(CL, "Failed - Not a Locked Channel");

                    }
                }
                
                else
                {

                    if (evtName == "No programme information available")
                    {
                        LogCommentInfo(CL, "Verified event not information available");
                    }
                    else
                    {
                        FailStep(CL, "Failed - Event information is  available");

                    }
                }
            }
        }

    
    }
}