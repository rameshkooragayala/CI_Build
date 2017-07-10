using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//LightSanity-012-LIB-BOOK-Recording delete
public class LightSanity_012 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string Short_SD_1;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: LightSanity-012-LIB-BOOK-Recording delete
        //Testing sanity for Deleting a Record.
        //Preconditions: Have Default Settings. Have a Record on Disk
        //Based on QualityCenter test version 3.
        //Variations from QualityCenter: None.
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File, Sync & Have a Recording on Disk");
        this.AddStep(new Step1(), "Step 1: Access the Recording List and Pick Event");
        this.AddStep(new Step2(), "Step 2: Delete the Recorded Event");
        this.AddStep(new Step3(), "Step 3: Return To Live Viewing and Check Recording List Again");

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
            Short_SD_1 = CL.EA.GetValue("Short_SD_1");


            //Have a Recording on Disk
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Short_SD_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel");
            }
            //Check enough time left for the event
            int timeToEventEnd_sec = 0;
            res = CL.EA.GetCurrentEventLeftTime(ref timeToEventEnd_sec);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Time Left to Current Event");
            }
            if (timeToEventEnd_sec < 120)
            {
                CL.EA.ReturnToLiveViewing();
                CL.IEX.Wait(timeToEventEnd_sec);
            }
            res = CL.EA.PVR.BookFutureEventFromBanner("Event1", 1, 2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Banner");
            }

            res = CL.EA.WaitUntilEventEnds("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Wait Until Event Ends");
            }

            //Wait for GT to end + a few seconds to make sure PCAT is updated
            double Default_Guard_Time = 120;
            double PCAT_Wait = 160;
            double Test_Wait = Default_Guard_Time + PCAT_Wait;
            CL.IEX.Wait(Test_Wait);

            res = CL.EA.PCAT.VerifyEventPartialStatus("Event1", "ALL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify the Event is Fully Recorded");
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

            //Access the Recording List and verify the Record is Displayed
            res = CL.EA.PVR.VerifyEventInArchive("Event1", true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Found Event in the Recording List");
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

            //Delete the Record
            res = CL.EA.PVR.DeleteRecordFromArchive("Event1", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Delete Past Record from Recording List");
            }

            //Verify the Record is Removed from Recording List
            res = CL.EA.PVR.VerifyEventInArchive("Event1", false, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Return to Live Viewing
            res = CL.EA.ReturnToLiveViewing(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Return to Live Viewing after Deleteing Recording in Recording List");
                return;
            }

            //Check Recording List Again, and Verify the Deleted Record is Still Not There.
            res = CL.EA.PVR.VerifyEventInArchive("Event1", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Deleted Event WAS FOUND in the Recording List in the Second Check");
                return;
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
