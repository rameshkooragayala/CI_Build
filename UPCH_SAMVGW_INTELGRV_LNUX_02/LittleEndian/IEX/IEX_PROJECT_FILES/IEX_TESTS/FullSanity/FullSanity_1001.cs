using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-1001-AUD-Audio stream selection
public class FullSanity_1001 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Multiple_Audio_1;

    //Shared members between steps
    static string audioMenu = "MAIN MENU/LIVE/ACTION BAR/A//V SETTINGS/AUDIO/";
    static string defaultAudio = "";
    static string audioToChangeTo = "";

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-0160-AUD-Audio stream selection
        //Pre-conditions: On service S1, Event E1 has at least two audio streams available in its component descriptor.
        //Based on QualityCenter test version 4.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Change Audio on Live");
        this.AddStep(new Step2(), "Step 2: Change Audio on RB");
        this.AddStep(new Step3(), "Step 3: Change Audio on Playback");

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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Multiple_Audio_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }

            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 180)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
            }

            //Change to other audio (nav to audio and select the next item);
            res = CL.IEX.MilestonesEPG.Navigate(audioMenu);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + audioMenu);

            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Audio Track Name");
            }


            //Change to any audio
            string timeStamp = "";
            res = CL.IEX.IR.SendIR("SELECT_UP", out timeStamp, 5000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Send SELECT_UP IR Key");
            }


            //Get destination audio      
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out audioToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Audio Track Name");
            }


            //Select the audio
            res = CL.IEX.MilestonesEPG.Navigate(audioToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select Audio Track: " + audioToChangeTo);
            }


            //Check if audio alive
            // res = CL.EA.CheckForAudio(true, 10);
            //  if (!res.CommandSucceeded)
            // {
            //     FailStep(CL,res, "Failed to Check for Audio");
            // }


            //Return to original audio
            res = CL.IEX.MilestonesEPG.Navigate(audioMenu + defaultAudio);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Select Audio Track: " + defaultAudio);
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

            if (CL.EA.Project.Name.ToUpper() != "ISTB")
            {
                //Create play form RB (press pause, wait for timeshifting to be activated);
                res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Pause From Live");
                }

                res = CL.EA.CheckForVideo(false, false, 10);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify that Video is Paused");
                }

                int RB_Initial_Depth = 5;
                CL.IEX.Wait(RB_Initial_Depth);

                res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Play From RB");
                }

                //Change to other audio (nav to audio and select the next item);
                //Go to destenetion Audio
                res = CL.IEX.MilestonesEPG.Navigate(audioMenu + audioToChangeTo);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Select Audio: " + audioToChangeTo);
                }

                //Check if audio alive
                //  res = CL.EA.CheckForAudio(true, 10);
                //  if (!res.CommandSucceeded)
                {
                    //      FailStep(CL,res, "Failed to Check for Audio");
                }

                //Return to original audio
                res = CL.IEX.MilestonesEPG.Navigate(audioMenu + defaultAudio);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Select Audio: " + defaultAudio);
                }

                //Stop the Review Buffer Playback
                res = CL.EA.PVR.StopPlayback(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Stop Playback of RB");
                }
            }
            else
            {
                CL.IEX.LogComment("Audio change in RB is not required for ISTB");
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

            if (CL.EA.Project.Name.ToUpper() != "ISTB")
            {
                //Record event and play it
                res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent", 2);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Record Current Event");
                }

                res = CL.EA.WaitUntilEventEnds("RecEvent");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                //Playback the Already Recorded Event from Archive.
                res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent", 0, true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res);
                }

                //Change to other audio (navigate to audio and select the next item)
                res = CL.IEX.MilestonesEPG.Navigate("ACTION BAR/A//V SETTINGS/AUDIO");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navegate To ACTION BAR/A/V SETTINGS/AUDIO");
                }

                //find manualy any audio
                string timeStamp = "";
                res = CL.IEX.IR.SendIR("SELECT_UP", out timeStamp, 3000);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Send SELECT_UP IR Key");
                }

                res = CL.IEX.IR.SendIR("SELECT", out timeStamp, 5000);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Send SELECT IR Key");
                }

                //Verify return to playback state
                string currentState = "";
                res = CL.IEX.MilestonesEPG.GetActiveState(out currentState);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get Current State");
                }

                if (!currentState.Equals("PLAYBACK"))
                {
                    FailStep(CL, "Failed to Return to Playback. Current State is: " + currentState);
                }

                //  res = CL.EA.CheckForAudio(true, 10);
                //  if (!res.CommandSucceeded)
                {
                    //    FailStep(CL,res, "Failed to Check for Audio");
                }
            }
            else
            {
                CL.IEX.LogComment("Audio change in PLB is not required for ISTB");
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