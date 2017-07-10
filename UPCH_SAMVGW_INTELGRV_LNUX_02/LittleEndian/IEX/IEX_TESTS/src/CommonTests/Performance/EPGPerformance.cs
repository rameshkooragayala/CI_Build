/// <summary>
///  Script Name : Performance.cs
///  Test Name   : EPG_Performance
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Mayank Srivastava
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;
using System.IO;

[Test("Performance")]
public class EPGPerformance : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration

    //Shared members between steps
    static string context;
    static string listItem;
    static string irKey;
    static string startString;
    static string endString;
    static string loop;
    static string threshold;
    static string timeout;
    static  double min;
    static double max;
	static  double averageTime;
	static     int runningNumOfIter ;
	static double firstLaunchValue ;
	static int thresholdValue;
    
	//constant string is defined for TEST PARAMS
    static string parameter = "TEST PARAMS";
    static helper Helper = new helper();

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Value for Test Parameters and Print their Value";
    private const string STEP1_DESCRIPTION = "Step 1: Perform EPG Performance Test ";

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

            //Get Values From Test.ini File and do NULL Check for every field
            context = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "CONTEXT");
            if (context == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for CONTEXT" );
            }

            irKey = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "IR_KEY");
            if (irKey == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for IR KEY");
            }

            startString = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "START_STRING");
            if (startString == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for START STRING");
            }

            endString = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "END_STRING");
            if (endString == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for END STRING");
            }

            loop = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "LOOP");
            if (loop == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for LOOP");
            }

            threshold = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "THRESHOLD");
            if (threshold == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for THRESHOLD");
            }
            timeout = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "TIMEOUT");
            if (timeout == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for TIME OUT");
            }
            //This is optional parameter whose value depends upon context
            listItem = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "ITEM");
            if (listItem == null)
            {
                LogCommentWarning(CL, "ITEM is not defined in Test INI as there is no need for it according to context");
            }
            PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {

        //These two variables are used for storing value for time diffrence between two milestones and finally total time diffrence after number of iteration

        public override void Execute()
        {
            StartStep();

            //These two variables are used for storing value for time diffrence between two milestones and finally total time diffrence after number of iteration
            double timeDifference = 0.0;
            double totalTimeDifference = 0.0;
			//These variables are used to store values for runningIteration,first launch value and failed threshold value  
           // to be passed in xml file to display
			
           
          
            //Appending Start String and End String Value in a Single String to avoid multiple begin wait and end wait
            string milestones = startString + "," + endString;

            LogCommentInfo(CL, "String after appending start and end string :" + milestones);

            int numberOfIteration = Convert.ToInt32(loop);

            LogCommentInfo(CL, "These many number of iterations are going to be performed :" + loop);
             bool message = true;
          
            //This list stores value for time difference in each iteration
		    List<double> timedifferenceArray = new List<double>();
            for (int i = 1; i <= numberOfIteration; i++)
            {

                ArrayList Actualline = new ArrayList();

                LogCommentInfo(CL, "Trying to Navigate To : " + context + "Iteration : " + i.ToString());
                CL.IEX.MilestonesEPG.Navigate(context);

                if (listItem != String.Empty)
                {
                    LogCommentInfo(CL, "Navigating to Item : " + listItem);
                    CL.IEX.MilestonesEPG.SelectMenuItem(listItem);
                }


                string timeStamp = "";
                
                //logTimeout variable is used to do begin wait for particular time which depends upon project
                int logTimeout = Convert.ToInt32(timeout);
                CL.EA.UI.Utils.BeginWaitForDebugMessages(milestones,logTimeout );

                LogCommentInfo(CL, "Sending IR Key : " + irKey);
                res = CL.IEX.IR.SendIR(irKey, out timeStamp, 3000);

                message = CL.EA.UI.Utils.EndWaitForDebugMessages(milestones, ref Actualline);
                if(message == false)
                {
                    FailStep(CL,"Cannot find proper time stamp logs please check udp log for further investigation");
                }

                //Actual Line value is sent to function to parse time which is stored as list

                List<double> timeArray = CL.EA.UI.Utils.ParseMileStoneTimeFromLogArray(Actualline);

                //time difference is calculated from after subtracting value from timearray
                timeDifference = timeArray[1] - timeArray[0];
				//this stores value for the first time
                if (i == 1)
                {
                    firstLaunchValue = timeDifference;
                    firstLaunchValue = Math.Round(firstLaunchValue, 3);
                }
                LogCommentImportant(CL, "Adding time difference value for each iteration in an array");
                timedifferenceArray.Add(timeDifference);
                //for each iteration time difference value is stored in variable totalTimeDifference
                totalTimeDifference = totalTimeDifference + timeDifference;
                LogCommentInfo(CL, "Time taken by the scenario in milli seconds : " + context + "---> " + timeDifference.ToString());
               
                //stores present running number of iteration
                runningNumOfIter = i;

                LogCommentImportant(CL, "Running number of iteration : " + runningNumOfIter.ToString());
                CL.EA.ReturnToLiveViewing(true);
            }

            foreach (double timedifference in timedifferenceArray)
            {
                LogCommentImportant(CL, "Complete List is as : " + timedifference.ToString());
            }
			
			//Sorting the array and finding max and min value
            timedifferenceArray.Sort();
             min = timedifferenceArray[0];
             max = timedifferenceArray[timedifferenceArray.Count -1];
			
			//Rounding value to 3 decimal places
            min = Math.Round(min, 3);
            max = Math.Round(max, 3);
            LogCommentImportant(CL, "Minimum value is " + min.ToString());
            LogCommentImportant(CL, "Maximum Value is " + max.ToString());
           
            //Average time taken for complete iteration is calculated     
            averageTime = totalTimeDifference / ((double)(numberOfIteration));

            averageTime = Math.Round(averageTime, 3);
            LogCommentInfo(CL, "Average time taken for" + numberOfIteration + " executions:" + averageTime.ToString());

            //Comparing Threshold Value and Average Time 

          thresholdValue = Convert.ToInt32(threshold);

            LogCommentInfo(CL, "Checking against threshold value set for execution :" + threshold);
            if (averageTime > ((double)(thresholdValue)))
            {
                Helper.test();
                FailStep(CL, "Average Time:" + averageTime.ToString() + "was more than Threshold Value:" + threshold);

            }

            Helper.test();
            PassStep();
        }
    }
    #endregion
    #endregion
    #region helper
    public class helper : _Step
    {
        public override void Execute()
        { }
        public void test()
        {
           //creating performance object and adding values
            Performance perfobj = new Performance();
            perfobj.MinimumTime = min;
            perfobj.MaximumTime = max;
            perfobj.AverageTime = averageTime;
            perfobj.TimeForFirstLaunchTime = firstLaunchValue;
            perfobj.RunningNumberOfIteration = runningNumOfIter;
			perfobj.Thresholdvalue = thresholdValue;
			//Sending Passed Parameter for XML file generation
            LogCommentInfo(CL, "Creating Performance.Xml file inside Log Directory");
            CL.EA.UI.Utils.CreateandWritePerformanceResultXML(perfobj);
        }
    }
    #endregion
}