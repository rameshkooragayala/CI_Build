/// <summary>
///  Script Name : LIVE_0501_DCA.cs
///  Test Name   : LIVE-0501-DCA
///  TEST ID     : 63759
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0501 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string One_digit_channel = "1";
    private static string singleDigitChannel;
    private static string Two_digit_channel = "12";
    private static string doubleDigitChannel;
    private static string FTA_1st_Mux_4;
    private static string Lowest_two_digit_Service_Number;
    private static string Lowest_three_digit_Service_Number;
    private static string Lowest_Service_Number;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: Live - 0501 - DCA
        //Performs DCA surfing with 1 digit and 2 digits (error channel number)
        //Based on QualityCenter - 5
        //Variations from QualityCenter: Don't have 1 or 2 digits channles in stream, checking that it goes to the nearest channel
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Tune To Channel S1");
        this.AddStep(new Step2(), "Step 2: Channel Surf With 1 Digit Channel Number");
        this.AddStep(new Step3(), "Step 3: Tune To Channel S1");
        this.AddStep(new Step4(), "Step 4: Channel Surf With 2 Digits Channel Number");

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

            //Get Values From ini File
            FTA_1st_Mux_4 = CL.EA.GetValue("FTA_1st_Mux_4");
            Lowest_Service_Number = CL.EA.GetValue("Lowest_Service_Number");
            singleDigitChannel = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "one_digit_channel");
            if (String.IsNullOrEmpty(singleDigitChannel))
            {
                singleDigitChannel = One_digit_channel;
            }

            doubleDigitChannel = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "two_digit_channel");
            if (String.IsNullOrEmpty(doubleDigitChannel))
            {
               doubleDigitChannel = Two_digit_channel;
            }

            Lowest_two_digit_Service_Number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Lowest_two_digit_Service_Number");
            if (String.IsNullOrEmpty(Lowest_two_digit_Service_Number))
            {
                Lowest_two_digit_Service_Number = Lowest_Service_Number;
            }

            Lowest_three_digit_Service_Number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Lowest_three_digit_Service_Number");
            if (String.IsNullOrEmpty(Lowest_three_digit_Service_Number))
            {
                Lowest_three_digit_Service_Number = Lowest_Service_Number;
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        //Step 1: Tune To Channel S1 And Set It As The Default Channel
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + FTA_1st_Mux_4 + " With DCA");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 2: Channel Surf With Low Digit Channel Number
        public override void Execute()
        {
            StartStep();

            // Tune With 1 Digit Channel Number
            CL.EA.ChannelSurf(EnumSurfIn.Live, singleDigitChannel);

            //Get Channel Number
            string ChNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out ChNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Number");
            }

            // Verify That The Current Channel Is The First One (Since there Is No 1 Digit Channel)
            if (ChNumber != Lowest_two_digit_Service_Number)
            {
                FailStep(CL, "Received Differnt Channel Number (" + ChNumber + ") Than Expected  (" + Lowest_two_digit_Service_Number + ")");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step13

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + FTA_1st_Mux_4 + " With DCA");
            }

            PassStep();
        }
    }

    #endregion Step13

    #region Step4

    private class Step4 : _Step
    {
        //Step 4 : Channel Surf With High Channel Number
        public override void Execute()
        {
            StartStep();

            // Tune With 2 Digit Channel Number
            CL.EA.ChannelSurf(EnumSurfIn.Live, doubleDigitChannel);

            //Get Channel Number
            string ChNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out ChNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Number");
            }

            if (ChNumber != Lowest_three_digit_Service_Number)
            {
                FailStep(CL, "Received Differnt Channel Number (" + ChNumber + ") Than Expected  (" + Lowest_three_digit_Service_Number + ")");
            }

            PassStep();
        }
    }

    #endregion Step4

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}