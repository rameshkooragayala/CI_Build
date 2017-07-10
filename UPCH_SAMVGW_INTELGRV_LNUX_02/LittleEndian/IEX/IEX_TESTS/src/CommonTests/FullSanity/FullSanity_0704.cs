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
//  Script Name : MC-Audio-Navigator
//  Source test name : FullSanity-0704-LIB-MC-Audio-Navigator
//  MQC Project : FR_FUSION \ UPC 
//  TEST ID : 17351
//  Variation from Quality Center : None 
// -----------------------------------------------
//  Scripted by : Varsha Deshpande
//  Last modified : 14  May 2013
// -----------------------------------------------
public class FullSanity_0704 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    //Channels used by the test
    static string FTA_Channel;

    //Shared members between steps
    static string AudioFile = "";

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:
        //Pre-conditions:
        //Based on QualityCenter test version 
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Navigate to MyDevices");
        this.AddStep(new Step2(), "Step 2: Select Audio file to be played");
        this.AddStep(new Step3(), "Step 3: Play audio file");


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
            AudioFile = CL.EA.GetValue("AudioFile");

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
            //Select Music file to be played
            StartStep();

            bool res2 = CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("Music");
            if (!res2)
            {
                FailStep(CL, "Failed to navigate to Music");
            }

            CL.EA.UI.Utils.SendIR("SELECT");

            res2 = CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("All Music");
            CL.EA.UI.Utils.SendIR("SELECT");
            if (!res2)
            {
                FailStep(CL, "Failed to navigate to All Music");
            }


            string CurrentAudioFile;
            string FirstAudioFile;
            string timeStamp = "";

            res = CL.IEX.IR.SendIR("SELECT_DOWN", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT_DOWN");
            }
            CL.IEX.Wait(3);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out FirstAudioFile);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get audio file Name");
            }

            if (!AudioFile.Equals(FirstAudioFile))
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

                    res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out CurrentAudioFile);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Get audio file Name");
                    }

                    if (AudioFile.Equals(CurrentAudioFile))
                    {
                        break;
                    }
                } while (!CurrentAudioFile.Equals(FirstAudioFile));
            }

            //Verify if the item is of type Music content
            string audioContent = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("object", out audioContent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get audio content object");
            }

            if (audioContent != "[object AudioContent]")
            {
                FailStep(CL, "Targeted content is not audio object");
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
                FailStep(CL, "Targeted audio content is not playable");
            }

            //Select the item
            CL.IEX.Wait(3);
            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT");
            }

            //Verify if the selected file is same as audio file
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out CurrentAudioFile);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get audio file Name");
            }

            if (!AudioFile.Equals(CurrentAudioFile))
            {
                FailStep(CL, res, "Selected file name is not same as targeted file name");
            }

            //Play Video file
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
            //Check for audio and End of File
            StartStep();
            res = CL.EA.CheckForAudio(true, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Audio is Present On Music content");
            }

            //Verify State:MUSIC LIBRARY state
            bool resBool = CL.EA.UI.Utils.VerifyState("MUSIC LIBRARY");
            if (!resBool)
            {
                FailStep(CL, "Failed to verify MUSIC LIBRARY state");
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



