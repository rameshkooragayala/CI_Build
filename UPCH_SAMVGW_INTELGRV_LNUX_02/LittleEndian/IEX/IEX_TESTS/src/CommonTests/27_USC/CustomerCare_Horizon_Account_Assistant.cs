using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class CustomerCare : _Test
{
    [ThreadStatic]
    static _Platform CL;

    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Preconditions: Navigate to Customer Care, Horizon Assistant FAQ and verify the text");
        this.AddStep(new Step1(), "Step 1: Navigate to Customer Care, Account Assistant FAQ and verify the text");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CUSTOMER CARE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to CUSTOMER CARE");
            }

            string expectedFAQValue = CL.EA.UI.Utils.NavigateToCustomerCareFAQ("HORIZON ASSISTANT");
            if (expectedFAQValue == "")
            {
                FailStep(CL,res,"Failed to Navigate to Customer Care FAQ and Verify the FAQ String");
            }
            string ObtainedFAQValue = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out ObtainedFAQValue);
           if (!res.CommandSucceeded)
           {
               FailStep(CL, res, "Failed to get the EPG info from " + ObtainedFAQValue);
           }
            //As there is an issue with the selection title milestone we are using contains which needs to be changed once the CQ is resolved
           if (expectedFAQValue.StartsWith(ObtainedFAQValue))
           {
               LogCommentImportant(CL, "Verified that the Obtained FAQ from Dictionary " + expectedFAQValue + " is same as obtained value from EPG " + ObtainedFAQValue);
           }
           else
           {
               FailStep(CL, "Failed to verify that the Obtained FAQ from Dictionary " + expectedFAQValue + " is same as obtained value from EPG " + ObtainedFAQValue);
           }

           res = CL.EA.ReturnToLiveViewing();
           if (!res.CommandSucceeded)
           {
               FailStep(CL,res,"Failed to return to live viewing");
           }
            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CUSTOMER CARE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to CUSTOMER CARE");
            }

            string expectedFAQValue = CL.EA.UI.Utils.NavigateToCustomerCareFAQ("ACCOUNT ASSISTANT");
            if (expectedFAQValue == "")
            {
                FailStep(CL, res, "Failed to Navigate to Customer Care FAQ and Verify the FAQ String");
            }
            string ObtainedFAQValue = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("title", out ObtainedFAQValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG info from " + ObtainedFAQValue);
            }
            //As there is an issue with the selection title milestone we are using contains which needs to be changed once the CQ is resolved
            if (expectedFAQValue.StartsWith(ObtainedFAQValue))
            {
                LogCommentImportant(CL, "Verified that the Obtained FAQ from Dictionary " + expectedFAQValue + " is same as obtained value from EPG " + ObtainedFAQValue);
            }
            else
            {
                FailStep(CL, "Failed to verify that the Obtained FAQ from Dictionary " + expectedFAQValue + " is same as obtained value from EPG " + ObtainedFAQValue);
            }
            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}