/// <summary>
///  Script Name : Sanity_2016_DNL_Forced_FirstInst_N_on_N.cs
///  Test Name   : SANITY_2016_First_Installation
///  TEST ID     : 23337
///  QC Version  : 2
///  ///  Description : Verifies the SSU download after first installation for Forced Mode
///  ///                
///                     
/// ----------------------------------------------- 
///  Modified by : ASWIN KOLLAIKKAL
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.ElementaryActions.EPG;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using FailuresHandler;

[Test("Sanity_2016_First_Installation")]
public class Sanity_2016_First_Installation : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;
   
    //Test Duration
    private static string defaultPin;
    static string currentVersionId;
    static string modifiedVersionId;
    static string usageId;
    static string isLastDelivery;
    static string nitTable;
    static string defaultNitTable;
    static string rfFeed;
    static string message = "";
    //Shared members between steps
     static string mountCommand = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition:Check current Version & Image ID ";
    private const string STEP1_DESCRIPTION = "Step 1:Perform First Installation by doing factory reset ";
    private const string STEP2_DESCRIPTION = "Step 2: Perform OTA Download and Software Update ";
    private const string STEP3_DESCRIPTION = "Step 3:Verify Software Version & Image ID has been changed";
   

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        

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
                                 
            //Fetch Default PIN
            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }

           //Fetch NITTable Name
            nitTable = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NIT_TABLE");
            if (string.IsNullOrEmpty(nitTable))
            {
                FailStep(CL, "Failed to fetch NITTABLE from Test.ini");
            }

            //Fetch Default NITTable Name
            defaultNitTable = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEFAULT_NIT_TABLE");
            if (string.IsNullOrEmpty(nitTable))
            {
                FailStep(CL, "Failed to fetch DEFAULTNITTABLE from Test.ini");
            }

           
            //Fetch IS Last Delivery 
            isLastDelivery = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_LASTDELIVERY");
            if (string.IsNullOrEmpty(isLastDelivery))
            {
                FailStep(CL, "Failed to fetch ISLASTDELIVERY from Test.ini");
            }
            //Fetch IS RF Feed 
            rfFeed = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RF_FEED");
            if (string.IsNullOrEmpty(rfFeed))
            {
                FailStep(CL, "Failed to fetch RFEED from Test.ini");
            }

            //Fetch mount command to fetch from environment.ini
             mountCommand = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "MountCommand_FI");
             if (string.IsNullOrEmpty(mountCommand))
            {
                FailStep(CL, "Failed to fetch mountCommand from Environment.ini");
            }


         //   Check current version & Image ID
             res = CL.EA.GetAndVerifySoftVersion(ref currentVersionId, ref usageId);
             if (!res.CommandSucceeded)
             {
                 FailStep(CL, "Failed to verify software version");
             }

             CL.IEX.LogComment("Usgage ID is: " + usageId);

             string timeStamp = "";

            res = CL.IEX.SendIRCommand("MENU", 1,ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to send IR Main Menu");
            }
			CL.IEX.Wait(5);
            res = CL.IEX.SendIRCommand("RETOUR", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }


             if (!CL.EA.UI.Utils.VerifyState("LIVE", 20))
             {
                 FailStep(CL, "Failed to verify state:LIVE");
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
            
            //Perform Factory reset 

            res = CL.EA.STBSettings.FactoryReset(SaveRecordings: false, KeepCurrentSettings: false, PinCode: defaultPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform factory reset");
            }

            CL.IEX.Wait(240);
            
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

            //Perform OTA Download

            res = CL.EA.OtaDownload(currentVersionId, "0", nitTable, Convert.ToBoolean(isLastDelivery), rfFeed);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform OTA Download");
            }
            CL.IEX.SendPowerCycle("OFF");
            CL.IEX.Wait(5);
            CL.IEX.SendPowerCycle("ON");

            //mounting STB and check for OTA download
            try
            {
                CL.EA.UI.Mount.WaitForPrompt(false);
                CL.EA.UI.Mount.SendMountCommand(true, mountCommand: mountCommand);

            }
            catch (Exception ex)
            {
                LogCommentInfo(CL, "Failed to mount the box as download detected during mount:" + ex.Message);
            }

            //wait for download to complete
            CL.IEX.Wait(2100);
             
            //Mount after download is complete
            try
            {

                CL.IEX.SendPowerCycle("OFF");
                CL.IEX.Wait(5);
                CL.IEX.SendPowerCycle("ON");

                CL.EA.UI.Mount.WaitForPrompt(false);

                CL.EA.UI.Mount.SendMountCommand(true, mountCommand: mountCommand);
             
                CL.EA.UI.Mount.HandleFirstScreens(true);
                
                CL.EA.UI.Mount.InitializeStb(ref message);
            }
            catch (Exception ex)
            {
                LogCommentInfo(CL, "Failed to mount the box after download" + ex.Message);
            }

            //Check for video afer mount
            res = CL.EA.CheckForVideo(true, false, 15);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present");
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
            //Verify modified ImageID and Version Number
            res = CL.EA.GetAndVerifySoftVersion(ref  modifiedVersionId, ref usageId, true,currentVersionId);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify software version after OTA Download");
            }

         
            CL.IEX.LogComment("Modified Usage ID is: " + usageId);

            PassStep();
        }
    }
    #endregion
    
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

        CL.EA.UI.OTA.NITBraodcast(defaultNitTable);

     
    }
    #endregion

    

}

