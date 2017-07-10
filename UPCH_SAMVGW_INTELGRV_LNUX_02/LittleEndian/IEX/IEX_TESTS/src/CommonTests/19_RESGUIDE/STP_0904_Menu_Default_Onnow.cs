/// <summary>
///  Script Name : Menu_Default_Onnow
///  SourceTest Name   : STP_0904_Mainmenu-default-Onnow
///  MQC Project: FR_FUSION \ UPC
///  TEST ID     : 16678
///  Variation from Quality Center:None
///  Scripted by:Appanna Kangira

/// -----------------------------------------------
///  Last Modified  : 20 June 2013 
/// </summary>


using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;


public class STP_0904_Menu_Default_Onnow : _Test
{
    static mPlatform GW = new mPlatform();
    static mPlatform CL = new mPlatform();

    //Test Duration
    static int testDuration = 0;

    //Channels used by the test
    static string FTA_1st_Mux_1;
    static string First_Item_Focus="";
    static string Title = "";

    //Shared members between steps
    static IEXGateway._IEXResult res;
    static bool multiBoxTest = false;

    #region Create Structure
    public override void CreateStructure()
    {
        string errorMsg = null;

        //Brief Description:
        //Pre-conditions:
        //Based on QualityCenter test version 
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync & enter Standby to clear review buffer");
        this.AddStep(new Step1(), "Step 1:  Zap to  FTA Channel,Verify launching Menu & Schedule a recording");
        this.AddStep(new Step2(), "Step 2: Rewind back in Review buffer,Switch to Play mode ");
        this.AddStep(new Step3(), "Step 3: On Review Buffer playback Launch Main Menu,Verify PIP");
        this.AddStep(new Step4(), "Step 4: Playback the recorded Event Launch Main Meny,Verify PIP");


        //Initialize Platforms EA's
        if (Platfroms.ContainsKey("GW")) 
        {
            GW.Platform = Platfroms["GW"];
            var tmpIEX = GW.Platform.IEX;
            GW.EA.TestName = this.ToString();
            GW.EA.Init(ref tmpIEX, Project, ref errorMsg);
            if (errorMsg != null)
            {
                GW.LogComment("Failed to initilaized Gateway EA object!");
            }
            Platfroms["GW"].EA = GW.EA;
            multiBoxTest = true;
        }

        CL.Platform = Platfroms["CL1"];
        var tmpIEX2 = CL.Platform.IEX;
        CL.EA.TestName = this.ToString();
        CL.EA.Init(ref tmpIEX2, Project, ref errorMsg);
        if (errorMsg != null)
        {
            CL.LogComment("Failed to initilaized Client EA object!");
        }
        Platfroms["CL1"].EA = CL.EA;
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            //Get Values From ini File
            FTA_1st_Mux_1 = CL.EA.GetValue("FTA_1st_Mux_1");
            CL.LogComment("Retrieved Value From ini File: FTA_1st_Mux_1 = " + FTA_1st_Mux_1);

            //Get the Default Item attribute from the ini file

            First_Item_Focus = CL.EA.GetValueFromINI(EnumINIFile.Project, "PROJATTRIB", "MAIN_MENU_FIRST_FOCUSSED_ITEM");

            if (First_Item_Focus.Equals(""))
            {
                CL.FailStep("Failed to fetch Main Menu items from the Project attributes file!");
            }
            

             if (multiBoxTest)
            {
                GW.StartStep(this);
                CL.StartStep(this);

                bool MountPassed = Env.MountGroup(Platfroms, testDuration);
                if (MountPassed == false)
                {
                    GW.FailStep("Failed To Mount Gateway", false);
                    CL.FailStep("Failed To Mount Client(s)");
                }
                GW.PassStep();
            }
            else
            {
                CL.StartStep(this);

                res = Env.MountSingle(CL, testDuration);
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res);
                }

            }
            
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {

                CL.FailStep(res, "Failed to put Box in Standby");
            }
          
            res = CL.EA.CheckForVideo(false, false, 15);
            if (!res.CommandSucceeded)
            {
                CL.FailStep(res, "Failed: Video is Present After entering Standby");
            }

            res = CL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                CL.FailStep(res,"Failed Exiting Standby");
            }
            res = CL.EA.CheckForVideo(true, false, 60);
            if (!res.CommandSucceeded)
            {
                CL.FailStep(res, "Failed: Video is not recovered  on Exit From Standby");
            }

            CL.PassStep();

        }
        }
    #endregion
        #region Step1
        private class Step1 : _Step
        {
            public override void Execute()
            {
                CL.StartStep(this);

                //Zap to first FTA_Channel and wait till review buffer is collected
                res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_1);
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to Tune to Channel" + FTA_1st_Mux_1);
                }

                //Launch Main Menu
                res = CL.Platform.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to Navigate to MAIN MENU");
                }

                // Verify the Default Item is Focussed on on Launch of Menu
                
                res = CL.Platform.IEX.MilestonesEPG.GetEPGInfo("title", out Title);
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to Get Menu  title");
                }

                if (String.Equals(First_Item_Focus, Title, StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    CL.LogComment("First_Item_Focus is correct ");
                }

                else
                {
                    CL.FailStep("First_Item_Focus not matching with  Title");
                }

                 //Record on going event from Banner 
                res = CL.EA.PVR.RecordCurrentEventFromBanner("EveRecFromBanner", 1, false, false, false);
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to Record Current Event From Banner");
                }
              
                CL.PassStep();
            }
        }
        #endregion
        #region Step2
        private class Step2 : _Step
        {
            public override void Execute()
            {
                CL.StartStep(this);

                //Stay for a 30 secs to get RB filled up.
                int Wait_Period = 30;
                CL.Platform.IEX.Wait(Wait_Period);
               

                 //rewind 
                res = CL.EA.PVR.SetTrickModeSpeed("RB", -2, false,false);
             if (!res.CommandSucceeded)
             {
                 CL.FailStep(res, "Failed to activate rewind from Review Buffer  at x(-2) Speed");
             }

             //Rewind shall continue for 30 secs
              Wait_Period = 5;
             CL.Platform.IEX.Wait(Wait_Period);

             // Play the event from Review Buffer
             res = CL.EA.PVR.SetTrickModeSpeed("RB", 1, false);
             if (!res.CommandSucceeded)
             {
                 CL.FailStep(res, "Failed to Set the Trick mode to Play ");
             }  

                CL.PassStep();
            }
        }
        #endregion
        #region Step3
        public class Step3 : _Step
        {
            public override void Execute()
            {
                CL.StartStep(this);
                
                //Launch Menu &Verify PIP while the Event is played back from Review Buffer
                
                //Get Mile Stone values which verify PIP
                String Milestones = CL.EA.UI.Utils.GetValueFromMilestones("ChannelSurf");


                //Begin wait for PIP milestones arrival
                CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, 10);
                
                //Launch Menu 
                res = CL.Platform.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");

                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to navigate to Menu");
                }

               // Verify the Default Item is Focussed on on Launch of Menu
                res = CL.Platform.IEX.MilestonesEPG.GetEPGInfo("title", out Title);
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to Get Menu  title");
                }

                if (String.Equals(First_Item_Focus, Title, StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    CL.Platform.IEX.LogComment("First_Item_Focus is correct ");
                }

                else
                {
                    CL.FailStep("First_Item_Focus not matching with  Title");
                }
                
                
               

                //End wait for Milestones arrival
                ArrayList arraylist = new ArrayList();
                if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arraylist))
                {
                    CL.FailStep("Failed to verify PIP while the Event is played back from Review Buffer");
                }
                    CL.PassStep();
                }
            }


        

        #endregion
        #region Step4
        public class Step4 : _Step
        {
            public override void Execute()
            {
                CL.StartStep(this);

                //Stop recording the event that was scheduled in step 1
                
                res = CL.EA.PVR.StopRecordingFromBanner("EveRecFromBanner");
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to stop the ongoing recording  ");
                }
                
                //Playback the recorded event from Archive 
                res = CL.EA.PVR.PlaybackRecFromArchive("EveRecFromBanner", 0, true, false);
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to Playback the recorded event From Archive");
                }


                //Launch Menu & Verify PIP on the Recorded Event Playback from My Library.

                //Get Mile Stone values which verify PIP
                String Milestones = CL.EA.UI.Utils.GetValueFromMilestones("ChannelSurf");


                //Begin wait for PIP milestones arrival
                CL.EA.UI.Utils.BeginWaitForDebugMessages(Milestones, 10);

                //Launch Menu
                res = CL.Platform.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");

                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to navigate to Menu");
                }

                res = CL.Platform.IEX.MilestonesEPG.GetEPGInfo("title", out Title);
                if (!res.CommandSucceeded)
                {
                    CL.FailStep(res, "Failed to Get Menu  title");
                }

                if (String.Equals(First_Item_Focus, Title, StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    CL.Platform.IEX.LogComment("First_Item_Focus is correct ");
                }

                else
                {
                    CL.FailStep("First_Item_Focus not matching with  Title");
                }

                //Enf wait for Milestones arrival
                ArrayList arraylist = new ArrayList();
                if (!CL.EA.UI.Utils.EndWaitForDebugMessages(Milestones, ref arraylist))
                {
                    CL.FailStep("Failed to Verify PIP on the Recorded Event Playback ");
                }
                CL.PassStep();



                PassTest();
            }
        }
        #endregion
    #endregion

        #region PostExecute
        public override void PostExecute()
        {
            CL.EA.TearDown();
            if (multiBoxTest)
            {
                GW.EA.TearDown();
            }

        }


    
        #endregion
}