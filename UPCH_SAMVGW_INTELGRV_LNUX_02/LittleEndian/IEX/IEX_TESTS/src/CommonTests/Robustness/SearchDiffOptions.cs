/// <summary>
///  Script Name : SearchDiffOptions.cs
///  Test Name   : SearchDiffOptions
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Created by : Avinash Budihal.
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;
using System.Diagnostics;
using IEX.ElementaryActions.Functionality;

[Test("SearchDiffOptions")]
public class SearchDiffOptions : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static int LOOP_COUNT;
    static int FAIL_COUNT = 0;
    private const string PRECONDITION_DESCRIPTION = "Precondition:Fetch Value from Test INI";
    private const string STEP1_DESCRIPTION = "Step 1: Search based on RESET KEYWORD,SOURCE,GENRE,CHANNEL,DAY,VIDEO FORMAT,DAYPART,PRICE";
    private const string STEP2_DESCRIPTION = "Step 2:Health check";

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
        base.PreExecute();
    }
    #endregion

    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
            try
            {
                LOOP_COUNT = Convert.ToInt32(CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "LOOP_COUNT"));
            }
            catch (Exception ex)
            {
                FailStep(CL, "LOOP_COUNT not defined in Test INI");
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

            var watch = Stopwatch.StartNew();

            while (LOOP_COUNT > 0)
            {
                //Navigate to Search
                try
                {
                    string[] diffOptions = (CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEFF_OPTIONS")).Split(',');

                    foreach (string opt in diffOptions)
                    {
                    res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SEARCH");
                    CL.IEX.Wait(2);

                    CL.EA.UI.Utils.SendIR("KEY_1");
                    CL.IEX.Wait(2);

                    CL.EA.UI.Utils.SendIR("SELECT_DOWN");
                    CL.IEX.Wait(2);

                    CL.EA.UI.Utils.SendIR("SELECT");
                    CL.IEX.Wait(2);

                    CL.EA.UI.Utils.SendIR("SELECT_RIGHT");
                    CL.IEX.Wait(2);
                    string options = "RESET KEYWORD";
                    while (options != opt)
                    {
                        CL.EA.UI.Utils.SendIR("SELECT_DOWN");
                        CL.IEX.Wait(2);

                        res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out options);
                        if (!res.CommandSucceeded)
                        {
                            FailStep(CL, res, "Failed to Get the " + opt);
                        }
                    }
                        CL.EA.UI.Utils.SendIR("SELECT");
                        CL.IEX.Wait(2);

                        CL.EA.UI.Utils.SendIR("MENU");
                        CL.IEX.Wait(2);
                    }

                    LOOP_COUNT--;

                    LogCommentInfo(CL, "Search Navigation Passed for " + LOOP_COUNT.ToString() + "Times");
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                }
                catch
                {
                    FAIL_COUNT++;
                    if (FAIL_COUNT > (0.1 * LOOP_COUNT))
                    {
                        watch.Stop();
                        var elapsedMs = watch.ElapsedMilliseconds;


                        FailStep(CL, res, FAIL_COUNT + " times failed to navigate to search. Test ran for " + elapsedMs.ToString() + "MilliSeconds");
                    }

                    LogCommentWarning(CL, "Number of times Search navigation failed: " + FAIL_COUNT);
                }
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

            /// Health check
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "", NumberOfPresses: 1, DoTune: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf in live");
            }
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Guide");
            }

            PassStep();
        }
    }
    #endregion

    #region PostExecute

    public override void PostExecute()
    {
    }

    #endregion PostExecute
}
