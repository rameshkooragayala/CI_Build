/// <summary>
///  Script Name : ACTION_0002_ChannelName_Playback
///  Test Name   : ACTION-0002-Channel Logo-Not Available On Playback
///  TEST ID     : 63839
///  QC Version  : 1
///  Jira ID     : FC-257
/// -----------------------------------------------
///  Modified by : Scripted by : Madhu Renukaradhya
///  Last modified : 08 JULY 2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION_0002_ChannelName_Playback")]
public class ACTION_0002 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service serviceWithoutChannelLogo;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1:Playback the recorded content R1 and verify channel name is displayed for service which  does not have channel logo on Action Menu";

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
            //Get Values From ini File
            serviceWithoutChannelLogo = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=False;IsEITAvailable=True", "ParentalRating=High");
            if (serviceWithoutChannelLogo.Equals(null))
            {
                FailStep(CL, "serviceWithoutChannelLogo is null: " + serviceWithoutChannelLogo);
            }

            else
            {
                LogCommentInfo(CL, "serviceWithoutChannelLogo: " + serviceWithoutChannelLogo.LCN);
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithoutChannelLogo.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + serviceWithoutChannelLogo);
            }

            String expectedChannelName = serviceWithoutChannelLogo.Name;
            LogComment(CL, "Expected Channel name is" + expectedChannelName);

            //Record event and play it
            res = CL.EA.PVR.RecordCurrentEventFromBanner("RecEvent", Constants.minTimeBeforeEventEnd);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event");
            }

            //Wait to Record for 2mins
            res = CL.IEX.Wait(Constants.recordDuration);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
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
                FailStep(CL, res, "Failed to Playback");
            }

            //Wait to launch Action Menu
            res = CL.IEX.Wait(10);

            //Navigate to Action Bar

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION MENU ON PLAYBACK");
            res = CL.EA.LaunchActionBar(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu ");
            }

            String obtainedChannelLogoURL = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("channel_logo", out obtainedChannelLogoURL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel logo");
            }
            String obtainedChannelName = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out obtainedChannelName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel name");
            }
            LogComment(CL, "obtainedChannelName is " + obtainedChannelName);

            //Validating for channel name in the absence of channel logo.

            if (!String.IsNullOrEmpty(obtainedChannelLogoURL))
            {
                FailStep(CL, res, "Channel logo is present and is diplayed instead of channel name " + obtainedChannelLogoURL);
            }
            else
            {
                LogComment(CL, "obtainedChannelName is " + obtainedChannelName);
                LogComment(CL, "Expected Channel name is " + expectedChannelName);

                if (obtainedChannelName.Equals(expectedChannelName))
                {
                    LogComment(CL, "Expected Channel name recieved " + expectedChannelName);
                }

                else
                {
                    FailStep(CL, res, "Expected Channel name not recieved " + expectedChannelName);
                }
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
    }

    #endregion PostExecute
}