/// <summary>
///  Script Name : SET-SUBT-0017-Live-different subtitle language.cs
///  Test Name   : SET-SUBT-0017-Live-different subtitle language,SET-SUBT-0004-Live-subtitle language default,SET-SUBT-0007- Live-Reset subtitle -current profile
///  TEST ID     : 34629
///  QC Version  : 2
///  Variations from QC: Validating only 5 subtitle languages instead of all the languages as its taking very long time to validate all the languages.
/// ----------------------------------------------- 
///  Modified by : Scripted by : Madhu Kumar K
///  Last modified : 10th Feb, 2014
/// </summary>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************/
/**
* @class  SUBT_0017 
*
* @brief   Setting of the different subtitle languages
*
* @author  Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

[Test("SUBT_DifferentLanguages")]
public class SUBT_DifferentLanguages : _Test
{
    /**********************************************************************************************/
    /**
* @brief   The cl.
**************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************/
    /**
* @brief   The Subtitle service.
**************************************************************************************************/

    private static Service subtitleService;

    /**********************************************************************************************/
    /**
* @brief   The service.
**************************************************************************************************/

    private static Service service;

    /**********************************************************************************************/
    /**
* @brief   Helper Instance
**************************************************************************************************/

    static Helper helper = new Helper();

    /**********************************************************************************************/
    /**
* @brief   The Key for Selection from project ini file.
**************************************************************************************************/

    private static string selectSubtitle = "";

    /**********************************************************************************************/
    /**
  * @brief   The key for the next subtitle from Project ini file.
     **************************************************************************************************/

    private static string nextSubtitle = "";

    /**********************************************************************************************/
    /**
  * @brief   Default Subtitle display from project ini file.
     **************************************************************************************************/

    private static string defaultSubtitleDisplay = "";

    /**********************************************************************************************/
    /**
/**
* @brief   Obtained Subtitle from Action bar Subtitle
**************************************************************************************************/

    private static string obtainedSubtitleToChangeTo = "";

    /**********************************************************************************************/
    /**
* @brief   Obtained Subtitle from Main Menu Subtitle Language
**************************************************************************************************/

    private static string expectedSubtitleToChangeTo = "";
    
    /**********************************************************************************************/
    /**
* @brief   First Subtitle Language -- Default Subtite Language
**************************************************************************************************/

    private static string FirstSubtitleLanguage = "";

    /**********************************************************************************************/
    /**
* @brief   Next Subtitle Language
**************************************************************************************************/

    private static string NextSubtitleLanguage = "";

    /**********************************************************************************************/
    /**
* @brief   Next Subtitle Language
**************************************************************************************************/

    private static int VerifiedSubtitleCount = 0;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File and set the Subtitle Display to ON";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Main Menu Subtitle Language";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Fetch the Default subtitle language";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 3.
**************************************************************************************************/

    private const string STEP3_DESCRIPTION = "Step 3: Navigate to Action Bar Subtitles and verify the default Subtitle";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 4.
**************************************************************************************************/

    private const string STEP4_DESCRIPTION = "Step 4: Perform the same action for all the Subtitle Languages";

    /**********************************************************************************************/
    /**
  

* @class   Constants
*
* @brief   Constants.
*
* @author Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

    private static class Constants
    {
        /**********************************************************************************************/
        /**
* @brief   Wait after Sending Command in milliseconds.
**************************************************************************************************/

        public const int waitAfterSendingCommand = 1000;

        /**********************************************************************************************/

    }


    #region Create Structure

    /**********************************************************************************************/
    /**
* @fn  public override void CreateStructure()
*
* @brief   Creates the structure.
*
* @author Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);


        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute

    /**********************************************************************************************/
    /**
* @fn  public override void PreExecute()
*
* @brief  Pre execute
*
* @author Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************/
    /**
* @class   PreCondition
*
* @brief   Gets the required service from xml and sets the Subtitle Display to ON.
* @author  Madhu Kumar K
* @date   10 OCT,13
**************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author Madhu Kumar K
* @date   10 OCT,13
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Get Values From xml File
            subtitleService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;NoOfSubtitleLanguages=0,1,2");
            if (subtitleService == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + subtitleService.LCN);
            }
            var optionlist = subtitleService.SubtitleLanguage;
            if (optionlist.Count == 0)
            {
                FailStep(CL, "There are no subtitle languages assigned for this service in xml file");

            }

            service = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + subtitleService.LCN);
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE ON");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Subtitle Display to ON");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   Step 1 : Navigate to Main Menu Subtitle Language
*
* @author Madhu Kumar K
* @date   10 OCT,13
**************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief  Executes this object.
*
* @author Madhu Kumar K
* @date   10 OCT,13
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            res = CL.EA.TuneToChannel(service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Tune to channel:" + service.LCN,false);
            }

            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Main Menu Subtitle Language menu");
            }
         

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************/
    /**
* @class   Step2
*
* @brief   Step 2 : Fetch the Default Subtitle Language
*
* @author  Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief  Executes this object.
*
* @author  Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSubtitleToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to fetch the value title from Subtitle Language Menu");
            }
           
            //fetch next subtitle key from project ini
            nextSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_SUBTITLE");

            LogCommentInfo(CL, "Key For Next subtitle fetched from Project ini : " + nextSubtitle);

            //Fetch Subtitle Select Key from Project ini file
            selectSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "SELECT_KEY");

            LogCommentInfo(CL, "Select Key fetched from Project ini : " + selectSubtitle);

            FirstSubtitleLanguage = expectedSubtitleToChangeTo;
            LogCommentImportant(CL, "First Subtitle Language from Main Menu: " + expectedSubtitleToChangeTo);
            PassStep();
        }
    }
    #endregion
    #region Step3

    /**********************************************************************************************/
    /**
* @class   Step3
*
* @brief   Step 3 : Navigate to Action Bar Subtitles and verify the default Subtitle
*
* @author  Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author Madhu Kumar K
* @date   10 OCT,13
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            if (!helper.VerifySubtitleLanguage())
            {
                FailStep(CL, "Failed to verify the obtained Subtitle Language from Action Bar is same as the Expected Subtitle Language");
            } 

            PassStep();
        }
    }
    #endregion
    #region Step4

    /**********************************************************************************************/
    /**
* @class   Step4
*
* @brief   Step 4 : Perform the same action for all the Subtitle Languages
*
* @author  Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

    [Step(4, STEP4_DESCRIPTION)]
    private class Step4 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author Madhu Kumar K
* @date   10 OCT,13
**************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //verifying only 5 subtitle languages as verying all the languages is consuming lot of time.
            while (FirstSubtitleLanguage != NextSubtitleLanguage&&VerifiedSubtitleCount<5)
            {
                VerifiedSubtitleCount++;
                res = CL.EA.TuneToChannel(service.LCN);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to Tune to channel:" + service.LCN, false);
                }

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Navigate to Main Menu Subtitle Language menu");
                }

                res = CL.IEX.MilestonesEPG.ClearEPGInfo();

                //Navigate to next option in the list

                string timeStamp = "";

                res = CL.IEX.IR.SendIR(nextSubtitle, out timeStamp, Constants.waitAfterSendingCommand);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to next subtitle in the list");
                }


                //Navigate to next option in the list
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out expectedSubtitleToChangeTo);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to fetch the value title from Main Menu Subtitles Language");
                }


                //fetch subtitle navigation from project ini

                string time_Stamp = "";

                res = CL.IEX.IR.SendIR(selectSubtitle, out time_Stamp, Constants.waitAfterSendingCommand);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to select the Language from the list");
                }

                NextSubtitleLanguage = expectedSubtitleToChangeTo;
                LogCommentImportant(CL, "Fetched Subtitle Language from Main Menu: " + expectedSubtitleToChangeTo);

                if (!helper.VerifySubtitleLanguage())
                {
                    FailStep(CL,"Failed to verify the obtained Subtitle Language from Action Bar is same as the Expected Subtitle Language");
                }
                
                
            }
            PassStep();
        }

    }
    #endregion

    #endregion

    #region PostExecute

    /**********************************************************************************************/
    /**
* @fn  public override void PostExecute()
*
* @brief   Executes this object.
*
* @author  Madhu Kumar K
* @date    10 OCT,13
**************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Setting the Default Subtitle Language
        if (FirstSubtitleLanguage != expectedSubtitleToChangeTo)
        {
            LogComment(CL,"Default subtitle "+FirstSubtitleLanguage+" is different from Current Subtitle "+expectedSubtitleToChangeTo);

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SETTINGS SUBTITLE");
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to Navigate to Main Menu Subtitle Language Menu");
            }
            res = CL.IEX.MilestonesEPG.Navigate(FirstSubtitleLanguage);
            if (!res.CommandSucceeded)
            {
                CL.IEX.FailStep("Failed to Set the Subtitle Language toDefault");
            }
        }
        //Set the Subtitle display to the default

        //fetch default subtitle from project ini
        defaultSubtitleDisplay = CL.EA.GetValueFromINI(EnumINIFile.Project, "SUBTITLE", "DEFAULT");

        LogCommentInfo(CL, "Default Subtitled fetched from ini file : " + defaultSubtitleDisplay);

        if (defaultSubtitleDisplay.ToUpper() == "OFF"||defaultSubtitleDisplay.ToUpper()=="DISABLED")
        {
                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUBTITLE DISPLAY OFF");
                if (!res.CommandSucceeded)
                {
                    CL.IEX.FailStep("Failed to Set Subtitle Display to OFF");
                }
        }
    }
    #endregion

    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Tunes to Channel with subtitles and verifies the defualt subtitle language in Action bar.
        /// </summary>
        /// <returns>bool</returns>
        public bool VerifySubtitleLanguage()
        {
            IEXGateway._IEXResult res;
            bool isPass = true;
            res = CL.EA.TuneToChannel(subtitleService.LCN);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Tune to Channel - " + subtitleService.LCN);
                isPass = false;
            }
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AV SETTING SUBTITLES");
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to Navigate to Action bar Subtitles menu");
                isPass = false;
            }

            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedSubtitleToChangeTo);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to fetch the value title from Action Bar");
                isPass = false;
            }

            LogCommentImportant(CL, "Fecthed Subtitle Language from Action bar Settings:" + obtainedSubtitleToChangeTo);
            if (expectedSubtitleToChangeTo != obtainedSubtitleToChangeTo)
            {
                var optionlist = subtitleService.SubtitleLanguage;
                if (optionlist.Contains(expectedSubtitleToChangeTo))
                {
                    LogCommentFailure(CL, "Subtitle Present in Action Bar :" + obtainedSubtitleToChangeTo + " is different from the value which is set in Subtitle Language :" + expectedSubtitleToChangeTo);
                    isPass = false;
                }
                else
                {
                    LogComment(CL, "Selected Main Menu Subtitle language " + expectedSubtitleToChangeTo + " is not present in this particular service");
                }
            }
            else
            {
                LogComment(CL, "Found the subtitle language " + expectedSubtitleToChangeTo + " in Action Bar");
            }

            return isPass;
        }
    }
    #endregion
}