/// <summary>
///  Script Name : STC_0022_LastViewed.cs
///  Test Name   : STC-0022-LastViewed.cs
///  TEST ID     : 71556
///  QC Version  : 2
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : Madhu Renukaradhya
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("STC_0022")]
public class STC_0022 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    

    //Shared members between steps
    static Service service1;
    static Service service2;
    static Boolean isSettingsSupported = false;
    static string  lastViewedService = "";
    static string serviceBeforeStandBy = "";
    static string defaultSetting = "";
    static string imageLoadDelay = "";
    static int delaytime = 0;
    static string standbyAfterBoot = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File ";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to AV Service1, enter Standy and Exit and Verify the Channel tuned is same as Last Channel->Service 1 ";
    private const string STEP2_DESCRIPTION = "Step 2: Tune to AV Service 2 ,Reboot and Allow the box to complete Power cycle and Verify the Channel tuned is tuned to Last viewed Channel->Service 1";

    static class Constants
    {
        public const bool exitStandBy = true;
        public const bool enterStandBy = false; 


    }

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

            //Get Values From xml File
            service1 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (service1.Equals(null))
            {
                FailStep(CL, "Failed to fetch service1 from content xml.");

            }
            else
            {
                LogCommentInfo(CL, "Service1 fetched from content xml is: " + service1.LCN);

            }
            service2 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (service2.Equals(null))
            {
                FailStep(CL, "Failed to fetch service2 from content xml.");

            }
            else
            {
                LogCommentInfo(CL, "Service2 fetched from content xml is: " + service2.LCN);

            }
            isSettingsSupported = Convert.ToBoolean(CL.EA.GetValueFromINI(EnumINIFile.Project, "SETTINGS", "START_CHANNEL"));
            if (isSettingsSupported)
            {
                //Get the default settings
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:START CHANNEL");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to start channel");
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultSetting);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get default setting.");
                }

                //Set START CHANNEL as LAST Viewed channel under Setting
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:START CHANNEL - LAST VIEWED CHANNEL");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to set start channel as last viewed channel");
                }
            }
            else
            {
                LogCommentInfo(CL,"Settings not supported");
            }

            standbyAfterBoot = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "STANDBY_AFTER_REBOOT");

            imageLoadDelay = CL.EA.GetValueFromINI(EnumINIFile.Project, "BOOTUP", "IMAGE_LOAD_DELAY_SEC");
            if (imageLoadDelay == null)
            {
                FailStep(CL, res, "Failed to load image load delay time from Project INI file");
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

            //Tune to service1
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service1 " + service1.LCN);
            }
            //Fetch the service before stand by
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out serviceBeforeStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel name of service before Stand By");
            }
            //Enter Stand by
            res = CL.EA.StandBy(Constants.enterStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby  ");
            }

            //Exit Stand by
            res = CL.EA.StandBy(Constants.exitStandBy);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby ");
            }

            // Fetch the last viewed service

             res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out lastViewedService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel name of last viewed service");
            }

            if (serviceBeforeStandBy.Equals(lastViewedService))
            {
                LogCommentInfo(CL,"Tuned Successfully to last viewed service after Stand By");
            }
            else
            {
                FailStep(CL,res, "Unable to tune to last viewed service after Stand By");
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

            //Tune to service2
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to service2 " + service2.LCN);
            }

            //reboot the STB

            //converting the image load time to int for Wait 
            int.TryParse(imageLoadDelay, out delaytime);
            
            LogCommentInfo(CL, "mounting the image to STB"); 
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT);            
            if (!res.CommandSucceeded) 
            { 
                FailStep(CL, res, "Failed to power cycle STB to live"); 
            } 
            
            //Wait for some time for STB to come to standby mode 
            res = CL.IEX.Wait(delaytime); 
            if (!res.CommandSucceeded) 
            { 
                FailStep(CL, res, "Failed to wait for image to load "); 
            } 

            
            //Checking if STB enters standby after reboot. For few projects STB goes to LIVE. This flag STANDBY_AFTER_REBOOT should be in Project.ini

            if (Convert.ToBoolean(standbyAfterBoot))
            {
                //Navigate out of standby 
                res = CL.EA.StandBy(Constants.exitStandBy);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to exit out of standby");
                }
            }
                // Fetch the service after reboot

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to launch channel bar");
                }

                string serviceAfterReboot = "";
                res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out serviceAfterReboot);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Get Channel name of service After Reboot");
                }

                if (lastViewedService.Equals(serviceAfterReboot))
                {
                    LogCommentInfo(CL, "Successfully tuned to last viewed service after reboot");
                }
                else
                {
                    FailStep(CL, res, "Failed to tune to last viewed service after reboot");
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
        //Reset to deafult settings
        IEXGateway._IEXResult res;

        if (isSettingsSupported)
        {
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:START CHANNEL");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to navigate to start channel");
            }

            res = CL.IEX.MilestonesEPG.Navigate(defaultSetting);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to set to default setting");
            }
        }
    }
    #endregion
}