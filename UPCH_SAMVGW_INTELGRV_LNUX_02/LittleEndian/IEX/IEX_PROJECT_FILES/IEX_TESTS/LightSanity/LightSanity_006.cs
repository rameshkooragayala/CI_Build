using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-006-EPG-Action-menu
public class LightSanity_006 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Multiple_Audio_1;
    static string FTA_Channel;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-006-EPG-Action-menu
        //Checking Action menu options.
        //Pre-conditions: Service is not the default channel (to verify Last Channel option).
        //Based on QualityCenter test version 4.
        //Variations from QualityCenter: Not checking recommendation (choosing the option leads to "information unavailale").
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Navigate Within Action Menu");

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
            Multiple_Audio_1 = CL.EA.GetValue("Multiple_Audio_1");
            FTA_Channel = CL.EA.GetValue("FTA_Channel");

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

            //Tune to a service and navigate within its action menu
            //The channel has to be without series in order to verify Confirm Recording action
            //and with multiple audio streams to verify AV settings
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Don't remove the previous ChannelSurf as it requisite for Last Channel action
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Multiple_Audio_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Check for enough time left on the event
            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 100)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec + 60);
            }

            //Navigate within the action bar
            res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to ACTION BAR");
            }

            //Navigate within the action bar
            res = CL.IEX.MilestonesEPG.SelectMenuItem("A//V SETTINGS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select Menu Item A/V SETTINGS");
            }

            if (CL.EA.Project.Name.ToUpper() != "ISTB")
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem("PAUSE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Select Menu Item PAUSE");
                }
            }

            res = CL.IEX.MilestonesEPG.SelectMenuItem("INFO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select Menu Item INFO");
            }

            if (CL.EA.Project.Name.ToUpper() == "COGECO")
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem("LAST CHANNEL");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Select Menu Item LAST CHANNEL");
                }

                res = CL.IEX.MilestonesEPG.SelectMenuItem("LOCK CHANNEL");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Select Menu Item LOCK CHANNEL");
                }
            }

            if (CL.EA.Project.Name.ToUpper() == "IPC")
            {
                res = CL.IEX.MilestonesEPG.SelectMenuItem("MAKE FAVOURITE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Select Menu Item MAKE FAVOURITE");
                }
            }


            if (CL.EA.Project.Name.ToUpper() != "ISTB")
            {
                res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR/RECORD");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navigate to ACTION BAR/RECORD");
                }
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing After Navigating in Action Bar");
            }

            PassStep();

        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}