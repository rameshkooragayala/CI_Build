/// <summary>
///  Script Name : SCRSAV_1203_Exit_ Screen_Saver.cs
///  Test Name   : SCRSAV_1203_Exit_ Screen_Saver
///  TEST ID     : 8905
///  QC Version  : 2
///  Variations from QC:none
/// QC Repository : UPC/FR_FUSION
/// ----------------------------------------------- 
///  Modified by : Mithlesh Kumar
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("SCRSAV_1203")]
public class SCRSAV_1203 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static Service PCLockedChannel;
    static Service service;
    static string ScreenSaver = "";
    static string screenSaverWaitTime = "";
    private static string backKeyName = "";
    private static string timeStamp = "";
    static bool isRFActive = true;
    static string rfSwitch;//Whether it is A or B

    //Variables which are used in different steps

    //Shared members between steps
    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch list of Service from Test.INI & Project.INI file. ";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to PCLocked event : User remains inactive for 90 min & CPE Device will present embeded screen saver , After that Exit from screensaver.";
    private const string STEP2_DESCRIPTION = "Step 2: Tune to TV Guide , user remains inactive for 90 min on guide screen & CPE Device will present embeded screen saver";
    private const string STEP3_DESCRIPTION = "Step 3: User remains inactive for atleast 90 minutes on any error message like & after 90 min CPE Device will present embeded screen saver";

    private static class Constants
    {
        public const int totalDurationInSecond = 30;
        public const int bufferdurationInSecond = 2;

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

            //Get channel number content xml file
            service = CL.EA.GetServiceFromContentXML("Type=Video;IsMinEventDuration=True;IsEITAvailable=True", "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch retrieve channel from Content.xml for the passed criterion");
            }
            else
            {
                LogCommentInfo(CL, "Channel fetched from Content.xml: " + service.LCN);
            }

            //Get the PCLocked Channels from xml File
            PCLockedChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;ParentalRating=High", "");

            if (PCLockedChannel == null)
            {
                FailStep(CL, "Failed to fetch lockedChannel from Content.xml for the passed criterion");
            }
            else
            {
                LogCommentInfo(CL, "lockedChannel fetched from Content.xml: " + PCLockedChannel.LCN);
            }

            //Get EPG_INACTIVE_SCREENSAVER_WAIT from Project ini           
            screenSaverWaitTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "EPG_INACTIVE_SCREENSAVER_WAIT"); //5400 seconds
            if (screenSaverWaitTime == "")
            {
                FailStep(CL, "Failed to fetch the EPG_INACTIVE_SCREENSAVER_WAIT value from Project ini :" + screenSaverWaitTime);
            }

            rfSwitch = CL.EA.GetValueFromINI(EnumINIFile.Project, "RF_SWITCH", "RF_SWITCH");
            if (rfSwitch == "")
            {
                FailStep(CL, "RF switch is not defined in the Test ini file");
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

            res = CL.EA.TuneToChannel(PCLockedChannel.LCN, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a locked Channel");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            //loop for every 9 mins 58 seconds to check the screen Saver milestone
            for (int i = 0; i < 9; i++)
            {
                LogCommentInfo(CL, "User remains inactive for " + ((Convert.ToInt32(screenSaverWaitTime) / 9) - Constants.bufferdurationInSecond));
                res = CL.IEX.Wait(Convert.ToInt32(screenSaverWaitTime) / 9 - Constants.bufferdurationInSecond);
                if (!res.CommandSucceeded)
                {

                    FailStep(CL, res, "Failed to wait for : " + ((Convert.ToInt32(screenSaverWaitTime) / 9) - Constants.bufferdurationInSecond));
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("ScreenSaver", out ScreenSaver);
                if (!res.CommandSucceeded)
                {
                    //the below condition is to Handle : "sometimes the ScreenSaver doesn't exist in the EPG info dictionary B'coz we are trying to getEPG beofre 90 mins."
                    if (res.FailureReason.Contains("doesn't exist in the EPG info dictionary"))
                    {
                        LogCommentInfo(CL, "Screen Saver do not appear on the lock channel before 90 mins");
                    }
                    else if (ScreenSaver == "")
                    {
                        LogCommentInfo(CL, "Screen Saver do not appear on the lock channel before 90 mins");
                    }
                    else
                    {
                        FailStep(CL, res, "Screen Saver appear on the lock channel before 90 mins.");

                    }
                }
                else
                {
                    if (ScreenSaver == "ON")
                    {
                        FailStep(CL, res, "Screen Saver appear on the lock channel before 90 mins.");
                    }
                    else
                    {
                        LogCommentInfo(CL, "Screen Saver appear do not appear on the lock channel before 90 mins");
                    }

                }

            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            LogCommentInfo(CL, "Wait : for some more time so that user remains inactive for more than 90 mins and Screen Saver should appear on PCLockedChannel");
            res = CL.IEX.Wait(Constants.totalDurationInSecond * 4);
            if (!res.CommandSucceeded)
            {

                FailStep(CL, res, "Failed to wait");
            }

            CL.EA.UI.Utils.GetEpgInfo("ScreenSaver", ref ScreenSaver);
            if (ScreenSaver != "ON")
            {
                FailStep(CL, res, "Screen Saver not appeared on the PClocked channel when the User remains inactive for 90 mins");
            }
            else
            {
                LogCommentInfo(CL, "Screen Saver appeared on PClocked channel when the User remains inactive for 90 mins.");

            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            LogCommentInfo(CL, "Exit from screensaver by pressing any key of the STB remote control, except the power/standby key.");


            res = CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to press 'Retour' to go back .");
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

            //Tune to service 720
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + service.LCN);
            }

            //Navigating to TV guide
            LogCommentInfo(CL, " Navigate to TV guide on the service :" + service.LCN);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            for (int i = 0; i < 9; i++)
            {
                LogCommentInfo(CL, "User remains inactive for " + ((Convert.ToInt32(screenSaverWaitTime) / 9) - Constants.bufferdurationInSecond));
                res = CL.IEX.Wait(Convert.ToInt32(screenSaverWaitTime) / 9 - Constants.bufferdurationInSecond);
                if (!res.CommandSucceeded)
                {

                    FailStep(CL, res, "Failed to wait for : " + ((Convert.ToInt32(screenSaverWaitTime) / 9) - Constants.bufferdurationInSecond));
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("ScreenSaver", out ScreenSaver);
                if (!res.CommandSucceeded)
                {
                    //the below condition is to Handle : "sometimes the ScreenSaver doesn't exist in the EPG info dictionary B'coz we are trying to getEPG beofre 90 mins."
                    if (res.FailureReason.Contains("doesn't exist in the EPG info dictionary"))
                    {
                        LogCommentInfo(CL, "Screen Saver do not appear on the lock channel before 90 mins");
                    }
                    else if (ScreenSaver == "")
                    {
                        LogCommentInfo(CL, "Screen Saver do not appear on the lock channel before 90 mins");
                    }
                    else
                    {
                        FailStep(CL, res, "Screen Saver appear on the lock channel before 90 mins.");

                    }
                }
                else
                {
                    if (ScreenSaver == "ON")
                    {
                        FailStep(CL, res, "Screen Saver appear on the TV guide before 90 mins.");
                    }
                    else
                    {
                        LogCommentInfo(CL, "Screen Saver appear do not appear on the TV guide before 90 mins");
                    }

                }
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            LogCommentInfo(CL, "Wait : for some more time so that user remains inactive for more than 90 mins and Screen Saver should appear on TV guide");
            res = CL.IEX.Wait(Constants.totalDurationInSecond * 4);
            if (!res.CommandSucceeded)
            {

                FailStep(CL, res, "Failed to wait");
            }

            CL.EA.UI.Utils.GetEpgInfo("ScreenSaver", ref ScreenSaver);
            if (ScreenSaver != "ON")
            {
                FailStep(CL, res, "Screen Saver not appeared on the TV GUIDE when the User remains inactive for 90 mins");
            }
            else
            {
                LogCommentInfo(CL, "Screen Saver appeared on TV GUIDE when the User remains inactive for 90 mins.");

            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            LogCommentInfo(CL, "Exit from screensaver by pressing any key of the STB remote control, except the power or standby key.");

            res = CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to press 'Retour' to go back");
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

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Turn Off RF Switch
            LogCommentInfo(CL, "Turn Off RF Switch");
            res = CL.IEX.RF.TurnOff(instanceName: "1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unplug RF signal!");
            }
            isRFActive = false;


            //Wait till the error pop up is launched
            LogCommentInfo(CL, "Wait till the Error pop is launched and to recieve the Milestone");
            CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait for 10 seconds");
            }

            string ErrorMessage = "";
            CL.EA.UI.Utils.GetEpgInfo("state", ref ErrorMessage);
            if (ErrorMessage != "ErrorMessageState")
            {
                FailStep(CL, res, "Failed to Recieve the ErrorMessage after unplug RF signal!");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            for (int i = 0; i < 9; i++)
            {
                LogCommentInfo(CL, "User remains inactive for " + ((Convert.ToInt32(screenSaverWaitTime) / 9) - Constants.bufferdurationInSecond));
                res = CL.IEX.Wait(Convert.ToInt32(screenSaverWaitTime) / 9 - Constants.bufferdurationInSecond);
                if (!res.CommandSucceeded)
                {

                    FailStep(CL, res, "Failed to wait for : " + ((Convert.ToInt32(screenSaverWaitTime) / 9) - Constants.bufferdurationInSecond));
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("ScreenSaver", out ScreenSaver);
                if (!res.CommandSucceeded)
                {
                    //the below condition is to Handle : "sometimes the ScreenSaver doesn't exist in the EPG info dictionary B'coz we are trying to getEPG beofre 90 mins."
                    if (res.FailureReason.Contains("doesn't exist in the EPG info dictionary"))
                    {
                        LogCommentInfo(CL, "Screen Saver do not appear on the lock channel before 90 mins");
                    }
                    else if (ScreenSaver == "")
                    {
                        LogCommentInfo(CL, "Screen Saver do not appear on the lock channel before 90 mins");
                    }
                    else
                    {
                        FailStep(CL, res, "Screen Saver appear on the lock channel before 90 mins.");

                    }
                }
                else
                {
                    if (ScreenSaver == "ON")
                    {
                        FailStep(CL, res, "Screen Saver appear on the ErrorMessage Screen before 90 mins.");
                    }
                    else
                    {
                        LogCommentInfo(CL, "Screen Saver appear do not appear on the ErrorMessage Screen before 90 mins");
                    }

                }
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();
            LogCommentInfo(CL, "Wait : for some more time so that user remains inactive for more than 90 mins and Screen Saver should appear on ErrorMessage Screen");
            res = CL.IEX.Wait(Constants.totalDurationInSecond * 4);
            if (!res.CommandSucceeded)
            {

                FailStep(CL, res, "Failed to wait");
            }

            CL.EA.UI.Utils.GetEpgInfo("ScreenSaver", ref ScreenSaver);
            if (ScreenSaver != "ON")
            {
                FailStep(CL, res, "Screen Saver not appeared on 'ErrorMessage Screen (ATTENTION)' when the User remains inactive for 90 mins");
            }
            else
            {
                LogCommentInfo(CL, "Screen Saver appeared on 'ErrorMessage Screen (ATTENTION)' when the User remains inactive for 90 mins.");

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

        //Restore default settings
        IEXGateway._IEXResult res;
        //Plug back RF if there was failure in reconnection
        if (!isRFActive)
        {
            //Connecting the RF Signal
            if (rfSwitch.Equals("A"))
            {
                res = CL.IEX.RF.ConnectToA(instanceName: "1");
            }
            else
            {
                res = CL.IEX.RF.ConnectToB(instanceName: "1");
            }
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to plug back RF signal!");
            }
        }

    }
    #endregion
}