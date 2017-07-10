/// <summary>
///  Script Name : CHBAR_0572_ChannelBarInformation_live.cs
///  Test Name   : CHBAR-0572-ChannelBar-Channel Line-up and Information in live Viewing
///  TEST ID     : 68087
///  JIRA ID     : FC-467
///  QC Version  : 2
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 13/08/2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("CHBAR_0572")]
public class CHBAR_0572 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service logoVideoChannel;
    static Service noLogoVideoChannel;
    static Service logoRadioChannel;
    static Service noLogoRadioChannel;

    static Helper helper = new Helper();

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Launch Channel Bar on Channel having no logo and check if channel name and number are present";
    private const string STEP2_DESCRIPTION = "Step 2: Press up/down arrow key from Channel bar and go to channel having logo and check if channel logo and number are present";
    private const string STEP3_DESCRIPTION = "Step 3: Launch Channel Bar on Radio Channel having logo and check if channel logo and number are present";
    private const string STEP4_DESCRIPTION = "Step 4: Launch Channel Bar on Radio Channel having no logo and check if channel name and number are present";

    static class Constants
    {
        public const bool verifyChannelLogo = true;
    }

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

    //Get Values from Project.ini


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

            //Get Values From xml File
            noLogoVideoChannel = CL.EA.GetServiceFromContentXML("Type=Video", "HasChannelLogo=True;ParentalRating=High");
            logoVideoChannel = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "ParentalRating=High");
            logoRadioChannel = CL.EA.GetServiceFromContentXML("Type=Radio;HasChannelLogo=True", "");
            noLogoRadioChannel = CL.EA.GetServiceFromContentXML("Type=Radio", "HasChannelLogo=True");

            if (noLogoVideoChannel == null || logoVideoChannel == null || logoRadioChannel == null || noLogoRadioChannel == null)
            {
                FailStep(CL, "No Channels found for the parameters passed in GetServiceFromContentXML");
            }

            LogCommentInfo(CL, "Channel fetched from Content.xml: " + logoVideoChannel.LCN + "," + noLogoVideoChannel.LCN + "," + logoRadioChannel.LCN + "," + noLogoRadioChannel.LCN);
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

            //Tune to a video channel having no logo
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, noLogoVideoChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel: " + noLogoVideoChannel.LCN);
            }

            //clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Launch channel bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch channel bar on channel: " + noLogoVideoChannel.LCN);
            }

            if (!helper.VerifyChannelInfoOnChannelBar(noLogoVideoChannel))
            {
                FailStep(CL, "Failed to verify information on channel bar");
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

            bool isNext = true; // surf on next channel on channel bar

            int noOfPresses = int.Parse(logoVideoChannel.PositionOnList) - int.Parse(noLogoVideoChannel.PositionOnList);

            if (noOfPresses < 0)
            {
                isNext = false; //surf on previous channel from channel bar
                noOfPresses = Math.Abs(noOfPresses);
            }

            //Clearing EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Surfing from Channel Bar to Channel having
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", isNext, noOfPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch channel bar and press down to channel:" + logoVideoChannel.LCN + " having logo");
            }

            if (!helper.VerifyChannelInfoOnChannelBar(logoVideoChannel, Constants.verifyChannelLogo))
            {
                FailStep(CL, "Failed to verify information on channel bar");
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

            //Tune to a radio Channel having Radio
            res = CL.EA.TuneToRadioChannel(logoRadioChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Radio Channel: " + logoRadioChannel.LCN);
            }

            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to Clear EPG Info");
            }

            //Launch Channel Bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Channel Bar on Radio Channel: " + logoRadioChannel.LCN);
            }

            if (!helper.VerifyChannelInfoOnChannelBar(logoRadioChannel, Constants.verifyChannelLogo))
            {
                FailStep(CL, "Failed to verify information on channel bar");
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

            //Tune to a Radio Channel having no logo
            res = CL.EA.TuneToRadioChannel(noLogoRadioChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Radio Channel: " + noLogoRadioChannel.LCN);
            }

            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to Clear EPG Info");
            }

            //Launch Channel Bar on Radio Channel
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Channel Bar on Radio Channel: " + noLogoRadioChannel.LCN);
            }

            if (!helper.VerifyChannelInfoOnChannelBar(noLogoRadioChannel))
            {
                FailStep(CL, "Failed to verify information on channel bar");
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

    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }
        /// <summary>
        /// verifies channel logo, name and number on channel bar
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="verifyChannelLogo"></param>
        /// <returns></returns>
        public bool VerifyChannelInfoOnChannelBar(Service channel, bool verifyChannelLogo = false)
        {
            bool isPass = true;
            string obtainedChannelLogo = "";
            string obtainedChannelName = "";
            string obtainedChannelNumber = "";

            if (verifyChannelLogo)
            {
                //Get channel logo from channel bar
                obtainedChannelLogo = CL.EA.GetChannelLogo();

                if (string.IsNullOrEmpty(obtainedChannelLogo))
                {
                    LogCommentFailure(CL, "Channel Logo is empty in channel: " + channel.LCN);
                    isPass = false;
                }
                //checking if channel logo is as expected
                else if (obtainedChannelLogo != channel.ChannelLogo)
                {

                    LogCommentInfo(CL, "Obtained Radio Channel logo from Channel Bar: " + obtainedChannelLogo);
                    LogCommentFailure(CL, "Channel Logo is different than expected logo: " + channel.ChannelLogo);
                    isPass = false;
                }
            }
            else
            {
                //Get Channel Name from Channel Bar
                res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out obtainedChannelName);
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to get Channel Name from Channel Bar");
                    isPass = false;
                }
                else if (string.IsNullOrEmpty(obtainedChannelName))
                {
                    LogCommentFailure(CL, "Channel Name is Empty in Channel Bar");
                    isPass = false;
                }
                //checking if Channel Name is as expected
                else if (obtainedChannelName != channel.Name)
                {


                    LogCommentInfo(CL, "Obtained Channel Name from Channel Bar: " + obtainedChannelName);
                    LogCommentFailure(CL, "Channel Name is different then its original Channel Name: " + channel.Name);

                    isPass = false;
                }

            }


            //Get Channel Number from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out obtainedChannelNumber);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to get Channel Number from Channel Bar");
                isPass = false;
            }
            else if (string.IsNullOrEmpty(obtainedChannelNumber))
            {
                LogCommentFailure(CL, "Channel Number is Empty in Channel Bar");
                isPass = false;
            }
            //Checking if Channel Number is as expected
            else if (obtainedChannelNumber != channel.LCN)
            {

                LogCommentInfo(CL, "Obtained Channel Number from Channel Bar: " + obtainedChannelNumber);
                LogCommentFailure(CL, "Channel Number is different then original Channel Number: " + channel.LCN);

                isPass = false;
            }

            return isPass;

        }
    }
    #endregion
}