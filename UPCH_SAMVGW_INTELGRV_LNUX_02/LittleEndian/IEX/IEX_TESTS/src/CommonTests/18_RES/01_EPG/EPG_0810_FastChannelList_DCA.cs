/// <summary>
///  Script Name : EPG_0810_FastChannelList_DCA.cs
///  Test Name   : EPG-0810-DCA In Fast Channel List
///  TEST ID     : 63828
///  JIRA TASK   : FC-247
///  QC Version  : 1
///  Variations from QC: None
/// -----------------------------------------------
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("EPG_0810_DCA_FastChannelList")]
public class EPG_0810 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service videoService;
    private static Service zapToServiceFromChannelLineUP;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Surf to video channel";
    private const string STEP2_DESCRIPTION = "Step 2: During live vieving access the fast channel list";
    private const string STEP3_DESCRIPTION = "Step 3: Enter DCA of particular channel and zap to that channel";

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
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            LogCommentInfo(CL, "Retrieved Value From XML File: videoService = " + videoService.LCN);

            zapToServiceFromChannelLineUP = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + videoService.LCN);
            LogCommentInfo(CL, "Retrieved Value From XML File: zapToServiceFromChannelLineUP = " + zapToServiceFromChannelLineUP.LCN);

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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + videoService.LCN);
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

            //Channel Surf to videoService
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelLineup, "", true, -1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Launch Channel Lineup From Live");
            }

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

            //Send IR command to zap to channel through channel line up
            CL.EA.UI.Utils.SendChannelAsIRSequence(zapToServiceFromChannelLineUP.LCN);

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
            if (chNum != zapToServiceFromChannelLineUP.LCN)
            {
                FailStep(CL, res, "Failed to zap to " + zapToServiceFromChannelLineUP.LCN);
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