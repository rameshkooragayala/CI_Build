using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.HealthCheck;

public class HealthCheck : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps    
    static string navigationsList = "HealthCheck.Navigations.txt";
    static string expectedMilestonesFile = "HealthCheck.ExpectedMilestonesPerState.txt";
    static HealthCheckManager m;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new Step1(), "Step 1: Health Check Execution");

        //Get Client Platform
        CL = GetClient();

        m = new HealthCheckManager(this, navigationsList, expectedMilestonesFile);
    }
    #endregion

    #region Steps
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            bool hasFailures = false;

            CL.EA.ReturnToLiveViewing();

            hasFailures = m.Run();
            if (hasFailures)
            {
                FailStep(CL, "Some Navigations Failed");
            }

            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}