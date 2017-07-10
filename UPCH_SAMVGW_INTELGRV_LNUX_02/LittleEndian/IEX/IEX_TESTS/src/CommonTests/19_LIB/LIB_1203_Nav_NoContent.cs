/// <summary>
///  Script Name : LIB_1203_Nav_NoContent.cs
///  Test Name   : LIB-1203-No Content
///  TEST ID     : 61958
///  JIRA Test ID : FC-270
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Bharath Pai
/// </summary>

using System;
using IEX.Tests.Engine;


public class LIB_1203 : _Test
{
    [ThreadStatic]
    private static _Platform CL;


    //Constants to be used in the test case
    static string EventName = "";

    #region Create Structure

    public override void CreateStructure()
    {
        //Adding steps
        this.AddStep(new Step1(), "Step 1: Navigate to library and check if no content state occured");

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

            //Navigating to Archieve
            res = CL.EA.PVR.NavigateToArchive();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Archive");
            }

            //Checking if Archive is Empty
            LogComment(CL, "Checking if Archive is Empty");

            res = CL.IEX.MilestonesEPG.GetEPGInfo("Displaytitle", out EventName);

            //the below condition is to Handle : "sometimes the DisplayTitle will not exist in EPG info dictionary colllection if the Archive is having no recording."
            if (!res.CommandSucceeded && res.FailureReason.Contains("doesn't exist in the EPG info dictionary"))
            {
                LogComment(CL, "Archive is Empty and library has no content!");
            }

            if (res.CommandSucceeded)
            {
                if (EventName == "")
                {
                    LogComment(CL, "Archive is Empty and library has no content!");
                }
                else
                {
                    FailStep(CL, res, "Failed to verify whether library has no content!");

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

    #endregion PostExecute
}