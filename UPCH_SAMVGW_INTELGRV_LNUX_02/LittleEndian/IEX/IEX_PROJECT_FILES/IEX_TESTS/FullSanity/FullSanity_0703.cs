using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

// -----------------------------------------------
//  Script Name : MC-Photo-Navigator
//  Source test name : FullSanity-0703-LIB-MC-Photo-Navigator
//  MQC Project : FR_FUSION \ UPC 
//  TEST ID : 17350
//  Variation from Quality Center : None 
// -----------------------------------------------
//  Scripted by : Varsha Deshpande
//  Last modified : 14  May 2013
// -----------------------------------------------
public class FullSanity_0703 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    //Channels used by the test
    static string FTA_Channel;

    //Shared members between steps
    static string PictureFile = "";

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:
        //Pre-conditions:
        //Based on QualityCenter test version 
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Navigate to MY Devices");
        this.AddStep(new Step2(), "Step 2: Select Pictiure to be displayed");
        this.AddStep(new Step3(), "Step 3: View  a Picture");


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
            PictureFile = CL.EA.GetValue("PictureFile");

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
            //res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MY DEVICES");
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL,"Failed to Navigate to My Devices");
            //}

            CL.EA.UI.Utils.ClearEPGInfo();
            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY");

            if (CL.EA.UI.Menu.IsLibraryNoContent())
            {
                CL.EA.UI.Utils.EPG_Milestones_Navigate("LIBRARY ERROR/OK/MY DEVICES");
            }
            else
            {
                CL.EA.UI.Utils.EPG_Milestones_Navigate("MY DEVICES");
            }

            string timeStamp = "";
            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Send IR command Select");
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

            //Select Picture to be displayed
            StartStep();

            bool res2 = CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("Picture");
            if (!res2)
            {
                FailStep(CL, "Failed to navigate to Picture");
            }
            CL.EA.UI.Utils.SendIR("SELECT");


            res2 = CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("All Pictures");
            CL.EA.UI.Utils.SendIR("SELECT");
            if (!res2)
            {
                FailStep(CL, "Failed to navigate to All Pictures");
            }



            string CurrentPictureFile;
            string FirstPictureFile;
            string timeStamp = "";

            res = CL.IEX.IR.SendIR("SELECT_DOWN", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT_DOWN");
            }
            CL.IEX.Wait(3);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out FirstPictureFile);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get PICTURE file Name");
            }

            if (!PictureFile.Equals(FirstPictureFile))
            {
                do
                {
                    CL.IEX.Wait(3);
                    res = CL.IEX.IR.SendIR("SELECT_UP", out timeStamp, 3);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Send IR Key SELECT_UP");
                    }
                    CL.IEX.Wait(3);

                    res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out CurrentPictureFile);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Get Picture file Name");
                    }

                    if (PictureFile.Equals(CurrentPictureFile))
                    {
                        break;
                    }
                } while (!CurrentPictureFile.Equals(FirstPictureFile));
            }

            //Verify if the item is of type picture content
            string pictureContent = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("object", out pictureContent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get picture content object");
            }

            if (pictureContent != "[object PictureContent]")
            {
                FailStep(CL, "Targeted content is not picture object");
            }


            //Verify is the item is playable
            string isPlayable = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("isPlayable", out isPlayable);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get isPlayable");
            }

            if (isPlayable != "true")
            {
                FailStep(CL, "Targeted picture content is not playable");
            }

            //Select the item
            CL.IEX.Wait(3);
            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT");
            }

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

            bool res_2 = CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("VIEW");
            CL.EA.UI.Utils.SendIR("SELECT");

            //Verify State:Slide Show state
            bool resBool = CL.EA.UI.Utils.VerifyState("SLIDE SHOW");
            if (!resBool)
            {
                FailStep(CL, "Failed to verify SLIDE SHOW state");
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