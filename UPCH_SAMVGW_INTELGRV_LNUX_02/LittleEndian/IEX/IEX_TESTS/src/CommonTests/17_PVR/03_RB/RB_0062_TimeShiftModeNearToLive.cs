/// <summary>
///  Script Name : RB_0062_TimeShiftModeNearToLive.cs
///  Test Name   : RB-0062-Time-shift mode near to Live
///  TEST ID     : 16423
///   Jira ID     : FC-706
///  QC Version  : 3
/// -----------------------------------------------
///  Written by : Aditya Kambampati
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

/**********************************************************************************************//**
 * @class   RB_0062
 * @author  Adityaka
 * @date    10/3/2013
 **************************************************************************************************/

public class RB_0062 : _Test
{
   
    [ThreadStatic]
    private static _Platform CL;
    private static Service clearChannel;
    
    private static class Constants
    {
        public const int timeToWaitinRB = 90;
        public const int timeToWaitinPause = 5;
        public const int speedForPlay = 1;
        public const int speedForPause = 0;
        public const double speedForSlowMotion = 0.5;

    }

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Adityaka
     * @date    10/3/2013
     **************************************************************************************************/

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to Channel, Pause for 5sec to enter time shift mode");
        this.AddStep(new Step2(), "Step 2: Wait for 5 mins and playback RB in 0.5x speed ");
        this.AddStep(new Step3(), "Step 3: Stop the RB to check the STB is still playing in timeshift mode");
        
        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Precondition: Get Channel Numbers From ini File & Sync
         *
         * @author  Adityaka
         * @date    10/3/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //fetching a channel number from Content.xml
            clearChannel = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High");
            if (clearChannel == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

   
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Tune to Channel, Pause for 5sec to enter time shift mode
         *
         * @author  Adityaka
         * @date    10/3/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //tuning to the channel
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel.LCN);
            }
            //Pause on LVIE
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to pause on Live");
            }
            
            res = CL.IEX.Wait(Constants.timeToWaitinPause);
            //PLayback on RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play RB");
            }
                             
            PassStep();
        }
    }

    #endregion Step1

    #region Step2


    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Wait for 5 mins and playback RB in 0.5x speed
         *
         * @author  Adityaka
         * @date    10/3/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            res = CL.IEX.Wait(Constants.timeToWaitinRB);
            //playing RB in slowmotion

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0.5, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play RB at 0.5x speed ");
            }

            res = CL.IEX.Wait(Constants.timeToWaitinRB);
                        
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

       private class Step3 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Stop the RB to check the STB is still playing in timeshift mode.
         *
         * @author  Adityaka
         * @date    10/3/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
          //Stop the playback in RB to ensure playback didnot catch up with the live 
            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
            }

            PassStep();
        }
    }

    #endregion Step3
    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {


    }

    #endregion PostExecute
}