/// <summary>
///  Script Name        : AMS_End_Of_PlayBack.cs
///  Test Name          : AMS-0504-End-of-Playback, AMS-0507-End-of-RB-Playback
///  TEST ID            : 15810, 15987
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 06th NOV, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_End_Of_PlayBack : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file, collect RB and play till EOF and Verify AMS TAGS");
        this.AddStep(new Step1(), "Step 1: Record an event play till EOF and Verify AMS TAGS for End of playback event");

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

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+Service_1.LCN);
            }
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 0, Verify_EOF_BOF: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to set the speed to 0");
            }
            CL.IEX.Wait(60);
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Speed: 2, Verify_EOF_BOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the speed to 0");
            }
            CL.IEX.Wait(550);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.EndOfPlaybackFile, service: Service_1, IsRBPlayback: "TRUE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify the AMS tagsfor End of Playback Event");
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
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
            {
                FailStep(CL, res, "Failed to set the Personalization to NO");
            }
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }
			CL.IEX.Wait(10);
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENTBASED", VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record current event from banner");
            }
            CL.IEX.Wait(60);
            res = CL.EA.PVR.StopRecordingFromBanner("EVENTBASED");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL,"Failed to stop the recording from Banner");
            }
            res = CL.EA.PVR.PlaybackRecFromArchive("EVENTBASED", SecToPlay: 0, FromBeginning: true, VerifyEOF: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to playback the record till EOF");
            }
            CL.IEX.Wait(500);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.EndOfPlaybackFile, service: Service_1, IsRBPlayback: "FALSE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify the AMS End of Playback Event");
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PlaybackEvent, service: Service_1, IsRBPlayback: "FALSE", Speed: 1000);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Playback Event with Speed 1");
            }
            PassStep();
        }
    }

    #endregion Step1




    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
        {
            LogCommentFailure(CL, "Failed to set the Personalization to No");
        }

    }

    #endregion PostExecute
}
