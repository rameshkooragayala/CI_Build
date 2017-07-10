/// <summary>
///  Script Name : ADayInLife_Stage_1.cs 
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

[Test("ADayInLife_Stage_1")]
public class ADayInLife_Stage_1 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static string avMilestones;
    static string timeOut;
    static string service_1;
    static string service_2;
    static string testDuration;
    

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Do a channel Surf from GUIDE and Do a Channel Surf P+/P- ";
    

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
           
            avMilestones = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "AV_MILESTONES");
            if (avMilestones == null)
            {
                FailStep(CL, "AV Milestones value is not updated in test ini");
            }
            
            timeOut = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TIMEOUT");
            if (timeOut == null)
            {
                FailStep(CL, "Time Out value is not updated in test ini");
            }

            testDuration = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TEST_DURATION");
            if (testDuration == null)
            {
                FailStep(CL, "Test Duration is not defined in Test Ini");
            }
            CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            //we've to find two continuous av services for this we'll to a channel surf and check for av milestones if it is present take chnum and put it in the list
           // also we'll keep track for number of surf_up/down happened
            
            bool message = true;
            string currentChName = "";
            List<string> list = new List<string>();
           
            while (list.Count!= 2)
            {
                ArrayList Actualline = new ArrayList();
               
                CL.EA.UI.Utils.BeginWaitForDebugMessages(avMilestones, Convert.ToInt32(timeOut));

                //call channel surf 
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, IsNext: false,NumberOfPresses:1);
                if (!res.CommandSucceeded)
                {
                    LogCommentWarning(CL, "Channel Surf has failed");
                }
                message = CL.EA.UI.Utils.EndWaitForDebugMessages(avMilestones, ref Actualline);
                if (message == true)
                {
                    LogCommentInfo(CL, "Expected AV Milestones have arrived,Fetching chnum and adding channel in list");
                    CL.EA.UI.Utils.GetEpgInfo("chnum", ref currentChName, false);
                    list.Add(currentChName);
                    currentChName = "";
                }
                //this logic will always take 2 continous AV service 
                if (list.Count == 2)
                {
                    if((Convert.ToInt32(list[0])+1)!=(Convert.ToInt32(list[1])))
                    {
                        list.RemoveAt(0);   
                    }
                }
 
            }

            //fetch services value and store it in a list
            service_1 = list[0];
            service_2 = list[1];

            LogCommentImportant(CL, "Services are as :" + service_1 + ";" + service_2);
            //this channelsurf  is to start from proper position
         
            CL.EA.ChannelSurf(EnumSurfIn.Live, service_1);

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

            string obtainedChName = "";
            int counter = 1;
            bool next = true;
            
            //fetching current system time
            DateTime dateTime = DateTime.Now;
            LogCommentImportant(CL, "Current Time is :" + dateTime.ToString());

            //adding test duration to be ran
            TimeSpan timespan = new TimeSpan(Convert.ToInt32(testDuration), 0, 0);

            //getting added value upto which test to be executed

            DateTime combinedTime = dateTime.Add(timespan);
            LogCommentImportant(CL, "Combined Time upto which script to be ran :" + combinedTime);

            DateTime now = DateTime.Now;
            int total = 0;
            while (now < combinedTime)
            {
               
                if (counter % 2 == 0)
                    next = false;
                else
                    next = true;


                LogCommentImportant(CL, "Doing a channel surf from GUIDE");
                  //Surf from GUIDE
                    res = CL.EA.ChannelSurf(EnumSurfIn.Guide, IsNext: next, NumberOfPresses: 1,DoTune:true);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, "Tuning from GUIDE has failed");
                    }
                 CL.EA.UI.Utils.GetEpgInfo("chnum", ref obtainedChName, false);
                 LogCommentImportant(CL, "obtained channel number after tuning from guide is :" + obtainedChName);
                
                obtainedChName = "";

                //do a channel surf after tuning from guide is done
                //it is sending up/down and getting back to original position i.e P+/P-
                
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, IsNext: !next, NumberOfPresses: 1);
                CL.EA.UI.Utils.GetEpgInfo("chnum", ref obtainedChName, false);
                obtainedChName = "";

                CL.IEX.Wait(10);

                res = CL.EA.ChannelSurf(EnumSurfIn.Live, IsNext:next, NumberOfPresses: 1);
                CL.EA.UI.Utils.GetEpgInfo("chnum", ref obtainedChName, false);
                obtainedChName = "";
               
                counter++;
                total++;
               
                LogCommentImportant(CL,"running number of iteration is :" + total.ToString());

                CL.IEX.Wait(10);

                now = DateTime.Now;
            }
            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}