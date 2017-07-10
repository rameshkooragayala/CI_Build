using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IEX.HealthCheck
{
    /**
     * This class get the expected milestones input to validate
     * and converts that to a dictionary 
     * 
     **/
    internal class ExpectedMilestonesParser
    {
        //string milestone;
        //Dictionary<string, string> milestoneData;

        //internal MilestoneParser(string milestone)
        //{
        //    this.milestone = milestone;
        //    this.milestoneData = StrToDic(milestone);
        //}

        internal static Dictionary<string, string> Parse(string milestone)
        {
            Dictionary<string, string> milestoneDataDictionary = new Dictionary<string, string>();

            try
            {
                milestone = milestone.Remove(0, 1);
                milestone = milestone.Remove(milestone.Length - 1, 1);

                string[] rowData = milestone.Split(',');

                foreach (string dataMember in rowData)
                {
                    string[] keyVal = dataMember.Split(':');

                    milestoneDataDictionary.Add(keyVal[0].Replace("\"", ""), keyVal[1].Replace("\"", ""));
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Exception occured While Parsing Expected Milestones Input : " + milestone + " " + ex.Message);
            }
            return milestoneDataDictionary;
        }
    }
}
