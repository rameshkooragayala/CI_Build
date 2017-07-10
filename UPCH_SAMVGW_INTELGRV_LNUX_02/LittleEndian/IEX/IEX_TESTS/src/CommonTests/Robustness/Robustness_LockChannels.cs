/// <summary>
///  Script Name : Robustness_LockChannels.cs
///  Test Name   : 
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : 
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("Robustness_LockChannels")]
public class Robustness_LockChannels : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1:Lock Channels , unlock with out PIN and unlock with PIN";
    

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

            //Get Values From ini File
            FTA_Channel = CL.EA.GetValueFromINI(EnumINIFile.Channels, "CHANNELS", "FTA_Channel");

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
            DateTime Currenttime=DateTime.Now;
            DateTime TestENdTime=DateTime.Now.AddHours(12);
            int pass = 0;
            int fail=0;

            for (double i = 1; i >= 1; i++)
            {
                try
                {

                    //lock channels

                    CL.EA.STBSettings.SetLockChannel("Clear Channel 1");
                    CL.EA.STBSettings.SetLockChannel("Clear Channel 2");
                    CL.EA.STBSettings.SetLockChannel("Clear Channel 3");
                    CL.EA.STBSettings.SetLockChannel("Clear Channel 4");
                    CL.EA.STBSettings.SetLockChannel("Clear Channel 5");
                    CL.EA.STBSettings.SetLockChannel("Clear Channel 7");
                    CL.EA.STBSettings.SetLockChannel("Clear Channel 8");


                    // unlock without entering PIN

                    //Flag to check if Pin entry screen is present at entry
                    bool isPinScreenPresent = false;

                    CL.EA.UI.Settings.NavigateToParentalControlLockUnlock(); // navigating to parential control loack/unloack channel


                    //try
                    //     {
                    //    LogCommentInfo(CL,"Checking whether Pin entry is required when entering Parental Control screen")
                    //    string  isPinScreenPresentStr  = CL.EA.UI.Utils.GetValueFromProject("CHANNEL_BLOCK", "IS_PIN_SCREEN_ON_UNLOCK_ENTRY"); // check pin required
                    //    isPinScreenPresent = Convert.ToBoolean(isPinScreenPresentStr);
                    //     }
                    //catch
                    //{
                    //   LogCommentWarning(CL,"Value not defined in project.ini. Please define it in case it is different from the default value! Taking default value..");

                    //    isPinScreenPresent = false;
                    //}


                    CL.EA.UI.Utils.SendIR("SELECT_LEFT"); // move to confirm screen

                    CL.IEX.Wait(3);

                    CL.EA.UI.Utils.SendIR("SELECT");

                    string Title = "";
                    CL.EA.UI.Utils.GetEpgInfo("title", ref Title);
                    string EpgText = "";
                    try
                    {
                      

                        EpgText = CL.EA.UI.Utils.GetValueFromDictionary("DIC_SETTINGS_PARENTAL_LOCK_CHANNEL");
                    }
                    catch
                    {

                    }
                    if (Title.Contains(EpgText))
                    {
                        LogCommentWarning(CL, "Unlocked all channels without entering PIN");
                    }
                    else
                    {
                        CL.EA.ReturnToLiveViewing(false); // return to live without entering PIN
                    }

                    CL.IEX.Wait(2);


                    CL.EA.STBSettings.SetUnLockChannel(""); // un lock all channels






                    pass = pass + 1;
                    Currenttime = DateTime.Now;
                    if (Currenttime >= TestENdTime)
                    {
                        break;
                    }

                }
                catch (Exception ex)
                {
                    fail = fail + 1;
                    CL.IEX.GetSnapshot(ex.Message);
                    LogCommentWarning(CL,"Failed in Iteration:"+ i.ToString()+"Failed due to "+ ex.Message);
                }
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