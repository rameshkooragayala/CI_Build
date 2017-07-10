using System.Collections.Generic;

namespace IEX.HealthCheck
{
    /*
     * This class represents a state data object
     */

    internal class State
    {
        internal string Name { get; set; }

        internal bool isValid { get; set; }

        internal int CounterVisits { set; get; }

        internal Dictionary<string, string> ExpectedMS { get; set; }
    }
}