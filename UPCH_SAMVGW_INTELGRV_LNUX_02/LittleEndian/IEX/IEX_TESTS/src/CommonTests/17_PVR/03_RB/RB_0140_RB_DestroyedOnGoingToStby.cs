/// <summary>
///  Script Name : RB-0140_RB_DestroyedOnGoingToStby.cs
///  Test Name   : RB-0140-RB is destroyed when going to standby
///  TEST ID     : 16025
///  QC Version  : 3
/// -----------------------------------------------
///  Written by : Aditya Kambampati
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

/**********************************************************************************************//**
 * @class   RB_0140
 *
 * @brief  To check if the RB is destroyed after going to stby
 *
 * @author  Adityaka
 * @date    10/22/2013
 **************************************************************************************************/

public class RB_0140 : _Test
{
    private static _Platform CL;
    private static Service clearChannel1;
    private static double rbDepthInMin;
    private static string timeStamp = "";
    private static string timeStampMarginLine = "";
    private static string rbDepth = "";
    private static string pause;
    private static string Milestone = "";
    

    private static class Constants
    {
        public const int timeInStby = 10;
        public const int waitInLive = 180;
        public const int waitForMilestones = 30;
        public const int timeToReachLive = 1;     //in mins           
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune to Channel, wait for RB to grow");
        this.AddStep(new Step2(), "Step 2: Go to Standby and wakeup the box");
        this.AddStep(new Step3(), "Step 3: Checking previous RB was destroyed");
                //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    /**********************************************************************************************/
    /**
* @class   PreCondition
*
* @brief   Get Channel Numbers From ini File & Sync.
*
* @author  Adityaka
* @date    10/22/2013
**************************************************************************************************/

    private class PreCondition : _Step
    {
       
        public override void Execute()
        {
            StartStep();

            //Tuning to a FTA channel and waiting for the RB to be filled for a duration of 10 mins
            clearChannel1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;Type=Radio");
            if (clearChannel1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
                        
            Milestone = CL.EA.UI.Utils.GetValueFromMilestones("GetReviewBufferCurrentDepth");
            
            pause = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "PAUSE");
            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Tune to a serivce and let RB to grow.
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    private class Step1 : _Step
    {
       
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel1.LCN);
            }

            CL.IEX.Wait(Constants.waitInLive);


            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief  Go to stby abd wakeup .
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //flushing RB by entering stby

            res = CL.EA.FlushRB();

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to standy and wakeup the STB");
            }
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after Playback from Last Viewed Position ");
            }

           PassStep();
        }
    }

    #endregion Step2

    #region Step3

    /**********************************************************************************************//**
     * @class   Step3
     *
     * @brief   check if the previous RB persists after wakeup.
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    private class Step3 : _Step
    {
       public override void Execute()
        {
            StartStep();

           //calculating the RB depth
            bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
            if (!verifyMilestones)
            {
                FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.PVR.SetTrickModeSpeed("RB", Convert.ToDouble(pause), false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
            bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
            if (!endVerifyMilestones)
            {
                FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
            }

            res = CL.EA.GetRBDepthInSec(timeStampMarginLine, ref rbDepthInMin);


            CL.IEX.LogComment("RB Depth = " + rbDepthInMin.ToString());

            if (rbDepthInMin > Constants.timeToReachLive)
            {
                FailStep(CL, res, "Failed: Review Buffer is not destroyed on going to stby");
            }

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

    /**********************************************************************************************//**
     * @fn  public override void PostExecute()
     *
     * @brief   Posts the execute.
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    public override void PostExecute()
    {

    }

    #endregion PostExecute
}