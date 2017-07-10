using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_0906 : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;


    //Channels used by the test
    static string Medium_SD_2;
    static string Long_SD_1;
    static Boolean isHomeNetwork;
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Preconditions: Get Values from ini File, SyncStream and Have a Future Recording Booked");
        this.AddStep(new Step1(), "Step 1: Wait for Recording to Start");
        this.AddStep(new Step2(), "Step 2: During Recording Remove Power, Wait 2 Minutes and Power On");
        this.AddStep(new Step3(), "Step 3: Wait for the Record to be Finished");
        this.AddStep(new Step4(), "Step 4: Verify Recording Exception Reason");
        this.AddStep(new Step5(), "Step 5: Playback the Recording");

        //Get Client Platform
        CL = GetClient();
        isHomeNetwork = Convert.ToBoolean(CL.EA.GetTestParams("IsHomeNetwork"));
        if (isHomeNetwork)
        {
            GW = GetGateway();
        }
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values from ini File
			Medium_SD_2 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Medium_SD_2");
            if (String.IsNullOrEmpty(Medium_SD_2))
            {
            Medium_SD_2 = CL.EA.GetValue("Medium_SD_2");
			}
            CL.IEX.LogComment("Retrieved Value From ini File: Medium_SD_2 =  " + Medium_SD_2);

			Long_SD_1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Long_SD_1");
            if (String.IsNullOrEmpty(Long_SD_1))
            {
            Long_SD_1 = CL.EA.GetValue("Long_SD_1");
			}
            CL.IEX.LogComment("Retrieved Value From ini File: Long_SD_1 = " + Long_SD_1);


            // Pre-condition: Have a future record booked. Choose a channel with events at least 10 minutes long, but not much longer is needed.
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            //Check enough time left for the event
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

            //Book on going event from Banner 
            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event1", 1, false, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Current Event From Banner");
            }

            //   res = CL.EA.PVR.BookFutureEventFromBanner("Event1", 1, 2);
            //    if (!res.CommandSucceeded)
            //   {
            //       FailStep(CL,res, "Failed to Book Future Event From Banner");
            //   }
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

            // Wait for the Recording to Start
            res = CL.EA.WaitUntilEventStarts("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Starts");
            }

            //EPG time has accuracy of minutes, so wait a few more seconds to be sure recording has started
            int Test_Wait = 30;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.PCAT.VerifyEventIsRecording("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify the Event is Recording in PCAT");
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

            if (isHomeNetwork)
            {
                res = GW.EA.PowerCycle(30, false, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Reboot GW STB (Without Format");
                }
            }
            
            //During Recording Remove Power (Off);, Wait 1 minutes, Power On.
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT);

           // Don't check the return type as it is validating BOX is rebooted or not with debug traces without run the MW
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Reboot STB (Without Format");
            }

            //Stay in Standby for a few seconds 
            int Time_In_Standby = 50;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            //Tune to a channel other than the one with the recording event
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Medium_SD_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On Second FTA Channel After DCA");
            }

            res = CL.EA.PCAT.VerifyEventIsRecording("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify the Event is Recording in PCAT");
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

            // Wait for the record to be finished (inc. GT);.
            res = CL.EA.WaitUntilEventEnds("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Ends ");
            }
            // Wait for GT to end
            
            int Default_Guard_Time = 180;
            CL.IEX.Wait(Default_Guard_Time);

            PassStep();
        }
    }
    #endregion
    #region Step4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Access the recording in Recording List.
            res = CL.EA.PCAT.VerifyEventPartialStatus("Event1", "PARTIAL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify the Event is Partially Recorded");
            }

            res = CL.EA.PCAT.VerifyEventExceptionReason("Event1", "POWER_FAILURE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify the Event's Exception Reason");
            }
            PassStep();
        }
    }
    #endregion
    #region Step5
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //5. Playback the recording, especially around the append point.
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback The Event");
            }

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing (with Video); after Playback of Appended Recording");
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