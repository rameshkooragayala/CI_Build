/// <summary>
///  Script Name : FullSanity_0408.cs
///  Test Name   : FullSanity-0408-SET-Subtitle display
///  TEST ID     : 17315
///  QC Version  : 4
///  Variations from QC:
/// ----------------------------------------------- 
/// Created By : Aswin Kollaikkal
///  Modified by :
/// </summary>

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0408-change subtitles settings
public class FullSanity_0408 : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;

    //Channels used by the test

    static string timestamp;
    //Shared members between steps
    static string selected_option;
    static string defaultPin;
    static string subtitleService1;
    static string subtitleService2;
    static bool ishomenet = false;
    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File. Perform FACTORY RESET to reset to default settings");
        this.AddStep(new Step1(), "Step 1: Verify preferred sub title is OFF. Zap to channel S1 & Validate subtitles is OFF");
        this.AddStep(new Step2(), "Step 2:SET Preferred subtitle language ON");
        this.AddStep(new Step3(), "Step 3:Verify  subtitles is ON  in Service S1");
        this.AddStep(new Step4(), "Step 4:Zap to s2 & verify subtitle is ON  in Service S2");
       

        //Get Client Platform
        CL = GetClient();
        string isHomeNetwork = "FALSE";
         try
        {
             isHomeNetwork = CL.EA.GetValueFromINI(EnumINIFile.Test,"TEST PARAMS","IsHomeNetwork");
             ishomenet = Convert.ToBoolean(isHomeNetwork);
        }
        catch
        {
            LogCommentWarning(CL, "Fail to fetch ISHOMENETWORK value from Test INI Hence Consider as FALSE");
        }

        if (ishomenet)
        {
            //Get gateway platform
            GW = GetGateway();

        }
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Fetch Default PIN
            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }

            // get s1 from XML

            subtitleService1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LCN1");
            if (subtitleService1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            // get s2 from XML

            subtitleService2 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LCN2");
            if (subtitleService2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            
            //Do Factory Reset to reset to default
            res = CL.EA.STBSettings.FactoryReset(SaveRecordings: false, KeepCurrentSettings: false, PinCode: defaultPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform factory reset");
            }

            if (ishomenet)
            {
                res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount client");
                }
            }
            else
            {
                res = CL.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }
            }
             
            PassStep();

            
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //zap to channel s1 and check for default settings
        public override void Execute()
        {
            StartStep();

            //Check whether preferred sub title is OFF

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to setting AV SETTING SUBTITLES ON ");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selected_option);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the selected value in A/V SETTING - SUBTITLES ");
            }

            if (selected_option != "OFF")
            {
                FailStep(CL, res, "The default subtitles value is not OFF, it is: " + selected_option);
            }

            // surf to channel S1

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, subtitleService1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            // verify that the default option is OFF
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTING SUBTITLES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to A/V SETTING - SUBTITLES in ACTION BAR");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selected_option);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the selected value in A/V SETTING - SUBTITLES ");
            }
            if (selected_option != "OFF")
            {
                FailStep(CL, res, "The default subtitles value is not OFF, it is: " + selected_option);
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Activate preferred sub title language to English
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE ON");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to setting AV SETTING SUBTITLES ON ");
            }

          

            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return To Live Viewing after change the subtitle settings");
            }
            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
       // validate subtitle setting after activate
        public override void Execute()
        {
            StartStep();

            // verify that the default option now in the channel is ON


            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTING SUBTITLES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to A/V SETTING - SUBTITLES in ACTION BAR");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selected_option);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the selected value in subtitles ");
            }
            if (selected_option == "OFF")
            {
                FailStep(CL, res, "The subtitles value is OFF in Service S1");
            }


            PassStep();
        }
    }
    #endregion

    #region Step4
    private class Step4 : _Step
    {
        
             //Zap to s2 and validate subtitle preferred setting is ON
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, subtitleService2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTING SUBTITLES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to A/V SETTING - SUBTITLES in ACTION BAR");
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out selected_option);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the selected value in subtitles ");
            }
            if (selected_option == "OFF")
            {
                FailStep(CL, res, "The subtitles value is OFF in Service s2 ");
            }

            PassStep();
        }
           
        }
    
   
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}  
#endregion