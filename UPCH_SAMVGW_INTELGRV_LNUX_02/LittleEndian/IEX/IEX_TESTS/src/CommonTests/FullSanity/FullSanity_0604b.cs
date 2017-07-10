using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_0604b : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Short_SD_1;
    static string Medium_HD_1;
    static string Long_SD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File, Sync Stream");
        this.AddStep(new Step1(), "Step 1: Have a Recording on the Disk");
        this.AddStep(new Step2(), "Step 2: Go to Non-Live TV Viewing, like Playback of Recorded Event");
        this.AddStep(new Step3(), "Step 3: Tune to a Channel and Playback the RB");



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
            CL.IEX.LogComment("Retrieved Value From ini File: Short_SD_1 =" + Short_SD_1);

            Medium_HD_1 = CL.EA.GetValue("Medium_HD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Medium_HD_1 =  " + Medium_HD_1);

            Long_SD_1 = CL.EA.GetValue("Long_SD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: : Long_SD_1 =  " + Long_SD_1);




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
            //Have a Recording on the Disk
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_HD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event1", 4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event");
            }
            //Wait to have recorded content of reasonable length
            int Time_Recording = 180;
            CL.IEX.Wait(Time_Recording);

            res = CL.EA.PVR.StopRecordingFromBanner("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
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

            //Go to Non-Live TV Viewing, like Playback of Recorded Event
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback Record from Archive");
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

            // Tune to a channel and PB the RB
            //(Tune to a different channel than in step 1
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