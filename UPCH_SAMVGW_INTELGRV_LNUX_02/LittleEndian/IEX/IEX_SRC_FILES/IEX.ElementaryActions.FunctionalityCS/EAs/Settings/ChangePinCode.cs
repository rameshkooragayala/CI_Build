using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.ElementaryActions.Functionality;
using FailuresHandler;


namespace EAImplementation
{

    /// <summary>
    /// Change pin code from settings
    /// </summary>
    public class ChangePinCode : IEX.ElementaryActions.BaseCommand
    {
        private IEX.ElementaryActions.EPG.SF.UI EPG;

        private IEX.ElementaryActions.Functionality.Manager _manager;
        private string _NewPin;
        private string _OldPin;
		string confirmChangePin;

        /// <param name="OldPin">Change Pin Code - Old Pin</param>
        /// <param name="NewPin">Change Pin Code - New Pin</param>
        /// <param name="pManager">Manager</param>
        public ChangePinCode(string OldPin, string NewPin, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._NewPin = NewPin;
            this._OldPin = OldPin;
            this._manager = pManager;
            EPG = this._manager.UI;
        }

        /// <summary>
        /// EA Execution
        /// </summary>
 protected override void Execute()
        {
           

            EPG.Utils.EPG_Milestones_NavigateByName("STATE:CHANGE PIN CODE");

            this._manager.EnterPIN(_OldPin, "CHANGE PIN CODE");

            confirmChangePin = EPG.Utils.GetValueFromProject("FACTORY_RESET", "CONFIRM_CHANGE_PIN");

          if (String.IsNullOrEmpty(confirmChangePin) || confirmChangePin=="True")
          {
                //Change pin code information.
                EPG.Utils.EPG_Milestones_SelectMenuItem("CONTINUE");
                EPG.Utils.SendIR("SELECT");

                //Enter new pin
                this._manager.EnterPIN(_NewPin, "CHANGE PIN CODE");

                //Confirm new Pin
                this._manager.EnterPIN(_NewPin, "CHANGE PIN CODE");



                //Change pin code successful
                EPG.Utils.EPG_Milestones_SelectMenuItem("CONTINUE");

                EPG.Utils.SendIR("SELECT");
            }
          else
            {

                //Enter new pin
                this._manager.EnterPIN(_NewPin, "CHANGE PIN CODE");

                //Confirm new Pin
                this._manager.EnterPIN(_NewPin, "CHANGE PIN CODE");

               
            }

            
        }
    }

     
}


