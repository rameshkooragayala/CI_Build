/// <summary>
///  Script Name        : AMS_0403_RecordingSegments
///  Test Name          : AMS-0403-Recording-segments
///  TEST ID            : 15805
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 18th NOV, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0403 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    private static Service Service_2;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file, Record and stop the Recording and verify the AMS tags");
        this.AddStep(new Step1(), "Step 1: Resume the recording");
        this.AddStep(new Step2(), "Step 2: Stop the recording and verify the AMS tags for the Recording event");
        this.AddStep(new Step3(), "Step 3: Resume the recording, wait until the event is completed and verify the Recording AMS Tag");
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

           // if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
           // {
           //     FailStep(CL, res, "Failed to set the Personalization to YES");
           // }
            string serviceLCN = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_NUMBER");
            if (serviceLCN == "")
            {
                FailStep(CL, "Failed to fetch the Service number from Test ini");
            }
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN=" + serviceLCN, "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + "726");
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_2.LCN);
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service " + Service_1.LCN);
            }
            //Recording an event from Banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED", MinTimeBeforeEvEnd: 40, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }
            //Waiting Few minutes and stopping the record from Banner
            CL.IEX.Wait(120);
            res = CL.EA.PVR.StopRecordingFromBanner("EVENT_BASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to stop the recording from Banner");
            }
            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.RecordingEvent, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags");
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
            //Tune to a service & Tune back and verify the AMs tags for the Recording Event
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service " + Service_2.LCN);
            }

            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live,Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service " + Service_1.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED_1", MinTimeBeforeEvEnd: 20, VerifyIsRecordingInPCAT: false, IsResuming: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
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
            //Stop the recording after few seconds and verify the AMS tags for the Recording event
            CL.IEX.Wait(60);
            res = CL.EA.PVR.StopRecordingFromBanner("EVENT_BASED_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to stop the recording from Banner");
            }
            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.RecordingEvent, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags");
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
            //Resume the recording again, Wait for the Event Ends and verify the AMS tags for the recording Event
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service " + Service_2.LCN);
            }

            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to service " + Service_1.LCN);
            }
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED_2", MinTimeBeforeEvEnd: 5, VerifyIsRecordingInPCAT: false, IsResuming: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to record current event from banner");
            }
            res = CL.EA.WaitUntilEventEnds("EVENT_BASED_2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event ends");
            }

            CL.IEX.Wait(600);

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.RecordingEvent, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the AMS tags");
            }

            PassStep();
        }
    }

    #endregion Step3


    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to delete all the records from Archive");
        }

        if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
        {
            LogCommentFailure(CL, "Failed to set the Personalization to No");
        }

    }

    #endregion PostExecute
}
