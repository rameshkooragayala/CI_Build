/// <summary>
///  Script Name : ACTION_0001_ChannelLogo_Playback
///  Test Name   : ACTION-0001-Channel Logo-Available On Playback
///  TEST ID     : 63822
///  Jira ID     : FC-267
///  QC Version  : 1
///  Variations from QC:None
/// -----------------------------------------------
///  Modified by : Scripted by : Madhu Renukaradhya
///  Last modified : 24 May 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION_0001_ChannelLogo_Playback")]
public class ACTION_0001 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service serviceWithChannelLogo;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Playback the recorded content R1 and verify channel logo is displayed on Action menu in its presence ";

    private static class Constants
    {
        public const int minTimeBeforeEventEnd = 5;
        public const int recordDuration = 2 * 60;
        public const int secToPlay = 0;
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);

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

            //Get Values From Content XML File
            serviceWithChannelLogo = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True;IsEITAvailable=True", "ParentalRating=High");
            if (serviceWithChannelLogo == (null))
            {
                FailStep(CL, "Failed to fetch the service with channel logo from content xml.");
            }

            else
            {
                LogCommentInfo(CL, "ServiceWithChannelLogo: " + serviceWithChannelLogo.LCN);
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithChannelLogo.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + serviceWithChannelLogo);
            }


            //Record event and play it
            res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent", Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event");
            }

            //Wait to Record for 2 mins
            res = CL.IEX.Wait(Constants.recordDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res ,"Failed while waiting to record");
            }

            res = CL.EA.PVR.StopRecordingFromBanner("RecEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop Recording Current Event From Live");
            }

            //Playback the Already Recorded Event from Archive.
            res = CL.EA.PVR.PlaybackRecFromArchive("RecEvent", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res,"Failed to play back from archieve");
            }

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Navigate to Action Bar
            res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on Playback");
            }

            String obtainedChannelLogo = CL.EA.GetChannelLogo();
            LogComment(CL, "Obtained ChannelLogo is =  " + obtainedChannelLogo);

            if (!String.IsNullOrEmpty(obtainedChannelLogo))
            {
                  LogComment(CL, "Channel logo is present = " + obtainedChannelLogo);
            }

            else
             {
                    FailStep(CL, res, "Channel logo is empty ");
             }
            res = CL.IEX.MilestonesEPG.Navigate("STOP");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to Stop recording from Archieve");
            }

            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.NavigateToArchive();
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to navigate Archieve");
        }

        res = CL.EA.PVR.DeleteRecordFromArchive("RecEvent");
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to Delete recording from Archieve");
        }
    }

    #endregion PostExecute
}