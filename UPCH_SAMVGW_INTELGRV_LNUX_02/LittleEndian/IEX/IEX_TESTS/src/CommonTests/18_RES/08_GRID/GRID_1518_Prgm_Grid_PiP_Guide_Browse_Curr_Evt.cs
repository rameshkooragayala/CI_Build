/// <summary>
///  Script Name : GRID_1518_Prgm_Grid_PiP_Guide_Browse_Curr_Evt.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("GRID_1518_Prgm_Grid_PiP_Guide_Browse_Curr_Evt")]
public class GRID_1518 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static string channel1;
    static string evtName;
    static string evtNameFrmGrid;
    static bool isInGuide;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1:Launch guide from Servie s1 & verify focus is on current event ";
    private const string STEP2_DESCRIPTION = "Step 2:Surf to service s2 and verify PIP ";
  

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

             channel1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "channel1");

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, channel1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Fail to Tune to service s1");
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
            // fetch event name
            CL.IEX.Wait(1);
           res=  CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtName);
           if (!res.CommandSucceeded)
           {
               FailStep(CL, "Fail to get evtname Milestone");
           }
            // navigate to All Channel
            CL.EA.UI.Guide.Navigate();
           isInGuide= CL.EA.UI.Guide.IsGuide();
           if (!isInGuide)
           {
               FailStep(CL,"Failed to launch All channels Guide");
           }
            else
           {
               LogCommentInfo(CL, "Verified All Channels guide launched");
           }
             CL.IEX.Wait(1);

            // fetch evtName from grid

             res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtNameFrmGrid);
             if (!res.CommandSucceeded)
             {
                 FailStep(CL, "Fail to get evtName Milestone");
             }

             if (evtName == evtNameFrmGrid)
             {
                 LogCommentInfo(CL, "Verified focus is on current event after launching grid");
             }
             else
             {
                 FailStep(CL, "Fail to verify focus is on current event after launching grid");
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
            try
            {

            CL.EA.UI.Guide.SurfChannelDown("WithPIP");
            }
            catch(Exception ex)
            {
                FailStep(CL, "Fail to verify PIP in Service s2, Reason" + ex.Message);
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