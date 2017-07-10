using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//Robustness test - Zapping 001
public class Robustness_InOutStandby_001 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static _Platform GW;

    //Channels used by the test
    static string isHomeClient;
    static string FTA_1st_Mux_3;
    static string channelNumBeforeStandBy = "";
    static string channelNumAfterStandby = "";
    static int Time_In_Standby = 30;
    static int failcount = 0;
    //Shared members between steps
  
    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: Robustness to tests Zapping

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to predefined channel before start of the test");
        this.AddStep(new Step2(), "Step 2: Enter and Exit standby with 30sec delay for 1000 times");

        //Get Client Platform
        CL = GetClient();

        isHomeClient = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_HOMECLIENT");
        if (Convert.ToBoolean(isHomeClient))
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


            //Get Values From ini File
            FTA_1st_Mux_3 = CL.EA.GetValue("FTA_1st_Mux_3");
            CL.IEX.LogComment("Retrieved Value From ini File: FTA_1st_Mux_3 = " + FTA_1st_Mux_3);


            if (isHomeClient.ToUpper() != "TRUE")
            {
                res = CL.EA.STBSettings.SetAutoStandBy("OFF");
                if (!(res.CommandSucceeded))
                {
                    LogCommentFailure(CL, "Failed to set to default standBy");
                }

            }
            else
            {
                //Setting the auto stand by to off in GW
                res = GW.EA.STBSettings.SetAutoStandBy("OFF");
                if (!(res.CommandSucceeded))
                {
                    LogCommentFailure(CL, "Failed to set to default standBy in GW");
                }
            }
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
        //Step 1: Standby and wakeup continously
        public override void Execute()
        {

            StartStep();

            for (int NumberOfstandby = 1; NumberOfstandby < 450; NumberOfstandby++)
            {
                //Enter standby
                res = CL.EA.StandBy(false);
                
                if (!res.CommandSucceeded)
                {
                    //increment counter if entering into standby fails
                    LogCommentFailure(CL, "Failed to Enter Standby after " + NumberOfstandby + "standby wake-ups");

                    failcount += 1;
                }
                //Reset the fail count to zero if standby is successful
                else
                {
                    failcount = 0;
                }

                //Stay in Standby for a few seconds
                CL.IEX.Wait(Time_In_Standby);

                //Exit standby
                res = CL.EA.StandBy(true);

                if (!res.CommandSucceeded)
                {
                    //increment counter if exit from standby fails
                    LogCommentFailure(CL, "Failed to exit standby " + NumberOfstandby + "standby wake-ups");
                    failcount += 1;
                }
                //Reset the fail count to zero if wake up is successful
                else
                {
                    failcount = 0;
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out channelNumAfterStandby);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to Get Channel Number (chnum) of Current Channel");
                }


                if (channelNumAfterStandby != channelNumBeforeStandBy)
                {
                    LogCommentFailure(CL, "Got differnt channel number thanm expected after Standby" + channelNumAfterStandby);
                }

                CL.IEX.Wait(Time_In_Standby);
                //Fail if the fail count is more than 50
                if (failcount > 50)
                {
                    FailStep(CL, res, "Failed to enter and exit stanby continously for more than 50 times after " + NumberOfstandby + "standby wake ups");
                }
                LogCommentInfo(CL, "Standby wakeup Count: " + NumberOfstandby);

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