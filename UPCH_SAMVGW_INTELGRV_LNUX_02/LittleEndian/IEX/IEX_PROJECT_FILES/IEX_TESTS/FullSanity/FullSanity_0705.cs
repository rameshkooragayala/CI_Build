using IEX.Tests.Engine;
using IEX.Tests.Utils;
using System.Collections;
using System;


// -----------------------------------------------
//  Script Name : MC-Video-Navigator
//  Source test name : FullSanity-0705-LIB-MC-Video-Navigator
//  MQC Project : FR_FUSION \ UPC 
//  TEST ID : 17352
//  Variation from Quality Center : None 
// -----------------------------------------------
//  Scripted by : Varsha Deshpande
//  Last modified : 14  May 2013
// -----------------------------------------------

public class FullSanity_0705 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps
    static string VideoFile = "";

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:
        //Pre-conditions:
        //Based on QualityCenter test version 
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Navigate to My Devices");
        this.AddStep(new Step2(), "Step 2: Select Video file to be played");
        this.AddStep(new Step3(), "Step 3: PLay the video file");

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
            //VideoFile = CL.EA.GetTestParams("VIDEO_FILE_Type");
            VideoFile = CL.EA.GetValue("VideoFile");

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {

            //Navigate to My Devices
            StartStep();
            //res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MY DEVICES");
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL,"Failed to Navigate to My Devices");
            //}

            CL.EA.UI.Utils.ClearEPGInfo();
            bool resBool = CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:MY LIBRARY");
            if (!resBool)
            {
                FailStep(CL, "Failed to navigate to MY Library");
            }

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
            //Select Video file to be played
            StartStep();

            bool res2 = CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("Video");
            if (!res2)
            {
                FailStep(CL, "Failed to select Video");
            }

            CL.EA.UI.Utils.SendIR("SELECT");

            res2 = CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("All Videos");
            CL.EA.UI.Utils.SendIR("SELECT");
            if (!res2)
            {
                FailStep(CL, "Failed to navigate to All Videos");
            }


            string CurrentVideoFile;
            string FirstVideoFile;
            string timeStamp = "";

            res = CL.IEX.IR.SendIR("SELECT_DOWN", out timeStamp, 3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send IR Key SELECT_DOWN");
            }
            CL.IEX.Wait(3);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out FirstVideoFile);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get video file Name");
            }

            if (!VideoFile.Equals(FirstVideoFile))
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

                    res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out CurrentVideoFile);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Get video file Name");
                    }

                    if (VideoFile.Equals(CurrentVideoFile))
                    {
                        break;
                    }
                } while (!CurrentVideoFile.Equals(FirstVideoFile));
            }

            //Verify if the item is of type video content
            string videoContent = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("object", out videoContent);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get video content object");
            }

            if (videoContent != "[object VideoContent]")
            {
                FailStep(CL, "Targeted content is not video object");
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
                FailStep(CL, "Targeted video content is not playable");
            }

            //Select the item
            CL.IEX.Wait(5);
            res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 10);
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
            //Play the video file
            StartStep();

            CL.EA.UI.Utils.ClearEPGInfo();

            res = CL.IEX.MilestonesEPG.Navigate("PLAY");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Navigate to play");
            }

            //Verify State:TrickMode Bar after playback
            bool resBool = CL.EA.UI.Utils.VerifyState("TRICKMODE BAR");
            if (!resBool)
            {
                FailStep(CL, "Failed to verify TRICKMODE BAR state");
            }

            //Check for Video
            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On Video content");
            }

            //Check for Audio
            res = CL.EA.CheckForAudio(true, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Audio is Present On Video content");
            }

            //Forward
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 2, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set review buffer to 2 ");
            }
            CL.IEX.Wait(3);

            //Rewind
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set review buffer to -2 ");
            }
            CL.IEX.Wait(3);

            //Play
            res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play");
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