using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Globalization;

public class FullSanity_0502 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    //Channels used by the test
    private static Service Service_1;


    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Preconditions: Get Values from ini File & Verify Frequency parameters and Do a Manual Recording from Planner");
        this.AddStep(new Step1(), "Step 1: Verify the Event in Archive");
        this.AddStep(new Step2(), "Step 2: Verify Different event information in the Archive");


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

            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Service fetched from content xml " + Service_1.LCN);
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+Service_1.LCN);
            }
            CL.EA.UI.ManualRecording.Navigate();
            CL.EA.UI.ManualRecording.NavigateToFrequency();

            string[] arrFrequency = { "ONE TIME", "DAILY", "WEEKLY", "WEEKDAY", "WEEKEND" };
            foreach (string frequency in arrFrequency)
            {
                LogCommentImportant(CL,"Trying to Highlight the Frequency "+frequency);

                   res= CL.IEX.MilestonesEPG.SelectMenuItem(frequency);
                   if (!res.CommandSucceeded)
                   {
                       FailStep(CL,res,"Failed to highlight on "+frequency);
                   }

                LogCommentImportant(CL, "Successfully highlighted on the Frequency " + frequency);
            }

            res = CL.EA.PVR.RecordManualFromPlanner("EVENT_BASED", Convert.ToInt32(Service_1.LCN), DaysDelay: -1, MinutesDelayUntilBegining: 3, DurationInMin: 10, Frequency: EnumFrequency.ONE_TIME, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to record Manual from Planner");
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
            res = CL.EA.WaitUntilEventStarts("EVENT_BASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until event starts");
            }
            res = CL.EA.PVR.VerifyEventInArchive("EVENT_BASED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to find the event in Archive");
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
            //Verifying the Event name of the Time based recording which should be same as the Service name 
            string obtainedEventName = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedEventName);
            if (!res.CommandSucceeded)
            {
              FailStep(CL, res, "Failed to get the info from EPG");
            } 
            if(obtainedEventName!=Service_1.Name)
            {
                FailStep(CL,"Failed to verify that the Obtained event name"+obtainedEventName+" is different from the expected "+Service_1.Name);
            }
            LogCommentInfo(CL,"Verified that the event name is "+obtainedEventName+" is same as expected "+Service_1.Name);
            //Verifying the Event date
            string obtainedEventDate = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("description", out obtainedEventDate);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the info from EPG");
            }
            string expectedEPGDate = CL.EA.GetEventInfo("EVENT_BASED", EnumEventInfo.EventDate);
            LogCommentInfo(CL, "Expected Date fetched from the event collection is " + expectedEPGDate);
            string eventDictionaryDateFormat = CL.EA.UI.Utils.GetValueFromProject("EPG", "DATE_FORMAT_FOR_EVENT_DIC");
            string EPGDateFormat = CL.EA.UI.Utils.GetEpgDateFormat();
            string EPGEventdate = DateTime.ParseExact(expectedEPGDate, eventDictionaryDateFormat, CultureInfo.InvariantCulture).ToString(EPGDateFormat);
            LogCommentImportant(CL, "Event Date after Parsing is" + EPGEventdate);
            if (obtainedEventDate.Contains(EPGEventdate))
            {
                LogCommentInfo(CL, "Verified that the evtdetails " + obtainedEventDate + " contains EPG date " + EPGEventdate);
            }
            else
            {
                FailStep(CL, "Failed to verify that the evtdetails " + obtainedEventDate + " contains EPG date " + EPGEventdate);
            }
            //Verifying the Start Time
            string obtainedEventTime = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evttime", out obtainedEventTime);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the info from EPG");
            }
            string obtainedStartTime="";
            CL.EA.UI.Utils.ParseEventTime(ref obtainedStartTime, TimeString: obtainedEventTime, IsStartTime: true);
            if (obtainedStartTime == "")
            {
                FailStep(CL,"Failed to Parse the Event Start time");
            }
            string expectedStartTime = CL.EA.GetEventInfo("EVENT_BASED", EnumEventInfo.EventStartTime);
            LogCommentInfo(CL,"Expected Start time fetched from the event collection is "+expectedStartTime);
            if (obtainedEventDate.Contains(expectedStartTime))
            {
			       LogCommentImportant(CL, "Verified that the Description " + obtainedEventDate + " Contains " + expectedStartTime);  
            }
			else
            {
                  FailStep(CL, "Failed to verify that the Description " + obtainedEventDate + " Contains " + expectedStartTime);
			}
             //Verifying the Keep status
            string obtainedKeepstatus = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("keepIcon", out obtainedKeepstatus);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the info from EPG");
            }
            if (obtainedKeepstatus.ToUpper() != "NOT PRESENT")
            {
                FailStep(CL, "Failed to verify the obtained KeepStatus " + obtainedKeepstatus + " is same as expctected NOT PRESENT");
            }
            else
            {
                LogCommentImportant(CL, "Verified the obtained KeepStatus " + obtainedKeepstatus + " is same as expctected NOT PRESENT");
            }
            string timeStamp = "";
            res = CL.IEX.SendIRCommand("SELECT", timeToPress: -1, timestamp: ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to send IR SELECT");
            }
            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.Navigate("INFO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to INFO on Action Bar");
            }
            CL.IEX.Wait(5);
            string obtainedFrequency = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("bookfreq", out obtainedFrequency);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the info from EPG");
            }
            if (obtainedFrequency.ToUpper() != "ONCE")
            {
                FailStep(CL, "Failed to verify the obtained Frequency " + obtainedFrequency + " is same as expctected ONCE");
            }
            else
            {
                LogCommentImportant(CL, "Verified the obtained Frequency " + obtainedFrequency + " is same as expctected ONCE");
            }
            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to delete all records from Archive");
        }
    }
    #endregion
}