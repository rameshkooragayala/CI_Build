/// <summary>
///  Script Name : CRUMB_EPGScreen
///  Test Name   : EPGScreen_Crumb display 
///  TEST ID     : 71347
///  QC Version  : 1
///  Variations from QC: No UI validation and dictionary-key values from Dictionary file are not validated.
///   QC Repository :STB_DIVISION-Unified_ATP_For_HMD_Cable-WP2429 Crumbtrail
/// ----------------------------------------------- 
///  Scripted by : Ponraman Vijayakumar
///  Last modified :09/10/2013 
/// </summary>
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
* @class   CRUMB_EPGScreen
*
* @brief   Check CRUMBTILTE in EPG Screen.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
[Test("CRUMB_EPGScreen")]
public class CRUMB_EPGScreen: _Test
{
    /**********************************************************************************************/
    /**
* @brief   Navigate to Screen and Check CRUMTILTE &amp; CRUMBTEXT.
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
* @brief   The crumtextfromepg.
**************************************************************************************************/
    private static string crumtextfromepg;
    /**********************************************************************************************/
    /**
* @brief   The crumbtextdisplay.
**************************************************************************************************/
    static string numberOfCrumTextVisibility;
    private static string crumbtextdisplay;
    static string firstCrumbTextVisibility;
    static string secondCrumTextVisibility;
    static string expectedState;

    /**********************************************************************************************/
    /**
* @brief   The crumbtextdisplay.
**************************************************************************************************/

    private static string crumbtext;

     /**********************************************************************************************/
    /**
* @brief   The crumbtextdisplay.
**************************************************************************************************/
    private static string EpgScreen;
   
    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers,Crumbtitle & crumbtilte value From ini File";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/
    private const string STEP1_DESCRIPTION = "Step 1:Navigate to EPG Screen and check the Crumtext display ";
    #region Create Structure
    /**********************************************************************************************/
    /**
* @fn  public override void CreateStructure()
*
* @brief   Creates the structure.
*
* @author  Ponraman V
* @date    10/21/2013
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
* @author  Ponraman V
* @date    10/21/2013
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
* @brief   A pre condition.Retreive Channel detail from Content XMl.
*
* @author  Ponraman V
* @date    10/21/2013
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
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            LogCommentImportant(CL, "Warning:No UI validation and dictionary-key values from Dictionary file are not validated");
           
            //Get Values From Content XML File
            
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (audioVideoService == null)
            {
                FailStep(CL, "Failed to fetch proper value for service from content.xml");
            }

            //Fetch value from test ini for crumtext visibility or not

            numberOfCrumTextVisibility = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "VISIBLE_NUMBER");
            if (numberOfCrumTextVisibility == null)
            {
                FailStep(CL, "Failed to fetch value for number of crum visibility screens");
            }

            firstCrumbTextVisibility = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "FIRST_VISIBLE");
            if (firstCrumbTextVisibility == null)
            {
                FailStep(CL, "Failed to fetch value for Crum Text Visibility ");
            }

            secondCrumTextVisibility = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SECOND_VISIBLE");
            if (secondCrumTextVisibility == null)
            {
                LogCommentWarning(CL, "Not defined as per screen requirement");
            }
            //Get Dictionary key values  from dictionary file 
            crumbtext = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DICTIONARYVALUE");
            if (crumbtext == null)
            {
                FailStep(CL, "Failed to fetch value from Test.ini ");
            }
           
            crumbtextdisplay = CL.EA.UI.Utils.GetValueFromDictionary(crumbtext);
            if (crumbtextdisplay == null)
            {
                FailStep(CL, "Failed to fetch from Dictionary");
            }
            
            LogCommentInfo(CL, "Fetched value from dictionary for" + crumbtext + " is " + crumbtextdisplay);

            EpgScreen = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SCREEN");
            if (EpgScreen == null)
            {
                FailStep(CL, "Failed to Fetch value from Test ini for EPG Screen");
            }
            //fetch expected state
            expectedState = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EXPECTED_STATE");
            if (expectedState == null)
            {
                LogCommentWarning(CL, "not defined as per project specification");
            }
           
            CL.IEX.MilestonesEPG.ClearEPGInfo();
           
            PassStep();
        }
    }
    #endregion
    #region Step1
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.Navigate to EPG Screen and check the Crumtext display .
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
        public override void Execute()
        {

            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }
            
            //Navigate To Screen

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:"+EpgScreen);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to " + EpgScreen);
            }
           
            //Retreving CRUMBTEXT info 

            CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTEXT", out crumtextfromepg);
            if (crumtextfromepg != null)
            {
                if (firstCrumbTextVisibility == "NO")
                {
                    LogCommentImportant(CL, "Since as expected there is no CRUMTEXT present,validation is proper");
                }
 
            }

            if (Convert.ToInt32(numberOfCrumTextVisibility) >1)
            {
                    //insert default
                    CL.EA.EnterDeafultPIN(expectedState);
                
                      //fetch crumtext value and compare with expected value
                    string crumName = "";
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTEXT", out crumName);

                    if (crumName != secondCrumTextVisibility)
                    {
                        FailStep(CL, "Crum Title did not matched as expected");
                    }

            }
            PassStep();
        }
    }
    #endregion
    #endregion

}
