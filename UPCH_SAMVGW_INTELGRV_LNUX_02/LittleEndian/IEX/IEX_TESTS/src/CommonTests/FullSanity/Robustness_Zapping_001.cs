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
public class Robustness_Zapping_001 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static _Platform GW;

    //Channels used by the test
    static string First_of_10_Consecutive_FTA;
    static string isHomeClient;
    //Shared members between steps
  
    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: Robustness to tests Zapping

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Channel UP 10 times/ Channel Down 10 times in a loop for 4000 zaps");


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

            
            First_of_10_Consecutive_FTA = CL.EA.GetValue("First_of_10_Consecutive_FTA");
            CL.IEX.LogComment("Retrieved Value From ini File: First_of_10_Consecutive_FTA = " + First_of_10_Consecutive_FTA);


            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_10_Consecutive_FTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + First_of_10_Consecutive_FTA);
            }
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
    //create not predicted by DCA to the other dirction 

    private class Step1 : _Step
    {
        //Step 1: Channel UP 10 times and channel down 10 times in loop
        public override void Execute()
        {

            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, First_of_10_Consecutive_FTA);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + First_of_10_Consecutive_FTA);
            }
            for (int NumberOfZaps = 1; NumberOfZaps < 2500; )
            {

                for (int zaps_up = 1; zaps_up < 10; zaps_up++)
               {
                 res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", true, 1, EnumPredicted.Default);
                 if (!res.CommandSucceeded)
                 {
                     FailStep(CL, res, "Failed Channel UP as not predicted in " + NumberOfZaps + " zap ");
                 }

                 CL.IEX.Wait(5);

                 LogCommentInfo(CL, "Zap Count: " + NumberOfZaps);

                NumberOfZaps = NumberOfZaps + 1;
            }
                for (int zaps_down = 1; zaps_down < 10; zaps_down++)
            {

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", false, 1, EnumPredicted.Default);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed Channel Up  as not predicted in " + NumberOfZaps + " zap ");
                }

                CL.IEX.Wait(5);

                LogCommentInfo(CL, "Zap Count: " +NumberOfZaps);
 
                NumberOfZaps = NumberOfZaps + 1;
            }
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