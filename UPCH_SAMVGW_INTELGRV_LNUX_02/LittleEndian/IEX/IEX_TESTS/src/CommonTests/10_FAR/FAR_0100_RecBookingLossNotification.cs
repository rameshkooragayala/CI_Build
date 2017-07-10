/// <summary>
///  Script Name : FAR_0100_RecBookingLossNotification.cs
///  Test Name   : FAR-0100-Recording and Booking Loss Notification
///  TEST ID     : 68256/68257/68258
///  QC Version  : 1
///  Variations from QC: NONE
/// ----------------------------------------------- 
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**
 * @class   FAR_0100
 *
 * @brief   Far 0100.
 *
 * @author  Varshad
 * @date    20-Sep-13
 */

[Test("FAR_0100")]
public class FAR_0100 : _Test
{
    /**
     * @brief   The _Platform cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @brief   The retain recording screen.
     */

    static string retainRecordingScreen;

    /**
     * @brief   The confirm delete record screen.
     */

    static string confirmDeleteRecScreen;

    static string pinEnterState;

    /**
    * @brief   The select
    */

    static string selectKey;

    /**
     * @brief   Get values from project.ini.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get values from project.ini";

    /**
     * @brief   Navigate to Factory reset.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Factory reset";

    /**
     * @brief   Verify Pin entry state is obtained before performing factory reset.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Verify Pin entry state is obtained before performing factory reset";

    /**
     * @brief   Verify Save Recodings is the default option.
     */

    private const string STEP3_DESCRIPTION = "Step 3: Verify Save Recodings is the default option";

    /**
     * @brief   Verify Recording and Booking Loss Notification is obtained.
     */

    private const string STEP4_DESCRIPTION = "Step 4: Verify Recording and Booking Loss Notification is obtained";

    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

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

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**
     * @class   PreCondition
     *
     * @brief   Pre condition.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    20-Sep-13
         */

        public override void Execute()
        {
            StartStep();
            
            //Get Values From ini File
            retainRecordingScreen = CL.EA.GetValueFromINI(EnumINIFile.Project, "FACTORY_RESET", "LOG_SAVE_RECORD_SCREEN");
            confirmDeleteRecScreen = CL.EA.GetValueFromINI(EnumINIFile.Project, "FACTORY_RESET", "LOG_CONFIRM_DELETE_REC_SCREEN");
            pinEnterState = CL.EA.GetValueFromINI(EnumINIFile.Project, "STATES", "PIN_ENTER_STATE");
            selectKey = CL.EA.GetValueFromINI(EnumINIFile.Project, "KEY_MAPPING", "SELECT_KEY");


            if (string.IsNullOrEmpty(retainRecordingScreen) || string.IsNullOrEmpty(confirmDeleteRecScreen) || string.IsNullOrEmpty(selectKey)
               || string.IsNullOrEmpty(pinEnterState))
            {
                FailStep(CL, "One of the values is null. LOG_SAVE_RECORD_SCREEN: " + retainRecordingScreen +
                    ", LOG_CONFIRM_DELETE_REC_SCREEN:" + confirmDeleteRecScreen + ", SELECT_KEY" + selectKey + ", PIN_ENTER_STATE: " + pinEnterState);
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * @class   Step1
     *
     * @brief   Navigate to Factory reset.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    20-Sep-13
         */

        public override void Execute()
        {
            StartStep();
            
            //Navigate to Factory reset
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:FACTORY RESET");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to navigate to Factory Reset");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**
     * @class   Step2
     *
     * @brief   Verify Pin entry state is obtained before performing factory reset.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    20-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Verify Factory Reset State
            if (!CL.EA.UI.Utils.VerifyState(pinEnterState))
            {
                FailStep(CL, "Failed to verify" + pinEnterState +" state");
            }

            //Enter PIN
            res = CL.EA.EnterDeafultPIN("YES_NO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to Enter PIN");
            }

            //Verify YES_NO State
            if (!CL.EA.UI.Utils.VerifyState("YES_NO"))
            {
                FailStep(CL, "Failed to verify YES_NO state");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3

    /**
     * @class   Step3
     *
     * @brief   Verify Save Recodings is the default option.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    20-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            string obtainedValue = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("PopupType", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to get PopupType");
            }

            if (!obtainedValue.Equals(retainRecordingScreen))
            {
                FailStep(CL, "Save Recording Screen is not launched. Obtained Value: " + obtainedValue);
            }

            //Verify if the Default option is Keep_Recordings
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get EPG Info title");
            }

            if (!obtainedValue.Equals("YES", StringComparison.OrdinalIgnoreCase))
            {
                FailStep(CL, "Default option is not Keep_Recording. Obtained Value is: " + obtainedValue);
            }

            PassStep();
        }
    }
    #endregion

    #region Step4

    /**
     * @class   Step4
     *
     * @brief   Verify Recording and Booking Loss Notification is obtained.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

    [Step(4, STEP4_DESCRIPTION)]
    public class Step4 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    20-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            string obtainedValue = "";

            //Clear EPG Info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to perform clear EPG Info");
            }

            //Do not Keep records
            res = CL.IEX.MilestonesEPG.SelectMenuItem("NO");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, " Failed to select menu item");
            }

            string timeStamp = "";
            res = CL.IEX.SendIRCommand(selectKey, -1, ref timeStamp); // -1 Time to press
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res.FailureReason);
            }

            //Verify if Confirm Delete recording screen is displayed
            res = CL.IEX.MilestonesEPG.GetEPGInfo("PopupType", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res.FailureReason);
            }

            if (!obtainedValue.Equals(confirmDeleteRecScreen))
            {
                FailStep(CL, "Confirm Recording screen is not Launched. Obtained Value: " + obtainedValue);
            }

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute

    /**
     * @fn  public override void PostExecute()
     *
     * @brief   Posts the execute.
     *
     * @author  Varshad
     * @date    20-Sep-13
     */

    [PostExecute()]
    public override void PostExecute()
    {

    }
    #endregion
}