/// <summary>
///  Script Name        : AMS_0202_PowerUp.cs
///  Test Name          : AMS-0202-Power-up
///  TEST ID            : 15991
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

public class AMS_0202 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    static string imageLoadDelay;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch Channel Numbers from xml file & Sync");
        this.AddStep(new Step1(), "Step 1: Set the Power mode to medium and verify the Power on Tag");
        this.AddStep(new Step2(), "Step 2: Set the Power mode to cold and verify the AMS Power on Tag");
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
           // if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
           // {
           //     FailStep(CL, res, "Failed to set the Personalization to YES");
           // }
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                LogCommentWarning(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }

            string timeStamp = "";
            CL.IEX.SendIRCommand("MENU", -1, ref timeStamp);
			CL.IEX.Wait(10);
			
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL,"Failed to tune to service "+Service_1.LCN);
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
           //As we dont have Medium in IPC project
            if (CL.EA.Project.Name.ToUpper() != "IPC")
            {
                res = CL.EA.STBSettings.SetPowerMode("MEDIUM");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to set the power mode to Medium");
                }
                res = CL.EA.STBSettings.VerifyPowerMode("MEDIUM", jobPresent: false);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the Power mode is MEDIUM");
                }
                CL.IEX.Wait(300);
                res = CL.EA.VerifyAMSTags(EnumAMSEvent.PowerOn);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the power on event");
                }
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
			CL.IEX.Wait(10);
			 //Stopping End wait for debug message for which Begin wait for debug message in the Verify power mode EA
            System.Collections.ArrayList arraylist = new System.Collections.ArrayList();
            try
            {
                CL.EA.UI.Utils.EndWaitForDebugMessages("IEX_PLAYER_EVENT_COMPONENT_VIDEO_UPDATE", ref arraylist);
            }
            catch
            {
                LogCommentImportant(CL, "Please ignore the Failure");
            }
			
			CL.IEX.Wait(10);
			
            res = CL.EA.STBSettings.SetPowerMode("ECO MODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set the power mode to ECO MODE");
            }
            res = CL.EA.STBSettings.VerifyPowerMode("ECO MODE", jobPresent: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Power mode is ECO MODE");
            }
            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.PowerOn);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the power on event");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.STBSettings.SetPowerMode("HOT STANDBY");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to set the power mode to HOT STANDBY");
        }
        if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
        {
            LogCommentFailure(CL, "Failed to set the Personalization to No");
        }
    }

    #endregion PostExecute
}
