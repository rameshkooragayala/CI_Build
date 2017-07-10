/// <summary>
///  Script Name : RB-0080_BackToLiveFromRB.cs
///  Test Name   : RB-0080-Back to live from review buffer
///  TEST ID     : 12908
///  Jira ID     : FC-707
///  QC Version  : 3
/// -----------------------------------------------
///  Written by : Aditya Kambampati
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

/**********************************************************************************************/
/**
* @class   RB_0080
*
* @brief   Catching up with live through main menu . MAINMENU + OK on RB and persistent RB.
*
* @author  Adityaka
* @date    10/3/2013
**************************************************************************************************/

public class RB_0080 : _Test
{

    [ThreadStatic]
    private static _Platform CL;
    private static Service clearChannel1;
    private static Service clearChannel2; 
    private static Service clearChannel3;
    private static Service clearChannel4;
    private static Service clearChannel5;

    private static class Constants
    {
        public const int timeInPlayback = 15;
        public const int timeToWaitinPause = 15;
        public const int timeToWaitinEachService = 90;
        public const int timeToRewind = 15;
        public const int waitForMilestones = 30;
        public const int speedForPlay = 1;
        public const int speedForPause = 0;
        public const int speedForRewind = -30;
    }

    #region Create Structure

    /**********************************************************************************************/
    /**
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
        this.AddStep(new Step1(), "Step 1: Enter time-shift mode and stop RB playback");
        this.AddStep(new Step2(), "Step 2: Zap to multiple channels and Wait for RB to grow & stop RB playback");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Fetching the channel numbers from content XML as a precondition
*
* @author  Adityaka
* @date    10/3/2013
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            clearChannel1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High");
            if (clearChannel1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            clearChannel2 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;LCN=" + clearChannel1.LCN);

            if (clearChannel2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            clearChannel3 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;LCN=" + clearChannel1.LCN + "," + clearChannel2.LCN);

            if (clearChannel3 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            clearChannel4 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;LCN=" + clearChannel1.LCN + "," + clearChannel2.LCN + "," + clearChannel3.LCN);
            
            if (clearChannel4 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
            
            clearChannel5 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;LCN=" + clearChannel1.LCN +","+clearChannel2.LCN+","+clearChannel3.LCN+"," +clearChannel4.LCN);

            if (clearChannel5 == null)
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
        /**********************************************************************************************/
        /*** @fn  public override void Execute()
         * @brief   Tune to a clear service and pause for a while . Then playback the RB . Catch up with live through Main menu
         *
         * @author  Adityaka
         * @date    10/3/2013
        **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //tuning to the claer channel
            
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel1.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel1.LCN);
                }

                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to pause on Live");
                }

                res = CL.IEX.Wait(Constants.timeToWaitinPause);

                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Play RB");
                }

                res = CL.IEX.Wait(Constants.timeInPlayback);

                res = CL.EA.PVR.StopPlayback(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Return to Live Viewing From RB");
                }

            //fetching the expecting milstones from milestones.ini file
                System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
                string Milestone = CL.EA.UI.Utils.GetValueFromMilestones("TrickModeStopInReviewBuffer");

            //pausing the LIVE
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPause, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to pause on Live");
                }

                res = CL.IEX.Wait(Constants.timeToWaitinPause);

            //playing back in timeshift mode (RB)
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Play RB");
                }

                res = CL.IEX.Wait(Constants.timeInPlayback);

            //waiting for the expected milestones
                bool verify = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
                if (!verify)
                {
                    FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
                }

            //catching up with live from main menu
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:WATCH");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to catchup with live");
                }

                bool endVerify = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
                if (!endVerify)
                {
                    FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
                }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        /**********************************************************************************************/
        /*** @fn  public override void Execute()
         * @brief   Tune to a 5 different services and rewing for a while . Then playback the RB . Catch up with live through Main menu
         *
         * @author  Adityaka
         * @date    10/3/2013
        **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

              //waiting in 5 different services for sometime to get the persistent RB
               
              res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel2.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel2.LCN);
                }

                CL.IEX.Wait(Constants.timeToWaitinEachService);

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel3.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel3.LCN);
                }

                CL.IEX.Wait(Constants.timeToWaitinEachService);
                
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel4.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel4.LCN);
                }

                CL.IEX.Wait(Constants.timeToWaitinEachService);

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel5.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel4.LCN);
                }

                CL.IEX.Wait(Constants.timeToWaitinEachService);

                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForRewind, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Rewind into RB");
                }

                res = CL.IEX.Wait(Constants.timeToRewind);

                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to playback RB");
                }

                res = CL.EA.PVR.StopPlayback(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Return to Live Viewing From RB");

                }

            //fetching the expecting milstones from milestones.ini file
                System.Collections.ArrayList ActualLines = new System.Collections.ArrayList();
                string Milestone = CL.EA.UI.Utils.GetValueFromMilestones("TrickModeStopInReviewBuffer");

            //rewinding in the RB                
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForRewind, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Rewind into RB");
                }

                CL.IEX.Wait(Constants.timeToRewind);
            
            //playing back the RB
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.speedForPlay, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to playback in RB");
                }

             //waiting for the expected milestones
                bool verifyMilestones = CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestone, Constants.waitForMilestones);
                if (!verifyMilestones)
                {
                    FailStep(CL, res, "Failed to BeginWaitForMessage TrickModeStopInReviewBuffer");
                }

             //catching up with LIVE    by pressing OK on main menu
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:WATCH");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to catchup with live");
                }

                bool endVerifyMilestones = CL.EA.UI.Utils.EndWaitForDebugMessages(Milestone, ref ActualLines);
                if (!endVerifyMilestones)
                {
                    FailStep(CL, res, "Failed to get EndWaitForMessage TrickModeStopInReviewBuffer");
                }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {


    }

    #endregion PostExecute
}