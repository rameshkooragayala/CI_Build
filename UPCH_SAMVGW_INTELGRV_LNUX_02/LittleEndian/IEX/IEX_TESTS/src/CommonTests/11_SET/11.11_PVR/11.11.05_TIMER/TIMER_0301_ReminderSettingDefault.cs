/// <summary>
///  Script Name : TIMER_0301_ReminderSettingDefault
///  Test Name   : TIMER-0301-ReminderSettingDefault
///  TEST ID     : 68251
///  JIRA ID     : FC-652
///  QC Version  : 2
///  Variations from QC:None
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

[Test("TIMER_0301")]
public class TIMER_0301 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Shared members between steps

    static Service videoService;
    static string expectedReminderNotification = "";

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";
    private const string STEP1_DESCRIPTION = "Step 1: Verify for Default Reminder Notification from settings is set to default.";



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


            //Get Values From XML File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;EventDuration=10", "ParentalRating=High");
            if (videoService == (null))
            {
                FailStep(CL, "Failed to retrieve Service1 for Reminder from content xml");
            }

            //Fetch the default reminder notification 
            expectedReminderNotification = CL.EA.GetValueFromINI(EnumINIFile.Project, "REMINDER NOTIFICATION", "DEFAULT");
            if (String.IsNullOrEmpty(expectedReminderNotification))
            {
                FailStep(CL, res, "DEFAULT value for reminder notification is not present in the Project.ini");
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

            //Navigate to Reminders in settings
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:REMINDER NOTIFICATION");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to REMINDER NOTIFICATION!");
            }

            string obtainedReminderNotification = "";

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedReminderNotification);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get obtainedReminderNotification");
            }


            //Verify the default value for reminder notification is ON
            if (obtainedReminderNotification.Equals(expectedReminderNotification))
            {
                LogCommentInfo(CL, "Default Reminder Notification is set to default");
            }
            else
            {
                FailStep(CL, res, "Default Reminder Notification is not set to default");
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