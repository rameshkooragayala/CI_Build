/// <summary>
///  Script Name        : AMS_0001_Live_Viewing.cs
///  Test Name          : AMS-0001-Live-Viewing,AMS-0201-Standby-Event,AMS-0240-Signal-Loss-Event,AMS-0024-Suggested-Main menu,AMS-0025-FEATURED-Main menu,AMS-0101-MLT-CHAN-BAR-Live,AMS-0110-MLT-ACTION MENU-LIVE
///  TEST ID            : 15574,15579,23463
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 27th OCT, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0001 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
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

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
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
            //Enter and Exit Stand By
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Enter Standby");
            }
            CL.IEX.Wait(15);
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Exit Standby");
            }
            CL.IEX.Wait(5);
            //Tune to a service and wait for few seconds
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_1.LCN);
            }
            CL.IEX.Wait(30);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_2.LCN);
            }
			CL.IEX.Wait(80);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "9");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_2.LCN);
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName",out evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to get the EPG info for event name");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to navigate to CHANNEL BAR MORE LIKE THIS");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to navigate to ACTION BAR MORE LIKE THIS");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUGGESTED FEATURED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to STATE:SUGGESTED FEATURED");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUGGESTED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to SUGGESTED");
            }
            CL.IEX.Wait(45);
            //Unplug RF signal
           // res = CL.IEX.RF.TurnOff(instanceName: "1");
           // if (!res.CommandSucceeded)
           // {
           //     FailStep(CL, res, "Failed to unplug RF signal!");
           // }
           // res = CL.IEX.Wait(45);
            //Connecting the RF Signal
           // if (rfSwitch.Equals("A"))
           // {
          //      res = CL.IEX.RF.ConnectToA(instanceName: "1");
           // }
           // else
           // {
           //     res = CL.IEX.RF.ConnectToB(instanceName: "1");
           // }
          //  if (!res.CommandSucceeded)
          //  {
           //     FailStep(CL, res, "Failed to Connect RF back");
           // }

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
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PowerOn);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Power On Event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.StandByIn);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the StandBy in Event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.StandByOut);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the StandbyOut Event");
            }

            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.LiveViewEvent, service: Service_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.SUGGESTED, service: Service_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.FEATURED, service: Service_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
						
			//According to the new FT change we will be getting the LTV on LIVE
            evtName = "LTV " + evtName;
			
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.CHANNEL_BAR, service: Service_2,commonVariable:evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
			
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.ACTION_MENU, service: Service_2, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.MORE_LIKE_THIS, service: Service_2, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
           // res = CL.EA.VerifyAMSTags(EnumAMSEvent.SignalFailureEvent, service: Service_2);
           // if (!res.CommandSucceeded)
           // {
          //      FailStep(CL, res, "Failed to find the signal failure event");
          //  }

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
