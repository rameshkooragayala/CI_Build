using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-0407-change language settings
public class FullSanity_0407 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Channels used by the test
    static string default_option = "ENGLISH";
    static string select_option;
    static string selected_option = "";
    static string diffMenuLanguage = "";


    #region Create Structure

    public override void CreateStructure()
    {
        //Adding steps
        this.AddStep(new Step1(), "Step 1: Navigate to Menu Language screen and verify the default Language and list of languages Present");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigation: "STATE:MENU LANGUAGE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state Menu language");
            }
            string menuLanguages = CL.EA.GetValueFromINI(IEX.ElementaryActions.Functionality.EnumINIFile.Test, "TEST PARAMS", "MENU_LANGUAGES");
            if (menuLanguages == "")
            {
                FailStep(CL, "MENU_LANGUAGES is not present in test ini");
            }
            string defaultLanguage;
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out defaultLanguage);
            if (defaultLanguage.ToUpper() != "ENGLISH")
            {
                FailStep(CL, res, "Default language is not ENGLISH");
            }

            string[] menuLanguageList = menuLanguages.Split(',');
            foreach (string language in menuLanguageList)
            {
                LogCommentImportant(CL, "Trying to find the Menu Language " + language);
                string currentManuLanguage = "";
                string timestamp = "";
                int count = 0;
                bool ispass = false;
                while (count < menuLanguageList.Length)
                {
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out currentManuLanguage);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to get the EPG info for title");
                    }

                    if (currentManuLanguage == language)
                    {
                        LogCommentImportant(CL, "Found the language " + language + " in Menu Language Screen");
                        ispass = true;
                        break;
                    }

                    res = CL.IEX.SendIRCommand("SELECT_DOWN", -1, ref timestamp);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to send IR select down");
                    }
                    CL.IEX.Wait(2);
                    count++;
                }
                if (!ispass)
                {
                    FailStep(CL, "Failed to find the Menu Language " + language);
                }
            }


            PassStep();
        }
    }

    #endregion Step1

    #endregion Steps

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}