using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.HealthCheck;


public class HealthCheck : _Test
{
[ThreadStatic]
     static _Platform CL;

    //Test Duration
    //Channels used by the test
    static string FTA_Channel;

    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Check Milestones");

        //Get Client Platform
	CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
StartStep();

            //Get Values From ini File
            FTA_Channel = CL.EA.GetValue("FTA_Channel");


                res = Env.MountSingle(CL, testDuration);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res);
                }
            }
            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            bool hasFailure = false;
            CL.IEX.Wait(15);

            MilestonesManager test = new MilestonesManager(@"Health Check Input\ExpectedMilestonesPerState.txt");
            NavigationsManager navigations = new NavigationsManager(@"Health Check Input\Navigations.txt");
            OutputManager outPut = new OutputManager(CL);

            Navigation currentNavigation = navigations.GetNextNavigation();
            while (currentNavigation != null)
            {
                outPut.StartNavigation(currentNavigation.GetFullPath());

                string nextDest = currentNavigation.GetNextState();
                while (nextDest != null)
                {

                    outPut.StartDestination(test.GetStateByName(nextDest));
                    MilestonesChecker.Start(test.GetStateByName(nextDest), CL);
                    try
                    {
                        res = CL.IEX.MilestonesEPG.Navigate(nextDest.Replace("amp;", ""));//"PIN & PARENTAL CONTROL");
                    }
                    catch (Exception)
                    {

                    }
                    List<KeyValuePair<string, string>> msError = MilestonesChecker.Finish(test.GetStateByName(nextDest), CL);
                    if (msError.Count > 0)
                    {
                        hasFailure = true;

                        if (!res.CommandSucceeded)
                        {
                            msError.Add(new KeyValuePair<string, string>("Navigation Error: ", res.FailureReason));
                        }
                        outPut.DestinationFail(msError);
                        break;
                    }
                    nextDest = currentNavigation.GetNextState();
                }

                outPut.FinishNavigation();
                
                //return to root 
                res = CL.IEX.MilestonesEPG.Navigate("MAIN MENU");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,"Box is Dead !! :-( ");
                }
                CL.IEX.Wait(15);

                currentNavigation = navigations.GetNextNavigation();
            }
            outPut.WriteSummary();

            if (hasFailure)
            {
              //  FailStep(CL,"Some navigation(s) failed");
            }
            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();


            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();


            PassStep();
        }
    }
    #endregion
    #region Step4
    public class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();


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