/// <summary>
///  Script Name : CHBAR_0574_InformationOnFutureProgram.cs
///  Test Name   : EPG-0574-Channel Bar-Information on Future Program
///  TEST ID     : 64524
///  JIRA ID     : FC-469
///  QC Version  : 2
///  Variations from QC: None
/// -----------------------------------------------
///  Modified by : Avinob Aich
///  Modified Date: 28/7/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("CHBAR_0574")]
public class CHBAR_0574 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service logoChannel;
    private static Service noLogoChannel;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Focus on Future Program on Channel Bar in Channel having logo and check if channel logo and number are present";
    private const string STEP2_DESCRIPTION = "Step 2: Focus on Future Program on Channel Bar in Channel having no logo and check if channel name and number are present";

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

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
            logoChannel = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True;IsEITAvailable=True", "ParentalRating=High");
            noLogoChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "HasChannelLogo=True;ParentalRating=High");

            if (logoChannel == null || noLogoChannel == null)
            {
                FailStep(CL, "No Channel found in Content.xml for the parameters passed");
            }
            LogCommentInfo(CL, "Channel fetched from Content.xml: " + logoChannel.LCN + "," + noLogoChannel.LCN);
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

            CHBAR_0574 chBar = new CHBAR_0574();

            //Tune to a channel having logo
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, logoChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Channel: " + logoChannel.LCN);
            }

            if (!chBar.VerifyChannelInfoOnChannelBar(logoChannel, true))
            {
                FailStep(CL, "Failed to Verify Channel Info");
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

            CHBAR_0574 chBar = new CHBAR_0574();

            //Tune to a channel having logo
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, noLogoChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Channel: " + noLogoChannel.LCN);
            }

            if (!chBar.VerifyChannelInfoOnChannelBar(noLogoChannel))
            {
                FailStep(CL, "Failed to Verify Channel Info");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute

    #region CommonFunction

    /// <summary>
    /// Verifies if Channel Logo/Name and Channel Number is present on Future Event in Channel Bar
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="verifyChannelLogo"></param>
    /// <returns></returns>
    private bool VerifyChannelInfoOnChannelBar(Service channel, bool verifyChannelLogo = false)
    {
        IEXGateway._IEXResult res;
        string obtainedChannelLogo = "";
        string obtainedChannelName = "";
        string obtainedChannelNumber = "";
        bool isPass = true;
        //Get Value from Project.ini
        string delayStateTransition = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DELAY_STATE_TRANSITION");
        //Clear EPG Info
        res = CL.IEX.MilestonesEPG.ClearEPGInfo();
        if (!res.CommandSucceeded)
        {
            LogCommentWarning(CL, "Failed to Clear EPG Info");
        }

        //Launch Channel Bar
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to launch Channel Bar");
        }

        //Focus on future program on Channel Bar
        res = CL.IEX.MilestonesEPG.SelectMenuItem("NEXT");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to focus on future event on Channel: " + logoChannel.LCN);
        }

        //waiting for state transition
        res = CL.IEX.Wait(Convert.ToDouble(delayStateTransition));
        if (!res.CommandSucceeded)
        {
            LogCommentWarning(CL, "Failed to wait for state transition");
        }

        //Get Channel Logo from Channel Bar
        obtainedChannelLogo = CL.EA.GetChannelLogo();

        if (verifyChannelLogo)
        {
            LogCommentInfo(CL, "Obtained Channel logo from Channel Bar: " + obtainedChannelLogo);
            LogCommentInfo(CL, "Expected Channel logo from Channel Bar: " + channel.ChannelLogo);

            //Checking if Channel Logo is as Expected
            if (obtainedChannelLogo != channel.ChannelLogo)
            {
                if (string.IsNullOrEmpty(obtainedChannelLogo))
                {
                    LogCommentFailure(CL, "Channel Logo is empty in for channel: " + channel.LCN);
                }
                else
                {
                    LogCommentFailure(CL, "Channel Logo is different than expected logo: " + channel.ChannelLogo);
                }
                isPass = false;
            }
        }
        else
        {
            //checking if channel logo is present for channel having no logo
            if (!string.IsNullOrEmpty(obtainedChannelLogo))
            {
                LogCommentFailure(CL, "Channel Logo is present for channel having no logo");
                isPass = false;
            }

            //Get Channel Name from Channel Bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out obtainedChannelName);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to get channel name from Channel Bar on Channel: " + channel.LCN);
                isPass = false;
            }

            LogCommentInfo(CL, "Obtained Channel Name from Channel Bar: " + obtainedChannelName);
            LogCommentInfo(CL, "Expected Channel Name from Channel Bar: " + channel.Name);

            //Checking if channel Name is as Expected
            if (obtainedChannelName != channel.Name)
            {
                if (string.IsNullOrEmpty(obtainedChannelName))
                {
                    LogCommentFailure(CL, "Channel Name is empty in for channel: " + channel.LCN);
                }
                else
                {
                    LogCommentFailure(CL, "Channel Name is different than expected logo: " + channel.Name);
                }
                isPass = false;
            }
        }

        //Get Channel Number From Channel Bar
        res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out obtainedChannelNumber);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to get Channel Number from Channel Bar on Channel: " + channel.LCN);
        }

        LogCommentInfo(CL, "Obtained Channel Number from Channel Bar: " + obtainedChannelNumber);
        LogCommentInfo(CL, "Expected Channel Numberfrom Channel Bar: " + channel.LCN);

        //Checking if channel number as expected
        if (obtainedChannelNumber != channel.LCN)
        {
            if (string.IsNullOrEmpty(obtainedChannelNumber))
            {
                LogCommentFailure(CL, "Channel Number is empty in channel Bar on channel: " + channel.LCN);
            }
            else
            {
                LogCommentFailure(CL, "Channel Number is different than expected channel Number" + channel.LCN);
            }
            isPass = false;
        }

        return isPass;
    }

    #endregion CommonFunction
}