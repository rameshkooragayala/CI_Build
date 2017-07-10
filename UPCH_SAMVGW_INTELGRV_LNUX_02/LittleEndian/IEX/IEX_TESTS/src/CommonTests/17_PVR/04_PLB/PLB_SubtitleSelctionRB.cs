/// <summary>
///  Script Name : PLB_SubtitleSelctionRB.cs
///  Test Name   : PLB-0360-SubtitleSelectionDvb-RB,PLB-0361-SubtitleSelectionTeletext-RB
///  TEST ID     : 68956
///  Test Repository : FR_Fusion/UPC
///  QC Version  : 2
///  Variations from QC:Subtitle Sync with audio cannot be verified
/// ----------------------------------------------- 
///  Modified by : 
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

/**********************************************************************************************//**
 * @class   PLB_SubtitleSelctionRB.
 *
 * @brief   PLB_SubtitleSelctionRB.
 *
 * @author  Madhur
 *          
 * @date    10/4/2013
 **************************************************************************************************/

[Test("PLB_SubtitleSelctionRB")]
public class PLB_SubtitleSelctionRB : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The service.
     **************************************************************************************************/
    
    private static Service service;

    /**********************************************************************************************//**
     * @brief   The no ofavailable subtile languages.
     **************************************************************************************************/

    private static int NoOfavailableSubtileLanguages = 0;

    /**********************************************************************************************//**
     * @brief   The next subtitle.
     **************************************************************************************************/

    private static string nextSubtitle = "";

    /**********************************************************************************************//**
     * @brief   The default subtitle.
     **************************************************************************************************/

    private static string defaultSubtitle = "";

    /**********************************************************************************************//**
     * @brief   The expected subtitle to change to.
     **************************************************************************************************/

    private static string expectedSubtitleToChangeTo = "";

    /**********************************************************************************************//**
     * @brief   The obtained subtitle to change to.
     **************************************************************************************************/

    private static string obtainedSubtitleToChangeTo = "";

    private static Dictionary<EnumEpgKeys, String> dictionary = new Dictionary<EnumEpgKeys, String>();
    /**********************************************************************************************//**
     * @brief   Information describing the precondition.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From xml File";

    /**********************************************************************************************//**
     * @brief   Information describing the step 1.Fill RB and Playback from RB
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1: Fill RB and Playback from RB";

    /**********************************************************************************************//**
     * @brief   Information describing the step 2.Check expected subtitle streams are displayed
     **************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2: Check expected subtitle streams are displayed (no subtile type missing)";

    /**********************************************************************************************//**
     * @brief   Information describing the step 3.Select another subtitle track than the one currently presented
     **************************************************************************************************/

    private const string STEP3_DESCRIPTION = "Step 3: Select another subtitle track than the one currently present";

    private static int trickmodebar_timeout = 0;
    /**********************************************************************************************//**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    private static class Constants
    {
        /**********************************************************************************************//**
         * @brief   The timeout.
         **************************************************************************************************/

        public const int timeout = 30;

        /**********************************************************************************************//**
         * @brief   The pause.
         **************************************************************************************************/

        public const int pause = 0;

        /**********************************************************************************************//**
         * @brief   The play.
         **************************************************************************************************/

        public const int play = 1;

        /**********************************************************************************************//**
         * @brief   Depth of the rb initial.
         **************************************************************************************************/

        public const int rbInitialDepth = 20;

        /**********************************************************************************************//**
         * @brief   In miliseconds.
         **************************************************************************************************/

        public const int waitAfterSendingCommand = 1000;

    }


    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);


        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute

    /**********************************************************************************************//**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Gets the required service from xml.
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhur
         * @date    10/4/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Values From ini File and service from xml file.
            string serviceType = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_TYPE");
            
			trickmodebar_timeout = Convert.ToInt32(CL.EA.UI.Utils.GetValueFromProject("CHANNEL_BAR_TIMEOUT","MAX"));
			
		   // string serviceType = "ParentalRating=High;NoOfSubtitleLanguages=0,1;SubtitleType=Dvb";
            service = CL.EA.GetServiceFromContentXML("Type=Video", serviceType);
            if (service == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + service.LCN);
            }


            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Step 1 : Fill RB and Playback from RB.
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhur
         * @date    10/4/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Tune to the service to fill RB
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, service.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + service.LCN);
            }
            //Pause to fill RB 
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.pause, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Pause From Live");
            }

           //Wait to fill RB

            CL.IEX.Wait(Constants.rbInitialDepth);
            LogComment(CL, "Waiting to fill RB");

            //Play from RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.play, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Play From RB");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   Step 2 : Check expected subtitle streams are displayed (no subtile type missing).
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhur
         * @date    10/4/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            
            CL.IEX.Wait(trickmodebar_timeout);

            //Get the list of subtitles on the service
            var optionList = service.SubtitleLanguage;

            //Navigate to subtitles from RB
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AV SETTINGS SUBTITLES");
            }

            //Get the curreent selction
            string currSel = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currSel);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to get the current selection");
            }

            //fetch subtitle navigation from project ini
            nextSubtitle = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "NEXT_SUBTITLE");
            LogCommentInfo(CL, "Next Subtitle feteched from project ini is : " + nextSubtitle);

            NoOfavailableSubtileLanguages = Convert.ToInt32(service.NoOfSubtitleLanguages);

            for (int match = 0; match < NoOfavailableSubtileLanguages; match++)
            {
                if (optionList.Contains(currSel))
                {
                    LogCommentInfo(CL, "option" + match + ":" + currSel + "is present in the list");
                }
                else
                {
                    FailStep(CL, res, "option" + match + ":" + currSel + "is not present in the list");
                }

                //clear EPG info
                res = CL.IEX.MilestonesEPG.ClearEPGInfo();
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to clear epg info", false);
                }

                //Navigate to next option in the list
                string timeStamp = "";
                res = CL.IEX.IR.SendIR(nextSubtitle, out timeStamp, Constants.waitAfterSendingCommand);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate to next subtitle in the list");
                }
                res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out currSel);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get the current selection");
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step3

    /**********************************************************************************************//**
     * @class   Step3
     *
     * @brief   Step 3 : Select another subtitle track than the one currently present and verify the subtitle is changed.
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    [Step(3, STEP3_DESCRIPTION)]
    private class Step3 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Madhur
         * @date    10/4/2013
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Change from default subtitle to different subtitle on RB from action menu;
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:LIVE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Subtitle on action menu ");
            }
            CL.IEX.Wait(trickmodebar_timeout);

            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AV SETTINGS SUBTITLES");
            }
            //fetch default subtitle
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out defaultSubtitle);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Current Subtitle Track Name");
            }

            LogCommentInfo(CL, "The default Subtitle is: " + defaultSubtitle);

            //Change to any other subtitle
            string timeStamp = "";
            res = CL.IEX.IR.SendIR(nextSubtitle, out timeStamp, Constants.waitAfterSendingCommand);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to next Subtitle in the list");
            }

            //Get destination subtitle
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out expectedSubtitleToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Audio Track Name" + expectedSubtitleToChangeTo);
            }

            //Select the subtitle
            //dictionary.Add(EnumEpgKeys.TITLE, expectedSubtitleToChangeTo);
            //res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to Navigate to AV SETTINGS SUBTITLES");
            //}
            CL.EA.UI.Utils.SendIR("SELECT");
            LogCommentInfo(CL, "expectedSubtitleToChangeTo: " + expectedSubtitleToChangeTo);

            //Verify for the changed subtitle
            dictionary.Clear();
            res = CL.EA.NavigateAndHighlight("STATE:AV SETTING SUBTITLES", dictionary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AV SETTINGS SUBTITLES");
            }

            //Get destination subtitle
            res = CL.IEX.MilestonesEPG.GetEPGInfo("TITLE", out obtainedSubtitleToChangeTo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Next Audio Track Name" + obtainedSubtitleToChangeTo);
            }
            LogCommentInfo(CL, "obtainedSubtitleToChangeTo: " + obtainedSubtitleToChangeTo);

            if (obtainedSubtitleToChangeTo.Equals(expectedSubtitleToChangeTo))
            {
                LogCommentInfo(CL, "Subtitle changed to another than default");
            }
            else
            {
                FailStep(CL, res, "Subtitle not changed to expected subtitle");
            }
            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute

    /**********************************************************************************************//**
     * @fn  public override void PostExecute()
     *
     * @brief   Flushes the filled RB .
     *
     * @author  Madhur
     * @date    10/4/2013
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        //Flush RB ans reset to default subtitle
        // 
        res = CL.EA.StandBy(false);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Unable to move to stand by");
        }

         res = CL.EA.StandBy(true);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Unable to come out of stand by");
        }

    #endregion
    }
}