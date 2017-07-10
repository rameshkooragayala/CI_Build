/// <summary>
///  Script Name : MENU_3912_MainMenu_Live.cs
///  Test Name   : EPG-3912-Main Menu from live viewing
///  TEST ID     : 64474
///  JIRA ID     : FC-384
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

[Test("MENU_3912")]
public class MENU_3912 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service videoService;
    static int noOfMainMenuItems;
    static string[] Main_Menu_Item;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Navigate to Main Menu and main menu items";


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

            //Get Values From xml File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video Service fetched from Content.xml is null");
            }

            string mainMenuItemsStringVal = CL.EA.GetValueFromINI(EnumINIFile.Project, "MENUS", "MAIN_MENU_ITEMS");
            if (string.IsNullOrEmpty(mainMenuItemsStringVal))
            {
                FailStep(CL, "MAIN_MENU_ITEMS is not defined under MENUS section in Project.ini. Value is null");
            }
            
            noOfMainMenuItems = Int16.Parse(mainMenuItemsStringVal);
            Main_Menu_Item = new String[noOfMainMenuItems];

            for (int i = 0; i < noOfMainMenuItems; i++)
            {
                Main_Menu_Item[i] = CL.EA.GetValueFromINI(EnumINIFile.Project,"MENUS", "MAIN_MENU_ITEM_" + i);
                if (string.IsNullOrEmpty(Main_Menu_Item[i]))
                {
                    FailStep(CL, "MAIN_MENU_ITEM_" + i + " is not defined in Project.ini file. Valu0e is null");
                }
            }

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

            //Navigate to Main Menu and main menu items
            int i = 0;
            LogComment(CL,"Main_Menu_Item.Length is :" + Main_Menu_Item.Length);
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res, "Failed to Navigate to STATE:MAIN MENU");
            }

            // Loop thru menu items 
            LogCommentInfo(CL,"Loop thru all the menu items");
            for (i = 0; i < Main_Menu_Item.Length; i++)
            {
                LogComment(CL,"Main_Menu_Item is :" + Main_Menu_Item[i]);
                if (!SelectMenuItem(Main_Menu_Item[i]))
                {
                    FailStep(CL,res, "Failed to SelectMenuItem : " + Main_Menu_Item[i]);
                }
            }

            PassStep();
        }

        //Select Each Menu item
        bool SelectMenuItem(string Item)
        {
            res = CL.IEX.MilestonesEPG.NavigateByName(Item);
            return res.CommandSucceeded;
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