/// <summary>
///  Script Name        : AMS_0116_MLT_ActionMenu_PLB_RecList.cs
///  Test Name          : AMS-0116-MLT-ACTION MENU-PLB, AMS - 0116-MLT-ACTION MENU REC-LSIT
///  TEST ID            : 
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 18th March, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0116 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
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

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN=9", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }


            //Tune to a service and wait for few seconds
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_1.LCN);
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("EVENT_BASED", MinTimeBeforeEvEnd: 5, VerifyIsRecordingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record cirrent event from Banner");
            }
            CL.IEX.Wait(300);
            res = CL.EA.PVR.StopRecordingFromArchive("EVENT_BASED");
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL,"Failed to stop the Recording from Archive");
            }
            CL.IEX.Wait(10);

            res = CL.EA.PVR.VerifyEventInArchive("EVENT_BASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to verify Event in Archive");
            }
            CL.IEX.Wait(5);
            string timestamp = "";
            CL.IEX.SendIRCommand("SELECT",-1, ref timestamp);
            CL.IEX.Wait(5);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info for event name");
            }
            res = CL.IEX.MilestonesEPG.Navigate("MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to MORE LIKE THIS on Action Menu for Rec List");
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
 			
			//According to the new FT change we will be getting the LTV on LIVE
            evtName = "DVR " + evtName;
			
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.ACTION_MENU, service: Service_1, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Action Menu event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.MORE_LIKE_THIS, service: Service_1, commonVariable: evtName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the MLT event");
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
