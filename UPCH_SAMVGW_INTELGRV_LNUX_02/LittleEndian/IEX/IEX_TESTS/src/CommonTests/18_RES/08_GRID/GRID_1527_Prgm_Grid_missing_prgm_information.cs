/// <summary>
///  Script Name : GRID_1527_Prgm_Grid_missing_prgm_information.cs
///  Test Name   : GRID-1527-Prgm-Grid-missing-prgm-information
///  TEST ID     : 18048
///  QC Version  : 9
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

[Test("GRID_1527_Prgm_Grid_missing_prgm_information")]
public class GRID_1527 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static Service S1;
    static Service S2;
    static Service S3;
    static Service S4;

    static string defaultThumbnail;
    static bool isInGuide;
    static string evtName;
    static string timestamp;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Service from content XML";
    private const string STEP1_DESCRIPTION = "Step 1:Launch guide from S1 & Verify focus is on current event ";
    private const string STEP2_DESCRIPTION = "Step 2:Move to Service S2 & verify No Event Information Available";
    private const string STEP3_DESCRIPTION = "Step 3: Launch S3 , channel bar & verify No Event Information Available ";
    private const string STEP4_DESCRIPTION = "Step 4: Launch guide from S4 & Verify Thumbnail on future event";

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);

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
            FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");
            // get service from content xml
            S4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;HasThumbnail=True", "ParentalRating=High");
            S1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + S4.LCN  + "");
            S2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=False", "ParentalRating=High;LCN=" + S1.LCN + "," + S4.LCN + "");
            S3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=False", "ParentalRating=High;LCN=" + S1.LCN + "," + S2.LCN + "," + S4.LCN + "");


            defaultThumbnail = CL.EA.GetValueFromINI(EnumINIFile.Project, "THUMBNAIL", "DEFAULT");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, S1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + S1.LCN);
            }

            // navigate to All Channel
            CL.EA.UI.Guide.Navigate();
            isInGuide = CL.EA.UI.Guide.IsGuide();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch All channels Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified All Channels guide launched");
            }

            CL.IEX.Wait(1);
            // surf to Service S2
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, S2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + S2.LCN + " from Guide");
            }

            CL.IEX.Wait(1);

            // fetching event name

            CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);

            if (evtName == "No programme information available")
            {

                LogCommentInfo(CL, "Verified No information available");
            }
            else
            {
                FailStep(CL, "Failed - Event information is  available");
            }

            //Return to Live
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed return to live");
            }
            
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
            CL.IEX.Wait(1);
            // Surf Service S2
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, S2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + S2.LCN);
            }
            CL.IEX.Wait(1);

            //getting event information in channel bar

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
            if (evtName == "Sorry, there's no information available")
            {

                LogCommentInfo(CL, "Verified No information available");
            }
            else
            {
                FailStep(CL, "Failed - Event information is  available");
            }
      

            PassStep();
        }
    }
    #endregion
    #region Step3
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            CL.IEX.Wait(1);

            // surf to service S3
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, S3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + S3.LCN);
            }

            // navigate to All Channel
            CL.EA.UI.Guide.Navigate();
            isInGuide = CL.EA.UI.Guide.IsGuide();
            if (!isInGuide)
            {
                FailStep(CL, "Failed to launch All channels Guide");
            }
            else
            {
                LogCommentInfo(CL, "Verified All Channels guide launched");
            }
            
            CL.IEX.Wait(1);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
            if (evtName == "No programme information available")
            {

                LogCommentInfo(CL, "Verified No information available");
            }
            else
            {
                FailStep(CL, "Failed - Event information is  available");
            }
      

            PassStep();
        }
    }
    #endregion
    #region Step4
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        public override void Execute()
        {
           

            StartStep();

           
            CL.IEX.Wait(1);

            // surf to Service S4
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, S4.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + S4.LCN + " from Guide");
            }

            CL.IEX.MilestonesEPG.ClearEPGInfo();

            //Move to future event

            res= CL.IEX.SendIRCommand("SELECT_RIGHT",1, ref timestamp);

            CL.IEX.Wait(1);
            res = CL.IEX.SendIRCommand("SELECT", 1, ref timestamp);

            CL.IEX.Wait(1);

            // fetching thumbnail  milestone and verify
            CL.IEX.Wait(2);
            String thumbnailName="";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out thumbnailName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get thumbnail From Channel Bar");
            }
            CL.IEX.Wait(4);
            if (string.IsNullOrEmpty(thumbnailName))
            {
                FailStep(CL, "Thumbnail value fetched from EPG is null");
            }
            else if (thumbnailName.Trim() == defaultThumbnail.Trim())
            {
                FailStep(CL, "Thumbnail value fetched from EPG is Default Thumbnail");
            }
            else
            {
                LogCommentInfo(CL, "Verified Thumbnail " + thumbnailName);
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