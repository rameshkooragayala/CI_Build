using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
//LightSanity-409-STC-StartChannelForCurrentProfile 
public class LightSanity_409 : _Test
{
    [ThreadStatic]
    static _Platform CL;

  //Test Duration
    //Channels used by the test
    static string FTA_1st_Mux_3;
    static string channelNumBeforeStandBy = "";
    static string channelNumAfterStandby ="";
    static string channelNumBeforePowerCycle = "";
    static string channelNumAfterPowerCycle = "";
    static string FTA_1st_Mux_2;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        //Brief Description: LightSanity-409-STC-StartChannelForCurrentProfile 
        //Check that User is able to change the default channel
        //Pre-conditions: none
        //Based on QualityCenter test version 
        //Variations from QualityCenter: we can't change the definition of the default channel, as described in step 2. just checking that the start channel is different than before 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1:Tune to AV Service 1 ");
        this.AddStep(new Step2(), "Step 2:Enter Standy and Exit,Verify the Channel tuned is same as Last Channel->AV Service 1 ");
        this.AddStep(new Step3(), "Step 3:Tune to AV Service 2");
        this.AddStep(new Step4(), "Step 4:Reboot and Allow the box to complete Power cycle,Verify the Channel tuned is same as Last Channel->AV Service 1 ");
        
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
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            CL.IEX.LogComment("Retrieved Value From ini File: FTA_1st_Mux_3 = " + FTA_1st_Mux_3);

            FTA_1st_Mux_2 = CL.EA.GetValue("FTA_1st_Mux_2");
            CL.IEX.LogComment("Retrieved Channel name From ini File: Name_FTA_1st_Mux_3 = " + FTA_1st_Mux_2);


           
            PassStep();
        }
    }
    #endregion
    #region Step1
    public class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Tune to the given channel
            res = CL.EA.TuneToChannel(FTA_1st_Mux_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Tune to the given channel ");
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumBeforeStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the channel number ");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    public class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Stay in Standby for a few seconds
            int Time_In_Standby = 5;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            // check what is the start channel
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumAfterStandby);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Number (chnum) of Current Channel");
            }


            if (channelNumAfterStandby != channelNumBeforeStandBy)
            {
                FailStep(CL, "Got differnt channel number thanm expected after Standby" + channelNumAfterStandby);
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

            // Tune to a Different Chanel other then Channel tuned before Standby
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to do Channel Surf to Channel" + FTA_1st_Mux_2);
            }


            //res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumBeforePowerCycle);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to Get Channel Number (chnum) of Current Channel");
            //}

            PassStep();
        }
    }
    #endregion
    #region Step4
    public class Step4 : _Step
    {
        public override void Execute()
        {
            

            StartStep();
            res = CL.EA.PowerCycle(0, true, false);
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Stay in Standby for a few seconds
            int Time_In_Standby = 240;
            CL.IEX.Wait(Time_In_Standby);

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            CL.IEX.Wait(5);
            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Channel After DCA");
            }
            
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumAfterPowerCycle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present On FTA Channel After DCA");
            }



            CL.IEX.LogComment("channelNumAfterPowerCycle = " + channelNumAfterStandby + " channelNumAfterPowerCycle = " + channelNumAfterPowerCycle);

            if (channelNumAfterPowerCycle != channelNumAfterStandby)
            {
                FailStep(CL, res, "Failed to Verify that the CPE tuned to same channel after Power Cycle");
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