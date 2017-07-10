/// <summary>
///  Script Name : CRUMB_0001_EPGScreensNoCrum
///  Test Name   : EPG-Screens where Crumb Not available
///  TEST ID     : 71334
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
* @class   CRUMB_0001
*
* @brief   A crumb 0001.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
/**********************************************************************************************/
/**
* @class   CRUMB_0001
*
* @brief   A crumb 0001.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
[Test("CRUMB_0001")]
public class CRUMB_0001 : _Test
{
    /**********************************************************************************************/
    /**
* @brief   Checking Crumtitle on EPG Screens where the CRUMBTITLE shouldn't be displayed.
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @brief   The cl.
**************************************************************************************************/
    [ThreadStatic]
    static _Platform CL;
    /**********************************************************************************************/
    /**
* @brief   The audio video service.
**************************************************************************************************/
    private static Service AudioVideoService;
    /**********************************************************************************************/
    /**

* @brief   The crumbonmain.
**************************************************************************************************/
    private static string crumbonmain;
    /**********************************************************************************************/
    /**
* @brief   The crumboncb.
**************************************************************************************************/
    private static string crumboncb;
    /**********************************************************************************************/
    /**
* @brief   The crumbonlive.
**************************************************************************************************/
    private static string crumbonlive;
    /**********************************************************************************************/
    /**
* @brief   The crumbtilte visibilty.
**************************************************************************************************/
    private static string CrumbtilteVisibilty;
    /**********************************************************************************************/
    /**
* @brief   The crumbonmodifypinscreen.
**************************************************************************************************/
    private static string crumbonmodifypinscreen;
    /**********************************************************************************************/
    /**
    /**********************************************************************************************/
    /**
* @brief   The lockchannel menu item.
**************************************************************************************************/
    private static string LockchannelMenuItem;
    /**********************************************************************************************/
    /**
* @brief   Information describing the precondition.
**************************************************************************************************/
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers,Crumbtitle & crumbtilte value From ini File";
    /**********************************************************************************************/
    /**
* @brief   Navigating to MAIN MENU,CHANNEL BAR &amp; LIVE and check the CRUMBTITLE display.
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 1.
**************************************************************************************************/
    private const string STEP1_DESCRIPTION = "Step 1: Checking CrumbtitleVisibilty while doing DCA ";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 2.
**************************************************************************************************/
    private const string STEP2_DESCRIPTION = "Step 2: Checking CrumbtitleVisibilty While doing Zapping";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 3.
**************************************************************************************************/
    private const string STEP3_DESCRIPTION = "Step 3: Checking CrumbtitleVisibilty While doing Fast Zapping";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 4.
**************************************************************************************************/
    private const string STEP4_DESCRIPTION = "Step 4: Checking CrumbtitleVisibilty While launching Main Menu";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 5.
**************************************************************************************************/
    private const string STEP5_DESCRIPTION = "Step 5: Checking CrumbtitleVisibilty while launching ChannelBar";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 6.
**************************************************************************************************/
    private const string STEP6_DESCRIPTION = "Step 6: Checking CrumbtitleVisibilty in Live";
    /**********************************************************************************************/
    /**
* @brief   Information describing the step 7.
**************************************************************************************************/
    private const string STEP7_DESCRIPTION = "Step 7: Checking CrumbtitleVisibilty in Modify Pin Screen ";
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
    /**********************************************************************************************/
    /**
* @fn  public override void CreateStructure()
*
* @brief   Creates the structure.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);
        this.AddStep(new Step4(), STEP4_DESCRIPTION);
        this.AddStep(new Step5(), STEP5_DESCRIPTION);
        this.AddStep(new Step6(), STEP6_DESCRIPTION);
        this.AddStep(new Step7(), STEP7_DESCRIPTION);
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
    /**********************************************************************************************/
    /**
* @fn  public override void PreExecute()
*
* @brief   Pre execute.
*
* @author  Ponramanv
* @date    10/31/2013
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
* @brief   A pre condition.:Retreving channel  from ContentXML.
*
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
    /**********************************************************************************************/
    /**
* @class   PreCondition
*
* @brief   A pre condition.
*
* @author  Ponramanv
* @date    10/31/2013
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
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            LogCommentWarning(CL, "Warning:No UI validation and dictionary-key values from Dictionary file are not validated");
            //Get Values From Content XML File
            AudioVideoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (AudioVideoService == null)
            {
                FailStep(CL,"Failed to fetch Audio Video Service from content.xml");
            }

            //Get Dictionary key values  from dictionary file 
            CrumbtilteVisibilty = CL.EA.UI.Utils.GetValueFromDictionary("DIC_ACTION_NO");
            if (CrumbtilteVisibilty == null)
            {
                FailStep(CL, "Failed to fetch value for CrumbtilteVisibilty from dictionary");
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
* @brief   A step 1.Navigating to MAIN MENU,CHANNEL BAR & LIVE and check the CRUMBTITLE display
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
    /**********************************************************************************************/
    /**
* @class   Step1
*
* @brief   A step 1.
*
* @author  Ponramanv
* @date    10/31/2013
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
* @author  Ponramanv
* @date    10/17/2013
**************************************************************************************************/
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, AudioVideoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA");
            }
            //Retreving CRUMBTITLE info in DCA
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumbonmain);
            if (String.IsNullOrEmpty(crumbonmain))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumbonmain.Equals(CrumbtilteVisibilty))
            {
                LogCommentInfo(CL, "CRUMBTITLE is not displayed in DCA  " + "Expected :" + crumbonmain + "Actual:" + CrumbtilteVisibilty);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed in DCA" + "Expected :" + crumbonmain + "Actual: " + CrumbtilteVisibilty);
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
* @brief   A step 2.
*
* @author  Ponramanv
* @date    10/31/2013
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
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Zap between two channel and check the CRUMTITLE display
            //Tune to service s2 using channel bar
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, IsNext: false, NumberOfPresses: 1, DoTune: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service from channel bar" );
            }
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumbonmain);
            if (String.IsNullOrEmpty(crumbonmain))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumbonmain.Equals(CrumbtilteVisibilty))
            {
                LogCommentInfo(CL, "CRUMBTITLE is not displayed while Zapping between two channel  " + "Expected :" + crumbonmain + "Actual: " + CrumbtilteVisibilty);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed while Zapping " + "Expected :" + crumbonmain + "Actual: " + CrumbtilteVisibilty);
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
* @brief   A step 3.
*
* @author  Ponramanv
* @date    10/31/2013
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
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Go to live and perform Fast Zapping then check the crumtitle
            res = CL.EA.ReturnToLiveViewing(false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to fullscreen!");
            }
            //Surf to next predicted service
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, IsNext: true, NumberOfPresses: 1,IsPredicted:EnumPredicted.Predicted, DoTune: true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to the next service!");
            }
            //Retreving CRUMBTITLE info in Fast Zapping
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumbonmain);
            if (String.IsNullOrEmpty(crumbonmain))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumbonmain.Equals(CrumbtilteVisibilty))
            {
                LogCommentInfo(CL, "CRUMBTITLE is not displayed in FastZapping " + "Expected :" + crumbonmain + "Actual:" + CrumbtilteVisibilty);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed in FastZapping" + "Expected :" + crumbonmain + "Actual: " + CrumbtilteVisibilty);
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
* @brief   A step 4.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Launching MAIN MENU
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MAIN MENU");
            }
            //Retreving CRUMBTITLE info while launching MAIN MENU
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumbonmain);
            if (String.IsNullOrEmpty(crumbonmain))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumbonmain.Equals(CrumbtilteVisibilty))
            {
                LogCommentInfo(CL, "CRUMBTITLE is not displayed in Main Menu  screen " + "Expected :" + crumbonmain + "Actual: " + CrumbtilteVisibilty);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed in Main Menu Screen" + "Expected :" + crumbonmain + "Actual: " + CrumbtilteVisibilty);
            }
            PassStep();
        }
    }
    #endregion
    #region Step5
    /**********************************************************************************************/
    /**
* @class   Step5
*
* @brief   A step 5.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
    [Step(5, STEP5_DESCRIPTION)]
    public class Step5 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Navigate To CHANNEL BAR 
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to CHANNEL BAR");
            }
            //Retreving CRUMBTITLE info while launching channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumboncb);
            if (String.IsNullOrEmpty(crumboncb))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumboncb.Equals(CrumbtilteVisibilty))
            {
                LogCommentInfo(CL, "CRUMBTITLE is not displayed in Channel bar  screen " + "Expected :" + crumboncb + "Actual: " + CrumbtilteVisibilty);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed in Channel bar Screen" + "Expected :" + crumboncb + "Actual: " + CrumbtilteVisibilty);
            }
            PassStep();
        }
    }
    #endregion
    #region Step6
    /**********************************************************************************************/
    /**
* @class   Step6
*
* @brief   A step 6.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
    [Step(6, STEP6_DESCRIPTION)]
    public class Step6 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Navigate To LIVE
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TO FSV");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to LIVE");
            }
            //Retreving CRUMBTITLE info on FSV
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumbonlive);
            if (String.IsNullOrEmpty(crumbonlive))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumbonlive.Equals(CrumbtilteVisibilty))
            {
                LogCommentInfo(CL, "CRUMBTITLE value is not diplayed in FSV screen " + "Expected :" + crumbonlive + "Actual: " + CrumbtilteVisibilty);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed in FSV Screen" + "Expected :" + crumbonlive + " Actual:" + CrumbtilteVisibilty);
            }
            PassStep();
        }
    }
    #endregion
    #region Step7
    /**********************************************************************************************/
    /**
* @class   Step7
*
* @brief   A step 7.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
    [Step(7, STEP7_DESCRIPTION)]
    public class Step7 : _Step
    {
        /**********************************************************************************************/
        /**
* @fn  public override void Execute()
*
* @brief   Executes this object.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
        public override void Execute()
        {
            StartStep();
            //Navigate To MODIFYPINSCREEN
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Action Bar");
            }
            res = CL.IEX.MilestonesEPG.Navigate("LOCK CHANNEL");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to MODIFYPINSCREEN");
            }
            //Retreving CRUMBTITLE info in MODIFYPINSCREEN screen
            res = CL.IEX.MilestonesEPG.GetEPGInfo("CRUMBTITLE_VISIBLE", out crumbonmodifypinscreen);
            if (String.IsNullOrEmpty(crumbonmodifypinscreen))
            {
                CL.IEX.FailStep("Failed to get CRUMBTITLE_VISIBLE trace from EPG");
            }
            if (crumbonmodifypinscreen.Equals(CrumbtilteVisibilty))
            {
                LogCommentInfo(CL, "CRUMBTITLE is not displayed in Modify Pin  screen " + "Expected :" + crumbonmodifypinscreen + "Actual:" + CrumbtilteVisibilty);
            }
            else
            {
                FailStep(CL, "CRUMBTITLE is getting displayed in Modify Pins Screen" + "Expected :" + crumbonmodifypinscreen + "Actual:"+ CrumbtilteVisibilty);
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
    /**********************************************************************************************/
    /**
* @fn  public override void PostExecute()
*
* @brief   Posts the execute.
*
* @author  Ponramanv
* @date    10/31/2013
**************************************************************************************************/
    [PostExecute()]
    public override void PostExecute()
    {
    }
    #endregion
}