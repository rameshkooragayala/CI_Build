/// <summary>
///  Script Name : LIB_2111_Nav_AdultSessionPIN.cs
///  Test Name   : LIB-2111-PIN code when adult session is accessed
///  TEST ID     : 23586
///  QC Version  : 4
///  QC Domain   : FR_FUSION
///  QC Project  : UPC
///  QC repository : Software Test Plan (STP)/20 The Library/Video/Navigation (LIB-EPG)
/// -----------------------------------------------
///  Modified by : Frederic Luu
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIB_2111 : _Test
{
    [ThreadStatic]
    private static _Platform CL;
    private static Service adultChannel;    // Service to be recorded
    private static string eventName;
    private static string timeStamp = "";
    private static string lockedEventName;
    private static string defaultPIN;
    private static string selectKeyName;
    private static string backKeyName;

    private static class Constants
    {
        public const int minRecordDurationInMin = 1;
        public const string eventKey = "adult_event";   // Key to retrieve the event in the UI event collection
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Record one adult event");
        this.AddStep(new Step1(), "Step 1: (Adult session not yet open in Library) Select an adult content item in Library -> PIN requested");
        this.AddStep(new Step2(), "Step 2: (Adult session already open in Library) Select an adult content item in Library -> No PIN requested");

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

            //Get locked adult item name in 'My recordings'
            lockedEventName = CL.EA.UI.Utils.GetValueFromDictionary("DIC_STORE_ADULT_CONTENT");

            //Get PIN code
            defaultPIN = CL.EA.UI.Utils.GetValueFromEnvironment("DefaultPIN");

            //Get Channel Value From ini File
            adultChannel = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High", "");
            if (adultChannel == null)
            {
                FailStep(CL, "Service fetched from Content.xml is null");
            }

            //Get key names from Project.ini
            selectKeyName = CL.EA.UI.Utils.GetValueFromProject("KEY_MAPPING", "SELECT_KEY");
            backKeyName = CL.EA.UI.Utils.GetValueFromProject("KEY_MAPPING", "BACK_KEY");

            //Delete all recordings
            //CL.EA.UI.ArchiveRecordings.DeleteAllEvents();     // not implemented for UPC yet

            //Tune to the service to be recorded
            res = CL.EA.TuneToLockedChannel(adultChannel.LCN, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + adultChannel.LCN);
            }

            //Record current event from banner
            res = CL.EA.PVR.RecordCurrentEventFromBanner(Constants.eventKey, Constants.minRecordDurationInMin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to initiate recording of current event on service " + adultChannel.LCN);
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

            //Navigate to 'My recordings'
            res = CL.EA.PVR.NavigateToArchive();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Could not navigate to 'My recordings'");
            }

            //Select the recorded adult event  (IMPORTANT: Only one recording is supposed to be in 'My recordings')
            res = CL.IEX.SendIRCommand(selectKeyName, -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to press 'Select' on the adult item");
            }
            CL.IEX.Wait(1);

            //Check the PIN is asked
            if (!CL.EA.UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL"))
            {
                FailStep(CL, res, " PIN must be asked to open an adult session");
            }

            //Enter PIN
            CL.EA.UI.Utils.EnterPin(defaultPIN);

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

            //Exit 'My recordings' by pressing BACK key 
            res = CL.IEX.SendIRCommand(backKeyName, -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to press 'Retour' to go back to 'My Library' menu");
            }
            CL.IEX.Wait(2);

            //Re-enter 'My recordings' menu 
            res = res = CL.IEX.MilestonesEPG.Navigate("MY RECORDINGS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to re-enter 'My recordings' menu");
            }
            CL.IEX.Wait(2);

            //Select the recorded adult event  (IMPORTANT: Only one recording is supposed to be in 'My recordings')
            res = CL.IEX.SendIRCommand(selectKeyName, -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to press 'Select' on 'My recordings' item");
            }

            //Check the PIN is not asked (IMPORTANT: Only one recording is supposed to be in 'My recordings')
            if (CL.EA.UI.Utils.VerifyState("INSERT PIN UNLOCK CHANNEL"))
            {
                FailStep(CL, res, " PIN must not be asked when adult session is already open");
            }

            //Check selected item is no more locked           
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get selected Event Name from 'My recordings' list");
            }
            if (eventName == lockedEventName)
            {
                FailStep(CL, "Adult item name is still locked");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        //CL.EA.UI.ArchiveRecordings.DeleteAllEvents();   // not implemented for UPC yet
    }

    #endregion PostExecute
}

