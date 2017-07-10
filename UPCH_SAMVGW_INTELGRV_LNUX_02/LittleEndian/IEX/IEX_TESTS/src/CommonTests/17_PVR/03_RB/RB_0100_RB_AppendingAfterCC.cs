/// <summary>
///  Script Name : RB_0100_RB_AppendingAfterCC.cs
///  Test Name   : RB-0100-Review Buffer appending after CC
///  TEST ID     : 16021
///   JIRA ID     : FC-734
///  QC Version  : 3
/// -----------------------------------------------
///  Written by : Aditya Kambampati
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

/**********************************************************************************************//**
 * @class   RB_0100
 *
 * @brief   A rb 0100.
 *
 * @author  Adityaka
 * @date    10/22/2013
 **************************************************************************************************/

public class RB_0100 : _Test
{
    
    [ThreadStatic]
    private static _Platform CL;
    private static Service AVService1;
    private static Service AVService2;
    private static string evtInfo1;
    private static string evtInfo2_1;
    private static string evtInfo2_2;
    private static string evtInfo3;
    private static string evtInfoOnPlb1;
    private static string evtInfoOnPlb2;
    private static string evtInfoOnPlb3;
    private static string Rewind;
    private static double rewindSpeed;
    private static int evtTimeLeft = 0;
    
    private static class Constants
    {
        public const int timeInStby = 10;
        public const int waitInLive = 90;
        public const int minTimeForEventToEnd = 5;
        public const int timeInPlb = 60;
        public const int waitInRB = 45;
        public const int channelBarTimeout = 20;
        
    }

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   To check if the RB is appended to the previous RB in case of channel change .
     *          RB is checked across services to make sure all the content is available in RB  
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync and clean up RB");
        this.AddStep(new Step1(), "Step 1: Tune to different Channels");
        this.AddStep(new Step2(), "Step 2: Rewind the RB to BOF and playback");
        this.AddStep(new Step3(), "Step 3: Check if RB has all the desired content ");
        
        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition
    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Get Channel Numbers From ini File & Sync and clean up RB by entering stby .
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    private class PreCondition : _Step
    {
       
        public override void Execute()
        {
            StartStep();

            AVService1 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High");
            if (AVService1 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            AVService2 = CL.EA.GetServiceFromContentXML("IsRecordable=True;Type=Video", "ParentalRating=High;LCN=" + AVService1.LCN);

            if (AVService2 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            Rewind = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");
            rewindSpeed = double.Parse(Rewind);
            
            //flushing RB by entering stby
            //Cleaning RB by entering stby

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService1.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.EA.GetCurrentEventLeftTime(ref evtTimeLeft);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }

            if (evtTimeLeft > 90)
            {
                CL.IEX.Wait(evtTimeLeft);
            }

            res = CL.EA.FlushRB();

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to standy and wakeup the STB");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Tune to a channel wait for a while . Tune to another channel wait and come back to the first channel 
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();



            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService1.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService2.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo2_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo2_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AVService1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + AVService1.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfo3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
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
     * @brief  Rewind the RB to the BOF and play back 
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    private class Step2 : _Step
    {
        
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.SetTrickModeSpeed("RB", rewindSpeed, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind into RB");
            }

            CL.IEX.Wait(Constants.waitInRB);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }


            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfoOnPlb1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfoOnPlb2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.IEX.Wait(Constants.waitInLive);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtInfoOnPlb3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            CL.IEX.Wait(Constants.channelBarTimeout);

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

    /**********************************************************************************************//**
     * @class   Step3
     *
     * @brief   Check if all the content is there in the RB by verifying event names
     *
     * @author  Adityaka
     * @date    10/22/2013
     **************************************************************************************************/

    private class Step3 : _Step
    {
       public override void Execute()
        {
            StartStep();

            if (!(evtInfo1.Equals(evtInfoOnPlb1) && (evtInfo2_1.Equals(evtInfoOnPlb2)||evtInfo2_2.Equals(evtInfoOnPlb2)) && evtInfo3.Equals(evtInfoOnPlb3)))
            {
                FailStep(CL, "Event information doesnt match on RB playback");
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