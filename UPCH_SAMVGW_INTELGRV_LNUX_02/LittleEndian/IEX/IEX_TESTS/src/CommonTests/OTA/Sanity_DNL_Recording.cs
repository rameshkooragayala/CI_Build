/// <summary>
///  Script Name : Sanity_DNL_Record.cs
///  Test Name   : Sanity_DNL_RecordOn+n on n
///  TEST ID     : 
///  QC Version  : 2
///  Variations from QC:none
///  QC Repository :
/// ----------------------------------------------- 
///  Modified by : Ganpat Singh
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("Sanity_DNL_Record")]
public class Sanity_DNL_Record : _Test
{

    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    private static Service recordingService;
    static string oldVersion = "";
    static string usageID = "";
    static string otaDownloadOption = "";
    static string rfFeed = "";
    static string isLastDelivery = "";
    static string nitTable = "";
    static string defaultNitTable = "";

    #region Create Structure


    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & Start manual recording");
        this.AddStep(new Step1(), "Step 1: Broadcasting and downloading the Version");
        this.AddStep(new Step2(), "Step 2: Verifying the Software Version after OTA download");
        this.AddStep(new Step3(), "Step 3: Verifying recording is still going on");
        this.AddStep(new Step4(), "Step 4: PlayBack the recording");


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

    private class PreCondition : _Step
    {

        public override void Execute()
        {
            StartStep();
			
            String ChannelNumber = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_NUMBER");
            if (ChannelNumber == "")
            {
                FailStep(CL, "Failed to get the Service number from Test ini");
            }
            else
            {
                LogCommentInfo(CL, "SERVICE_NUMBER fetched from Test ini " + ChannelNumber);
            }

            recordingService = CL.EA.GetServiceFromContentXML("IsRecordable=True;LCN="+ChannelNumber, "IsDefault=True;ParentalRating=High");
            if (recordingService == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }

            //Fetching the Last delivery from Test Ini using which we will decide the Decide the download on either Last delivery or current
            isLastDelivery = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IS_LASTDELIVERY");
            if (isLastDelivery == "")
            {
                FailStep(CL, "Failed to get the is last delivery from Project ini");
            }
            else
            {
                LogCommentInfo(CL, "Is LastDelivery fetched from Test ini " + isLastDelivery);
            }
            //Fetching the RF feed from Test ini
            rfFeed = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RF_FEED");
            if (rfFeed == "")
            {
                FailStep(CL, "Failed to get the RF feed from Test ini");
            }
            else
            {
                LogCommentInfo(CL, "RF Feed fetched from Test ini " + rfFeed);
            }
            //Fetching the OTA download option Forced, Manual or Automatic
            otaDownloadOption = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "OTA_DOWNLOAD_OPTION");
            if (otaDownloadOption == "")
            {
                FailStep(CL, "Failed to fetch the otaDownloadOption from Project ini");
            }
            else
            {
                LogCommentInfo(CL, "OTA Download option fetched from Test ini " + otaDownloadOption);
            }
            //NIT table from Test ini
            nitTable = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NIT_TABLE");
            if (nitTable == "")
            {
                FailStep(CL, "Failed to get the nit Table value from test ini file");
            }
            else
            {
                LogCommentInfo(CL, "Nit Table fetched from test ini " + nitTable);
            }
            defaultNitTable = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEFAULT_NIT_TABLE");
            if (defaultNitTable == "")
            {
                FailStep(CL, "Failed to get the Default nit Table value from test ini file");
            }
            else
            {
                LogCommentInfo(CL, "Default Nit Table fetched from test ini " + defaultNitTable);
            }

            //Navigate to Diagnostics and get the Software version and Usage ID
            res = CL.EA.GetAndVerifySoftVersion(ref oldVersion, ref usageID);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the Software version from EPG");
            }
            else
            {
                LogCommentInfo(CL, "Fetched values from Diagnostics : Diagnostics - " + oldVersion + " UsageID - " + usageID);
            }

			string timeStamp = "";
			res = CL.IEX.SendIRCommand("MENU", 1, ref timeStamp);
			if (!res.CommandSucceeded)
			{
				FailStep(CL, res, "Failed to send IR Main Menu");
			}
			CL.IEX.Wait(5);
            res = CL.IEX.SendIRCommand("RETOUR", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }
			CL.IEX.Wait(5);
            res = CL.EA.PVR.RecordManualFromCurrent("TB_Rcording", recordingService.LCN, 90, EnumFrequency.ONE_TIME, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to start Manual Recording");
            }

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
            //Download binary On Air
            res = CL.EA.OtaDownload(oldVersion, usageID, nitTable, Convert.ToBoolean(isLastDelivery), rfFeed);
            if (!(res.CommandSucceeded))
            {
                FailStep(CL, res, "Failed to download binary on air");
            }
            else
            {
                LogCommentInfo(CL, "Binary downloaded successfully on the box");
            }
            //Verify the Download option
            if (otaDownloadOption.ToUpper() == "FORCED")
            {
                res = CL.EA.OtaDownloadOption(EnumOTADownloadOption.FORCED);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the Download option Forced");
                }
            }
            else
            {
                res = CL.EA.OtaDownloadOption(EnumOTADownloadOption.AUTOMATIC);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the Download option Automatic");
                }
            }

            //Mount the Gateway
            res = CL.EA.MountGw(EnumMountAs.NOFORMAT_NOREBOOT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to mount the Gateway");
            }

            LogCommentInfo(CL, "Waiting fews seconds for the box to come to live");

            CL.IEX.Wait(120);

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

            //check for live state
            bool liveState = CL.EA.UI.Utils.VerifyState("LIVE", 20);
            if (!liveState)
            {
                FailStep(CL, res, "Unable to verify the live state after on air OTA download");

            }
            //Verifying the Software version with the old version and comparing
            res = CL.EA.GetAndVerifySoftVersion(ref oldVersion, ref usageID, IsVerify: true, OldSoftVersion: oldVersion);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the Software version from EPG");
            }
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

            res = CL.EA.PCAT.VerifyEventIsRecording("TB_Rcording");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify recording is resumed");
            }
			
			string timeStamp = "";
			res = CL.IEX.SendIRCommand("MENU", 1, ref timeStamp);
			if (!res.CommandSucceeded)
			{
				FailStep(CL, res, "Failed to send IR Main Menu");
			}

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

            res = CL.EA.PVR.PlaybackRecFromArchive("TB_Rcording", 30, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback the recording");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps
    #region PostExecute

    public override void PostExecute()
    {
	    IEXGateway._IEXResult res;
        LogComment(CL, "Broadcasting Manual NIT");
        //Fetch the default Manual OTA NIT
        CL.EA.UI.OTA.NITBraodcast(defaultNitTable);
        CL.IEX.Wait(20);
		
		res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
           LogCommentFailure(CL,"Failed to delete all records from Archive");
        }
    }

    #endregion PostExecute
}
