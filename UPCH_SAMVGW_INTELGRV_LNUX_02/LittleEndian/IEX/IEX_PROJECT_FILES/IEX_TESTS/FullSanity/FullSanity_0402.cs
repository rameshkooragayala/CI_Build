using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0402-EPG-MainMenu
public class FullSanity_0402 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FTA_2nd_Mux_1;
    static string FTA_1st_Mux_3;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0402-EPG-MainMenu
        //
        //Pre-conditions: None.
        //Based on QualityCenter test version 4.
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1:launch main menu using dedicated remote control key and select all items");
        this.AddStep(new Step2(), "Step 2:Access all the sub menu's of main menu");

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
            FTA_2nd_Mux_1 = CL.EA.GetValue("FTA_2nd_Mux_1");
            CL.IEX.LogComment("Retrieved Value From ini File: FTA_2nd_Mux_1 = " + FTA_2nd_Mux_1);

            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            CL.IEX.LogComment("Retrieved Value From ini File: FTA_1st_Mux_3 = " + FTA_1st_Mux_3);

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //select all main menu items 
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_2nd_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }



            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MAIN MENU");
            }

            SelectMenuItem("TV GUIDE/ALL CHANNELS");
            //ON DEMAND
            SelectMenuItem("STORE");
            SelectMenuItem("SEARCH");
            SelectMenuItem("TOOLBOX");
            //SelectMenuItem("FOR ME");
            SelectMenuItem("MY LIBRARY");
            SelectMenuItem("CHANNELS");
            PassStep();

        }

        void SelectMenuItem(string Item)
        {
            res = CL.IEX.MilestonesEPG.SelectMenuItem(Item);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to SelectMenuItem : " + Item);
            }
        }

    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Navigate into all main menu items 
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU/TV GUIDE/ALL CHANNELS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE/ALL CHANNELS");
            }

            //returnToLive("MAIN MENU/TV GUIDE");

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STORE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STORE");
            }
            CL.IEX.Wait(10);
            /*
            navigate("MAIN MENU/SEARCH");
            returnToLive("MAIN MENU/SEARCH");

            navigate("MAIN MENU/SETTINGS");
            returnToLive("MAIN MENU/SETTINGS");
            /*
            navigate("MAIN MENU/FOR ME");
            returnToLive("MAIN MENU/FOR ME");
            */
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MY LIBRARY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MY LIBRARY");
            }
            //returnToLive("MAIN MENU/MY LIBRARY");

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNELS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to CHANNELS");
            }
            //returnToLive("MAIN MENU/CHANNELS");

            PassStep();
        }
        void returnToLive(string from)
        {
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                //FailStep(CL,"Failed to Return To Live Viewing From: " + from+" first time",false);
                CL.IEX.Wait(3);
                res = CL.EA.ReturnToLiveViewing(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Return To Live Viewing From: " + from + " Secend Time ");
                }
            }
        }

        void navigate(string To)
        {
            res = CL.IEX.MilestonesEPG.Navigate(To);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to: " + To);
            }

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