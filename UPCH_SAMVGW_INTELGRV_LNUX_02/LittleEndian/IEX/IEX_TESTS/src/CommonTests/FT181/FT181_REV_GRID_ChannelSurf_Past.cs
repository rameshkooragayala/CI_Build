/// <summary>
///  Script Name : FT181_REV_GRID_ChannelSurf_Past.cs
///  Test Name   : FT181-REV-GRID-ChannelSurf-Past
///  TEST ID     : -
///  QC Version  : 1
/// -----------------------------------------------
///  Modified by : Ganpat Singh
///  Modified Date : 17/10/2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("REVERSE GRID- ChannelSurf")]
public class FT181_RevGrid_ChSurf : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static string Service1 = "";
    private static string Service2 = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From XML File & Sync";
    private const string STEP1_DESCRIPTION = "Step 1:Navigating to past event on Guide, channel surf using DCA and verifying that the focused event is past event after channel surf";
    private const string STEP2_DESCRIPTION = "Step 2:Channel surf UP/DOWM and verifying that the focused event is past event after channel surf";

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

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From Test ini File
            Service1 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Service1");
            if (String.IsNullOrEmpty(Service1))
            {
                FailStep(CL, "Failed to get the service number from test.ini");
            }

            Service2 = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Service2");
            if (String.IsNullOrEmpty(Service2))
            {
                FailStep(CL, "Failed to get the service number from test.ini");
            }

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Navigate To grid
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to TV GUIDE");
            }


            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, Service1, IsPredicted: EnumPredicted.Ignore);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do channel surf");
            }

            //Navigate to past event of current service
            CL.EA.UI.Guide.PreviousEvent(2);

            //verify the focused event is past event
            if (CL.EA.UI.Guide.VerifyIsPastEvent())
            {
                LogCommentImportant(CL, "Fucused event is the past event of tunned service");
            }

            else
            {
                FailStep(CL, "Fucused event is not a past event of tunned service");
            }

            //Channel surf using DCA and verify focuse should be on past event
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, Service2, IsPredicted: EnumPredicted.Ignore, IsPastEvent: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do channel surf");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        //Step 2: Channel Surf With Low Digit Channel Number
        public override void Execute()
        {
            StartStep();

            //Channel surf using UP key and verify focuse should be on past event
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, IsNext: false, NumberOfPresses: 4, IsPastEvent: true);
            if (!res.CommandSucceeded)
            {
                 FailStep(CL, res, "Failed to do channel surf");
            }

            //Channel surf using DOWN key and verify focuse should be on past event
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, IsNext: true, NumberOfPresses: 4, IsPastEvent: true);
            if (!res.CommandSucceeded)
            {
                 FailStep(CL, res, "Failed to do channel surf");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
    }

    #endregion PostExecute
}