using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_0901 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Short_SD_1;
    private static int endGuardTimeInt = 0;
    private static int startGuardTimeInt = 0;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File & SyncStream");
        this.AddStep(new Step1(), "Step 1: Book Event-Based Recording");
        this.AddStep(new Step2(), "Step 2: Wait for Recording to Start and End");
        this.AddStep(new Step3(), "Step 3: Playback the Recording");

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
            //Get Values from ini File
            Short_SD_1 = CL.EA.GetValue("Short_SD_1");
            CL.IEX.LogComment("Retrieved Value From ini File: Short_SD_1 = " + Short_SD_1);
            string sgtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SGT_VAL");

            string egtFriendlyName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EGT_VAL");

            LogComment(CL, "Retrieved value for Start Guard Time is" + sgtFriendlyName);

            LogComment(CL, "Retrieved value for End Guard Time is" + egtFriendlyName);

            endGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(egtFriendlyName);

            startGuardTimeInt = CL.EA.UI.Utils.GetGuardTimeFromFriendlyName(sgtFriendlyName);


            LogComment(CL, "Setting the Start Guard Time to " + sgtFriendlyName);
            res = CL.EA.STBSettings.SetGuardTime(true, sgtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the SGT to " + sgtFriendlyName);
            }


            LogComment(CL, "Setting the End Guard Time to " + egtFriendlyName);
            res = CL.EA.STBSettings.SetGuardTime(false, egtFriendlyName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the EGT to " + egtFriendlyName);
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

            //Tune to SD channel on Live
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }

            //Select a program Event on Live Content
            res = CL.EA.PVR.BookFutureEventFromBanner("Event1", 1, 3, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event from Banner");
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

            // Wait for the Recording to Finish
            res = CL.EA.WaitUntilEventEnds("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "ailed to Wait Until Event Ends");
            }

            //Wait for GT to end + a few seconds to make sure PCAT is updated
            LogComment(CL, "Waiting until the EGT is completed");
            res = CL.IEX.Wait(endGuardTimeInt * 60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait until event EGT ends");
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

            // Playback the Already Recorded Event from Archive
            res = CL.EA.PVR.PlaybackRecFromArchive("Event1", 0, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback The Event to EOF ");
            }
            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}
