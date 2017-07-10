using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class FullSanity_1203 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    //Channels used by the test
    static string Short_SD_Scrambled_1;
    static string rf_port = "";
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        //Brief Description: Playback a Recording from Different Start Positions

        //Based on QualityCenter test version 2.
        //Variations from QualityCenter: Not checking Audio. 

        this.AddStep(new PreCondition(), "Preconditions: Synced Stream and Have a Recording on Disk");
        this.AddStep(new Step1(), "Step 1: Playback Starts from the Beginning");
        this.AddStep(new Step2(), "Step 2: Playback the Same Event Again, From Last Viewed Position");
        this.AddStep(new Step3(), "Step 3: Playback Again from the Beginning");

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

              //Short_SD_Scrambled_1 = 4 & ChName= Service 2444
              Short_SD_Scrambled_1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Short_SD_Scrambled_1");
              if (string.IsNullOrEmpty(Short_SD_Scrambled_1))
              {
                    Short_SD_Scrambled_1 = CL.EA.GetValue("Short_SD_Scrambled_1");  
              }               

            CL.IEX.LogComment("Retrieved Value From ini File: Short_SD_Scrambled_1 = " + Short_SD_Scrambled_1);

            // Change the channel banner timeout to 5 sec
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 5 Sec");
            }


            //Have a Recording on Disk
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_Scrambled_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel ");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event1", -1, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Banner ");
            }

            //res = CL.EA.WaitUntilEventEnds("Event1");
            //if (!res.CommandSucceeded)
            //{
            //FailStep(CL,res, "Failed to Wait Until Event Ends ");
            //}

            //Wait for GT to end
            int RECORDED_Time = 120;
            res = CL.IEX.Wait(RECORDED_Time);

            //res = CL.EA.PCAT.VerifyEventPartialStatus("Event1", "ALL");
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL,res, "Failed to Verify the Event is Fully Recorded ");
            //}
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
            //Playback event from the beginning
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", 120, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event from the Beginning for 2 minutes  ");
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
            //Navigate back to Live Viewing and Playback the Same Event Again, From Last Viewed Position
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing after Playback of 2 minutes of the Event  ");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", 0, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event from Last Viewed Position  ");
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
            //Navigate back to Live Viewing and Playback Event Again from the Beginning
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after Playback from Last Viewed Position ");
            }

            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event a Second Time from the Beginning  ");
            }

            // res = CL.EA.ReturnToLiveViewing(true);
            // if (!res.CommandSucceeded)
            // {
            //    FailStep(CL,res, "Failed to Return to Live Viewing after Second Playback to EOF ");
            // }

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
