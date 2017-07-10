/// <summary>
///  Script Name : TimeSpanChannelSurfStability.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Mayank Srivastava
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using System.Threading;
using System.Collections;

[Test("TimeSpanChannelSurfStability")]
public class TimeSpanChannelSurfStability : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //shared members between steps
    static string firstZapMilestone;
    static string restZapMilestone;
    static string serviceLineup;
    static string testDuration;
    static string firstService;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch proper values from INIs";
    private const string STEP1_DESCRIPTION = "Step 1: Start Executing Time Span Stability Test consisting of Channel Surf";


    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);


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

            serviceLineup = CL.EA.GetValueFromINI(EnumINIFile.Project, "SERVICE LIST", "SERVICES");
            if (serviceLineup == null)
            {
                FailStep(CL, "Service Line up in not defined in your project ini ");
            }
            firstZapMilestone = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FIRST_ZAP_MILESTONES");
            if (firstZapMilestone == null)
            {
                FailStep(CL, "First Milestones are not defined in Test Ini");
            }
            restZapMilestone = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "REST_ZAP_MILESTONES");
            if (restZapMilestone == null)
            {
                FailStep(CL, "Rest Milestones are not defined in Test Ini");
            }

            testDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TEST_DURATION");
            if (testDuration == null)
            {
                FailStep(CL, "Test Duration is not defined in Test Ini");
            }
            
            firstService = CL.EA.GetValueFromINI(EnumINIFile.Project, "SERVICE LIST", "FIRST_SERVICE");
            if (firstService == null)
            {
                FailStep(CL, "Test Duration is not defined in Test Ini");
            }
           
            res  = CL.EA.TuneToChannel(firstService);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to tune to first service");
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

          //this is to split radio and video service from list
            string[] arrServiceLineup = serviceLineup.Split(',');
            Dictionary<int, string> dicarrServiceLineup = new Dictionary<int, string>();
            foreach (string str in arrServiceLineup)
            {
                if (str.Contains("R"))
                {
                    dicarrServiceLineup.Add(Convert.ToInt32(str.Substring(0, str.Length - 1)), "R");
                }
                else
                    dicarrServiceLineup.Add(Convert.ToInt32(str), "V");
            }
            List<int> lst = new List<int>();
            foreach (int key in dicarrServiceLineup.Keys)
            {
                lst.Add(key);
            }
            //sorting the list 
            lst.Sort();

            //fetching current system time
            DateTime dateTime = DateTime.Now;
            LogCommentImportant(CL, "Current Time is :" + dateTime.ToString());

            //adding test duration to be ran
            TimeSpan timespan = new TimeSpan(Convert.ToInt32(testDuration), 0, 0);
           
            //getting added value upto which test to be executed
          
            DateTime combinedTime = dateTime.Add(timespan);
            LogCommentImportant(CL, "Combined Time upto which script to be ran :" + combinedTime);

            int counter = 1;
            string keyToSend = "";
            bool message = true;
           

            DateTime now = DateTime.Now;
            int total = 0;
            while (now < combinedTime)
            {

                keyToSend = "";

                if (counter % 2 == 0)
                    keyToSend = "SELECT_UP";
                else
                    keyToSend = "SELECT_DOWN";

                ArrayList Actualline = new ArrayList();
                string currentChName = "";
                string obtainedChName = "";

                CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
                CL.IEX.Wait(10);

                LogCommentWarning(CL, "COUNTER VALUE" + counter.ToString());

                CL.EA.UI.Utils.GetEpgInfo("chnum", ref currentChName, false);
                LogCommentInfo(CL, "Current Channel Number is : " + currentChName);
                int test = 0;
                
                for (int i = 0; i < lst.Count - 1; i++)
                {
                    message = true;
                    try
                    {
                        if (i == 0)
                        {
                            CL.EA.UI.Utils.BeginWaitForDebugMessages(firstZapMilestone, 60);
                            CL.EA.UI.Utils.SendIR(keyToSend, 4000);
                            message = CL.EA.UI.Utils.EndWaitForDebugMessages(firstZapMilestone, ref Actualline);

                            if (!message)
                            {
                                FailStep(CL, "No Response from BOX observed");
                            }
                            test++;
                        }
                        else
                        {
                            CL.EA.UI.Utils.BeginWaitForDebugMessages(restZapMilestone, 60);
                            CL.EA.UI.Utils.SendIR(keyToSend, 4000);
                            message = CL.EA.UI.Utils.EndWaitForDebugMessages(restZapMilestone, ref Actualline);

                            if (!message)
                            {
                                FailStep(CL, "No Response from BOX observed");
                            }
                            test++;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogCommentFailure(CL,"Unable to Send Key even after 2 retries" + ex.Message);
                        FailStep(CL, "Test exited after IR key retries");
                    }

                }
                LogCommentWarning(CL, "Test value " + test);

                CL.EA.UI.Utils.GetEpgInfo("chnum", ref obtainedChName, false);
                if (obtainedChName == lst[lst.Count - 1].ToString())
                {
                    //once complete traversing of list is done reverse list to continue execution
                    lst.Reverse();
                    counter++;
                }
               //adding total value
                total = total + test;
                test = 0;
              
                LogCommentImportant(CL, "Total Execution ran is : " + total.ToString());
                
                now = DateTime.Now;

            }
            //adding basic integrity test after duration of test gets completed
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR TIME OUT");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Basic Integrity Test to set channel bar time out failed after performing continous zaps");
            }

            PassStep();
        }
    }
    #endregion

    #endregion

}