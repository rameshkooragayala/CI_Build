using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

public class FullSanity_2201 : _Test
{
    [ThreadStatic]
    static _Platform CL;


    //Channels used by the tes
    private static string defaultPin;
    private static bool isPinChanged = false;
    //Shared members between steps
    private static class Constants
    {
        public const string newPIN = "1111";
    }
    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Preconditions: Change the Default pin and Reset it from the PS Server");
        this.AddStep(new Step1(), "Step 1: Verify that the MPIN is reset to the default");



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
            CL.IEX.Wait(10);

            defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }
            string boxID = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID").ToString();
            if (boxID == "")
            {
                FailStep(CL, "Failed to get the BOX_ID from Environment ini");
            }
            else
            {
                LogCommentImportant(CL,"Box ID fetched from Environment ini file is "+boxID);
            }
            string remotePSURL = CL.EA.UI.Utils.GetValueFromEnvironment("RemotePSServerURL").ToString();
            if (remotePSURL == "")
            {
                FailStep(CL, "Failed to get the RemotePSServerURL from Environment ini");
            }
            else
            {
                LogCommentImportant(CL, "RemotePSServerURL fetched from Environment ini file is " + remotePSURL);
            }
            res = CL.EA.STBSettings.ChangePinCode(defaultPin, Constants.newPIN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to change pin code");
            }
            isPinChanged = true;

                FirefoxDriver driver = new FirefoxDriver();
            try
            {
                driver.Navigate().GoToUrl(remotePSURL);
                LogCommentImportant(CL, "Navigating to the Remote PS server which is 10.201.96.19");
                driver.Manage().Window.Maximize();
                driver.FindElement(By.Id("element_1")).Click();
                driver.FindElement(By.Id("element_1")).Clear();
                driver.FindElement(By.Id("element_1")).SendKeys(boxID);
                LogCommentImportant(CL, "Entering the BOX ID");
                CL.IEX.Wait(2);
                LogCommentImportant(CL, "Selecting the API in this case Reset MPIN");
                SelectElement APIselector = new SelectElement(driver.FindElementById("element_2"));
                APIselector.SelectByIndex(3);
                CL.IEX.Wait(2);
                LogCommentImportant(CL, "Selecting the Preferred Language which is en");
                SelectElement Languageselector = new SelectElement(driver.FindElementById("language"));
                Languageselector.SelectByValue("en");
                CL.IEX.Wait(2);
                LogCommentImportant(CL, "Selecting the LAB which is UM");
                SelectElement Labselector = new SelectElement(driver.FindElementById("lab"));
                Labselector.SelectByIndex(3);
                CL.IEX.Wait(5);
                driver.FindElementById("submit_form").Click();
                CL.IEX.Wait(10);
                driver.Quit();
            }
            catch (Exception ex)
            {
                LogCommentFailure(CL, "Failed to reset the Pin from Remote PS server. Reason :" + ex.Message);
				driver.Quit();
            }
            isPinChanged = false;

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
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANGE PIN CODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Change PIN code state.");
            }
            res = CL.EA.EnterPIN(defaultPin, "CHANGE PIN CODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to Change PIN code state. Enterd Pin code is incorrect");
            }
            string title="";
            res=CL.IEX.MilestonesEPG.GetEPGInfo("title",out title);
            if (title.ToUpper() == "CONTINUE")
            {
                LogCommentImportant(CL, "We are able to Enter the Default PIN which indicates that the MPIN is RESET to the Default");
            }
            else
            {
                FailStep(CL,res,"Failed to reset the Default pin from the Remote PS server");
            }

            PassStep();
        }
    }
    #endregion
   
    #endregion

    #region PostExecute
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        if (isPinChanged)
        {
            res = CL.EA.STBSettings.ChangePinCode(Constants.newPIN,defaultPin );
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to change pin code");
            }
        }
    }
    #endregion
}