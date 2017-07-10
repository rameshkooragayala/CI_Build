/// <summary>
///  Script Name : STORE_0200_Navigation_Display.cs
///  Test Name   : STORE-0200-Navigation-Display
///  TEST ID     : 17448
///  QC Version  : 8
///  Variations from QC: None
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date : 03/03/2014
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("STORE_NAV_0200")]
public class STORE_NAV_0200 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static string itemsInStore;
    static string itemToNavigate;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get the list of NameNavigation in STORE from test.ini";
    private const string STEP1_DESCRIPTION = "Step 1: HighLight the Classifications";
    private const string STEP2_DESCRIPTION = "Step 2: Enter one of the Classifications";


    static class Constants
    {
        public const double secToWait = 3;
    }

    #region Create Structure
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
            /*In Test.ini NAMED_NAVIGATIONS_STORE should have all the named navigation listed inside ON DEMAND seperated by comma.
             eg: NAMED_NAVIGATIONS_STORE = STATE:ALL VIDEOS,STATE:COD,STATE:SANITY, ... */

            itemsInStore = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NAMED_NAVIGATIONS_STORE");
            if (string.IsNullOrEmpty(itemsInStore))
            {
                FailStep(CL, "NAMED_NAVIGATIONS_STORE is empty in Test.ini in section TEST_PARAMS");
            }

            /*In Test.ini ITEM_NAVIGATE should have any one named navigation in ON DEMAND
             eg: ITEM_NAVIGATE = STATE:ALL VIDEOS*/

            itemToNavigate = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ITEM_NAVIGATE");
            if (string.IsNullOrEmpty(itemToNavigate))
            {
                FailStep(CL, "ITEM_NAVIGATE is empty in Test.ini in section TEST_PARAMS");
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

            string[] namedNavigationsOfItems = itemsInStore.Split(',');

            //Navigate and Highlight to each Category in the STORE
            foreach (string namedNavigations in namedNavigationsOfItems)
            {

                res = CL.EA.NavigateAndHighlight(namedNavigations);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to navigate and highlight by named navigation " + namedNavigations);
                }

                //Optional: waiting for few secs between each navigations
                res = CL.IEX.Wait(Constants.secToWait);
                if (!res.CommandSucceeded)
                {
                    LogCommentWarning(CL, res.FailureReason);
                }
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Navigating to any Category in Store
            res = CL.EA.NavigateAndHighlight(itemToNavigate);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate and highlight by named navigation " + itemToNavigate);
            }

            //Selecting on the item 
            CL.EA.UI.Utils.SendIR("SELECT");

            PassStep();
        }
    }
    #endregion
    #endregion
}