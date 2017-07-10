/// <summary>
///  Script Name : PLB_RB_TM.cs
///  Test Name | TEST ID : PLB-0240-RB TM-SD MPEG2 Clear
///  Test Name | TEST ID : PLB-0241-RB TM-SD MPEG2 Scrambled
///  Test Name | TEST ID : PLB-0242-RB TM-SD MPEG4 Clear
///  Test Name | TEST ID : PLB-0243-RB TM-SD MPEG4 Scrambled
///  Test Name | TEST ID : PLB-0244-RB TM-HD MPEG2 Clear
///  Test Name | TEST ID : PLB-0245-RB TM-HD MPEG2 Scrambled
///  Test Name | TEST ID : PLB-0246-RB TM-HD MPEG4 Clear
///  Test Name | TEST ID : PLB-0247-RB TM-HD MPEG4 Scrambled
///  QC Version  :
/// -----------------------------------------------
///  Modified by : Francis Lobo
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class PLB_RB_TM : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    static Service service;
    private static String[] TM_Array;
    //Constants used in the test
    private static class Constants
    {
        public const int RBDurationInMin = 3;
    }
    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync");
        this.AddStep(new Step1(), "Step 1: Enter & Exit From Standby");
        this.AddStep(new Step2(), "Step 2: Tune to Live Channel Using DCA");
        this.AddStep(new Step3(), "Step 3: Verify RB Playback, Trick-Modes and Information");

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

            //Get Values From ini File
            string serviceType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_TYPE");
            if (string.IsNullOrEmpty(serviceType))
            {
                FailStep(CL, res, "Failed to fetch Service type from test ini");
            }

            service = CL.EA.GetServiceFromContentXML(serviceType, "ParentalRating=High");
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogComment(CL, "Service fetched from Content XML is " + service.LCN);
            }

            //Fetch the trickmode
            String trickModeArrayInStr = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LIST_TM");
            if (String.IsNullOrEmpty(trickModeArrayInStr))
            {
                FailStep(CL, "Trick mode list not present in Test ini file.");
            }
            TM_Array = trickModeArrayInStr.Split(',');

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

            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.EA.CheckForVideo(false, false,Timeout:15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed: Video is Present After Standby");
            }

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }
            res = CL.EA.CheckForVideo(true, false, Timeout:60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present After Exiting From Standby");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            res = CL.EA.CheckForVideo(true, false, Timeout:60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify that Video is Present");
            }

           res= CL.IEX.Wait(Constants.RBDurationInMin * 60);
           if (!res.CommandSucceeded)
           {
               LogCommentWarning(CL,"Failed to Wait");
           }
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

            //play all trick modes speeds 
            foreach (string TM in TM_Array)
            {
                res = CL.EA.PVR.SetTrickModeSpeed("RB", Convert.ToDouble(TM), true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Set RB TM x" + TM + "Speed to EOF");
                }
                CL.IEX.Wait(seconds:5);
            }

            //play at slow motion in RB can't be till end of file
            res = CL.EA.PVR.SetTrickModeSpeed("RB",Speed:0.5,Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set RB TM x0.5 Speed to EOF");
            }
            CL.IEX.Wait(seconds:30);

            //Stop Playing
            res = CL.EA.PVR.StopPlayback(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing from RB");
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

