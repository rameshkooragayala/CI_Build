/// <summary>
///  Script Name : CRUMB_0009_Profile
///  Test Name   : EPG_Crumb display_profile
///  TEST ID     : 71341
///  QC Version  : 1
///  Variations from QC:No UI validation and dictionary-key values from Dictionary file are not validated.
///   QC Repository :STB_DIVISION-Unified_ATP_For_HMD_Cable-WP2429 Crumbtrail 
/// ----------------------------------------------- 
///  Modified by : Scripted by : Ponraman Vijayakumar
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
* @class   CRUMB_0009_Profile
*
* @brief   Checking CRUMBTITLE &amp; Crumbtext in EPG-PROFILE  Screen.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
[Test("CRUMB_0009")]
public class CRUMB_0009: _Test
{
    /**********************************************************************************************/
    /**
* @brief   Navigate to Profile Screen and Check CRUMTILTE &amp; CRUMBTEXT.
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
* @brief   The crumbon profile.
**************************************************************************************************/
    private static string crumbonProfile;
    
    /**********************************************************************************************/
    /**
* @brief   The crumbonnewprofile.
**************************************************************************************************/
    private static string crumbonnewprofile;
    /**********************************************************************************************/
    /**
* @brief   The Crumbtextdisplayin profile.
**************************************************************************************************/
    private static string CrumbtextdisplayinProfile;
    /**********************************************************************************************/
    /**
* @brief   The Crumbtextdisplayinnewprofile.
**************************************************************************************************/
    private static string Crumbtextdisplayinnewprofile;
    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers,Crumbtitle & crumbtilte value From ini File";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/
    private const string STEP1_DESCRIPTION = "Step 1:Enter Menu->profile";

    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/
    private const string STEP2_DESCRIPTION = "Step 2:Enter New profile";
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
        this.AddStep(new Step2(), STEP2_DESCRIPTION);


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
* @brief   A pre condition.Retreive Channel detail from Content XML.
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
            LogCommentWarning(CL, "Warning:No UI validation and dictionary-key values from Dictionary file are not validated");
            //Get Values From Content XML File
            audioVideoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            //Get Dictionary key values  from dictionary file 
            Crumbtextdisplayinnewprofile = CL.EA.UI.Utils.GetValueFromDictionary("DIC_PROFILE_NEW_PROFILE");
            CrumbtextdisplayinProfile = CL.EA.UI.Utils.GetValueFromDictionary("DIC_CRUMBTRAILUTIL_PROFILES");
            PassStep();
        }
    }
    #endregion
    #region Step1
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.Navigate to Profile Screen and checking CRUMTITLE.
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
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, audioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }
            //Navigate To PROFILE Screen
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:PROFILES");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to PROFILEScreen");
            }
            //Retreving CRUMBTITLE info in PROFILEScreen
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE", out crumbonProfile);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get CRUMBTITLE in PROFILE Screen");
            }
            if (crumbonProfile.Equals(CrumbtextdisplayinProfile))
            {
                LogCommentInfo(CL, "CRUMBTITLE is displayed in PROFILE  Screen " + crumbonProfile + " " + CrumbtextdisplayinProfile);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is not displayed in PROFILE Screen" + crumbonProfile + " " + CrumbtextdisplayinProfile);
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
* @brief   A step 2.Navigating to NEW PROFILE and check the CRUMBTEXT.
*
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    [Step(2, STEP2_DESCRIPTION)]
    public class Step2 : _Step
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
            //Navigate To NEW PROFILE
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:NEW PROFILE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to NEW PROFILE Screen");
            }
            //Retreving CRUMBTITLE info in NEW PROFILE screen
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTEXT", out crumbonnewprofile);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get CRUMBTITLE in NEW PROFILE Screen");
            }
            if (crumbonnewprofile.Equals(Crumbtextdisplayinnewprofile))
            {
                LogCommentInfo(CL, "CRUMBTEXT is displayed in NEW PROFILE Screen " + crumbonnewprofile + " " + Crumbtextdisplayinnewprofile);
            }
            else
            {
                FailStep(CL, res, "CRUMBTEXT is not displayed in NEW PROFILE Screen" + crumbonnewprofile + " " + Crumbtextdisplayinnewprofile);
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
* @author  Ponraman V
* @date    10/21/2013
**************************************************************************************************/
    [PostExecute()]
    public override void PostExecute()
    {
    }
    #endregion
}