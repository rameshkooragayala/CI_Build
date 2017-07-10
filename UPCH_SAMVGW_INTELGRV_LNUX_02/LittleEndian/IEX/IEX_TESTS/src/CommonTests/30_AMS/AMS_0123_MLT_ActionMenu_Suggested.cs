/// <summary>
///  Script Name        : AMS_0123_MLT_ActionMenu_Suggested.cs
///  Test Name          : AMS-0123-MLT-ACTION MENU-SUGGESTED
///  TEST ID            : 
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 25th MAR, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_0123 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test


    static string evtNameMLT;
    static string noEITInfoActionMenu;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file and Navigate to Suggested MLT");
        this.AddStep(new Step1(), "Step 1: Verify the AMS tags for the events");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }

            noEITInfoActionMenu = CL.EA.GetValueFromINI(EnumINIFile.Project, "SEARCH", "NO_EIT_INFO");
            if (noEITInfoActionMenu == "")
            {
                FailStep(CL,"Failed to get the value from project ini");
            }

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:SUGGESTED");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Filed to Navigate to Suggested ");
            }

            CL.IEX.Wait(10);

            string timeStamp = "";
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend SELECT");
            }

            CL.IEX.Wait(10);

            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out evtNameMLT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to get the evtname from EPG info");
            }

            res = CL.IEX.MilestonesEPG.Navigate("MORE LIKE THIS");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate to MORE LIKE THIS");
            }


            PassStep();
        }
    }

    #endregion PreCondition
    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            CL.IEX.Wait(600);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.SUGGESTED);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            if (evtNameMLT == noEITInfoActionMenu)
            {
                evtNameMLT = "_UNKNOWN_TITLE_";
            }
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.ACTION_MENU, commonVariable: evtNameMLT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Action Menu in Event");
            }

            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.MORE_LIKE_THIS, commonVariable: evtNameMLT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Action Menu MORE LIKE THIS in Event");
            }
            PassStep();
        }
    }

    #endregion Step1




    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
        {
            LogCommentFailure(CL, "Failed to set the Personalization to No");
        }

    }

    #endregion PostExecute
}
