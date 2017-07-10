using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class FullSanity_0504 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the test
    static string SD_Channel;
    static string HD_Channel;
    static string FTA_Channel;

    private static int endGuardTimeInt = 0;

    private static int startGuardTimeInt = 0;
    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Values from ini File, Sync Stream");
        this.AddStep(new Step1(), "Step 1: Book HD Event From Guide And Cancel Booking From Guide");
        this.AddStep(new Step2(), "Step 2: Book SD Event From Guide And Cancel Booking From Banner ");
        this.AddStep(new Step3(), "Step 3: Book Event From banner  And Cancel Booking From Planner ");

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
            SD_Channel = CL.EA.GetValue("Short_SD_1");
            HD_Channel = CL.EA.GetValue("Short_HD_1");
            FTA_Channel = CL.EA.GetValue("FTA_1st_Mux_5");


            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
            }
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
            res = CL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Banner Display Timeout to 10 Sec");
            }
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Book HD Event From Guide And Cancel Booking From Guide
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, HD_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Change Channel To HD_Channel " + HD_Channel + " Using DCA");
            }
            res = CL.EA.PVR.BookFutureEventFromGuide("HDEventFromGuide", HD_Channel, 1, 5, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Guide On HD Channel " + HD_Channel);
            }

            res = CL.EA.PVR.CancelBookingFromGuide("HDEventFromGuide", false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Cancel Booking HD Event From Guide");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("HDEventFromGuide", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Canceled Event WAS Found in the Planner");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Book SD Event From Guide And Cancel Booking From banner 
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, SD_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Change Channel To SD Channel " + SD_Channel + " Using DCA");
            }

            res = CL.EA.PVR.BookFutureEventFromGuide("SDEventFromGuide", SD_Channel, 1, 5, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From Guide On SD Channel " + SD_Channel);
            }

            res = CL.EA.PVR.CancelBookingFromBanner("SDEventFromGuide", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Cancel Booking SD Event From Banner");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("SDEventFromGuide", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Canceled Event WAS Found in the Planner");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3
    private class Step3 : _Step
    {
        //Book Event From Banner And Cancel Booking From Planner 
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_Channel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Change Channel To FTA_Channel " + FTA_Channel + " Using DCA");
            }

            res = CL.EA.PVR.BookFutureEventFromBanner("EventFrombanner", 1, 5, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Future Event From banner On FTA Channel " + FTA_Channel);
            }

            res = CL.EA.PVR.CancelBookingFromPlanner("EventFrombanner", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Cancel Booking From Planner");
            }

            res = CL.EA.PVR.VerifyEventInPlanner("EventFrombanner", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Canceled Event WAS Found in the Planner");
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
