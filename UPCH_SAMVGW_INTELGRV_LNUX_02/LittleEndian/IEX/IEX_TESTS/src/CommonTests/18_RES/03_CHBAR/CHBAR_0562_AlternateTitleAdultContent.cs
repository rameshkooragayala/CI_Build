/// <summary>
///  Script Name : CHBAR_0562_AlternateTitleAdultContent.cs
///  Test Name   : EPG-0562-Channel Bar-Alternative Title For Adult Content For Other Channel
///  TEST ID     : 64223
///  JIRA ID     : FC-466
///  QC Version  : 1
///  Variations from QC: None
/// -----------------------------------------------
///  Modified by : Avinob Aich
///  Modified Date: 17/07/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("CHBAR_0562_AlternateTitleAdultContent_live")]
public class CHBAR_0562 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service liveChannel;
    private static Service adultChannel;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to a Channel";
    private const string STEP2_DESCRIPTION = "Step 2: Launch Channel Bar and browse to Adult Service";

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

            //Get Values From xml File
            liveChannel = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            adultChannel = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High", "");

            if (liveChannel.Equals(null) || adultChannel.Equals(null))
            {
                FailStep(CL, "No Channels found for the parameters passed in GetServiceFromContentXML");
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

            //Get Value from Project.ini for Audio and Video time out
            string audioTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC");
            string videoTimeOut = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");

            //Tune to a channel which is not locked
            res = CL.EA.TuneToChannel(liveChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Channel: " + liveChannel.LCN);
            }

            //Check if Audio is present
            res = CL.EA.CheckForAudio(true, int.Parse(audioTimeOut));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify audio in channel: " + liveChannel.LCN);
            }

            //Check if Video is present
            res = CL.EA.CheckForVideo(true, false, int.Parse(videoTimeOut));
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Check video in channel: " + liveChannel.LCN);
            }

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

            string adultChannelPosition = adultChannel.PositionOnList; //position of adult channel in list
            string liveChannelPosition = liveChannel.PositionOnList; //position of unlocked channel in list
            bool isNext = true; // surf on next channel on channel bar
            string obtainedAdultEventName = "";

            //Get alternate event name for Adult Event from Project.ini
            string expectedAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");

            int noOfPresses = int.Parse(adultChannelPosition) - int.Parse(liveChannelPosition);

            if (noOfPresses < 0)
            {
                isNext = false; //surf on previous channel from channel bar
                noOfPresses = Math.Abs(noOfPresses);
            }

            //Surf from channel bar till adult channel
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", isNext, noOfPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to Adult Channel: " + adultChannel.LCN + " from Channel Bar");
            }

            //Get event name for Locked Event
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedAdultEventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel name from channel bar");
            }

            //Cheking if adult event name is alternate title
            if (obtainedAdultEventName != expectedAdultEventName)
            {
                FailStep(CL, "Event Name of Adult Event is not Alternate title");
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