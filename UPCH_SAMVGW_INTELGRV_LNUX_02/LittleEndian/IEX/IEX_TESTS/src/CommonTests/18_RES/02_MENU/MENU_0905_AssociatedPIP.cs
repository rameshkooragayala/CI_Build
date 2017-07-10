/// <summary>
///  Script Name : MENU_0905_AssociatedPIP.cs
///  Test Name   : EPG-0905-Main Menu-PIP On Running Mode
///  TEST ID     : 64454
///  JIRA ID     : FC-385
///  QC Version  : 1
///  Variations from QC: NONE
/// -----------------------------------------------
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using System.Collections;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("MENU_0905")]
public class MENU_0905 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service videoServiceS1;
    private static Service videoServiceS2;
    private static int rewindMin;
    private static int play;
    private static string menuFirstFocusItem;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Zap to s1 and launch main menu";
    private const string STEP2_DESCRIPTION = "Step 2: Zap to another channel and rewind to previous channel content";
    private const string STEP3_DESCRIPTION = "Step 3: Launch Main Menu, PIP of S2 is displayed";

    private static class Constants
    {
        public const int rbCollectionWaitTime = 30; //in seconds
        public const int waitForRewind = 15;//in seconds
        public const int waitForMilestones = 15;//in seconds
    }

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

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
            videoServiceS1 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            videoServiceS2 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + videoServiceS1.LCN);

            if (videoServiceS1 == null || videoServiceS2 == null)
            {
                FailStep(CL, "One if the Service is null. videoServiceS1: " + videoServiceS1 + " videoServiceS2: " + videoServiceS2);
            }
            else
            {
                LogCommentInfo(CL, "First video service videoServiceS1: " + videoServiceS1.LCN);
                LogCommentInfo(CL, "First video service videoServiceS2: " + videoServiceS2.LCN);
            }

            //Get the Default Item attribute from the Project ini file
            menuFirstFocusItem = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MAIN_MENU_FIRST_FOCUSSED_ITEM");
            if (menuFirstFocusItem.Equals(""))
            {
                menuFirstFocusItem = CL.EA.GetValueFromINI(EnumINIFile.Project, "MENUS", "MAIN_MENU_FIRST_FOCUSSED_ITEM");
                if (menuFirstFocusItem.Equals(""))
                {
                    FailStep(CL, "Failed to fetch Main Menu items from the Project attributes file!");
                }

            }

            LogCommentImportant(CL, "Expected First Focussed Item on Main Menu :" + menuFirstFocusItem);

            //Get value from Project.ini
            string rewindMinStringVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");
            string playStringVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "PLAY");

            if (string.IsNullOrEmpty(rewindMinStringVal) || string.IsNullOrEmpty(playStringVal))
            {
                FailStep(CL, "One of the values is empty or null. REW_MIN: " + rewindMinStringVal + "PLAY: " + playStringVal);
            }
            else
            {
                LogCommentInfo(CL, "REW_MIN: " + rewindMinStringVal);
                LogCommentInfo(CL, "PLAY: " + playStringVal);
            }

            rewindMin = Int16.Parse(rewindMinStringVal);
            play = Int16.Parse(playStringVal);

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

            //Zap to videoServiceS1 and wait till review buffer is sufficiently collected
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoServiceS1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + videoServiceS1);
            }

            //Launch menu and verify if the focus is on correct meny item
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Menu");
            }

            string title = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to display channel logo");
            }

            //Verify if the channel is same as currently tuned channel
            if (!title.Equals(menuFirstFocusItem))
            {
                LogCommentFailure(CL, "Obtained value, focus is present on: " + title);
                FailStep(CL, "First focused item is not on " + menuFirstFocusItem);
            }

            //Wait for 30 seconds for review buffer to get collected
            LogCommentInfo(CL, "Wait for review buffer collection");
            res = CL.IEX.Wait(Constants.rbCollectionWaitTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for review buffer collection time: " + Constants.rbCollectionWaitTime);
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
            //Zap to another channel and rewind to previous channel content
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoServiceS2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + videoServiceS2.LCN);
            }

            //Rewind review buffer such that previous service content is played.
            res = CL.EA.PVR.SetTrickModeSpeed("RB", rewindMin, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set review buffer speed to rewind: " + rewindMin);
            }
            LogCommentInfo(CL, "Waiting for RB to be rewinded to playback prvious viewd service");

            res = CL.IEX.Wait(Constants.waitForRewind);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait for: " + Constants.waitForRewind + " seconds");
            }

            //Rewind review buffer such that previous service content is played.
            res = CL.EA.PVR.SetTrickModeSpeed("RB", play, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set review buffer to play" + play);
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Get Mile stone values which verify PIP
            String Milestones = CL.EA.UI.Utils.GetValueFromMilestones("ChannelSurf");

            //Begin wait for PIP mile stones arrival
            CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, Constants.waitForMilestones);

            //Launch menu over prvious service content and verify PIP of current event
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Menu");
            }

            string project = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "Project");
            LogCommentInfo(CL, "project value fetched from environment file is " + project);
            if (project != "VOO")
            {
                CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("CHANNELS");
            }


            //End wait for Milestones arrival
            ArrayList arrayList = new ArrayList();
            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arrayList))
            {
                FailStep(CL, "Failed to verify PIP");
            }

            //Verify if previously tuned channel number is obtained
            string channelNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out channelNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel number information");
            }

            //Verify if the channel is same as currently tuned channel
            if (channelNumber != videoServiceS2.LCN)
            {
                FailStep(CL, "Not tuned to " + videoServiceS2 + " channel");
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}