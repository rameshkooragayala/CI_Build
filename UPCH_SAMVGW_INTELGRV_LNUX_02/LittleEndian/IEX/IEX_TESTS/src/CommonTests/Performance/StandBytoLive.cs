/// <summary>
///  Script Name : StandBytoLive.cs
///  Test Name   : Performance Measurement From Standy to Live
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : 
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;


[Test("StandBytoLive")]
public class StandBytoLive : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static string startString;
    static string endString;
    static string loop;
    static string threshold;
    static string timeout;

    //constant string is defined for TEST PARAMS
    static string testIniSection = "TEST PARAMS";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Measuring Performance from Stand By to Live ";
    

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


    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Fetch following values from Test INI and do a NULL Check

            startString = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "START_STRING");
            if (startString == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for START STRING");
            }

            endString = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "END_STRING");
            if (endString == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for END STRING");
            }

            loop = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "LOOP");
            if (loop == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for LOOP");
            }

            threshold = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "THRESHOLD");
            if (threshold == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for THRESHOLD");
            }

            timeout = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "TIMEOUT");
            if (timeout == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for TIME OUT");
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

            //Appending Start String and End String Value in a Single String to avoid multiple begin wait and end wait


            //These two variables are used for storing value for time diffrence between two milestones and finally total time diffrence after number of iteration
            double timeDifference = 0.0;
            double totalTimeDifference = 0.0;

            //logTimeout variable is used to do begin wait for particular time which depends upon project
            int logTimeout = Convert.ToInt32(timeout);
            //Appending Start String and End String Value in a Single String to avoid multiple begin wait and end wait
            string milestones = startString + "," + endString;
           
            string standByWait = CL.EA.GetValueFromINI(EnumINIFile.Project, "STANDBY", "SAFE_DELAY_SEC");
            int numberOfIteration = Convert.ToInt32(loop);

            LogCommentInfo(CL, "These many number of iterations are going to be performed :" + loop);

            for (int i = 1; i <= numberOfIteration; i++)
            {

                ArrayList Actualline = new ArrayList();
                
                
                //Box Should be in Stand By Mode
                //Entering Stanby 
                CL.EA.UI.Utils.BeginWaitForDebugMessages(milestones, logTimeout);
               
                res = CL.EA.StandBy(false);
                if (!res.CommandSucceeded)
                {
                    LogCommentImportant(CL, "Failed to Enter Standby");
                }
               
                //min time to stay on Stand By
                LogComment(CL, "Waiting for " + standByWait + " seconds in standby");
               
               CL.IEX.Wait(double.Parse(standByWait));
               
             
                //Stand By Out Request
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Exit from StandBy");
                }
                CL.EA.UI.Utils.EndWaitForDebugMessages(milestones, ref Actualline);

                //Actual Line value is sent to function to parse time which is stored as list

                List<double> timeArray = CL.EA.UI.Utils.ParseMileStoneTimeFromLogArray(Actualline);

                //time difference is calculated from after subtracting value from timearray
                timeDifference = timeArray[1] - timeArray[0];
                LogCommentInfo(CL, "Time taken by the scenario in milli seconds :---> " + timeDifference.ToString());

                //for each iteration time difference value is stored in variable totalTimeDifference
                totalTimeDifference = totalTimeDifference + timeDifference;
            }
            //Average time taken for complete iteration is calculated     
            double averageTime = totalTimeDifference / ((double)(numberOfIteration));

            LogCommentInfo(CL, "Average time taken for" + numberOfIteration + " executions:" + averageTime.ToString());

            //Comparing Threshold Value and Average Time 

            int thresholdValue = Convert.ToInt32(threshold);

            LogCommentInfo(CL, "Checking against threshold value set for execution :" + threshold);
            if (averageTime > ((double)(thresholdValue)))
            {
                FailStep(CL, "Average Time:" + averageTime.ToString() + "was more than Threshold Value:" + threshold);

            }
            PassStep();
        }
    }
    #endregion
    #endregion


}