using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using System.IO;
using System.Xml;

namespace IEX.HealthCheck
{
    /*
     * This class handles both the output file (summary + details) and the IEX log (Imhotep)
     * and collects data for the summary
     **/

    internal class LogManager
    {
        private string ROOT_DIRECTORY_TO_OUTPUT = @"Health Check Input";
        private string INITIAL_OUTPUT_FILENAME = @"\temp.txt"; //temp file before summary
        private string FINAL_OUTPUT_FILENAME = @"\outPut.txt"; //for summary + details
        private string FINAL_OUTPUT_XML_FILENAME = @"\healthcheck.xml";
        private string FULL_PATH_TO_INITIAL_OUTPUT;
        private string FULL_PATH_TO_FINAL_OUTPUT;
        private string FULL_PATH_TO_FINAL_XML_OUTPUT;

        _Platform platform;
        State currentDestination;

        Dictionary<string, State> failedStates;
        Dictionary<string, List<KeyValuePair<string, string>>> badMSPerState;
        FileStream initialOutputFile;
        StreamWriter writer;

        int totalNavigations = 0;

        //int failedNavigations = 0;
        //we use the xml document to collect all the data !
        XmlDocument navigationData;

        XmlNode navigationRoot;
        XmlNode summaryRoot;
        XmlNode currentNavigation;
        XmlNode currentDest;

        internal LogManager(_Platform platform, string finelOutputfolder = @"Health Check Input")
        {
            ROOT_DIRECTORY_TO_OUTPUT = finelOutputfolder;
            FULL_PATH_TO_INITIAL_OUTPUT = ROOT_DIRECTORY_TO_OUTPUT + INITIAL_OUTPUT_FILENAME;
            FULL_PATH_TO_FINAL_OUTPUT = ROOT_DIRECTORY_TO_OUTPUT + FINAL_OUTPUT_FILENAME;

            if (!System.IO.Directory.Exists(ROOT_DIRECTORY_TO_OUTPUT))
            {
                System.IO.Directory.CreateDirectory(ROOT_DIRECTORY_TO_OUTPUT);
            }
            this.platform = platform;
            failedStates = new Dictionary<string, State>();
            badMSPerState = new Dictionary<string, List<KeyValuePair<string, string>>>();
            initialOutputFile = new FileStream(FULL_PATH_TO_INITIAL_OUTPUT, FileMode.Create, FileAccess.ReadWrite);
            writer = new StreamWriter(initialOutputFile);
            navigationData = new XmlDocument();

            navigationData.LoadXml(@"<Root><Navigations/><Summary/></Root>");
            navigationRoot = navigationData.SelectSingleNode("Root/Navigations");
            summaryRoot = navigationData.SelectSingleNode("Root/Summary");
        }

        internal void StartNavigation(string fullPath)
        {
            string startAnnouncement = "Navigation To Path :" + fullPath;
            platform.IEX.StartHideFailures(startAnnouncement, true, colour: "Blue");
            writer.WriteLine(startAnnouncement);

            totalNavigations++;
            currentNavigation = navigationRoot.OwnerDocument.CreateElement("Navigation");
            XmlAttribute path = currentNavigation.OwnerDocument.CreateAttribute("Path");
            path.Value = fullPath;
            navigationRoot.AppendChild(currentNavigation);
            currentNavigation.Attributes.Append(path);
        }

        internal void StartDestination(State destState)
        {
            string startDestAnnouncement = "Starting Navigation To State : " + destState.Name;
            platform.IEX.LogComment(startDestAnnouncement);
            writer.WriteLine("      " + startDestAnnouncement);
            currentDestination = destState;

            currentDest = currentNavigation.OwnerDocument.CreateElement("Dest");
            XmlAttribute name = currentDest.OwnerDocument.CreateAttribute("Name");
            name.Value = destState.Name;
            currentNavigation.AppendChild(currentDest);
            currentDest.Attributes.Append(name);

            foreach (KeyValuePair<string, string> expMilestone in destState.ExpectedMS)
            {
                XmlNode expMS = currentDest.OwnerDocument.CreateElement("MilestoneToCheck");

                XmlAttribute key = currentDest.OwnerDocument.CreateAttribute("Key");
                key.Value = expMilestone.Key;

                XmlAttribute val = currentDest.OwnerDocument.CreateAttribute("Value");
                val.Value = expMilestone.Value;

                currentDest.AppendChild(expMS);
                expMS.Attributes.Append(key);
                expMS.Attributes.Append(val);
            }
            DEBUG();
        }

        internal void DestinationFail(List<KeyValuePair<string, MilestoneError>> badMilestones)
        {
            if (currentDest == null) return;

            XmlAttribute FailDest = currentDest.OwnerDocument.CreateAttribute("Failed");
            currentDest.Attributes.Append(FailDest);

            foreach (KeyValuePair<string, MilestoneError> badMs in badMilestones)
            {
                XmlNode failedMs = currentDest.SelectSingleNode("MilestoneToCheck [@Key='" + badMs.Key + @"']");

                if (failedMs == null) continue;

                XmlAttribute FailMS = currentDest.OwnerDocument.CreateAttribute("Failed");
                failedMs.Attributes.Append(FailMS);

                XmlNode actual = failedMs.OwnerDocument.CreateElement("ActualMs");
                XmlAttribute errorCode = actual.OwnerDocument.CreateAttribute("ErrorCode");
                XmlAttribute errorMessage = actual.OwnerDocument.CreateAttribute("ErrorMessage");
                XmlAttribute actualValue = actual.OwnerDocument.CreateAttribute("ActualValue");

                actualValue.Value = "";
                if (badMs.Value.ActualValue != null)
                {
                    actualValue.Value = badMs.Value.ActualValue;
                }

                errorCode.Value = badMs.Value.ErrorType.ToString();
                errorMessage.Value = badMs.Value.ErrorMsg;

                failedMs.AppendChild(actual);
                actual.Attributes.Append(errorCode);
                actual.Attributes.Append(errorMessage);
                actual.Attributes.Append(actualValue);
            }
        }

        internal void FinishNavigation(bool hasFailures)
        {
            string finishAnnouncement;

            platform.IEX.EndHideFailures();
            if (hasFailures)
            {
                platform.IEX.ForceShowFailure();
                finishAnnouncement = "====== Navigation Failed! ======";
                platform.IEX.LogComment(finishAnnouncement, colour: "Red");
            }
            else
            {
                finishAnnouncement = "====== Navigation Passed ======";
                platform.IEX.LogComment(finishAnnouncement, colour: "Green");
            }
            writer.WriteLine(finishAnnouncement);

            DEBUG();
        }

        internal void FailNavigation(string errorMsg)
        {
            XmlAttribute failNav = currentNavigation.OwnerDocument.CreateAttribute("Failed");
            currentNavigation.Attributes.Append(failNav);
            failNav.Value = errorMsg;
        }

        internal void WriteSummary()
        {
            StreamWriter finalWriter = null;
            FileStream finalLog = null;

            XmlNodeList allNavigations = navigationRoot.SelectNodes("Navigation");
            XmlNodeList failedNavigations = navigationRoot.SelectNodes("Navigation [@Failed]");
            XmlNodeList failedStates = navigationRoot.SelectNodes("Navigation/Dest [@Failed]");

            try
            {
                finalLog = new FileStream(FULL_PATH_TO_FINAL_OUTPUT, FileMode.Create, FileAccess.Write);
                finalWriter = new StreamWriter(finalLog);
                finalWriter.WriteLine("==========  Navigations Summary  ==========");
                finalWriter.WriteLine(failedNavigations.Count + "/" + allNavigations.Count + " Navigation" + ((failedNavigations.Count > 1) ? "s" : "") + " Failed");

                foreach (XmlNode failNav in failedNavigations)
                {
                    finalWriter.WriteLine("\n Navigation  " + failNav.Attributes["Path"].Value + " has Failed");
                    finalWriter.WriteLine(failNav.Attributes["Failed"].Value);
                }

                finalWriter.Write("\n==========  Failed States  ==========");
                foreach (XmlNode failState in failedStates)
                {
                    finalWriter.WriteLine("\nVerification of State " + failState.Attributes["Name"].Value + " Failed");
                    finalWriter.WriteLine("Expected Milestones :");
                    XmlNodeList allMS = failState.SelectNodes("MilestoneToCheck");
                    foreach (XmlNode expMS in allMS)
                    {
                        finalWriter.WriteLine("  " + expMS.Attributes["Key"].Value + " : " + ((expMS.Attributes["Value"].Value.Length < 1) ? "Any Value " : expMS.Attributes["Value"].Value));
                    }
                    finalWriter.WriteLine("Failed Milestones :");
                    XmlNodeList failMS = failState.SelectNodes("MilestoneToCheck [@Failed]");
                    foreach (XmlNode failedMs in failMS)
                    {
                        XmlNode actual = failedMs.SelectSingleNode("ActualMs");

                        finalWriter.WriteLine(" " + failedMs.Attributes["Key"].Value + " : " + ((actual.Attributes["ActualValue"].Value.Length < 1) ? "No Value " : actual.Attributes["ActualValue"].Value));
                        finalWriter.WriteLine("  " + actual.Attributes["ErrorCode"].Value + " : " + actual.Attributes["ErrorMessage"].Value);
                    }
                }
            }
            finally
            {
                if (finalWriter != null) finalWriter.Close();
                if (finalLog != null) finalLog.Close();
            }
            FULL_PATH_TO_FINAL_XML_OUTPUT = ROOT_DIRECTORY_TO_OUTPUT + FINAL_OUTPUT_XML_FILENAME;
            navigationData.Save(FULL_PATH_TO_FINAL_XML_OUTPUT);
        }

        void DEBUG()
        {
            navigationData.Save("TempNavigationData.xml");
        }
    }
}