/// <summary>
///  Script Name        : AMS_0240_SignalLoss_Event.cs
///  Test Name          : AAMS-0240-Signal-Loss-Event
///  TEST ID            : 23463
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 31st March, 2015
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0240 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_2;
    static string rfSwitch;//Whether it is A or B
    private static string evtName = "";
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file and Sync");
        this.AddStep(new Step1(), "Step 1: Verify the AMS tags for the events");

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

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }


            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_2.LCN);
            }

            rfSwitch = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "RF_SWITCH");
            if (rfSwitch == "")
            {
                FailStep(CL, "RF switch is not defined in the Test ini file");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_2.LCN);
            }


            CL.IEX.Wait(45);
            //Unplug RF signal
            res = CL.IEX.RF.TurnOff(instanceName: "1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unplug RF signal!");
            }
            res = CL.IEX.Wait(45);
            //Connecting the RF Signal
            if (rfSwitch.Equals("A"))
            {
                res = CL.IEX.RF.ConnectToA(instanceName: "1");
            }
            else
            {
                res = CL.IEX.RF.ConnectToB(instanceName: "1");
            }
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Connect RF back");
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

            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.SignalFailureEvent, service: Service_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the signal failure event");
            }

            PassStep();
        }
    }

    #endregion Step1




    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
        {
            LogCommentFailure(CL, "Failed to set the Personalization to No");
        }

    }

    #endregion PostExecute
}
