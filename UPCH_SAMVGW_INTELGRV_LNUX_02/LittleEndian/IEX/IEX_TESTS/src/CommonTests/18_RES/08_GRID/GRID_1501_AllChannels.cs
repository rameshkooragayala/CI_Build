/// <summary>
///  Script Name : GRID_1501_AllChannels.cs
///  Test Name   : GRID-1501-Program Grid-All Channels
///  TEST ID     : 63998
///  QC Version  : 1
///  JIRA ID     : FC-298
/// -----------------------------------------------
///  Modified by : Appanna Kangira
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("GRID-1501-Program Grid-All Channels")]
public class GRID_1501 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service audioVideoService;
    private static string tunedChnum = "";
    private static string focusedChnum = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Sync";
    private const string STEP1_DESCRIPTION = "Step 1:Navigate to Program Grid and launch all channels ";

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

            //Get Values From Content XML File
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");

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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            //Obtaining CH_Num on Live State

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out tunedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get tuned Channel number");
            }
            //Navigate To grid
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }

            //Obtaining CH_Num on Grid State

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out focusedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }

            //Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID
            if (tunedChnum == focusedChnum)
            {
                LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + tunedChnum + " " + focusedChnum);
            }

            else
            {
                FailStep(CL, "Failed to Focus on the Tuned Channel in Grid");
            }

            //Doing a Audio Check while in Guide State
            LogCommentInfo(CL, "Doing a Audio Check in Guide Screen");
            res = CL.EA.CheckForAudio(true, 10);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Audio Check failed");
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