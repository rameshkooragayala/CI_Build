using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace IEX.Tests.Engine
{
    internal class XmlInputParser
    {
        #region RestoreTests
        public List<TestParameters> RestoreTests(string path, ref bool debugMode, ref bool stopOnFailure, ref bool waitAfterMount, ref int testDuration)
        {
            FileInfo fi = new FileInfo(path);
            if (!fi.Exists)
            {
                return null;
            }

            XDocument doc = XDocument.Load(path);
            var tests = doc.Descendants("Test");
            if (!tests.Any())
            {
                throw new Exception("Invalid XML File: <Test> tag must appear at least once");
            }

            List<TestParameters> testParameters = tests.Select(testElement => new TestParameters(testElement.Value)).ToList();

            //get Debug mode from <DebugMode> tag
            var _debugModeElement = doc.Descendants("DebugMode");
            var _debugMode = _debugModeElement.FirstOrDefault();
            if (_debugMode != null)
            {
                debugMode = Convert.ToBoolean(_debugMode.Value);
            }
            else
            {
                throw new Exception("Invalid XML File: <DebugMode> tag must appear once");
            }

            //get stop on failure from <StopOnFailure> tag
            var _stopOnfailureElement = doc.Descendants("StopOnFailure");
            var _stopOnfailure = _stopOnfailureElement.FirstOrDefault();
            if (_stopOnfailure != null)
                stopOnFailure = Convert.ToBoolean(_stopOnfailure.Value);
            else
            {
                throw new Exception("Invalid XML File: <StopOnFailure> tag must appear once");
            }

            //get wait after mount from <WaitAfterMount> tag
            var _waitAfterMountElement = doc.Descendants("WaitAfterMount");
            var _waitAfterMount = _waitAfterMountElement.FirstOrDefault();
            if (_waitAfterMount != null)
            {
                waitAfterMount = Convert.ToBoolean(_waitAfterMount.Value);
            }
            else
            {
                throw new Exception("Invalid XML File: <WaitAfterMount> tag must appear once");
            }

            //get test duration from <ExpectedDuration> tag
            var _testDurationElement = doc.Descendants("ExpectedDuration");
            var _testDuration = _testDurationElement.FirstOrDefault();
            if (_testDuration != null)  //not mandatory those not throwing exception if not exists
            {
                testDuration = Convert.ToInt32(_testDuration.Value);
            }

            return testParameters;
        }
        #endregion

        #region TestParameters
        internal class TestParameters
        {
            public string args { get; set; }
            public Dictionary<string, string> argsDictionary = new Dictionary<string, string>();

            public TestParameters(string value)
            {
                args = value;
                ParseArgsToDictionary();
            }

            public string ParseDictionaryToArgs()
            {
                string arguments = "";
                const int maxNumOfClients = 4;


                if (argsDictionary.ContainsKey("-t"))
                {
                    arguments += "-t " + argsDictionary["-t"] + " ";
                }

                if (argsDictionary.ContainsKey("-g"))
                {
                    arguments += "-g " + argsDictionary["-g"] + " ";
                }

                for (int i = 1; i <= maxNumOfClients; i++)
                {
                    if (argsDictionary.ContainsKey("-s" + i.ToString()) && argsDictionary["-s" + i.ToString()] != "None" && argsDictionary["-s" + i.ToString()] != "")
                    {

                        arguments += "-s " + argsDictionary["-s" + i.ToString()] + " ";
                    }
                    else
                    {
                        return arguments;
                    }

                }
                return arguments;
            }

            private void ParseArgsToDictionary()
            {
                string[] arguments = args.Split(' ');
                int clientsCounter = 1;
                for (int i = 0; i < arguments.Length; i++)
                {
                    if (arguments[i] == "-t")
                    {
                        argsDictionary["-t"] = arguments[i + 1];
                    }
                    if (arguments[i] == "-g")
                    {
                        argsDictionary["-g"] = arguments[i + 1] + " " + arguments[i + 2];
                    }
                    if (arguments[i] == "-s")
                    {
                        argsDictionary["-s" + clientsCounter.ToString()] = arguments[i + 1] + " " + arguments[i + 2];
                        clientsCounter++;
                    }

                }
            }
        }
        #endregion
    }
}
