using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-007-EPG-programme-grid
public class LightSanity_007 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FTA_Channel;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-007-EPG-programme-grid
        //Checking grid options.
        //Pre-conditions: None.
        //Based on QualityCenter test version 4.
        //Variations from QualityCenter: Currently we don't have a milestone that checks adjust time line option.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Navigate in Grid Options");
        this.AddStep(new Step2(), "Step 2: Navigate into All Channels Grid and Tune to a Channel from Grid");
        this.AddStep(new Step3(), "Step 3: Navigate into Single Channel View and Validate that Viewed Channel is Displayed in Grid");

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


            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Navigate To grid
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }
            //Warning  !!!!!
            //Wait for Guide preformens!!!!!!!!!
            CL.IEX.Wait(10);

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
            //Check that pip is displayed on 'all channels grid' and tune to that channel
            if (CL.EA.Project.Name.ToUpper() == "COGECO")
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", true, 1, EnumPredicted.NotPredictedWithoutPIP, true);
            }
            else
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", true, 1, EnumPredicted.NotPredicted, true);
            }
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune from Grid");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //TODO: surf events in grid make sure that event name is higlighted and true..

            //Load the action menu
            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/LIVE/ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MAIN MENU/LIVE/ACTION BAR");
            }

            //Check program name in action menu
            string eventNameActionMenu = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventNameActionMenu);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from Action Menu");
            }


            //Navigate To grid
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }


            //Check program name in guide 
            string eventNameGuide = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventNameGuide);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from Action Menu");
            }


            //Compre the first 10 chars of the names (Handle nun full names)
            if ((eventNameActionMenu.Length > 10) && (eventNameGuide.Length > 10))
            {
                if (!(eventNameGuide.Substring(0, 10).Equals(eventNameActionMenu.Substring(0, 10))))
                {
                    FailStep(CL, "Failed: Event Name is Different Between Channel Bar and Action Menu");
                }
            }
            else
            {
                if (!(eventNameGuide.Equals(eventNameActionMenu)))
                {
                    FailStep(CL, "Failed: Event Name is Different Between Channel Bar and Action Menu");
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

    }
    #endregion
}