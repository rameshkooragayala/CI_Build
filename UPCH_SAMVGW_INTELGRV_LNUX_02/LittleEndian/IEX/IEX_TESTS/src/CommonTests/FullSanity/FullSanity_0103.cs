using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-0103-STB-Default Standby state
public class FullSanity_0103 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string FTA_1st_Mux_1;
    static string powerMode = "";
    static string defaultPowerMode = "";
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-016-STB-Default Standby state
        //Pre-conditions: There is no recording scheduled with next 15min.
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Put CPE in Standby");
        this.AddStep(new Step2(), "Step 2: Wait For 10 Minutes");
        this.AddStep(new Step3(), "Step 3: Wake Up CPE and Check A/V");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");
            CL.EA.ReturnToLiveViewing();
            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }

            //Get default mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            //set to any power mode
            res = CL.EA.STBSettings.SetPowerMode(powerMode);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to set the power mode option" + powerMode);
            }
            else
            {
                LogCommentInfo(CL, "Power mode set Successfully");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Stay in Standby for 10 Minutes.
            double timeInStandby = 5;
            CL.IEX.Wait(timeInStandby);

            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();


            //verify set power mode
            res = CL.EA.STBSettings.VerifyPowerMode(powerMode, false, "", "", "",isWakeUp:false);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to verify power mode option");
            }
            else
            {
                LogCommentInfo(CL, "Set Power mode verified Successfully");
            }

            CL.IEX.Wait(120);
            bool status=true;

            LogCommentInfo(CL, "Tring to telnet the Box for different power modes");

            Int32 imageLoadDelay = Int32.Parse(CL.EA.UI.Utils.GetValueFromProject("BOOTUP", "IMAGE_LOAD_DELAY_SEC"));
            Boolean standbyAfterBoot = Boolean.Parse(CL.EA.UI.Utils.GetValueFromProject("BOOTUP", "STANDBY_AFTER_REBOOT"));
            Boolean isHomeNetwork = Boolean.Parse(CL.EA.UI.Utils.GetValueFromProject("BOOTUP", "IsHomeNetwork"));
          

            LogCommentInfo(CL,"Trying to Bringing up the box");


            if (powerMode.ToUpper() == "HOT STANDBY")
            {
                CL.EA.UI.Utils.Standby(true);
                //Verify state LIVE after exiting standby
                bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", imageLoadDelay);
                if (liveState)
                {
                    LogCommentInfo(CL, "Live state verified sucessfully after mounting the box");
                }
                else
                {
                    FailStep(CL, res, "Unable to verify the live state after mount");
                }
            }
            else
            {
                CL.IEX.Wait(30);


                try
                {
                    CL.EA.UI.Utils.Standby(true);

                }
                catch
                {
                    CL.IEX.Wait(60);
                    if (powerMode.ToUpper() == "ECO MODE")
                    {
                        try
                        {
                            if (isHomeNetwork)
                            {
                                status = CL.EA.TelnetLogIn(false, false);
                            }
                            else
                            {
                                status = CL.EA.TelnetLogIn(false);
                            }
                            if (status != false)
                            {
                                FailStep(CL, "Able to login to telnet which is not expected behaviour");
                            }
                            else
                            {
                                LogCommentImportant(CL, "Not able to login to Telnet which is expected behaviuor in the ECO mode");
                            }
                        }
                        catch
                        {
                            LogCommentInfo(CL,"Not able to login to Telnet which is expected behaviuor in the ECO mode");
                        }

                       
                    }
                    else
                    {
                        try
                        {
                            if (isHomeNetwork)
                            {
                                status = CL.EA.TelnetLogIn(false, false);
                            }
                            else
                            {
                                status = CL.EA.TelnetLogIn(false);
                            }

                            if (status == false)
                            {
                                FailStep(CL, "Not able to telnet the STB which is not expected behaviour");
                            }
                            else
                            {
                                LogCommentImportant(CL, "We are able to Login to Telnet which is expected behaviuor in the " + powerMode);
                            }
                        }
                        catch
                        {
                            FailStep(CL,"Not able to telnet the STB which is not expected behaviour");
                        }
                       
                    }
                    //Mount the box
                    CL.IEX.Wait(350);

                    //if the box supports home network mount client
                    if (isHomeNetwork)
                    {
                        CL.EA.MountClient(EnumMountAs.NOFORMAT);

                    }
                    //if the box does not support home network mount only gateway
                    else
                    {
                        CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);

                        //Wait for some time for STB to come to standby mode 
                        CL.IEX.Wait(imageLoadDelay);
                    }
                    //Verify state LIVE

                    bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", imageLoadDelay);
                    if (liveState)
                    {
                        LogCommentInfo(CL, "Live state verified sucessfully after mounting the box");
                    }
                    else
                    {
                        FailStep(CL, res, "Unable to verify the live state after mount");
                    }
                }
            }

            PassStep();

        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        //Restore default settings
        IEXGateway._IEXResult res;
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:STANDBY POWER USAGE");
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to navigate to settings POWER MANAGEMENT");
        }
        res = CL.IEX.MilestonesEPG.Navigate(defaultPowerMode);
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to set to default POWER MODE");
        }
        else
        {
            LogCommentFailure(CL, "Restored to default POWER MODE SUCCESSFULLY");
        }

    }
    #endregion
}