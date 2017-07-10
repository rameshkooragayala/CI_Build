/// <summary>
///  Script Name        : AMS_MLT_ActioMenu_Next_NoInfo
///  Test Name          : AMS-001-Search-Main menu, AMS-005-Search-Main menu-Keyword, AMS-012-Search-Main menu-REFINE
///  TEST ID            : 
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 11th March, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class AMS_MainMenu_Search : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static string evtName1 = "";
    static string enterSearchFromMenu = "";
    static string searchToRefineKey = "";
    static string refineToSearchKey = "";
    static string evtNameMLT;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Fetch services from XML file and Naviaget to Search from Main Menu");
        this.AddStep(new Step1(), "Step 1: Verify the AMS tags for the Search events");

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
            enterSearchFromMenu = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERACH_MENU");
            if (enterSearchFromMenu == "")
            {
                FailStep(CL, "Failed to get the enterSearchFromMenu from test ini");
            }
            searchToRefineKey = CL.EA.GetValueFromINI(EnumINIFile.Project, "SEARCH", "SEARCH_TO_REFINE_KEY");
            if (searchToRefineKey == "")
            {
                FailStep(CL, "Failed to get the searchToRefineKey from test ini");
            }
            //STATE:LIBRARY SEARCH-STATE:SEARCH
            refineToSearchKey = CL.EA.GetValueFromINI(EnumINIFile.Project, "SEARCH", "REFINE_TO_SEARCH_KEY");
            if (refineToSearchKey == "")
            {
                FailStep(CL, "Failed to get the refineToSearchKey from test ini");
            }
           
            res = CL.IEX.MilestonesEPG.NavigateByName(enterSearchFromMenu);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to Navigate To Search");
            }
            string timeStamp = "";
            CL.IEX.SendIRCommand("KEY_1",- 1, ref timeStamp);

            CL.IEX.Wait(5);
            res = CL.IEX.MilestonesEPG.GetEPGInfo("keyWord", out evtName1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the keyword from the EPG");
            }
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend SELECT Down");
            }
            CL.IEX.Wait(5);


            res = CL.IEX.SendIRCommand(searchToRefineKey ,-1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend");
            }
            CL.IEX.Wait(5);
            res = CL.IEX.SendIRCommand(refineToSearchKey, -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend");
            }
            CL.IEX.Wait(5);

            res = CL.IEX.SendIRCommand(searchToRefineKey, -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend");
            }
            CL.IEX.Wait(5);
            res = CL.IEX.SendIRCommand(refineToSearchKey, -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend");
            }
            string title="";
            int count=1;
             while (title != "INFO")
            {
                res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to send the IR commend SELECT");
                }
                CL.IEX.Wait(10);
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title",out title);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL,res,"Failed to get the title from EPG");
                }
                count++;
                if (count > 10)
                {
                    FailStep(CL, "Failed to Navigate to INFO from SEARCH");
                    break;
                }

                LogCommentImportant(CL,"title fetched from EPG is "+title);
            }
            
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname",out evtNameMLT);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "failed to get the evtname from EPG info");
            }

            while (title != "MORE LIKE THIS")
            {
                res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timeStamp);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to send the IR commend SELECT");
                }
                CL.IEX.Wait(10);
                res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out title);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get the title from EPG");
                }
                count++;
                if (count > 10)
                {
                    FailStep(CL,"Failed to Navigate to MLT from ACTION MENU");
                    break;
                }
                LogCommentImportant(CL, "title fetched from EPG is " + title);
            }
            CL.IEX.Wait(10);
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend SELECT");
            }

            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            CL.IEX.Wait(2);
			CL.IEX.SendIRCommand("RETOUR", -1, ref timeStamp);
            CL.IEX.Wait(2);
            res = CL.IEX.MilestonesEPG.NavigateByName(enterSearchFromMenu);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate To Search");
            }
            CL.IEX.Wait(5);
            CL.IEX.SendIRCommand("KEY_1", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("KEY_1", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("KEY_1", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("KEY_1", -1, ref timeStamp);
            CL.IEX.Wait(2);
            CL.IEX.SendIRCommand("KEY_1", -1, ref timeStamp);
            CL.IEX.Wait(2);
            res = CL.IEX.SendIRCommand("SELECT", -1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send the IR commend SELECT Down");
            }
            CL.IEX.Wait(5);
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
            if (enterSearchFromMenu.Contains("LIBRARY"))
            {
                res = CL.EA.VerifyAMSTags(EnumAMSEvent.SEARCH_LIBRARY);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the Sewarch Main Menu Event");
                }
            }
            else
            {
                res = CL.EA.VerifyAMSTags(EnumAMSEvent.SEARCH_MAIN_MENU);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify the Sewarch Main Menu Event");
                }
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.SEARCH_TRIGGERED,commonVariable:"1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Search Trigerred in Event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.REFINE_SEARCH, commonVariable: "1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Refine Search in Event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.SEARCH_TRIGGERED, commonVariable: "11111");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify the Refine Search in Event");
            }

            CL.IEX.Wait(10);
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
