/// <summary>
///  Script Name : FT181_REV_GRID_Skip.cs
///  Test Name   : FT181_REV_GRID_Skip
///  TEST ID     : 
///  QC Version  :
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Ganpat Singh
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("FT181_RevGridSkip")]
public class FT181_RevGridSkip : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static string channel1;
    static string selectionDate = "";
    static string currentDate = "";
    static int maxEITAvailable;
    static int maxEITinReverseGrid;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Number from ini file";
    private const string STEP1_DESCRIPTION = "Step 1:Perform forward day skip till last day and than backword day skip till today  ";
    private const string STEP2_DESCRIPTION = "Step 2:Perform backword skip till last day having event informations and than forward skip till today";


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


               channel1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel1");

               maxEITAvailable = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "GUIDEMAXSELECTIONDATE"));

               maxEITinReverseGrid = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Project, "GUIDE", "MAX_DAYS_PAST_EVT"));


               res = CL.EA.ChannelSurf(EnumSurfIn.Live, channel1);
               if (!res.CommandSucceeded)
               {
                   FailStep(CL, "fail to tune to channel1");
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


            res = CL.EA.DaySkipInGuide(EnumGuideViews.ALL_CHANNELS, true, maxEITAvailable, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to perform day skip till last day eit");
            }

            res = CL.EA.DaySkipInGuide(EnumGuideViews.ALL_CHANNELS, false, maxEITAvailable-1 , false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to perform day skip till last day eit");
            }

            CL.EA.UI.Utils.GetEpgInfo("date", ref currentDate);
            CL.EA.UI.Utils.GetEpgInfo("selection date", ref selectionDate);
            currentDate = currentDate.Replace('_', '.');

            if (currentDate.StartsWith(selectionDate))
            {
                LogCommentImportant(CL, "Sucessfully reached to the event of today");
            }
            else
            {
                FailStep(CL, "Failed to reach the event of today after backword skip");
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

            res = CL.EA.DaySkipInGuide(EnumGuideViews.ALL_CHANNELS, false, maxEITinReverseGrid+1, true, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to perform day skip till last day eit");
            }

            res = CL.EA.DaySkipInGuide(EnumGuideViews.ALL_CHANNELS, true, maxEITinReverseGrid, false, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to perform day skip till last day eit");
            }

            CL.EA.UI.Utils.GetEpgInfo("date", ref currentDate);
            CL.EA.UI.Utils.GetEpgInfo("selection date", ref selectionDate);
            currentDate = currentDate.Replace('_', '.');

            if (currentDate.StartsWith(selectionDate))
            {
                LogCommentImportant(CL, "Sucessfully reached to the event of today after skip checking in revese grid");
            }
            else
            {
                FailStep(CL, "Failed to reach the event of today after skip checking in revese grid");
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