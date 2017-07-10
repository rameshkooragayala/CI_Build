using System;
using System.Collections.Generic;
using FailuresHandler;

namespace EAImplementation
{
    /// <summary>
    /// Modifications of the options available in the Action Bar screen.
    /// </summary>

    public class NavigateAndHighlight : IEX.ElementaryActions.BaseCommand
    {

        private IEX.ElementaryActions.EPG.SF.UI EPG;

        private String navigationPath;
        private String namedNavigation;
        private String navigationItem;
        private String[] navigationStatesArray;
        private Boolean isCompleteIEXNavigation = true;
        private Dictionary<EnumEpgKeys, String> currentScreenDictionary;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        public NavigateAndHighlight(String navigationPath, Dictionary<EnumEpgKeys, String> dictionary, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this.namedNavigation = navigationPath;
            this._manager = pManager;
            this.currentScreenDictionary = dictionary;
            this.EPG = this._manager.UI;
        }

        protected override void Execute()
        {
            // *** WORKAROUND to navigate in VOD catalog (FLUU) ***
            if (namedNavigation == "STATE:STORE_ADULT_ASSET_FROM_ADULT_CATEGORY")
            {
                string title = "";
                string expItem = "Infideles";
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        EPG.Utils.SendIR("SELECT_UP");
                    }
                    catch { }

                    EPG.Utils.GetEpgInfo("title", ref title);
                    if (title == expItem)
                    {
                        break;
                    }
                }

                EPG.Utils.GetEpgInfo("title", ref title);
                if (title != expItem)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.NavigationFailure, "Failed to navigate to '" + expItem + "'"));
                }
                else
                {
                    return;
                }
            }
            // *** END WORKAROUND (FLUU) ***

            EpgState state = new EpgState();
            EpgState prevState = new EpgState();
            bool sameStateAsPrevious = false;
            string namedNavigationPrefix = "STATE:";
            try
            {
                if (namedNavigation.Contains(namedNavigationPrefix))
                {
                    navigationPath = EPG.Utils.EPGStateMachine.GetNavigationPath(namedNavigation);
                }
                else
                {
                    navigationPath = namedNavigation;
                }
            }
            catch
            {
                EPG.Utils.LogCommentFail("Failed to get the full path for the Named Navigation: " + namedNavigation);
            }
            /*split the given Navigationpath with a delimitter '/'. 
            /*  */

            navigationStatesArray = TrimAndReplaceNavPath(navigationPath);

            int navigationIndex = 0;

            /* For each state in the navigation path, check if its possible to perform IEX navigation. 
            * if yes, continue fetching the next in the Navigation-list. 
            * Else, Navigate till the last possible path and for the remaining part, 
            * call the Highlightoption() API. */

            /*navigationItem can be local - it holds different values according to context !!!*/

            for (int numberOfPossibleNavigations = 0; numberOfPossibleNavigations < navigationStatesArray.Length; numberOfPossibleNavigations++)
            {
                navigationItem = navigationStatesArray[numberOfPossibleNavigations];

                /*Check wehther the current navigation item is a state or not*/

                try
                {
                    state = EPG.Utils.EPGStateMachine.GetState(navigationItem);
                }
                catch
                {
                    EPG.Utils.LogCommentInfo("Exception:: state is not defined for the entry:" + navigationItem);
                    state = null;
                }

                if (state == null)
                {
                    /*If its not a state, make sure that the item navigation is possible
                     * else, We need to call HighlightOption Method with 
                     * its parent Menu Layout type  */
                    bool isIEXNavigation = true;
                    try
                    {                        
                        isIEXNavigation = prevState.Menu.GetNavigationType(navigationStatesArray[numberOfPossibleNavigations]);
                    }
                    catch
                    {
                        EPG.Utils.LogCommentInfo("StandardNavigation is not defined for the item" + navigationItem + " in the EPGStateMachine.Assuming IEX Navigation is True in this case.");
                        isIEXNavigation = true;
                    }

                    if (!isIEXNavigation || sameStateAsPrevious || prevState.IsMultiLineMenu())
                    {
                        NavigateConstructedPath(navigationIndex, numberOfPossibleNavigations, navigationStatesArray);

                        /* Fetch the layout of the current option from the previous screen 
                        * and pass it to the HighlightOption() */
                        HighlightBasedOnTitle(prevState, navigationStatesArray[numberOfPossibleNavigations]);
            
                        if (numberOfPossibleNavigations != navigationStatesArray.Length - 1)
                        {
                            //Send select to enter the state
                            EPG.Utils.SendIR("SELECT");
                        }

                        /*Increment the StartIndex value by one to make it to move to the next navigation in the list*/
                        navigationIndex = numberOfPossibleNavigations + 1;
                    }
                }
                else if (state.IsActivationCriteriaSame(prevState))
                {
                    sameStateAsPrevious = true;
                    bool isPopUp = false;
                    //Navigate the part before
                    NavigateConstructedPath(navigationIndex, numberOfPossibleNavigations, navigationStatesArray);

                    //Highlight the required state entry and enter it
                    HighlightBasedOnTitle(prevState, navigationStatesArray[numberOfPossibleNavigations]);

                    try
                    {
                        foreach (Item item in prevState.Menu.Items)
                        {
                            if (item.Name == navigationItem)
                            {
                                isPopUp = item.IsPopup;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        EPG.Utils.LogCommentInfo("State is not defined in EPG state machine so handling it as a non pop up");
                    }


                    if (numberOfPossibleNavigations != navigationStatesArray.Length - 1 && !isPopUp)
                    {
                        //Send select to enter the state
                        EPG.Utils.SendIR("SELECT");
                    }

                    /*Increment the StartIndex value by one to make it to move to the next navigation in the list*/
                    navigationIndex = numberOfPossibleNavigations + 1;
                }

                prevState = state;

            }

            if (isCompleteIEXNavigation)
            {
                EPG.Utils.EPG_Milestones_Navigate(navigationPath);
            }

            //TODO:: Add validation dictionary later if required

            EPG.Utils.LogCommentInfo("Exiting the NavigateAndHighlight() EA");

        }

        private void NavigateConstructedPath(int navigationIndex, int numberOfPossibleNavigations, string[] navigationStatesArray)
        {
            isCompleteIEXNavigation = false;

            /* Constructing the Navigation path to pass it to EPG_Milestones_Navigate() API.*/
            string navigationItem = navigationStatesArray[navigationIndex];

            /*The first entry is store before the loop so that it is easy to append slash*/
            string newNavigationPath = navigationItem;

            /*Constructing IEX navigating path for the items that will use IEX navigate*/
            for (int j = navigationIndex + 1; j < numberOfPossibleNavigations; j++)
            {
                navigationItem = navigationStatesArray[j];

                newNavigationPath = newNavigationPath + "/" + navigationItem;
            }

            /*EDGE CASE - Navigate using IEX Navigate call only if there is at 
             * least one Navigation possible in the list of Navigations*/

            if (navigationIndex != numberOfPossibleNavigations)
            {
                EPG.Utils.EPG_Milestones_Navigate(newNavigationPath);
            }
        }

        private void HighlightBasedOnTitle(EpgState state, string navigationState)
        {
            String dictionaryValue = "";
            Dictionary<EnumEpgKeys, String> parentScrnDictionary = new Dictionary<EnumEpgKeys, String>();

            try
            {
                dictionaryValue = EPG.Utils.EPGStateMachine.GetDictionaryValueForItem(state, navigationState);
            }
            catch
            {
                EPG.Utils.LogCommentWarning("Dictionary value is not defined for the " + navigationState + ". Taking item name instead.");
                dictionaryValue = navigationState;
            }

            parentScrnDictionary.Clear();
            parentScrnDictionary.Add(EnumEpgKeys.TITLE, dictionaryValue);

            EPG.Utils.HighlightOption(state, parentScrnDictionary);
        }

        private String[] TrimAndReplaceNavPath(String navigationPath)
        {
            String[] navigationPathArray = { "" };

            EPG.Utils.LogCommentInfo("Entering the TrimAndReplaceNavPath() function. Input string::" + navigationPath);

            if (navigationPath.Contains("//"))
            {
                navigationPath = navigationPath.Replace("//", "$");
            }

            navigationPathArray = navigationPath.Split('/');

            for (int itr = 0; itr < navigationPathArray.Length; itr++)
            {
                if (navigationPathArray[itr].Contains("$"))
                {
                    navigationPathArray[itr] = navigationPathArray[itr].Replace("$", "//");
                }
            }

            EPG.Utils.LogCommentInfo("Exiting the TrimAndReplaceNavPath() function");
            return navigationPathArray;
        }

    }
}
