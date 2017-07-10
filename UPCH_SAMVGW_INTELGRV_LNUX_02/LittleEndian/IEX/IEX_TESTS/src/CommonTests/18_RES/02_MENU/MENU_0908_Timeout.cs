/// <summary>
///  Script Name : MENU_0908_Timeout.cs
///  Test Name   : EPG-0908-Main Menu-Timeout
///  TEST ID     : 64455
///  JIRA ID     : FC-382
///  QC Version  : 1
///  Variations from QC: NONE
/// -----------------------------------------------
///  Modified by : Ganpat Singh
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("MENU_0908")]
public class MENU_0908 : _Test
{
    [ThreadStatic]
    private static _Platform CL, GW;

    //Shared members between steps
    static string screenSaverWaitTime = "";
    static string ScreenSaver = "";
    static string defaultPin = "";
    static bool ishomenet = false;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Verifing that after first installation Main menu will be launched and it should not time out  ";
    private const string STEP2_DESCRIPTION = "Step 2: Verifying that Main Menu should disappear when BACK/MENU key is pressed and Main menu should not be displayed during wake up from HOT stand by";
    private const string STEP3_DESCRIPTION = "Step 3: Screensaver will be launched after 90 mins of inactivity on main menu";

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
        string isHomeNetwork = CL.EA.GetTestParams("IsHomeNetwork");

        //If Home network is true perform GetGateway
        ishomenet = Convert.ToBoolean(isHomeNetwork);
        if (ishomenet)
        {
            //Get gateway platform
            GW = GetGateway();
        }
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get EPG_INACTIVE_SCREENSAVER_WAIT from Project ini           
            screenSaverWaitTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "EPG_INACTIVE_SCREENSAVER_WAIT"); //5400 seconds
            if (screenSaverWaitTime == "")
            {
                FailStep(CL, "Failed to fetch the EPG_INACTIVE_SCREENSAVER_WAIT value from Project ini :" + screenSaverWaitTime);
            }

            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }


            //Perform Factory reset with Keep Recording option Yes
            res = CL.EA.STBSettings.FactoryReset(false,false, "0000");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform factory reset");
            }

            if (ishomenet)
            {
                res = GW.EA.MountGw(EnumMountAs.FACTORY_RESET, IsReturnToLive: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }
                res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET, IsReturnToLive: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount client");
                }
            }
            else
            {
                res = CL.EA.MountGw(EnumMountAs.FACTORY_RESET, IsReturnToLive:false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount cleint");
                }
            }


            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Verify channel bar time out on Launching MainMenu
            if (!CL.EA.UI.Utils.VerifyState("MAIN MENU", 2))
            {
                FailStep(CL, res, "Failed to verify MAIN MENU");
            }


            if (CL.EA.UI.Utils.VerifyState("Live", 120))
            {
                FailStep(CL, res, "Failed to verify that main menu is not timed out");
            }
            else
            {
                LogCommentImportant(CL, "Main Menu is not timed out");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.EA.UI.Utils.SendIR("MENU");
            if (!CL.EA.UI.Utils.VerifyState("LIVE", 4))
            {
                FailStep(CL, res, "Failed to verify LIVE");
            }

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to StandBy");
            }
            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for 10 sec");
            }
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to come up from StandBy");
            }

            //Verifying that main menu is not launched after standby wakeup
            if (CL.EA.UI.Utils.VerifyState("MAIN MENU", 10))
            {
                FailStep(CL, res, "Failed to verify LIVE");
            }

            else
            {
                LogCommentImportant(CL, "Main Menu is not launched after wakeup from hot standby");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
			
			String titleBeforeScrsaver = "";
            String titleAfterScrsaver = "";
            //navigate to Main Menu
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:MAIN MENU");
            if (!CL.EA.UI.Utils.VerifyState("MAIN MENU", 2))
            {
                FailStep(CL, res, "Failed to verify MAIN MENU");
            }

			//Focused item before scrsaver
			CL.EA.UI.Utils.GetEpgInfo("title", ref titleBeforeScrsaver);
			
            //Wait for ScreenSaverWaitTime
            res = CL.IEX.Wait((Convert.ToInt32(screenSaverWaitTime))+60);
            if (!res.CommandSucceeded)
            {

                FailStep(CL, res, "Failed to wait for : " + (Convert.ToInt32(screenSaverWaitTime)));
            }

            //Verifying that screen saver is ON
            res = CL.IEX.MilestonesEPG.GetEPGInfo("ScreenSaver", out ScreenSaver);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get screen saver value from Epg info dictionary");
            }
            if (ScreenSaver == "ON")
            {
                LogCommentImportant(CL, "Screen Saver appeared on the video service");
            }
            else
            {
                FailStep(CL, res, "Screen Saver is not appeared on the video service");
            }
						
            CL.EA.UI.Utils.SendIR("RETOUR");

            if (!CL.EA.UI.Utils.VerifyState("MAIN MENU", 4))
            {
                FailStep(CL, res, "Failed to verify MAIN MENU");
            }

            CL.EA.UI.Utils.GetEpgInfo("title", ref titleAfterScrsaver);

            if (titleBeforeScrsaver.Contains(titleAfterScrsaver))
            {
                LogCommentImportant(CL, "Focused item is same as " + titleBeforeScrsaver + " after and before Screensaver ");
            }
            else
            {
                FailStep(CL, "Focused item after Screen saver is "+ titleAfterScrsaver + " instead of "+ titleBeforeScrsaver);
            }
            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}