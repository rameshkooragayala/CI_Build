using System.Collections.Generic;
using System.Linq;

namespace IEX.HealthCheck
{
    /**
     * This class represents a navigation in 2 formats
     * full path or step by step (state by state)
     *
     **/

    internal class NavigationEntry
    {
        private string fullPath;
        private List<string> states = new List<string>();
        private int currentIndex = 0;

        public int CurrentIndex
        {
            get { return currentIndex; }
            set { currentIndex = value; }
        }

        internal NavigationEntry(string fullPath)
        {
            this.fullPath = fullPath;
            ParseStatesFromEntry();
        }

        private void ParseStatesFromEntry()
        {
            //trick: to enable splitting by slash so that double slashes won't disturb, they are replaced by another str before the split and restored after it
            const string DOUBLE_SLASH = "//";
            const string TEMP_STR = "$";
            const char POP_OUT_SEPURETOR = '-';

            string editPath = fullPath.Replace(DOUBLE_SLASH, TEMP_STR);
            string[] path = editPath.Split('/');

            for (int i = 0; i < path.Length; i++)
            {
                states.Add(path[i].Replace(TEMP_STR, DOUBLE_SLASH).Replace(POP_OUT_SEPURETOR, '/'));
            }
        }

     
        internal string GetNextState()
        {
            if (currentIndex >= states.Count)
                return null;
            return states.ElementAt(currentIndex++);
        }

        internal string GetFullPath()
        {
            return this.fullPath;
        }
    }
}