using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0802-EPG-MainMenu
public class FullSanity_0802 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-802-Navigate in the main menu Bar 
        //Check all the main menu items can be navigated
        //Pre-conditions: none
        // TODO - Have to add more menu options from LS_802
        this.AddStep(new PreCondition(), "Precondition: Reboot the box to tune to start channel/Box is in Active state");
        this.AddStep(new Step1(), "Step 1:Navigate to MAIN MENU & Check all the Menu Items ");
        this.AddStep(new Step2(), "Step 2:Navigate to Settings Menu and Change the INFO Menu time out");
        this.AddStep(new Step3(), "Step 3:Navigate to Guide");

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

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Step 1:Navigate & Check Start Channel Menu        
        public override void Execute()
        {

            string[] main_menu = new string[] { "MY LIBRARY", "ON DEMAND", "TOOLBOX", "SEARCH", "TV GUIDE", "CHANNELS" };
            int i = 0;

            StartStep();
            CL.IEX.LogComment("main_menu.Length is :" + main_menu.Length);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to STATE:MAIN MENU");
            }

            // Loop thru menu items 
            CL.IEX.LogComment("Loop thru all the menu items");
            for (i = 0; i < main_menu.Length; i++)
            {
                CL.IEX.LogComment("main_menu[i]) is :" + main_menu[i]);
                SelectMenuItem(main_menu[i]);
            }

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
        public override void Execute()
        {
            StartStep();

            // Change the channel banner timeout to 5 sec
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
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

            //Navigate To grid
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }

            CL.EA.ReturnToLiveViewing();

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