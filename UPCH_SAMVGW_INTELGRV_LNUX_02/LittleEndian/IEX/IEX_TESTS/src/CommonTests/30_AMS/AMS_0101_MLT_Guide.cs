/// <summary>
///  Script Name        : AMS_0101_MLT_Guide.cs
///  Test Name          : AMS-0101-MLT-Guide
///  TEST ID            : 
///  QC Version         : 
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Avinash Budihal
///  Modified Date      : 04th MAR, 2015
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using System.Xml;
using System.Xml.Linq;


public class AMS_0101_MLT_Guide : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    static string ATLs;
    private static Service Service_1;

    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch Channel Numbers from xml file & Sync");
        this.AddStep(new Step1(), "Step 1: Select main menu, launch tv guide and press ok and Launch action menu on focus event");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion Create Structure

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to launch TV GUIDE");
            }

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, "Failed to set the Personalization to YES");
            }
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                LogCommentWarning(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }


            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to tune to service " + Service_1.LCN);
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

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch TV GUIDE");
                LogCommentFailure(CL, "Failed to launch TV GUIDE");
            }

            CL.IEX.Wait(3);
            CL.EA.UI.Utils.SendIR("SELECT");
            CL.IEX.Wait(3);

            res = CL.IEX.MilestonesEPG.Navigate("MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Select MORE LIKE THIS" + res.FailureReason);
            }

            CL.IEX.Wait(3);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ADJUST TIMELINE 90 MINUTES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify ADJUST TIMELINE for 90 MINUTES");
            }

            string[] adjustTimeLineValues = CL.EA.GetValueFromINI(EnumINIFile.Project, "ADJUST TIMELINE", "LIST_ADJ_TL").Split(',');
            string eventName;

            foreach (string adjustTimeLine in adjustTimeLineValues)
            {
                eventName = "";

                LogCommentInfo(CL, "Select " + adjustTimeLine + " Minutes and Verify Grid");
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ADJUST TIMELINE " + adjustTimeLine);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Verify ADJUST TIMELINE " + adjustTimeLine);
                }

                res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get Event Name from Grid");
                }

                CL.EA.UI.Utils.SendIR("SELECT");
            }

            CL.IEX.Wait(600);
            
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.GUIDE, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify UI_SCREEN_ENTRY ALL_CHANNELS_GUIDE FROM MAIN_MENU");
            }

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.GUIDEATL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify AMS Tag in TV Guide :Adjudst Time Line");
            }
            else
            {
                LogCommentInfo(CL, "Verified AMS Tag in TV Guide :By Adjust Time Line");
            }
            
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Event Name from Grid");
            }

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.MORE_LIKE_THIS, commonVariable: eventName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify AMS Tag for MORE LIKE THIS");
            }
            else
            {
                LogCommentInfo(CL, "Verified AMS Tag for MORE LIKE THIS");
            }

            PassStep();
        }
    }

    #endregion Step1

    

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

