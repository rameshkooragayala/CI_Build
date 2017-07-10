/// <summary>
///  Script Name : DCAPerformance.cs
///  Test Name   : DCA Performance
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

[Test("DCAPerformance")]
public class DCAPerformance : _Test
{
    [ThreadStatic]
    static _Platform CL;
    //List of Service Variables to be used

    private static Service Service_1;
    private static Service Service_2;

    //Shared Variables between Steps
    static string loop;
    static string threshold;
    static string firstServicePositiveCrit;
    static string secondServicePositiveCrit;
    static string delimiter;
    static string timeout;
    //constant string is defined for TEST PARAMS
    static string testIniSection = "TEST PARAMS";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Fetch Value from Test INI ";
    private const string STEP1_DESCRIPTION = "Step 1: Performance Measurement for Channel Surf while performing DCA";
    

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

            //fetching value from test ini and do null check

            loop = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "LOOP");
            if (loop == null)
            {
                FailStep(CL, "Failed to Fetch Value for Loop");
            }

           
           
            threshold = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "THRESHOLD");
            if (threshold == null)
            {
                FailStep(CL, "Failed to Fetch Value for Threshold");
            }
            
            delimiter = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "DELIMITER");
            if (delimiter == null)
            {
                FailStep(CL, "Failed to Fetch Value for Delimiter ");
            }

            firstServicePositiveCrit = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "TRANSPONDER_1_SERVICE");
            if (firstServicePositiveCrit == null)
            {
                FailStep(CL, "Failed to Fetch Value for First Service ");
            }
            secondServicePositiveCrit = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "TRANSPONDER_2_SERVICE");
            if (secondServicePositiveCrit == null)
            {
                FailStep(CL, "Failed to Fetch Value for Second Service ");
            }

            timeout = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "TIMEOUT");
            if (timeout == null)
            {
                FailStep(CL, "Failed to Fetch Value for Time Out");
            }
            //Fetch value from Content XML First Service 
            Service_1 = CL.EA.GetServiceFromContentXML(firstServicePositiveCrit, "ParentalRating=High;IsDefault=True");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "First Service fetched is : " + Service_1.LCN);
            }

            //Fetch value from Content XML Second Service  
            Service_2 = CL.EA.GetServiceFromContentXML(secondServicePositiveCrit, "ParentalRating=High");
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Second Service fetched is : " + Service_2.LCN);
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
            //These two variables are used for storing value for time diffrence between two milestones and finally total time diffrence after number of iteration
            double timeDifference = 0.0;
            double totalTimeDifference = 0.0;
           

            int logTimeout = Convert.ToInt32(timeout);
            string ServiceMilestones = CL.EA.GetValueFromINI(EnumINIFile.Test, testIniSection, "STRING");
            if (ServiceMilestones == null)
            {
                FailStep(CL, "Failed to Fetch Value for Milestones to check");
            }
            LogCommentInfo(CL, "Performing following number of Iteration : "+ loop);
            int numberOfIteration = Convert.ToInt32(loop);
            
            for (int i = 1; i <= numberOfIteration; i++)
            {

                string toAppendServiceString = "";
                string channel_num = "";
                string milestoneCreated = "";

                ArrayList Actualline = new ArrayList();

                
                CL.IEX.LogComment("Performing DCA to the Service");

                

                //Channel Surf betwwen diffrent/same service to be performed
                // If A is first service and B is second Service then order is A-B-A-B
                //considering A as Odd and B as Even channel surf is performed.This is as to reduce depedency on multiple log of same type
                if (i % 2 == 0)
                {
                    channel_num = Service_2.LCN;
                }
                else
                {
                    channel_num = Service_1.LCN;
                }

               //every time we need unique milestones for validation after DCA
                toAppendServiceString = delimiter + channel_num;
                milestoneCreated = ServiceMilestones.Replace(delimiter, toAppendServiceString);

               
                LogCommentInfo(CL, "string after appending : " + milestoneCreated);

               //Validation Starts from Here
                CL.EA.UI.Utils.BeginWaitForDebugMessages(milestoneCreated, logTimeout);

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, channel_num);
                if (!res.CommandSucceeded)
                {
                    LogCommentWarning(CL, "Failed to Surf to channel" + channel_num);
                }

                CL.EA.UI.Utils.EndWaitForDebugMessages(milestoneCreated, ref Actualline);

               
                //Actual Line value is sent to function to parse time which is stored as list

                List<double> timeArray = CL.EA.UI.Utils.ParseMileStoneTimeFromLogArray(Actualline);

                //time difference is calculated from after subtracting value from timearray
                timeDifference = timeArray[1] - timeArray[0];

                totalTimeDifference = totalTimeDifference + timeDifference;
               
               LogCommentInfo(CL, "Total Time Difference is : " + totalTimeDifference.ToString());
                
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