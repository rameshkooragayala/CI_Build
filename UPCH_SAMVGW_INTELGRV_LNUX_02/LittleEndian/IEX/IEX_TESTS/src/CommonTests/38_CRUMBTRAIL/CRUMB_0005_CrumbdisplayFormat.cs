/// <summary>
///  Script Name : CRUMB_0005_CrumbdisplayFormat
///  Test Name   : EPG_Crumb display Format
///  TEST ID     : 71346
///  QC Version  : 1
///  Variations from QC: No UI validation and dictionary-key values from Dictionary file are not validated.
///   QC Repository :STB_DIVISION-Unified_ATP_For_HMD_Cable-WP2429 Crumbtrail
/// ----------------------------------------------- 
///  Scripted by : Ponraman Vijayakumar
///  Last modified :09/10/2013
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;
/**********************************************************************************************/
/**
* @class   CRUMB_0005_CrumbdisplayFormat
*
* @brief   A crumb 0005 crumbdisplay format.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
[Test("CRUMB_0005")]
public class CRUMB_0005 : _Test
{
    /**********************************************************************************************/
    /**
* @brief   Go to Audio language Screen and check the CRUMBTITLE &amp; Crumbtext  format.
**************************************************************************************************/
    [ThreadStatic]
    static _Platform CL;
    /**********************************************************************************************/
    /**
* @brief   The audio video service.
**************************************************************************************************/
    private static Service audioVideoService;
    /**********************************************************************************************/
    /**
* @brief   The crumbon audiolanguage.
**************************************************************************************************/
    private static string crumbonAudiolanguage;
    static string dictionaryKey;
    /**********************************************************************************************/
    /**
* @brief   The Crumbtextdisplayin audio language.
**************************************************************************************************/
    private static string CrumbtextonAudioLangScreen;
    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers,Crumbtitle & crumbtilte value From ini File";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/
    private const string STEP1_DESCRIPTION = "Step 1:Enter and highlight the submenu of any tab where the crumb is displayed.";

    #region Create Structure
    /**********************************************************************************************/
    /**
* @fn  public override void CreateStructure()
*
* @brief   Creates the structure.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
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
    /**********************************************************************************************/
    /**
* @fn  public override void PreExecute()
*
* @brief   Pre execute.
*
* @author  Ponramanv
* @date    10/17/2013
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
* @brief   A pre condition.
*
* @author  Ponramanv
* @date    10/17/2013
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
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            LogCommentImportant(CL, "Warning:No UI validation and dictionary-key values from Dictionary file are not validated");
           
            //Get Values From Content XML File
           
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (audioVideoService == null)
            {
                FailStep(CL, "Unable to fetch service from content.xml for passed parameter");
            }
           
            //Get value for Dictionary key from test ini
            dictionaryKey = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DICTIONARY");
            if (dictionaryKey == null)
            {
                FailStep(CL, "Proper Dictionary value is not defined in test ini");
            }

            //Fetch proper Dictionary key value from dictionary

            CrumbtextonAudioLangScreen = CL.EA.UI.Utils.GetValueFromDictionary(dictionaryKey);
            if (CrumbtextonAudioLangScreen == null)
            {
                FailStep(CL, "Failed to fetch value from dictionary");
            }

            LogCommentInfo(CL, "Fetched value for dictionaty key passed :- " + dictionaryKey + "is :- " + CrumbtextonAudioLangScreen);
            
            PassStep();
        }
    }
    #endregion
    #region Step1
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Enter and highlight the submenu of any tab where the crumb is displayed.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }
            //Navigate To Audio language Setting Screen

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:AUDIO LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to AudioLanguage");
            }
            //Retreving CRUMBTITLE info in Audio Language Setting screen
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTEXT", out crumbonAudiolanguage);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get CRUMBTITLE in Audio Language Screen");
            }
            if (crumbonAudiolanguage.Contains(CrumbtextonAudioLangScreen))
            {
                LogCommentInfo(CL, "CRUMBTEXT is displayed in Audio language Setting screen " + "Expected:" + CrumbtextonAudioLangScreen + "Actual:" + crumbonAudiolanguage);
            }
            else
            {
                FailStep(CL, "CRUMBTEXT is not displayed in Audio Language Screen" + "Expected:" + CrumbtextonAudioLangScreen + "Actual:" + crumbonAudiolanguage);
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
* @brief   Posts the execute.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
    [PostExecute()]
    public override void PostExecute()
    {
    }
    #endregion
}