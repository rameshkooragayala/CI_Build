/// <summary>
///  Script Name : EPG_0701_ChannelBar_ProgrammeInfo.cs
///  Test Name   : EPG-0701-Channel Bar-Programme Info During Live Viewing
///  TEST ID     : 62872
///  QC Version  : 1
///  JIRA TASK   : FC-501
///  Variations from QC: Thumbnails is not validated in the script.
/// -----------------------------------------------
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("EPG_0701")]
public class EPG_0701 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service serviceWithEventInfo;
    private static Service serviceWithoutEventInfo;
    private static string eventInfoWhenNoEITS;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to service s1 and launch channel bar and verify event information is available";
    private const string STEP2_DESCRIPTION = "Step 2: Tune to service s2 and launch channel bar and verify event information is not available";

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get service From content.xml File
            serviceWithEventInfo = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            serviceWithoutEventInfo = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=False", "ParentalRating=High");

            //Get value for event information to be displayed when there is no EITS
            eventInfoWhenNoEITS = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_NO_EVT_INFO_LIVE");

            if (serviceWithEventInfo == null || serviceWithoutEventInfo == null || string.IsNullOrEmpty(eventInfoWhenNoEITS))
            {
                FailStep(CL, "One of the following is null. \n serviceWithEventInfo: " + serviceWithEventInfo + " \n serviceWithoutEventInfo: " + serviceWithoutEventInfo + "\n eventInfoWhenNoEITS" + eventInfoWhenNoEITS);
            }
            else
            {
                LogCommentInfo(CL, "Retrieved Value From XML File: serviceWithEventInfo = " + serviceWithEventInfo.LCN);
                LogCommentInfo(CL, "Retrieved Value From XML File: serviceWithoutEventInfo = " + serviceWithoutEventInfo.LCN);
                LogCommentInfo(CL, "Retrieved Value From ini File: event information to be displayed when there is no EITS = " + eventInfoWhenNoEITS);
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Channel Surf to FTA_Channel - Programme information is available
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithEventInfo.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Navigate to channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to channel bar");
            }

            //Verify Currently viewed event name
            string obtainedEventName = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            if (obtainedEventName.Equals(eventInfoWhenNoEITS))
            {
                FailStep(CL, "Event name is is displayed as: " + eventInfoWhenNoEITS);
            }
            LogCommentInfo(CL, "Obtained Event Name: " + obtainedEventName);

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Channel Surf to FTA_Programme_Info_Unavaiable - Programme information is not available
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithoutEventInfo.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Navigate to channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Verify Currently viewed event name
            string obtainedEventName = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get event name From Channel Bar");
            }

            if (!obtainedEventName.Equals(eventInfoWhenNoEITS))
            {
                FailStep(CL, "Event name is not displayed for service without EITS info. Expected: " + eventInfoWhenNoEITS + ", Obtained: " + obtainedEventName);
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}