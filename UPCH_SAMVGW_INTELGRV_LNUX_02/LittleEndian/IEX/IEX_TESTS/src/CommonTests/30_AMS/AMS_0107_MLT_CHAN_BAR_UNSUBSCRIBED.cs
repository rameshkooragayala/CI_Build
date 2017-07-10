/// <summary>
///  Script Name        : AMS_0107_MLT_CHAN_BAR_UNSUBSCRIBED.cs
///  Test Name          : AMS_0107_MLT_CHAN_BAR_UNSUBSCRIBED
///  TEST ID            : 
///  QC Version         : 
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Avinash Budihal
///  Modified Date      : 06th MAR, 2015
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;


  public  class AMS_0107_MLT_CHAN_BAR_UNSUBSCRIBED : _Test
    {
        [ThreadStatic]
        private static _Platform CL;

        //Channels used by the test
        static string ATLs;
        private static Service Service_1;

        #region Create Structure
        public override void CreateStructure()
        {

            this.AddStep(new PreCondition(), "Precondition: Fetch Channel Numbers from xml file & Sync");
            this.AddStep(new Step1(), "Step 1: Launch action menu on unsubscribed channel");
            this.AddStep(new Step2(), "Step 2: Varify the AMS tags");

            //Get Client Platform
            CL = GetClient();
        }
        #endregion Create Structure

        #region PreCondition

        private class PreCondition : _Step
        {
            public override void Execute()
            {
                StartStep();

                // Navigate to unsubscribed channel and set personalized recommendation activation yes

                string unsubchnelNumber = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "UNSUBSCRIBED_CHANNEL");
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, unsubchnelNumber);
                if (!res.CommandSucceeded)
                {
                    LogCommentWarning(CL, "Failed to tune to service " + unsubchnelNumber);
                }

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to launch TV GUIDE");
                }

                if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
                {
                    FailStep(CL, "Failed to set the Personalization to YES");
                }
                
                PassStep();
            }
        }
        #endregion PreCondition

        #region Step1

        private class Step1 : _Step
        {
            public override void Execute()
            {
                StartStep();

                res = CL.IEX.MilestonesEPG.NavigateByName("STATE:TV GUIDE");
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to launch TV GUIDE");
                    LogCommentFailure(CL, "Failed to launch TV GUIDE");
                }
                
                CL.IEX.Wait(3);
                CL.EA.UI.Utils.SendIR("SELECT");
                CL.IEX.Wait(3);

                res = CL.IEX.MilestonesEPG.Navigate("MORE LIKE THIS");
                if (!res.CommandSucceeded)
                {
                    LogCommentFailure(CL, "Failed to Select MORE LIKE THIS" + res.FailureReason);
                }
                
                PassStep();
            }
        }

        #endregion Step1

        #region Step2

        private class Step2 : _Step
        {
            public override void Execute()
            {
                StartStep();

                // Verify the AMS tags

                CL.IEX.Wait(600);

                string eventName = string.Empty;
                res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out eventName);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to get Event Name from Grid");
                }

                res = CL.EA.VerifyAMSTags(EnumAMSEvent.UNSUBSCRIBED, service: Service_1, IsRBPlayback: eventName);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to verify AMS for MLT for UNSUBSCRIBED CHANNEL");
                }

                PassStep();
            }
        }

        #endregion Step2

        #region PostExecute

        public override void PostExecute()
        {


            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
            {
                LogCommentFailure(CL, "Failed to set the Personalization to No");
            }

        }

        #endregion PostExecute
    }

