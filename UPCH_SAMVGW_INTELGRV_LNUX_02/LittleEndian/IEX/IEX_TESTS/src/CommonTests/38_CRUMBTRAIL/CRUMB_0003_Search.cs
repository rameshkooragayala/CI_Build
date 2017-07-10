/// <summary>
///  Script Name : CRUMB_0003_Search
///  Test Name   : EPG_Crumb display_Search
///  TEST ID     : 71339
///  QC Version  : 1
///  Variations from QC: No UI validation and dictionary-key values from Dictionary file are not validated.
///   QC Repository : STB_DIVISION-Unified_ATP_For_HMD_Cable-WP2429 Crumbtrail
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
* @class   CRUMB_0003_Search
*
* @brief   Checking Crumbtitle in Search Screen.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
[Test("CRUMB_0003")]
public class CRUMB_0003 : _Test
{
    /**********************************************************************************************/
    /**
* @brief   Navigating to Search Screen and checking Crumbtitle.
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
* @brief   The crumbon search.
**************************************************************************************************/
    private static string crumbonSearch;
    /**********************************************************************************************/
    /**
* @brief   The crumbonkeyword.
**************************************************************************************************/
    private static string crumbonkeyword;
    /**********************************************************************************************/
    /**
* @brief   The crumbonon advance search.
**************************************************************************************************/
    private static string crumbononAdvSearch;
    /**********************************************************************************************/
    /**
* @brief   The CrumbtitleSearch.
**************************************************************************************************/
    private static string CrumbtitleSearch;
    /**********************************************************************************************/
    /**
* @brief   The CrumbtextKeyword.
**************************************************************************************************/
    private static string CrumbtextKeyword;

    /**********************************************************************************************/
    /**
  * @brief   The CrumbtextKeyword.
**************************************************************************************************/
    private static string CrumbtextAdvanced;

    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers,Crumbtitle & crumbtilte value From ini File";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/
    private const string STEP1_DESCRIPTION = "Step 1: Enter Menu->search ";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/
    private const string STEP2_DESCRIPTION = "Step 2:Enter keyword search ";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 3.
**************************************************************************************************/
    private const string STEP3_DESCRIPTION = "Step 3:Enter Advanced search ";
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
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
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
* @brief   A pre condition.Navigating to SEARCH Screen.
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
* @brief   Retreving channel detail from XML file .
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            LogCommentWarning(CL, "Warning:No UI validation and dictionary-key values from Dictionary file are not validated");
            //Get Values From Content XML File
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            //Get Dictionary key values  from dictionary file 
            CrumbtitleSearch = CL.EA.UI.Utils.GetValueFromDictionary("DIC_MAIN_MENU_SEARCH");
            CrumbtextKeyword = CL.EA.UI.Utils.GetValueFromDictionary("DIC_MAIN_MENU_KEYWORD");
            CrumbtextAdvanced = CL.EA.UI.Utils.GetValueFromDictionary("DIC_MAIN_MENU_ADVANCED");
            PassStep();
        }
    }
    #endregion
    #region Step1
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.Navigate Search Screen and check the Crumbtitle
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.
*
* @author  Ponraman V
* @date    10/23/2013
**************************************************************************************************/
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Navigate Search Screen and check the Crumbtitle.
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
            //Navigate To Search Screen
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SEARCH");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Search Screen");
            }
            //Retreving CRUMBTITLE info in Search Screen
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE", out crumbonSearch);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get CRUMBTITLE in Search Screen");
            }
            if (crumbonSearch.Equals(CrumbtitleSearch))
            {
                LogCommentInfo(CL, "CRUMBTITLE is displayed in Search screen " + "Expected:" + CrumbtitleSearch + "Actual: " + crumbonSearch);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is not displayed in Search Screen" + "Expected:" + CrumbtitleSearch + "Actual: " + crumbonSearch);
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
* @brief   Navigate keyword Screen and check the Crumbtext
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @class   Step2
*
* @brief   A step 2.
*
* @author  Ponraman V
* @date    10/23/2013
**************************************************************************************************/
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
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
            //Navigate To Keywrod Search

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SEARCH");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Search Screen");
            }
            //Retreving CRUMBTITLE info in Keyword search Screen
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTEXT", out crumbonkeyword);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get CRUMBTITLE in Keyword Search");
            }
            if (crumbonkeyword.Equals(CrumbtextKeyword))
            {
                LogCommentInfo(CL, "CRUMBTEXT is displayed in keyword Search screen " + "Expected:" + CrumbtextKeyword + "Actual:" + crumbonkeyword);
            }
            else
            {
                FailStep(CL, "CRUMBTEXT is not displayed in keyword Search Screen" + "Expected:" + CrumbtextKeyword + "Actual:" + crumbonkeyword);
            }
            PassStep();
        }
    }
    #endregion
    #region Step3
    /**********************************************************************************************/
    /**
* @class   Step3
*
* @brief    Navigate Advance Screen and check the Crumbtext
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @class   Step3
*
* @brief   A step 3.
*
* @author  Ponraman V
* @date    10/23/2013
**************************************************************************************************/
    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
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
            //Navigate To Advance Search Screen

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ADV SEARCH");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Advance Search");
            }
            //Retreving CRUMBTITLE info in Advance Search Screen
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTEXT", out crumbononAdvSearch);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get CRUMBTEXT in Advance Search Screen");
            }
            if (crumbononAdvSearch.Equals(CrumbtextAdvanced))
            {
                LogCommentInfo(CL, "Crumbtext is displayed in Advance Search screen " + "Expected:" + CrumbtextAdvanced + "Actual:" + crumbononAdvSearch);
            }
            else
            {
                FailStep(CL, "Crumbtext is not displayed in Advance Search Screen");
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