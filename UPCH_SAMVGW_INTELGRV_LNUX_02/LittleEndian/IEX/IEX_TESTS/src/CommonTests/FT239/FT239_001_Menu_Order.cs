/// <summary>
///  Script Name : FT239_MainMenu_Order_change.cs
///  Test Name   : FT239_MainMenu_Order_change
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Ganpat Singh
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using System.Globalization;

[Test("FT239_MainMenu_Order_change")]
public class FT239_001 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    private static Service pcService;
    private static Service recordingService;
    static Helper _helper = new Helper();


    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File  ";
    private const string STEP1_DESCRIPTION = "Step 1: Check the order of main menu items after first installation ";
    private const string STEP2_DESCRIPTION = "Step 2: Check the order of main menu items on playback from RB";
    private const string STEP3_DESCRIPTION = "Step 3: Check the order of main menu items on playback of recorded event";
    private const string STEP4_DESCRIPTION = "Step 4: Check the order of main menu items on PC service";


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

            pcService = CL.EA.GetServiceFromContentXML("ParentalRating=High;Type=Video", "IsDefault=True");
            if (pcService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            recordingService = CL.EA.GetServiceFromContentXML("IsRecordable=True;IsEITAvailable=True;Type=Video", "IsDefault=True;ParentalRating=High");
            if (recordingService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

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

            if (_helper.verifyOrder())
            {
                LogCommentImportant(CL, "Main Menu entry state order is verified after first installation");
            }
            else
            {
                FailStep(CL, "Failed to verify main Menu entry state order after first installation");
            }
			
			res = CL.EA.ChannelSurf(EnumSurfIn.Live, recordingService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to do channel surf");
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("event1", VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to record the event");
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


            res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to pause the video");
            }

            CL.IEX.Wait(120);

            res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to play the video");
            }

            if (_helper.verifyOrder())
            {
                LogCommentImportant(CL, "Main Menu entry state order is verified on plackback of RB");
            }
            else
            {
                FailStep(CL, "Failed to verify main Menu entry state order on plackback of RB");
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

            res = CL.EA.PVR.PlaybackRecFromArchive("event1", 0, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to playback the recording");
            }

            if (_helper.verifyOrder())
            {
                LogCommentImportant(CL, "Main Menu entry state order is verified on plackback of recording");
            }
            else
            {
                FailStep(CL, "Failed to verify main Menu entry state order on plackback of recording");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, pcService.LCN);
            if (res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to tune to PC service :"+ pcService.LCN );
            }

            if (_helper.verifyOrder())
            {
                LogCommentImportant(CL, "Main Menu entry state order is verified on PC service");
            }
            else
            {
                FailStep(CL, "Failed to verify main Menu entry state order on PC service");
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
       
    }
    #endregion

    public class Helper : _Step
    {

        public override void Execute() { }

        public bool verifyOrder()
        {

            String title = "";
            String menuOrder = CL.EA.UI.Utils.GetValueFromProject("MENUS", "MENU_ORDER");

            String[] mainMenuOrder = menuOrder.Split(',');

            CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:MAIN MENU");

            for (int i = 0; i < mainMenuOrder.Length; i++)
            {
                CL.EA.UI.Utils.GetEpgInfo("title", ref title);

                if (mainMenuOrder[i].Contains(title))
                {
                    LogCommentInfo(CL, "Expected item and focused item is same as " + mainMenuOrder[i]);
                    CL.EA.UI.Utils.SendIR("SELECT_RIGHT");
                }
                else
                {
                    LogCommentFailure(CL, "Expected item is " + mainMenuOrder[i] + ", while focused item is " + title);
                    return false;
                }
            }
           
            return true;
        }


    }
}