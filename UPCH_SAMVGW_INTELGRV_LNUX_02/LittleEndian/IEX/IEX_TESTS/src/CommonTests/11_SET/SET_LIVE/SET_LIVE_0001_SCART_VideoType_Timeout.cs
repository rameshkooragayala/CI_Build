/// <summary>
///  Script Name : SET-LIVE-0001-SCART video signal type timeout
///  Test Name   : SET_LIVE_0001_SCART_VideoSignalType_Timeout.cs
///  TEST ID     : 18906
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Madhu Kumar K
///  Modified Date: 25th Sept, 2014
/// </summary>

using System;
using IEX.Tests.Engine;


public class SET_LIVE_0001 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    private static string defaultTVColorOutput;
    private static string selectedTVColorOutput;
    private static string obtainedTVColorOutput;
    
    #region Create Structure

    public override void CreateStructure()
    {
        //Adding steps
        this.AddStep(new Step1(), "Step 1: Navigate to TV Color Output and select any option other then Default and confirm and verify that it is changed");
        this.AddStep(new Step2(), "Step 2: Navigate to TV Color Output and select any option and dont confirm it and wait after 20 seconds its returning and highlighting on default");
        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                LogCommentInfo(CL, "Failed to navigate to state TV COLOR OUTPUT");
                res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:VIDEO SETTINGS MAINVIDEOCABLE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to main video cable");
                }
                res = CL.IEX.MilestonesEPG.Navigate("HDMI");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to select HDMI");
                }
                res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res,"Failed to navigate to state TV COLOR OUTPUT");
                }
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultTVColorOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to get the EPG info");
            }
            string timeStamp="";
            res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to send IR Select Down");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selectedTVColorOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info");
            }
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Select");
            }
            CL.IEX.Wait(2);

            bool settingsConfirmationState = CL.EA.UI.Utils.VerifyState("SETTINGS CONFIRMATION STATE", 5);
            if (settingsConfirmationState)
            {
                LogCommentInfo(CL, "settings confirmation state verified sucessfully after few seconds");
            }
            else
            {
                FailStep(CL, res, "Unable to verify the settings confirmation state after few seconds");
            }
            res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Select Down");
            }
            CL.IEX.Wait(2);
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Select");
            }
            CL.IEX.Wait(5);

            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV COLOR OUTPUT");
            }

            CL.IEX.Wait(2);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedTVColorOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info");
            }
            if (obtainedTVColorOutput != selectedTVColorOutput)
            {
                FailStep(CL, "Obtaned vale is " + obtainedTVColorOutput + " is different from selected value " + selectedTVColorOutput);
            }
            else
            {
                LogComment(CL, "Obtaned vale is " + obtainedTVColorOutput + " is same as selected value " + selectedTVColorOutput);
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
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:TV COLOR OUTPUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV COLOR OUTPUT");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selectedTVColorOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info");
            }

            string timeStamp = "";
            res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Select Down");
            }
            CL.IEX.Wait(2);

            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Select");
            }
            CL.IEX.Wait(5);

            bool settingsConfirmationState = CL.EA.UI.Utils.VerifyState("SETTINGS CONFIRMATION STATE", 5);
            if (settingsConfirmationState)
            {
                LogCommentInfo(CL, "settings confirmation state verified sucessfully after few seconds");
            }
            else
            {
                FailStep(CL, res, "Unable to verify the settings confirmation state after few seconds");
            }

            CL.IEX.Wait(20);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedTVColorOutput);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info");
            }

            if (obtainedTVColorOutput != selectedTVColorOutput)
            {
                FailStep(CL, "Obtaned vale is " + obtainedTVColorOutput + " is different from selected value " + selectedTVColorOutput);
            }
            else
            {
                LogComment(CL, "Obtaned vale is " + obtainedTVColorOutput + " is same as selected value " + selectedTVColorOutput);
            }

            PassStep();
        }
    }

    #endregion Step1
    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        string timeStamp = "";
        CL.IEX.Wait(5);
        res = CL.IEX.SendIRCommand("SELECT_UP", -1, ref timeStamp);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to send IR Select Down");
        }
        CL.IEX.Wait(2);
        res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selectedTVColorOutput);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to get the EPG info");
        }
        res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to send IR Select");
        }
        CL.IEX.Wait(5);

        bool settingsConfirmationState = CL.EA.UI.Utils.VerifyState("SETTINGS CONFIRMATION STATE", 5);
        if (settingsConfirmationState)
        {
            LogCommentInfo(CL, "settings confirmation state verified sucessfully after few seconds");
        }
        else
        {
            LogCommentFailure(CL, "Unable to verify the settings confirmation state after few seconds");
        }
        res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to send IR Select Down");
        }
        CL.IEX.Wait(2);
        res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to send IR Select");
        }
        CL.IEX.Wait(5);
    }

    #endregion PostExecute
}