/// <summary>
///  Script Name : ChannelSurfStability.cs
///  Test Name   : Stability Measurement of Box after Channel Surf
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

[Test("ChannelSurfStability")]
public class ChannelSurfStability : _Test
{
    [ThreadStatic]
    static _Platform CL;
  
    //Shared members between steps
    static string outerLoop;       // outer loop checks for complete iteration required for test
    static string innerLoop;       // inner loop checks for number of iteration in one iteration
    static string milestoneToCheck;
    static string channelSurfUpKey;
    static string channelSurfDownKey;
    static string timeOut;
    static string timeToSendKey;
    static string positiveCriteria;
   
   
    //constant string is defined for TEST PARAMS
    static string parameter = "TEST PARAMS";


    private const string PRECONDITION_DESCRIPTION = "Precondition: Get test parameters and create a list of services fetched from content.xml";
    private const string STEP1_DESCRIPTION = "Step 1: Performs stability test as per conditions fetched from test ini";
    

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

            //Fetch Test Params from Test.ini and do a null check
            outerLoop = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "OUTER_LOOP");
            if (outerLoop == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Outer Loop");
            }

            innerLoop = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "INNER_LOOP");
            if (innerLoop == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Inner Loop");
            }

            milestoneToCheck = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "MILESTONES_FIELD");
            if (milestoneToCheck == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Milestones to check");
            }

            channelSurfUpKey = CL.EA.GetValueFromINI(EnumINIFile.Project, "LIVE", "CHANNEL_SURF_UP_KEY");
            if (channelSurfUpKey == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Channel Surf Up Key");
            }

            channelSurfDownKey = CL.EA.GetValueFromINI(EnumINIFile.Project, "LIVE", "CHANNEL_SURF_DOWN_KEY");
            if (channelSurfDownKey == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Channel Surf Down Key");
            }

            timeOut = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "TIMEOUT");
            if(timeOut == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Timeout");
            }
            
            timeToSendKey = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "TIME_TO_SEND_KEY");
            if (timeToSendKey == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Time to send key");
            }
           
            positiveCriteria = CL.EA.GetValueFromINI(EnumINIFile.Test, parameter, "POSITIVE_SERVICE_CRITERIA");
            if (positiveCriteria == null)
            {
                FailStep(CL, "Failed to Fetch Value from Script INI for Positive Service Criteria");
            
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
            
          
            
            // End Wait should pass for services with audio/video therefore doing a positive check for those services.
            Boolean servicesverify = true;
            
            //This variable changes the value for channel surf key depending upon number of iteration
            string keytobesent = "";
           
           
           
            //Adding ArrayList for End Wait
            ArrayList Actualline = new ArrayList();

            //logTimeout variable is used to do begin wait for particular time which depends upon project
            int logTimeout = Convert.ToInt32(timeOut);

            //logTimetoSendKey  variable is used to do send keys which will depend upon scenario to scenario
            int logTimetoSendKey = Convert.ToInt32(timeToSendKey);

            // Fetch the services listed in content.xml and then create a list for reference
            List<Service> allServicesList = new List<Service>();
            allServicesList = CL.EA.GetServiceListFromContentXML(positiveCriteria);
           
            //null check for list
            if (allServicesList == null)
            {
                FailStep(CL, "Failed to fetch value from content.xml check if services are listed or not");
            }
            //This prints the list fetched from content.xml
            foreach (Service service in allServicesList)
            {
                LogCommentImportant(CL,"List is as : "+ service.LCN);
            }
           
            int firstloop = Convert.ToInt32(outerLoop);
            int secondloop = Convert.ToInt32(innerLoop);
           
            LogComment(CL, "Performing following number of iteration required in test : " + outerLoop);
            for(int i = 1;i<= firstloop;i++)
            {
                LogCommentInfo(CL, "Performing following numbered iteration for the test: " + i.ToString());
              //This checks sets channel surf keys depending upon number of iteration
                if (i % 2 == 0)
                    keytobesent = channelSurfUpKey;
                else
                    keytobesent = channelSurfDownKey;
               
 
                LogComment(CL, "Performing following number of channel zaps in one iteration :" + innerLoop);
                for (int j = 1; j <= secondloop; j++)
                {
                    LogCommentInfo(CL, "Performing following numbered of channel zaps :" + j.ToString());
                  
                    CL.EA.UI.Utils.BeginWaitForDebugMessages(milestoneToCheck, logTimeout);
                   //Get Current Channel Number 
                    string currentChName = "";
                    CL.EA.UI.Utils.GetEpgInfo("chNum", ref currentChName, false);
                    LogCommentInfo(CL, "Current Channel Number is : " + currentChName);
                   
                    //Send Key
                    CL.EA.UI.Utils.SendIR(keytobesent, logTimetoSendKey);
                    
                    //this variable is used to obtain channel number after key is pressed
                    string obtainedChName = "";
                    CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedChName, false);
                    LogCommentInfo(CL, "Obtained Channel Number after Key Press is : " + obtainedChName);
                  
                   // Check for Key Miss and Retry Mechanism
                    if (currentChName == obtainedChName)
                    {
                        for (int k = 1; k <= 3; k++)
                        {
                            CL.EA.UI.Utils.SendIR(keytobesent, logTimetoSendKey);
                            CL.EA.UI.Utils.GetEpgInfo("chNum", ref obtainedChName, false);
                            if (obtainedChName != currentChName)
                                break;
                            else
                                FailStep(CL, "After 3 retries STB is not taking keys check if STB got hanged");
                        }
                    }
                    //obtainedChName value is used to search in list whether services are present or not

                    Service service = allServicesList.Find(item => item.LCN == obtainedChName);
                    if (service == null)
                    {
                        LogCommentInfo(CL, "Service is not present in list" + service);
                        CL.EA.UI.Utils.EndWaitForDebugMessages(milestoneToCheck, ref Actualline);
                        LogCommentInfo(CL, "Negative Check Passed for Dummy Services");
                        continue;
                    }

                    LogCommentImportant(CL, "Checking for milestones fetched from test ini");
                    servicesverify = CL.EA.UI.Utils.EndWaitForDebugMessages(milestoneToCheck, ref Actualline);
                    if (servicesverify == false)
                    {
                        FailStep(CL, "Failed to get EndWaitForMessage in Milestones to check");
                    }
                }
            }
       
            PassStep();
        }

    }
    #endregion

    #endregion


}