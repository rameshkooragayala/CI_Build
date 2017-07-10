using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-1502-PC-Locke_Home_Setting_Menu
public class FullSanity_1502 : _Test
{
    [ThreadStatic]
    static _Platform CL;




    //Channels used by the test
    static string Long_HD_1;


    static string PINENTRYOFF;
    static bool isPinEntryOn = false;


    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:FullSanity-1502-PC-Locked_Channel
        //Based on QualityCenter test version 6.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Verify that home setting page is not locked");
        this.AddStep(new Step2(), "Step 2: Lock the home setting page");
        this.AddStep(new Step3(), "Step 3: Verify that home setting page is locked");
        this.AddStep(new Step4(), "Step 4: Unlock the home setting page");
        this.AddStep(new Step5(), "Step 5: Verify that home setting page is not locked");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            Long_HD_1 = CL.EA.GetValue("Long_HD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Long_HD_1 = " + Long_HD_1);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Long_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to  Channel With DCA");
            }


            


            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to SETTINGS menu");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //CL.IEX.Wait(10);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:REQUIRE PIN ENTRY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MASTER PIN menu");
            }

            res = CL.EA.EnterDeafultPIN("REQUIRE PIN ENTRY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enter PIN and get REQUIRE PIN ENTRY menu");
            }

            // select the ON option , in order to lock the settings home page 
            res = CL.IEX.MilestonesEPG.Navigate("ON");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to ON option");
            }
            isPinEntryOn = true;
            //res = CL.EA.ReturnToLiveViewing(true);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to return to live viewing");
            //}

            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:PARENTAL CONTROL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to PIN &amp; PARENTAL CONTROL  menu");
            }

            PassStep();
        }
    }
    #endregion
    #region Step4
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            /*res = CL.EA.EnterDeafultPIN("SETTINGS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to enter PIN and get  menu");
            }

            // navigate to REQUIRE PIN ENTRY page 
            res = CL.IEX.MilestonesEPG.Navigate("PIN & PARENTAL CONTROL/PIN MANAGEMENT/REQUIRE PIN ENTRY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Navigate to REQUIRE PIN ENTRY menu");
            }

            // select the OFF option , in order to unlock the settings home page 
            res = CL.IEX.MilestonesEPG.Navigate("OFF");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Navigate to OFF option");
            }*/

            unlockSettingsHomePage();
            isPinEntryOn = false;
            PassStep();
        }

        private void unlockSettingsHomePage()
        {

            res = CL.EA.EnterDeafultPIN("PIN & PARENTAL CONTROL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enter PIN and get menu");
            }

            res = CL.IEX.MilestonesEPG.SelectMenuItem("REQUIRE PIN ENTRY");

            CL.EA.UI.Utils.SendIR("SELECT");

            PINENTRYOFF = CL.EA.UI.Utils.GetValueFromDictionary("DIC_SETTINGS_OPTION_OFF");



            res = CL.IEX.MilestonesEPG.Navigate(PINENTRYOFF);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to REQUIRE PIN ENTRY menu");
            }
        }
    }
    #endregion
        #region Step5
        public class Step5 : _Step
        {
            public override void Execute()
            {
                StartStep();
                string currentState = "";
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS");
                if (!res.CommandSucceeded)
                {
                    CL.IEX.MilestonesEPG.GetActiveState(out currentState);
                    if (!currentState.Equals("ENTER PIN"))
                    {
                        FailStep(CL, res, "Failed to Navigate to SETTINGS menu because it is still locked");
                    }
                    else
                    {
                        FailStep(CL, res, "Failed to Navigate to SETTINGS menu, we get this state: " + currentState);
                    }
                }


                PassStep();
            }
        }
        #endregion
    #endregion


        #region PostExecute
        public override void PostExecute()
        {
            if (isPinEntryOn)
            {
                IEXGateway._IEXResult res;
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:PARENTAL CONTROL");
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to Navigate to PIN &amp; PARENTAL CONTROL  menu");
                }
                res = CL.EA.EnterDeafultPIN("PIN & PARENTAL CONTROL");
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL,"Failed to enter PIN and get menu");
                }

                res = CL.IEX.MilestonesEPG.SelectMenuItem("REQUIRE PIN ENTRY");

                CL.EA.UI.Utils.SendIR("SELECT");

                PINENTRYOFF = CL.EA.UI.Utils.GetValueFromDictionary("DIC_SETTINGS_OPTION_OFF");

                res = CL.IEX.MilestonesEPG.Navigate(PINENTRYOFF);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL,"Failed to Navigate to REQUIRE PIN ENTRY menu");
                }
 
            }
        }
        #endregion

    }
