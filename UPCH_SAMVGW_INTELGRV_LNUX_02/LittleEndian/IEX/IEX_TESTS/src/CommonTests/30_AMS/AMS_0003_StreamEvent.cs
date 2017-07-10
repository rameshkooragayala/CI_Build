/// <summary>
///  Script Name        : AMS_0003_StreamEvent.cs
///  Test Name          : AMS-0003-Stream-Event
///  TEST ID            : 15577
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 29th OCT, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0003 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    //Channels used by the test

    private static Service Service_1;

    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file, Tune to service and change the audio of the event");
        this.AddStep(new Step1(), "Step 1: Verify the AMS tags for the Stream view event");

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
            int timeLeftInSec=0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeLeftInSec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to get the current event left time in Second");
            }
            if (timeLeftInSec < 240)
            {
                if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
                {
                    FailStep(CL, res, "Failed to set the Personalization to NO");
                }
                CL.IEX.Wait(timeLeftInSec);
            }
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;NoOfAudioLanguages=0,1");
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
                FailStep(CL, res, "Failed to tune to service " + Service_1.LCN);
            }
            CL.IEX.Wait(45);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTINGS AUDIO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:AV SETTINGS AUDIO");
            }

            string timeStamp = "";
            res = CL.IEX.SendIRCommand("SELECT_DOWN", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR command Select down");
            }
            CL.IEX.Wait(2);

            res = CL.IEX.SendIRCommand("SELECT", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR command Select down");
            }

            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to return to live viewing");
            }

            CL.IEX.Wait(45);
            //Enter Stand by
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby");
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
            CL.IEX.Wait(seconds: 480);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.StreamEvent,service:Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify the Stream Event AMS tags");
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
