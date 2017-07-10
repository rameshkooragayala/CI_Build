/// <summary>
///  Script Name        : TIMER_0032_Loss_ReminderNotification_FactoryReset.cs
///  Test Name          : TIMER-0032 Loss of reminder notification  during factory reset
///  TEST ID            : 
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 16th Sept, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using FailuresHandler;

public class TIMER_0032 : _Test
{
    [ThreadStatic]
    static _Platform CL, GW;
    //Channels used by the test
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static string defaultPin;
    static bool isHomeNet = false;
    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml ");
        this.AddStep(new Step1(), "Step 1: Set Two reminders on different services");
        this.AddStep(new Step2(), "Step 2: Do Factory reset and complete the First Installation process");
        this.AddStep(new Step3(), "Step 3: Verify that the reminders are deleted after the factory reset");

        //Get Client Platform
        CL = GetClient();
        string isHomeNetwork = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "IsHomeNetwork");

        //If Home network is true perform GetGateway
        isHomeNet = Convert.ToBoolean(isHomeNetwork);
        if (isHomeNet)
        {
            //Get gateway platform
            GW = GetGateway();
        }
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

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_1.LCN);
            }

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_2.LCN);
            }

            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_3.LCN);
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_3.LCN);
            }
            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
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
            res = CL.EA.BookReminderFromGuide("ReminderEvent1", Service_1.LCN, NumberOfPresses: 2, VerifyBookingInPCAT: false, ReturnToLive: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book reminder from Guide on " + Service_1.LCN);
            }
            res = CL.EA.BookReminderFromGuide("ReminderEvent2", Service_2.LCN, NumberOfPresses: 3, VerifyBookingInPCAT: false, ReturnToLive: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book reminder from Guide on " + Service_1.LCN);
            }



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
            res = CL.EA.STBSettings.FactoryReset(SaveRecordings: false, KeepCurrentSettings: true, PinCode: defaultPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do the Factory reset");
            }

            if (isHomeNet)
            {
                res = GW.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }
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
                    FailStep(CL, "Failed to mount cleint");
                }
            }
            PassStep();
        }
    }

    #endregion Step2
    #region Step3

    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.EA.WaitUntilReminder("ReminderEvent1");
            if (res.CommandSucceeded)
            {
                FailStep(CL, "Reminder is not cancelled after the factory reset");
            }
            else
            {
                if (res.FailureCode != ExitCodes.ReminderFailure.GetHashCode())
                {
                    FailStep(CL, res, "Failed to wait until reminder");
                }
                else
                {
                    LogCommentImportant(CL, "Reminder has been cancelled ");
                }
            }

            res = CL.EA.WaitUntilReminder("ReminderEvent2");
            if (res.CommandSucceeded)
            {
                FailStep(CL, "Reminder2 is not cancelled after the factory reset");
            }
            else
            {
                if (res.FailureCode != ExitCodes.ReminderFailure.GetHashCode())
                {
                    FailStep(CL, res, "Failed to wait until reminder");
                }
                else
                {
                    LogCommentImportant(CL, "Reminder has been cancelled ");
                }
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
