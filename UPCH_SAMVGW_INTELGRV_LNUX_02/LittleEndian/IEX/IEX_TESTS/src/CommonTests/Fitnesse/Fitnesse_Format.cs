/// <summary>
///  Script Name : Fitness.cs
///  Test Name   : Fitness
///  TEST ID     : 
///  QC Version  : 2
///  Variations from QC:none
/// ----------------------------------------------- 
///  Modified by : Avinash Budihal
/// </summary>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;
using IEX.Tests.Utils;
using System.Diagnostics;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Xml;
using System.Net;
using System.Xml.Linq;

[Test(" Fitnesse")]
public class Fitnesse_Format : _Test
{

    [ThreadStatic]
    static _Platform CL;
    static string ProcessPath = "";
    static string ProcessPathDrive = "";
    static string StartCommand = "";
    static string StartTestCommand = "";
    static string TargetResultPath = "";
    static string SourceResultPath = "";
    static string CSSPath = "";
    static string suiteName = "";
    static string TimeForuuiterun = "0";
    static string KillAntCommand = "";
    static string mountCommand = "";

    //Shared members between steps

    private const string PRECONDITION_DESCRIPTION = "Fetch the ini values.";
    private const string STEP1_DESCRIPTION = "Step 1: Run the Fitnesse Suite";
    private const string STEP2_DESCRIPTION = "Step 2: Retrieve results and store in target path";


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

            //Fetching the path of build xml to launch the test suite
            ProcessPathDrive = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ProcessPathDrive");
            if (ProcessPathDrive == "")
            {
                FailStep(CL, "Failed to get the ProcessPathDrive from Project ini");
            }
            else
            {
                LogCommentInfo(CL, "ProcessPathDrive fetched from Test ini " + ProcessPathDrive);
            }
            //Fetching the path of build xml to launch the test suite
            ProcessPath = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ProcessPath");
            if (ProcessPath == "")
            {
                FailStep(CL, "Failed to get the ProcessPath from test ini");
            }
            else
            {
                LogCommentInfo(CL, "ProcessPath fetched from Test ini " + ProcessPath);
            }
            //Fetching the path of build xml to launch the test suite
            StartTestCommand = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "StartTestCommand");
            if (StartTestCommand == "")
            {
                FailStep(CL, "Failed to get the StartTestCommand from test ini");
            }
            else
            {
                LogCommentInfo(CL, "StartTestCommand fetched from Test ini " + StartTestCommand);
            }

            //Fetching the path of build xml to kill the ant
            KillAntCommand = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "EndTestCommand");
            if (KillAntCommand == "")
            {
                FailStep(CL, "Failed to get the KillAnt from test ini");
            }
            else
            {
                LogCommentInfo(CL, "KillAnt fetched from Test ini " + KillAntCommand);
            }

            //Fetching url to launch test suite
            suiteName = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Suite_Name");
            if (suiteName == "")
            {
                FailStep(CL, "Failed to get the suite name from Test ini");
            }
            else
            {
                LogCommentInfo(CL, "Suite name fetched from Test ini " + suiteName);
            }

            //Fetching time for suite to run
            TimeForuuiterun = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "TimeForSuiteRun");
            if (suiteName == "")
            {
                FailStep(CL, "Failed to get TimeForSuiteRun from Test ini");
            }
            else
            {
                LogCommentInfo(CL, "TimeForSuiteRun fetched from Test ini " + TimeForuuiterun);
            }
            //Fetch mount command to fetch from environment.ini
            mountCommand = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "MountCommand_Fitnesse");
            if (string.IsNullOrEmpty(mountCommand))
            {
                FailStep(CL, "Failed to fetch mountCommand from Environment.ini");
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

            // Launch ant
            Process myProcess = new Process();
            myProcess = CL.EA.UI.OTA.StartProcess_Fitness(@"" + ProcessPathDrive + @"
            cd \
             " + ProcessPath + @"
             " + StartTestCommand);
            LogCommentInfo(CL, "Router launched");

           /*  while (count < 37)
              {
                  LogCommentInfo(CL, "Entered inside while loop"); 

                  // fetching logs till end of stream
                  LogCommentInfo(CL, myProcess.StandardOutput.ReadLine());
                  count++;
              }
              LogCommentInfo(CL, "While loop completed"); */

            // Reboot CPE
            LogCommentInfo(CL, "mounting the image to STB");
            CL.IEX.SendPowerCycle("OFF");
            CL.IEX.Wait(5);
            CL.IEX.SendPowerCycle("ON");

            //mounting STB and check for OTA download
            try
            {
                CL.EA.UI.Mount.WaitForPrompt(false);
                CL.EA.UI.Mount.SendMountCommand(true, mountCommand: mountCommand);

            }
            catch (Exception ex)
            {
                LogCommentInfo(CL, "Failed to mount the box:" + ex.Message);
            }
           

            //res = CL.EA.MountGw(EnumMountAs.FORMAT);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed to power cycle STB to live");
            //}
            //  CL.IEX.Wait(120);

            //Wait for some time for STB to come to standby mode 
            res = CL.IEX.Wait(120);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for image to load ");
            }

             //Exit from stand by
            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to bring the box out of Standby");
            }

            //check for live state
            bool liveState1 = CL.EA.UI.Utils.VerifyState("LIVE", 20);
            if (!liveState1)
            {
                FailStep(CL, res, "Unable to verify the live state");

            }

            //Launch Main Menu
            string timeStamp = "";

            res = CL.IEX.SendIRCommand("MENU", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }
            CL.IEX.Wait(5);
			
			 //Dismiss Main Menu
            res = CL.IEX.SendIRCommand("MENU", 1, ref timeStamp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to send IR Main Menu");
            }

            // To run suite with selenium
            FirefoxDriver driver = new FirefoxDriver();
            try
            {
                StringBuilder verificationErrors = new StringBuilder();
                string suiteURL = "http://localhost:8045/" + suiteName;
                driver.Navigate().GoToUrl(suiteURL);
                driver.Manage().Window.Maximize();
                driver.FindElement(By.XPath("//html/body/nav/ul/li[1]/a")).Click();

            }
            catch (Exception ex)
            {
                LogCommentInfo(CL, "Failed to execute Suite: exception - " + ex.Message);
            }

            CL.IEX.Wait(Convert.ToInt32(TimeForuuiterun));
            driver.Close();

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

            //Fetch Result path in CI
            string TargetPath = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "LogDirectory");

            TargetResultPath = TargetPath + @"\";

            if (string.IsNullOrEmpty(TargetResultPath))
            {
                LogCommentFailure(CL, "Failed to fetch LogDirectory from Environment.ini");
            }

            //Fetch Fitnesse Result path
            SourceResultPath = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SourceResultPath");
            if (SourceResultPath == "")
            {
                LogCommentFailure(CL, "Failed to get the SourceResultPath from test ini");
            }
            else
            {
                LogCommentInfo(CL, "SourceResultPath fetched from Test ini " + SourceResultPath);
            }

            // Css_Path
            //Fetch path for CSS
            CSSPath = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Css_Path");
            if (CSSPath == "")
            {
                LogCommentFailure(CL, "Failed to get the css from test ini");
            }
            else
            {
                LogCommentInfo(CL, "CSS from Test ini " + CSSPath);
            }

            List<TestResult> tests = new List<TestResult>();
            // Parse xml from source path to CI path
            try
            {
                string fitNesseResult = @SourceResultPath;
                string targetPath = @TargetResultPath;

                string path = fitNesseResult + suiteName;

                LogCommentInfo(CL, "Source path for fitnesse result with suite name" + path);

                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] files = di.GetFiles("*.xml");

                if (files.Count() == 0)
                {
                    LogCommentInfo(CL, "No files are there in " + path + " ");
                }

                FileInfo suitefile = (from f in files
                                      orderby f.LastWriteTime descending
                                      select f).First();

                //  Store suite XML in target location
                string des = targetPath + suitefile.Name;
                // suitefile.Name = suitefile.Name;
                try
                {
                    suitefile.CopyTo(des);
                    LogCommentInfo(CL, "Files are copied to target path - " + des);
                }
                catch (Exception ex)
                {
                    FailStep(CL, "Fail to copy the file in target path-" + des + "Exception-" + ex);
                }


                string temp = path + @"\" + suitefile.Name;
                XmlDocument doc = new XmlDocument();
                doc.Load(temp);

                XmlNodeList nodes = doc.DocumentElement.SelectNodes("/suiteResults/pageHistoryReference");



                foreach (XmlNode node in nodes)
                {
                    TestResult test = new TestResult();

                    test.testName = node.SelectSingleNode("name").InnerText;
                    test.exceptions = Convert.ToInt32(node.SelectSingleNode("counts/exceptions").InnerXml);
                    test.wrong = Convert.ToInt32(node.SelectSingleNode("counts/wrong").InnerXml);
                    if (test.testName.Contains("Test"))
                    {
                        tests.Add(test);
                    }
                }

                foreach (TestResult test in tests)
                {
                    try
                    {
                        string testpath = fitNesseResult + test.testName;
                        DirectoryInfo dir = new DirectoryInfo(testpath);
                        FileInfo[] file = dir.GetFiles("*.xml");
                        FileInfo fil = (from f in file
                                        orderby f.LastWriteTime descending
                                        select f).First();


                        string tp = testpath + @"\" + fil.Name;
                        string read = File.ReadAllText(tp);


                        string destinat = targetPath + fil.Name;
                        try
                        {
                            System.IO.File.WriteAllText(destinat, read);
                        }
                        catch (Exception ex)
                        {
                            LogCommentFailure(CL, "Fail to copy the xml file to target path - " + destinat);
                        }

                        string decoded = WebUtility.HtmlDecode(read).Replace("&", "&amp;").Replace("&lt", "<").Replace("&gt", ">").Replace("&quot", "\"").Replace("&apos", "'");
                        XmlDocument dc = new XmlDocument();
                        dc.LoadXml(decoded);
                        XmlNode content = dc.DocumentElement.SelectSingleNode("/testResults/result/content");
                        string xml = content.OuterXml;
                        string startHtml = "<!DOCTYPE html><html><head><link rel=\"stylesheet\" type=\"text/css\" href=\"css/fitnesse_wiki.css\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"css/fitnesse_pages.css\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"css/fitnesse_straight.css\" /><link rel=\"stylesheet\" type=\"text/css\" href=\"css/wysiwyg.css\" media=\"screen\"/><link rel=\"stylesheet\" type=\"text/css\" href=\"css/question.mark.css\" /><link rel=\"wysiwyg.base\" href=\"/\" /><link rel=\"wysiwyg.stylesheet\" type=\"text/css\" href=\"css/fitnesse.css\" media=\"screen\" /><link rel=\"wysiwyg.stylesheet\" type=\"text/css\" href=\"css/editor.css\" media=\"screen\" /><link rel=\"wysiwyg.stylesheet\" type=\"text/css\" href=\"css/custom.css\" media=\"screen\" /></head><body>";
                        string endHtml = "</body></html>";
                        string resultHtml = startHtml + xml + endHtml;
                        string destination = targetPath + test.testName + ".html";
                        try
                        {
                            System.IO.File.WriteAllText(destination, resultHtml);
                        }
                        catch (Exception ex)
                        {
                            LogCommentFailure(CL, "Fail to copy the html file in target path-" + destination + "Exception-" + ex);
                        }

                    }
                    catch (Exception e)
                    {
                        LogCommentFailure(CL, "Fail to convert xml to HTML");
                    }
                }
                StringBuilder html = new StringBuilder();
                html.Append("<table border = '1'>");
                html.Append("<tr>");
                html.Append("<th>");
                html.Append("Test Name");
                html.Append("</th>");
                html.Append("<th>");
                html.Append("Status");
                html.Append("</th>");
                html.Append("<th>");
                html.Append("Logs");
                html.Append("</th>");
                html.Append("</tr>");

                foreach (TestResult test in tests)
                {
                    html.Append("<tr>");

                    html.Append("<td>");
                    html.Append(test.testName);
                    html.Append("</td>");



                    if (test.wrong > 0 || test.exceptions > 0)
                    {
                        html.Append("<td style=\"color:red\">");
                        html.Append("Fail");
                        html.Append("</td>");
                    }
                    else
                    {
                        html.Append("<td style=\"color:blue\">");
                        html.Append("Pass");
                        html.Append("</td>");
                    }


                    html.Append("<td>");
                    html.Append("<a href=\"" + test.testName + ".html\" style=\"color:blue\">Click to see the logs</a>");
                    html.Append("</td>");

                    html.Append("</tr>");
                }

                html.Append("</table>");

                string destinations = targetPath + suiteName + ".html";
                try
                {
                    System.IO.File.WriteAllText(destinations, html.ToString());
                }
                catch (Exception ex)
                {
                    LogCommentFailure(CL, "Fail to copy the html file in target path-" + destinations + "Exception-" + ex);
                }


                // Copy the files and overwrite destination files if they already exist.
                DirectoryInfo dirSrc = new DirectoryInfo(CSSPath);
                string dicrectoryPath = targetPath + @"css\";
                DirectoryInfo diry = Directory.CreateDirectory(dicrectoryPath);
                FileInfo[] filres = dirSrc.GetFiles();
                foreach (FileInfo file in filres)
                {
                    // Create css folder in particular location

                    string destDirName = targetPath + @"css\";
                    string temppath = Path.Combine(destDirName, file.Name);
                    try
                    {
                        file.CopyTo(temppath, false);
                    }
                    catch (Exception ex)
                    {
                        LogCommentFailure(CL, "Fail to copy the CSS file in target path-" + temppath + "Exception-" + ex + " " + CSSPath);
                    }
                }
            }

            catch (System.IO.IOException e)
            {
                LogCommentFailure(CL, e.Message);
            }

            int failecount = 0;
            int totalcount = 0;
            foreach (TestResult tsRes in tests)
            {
                totalcount++;
                if (tsRes.exceptions > 0 || tsRes.wrong > 0)
                {
                    failecount++;
                }
            }
            if (failecount > 0)
            {
                FailStep(CL, failecount + "/" + totalcount + " tests failed");
            }

            PassStep();

        }
    }
    #endregion

    #endregion
    #region PostExecute

    public override void PostExecute()
    {
        // Kill the ant
        Process myProcess = new Process();
        myProcess = CL.EA.UI.OTA.StartProcess_Fitness(@"" + ProcessPathDrive + @"
            cd \
             " + ProcessPath + @"
             " + KillAntCommand);
        /*int count = 0;
        while (count < 34)
        {
            // fetching logs till end of stream
            LogCommentInfo(CL, myProcess.StandardOutput.ReadLine());
            count++;
        }
		*/
        // Delete a directory and all subdirectories with Directory static method...

        SourceResultPath = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SourceResultPath");
        if (SourceResultPath == "")
        {
            LogCommentFailure(CL, "Failed to get the SourceResultPath from test ini");
        }
        else
        {
            LogCommentInfo(CL, "SourceResultPath fetched from Test ini " + SourceResultPath);
        }

        if (System.IO.Directory.Exists(SourceResultPath))
        {
            try
            {
                System.IO.Directory.Delete(SourceResultPath, true);
                LogCommentInfo(CL, "Results are deleted from fitnesse source path");
            }

            catch (System.IO.IOException e)
            {
                LogCommentInfo(CL, e.Message);
            }
        }
		
		var firefoxDriverProcesses = Process.GetProcesses().
                                 Where(pr => pr.ProcessName == "firefox");
        foreach (var process in firefoxDriverProcesses)
        {
            process.Kill();
        }
    }

    #endregion PostExecute

    class TestResult
    {
        public string testName;
        public string status;
        public string testLink;
        public int exceptions;
        public int wrong;
    }

}
