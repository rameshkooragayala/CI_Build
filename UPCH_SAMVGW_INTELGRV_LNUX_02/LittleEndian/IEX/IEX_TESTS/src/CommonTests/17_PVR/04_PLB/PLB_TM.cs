/// <summary>
///  Script Name : PLB_TM.cs
///  Test Name | TEST ID : PLB-0230-TM-SD MPEG2 Clear
///  Test Name | TEST ID : PLB-0231-TM-SD MPEG2 Scrambled
///  Test Name | TEST ID : PLB-0232-TM-SD MPEG4 Clear
///  Test Name | TEST ID : PLB-0233-TM-SD MPEG2 Scrambled
///  Test Name | TEST ID : PLB-0234-TM-HD MPEG2 Clear
///  Test Name | TEST ID : PLB-0235-TM-HD MPEG2 Scrambled
///  Test Name | TEST ID : PLB-0236-TM-HD MPEG4 Clear
///  Test Name | TEST ID : PLB-0237-TM-HD MPEG2 Scrambled
///  QC Version  : 
/// ----------------------------------------------- 
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PLB_TM : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    static double[] TM_Array = { 0.5, 2, -2, 6, -6, 12, -12, 30, -30 };
    static int PLBduration = 30;
    static string eventToBeRecorded = "EVENT_TO_BE_RECORDED"; //The event to be recorded
    static Service service = new Service();


    //Constants used in the test
    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 5; // In minutes
        public const int minEventDuration = 30; // In Minutes
        public const bool isSGT = false;
    }

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync");
        this.AddStep(new Step1(), "Step 1: Record an Event From Banner");
        this.AddStep(new Step2(), "Step 2: Verify Recordings");
        this.AddStep(new Step3(), "Step 3: Verify Playback, Trick-Modes and Information");

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
            string serviceType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_TYPE");

            service = CL.EA.GetServiceFromContentXML(serviceType, "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            string maxEGTstr = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "MAX");
            if (String.IsNullOrEmpty(maxEGTstr))
            {
                FailStep(CL, "Max EGT value not present in Project.ini file.");
            }

            // Set EGT to Max
            res = CL.EA.STBSettings.SetGuardTime(Constants.isSGT, maxEGTstr);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set EGT to Max - " + maxEGTstr);
            }

            //Tune to the service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
            }

            PassStep();
        }
    }
    #endregion
    #region Step1


    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(eventToBeRecorded, Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule recording");
            }

            LogCommentInfo(CL, "Waiting for " + Constants.minEventDuration + " minutes to ensure sufficient duration of recording to test all trickmodes");
            res = CL.IEX.Wait(Constants.minEventDuration * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for minimum event time required");
            }

            // Stop Recording
            res = CL.EA.PVR.StopRecordingFromArchive(eventToBeRecorded, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop recording from Archive");
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

            //Verify recordig on library
            res = CL.EA.PVR.VerifyEventInArchive(eventToBeRecorded, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Recording in Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Use fast rewind (x2 x6 x12 x30) tricks modes,
        //Use fast forward (x2 x6 x12 x30) tricks mode,
        //Activate pause trick mode,
        //Activate play trick mode & slow motion
        public override void Execute()
        {
            StartStep();

            res = CL.EA.PVR.PlaybackRecFromArchive(eventToBeRecorded, 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event " + eventToBeRecorded + " From Archive");
            }

            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present During Playback");
            }

            //Play all trick-modes speeds {0.5,2,-2,6,-6,12,-12,30,-30}
            foreach (double TM in TM_Array)
            {
                res = CL.EA.PVR.SetTrickModeSpeed(eventToBeRecorded, TM, false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Set TM  x" + TM + " Speed");
                }
                CL.IEX.Wait(PLBduration);

            }
            //Stop Playing 
            res = CL.EA.PVR.StopPlayback(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Playback");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Delete all recordings in archive
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete records from archive because of the reason:" + res.FailureReason);
        }

        //Getting Default EGT from Project.ini
        string defaultEGT = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (String.IsNullOrEmpty(defaultEGT))
        {
            LogCommentFailure(CL, "Default EGT value not present in Project.ini file.");
        }
        // Set EGT value to default
        res = CL.EA.STBSettings.SetGuardTime(Constants.isSGT, defaultEGT);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set EGT to default");
        }

    }
    #endregion
}