using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IEX.HealthCheck
{
    /***
     * This class holds all the navigations paths by parsing each line of the input file as a single navigation path object
     * */

    internal class NavigationsManager
    {
        private LinkedList<string> actionsList = new LinkedList<string>();
        private LinkedList<NavigationEntry> navigationsList = new LinkedList<NavigationEntry>();
        private LinkedList<string> selectItemList = new LinkedList<string>();
        private int currentAction = 0;

        public int CurrentAction
        {
            get { return currentAction; }
            set { currentAction = value; }
        }
        private int currentNav = 0;

        public int CurrentNav
        {
            get { return currentNav; }
            set { currentNav = value; }
        }

        private int currentItem = 0;

        internal NavigationsManager(string inputFilePath)
        {
            CreateNavigationsList(inputFilePath);
        }

        internal string GetNextAction()
        {
            if (currentAction >= actionsList.Count)
                return null;
            return actionsList.ElementAt(currentAction++);
        }

        internal NavigationEntry GetNextNavigation()
        {
            if (currentNav >= navigationsList.Count)
                return null;
            return navigationsList.ElementAt(currentNav++);
        }

        internal string GetNextSelectMenuItem()
        {
            if (currentItem >= selectItemList.Count)
                return null;
            return selectItemList.ElementAt(currentItem++);
        }

        private void CreateNavigationsList(string inputFilePath)
        {
            Assembly _assembly = Assembly.GetEntryAssembly();
            Stream navigationFile = null;
            StreamReader reader = null;

            try
            {
                navigationFile = _assembly.GetManifestResourceStream(inputFilePath);
                reader = new StreamReader(navigationFile);
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (line == "" || line.StartsWith("#")) continue;

                    if (line.StartsWith("Navigation:"))
                    {
                        actionsList.AddLast("Navigation");
                        string command = line.Remove(0, line.IndexOf(":") + 2);
                        navigationsList.AddLast(new NavigationEntry(command));
                        System.Console.WriteLine("Navigation Read -> " + command);
                    }
                    else if (line.StartsWith("SelectMenuItem:"))
                    {
                        actionsList.AddLast("SelectMenuItem");
                        string command = line.Remove(0, line.IndexOf(":") + 2);
                        selectItemList.AddLast(command);
                        System.Console.WriteLine("SelectMenuItem Read -> " + command);
                    }
                    else
                    {
                        System.Console.WriteLine("Command Not Found : " + line);
                        System.Console.WriteLine("Each Entry Must Begin With 'Navigation:' or 'SelectMenuItem:' Command");
                        continue;
                    }
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (navigationFile != null)
                {
                    navigationFile.Close();
                }
            }
        }
    }
}