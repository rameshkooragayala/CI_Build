/// <summary>
///  Script Name : TIMER_0300_ConfigureReminderSetting
///  Test Name   : TIMER-0300-ConfigureReminderSetting
///  TEST ID     : 68250
///  JIRA ID     : FC-644
///  QC Version  : 2
///  Variations from QC:Step 3 tests is repetation of step 1 and thus so not implementd.
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Renukaradhya 
///  Last modified : 4 SEP 2013 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
using FailuresHandler;

[Test("TIMER_0300")]
public class TIMER_0300 : _Test
{
    [ThreadStatic]
    static _Platform CL;

   
    //Shared members between steps
    static Service service1ForFirstReminder;
    static string expectedReminderNotification = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Verify for Reminder Notification when enabled from settings.";
    private const string STEP2_DESCRIPTION = "Step 2: Verify for no Reminder Notification when disabled from settings";
    


    static class Constants
    {
        public const int numberofRightKeyPressinGrid = 1;
        public const int minTimeBeforeEventStart = 3;
     
    }

    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        


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

           
            //Get Values From XML File
            service1ForFirstReminder = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "ParentalRating=High");
            if (service1ForFirstReminder == (null))
            {
                FailStep(CL, "Failed to retrieve Service1 for Reminder from content xml");
            }

            //Fetch the default reminder notification 
            expectedReminderNotification = CL.EA.GetValueFromINI(EnumINIFile.Project, "REMINDER NOTIFICATION", "DEFAULT");
            if (String.IsNullOrEmpty(expectedReminderNotification))
            {
                FailStep(CL, res, "DEFAULT value for reminder notification is not present in the Project.ini");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service1ForFirstReminder.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to AV Service: " + service1ForFirstReminder.LCN);
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

            //From Settings, Enable Reminders.
            res = CL.EA.STBSettings.SetReminderNotifications(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Turn Reminder Notifications On");
            }


            //  Book Reminder on Future Event on S1 
            res = CL.EA.BookReminderFromGuide("Event1", service1ForFirstReminder.LCN, Constants.numberofRightKeyPressinGrid, Constants.minTimeBeforeEventStart);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Reminder from Guide on future event of service 1" + service1ForFirstReminder.LCN);
            }

            //Wait for the Reminder Time of a Reminded Event [Start Time - 60 sec]
            res = CL.EA.WaitUntilReminder("Event1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get reminder notification on Service1");
            }
            else
            {
                LogCommentInfo(CL,"Reminder notification displayed successfully.");
            }


            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //From Settings, Disable Reminders.
            res = CL.EA.STBSettings.SetReminderNotifications(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Turn Reminder Notifications OFF");
            }

            //  Book Reminder on Future Event on S1 
            res = CL.EA.BookReminderFromGuide("Event2", service1ForFirstReminder.LCN, Constants.numberofRightKeyPressinGrid, Constants.minTimeBeforeEventStart);

            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Book Reminder from Guide on future event of service 1" + service1ForFirstReminder.LCN);
            }

            //Wait for the Reminder Time of a Reminded Event [Start Time - 60 sec]
            res = CL.EA.WaitUntilReminder("Event2");
            int failureCode = res.FailureCode;
            if (!res.CommandSucceeded &&(failureCode.Equals(ExitCodes.ReminderFailure.GetHashCode())))
            {
                LogCommentInfo(CL, "Reminder notification is not  displayed,as the Reminder Notification is disabled from settings");
            }
            else
            {
                FailStep(CL, res, "Reminder notification is still displayed,even after the Reminder Notification is disabled from settings.");
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
        IEXGateway._IEXResult res;
        //Navigate to Reminders in settings
        res = CL.IEX.MilestonesEPG.NavigateByName("STATE:REMINDER NOTIFICATION");
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to navigate to default Disk Space Management option ");
        }
        res = CL.IEX.MilestonesEPG.Navigate(expectedReminderNotification);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to set the reminder to default ");
        }
    }
    #endregion
}