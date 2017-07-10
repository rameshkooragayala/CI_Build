using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-1301
public class FullSanity_1301 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FTA_1st_Mux_1;
    static string Name_FTA_1st_Mux_2;
    static string Name_FTA_1st_Mux_3;
    static string FTA_1st_Mux_4;
    static string FTA_Channel;
    static string FTA_2nd_Mux_1;


    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: FullSanity-0203-LIVE-Fast Zapping_numeric _P+_P-
        //Performs Fast Zapping between FTA, Scrambled, SD & HD services by Numeric Key and Channel Up\Down
        //Details:S1-S2-S3-S4 are FTA-Scrambled-SD-HD, one after the other in the channel Line-up
        //Based on QualityCenter ...
        //Variations from QualityCenter: Not testing Channel Bar Info in any step.

        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File & SyncStream");

        this.AddStep(new Step1(), "Step 1: Book Four Simultaneous Recordings");
        this.AddStep(new Step2(), "Step 2: Channel Change to 5th Channel");
        this.AddStep(new Step3(), "Step 3: Record 5th Event");
        this.AddStep(new Step3(), "Step 4: Channel Change to 6th Channel");

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
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");
            CL.IEX.LogComment("CL.EA.GetValue Retrieved Value From ini File: FTA_1st_Mux_1 =" + FTA_1st_Mux_1);

            Name_FTA_1st_Mux_2 = CL.EA.GetValue("Name_FTA_1st_Mux_2");
            CL.IEX.LogComment("CL.EA.GetValue Retrieved Value From ini File: Name_FTA_1st_Mux_2 = " + Name_FTA_1st_Mux_2);

            Name_FTA_1st_Mux_3 = CL.EA.GetValue("Name_FTA_1st_Mux_3");
            CL.IEX.LogComment("CL.EA.GetValue Retrieved Value From ini File: Name_FTA_1st_Mux_3 =" + Name_FTA_1st_Mux_3);

            FTA_1st_Mux_4 = CL.EA.GetValue("FTA_1st_Mux_4");
            CL.IEX.LogComment("CL.EA.GetValue Retrieved Value From ini File: FTA_1st_Mux_4 = " + FTA_1st_Mux_4);

            FTA_Channel = CL.EA.GetValue("FTA_Channel");
            CL.IEX.LogComment("CL.EA.GetValue Retrieved Value From ini File: FTA_Channel =" + FTA_Channel);

            FTA_2nd_Mux_1 = CL.EA.GetValue("FTA_2nd_Mux_1");
            CL.IEX.LogComment("CL.EA.GetValue Retrieved Value From ini File: FTA_2nd_Mux_1 = " + FTA_2nd_Mux_1);


            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Tune to  Free Air_SD -> Free Air_HD  -> Scramble_SD
        //Check for video
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA_1st_Mux_1 : " + FTA_1st_Mux_1 + " With DCA");
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on FTA_1st_Mux_1 : " + FTA_1st_Mux_1 + " After DCA");
            }

            res = CL.EA.PVR.RecordManualFromCurrent("Event1", FTA_1st_Mux_1, 13, EnumFrequency.ONE_TIME, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Manual Record Current Event1 ");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("Event2", Name_FTA_1st_Mux_2, -1, 2, 13, EnumFrequency.ONE_TIME, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Manual Record Current Event2 ");
            }

            res = CL.EA.PVR.RecordManualFromPlanner("Event3", Name_FTA_1st_Mux_3, -1, 2, 13, EnumFrequency.ONE_TIME, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Manual Record Current Event3 ");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel ");
            }

            res = CL.EA.PVR.RecordManualFromCurrent("Event4", FTA_1st_Mux_4, 13, EnumFrequency.ONE_TIME, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Manual Record Current Event4  ");
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
            //Channel change to 5th channel S5
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to 5th Channel ");
            }

            //Wait to be sure PCAT is apdated in any recording status changes due to the CC
            int PCAT_Wait = 15;
            CL.IEX.Wait(PCAT_Wait);

            //Verify all 4 items are still being recorded
            res = CL.EA.PCAT.VerifyEventIsRecording("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event1 is Recording in PCAT after CC to 5th Channel ");
            }

            res = CL.EA.PCAT.VerifyEventIsRecording("Event2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Verify Event2 is Recording in PCAT after CC to 5th Channel");
            }

            res = CL.EA.PCAT.VerifyEventIsRecording("Event3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event3 is Recording in PCAT after CC to 5th Channel  ");
            }

            res = CL.EA.PCAT.VerifyEventIsRecording("Event4");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Event4 is Recording in PCAT after CC to 5th Channel  ");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present on 5th Channel, with 4 Background Recordings ");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Tune to  Free Air_SD <- Free Air_HD  <- Scramble_SD (last step left us at Scramble_SD);
        //Check for video
        public override void Execute()
        {
            StartStep();
            //Book the 5th event on channel S5
            res = CL.EA.PVR.RecordManualFromCurrent("Event5", FTA_Channel, 5, EnumFrequency.ONE_TIME, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Fifth Event ");
            }

            //Fifth booking should cause Booking Conflict
            res = CL.EA.PVR.ResolveConflict("Event5", "DEFER", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Resolve Booking Conflict ");
            }

            //Wait to be sure PCAT is apdated in any recording status changes due to conflict resolution
            int PCAT_Wait = 15;
            CL.IEX.Wait(PCAT_Wait);

            //Verify all 5 items are being recorded
            res = CL.EA.PCAT.VerifyEventIsRecording("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to Verify in PCAT Event1 is Recording after 5th Recording Started ");
            }

            res = CL.EA.PCAT.VerifyEventIsRecording("Event2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify in PCAT Event2 is Recording after 5th Recording Started ");
            }


            res = CL.EA.PCAT.VerifyEventIsRecording("Event3");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify in PCAT Event3 is Recording after 5th Recording Started  ");
            }

            res = CL.EA.PCAT.VerifyEventIsRecording("Event4");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify in PCAT Event4 is Recording after 5th Recording Started ");
            }

            res = CL.EA.PCAT.VerifyEventIsRecording("Event5");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify in PCAT Event5 has Started Recording ");
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