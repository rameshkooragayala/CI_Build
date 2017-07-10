using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace MultiRunner
{
    public class StateManager
    {
        public List<RunningTest> RestoreState(string path, ref string fileName,ref bool debug,ref bool stopOnfailure,ref bool waitAfterMount)
        {
            FileInfo fi=new FileInfo(path);
            if (!fi.Exists)
            {
                return null;
            }
            XDocument doc = XDocument.Load(path);
            var tests = doc.Descendants("Test");
            if (!tests.Any())
            {
                throw new Exception("Invalid Xml File: <Test> tag must appear at list once ");
            }
            List<RunningTest> runningTests= tests.Select(testElement => new RunningTest(testElement.Value)).ToList();

            

            var executionFileElement = doc.Descendants("FileName");
            var executionFile = executionFileElement.FirstOrDefault();
            if (executionFile != null)
            {
                fileName = executionFile.Value;
            }
            else
            {
                throw new Exception("Invalid Xml File: <FileName> tag must appear once");
            }

            var debugModeElement = doc.Descendants("DebugMode");
            var debugMode = debugModeElement.FirstOrDefault();
            if (debugMode != null)
            {
                debug = Convert.ToBoolean(debugMode.Value);
            }
            else
            {
                throw new Exception("Invalid Xml File: <DebugMode> tag must appear once");
            }

            var stopOnFailureElement = doc.Descendants("stopOnFailure");
            var onFailure = stopOnFailureElement.FirstOrDefault();
            if (onFailure != null)
                stopOnfailure = Convert.ToBoolean(onFailure.Value);

            var waitAfterMountElement = doc.Descendants("waitAfterMount");
            var wait = waitAfterMountElement.FirstOrDefault();
            if (wait != null)
            {
                waitAfterMount = Convert.ToBoolean(wait.Value);
            }
            else
            {
                throw new Exception("Invalid Xml File: <waitAfterMount> tag must appear once");
            }

            return runningTests;

        }

        public void SaveState(string path, List<RunningTest> tests, string filename,  bool debug,  bool stopOnfailure,bool waitAfterMount)
        {

            if (tests.Count < 1)
            {
                MessageBox.Show("There is No tests to save", "Error");
                return;
            }

            XDocument doc = new XDocument(
                // XML declaration
              new XDeclaration("1.0", "utf-8", "yes"),
                // The root element
                new XElement("ExecutionPlan",
                 new XElement("Tests",
                // The Tests elements
                            from t in tests
                            select new XElement("Test",t.ParseDictionaryToArgs())
                          ),
                //End of Tests elements
                          new XElement("FileName",filename),
                          new XElement("DebugMode",debug),
                          new XElement("stopOnFailure",stopOnfailure),
                          new XElement("waitAfterMount",waitAfterMount)
                          
                          
                          )
                          );


            FileInfo fi=new FileInfo(path);
            if (!fi.Exists)
            {
                fi.Create().Dispose();
            }

            doc.Save(path);
            
        }

    }
}
