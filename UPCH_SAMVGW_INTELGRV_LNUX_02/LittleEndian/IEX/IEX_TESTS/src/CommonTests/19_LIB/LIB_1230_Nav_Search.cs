/// <summary>
///  Script Name : LIB_1230_Nav_Search.cs
///  Test Name   : LIB-1230-Search
///  TEST ID     : 61964
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.Tests.Engine;

public class LIB_1230 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Constants used in the test
    private static class Constants
    {
        public const string navigationToSearchInLibrary = "STATE:LIBRARY SEARCH";
    }

    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new Step1(), "Step 1: Navigate to search option in library");

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

            //Navigate to search option in library
            res = CL.IEX.MilestonesEPG.NavigateByName(Constants.navigationToSearchInLibrary);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Could not navigate to the search option in the library!!");
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

    #endregion PostExecute
}