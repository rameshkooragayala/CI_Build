using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace IEX.HealthCheck
{
    /*
     * This class is a container that reads and gives access (via state name) to all the expected milestones per state
     **/

    internal class StatesManager
    {
        private Dictionary<string, State> allStates = new Dictionary<string, State>();
        private string filePath;

        internal StatesManager(string filePath)
        {
            this.filePath = filePath;
            CreateStatesList();
        }

        internal State GetStateByName(string name)
        {
            if (!allStates.ContainsKey(name))
                return null;
            return allStates[name];
        }

        private void CreateStatesList()
        {
            Assembly _assembly = Assembly.GetEntryAssembly();
            Stream configFile = null;
            StreamReader reader = null;
            try
            {
                configFile = _assembly.GetManifestResourceStream(this.filePath);
                reader = new StreamReader(configFile);
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "" || line.StartsWith("#")) continue;

                    string[] state = line.Split('=');
                    State currentState = new State();
                    try
                    {
                        currentState.Name = state[0].Trim();
                        currentState.ExpectedMS = ParseInput(state[1].Trim());
                        allStates.Add(currentState.Name, currentState);
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception occured While Parsing Expected Milestones Input : " + line + " " + ex.Message);
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (configFile != null)
                {
                    configFile.Close();
                }
            }
        }

        internal Dictionary<string, string> ParseInput(string milestone)
        {
            Dictionary<string, string> milestoneDataDictionary = new Dictionary<string, string>();

            milestone = milestone.Remove(0, 1);
            milestone = milestone.Remove(milestone.Length - 1, 1);

            string[] rowData = milestone.Split(',');

            foreach (string dataMember in rowData)
            {
                string[] keyVal = dataMember.Split(':');

                milestoneDataDictionary.Add(keyVal[0].Replace("\"", ""), keyVal[1].Replace("\"", ""));
            }

            return milestoneDataDictionary;
        }
    }
}