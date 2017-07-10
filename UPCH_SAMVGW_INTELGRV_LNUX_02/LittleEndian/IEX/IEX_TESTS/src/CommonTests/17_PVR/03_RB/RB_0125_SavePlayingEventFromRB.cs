/// <summary>
///  Script Name : RB_0125_SavePlayingEventFromRB.cs
///  Test Name   : RB-0125-Save playing event from RB
///  TEST ID     : 20109
///  QC Version  : 3
/// -----------------------------------------------
///  Written by : Aditya Kambampati
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

/**********************************************************************************************/
/**
* @class   RB_0125
*
* @brief   A rb 0125.
*
* @author  Adityaka
* @date    10/22/2013
**************************************************************************************************/

public class RB_0125 : _Test
{

    [ThreadStatic]
    private static _Platform CL;
    private static Service clearChannel1;
    private static Service clearChannel2;
    private static Service clearChannel3;
    private static string maxRewind;

    private static class Constants
    {

        public const int waitInLive = 190;
        public const int timeInStby = 7;
        public const int waitInRB = 30;

    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync and clean RB");
        this.AddStep(new Step1(), "Step 1: Tune to different Channels and creat persistent RB");
        this.AddStep(new Step2(), "Step 2: Record various events from persistent RB");
        this.AddStep(new Step3(), "Step 3: Verify Archive for the recordings");
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
* @brief   Get Channel Numbers From ini File & Sync and clean RB by entering stby.
*
* @author  Adityaka
* @date    10/22/2013
**************************************************************************************************/

    private class PreCondition : _Step
    {

        public override void Execute()
        {
            StartStep();

            clearChannel1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video;IsConstantEventDuration=True;EventDuration=10", "ParentalRating=High");
            if (clearChannel1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            clearChannel2 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video;IsConstantEventDuration=True;EventDuration=10", "ParentalRating=High;LCN=" + clearChannel1.LCN);

            if (clearChannel2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            clearChannel3 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video;IsConstantEventDuration=True;EventDuration=10", "ParentalRating=High;LCN=" + clearChannel1.LCN + "," + clearChannel2.LCN);

            if (clearChannel3 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            //reading maximum rewind speend from project.ini

            maxRewind = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MAX");


            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   Tune to different Channels wait in each channel for sometime and create persistent RB
*
* @author  Adityaka
* @date    10/22/2013
**************************************************************************************************/

    private class Step1 : _Step
    {

        public override void Execute()
        {
            StartStep();

            //creating persistant RB by waiting in different serivces for sometime
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel1.LCN);
            }
            int timeLeftInSec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to get the current Event Time left");
            }
            if (timeLeftInSec < Constants.waitInLive * 3)
            {
               //Waiting till th event is completed
               CL.IEX.Wait(timeLeftInSec);
            }
            //Cleaning RB by entering stby
            res = CL.EA.FlushRB();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to standy and wakeup the STB");
            }

            res = CL.EA.STBSettings.SetGuardTime(false, "NONE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set EGT to 0");
            }
            //wait in live for the RB to get filled 
            CL.IEX.Wait(Constants.waitInLive);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel2.LCN);
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, clearChannel3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + clearChannel3.LCN);
            }

            CL.IEX.Wait(Constants.waitInLive);

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    /**********************************************************************************************/
    /**
* @class   Step2
*
* @brief   Rewing the RB until BOF and Record various events from persistent RB 
*
* @author  Adityaka
* @date    10/22/2013
**************************************************************************************************/

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Rewinding to begining of the file 
            res = CL.EA.PVR.SetTrickModeSpeed("RB", -12, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            CL.IEX.Wait(Constants.waitInRB);

            //recording events from RB playback 

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventToBeRecorded1", -1,IsPastEvent:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service ");
            }

            CL.IEX.Wait(Constants.waitInLive + 50);

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventToBeRecorded2", -1,IsPastEvent:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service ");
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EventToBeRecorded3", -1,IsPastEvent:true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service ");
            }

            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing From RB");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    /**********************************************************************************************/
    /**
* @class   Step3
*
* @brief   Check if all the recording  s are present in RB
*
* @author  Adityaka
* @date    10/22/2013
**************************************************************************************************/

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //verifying the archive for the recorded content

            res = CL.EA.PVR.VerifyEventInArchive("EventToBeRecorded1", true);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service ");
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventToBeRecorded2", true);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service ");
            }

            res = CL.EA.PVR.VerifyEventInArchive("EventToBeRecorded3", true);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event on service ");
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

