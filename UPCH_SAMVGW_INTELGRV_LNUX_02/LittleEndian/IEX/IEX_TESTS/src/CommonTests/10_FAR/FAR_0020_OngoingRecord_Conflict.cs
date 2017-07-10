/// <summary>
///  Script Name : FAR_0020_OngoingRecord_Conflict.cs
///  Test Name   : FAR-0020-Factory reset conflict with recording
///  TEST ID     : 68981
///  QC Version  : 1
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using FailuresHandler;
using IEX.Tests.Reflections;

[Test("FAR_0020_OngoingRecord_Conflict")]
public class FAR_0020 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static Service videoService;
	
	private static string defaultPin;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Start recoding";
    private const string STEP1_DESCRIPTION = "Step 1: Perform factory reset and validate if it gets cancelled due to ongoing record";
    private const string STEP2_DESCRIPTION = "Step 2: Book a record within one hour";
    private const string STEP3_DESCRIPTION = "Step 3: Perform factory reset and validate if it gets cancelled due to booking";

    private static class Constants
    {
        public const int minTimeBeforeEvtEnds = 2;
        public const bool keepRecordings = false;
        public const bool keepSettings = false;
    }
    
    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
			
			defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }
			
            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video service fetched from Content.xml is null");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to surf to Video service: " + videoService.LCN);  
            }

            //Record and Event
            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event", Constants.minTimeBeforeEvtEnds);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to record current event");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            //Perform Factory reset with Keep Recording option NO
            res = CL.EA.STBSettings.FactoryReset(Constants.keepRecordings, Constants.keepSettings, defaultPin);
            if (!res.CommandSucceeded && res.FailureCode == ExitCodes.FactoryResetPVRFailure.GetHashCode())
            {
                LogCommentInfo(CL, "Got PVR exception during Factory Reset as expected");
            }
            else
            {
                FailStep(CL, "Did not get PVR exception while performing factory reset");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to surf to Video service: " + videoService.LCN);
            }

            res = CL.EA.PVR.StopRecordingFromBanner("Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to stop record from banner");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.PVR.BookFutureEventFromGuide("Future_Event", videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to book a record from guide");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.STBSettings.FactoryReset(Constants.keepRecordings, Constants.keepSettings, defaultPin);
            if (!res.CommandSucceeded && res.FailureCode == ExitCodes.FactoryResetPVRFailure.GetHashCode())
            {
                LogCommentInfo(CL, "Got PVR exception during Factory Reset as expected");
            }
            else
            {
                FailStep(CL, "Did not get PVR exception while performing factory reset");
            }

            PassStep();
        }
    }
    #endregion
    
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}