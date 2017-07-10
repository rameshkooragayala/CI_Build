using System;
using System.Collections.Generic;
using IEX.Tests.Engine;

namespace IEX.HealthCheck
{
    /**
     * This class is a service class that triggers the listening on the log
     * and then parses the results
     **/

    internal static class MilestonesChecker
    {
        private const string SEPARATOR_AS_IS = "@#$";
        private const string SEPARATOR_ESCAPE_REGEX = "@#\\$";
        private const string MSG_KEY_NOT_FOUND = "Key Was Not Found";
        private const string MSG_EMPTY_VALUE = "Empty Value";

        internal static void Start(_Platform platform, string tag)
        {
            platform.IEX.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.Start, tag, IEXGateway.DebugDevice.Udp);
        }

        internal static List<KeyValuePair<string, MilestoneError>> Finish(State stateToListen, _Platform platform, IEXGateway._IEXResult resFromNav, string tag)
        {
            string logPart = null;

            platform.IEX.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.End, tag, IEXGateway.DebugDevice.Udp);
            platform.IEX.Debug.ImportLogPart(tag, out logPart, IEXGateway.DebugDevice.Udp);
            List<KeyValuePair<string, MilestoneError>> notFound = new List<KeyValuePair<string, MilestoneError>>();

            if (!resFromNav.CommandSucceeded)
            {
                //navigation failed -> check only navigation MS .....
            }

            bool foundKey = false;
            bool matchValue = false;
            MilestoneError error;
            foreach (string key in stateToListen.ExpectedMS.Keys)
            {
                foundKey = false;
                if (!logPart.Contains(key + SEPARATOR_AS_IS))
                {
                    if (!resFromNav.CommandSucceeded)
                    {
                        error = new MilestoneError("Check Your Configuration File", ErrorType.Milestone_Not_Found);
                    }
                    else
                    {
                        error = new MilestoneError("Milestone Key Might Have Changed", ErrorType.Milestone_Not_Found);
                    }
                    notFound.Add(new KeyValuePair<string, MilestoneError>(key, error));
                    continue;
                }
                string expectedVal = stateToListen.ExpectedMS[key];
                string searchPlace = logPart.Substring(logPart.IndexOf(key + SEPARATOR_AS_IS));
                string foundVal = null;

                int valStart = 0;
                int valLen = 0;
                while (searchPlace.Contains(key + SEPARATOR_AS_IS))
                {
                    foundKey = true;
                    matchValue = false;
                    valStart = searchPlace.IndexOf(key + SEPARATOR_AS_IS) + key.Length + SEPARATOR_AS_IS.Length;
                    valLen = expectedVal.Length;
                    if (valLen == 0)
                    {
                        // handle valLen = 0  !!
                        int valEnd = searchPlace.IndexOf(SEPARATOR_AS_IS, valStart);
                        if (valEnd < valStart) valEnd = valStart + 1;
                        valLen = valEnd - valStart;
                    }
                    try
                    {
                        if (searchPlace.Length > valStart + valLen - 1)
                        {
                            foundVal = searchPlace.Substring(valStart, valLen);
                        }
                        else
                        {
                            foundVal = "";
                        }
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine(e.Message);
                    }
                    if (foundVal.Equals(expectedVal) || (expectedVal.Length == 0 && foundVal.Length > 0))
                    {
                        matchValue = true;
                        break;
                    }
                    searchPlace = searchPlace.Substring(valStart);
                }

                //using found key found value and matchValue we can write the result
                if (!foundKey)
                {
                    error = new MilestoneError("Search Engine Has Failed", ErrorType.Execution_Error);
                    notFound.Add(new KeyValuePair<string, MilestoneError>(key, error));
                    continue;
                }
                if (!matchValue)
                {
                    error = new MilestoneError("Milestone Value Mismatch", ErrorType.Wrong_Milestone);
                    error.ActualValue = foundVal;
                    notFound.Add(new KeyValuePair<string, MilestoneError>(key, error));
                }
            }
            return notFound;
        }
    }
}