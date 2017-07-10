/// <summary>
///  Script Name : STORE_0201_Navigation_Browsing.cs
///  Test Name   : STORE-0201-Navigation-Browsing
///  TEST ID     : 17449
///  QC Version  : 7
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

[Test("STORE_NAV_0201")]
public class STORE_NAV_0201 : _Test
{
    [ThreadStatic]
    static _Platform CL;
    static string itemsInStoreHorizontal;
    static string itemsInStoreVertical;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get the list of NameNavigation in STORE from test.ini";
    private const string STEP1_DESCRIPTION = "Step 1: HighLight the sub-classifications in one classification";
    private const string STEP2_DESCRIPTION = "Step 2: Check that each level of the classification can be browsed";


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
            /*In Test.ini NAMED_NAVIGATIONS_STORE_HORIZONTAL should have the named navigation of assets/sub-classifications directly under a classification seperated by comma.
             eg: NAMED_NAVIGATIONS_STORE_HORIZONTAL = STATE:SERIES,STATE:PARENTAL RATINGS, ... */

            itemsInStoreHorizontal = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NAMED_NAVIGATIONS_STORE_HORIZONTAL");
            if (string.IsNullOrEmpty(itemsInStoreHorizontal))
            {
                FailStep(CL, "NAMED_NAVIGATIONS_STORE_HORIZONTAL is empty in Test.ini in section TEST_PARAMS");
            }

            /*In Test.ini NAMED_NAVIGATIONS_STORE_VERTICAL should have the named navigation of assets/sub-classifications at different levels inside a classification seperated by comma.
             eg: NAMED_NAVIGATIONS_STORE_VERTICAL = STATE:ALL VIDEOS*/

            itemsInStoreVertical = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NAMED_NAVIGATIONS_STORE_VERTICAL");
            if (string.IsNullOrEmpty(itemsInStoreVertical))
            {
                FailStep(CL, "NAMED_NAVIGATIONS_STORE_VERTICAL is empty in Test.ini in section TEST_PARAMS");
            }
            // Set purchase protection to ON
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable purchase protection");
            }

            // Set parental rating threshold to 'Unlock all'
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to UNLOCK ALL");
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

            string[] namedNavigationsOfItems = itemsInStoreHorizontal.Split(',');

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

            string[] namedNavigationsOfItems = itemsInStoreVertical.Split(',');

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
    #endregion
}
