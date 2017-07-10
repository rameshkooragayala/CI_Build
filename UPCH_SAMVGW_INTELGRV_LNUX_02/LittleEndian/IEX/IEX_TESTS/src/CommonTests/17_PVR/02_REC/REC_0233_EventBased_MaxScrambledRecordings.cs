/// <summary>
///  Script Name : REC_0233_EventBased_MaxScrambledRecordings.cs
///  Test Name   : REC-0233-Event Based-Max Simul Scrambled Recordings
///  TEST ID     : 60617
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class REC_0233 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string[] ChannelArray;
    private static int MaxRec;
    private static string Live_Service;
    private static int MinRecSecs = 60 * 4;

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Book Max Simultaneous recordings");
        this.AddStep(new Step2(), "Step 2: Tune to another service wait for RB and Recordings ");
        this.AddStep(new Step3(), "Step 3: Playback the recordings");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Fetch channel numbers
            string channelList = "";
            channelList = CL.EA.GetValue("SCR_LIST");
            ChannelArray = channelList.Split('_');

            //MaxRec is context dependant- hardcoding it for now. but has to be made dynamic based on gateway or client and is also dependant on project
            MaxRec = 2;
            Live_Service = ChannelArray[MaxRec];

            if (ChannelArray.Length < MaxRec + 1)
            {
                FailStep(CL, "Not enough scrambled services to test");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Check if each event has atleast MinRecMinutes
            for (int i = 0; i < MaxRec; i++)
            {
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, ChannelArray[i]);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel " + ChannelArray[i]);
                }

                //Check enough time left for the event
                int timeToEventEnd_sec = 120;
                res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Get Time Left to Current Event");
                }

                int MaxThresh = MinRecSecs;
                int MinThresh = 20;
                if ((MinThresh <= timeToEventEnd_sec) && (timeToEventEnd_sec <= MaxThresh))
                {
                    //Book Future
                    res = CL.EA.PVR.BookFutureEventFromBanner("RecEvent_" + i, 1, 1, false, true);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Book Future Event From Banner on service " + ChannelArray[i] + ", Loop# " + i);
                    }
                }
                else
                {
                    if (timeToEventEnd_sec <= MinThresh)
                    {
                        CL.IEX.Wait(MinThresh);
                    }

                    //Book Current
                    res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent_" + i);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to Book Current Event From Guide on service " + ChannelArray[i] + ", Loop# " + i);
                    }
                }
            }
            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Live_Service);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Live_Service + "after booking " + MaxRec + " recordings");
            }

            //wait for RB and Recordings
            CL.IEX.Wait(MinRecSecs);

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            for (int i = 0; i < MaxRec; i++)
            {
                res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent_" + i, 60, true, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Playback Event From Archive for service " + ChannelArray[i] + ", Loop# " + i);
                }
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