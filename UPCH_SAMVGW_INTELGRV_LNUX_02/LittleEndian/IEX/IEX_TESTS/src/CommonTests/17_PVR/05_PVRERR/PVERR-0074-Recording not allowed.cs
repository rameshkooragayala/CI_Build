/// <summary>
///  Script Name : PVERR-0074-Recording not allowed.cs
///  Test Name   : PVERR_0074_Recording_not_allowed
///  TEST ID     : 71571
///  QC Version  : 2
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified date: 11.11.2013
/// </summary>

using System;
using System.Linq;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("PVERR_0074")]
public class PVERR_0074 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service nonRecordableService;
    static string startGuardTime;
    static string endGuardTime;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Book a Future Event on a Non Recordable Service and wait till the Event Ends";
    private const string STEP2_DESCRIPTION = "Step 2: Verify  Event Error Info is failed with  Proper Description ";



    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        //this.AddStep(new Step2(), STEP2_DESCRIPTION);
        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    private static class Constant
    {
        public const int noOfPresses = 1;
        public const bool verifyInPCAT = false;
        public const bool sgtToSet = true;
        public const bool egtToSet = false;
    }

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

            //Get Values From xml File
            nonRecordableService = CL.EA.GetServiceFromContentXML("IsRecordable=False");
            if (nonRecordableService == null)
            {
                FailStep(CL, "Failed to fetch channel from Content.xml for the passed criterion");
            }
            LogCommentInfo(CL, "Channel fetched from Content.xml: " + nonRecordableService.LCN);

            //get value from project.ini for EGT
            string startGuardTimeList = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "LIST");
            if (string.IsNullOrEmpty(startGuardTimeList))
            {
                FailStep(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
            }


            startGuardTime = startGuardTimeList.Split(',').First();

            //set EGT to first value of list
            res = CL.EA.STBSettings.SetGuardTime(Constant.sgtToSet, startGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set Start Guard Time to " + startGuardTime);
            }

            //get value from project.ini for EGT
            string endGuardTimeList = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "LIST");
            if (string.IsNullOrEmpty(endGuardTimeList))
            {
                FailStep(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
            }


            endGuardTime = endGuardTimeList.Split(',').First();

            //set EGT to first value of list
            res = CL.EA.STBSettings.SetGuardTime(Constant.egtToSet, endGuardTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set End Guard Time to " + endGuardTime);
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

            //get friendly name for SGT
            int startGuardTimeNum = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(startGuardTime, true);
            res = CL.EA.PVR.BookFutureEventFromGuide("recEvent", nonRecordableService.LCN, Constant.noOfPresses, startGuardTimeNum + 2, Constant.verifyInPCAT);
            if (res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book future Event from Guide");
            }
			else
			{
			    LogCommentInfo(CL, "Cannot do recording on non recordable service");
			}

            //get friendly name  for end guard time
            //int endGuardTimeNum = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(endGuardTime, false);
            //LogCommentInfo(CL, "Waiting till the event ends");
            //res = CL.EA.WaitUntilEventEnds("recEvent",endGuardTime);
            //if (!res.CommandSucceeded)
            //{
             //   FailStep(CL, res, "Failed to wait untill the booked event ends");
            //}

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

            //verify record Error Info
            res = CL.EA.PVR.VerifyRecordErrorInfo("recEvent", EnumRecordErr.Failed_UnSubscribedChannel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Record Error Info for the Failed Event");
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
        IEXGateway._IEXResult res;

        //delete the failed recorded event
        res = CL.EA.PVR.DeleteFailedRecordedEvent("recEvent");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Delete Failed Recorded Event");
        }

        //get default value from project.ini for EGT
        string defaultStartGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "SGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultStartGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
        }

        //set SGT to default
        res = CL.EA.STBSettings.SetGuardTime(Constant.sgtToSet, defaultStartGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultStartGuardTime);
        }

        //get default value from project.ini for EGT
        string defaultEndGuardTime = CL.EA.GetValueFromINI(EnumINIFile.Project, "EGT", "DEFAULT");
        if (string.IsNullOrEmpty(defaultEndGuardTime))
        {
            LogCommentFailure(CL, "Failed to fetch LIST from Project.in for SGT or EGT");
        }

        //set SGT to default
        res = CL.EA.STBSettings.SetGuardTime(Constant.egtToSet, defaultEndGuardTime);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, res.FailureReason + "; Failed to set End Guard Time to " + defaultEndGuardTime);
        }
    }
    #endregion
}