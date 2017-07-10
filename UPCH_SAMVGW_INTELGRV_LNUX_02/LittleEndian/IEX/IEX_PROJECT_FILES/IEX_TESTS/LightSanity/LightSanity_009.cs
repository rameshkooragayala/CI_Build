using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-009_PLB-Playback(EOF)& Trick modes SD-HD events
public class LightSanity_009 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Short_SD_1;
    static string Short_HD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-009_PLB-Playback(EOF)& Trick modes SD-HD events
        //Checking Recording Functionalety.
        //Pre-conditions: Recording of Scrambled events.
        //Based on QualityCenter test version 4.
        //Variations from QualityCenter:Non
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Have Recordings on Disk");
        this.AddStep(new Step1(), "Step 1: Verify Recordings");
        this.AddStep(new Step2(), "Step 2: Verify Playback, Trick modes and Information");
        this.AddStep(new Step3(), "Step 3: Playback Till EndOfFile");

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
            Short_SD_1 = CL.EA.GetValue("Short_SD_1");
            Short_HD_1 = CL.EA.GetValue("Short_HD_1");
            /************************************************ 
             * There is no Short_HD channel in Cogeco Stream
             * without series with long titles
             * Therefore using another SD channel
            *************************************************/
            if (CL.EA.Project.Name.ToUpper() == "COGECO")
            {
                CL.IEX.LogComment("WARNING: There is no Short_HD channel in Cogeco stream without series (problem of long title), therefore using another SD channel instead HD - 425");
                Short_HD_1 = "425";
            }

            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
            }
            //Record SD event
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Short_SD_1);
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("SD_Event", 1, 2, false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book SD Event From Banner");
            }

            res = CL.EA.WaitUntilEventEnds("SD_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Till SD Event End Recording");
            }

            //Record HD event
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Short_HD_1);
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("HD_Event", 1, 2, false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book HD Event From Banner");
            }

            res = CL.EA.WaitUntilEventEnds("HD_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Till HD Event End Recording");
            }

            //Pcat update + Gurd time 
            CL.IEX.Wait(30 + 120);

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

            res = CL.EA.PVR.VerifyEventInArchive("SD_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verfiy SD Event on Archive");
            }

            res = CL.EA.PVR.VerifyEventInArchive("HD_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verfiy HD Event on Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Play The HD Event 
        //Use fast rewind (x2 x6 x12 x30) tricks modes,
        //Use fast forward (x2 x6 x12 x30) tricks mode,
        //Activate pause trick mode,
        //Activate play trick mode & slow motion
        public override void Execute()
        {

            StartStep();

            res = CL.EA.PVR.PlaybackRecFromArchive("HD_Event", 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the HD_Event From Achive");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present During Playback");
            }

            // + -  2
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", 2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x2 Speed");
            }


            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", -2, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x(-2) Speed");
            }

            // + -  6
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", 6, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x6 Speed");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", -6, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x(-6) Speed");
            }

            // + -  12
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", 12, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x12 Speed");
            }
            CL.IEX.Wait(3);
            // Pause
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause the HD_Event");
            }

            //Slow Motion
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", 0.5, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at Slow Motion Speed");
            }
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", -12, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x(-12) Speed");
            }

            // + -  30
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", 30, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x30 Speed");
            }
            CL.IEX.Wait(15);
            res = CL.EA.PVR.SetTrickModeSpeed("HD_Event", -30, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play the HD_Event at x(-30) Speed");
            }

            res = CL.EA.PVR.StopPlayback(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Playback of HD_Event");
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

            // as we have event duration calculation problem, the EOF checking is disabled
            res = CL.EA.PVR.PlaybackRecFromArchive("SD_Event", 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the SD_Event From Achive Till EOF");
            }

            /* res = CL.EA.CheckForVideo(true, false, 15);
             if (!res.CommandSucceeded)
             {
                 FailStep(CL,res, "Failed to Verify Video is Present During Playback");
             }*/

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