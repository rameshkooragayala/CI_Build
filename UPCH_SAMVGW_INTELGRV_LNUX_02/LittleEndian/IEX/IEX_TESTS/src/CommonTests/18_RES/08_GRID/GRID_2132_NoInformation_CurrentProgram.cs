/// <summary>
///  Script Name : GRID_2132_NoInformation_CurrentProgram
///  Test Name   : GRID-2132-No Information-Current Program
///  TEST ID     : 64476
///  JIRA ID     : FC-460
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 7/25/2013
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class GRID_2132 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service anyService;
    private static Service noEITService;
    private static Service radioService;
    private static Boolean isNext = true;
    private static int noOfPresses;

    private static class Constants
    {
        public const int timeToPress = -1;
        public const Boolean doTune = true;
    }

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: In the Programme Grid, when the user selects a focused running programme whose programme data is missing in the schedule, it enable the user to watch the corresponding channel.
        //Pre-conditions: Get Channel Numbers From ini File & Sync
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Launch Programme Grid on service S1 and select a view by ALL CHANNELS");
        this.AddStep(new Step2(), "Step 2: Bring the focus on service S2 and check there is PIP for service S2");
        this.AddStep(new Step3(), "Step 3: Launch action menu on the same event and select on VIEW");
        this.AddStep(new Step4(), "Step 4: Come back to programme grid and move the focus on to current event on service S3 (Radio service) and press long OK key");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From xml File
            noEITService = CL.EA.GetServiceFromContentXML("IsEITAvailable=False;Type=Video", "ParentalRating=High");
            if (noEITService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }

            anyService = CL.EA.GetServiceFromContentXML("Type=Video", "LCN=" + noEITService.LCN);
            if (noEITService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria");
            }

            //Check that project support Radio service or not
            String isRadio = CL.EA.GetValueFromINI(EnumINIFile.Project, "RADIO_CHANNELS", "SUPPORTED");
            if (Convert.ToBoolean(isRadio))
            {
                radioService = CL.EA.GetServiceFromContentXML("Type=Radio", "IsDefault=True");
                if (radioService == null)
                {
                    FailStep(CL, "Failed to get a radio service");
                }

                int noEITServicePositionOnGuide = int.Parse(noEITService.PositionOnGuide);
                int radioServicePositionOnGuide = int.Parse(radioService.PositionOnGuide);
                noOfPresses = (radioServicePositionOnGuide - noEITServicePositionOnGuide);

                if (noOfPresses < 0)
                {
                    isNext = false;
                    noOfPresses = Math.Abs(noOfPresses);
                }
            }
            else
            {
                LogCommentWarning(CL, "Skipping the radio channel related actions as they are not supported in project");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Tune to any service other then noEITService
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, anyService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to Service S1");
            }

            // Navigate to TV GUIDE
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate TV GUIDE");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            CL.IEX.Wait(10);
            //Tune to Service S2
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, noEITService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf the Service " + noEITService.LCN);
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            string timeStamp = "";

            //Press SELECT key
            res = CL.IEX.SendIRCommand("SELECT", Constants.timeToPress, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send SELECT key");
            }

            //Wait for state transition
            String delayStateTrans = CL.EA.GetValueFromINI(EnumINIFile.Project, "DELAYS", "DELAY_STATE_TRANSITION");
            Double dlyState = Convert.ToDouble(delayStateTrans);
            res = CL.IEX.Wait(dlyState);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for state transition");
            }

            //Focus on menu item VIEW
            res = CL.IEX.MilestonesEPG.SelectMenuItem("VIEW");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus on VIEW");
            }

            //Select VIEW
            res = CL.IEX.SendIRCommand("SELECT", Constants.timeToPress, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send SELECT key");
            }

            //Wait for state transition
            CL.EA.UI.Utils.VerifyState("LIVE", 20);

            LogCommentInfo(CL, "Verified that state is LIVE");

            PassStep();
        }
    }

    #endregion Step3

    #region Step4

    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            String channelType;
            String isRadio = CL.EA.GetValueFromINI(EnumINIFile.Project, "RADIO_CHANNELS", "SUPPORTED");

            if (Convert.ToBoolean(isRadio))
            {
                // Navigate to TV GUIDE
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate TV GUIDE");
                }

                //Tune to service S3
                res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", isNext, noOfPresses, EnumPredicted.Default, Constants.doTune);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to surf the Service " + radioService.LCN);
                }

                //Verifying that successfully tune to Radio Channel
                res = CL.IEX.MilestonesEPG.GetEPGInfo("chtype", out channelType);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get channel type");
                }

                if (channelType.Equals("Radio"))
                {
                    LogCommentInfo(CL, "Successfully tuned to Radio Service");
                }
            }
            else
            {
                LogCommentWarning(CL, "Skipping the radio channel related actions as they are not supported in project");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}