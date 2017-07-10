/// <summary>
///  Script Name : LIVE_0502_DCA_NotExists.cs
///  Test Name   : LIVE-0502-DCA-Not Exists
///  TEST ID     : 63760
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Israel Itzhakov
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class LIVE_0502 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Error_Low_channel = "1";
    private static string Error_High_channel = "999";
    private static string singleDigitChannel;
    private static string FTA_1st_Mux_1;
    private static string Lowest_Service_Number;
    private static string Highest_Service_Number;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description: Live - 0502 - DCA - Not Exists
        //Performs DCA surfing with Error channel number
        //Based on QualityCenter - 3
        //Variations from QualityCenter: None
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File");
        this.AddStep(new Step1(), "Step 1: Tune To Channel S1");
        this.AddStep(new Step2(), "Step 2: Channel Surf With Low Digit Channel Number");
        this.AddStep(new Step3(), "Step 3: Channel Surf With High Channel Number");

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
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");
            
            Lowest_Service_Number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Lowest_Service_Number");
             if (string.IsNullOrEmpty(Lowest_Service_Number))
             {
                 Lowest_Service_Number = CL.EA.GetValue("Lowest_Service_Number");
             }

            Highest_Service_Number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Highest_Service_Number");
            if (string.IsNullOrEmpty(Highest_Service_Number))
            {
                Highest_Service_Number = CL.EA.GetValue("Highest_Service_Number");
            }

            singleDigitChannel = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "one_digit_channel");
            if (String.IsNullOrEmpty(singleDigitChannel))
            {
                singleDigitChannel = Error_Low_channel;
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Free Air_SD Service : " + FTA_1st_Mux_1 + " With DCA");
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
            if (ChNumber != Lowest_Service_Number)
            {
                FailStep(CL, "Received Differnt Channel Number (" + ChNumber + ") Than Expected  (" + Lowest_Service_Number + ")");
            }

            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {
        //Step 3: Channel Surf With High Channel Number
        public override void Execute()
        {
            StartStep();

            // Tune With 2 Digits Channel Number
            CL.EA.ChannelSurf(EnumSurfIn.Live, Error_High_channel);

            //Get Channel Number
            string ChNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out ChNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Number");
            }

            // Verify That The Current Channel Is The nearest Higher Channel
            if (ChNumber != Highest_Service_Number)
            {
                FailStep(CL, "Received Differnt Channel Number (" + ChNumber + ") Than Expected  (" + Highest_Service_Number + ")");
            }

            PassStep();
        }
    }

    #endregion Step3

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}