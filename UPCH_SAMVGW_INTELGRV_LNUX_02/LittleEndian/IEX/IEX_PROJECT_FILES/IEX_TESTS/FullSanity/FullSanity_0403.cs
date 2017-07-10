using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0403-change recording settings
public class FullSanity_0403 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    
    static string selected_option;


    #region Create Structure
    public override void CreateStructure()
    {
        //Brief Description: FullSanity-0403-EPG-recording menu
        // Change one of the Recording Settings , in Settings menus. 
        // Check that the change was saved
        //Pre-conditions: None.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: 
        this.AddStep(new Step1(), "Step 1:Navigate to Preferences and validate the options");
        this.AddStep(new Step2(), "Step 2:Validate the default values");
        this.AddStep(new Step3(), "Step 3:Change the values and validate for SGT ");
        this.AddStep(new Step4(), "Step 4:Change the values and validate for EGT");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region Step1
    private class Step1 : _Step
    {
        //Navigate & validate recordings menu items
        public override void Execute()
        {
            StartStep();

            //Navigate to preferences
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:PREFERENCES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TIME BEFORE PROGRAMME");
            }
            SelectMenuItem("EXTRA TIME BEFORE PROGRAMME");
            SelectMenuItem("DISK SPACE MANAGEMENT");
            SelectMenuItem("SERIES RECORDING");
            SelectMenuItem("EXTRA TIME AFTER PROGRAMME");

            PassStep();

        }
     
        void SelectMenuItem(string Item)
        {
        res = CL.IEX.MilestonesEPG.SelectMenuItem(Item);

    }
   }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //change SGT value
        public override void Execute()
        {
            StartStep();

            verifyValues("STATE:EXTRA TIME BEFORE PROGRAMME", "AUTOMATIC");
            verifyValues("STATE:EXTRA TIME AFTER PROGRAMME", "AUTOMATIC");
            verifyValues("STATE:SERIES RECORDING", "FROM SINGLE CHANNEL");
            verifyValues("STATE:DISK SPACE MANAGEMENT", "AUTO DELETE RECORDINGS");

            PassStep();
        }
        void verifyValues(string state,string expected_value)
        {
            res = CL.IEX.MilestonesEPG.NavigateByName(state);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TIME BEFORE PROGRAMME");
            }

            String obtained_title = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtained_title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get title from TIME BEFORE PROGRAMME");
            }

            if (obtained_title == expected_value)
            {
                LogCommentInfo(CL, "Default value"+expected_value +"is set");
            }

        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //return to live viewing
        public override void Execute()
        {
            StartStep();

            //Validate SGT
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TIME BEFORE PROGRAMME");
            }
            res = CL.IEX.MilestonesEPG.Navigate("NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to naviagate to NONE ");
            }

            //Navigate to check the changes
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME BEFORE PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TIME BEFORE PROGRAMME");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selected_option);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the selected value in TIME BEFORE PROGRAMME menu");
            }

            if (selected_option == "AUTOMATIC")
            {
                FailStep(CL, res, "Failed to select the different option SGT");
            }
            else
            {
                LogComment(CL, "Selected option "+selected_option+ "is set successfully");
            }
            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        //Navigate into the menu and validate that the value is still chosen 
        public override void Execute()
        {
            StartStep();

            //Validate EGT
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TIME BEFORE PROGRAMME");
            }
            res = CL.IEX.MilestonesEPG.Navigate("NONE");
             if (!res.CommandSucceeded)
            {
                 FailStep(CL, res, "Failed to naviagate to NONE ");
            }

             res = CL.IEX.MilestonesEPG.NavigateByName("STATE:EXTRA TIME AFTER PROGRAMME");
             if (!res.CommandSucceeded)
             {
                 FailStep(CL, res, "Failed to Navigate to TIME BEFORE PROGRAMME");
             }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selected_option);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the selected value in TIME BEFORE PROGRAMME menu");
            }
            
            if (selected_option == "AUTOMATIC")
            {
                FailStep(CL, res, "Failed to select the different option SGT");
            }
            else
            {
                LogComment(CL, "Selected option" + selected_option + "is set successfully");
            }
            PassStep();
        }
    }
    #endregion
    #endregion

  
    public override void PostExecute()
    {

    }
   
}