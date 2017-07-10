/// <summary>
///  Script Name          : GRID_1511_System_Message.cs
///  Test Name            : GRID-1511-Programme-Grid-System-Message-screen
///  TEST ID              : 67884
///  JIRA Issue ID        : FC-438
///  QC Version           : 2
/// -----------------------------------------------
///  Modified by          : Bharath Pai
///  Deviations from HPQC : PMT removal of a particular service cannot be done on runtime 
///  and hence this part is not taken care of.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

public class GRID_1511 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static Service service1;                //First service
    static Service service2;                //Second service

    //Shared members used in the test
    static string videoCoordForOverlay;     //Co-ordinates for Overlay Video
    static int defaultVideoCheckTimeout;    //Default video check timeout
    static int defaultAudioCheckTimeout;    //Default Audio check timeout
    static bool isRFActive = true;          //Flag to check whether RF is active
    static int timeToPopulateGridInfo;      //Time to populate Grid info    
    #region Constants
    //Constants used in the test
    static class Constants
    {
        public const string preconditionDescription = "Precondition: Fetch services required for the test case.";
        public const string step1Description = "Step 1: Set the Guide appearance as Overlay mode.";
        public const string step2Description = "Step 2: Tune to service S1 and Launch Program Grid.";
        public const string step3Description = "Step 3: Focus on service S2.";
        public const string step4Description = "Step 4: Unplug signal and check for System message";
        public const string step5Description = "Step 5: Plug back signal and check for video";
    }
    #endregion

    #region Create Structure
    public override void CreateStructure()
    {
        //Adding steps of the test case
        AddStep(new PreCondition(), Constants.preconditionDescription);
        AddStep(new Step1(), Constants.step1Description);
        AddStep(new Step2(), Constants.step2Description);
        AddStep(new Step3(), Constants.step3Description);
        AddStep(new Step4(), Constants.step4Description);
        AddStep(new Step5(), Constants.step5Description);

        //Get Client Platform
        CL = GetClient();

    }
    #endregion

    #region Steps

    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //State a warning saying that this script requires RF removal
            LogCommentWarning(CL, "This test requires RF switch to create RF removal scenario!!");
            LogCommentWarning(CL, "Port A will have stream connected and Port B should be left open!!");

            //Fetch the first service
            service1 = CL.EA.GetServiceFromContentXML("Type=Video", "IsDefault=True;ParentalRating=High");
            if (service1 == null)
            {
                FailStep(CL, "Failed to fetch the first service matching the given criteria.");
            }

            //Fetch the second service
            service2 = CL.EA.GetServiceFromContentXML("Type=Video", "IsDefault=True;ParentalRating=High;LCN=" + service1.LCN);
            if (service2 == null)
            {
                FailStep(CL, "Failed to fetch the second service matching the given criteria.");
            }

            //Fetch the co-ordinates for Overlay video
            videoCoordForOverlay = CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "COORDINATES_FOR_OVERLAY_VIDEO");
            if (String.IsNullOrEmpty(videoCoordForOverlay))
            {
                FailStep(CL, res, "COORDINATES_FOR_OVERLAY value is not present in the Project.ini");
            }

            //Fetch the default timeout for Video check
            String defaultVideoCheckTimeInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_VIDEO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultVideoCheckTimeInStr))
            {
                FailStep(CL, res, "DEFAULT_VIDEO_CHECK_SEC value is not present in the Project.ini");
            }
            defaultVideoCheckTimeout = int.Parse(defaultVideoCheckTimeInStr);

            //Fetch the default timeout for Audio check
            String defaultAudioCheckTimeInStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DEFAULT_AUDIO_CHECK_SEC");
            if (String.IsNullOrEmpty(defaultAudioCheckTimeInStr))
            {
                FailStep(CL, res, "DEFAULT_AUDIO_CHECK_SEC value is not present in the Project.ini");
            }
            defaultAudioCheckTimeout = int.Parse(defaultAudioCheckTimeInStr);
            timeToPopulateGridInfo = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "TIME_TO_POPULATE_INFO"));
            LogCommentInfo(CL, "timeToPopulateGridInfo value retrieved from project ini is: " + timeToPopulateGridInfo);

            PassStep();
        }
    }
    #endregion

    #region Step 1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Set the TV Guide background to Overlay mode
            res = CL.EA.STBSettings.SetTvGuideBackgroundAsSolid();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set Guide appearance to solid background.");
            }

            PassStep();
        }
    }
    #endregion

    #region Step 2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Tune to first service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune on live to service - " + service1.LCN);
            }

            //Navigate to Guide
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to guide!");
            }

            //Check for video on overlay
            res = CL.EA.CheckForVideo(videoCoordForOverlay, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for video on overlay");
            }

            //Check for audio on overlay
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio in overlay mode");
            }

            PassStep();
        }
    }
    #endregion

    #region Step 3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Tune to service S2 through guide
            CL.EA.UI.Utils.SendChannelAsIRSequence(service2.LCN);
            res = CL.IEX.Wait(timeToPopulateGridInfo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune on Guide to service - " + service2.LCN);
            }

            //Check for video on overlay
            res = CL.EA.CheckForVideo(videoCoordForOverlay, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for video on overlay");
            }

            //Check for audio on overlay
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio on overlay");
            }

            PassStep();
        }
    }
    #endregion

    #region Step 4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Unplug RF signal
            res = CL.IEX.RF.TurnOff("1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unplug RF signal!");
            }
            isRFActive = false;

            //Check if Video is not present on overlay
            res = CL.EA.CheckForVideo(videoCoordForOverlay, false, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Getting video on overlay even after unplugging signal");
            }

            //Check if Audio is not present on Overlay
            res = CL.EA.CheckForAudio(false, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Getting audio in overlay even after unplugging signal");
            }

            PassStep();
        }
    }
    #endregion

    #region Step 5
    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Plug back RF signal
            res = CL.IEX.RF.ConnectToA("1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to plug back RF signal!");
            }
            isRFActive = true;

            //Check for video on overlay
            res = CL.EA.CheckForVideo(videoCoordForOverlay, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for video on overlay");
            }

            //Check for audio on overlay
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio on overlay");
            }

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Plug back RF if there was failure in reconnection
        if (!isRFActive)
        {
            res = CL.IEX.RF.ConnectToA();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to plug back RF signal!");
                //FailStep(CL, res, "Failed to plug back RF signal!");
            }
        }
    }
    #endregion
}