using System.Collections.Generic;
using System.IO;
using IEX.Tests.Engine;

namespace IEX.HealthCheck
{
    public class HealthCheckManager
    {
        private _Test test;
        private _Platform CL;
        private NavigationsManager actionsList;
        private StatesManager expectedMilestones;
        private LogManager Logger;

        public HealthCheckManager(_Test test, string navigationsFile, string expectedMilestonesFile)
        {
            this.test = test;
            this.actionsList = new NavigationsManager(navigationsFile);
            this.expectedMilestones = new StatesManager(expectedMilestonesFile);
            this.CL = test.Platforms[0];
            Logger = new LogManager(CL, Path.GetDirectoryName(CL.EA.LogFilePath));
        }

        public int Run()
        {
            bool hasFailures = false;             
            string action = actionsList.GetNextAction();
            int iFailuresCount = 0;
            while (action != null)
            {
                if (action == "Navigation")
                {
                    NavigationEntry currentNavigation = actionsList.GetNextNavigation();
                    hasFailures = HandleNavigationRequest(currentNavigation);
                    int iActionCount = 0;
                    while (hasFailures && iActionCount < 2)
                    {
                        actionsList.CurrentAction--;
                        action = actionsList.GetNextAction();
                        actionsList.CurrentNav--;
                        currentNavigation = actionsList.GetNextNavigation();
                        currentNavigation.CurrentIndex = 0;
                        hasFailures = HandleNavigationRequest(currentNavigation);
                        iActionCount++;
                        if (iActionCount == 2 && hasFailures)
                        {
                            iFailuresCount++;
                            LogWriter("**************Navigation Failed After " + (iActionCount + 1) + " Try**************");
                        }
                    }
                }
                else if (action == "SelectMenuItem")
                {
                    hasFailures = HandleSelectItemRequest();
                }
                action = actionsList.GetNextAction();
            }

            Logger.WriteSummary();
            return iFailuresCount;
        }

        private bool HandleNavigationRequest(NavigationEntry currentNavigation)
        {
            bool hasFailures = false;
            IEXGateway._IEXResult res;
            List<KeyValuePair<string, MilestoneError>> msError;

            Logger.StartNavigation(currentNavigation.GetFullPath());
            string nextState = currentNavigation.GetNextState();

            while (nextState != null)
            {
                State dest = expectedMilestones.GetStateByName(nextState);
                if (dest == null)
                {
                    LogWriter("Error: State " + nextState + " is Not Defined in ExpectedMilestonesPerState.txt");
                    break;
                }

                if (nextState.Equals("[PIN]"))
                {
                    nextState = currentNavigation.GetNextState();
                    CL.EA.EnterDeafultPIN(nextState.Replace("amp;", ""));
                }

                Logger.StartDestination(dest);
                MilestonesChecker.Start(CL, nextState);
                res = CL.IEX.MilestonesEPG.Navigate(nextState.Replace("amp;", ""));   //To handle "PIN & PARENTAL CONTROL";
                msError = MilestonesChecker.Finish(dest, CL, res, nextState);
                if (msError.Count > 0)
                {
                    hasFailures = true;
                    Logger.DestinationFail(msError);
                    if (!res.CommandSucceeded)
                    {
                        LogWriter("Navigation Failed : " + res.FailureReason);
                        CL.IEX.GetSnapshot("Navigation Failed");
                        Logger.FailNavigation(res.FailureReason);
                        break;
                    }
                    LogWriter("Navigation Succeeded But Validation Failed, Check outPut.txt For More Details");
                }
                nextState = currentNavigation.GetNextState();
            }
            Logger.FinishNavigation(hasFailures);

            if (hasFailures)
            {
                return true;
            }
            return false;
        }

        private bool HandleSelectItemRequest()
        {
            IEXGateway._IEXResult res;

            string item = actionsList.GetNextSelectMenuItem();
            CL.IEX.StartHideFailures("Select Menu Item: " + item, true, colour: "Blue");
            res = CL.IEX.MilestonesEPG.SelectMenuItem(item);
            CL.IEX.EndHideFailures();
            if (!res.CommandSucceeded)
            {
                CL.IEX.LogComment("====== Select Menu Item Failed! ======", colour: "Red");
                return false;
            }
            CL.IEX.LogComment("====== Select Menu Item Passed ======", colour: "Green");
            return true;
        }

        private void LogWriter(string text)
        {
            System.Console.WriteLine(text);
            test.LogCommentFailure(CL, text);
        }
    }
}