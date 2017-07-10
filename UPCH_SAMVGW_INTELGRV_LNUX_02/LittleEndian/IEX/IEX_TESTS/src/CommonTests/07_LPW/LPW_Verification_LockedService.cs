/// <summary>
///  Script Name : LPW_Verification_LockedService.cs
///  Test Name   : STB-0910-History of PC event from non hot standby,STB-0911-Boot on service with PC event from hot standby,STB-0913-Boot on service with pc locked or manual locked service
///  TEST ID     : 72750,72751,72752
///  QC Version  : 2
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : Madhu R
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("LPW_Locked")]
public class LPW_Locked : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps
    static Service PCLockedChannel;
    static Service ManualLockedChannel;
    static string powerMode = "";
    static string defaultPowerMode = "";
    static Boolean isManullyLocked = false;
	static Int64 channel_number = 0;
	
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to PC locked event and unlock and verify A/V being displayed. ";
    private const string STEP2_DESCRIPTION = "Step 2: Manually lock a service and zap to manually locked service,enter pin and verify that A/V is being displayed. ";
    private const string STEP3_DESCRIPTION = "Step 3: Switch to standBy and wake up from stand by";
    private const string STEP4_DESCRIPTION = "Step 4: Verify that PIN is prompted on both PC locked and manualy locked service";


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
                     
            //Get Channels from xml File
            PCLockedChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;ParentalRating=High", "");

            if (PCLockedChannel == null)
            {
                FailStep(CL, "Failed to fetch lockedChannel from Content.xml for the passed criterion");
            }
            else
            {
                LogCommentInfo(CL, "lockedChannel fetched from Content.xml: " + PCLockedChannel.LCN);
            }

            //Get channel number from test ini
			channel_number = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LCN"));
			
            ManualLockedChannel = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN="+channel_number, "ParentalRating=High");

            if (ManualLockedChannel == null)
            {
                FailStep(CL, "Failed to fetch ManualLockedChannel from Content.xml for the passed criterion");
            }
            else
            {
                LogCommentInfo(CL, "ManualLockedChannel fetched from Content.xml: " + ManualLockedChannel.LCN);
            }

           // Get Values From ini File
            powerMode = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "POWER_MODE");
            if (string.IsNullOrEmpty(powerMode))
            {
                FailStep(CL, res, "Unable to fetch the power mode value from test ini file");
            }

           //powerMode = "HOT STANDBY";

            //Get default mode from project ini
            defaultPowerMode = CL.EA.GetValueFromINI(EnumINIFile.Project, "POWER_MANAGEMENT", "DEFAULT_MODE");
            if (string.IsNullOrEmpty(defaultPowerMode))
            {
                FailStep(CL, res, "Unable to fetch the default value from project ini");
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
            //Tune to locked channel
            res = CL.EA.TuneToLockedChannel(PCLockedChannel.LCN, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a PC locked Channel");
            }

            //Verify for Live state after unlock
            bool display = CL.EA.UI.Utils.VerifyState("LIVE");
            if (!display)
            {
                FailStep(CL, "Unable to display Live");
            }
            else
            {
                LogComment(CL, "Live displayed succefully after unlock");
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

            //Manually lock the channel
            res = CL.EA.STBSettings.SetLockChannel(ManualLockedChannel.Name);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Lock Channel");
            }
            else
            {
                isManullyLocked = true;
                LogCommentInfo(CL, "Service" + ManualLockedChannel.LCN + "is manually locked successfully");
            }
            res = CL.EA.TuneToLockedChannel(ManualLockedChannel.LCN, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a locked Channel");
            }

            //Wait to ensure the last viewed service
            res = CL.IEX.Wait(10);
            LogCommentInfo(CL,"Wait to ensure the service to be last viewed");
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

            //verify set power mode
           res = CL.EA.STBSettings.VerifyPowerMode(powerMode,jobPresent:false);
           if (!(res.CommandSucceeded))
           {
               FailStep(CL, res, "Failed to Verify the power mode option" + powerMode);
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

            //Reverify for PIN entry State after standBy for manually locked service
            res = CL.EA.TuneToLockedChannel(ManualLockedChannel.LCN, true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Verify PIN State for MANUALLY LOCKED after stand by");
                
            }
            else
            {
                LogCommentInfo(CL, "PIN state verified for MANUALLY LOCKED after stand by");
            }

             //Reverify for PIN entry State after standBy for PC locked service
            res = CL.EA.TuneToLockedChannel(PCLockedChannel.LCN, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Verify PIN entry state for PC locked service after standy by");
            }
            else
            {
                LogCommentInfo(CL, "PIN state verified for PC LOCKED service after stand by");
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
        //Restore default settings
        IEXGateway._IEXResult res;

        //Clear EPG Info
        res = CL.IEX.MilestonesEPG.ClearEPGInfo();
        if (!(res.CommandSucceeded))
        {
            LogCommentFailure(CL, "Failed to clear EPG Info");
        }
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
		
        //check for live state
        bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 20);
        if (!liveState)
        {
            LogCommentFailure(CL, "Unable to verify the live state.Thus mounting the STB");
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Unable to mount gateway");
            }

            CL.IEX.Wait(120);
        }
        else
        {
            LogCommentInfo(CL, "Live state verified sucessfully.");
            //Unlock the manually locked channel
            if (isManullyLocked)
            {
                res = CL.EA.STBSettings.SetUnLockChannel(ManualLockedChannel.Name);
                if (!res.CommandSucceeded)
                {
                    CL.IEX.FailStep("Failed to unlock channel");
                }
            }

            //Reset to default power mode
            res = CL.EA.STBSettings.SetPowerMode(defaultPowerMode);
            if (!(res.CommandSucceeded))
            {
                LogCommentFailure(CL, "Failed to set to default POWER MODE");
            }
            else
            {
                LogCommentInfo(CL, "Restored to default POWER MODE SUCCESSFULLY");
            }

            //Unlock the PC rated service
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to move to Standby");
            }

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to bring the box out of Standby");
            }

        }
    }
    #endregion

}