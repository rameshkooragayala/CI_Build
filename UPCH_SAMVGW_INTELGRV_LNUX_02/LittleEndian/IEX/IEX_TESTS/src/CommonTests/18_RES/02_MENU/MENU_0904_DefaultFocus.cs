/// <summary>
///  Script Name : MENU_0904_DefaultFocus.cs
///  Test Name   : EPG-0904-Main Menu-Default Focus
///  TEST ID     : 64527
///  JIRA ID     : FC-60
///  QC Version  : 1
///  Variations from QC:None
///
/// -----------------------------------------------
///  Modified by : Appanna Kangira
///  Modified Date: 30/07/2013
/// </summary>

using System;
using System.Collections;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("EPG-0904-Main Menu-Default Focus")]
public class MENU_0904 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service avService;
    private static string firstItemFocus = "";
    private static string title = "";

    private static class Constants
    {
        public const int pipMilestonesWaitTime = 15; //Wait time for PIP FAS Milestones
        public const double reviewBufferFillWait = 40; //Time to fill Review Buffer
        public const int rewindTimeWait = 5;           //wait time to switch from rewind mode to Play
        public const int playMode = 1;                 // 1 stands for trick Mode Play
        public const int secToPlay = 0;                //If Set to 0 then it will Playback till it's Stopped
        public const int minTimeBeforeEventEnd = 3;    //MIn End time required  to consider a an Event for Booking.
    }

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File & enter Standby to clear review buffer";
    private const string STEP1_DESCRIPTION = "Step 1: Zap to FTA Channel, Verify launching Menu & Schedule a recording";
    private const string STEP2_DESCRIPTION = "Step 2: Rewind back in Review buffer, Switch to Play mode";
    private const string STEP3_DESCRIPTION = "Step 3: On Review Buffer playback Launch Main Menu, Verify Focused Item";
    private const string STEP4_DESCRIPTION = "Step 4: Playback the recorded Event. Launch Main Menu, Verify Focused Item";

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

            string standByWait = CL.EA.GetValueFromINI(EnumINIFile.Project, "STANDBY", "SAFE_DELAY_SEC");

            //Get Values From XML File
            avService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (avService.Equals(null))
            {
                FailStep(CL, "Service retrieved is NULL");
            }

            LogCommentInfo(CL, "Retrieved Value From XML File:  AV_Service = " + avService);

            //Get the Default Item attribute from the Project ini file
            firstItemFocus = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "MAIN_MENU_FIRST_FOCUSSED_ITEM");
            if (firstItemFocus.Equals(""))
            {
                firstItemFocus = CL.EA.GetValueFromINI(EnumINIFile.Project, "MENUS", "MAIN_MENU_FIRST_FOCUSSED_ITEM");
                if (firstItemFocus.Equals(""))
                {
                    FailStep(CL, "Failed to fetch Main Menu items from the Project attributes file!");
                }
 
            }

            LogCommentImportant(CL,"Expected First Focussed Item on Main Menu :"+firstItemFocus);

            //Entering to Standby to clear the review buffer
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to put Box in Standby");
            }
            else
            {
                //min time to stay on StandBy
                CL.IEX.Wait(double.Parse(standByWait));
                res = CL.EA.StandBy(true);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to exit standby");
                }
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

            //Zap to first AvService and wait till review buffer is collected
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, avService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + avService.LCN);
            }

            //Launch Main Menu
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MAIN MENU");
            }

            // Verify the Default Item is Focussed on on Launch of Menu
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Main Menu title");
            }

            if (String.Equals(firstItemFocus, title, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                CL.EA.UI.Utils.LogCommentInfo("First_Item_Focus is correct");
            }
            else
            {
                FailStep(CL, "Unexpected Focused Item on Main Menu. received value: " + title + " expected value: " + firstItemFocus);
            }
            res = CL.IEX.Wait(seconds:10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait few seconds");
            }
            //Record on going event from Banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner("EveRecFromBanner", Constants.minTimeBeforeEventEnd, false, false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Record Current Event From Banner");
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

            //Retrieve REW Min Speed from Proj file.
            string rewindSpeedMin = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "REW_MIN");

            //Stay for a 40 secs to get RB filled up.
            CL.IEX.Wait(Constants.reviewBufferFillWait);

            //rewind
            res = CL.EA.PVR.SetTrickModeSpeed("RB", double.Parse(rewindSpeedMin), false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to activate rewind from Review Buffer at min Speed");
            }

            //Rewind shall continue for 30 secs
            CL.IEX.Wait(Constants.rewindTimeWait);

            //Play the event from Review Buffer
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.playMode, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the Trick mode to Play");
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
            //Launch Menu & Verify PIP while the Event is played back from Review Buffer

            //Get Milestone values which verify PIP
            String Milestones = CL.EA.UI.Utils.GetValueFromMilestones("ChannelSurf");

            //Begin wait for PIP milestones arrival
            CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, Constants.pipMilestonesWaitTime);

            //Launch Menu
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Main Menu");
            }

            //Verify the Default Item is Focussed on Launch of Menu
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Menu title");
            }

            if (String.Equals(firstItemFocus, title, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                CL.EA.UI.Utils.LogCommentInfo("Focused Item is correct");
            }
            else
            {
                FailStep(CL, "Unexpected Focused Item on Main Menu. received value: " + title + " expected value: " + firstItemFocus);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNELS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Main Menu");
            }

            //End wait for Milestones arrival
            ArrayList arraylist = new ArrayList();
            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arraylist))
            {
                FailStep(CL, "Failed to verify PIP while the Event is played back from Review Buffer");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Stop recording the event that was scheduled in step 1
            //res = CL.EA.PVR.StopRecordingFromBanner("EveRecFromBanner");
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to stop the ongoing recording");
            //}

            //Playback the recorded event from Archive
            res = CL.EA.PVR.PlaybackRecFromArchive("EveRecFromBanner", Constants.secToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the recorded event From Archive");
            }

            //Launch Menu & Verify PIP on the Recorded Event Playback from My Library.
            //Get Mile Stone values which verify PIP
            String Milestones = CL.EA.UI.Utils.GetValueFromMilestones("ChannelSurf");

            //Begin wait for PIP milestones arrival
            CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, Constants.pipMilestonesWaitTime);

            //Launch Menu
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Menu");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Main Menu title");
            }

            if (String.Equals(firstItemFocus, title, StringComparison.CurrentCultureIgnoreCase) == true)
            {
                CL.EA.UI.Utils.LogCommentInfo("First_Item_Focus is correct");
            }
            else
            {
                FailStep(CL, "firstItemFocus not matching with Title");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNELS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Main Menu");
            }

            //Enf wait for Milestones arrival
            ArrayList arraylist = new ArrayList();
            if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arraylist))
            {
                FailStep(CL, "Failed to Verify PIP on the Recorded Event Playback");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}