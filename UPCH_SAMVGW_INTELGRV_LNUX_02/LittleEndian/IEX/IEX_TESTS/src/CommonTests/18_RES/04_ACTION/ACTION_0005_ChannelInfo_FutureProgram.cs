/// <summary>
///  Script Name : ACTION_0005_ChannelInfo_FutureProgram
///  Test Name   : ACTION-0005-Channel Info-Future Program
///  TEST ID     : 63678
///  QC Version  : 4
///  Jira ID     : FC-421
///  Variations from QC:None
/// -----------------------------------------------
///  Modified by : Scripted by : Madhu Renukaradhya
///  Last modified : 16 JULY 2013
/// </summary>
using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("ACTION_0005_ChannelInfo_FutureProgram")]
public class ACTION_0005 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service serviceWithChannelLogo;
    private static Service serviceWithoutChannelLogo;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1:Tune to future event on service S1 with channel logo and verify that channel logo is displayed ";
    private const string STEP2_DESCRIPTION = "Step 2: Tune to future event on service S2 without channel logo and verify that channel name is displayed in absence of channel logo";

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

            //Get Values From ini File
            serviceWithChannelLogo = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "ParentalRating=High");
            serviceWithoutChannelLogo = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=False", "ParentalRating=High");

            if (serviceWithChannelLogo.Equals(null) || serviceWithoutChannelLogo.Equals(null))
            {
                FailStep(CL, "One if the Service is null. serviceWithChannelLogo: " + serviceWithChannelLogo + " serviceWithoutChannelLogo: " + serviceWithoutChannelLogo);
            }

            else
            {
                LogCommentInfo(CL, "ServiceWithChannelLogo: " + serviceWithChannelLogo.LCN);
                LogCommentInfo(CL, "ServiceWithoutChannelLogo: " + serviceWithoutChannelLogo.LCN);
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithChannelLogo.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to serviceWithChannelLogo " + serviceWithChannelLogo);
            }
            String expectedLogo = serviceWithChannelLogo.ChannelLogo;
            LogComment(CL, "Expected Channel logo is" + expectedLogo);

            //Navigate to Action Bar

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on future event ");
            }

            String obtainedChannelLogo = CL.EA.GetChannelLogo();
            LogComment(CL, "Obtained ChannelLogo is =  " + obtainedChannelLogo);

            if (!String.IsNullOrEmpty(obtainedChannelLogo))
            {
                if (obtainedChannelLogo.Equals(expectedLogo))
                {
                    LogComment(CL, "Expected logo recieved and verified = " + obtainedChannelLogo);
                }

                else
                {
                    FailStep(CL, res, "Expected logo not verified " + obtainedChannelLogo);
                }
            }
            else
            {
                FailStep(CL, "Channel logo is Empty");
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
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceWithoutChannelLogo.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to serviceWithoutChannelLogo " + serviceWithoutChannelLogo);
            }

            String expectedChannelName = serviceWithoutChannelLogo.Name;
            LogComment(CL, "Expected Channel name is" + expectedChannelName);

            //Navigate to Action Bar

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR NEXT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu on future event ");
            }

            String obtainedChannelLogoURL = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("channel_logo", out obtainedChannelLogoURL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel logo");
            }
            String obtainedChannelName = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out obtainedChannelName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel name");
            }
            LogComment(CL, "obtainedChannelName is " + obtainedChannelName);

            //Validating for channel name in the absence of channel logo.

            if (!String.IsNullOrEmpty(obtainedChannelLogoURL))
            {
                FailStep(CL, res, "Channel logo is present and is diplayed instead of channel name " + obtainedChannelLogoURL);
            }
            else
            {
                LogComment(CL, "obtainedChannelName is " + obtainedChannelName);
                LogComment(CL, "Expected Channel name is " + expectedChannelName);

                if (obtainedChannelName.Equals(expectedChannelName))
                {
                    LogComment(CL, "Expected Channel name recieved " + expectedChannelName);
                }

                else
                {
                    FailStep(CL, res, "Expected Channel name not recieved " + expectedChannelName);
                }
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
}