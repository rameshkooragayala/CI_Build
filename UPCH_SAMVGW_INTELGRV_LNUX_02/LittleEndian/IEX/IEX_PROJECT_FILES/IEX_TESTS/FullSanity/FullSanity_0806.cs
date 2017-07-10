using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("FullSanity_0806")]
//FullSanity-0806-EPG_Fast_Channel_List
public class FullSanity_0806 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service videoService;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Surf to video service";
    private const string STEP2_DESCRIPTION = "Step 2: Access channel list from live";
    private const string STEP3_DESCRIPTION = "Step 3: Access channel list from channel bar";
    private const string STEP4_DESCRIPTION = "Step 4: Tune to the service from Fast Channel List";

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

            //Get Values From xml File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            LogCommentInfo(CL, "Retrieved Value From XML File: videoService = " + videoService.LCN);

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

            //Channel Surf to videoService
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "2");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
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

            //Launch ChannelLineUP
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelLineup, "", false, -1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Channel Lineup From Live");
            }
			
	        //Select to zap to channel
            CL.EA.UI.Utils.SendIR("SELECT");		

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Navigate to channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            //Launch Channel line up
            CL.EA.UI.ChannelLineup.Navigate();

            //Verify CHANNEL LINEUP state
            if (!CL.EA.UI.Utils.VerifyState("CHANNEL LINEUP", 10))
            {
                FailStep(CL, res, "Failed to verify Channel Lineup state");
            }

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Send IR command to zap to channel through channel line up
            CL.EA.UI.Utils.SendIR("SELECT_DOWN");

            //wait for the IR sequence to be sent
            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait until IR sequence is entered");
            }

            //Select to zap to channel
            CL.EA.UI.Utils.SendIR("SELECT");

            //Verify if the zapped channel is same as sent through IR sequence
            string chNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out chNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get channel number From Channel Bar");
            }
            if (chNum == "2")
            {
                FailStep(CL, res, "Failed to zap to " + chNum);
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