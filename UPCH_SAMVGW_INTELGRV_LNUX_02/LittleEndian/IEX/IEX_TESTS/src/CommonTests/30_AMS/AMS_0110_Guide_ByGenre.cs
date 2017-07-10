/// <summary>
///  Script Name : AMS_Guide_ByGenre.cs
///  Test Name   : AMS_Guide_ByGenre
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("AMS_Guide_ByGenre")]
public class AMS_0110_Guide_ByGenre : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;
    static string genres;

    //Shared members between steps
    static string FTA_Channel;
    static Service Service1;
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1:Go to TV Guide > By Channel, By Genre and navigate to all category  ";
    private const string STEP2_DESCRIPTION = "Step 2:Wait for 10 Minutes and Verify AMS Tags ";
  
    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
    
        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File

            //Get Service from content xml
            Service1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service1.LCN);
            }

            // get Genres list from Test INI

            genres = CL.EA.GetTestParams("GENRES");
            if(genres==null || genres=="")
            {
                FailStep(CL,"Fail ed to GENRES from Test INI");
            }



            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
             {
                FailStep(CL, "Failed to set the Personalization to YES");
            }

            //Tune to Servie 1

            res = CL.EA.TuneToChannel(Service1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Tune to Service:" + Service1.LCN);
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Launch Guide Single CHannel

            CL.EA.UI.Guide.NavigateToGuideSingleChannel();
            bool isInGuide = CL.EA.UI.Guide.IsGuideSingleChannel();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch single channel Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified single Channels guide launched");
            }

            CL.IEX.Wait(3);
            CL.EA.UI.Utils.SendIR("SELECT");
            CL.IEX.Wait(3);

            res = CL.IEX.MilestonesEPG.Navigate("MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Select MORE LIKE THIS" + res.FailureReason);
            }

            // Splitting genre list from Test INI

            string[] eachGenre = genres.Split(',');

            foreach (string category in eachGenre) // Navigate to each Genre Item mentioned in TEst INI
            {
                if (!CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:BY GENRE"))
                {
                    FailStep(CL, "Fail to navigate to By Genre");
                }
                if (!CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem(category))
                {
                    FailStep(CL, "Failed to navigate to By Genre category : " + category + "");
                }
                else
                {
                    LogCommentInfo(CL, "Navigated to category : " + category.ToUpper() + "");
                }

                CL.EA.UI.Utils.SendIR("SELECT");

                CL.IEX.Wait(5);
            }

            CL.IEX.Wait(660); // wait for 11 minutes


            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //verifying AMS Tags

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.GuideBySingleChannel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify AMS TAg in TV Guide :Single Channel");
            }
            else
            {
                LogCommentInfo(CL, "Verified AMS TAg in TV Guide :Single Channel");
            }

            res = CL.EA.VerifyAMSTags(EnumAMSEvent.GuideByGenre);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify AMS TAg in TV Guide :By Genre");
            }
            else
            {
                LogCommentInfo(CL, "Verified AMS TAg in TV Guide :By Genre");
            }

            string eventName;
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
    #endregion
 
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}