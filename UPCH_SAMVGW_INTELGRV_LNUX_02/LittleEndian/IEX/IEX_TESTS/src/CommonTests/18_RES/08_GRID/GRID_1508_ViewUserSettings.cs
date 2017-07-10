/// <summary>
///  Script Name : GRID_1508_ViewUserSettings.cs
///  Test Name   : GRID-1508-ViewUserSettings
///  TEST ID     : 68034
///  QC Version  : 2
///  Variations from QC:none
///  JIRA ID:FC-551
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Renukaradhya 
///  Last modified : 21 Aug 2013
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("GRID_1508")]
public class GRID_1508 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static Service service1;                //First service
    static Service service2;                //Second service

    //Shared members between steps
    static string videoCoordForPip;         //Co-ordinates for PIP Video
    static string videoCoordForMiniTv;     //Co-ordinates for Mini Tv Video
    static int defaultVideoCheckTimeout;    //Default video check timeout
    static int defaultAudioCheckTimeout;    //Default Audio check timeout
    static string firstTunedServiceName;    //First tuned service name
    static string secondTunedServiceName;   //Second tuned service name
    static int timeToPopulateGridInfo;      //Time to populate Grid info    

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    public const string  STEP1_DESCRIPTION = "Step 1: Set the Guide appearance as Overlay mode.";
    private const string STEP2_DESCRIPTION = "Step 2: Tune to service S1 and Launch Program Grid and verify overlay video and audio of service S1 is displayed";
    private const string STEP3_DESCRIPTION = "Step 3: Tune to service S2 and verify that overlay video,PIP and audio is displayed.";
    private const string STEP4_DESCRIPTION = "Step 4: Set the Guide appearance as MINI TV mode and repeat the steps";


    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);

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

            //Get Values From ini File
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

            //Fetch the co-ordinates for MINI TV video
            videoCoordForMiniTv = CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "COORDINATES_FOR_MINITV_VIDEO");
            if (String.IsNullOrEmpty(videoCoordForMiniTv))
            {
                FailStep(CL, res, "COORDINATES_FOR_MINITV_VIDEO value is not present in the Project.ini");
            }

             //Fetch the co-ordinates for PIP video

            videoCoordForPip = CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "COORDINATES_FOR_PIP");
            if (String.IsNullOrEmpty(videoCoordForPip))
            {
                FailStep(CL, res, "COORDINATES_FOR_PIP value is not present in the Project.ini");
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
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Set the TV Guide background to Overlay(Transparent) mode
            res = CL.EA.STBSettings.SetTvGuideBackgroundAsTransparent();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set Guide appearance to Transparent background.");
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
            
            //Tune to first service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune on live to service1 - " + service1.LCN);
            }

            firstTunedServiceName = service1.Name;
            LogCommentInfo(CL, "First tuned service name is" + firstTunedServiceName);

            //Navigate to Guide
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to guide!");
            }
            CL.IEX.Wait(10);

           //Fetch the focused service name on GRID
            String firstTunedServiceNameOnGrid = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out firstTunedServiceNameOnGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get service name on GRID");
            }
            else
            {
                LogCommentInfo(CL, "Service name is: " + firstTunedServiceNameOnGrid);
            }

            //verify default focus on grid is on current viewing channel.
            if (firstTunedServiceNameOnGrid.Equals(firstTunedServiceName))
            {
                LogCommentInfo(CL, "Focus on grid is on current viewing channel");
            }
            else
            {
                FailStep(CL,"Failed to focus current viewing channel on GRID");
            }


            //Check for video on PIP
            res = CL.EA.CheckForVideo(videoCoordForPip, false, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video is present in PIP/resized window area");
            }
            else
            {
                LogCommentInfo(CL, "The resized window is not displayed for the current event of current service S1");
            }
            //Check for audio
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio in overlay mode");
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

            secondTunedServiceName = service2.Name;
            LogCommentInfo(CL, "Second tuned service name is: " + secondTunedServiceName);

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Tune to second service
            CL.EA.UI.Utils.SendChannelAsIRSequence(service2.LCN);
            res = CL.IEX.Wait(timeToPopulateGridInfo);
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL,"Failed while waiting during data population on GRID");
            }
            //Fetch the focused service name on GRID
            String secondTunedServiceNameOnGrid = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out secondTunedServiceNameOnGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get service name on GRID");
            }
            else
            {
                LogCommentInfo(CL, "Second tuned service name on GRID is: " + secondTunedServiceNameOnGrid);
            }

            //verify default focus on grid is on current viewing channel.
            if (secondTunedServiceNameOnGrid.Equals(secondTunedServiceName))
            {
                LogCommentInfo(CL, "Focus on grid is on current viewing channel");
            }
            else
            {
                FailStep(CL, "Failed to focus current viewing channel on GRID");
            }

            //Check for video on PIP
            res = CL.EA.CheckForVideo(videoCoordForPip, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video is not present in PIP/resized window area");
            }
            else
            {
                LogCommentInfo(CL, "Video of the current event of S2 is displayed in resized window");
            }


            //Check for audio
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio in overlay mode");
            }

            PassStep();
        }
    }
    #endregion

    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Set the TV Guide background to Mini TV(Solid) mode
            res = CL.EA.STBSettings.SetTvGuideBackgroundAsSolid();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set Guide appearance to Solid background.");
            }

            //Verification on transparent backgroud.
            //Tune to first service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune on live to service1 - " + service1.LCN);
            }

            firstTunedServiceName = service1.Name;
            LogCommentInfo(CL, "First tuned service name is" + firstTunedServiceName);

            //Navigate to Guide
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to guide!");
            }
             CL.IEX.Wait(10);
            //Fetch the focused service name on GRID
            String firstTunedServiceNameOnGrid = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out firstTunedServiceNameOnGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get service name on GRID");
            }
            else
            {
                LogCommentInfo(CL, "Service name is: " + firstTunedServiceNameOnGrid);
            }

            //verify default focus on grid is on current viewing channel.
            if (firstTunedServiceNameOnGrid.Equals(firstTunedServiceName))
            {
                LogCommentInfo(CL, "Focus on grid is on current viewing channel");
            }
            else
            {
                FailStep(CL, "Failed to focus current viewing channel on GRID");
            }


            //Check for video on PIP
            res = CL.EA.CheckForVideo(videoCoordForPip, false, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video is present in PIP/resized window area");
            }
            else
            {
                LogCommentInfo(CL, "The resized window is not displayed for the current event of current service S1");
            }

            //Check for video on Mini TV

            res = CL.EA.CheckForVideo(videoCoordForMiniTv, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video is not present in MiniTV area");
            }

            //Check for audio
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio in overlay mode");
            }


            secondTunedServiceName = service2.Name;
            LogCommentInfo(CL, "Second tuned service name is: " + secondTunedServiceName);

            //Clear EPGInfo
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Tune to second service
            CL.EA.UI.Utils.SendChannelAsIRSequence(service2.LCN);
            res = CL.IEX.Wait(timeToPopulateGridInfo);
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL, "Failed while waiting during data population on GRID");
            }
            //Fetch the focused service name on GRID
            String secondTunedServiceNameOnGrid = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out secondTunedServiceNameOnGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get service name on GRID");
            }
            else
            {
                LogCommentInfo(CL, "Second tuned service name on GRID is: " + secondTunedServiceNameOnGrid);
            }
            CL.IEX.Wait(10);

            //verify default focus on grid is on current viewing channel.
            if (secondTunedServiceNameOnGrid.Equals(secondTunedServiceName))
            {
                LogCommentInfo(CL, "Focus on grid is on current viewing channel");
            }
            else
            {
                FailStep(CL, "Failed to focus current viewing channel on GRID");
            }

            //Check for video on PIP
            res = CL.EA.CheckForVideo(videoCoordForPip, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video is not present in PIP/resized window area");
            }
            else
            {
                LogCommentInfo(CL, "Video of the current event of S2 is displayed in resized window");
            }

            //Check for video on Mini TV

            res = CL.EA.CheckForVideo(videoCoordForMiniTv, true, defaultVideoCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Video is not present in MiniTV area");
            }

            //Check for audio
            res = CL.EA.CheckForAudio(true, defaultAudioCheckTimeout);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to check for audio in overlay mode");
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
        //Restore default settings
            IEXGateway._IEXResult res;
             //Fetch the default background settings frm project ini
            string defaultBackgroundSetting = CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "DEFAULT_BACKGROUND_SETTING");
            if (String.IsNullOrEmpty(defaultBackgroundSetting))
            {
                 CL.IEX.FailStep("DEFAULT_BACKGROUND_SETTING value is not present in the Project.ini");
            }
            //Navigate to background settings
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE BACKGROUND SETTINGS");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to Navigate to background settings ");

            }
            //Fetch the background settings
            string obtainedBackgroundSetting;

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedBackgroundSetting);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to get background setting");
            }
            if (obtainedBackgroundSetting.Equals(defaultBackgroundSetting))
            {
                CL.IEX.LogComment("TV background settings is set to default settings");
            }
            
            else
            {
                res = CL.IEX.MilestonesEPG.Navigate(defaultBackgroundSetting);
                if (!res.CommandSucceeded)
                {
                    CL.IEX.FailStep("Failed to restore to default background settings ");

                }
            }
        
    }
    #endregion
}