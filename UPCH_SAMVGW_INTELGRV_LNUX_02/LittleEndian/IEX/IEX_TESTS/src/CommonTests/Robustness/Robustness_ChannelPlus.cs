/// <summary>
///  Script Name : Robustness_ChannelPlus.cs
///  Test Name   : Robustness ChannelPlus Test
///  TEST ID     : 
///  QC Version  : 
///  Variations from QC:
/// ----------------------------------------------- 
///  Modified by : Aswin Kollaikkal
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("Robustness_ChannelPlus")]
public class Robustness_ChannelPlus : _Test
{
    [ThreadStatic]
    static _Platform CL,GW;

    //Test Duration
    static int testDuration = 0;

    //Shared members between steps
    static string FTA_Channel;
    static string timestamp;
    static string channel;
    static string hours;
    static string isGWIdle;
    static Helper _helper = new Helper();
    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1:Perform Channel Plus for 15 Hours ";
    
    #region Create Structure
    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
       
        //Get Client Platform
        CL = GetClient();
       
        GW = GetGateway();
       
    }
    #endregion

    #region PreExecute
    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition
    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
           channel = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "CHANNEL");
           hours= CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "HOURS");
           isGWIdle = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ISGWIDLE");

           CL.IEX.Wait(1);

           if (isGWIdle.ToUpper() != "TRUE")
           {
              res= GW.EA.STBSettings.SetAutoStandBy("OFF");
           }
            
           PassStep();
        }
    }
    #endregion
    #region Step1
    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();
            DateTime currentDateTIme = DateTime.Now;
            DateTime after15hrDateTime = DateTime.Now.AddHours(Convert.ToInt32(hours));
            int gwPass=0, gwFail=0, clPass=0, clFail = 0;


            for(double i=1;i>=1;i++)
            {
              
                try
                {
                    LogCommentInfo(CL, "start Iteration : " + i.ToString());
                    string chnumbefore = "";
                    string chnumbafter = "";

                    // For Gateway
                    if (isGWIdle.ToUpper() != "TRUE")
                    {
                       
                        GW.IEX.MilestonesEPG.GetEPGInfo("chnum",out chnumbefore);
                        GW.IEX.Wait(1);
                        LogCommentInfo(GW, "GW Channel Number before zapping : " + chnumbefore.ToString());

                        res = GW.IEX.SendIRCommand("CH_PLUS", -1, ref timestamp);
                        GW.IEX.Wait(2);
                         
                        GW.IEX.MilestonesEPG.GetEPGInfo("chnum",out chnumbafter);

                        LogCommentInfo(GW, "GW Channel Number after zapping : " + chnumbafter.ToString());

                        if (!res.CommandSucceeded)
                        {
                            LogCommentWarning(GW, "Failed to send IR Command in Gateway");
                            CL.IEX.GetSnapshot("Failed to send IR Command in Gateway");
                        }
                        else
                        {
                            if (chnumbefore == chnumbafter)
                            {
                                gwFail = gwFail + 1;
                               GW.IEX.GetSnapshot("Failed to Verify Channel in GW");
                                if (gwFail > 200)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                gwPass = gwPass + 1;
                            }
                        }
                    }

                    //Verify IPC

                    CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out chnumbefore);
                    CL.IEX.Wait(1);
                    LogCommentInfo(CL, "IPC Channel Number before zapping : " + chnumbefore.ToString());

                    res = CL.IEX.SendIRCommand("CH_PLUS",-1, ref timestamp);
                    CL.IEX.Wait(2);

                    CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out chnumbafter);

                    LogCommentInfo(CL, "IPC Channel Number after zapping : " + chnumbafter.ToString());
                    if (!res.CommandSucceeded)
                    {
                        LogCommentWarning(GW, "Failed to send IR Command in IPC");
                        CL.IEX.GetSnapshot("Failed to send IR Command in IPC");
                    }
                    else
                    {
                        if (chnumbefore == chnumbafter)
                        {
                            clFail = clFail + 1;
                            CL.IEX.GetSnapshot("Failed to Verify Channel in IPC");
                            if(clFail>200)
                            {
                                break;
                            }
                        }
                        else
                        {
                            clPass = clPass + 1;
                        }
                    }
                   

                    CL.IEX.Wait(10);

                    currentDateTIme = DateTime.Now;
                    if (currentDateTIme >= after15hrDateTime)
                    {
                        break;
                    }

                   
                    if (isGWIdle.ToUpper() != "TRUE")
                    {
                        
                        LogCommentInfo(GW, "Gateway Channel Plus passed " + gwPass + " times");
                        LogCommentInfo(GW, "Gateway Channel Plus failed " + gwFail + " times");

                       
                    }

                  
                    LogCommentInfo(CL, "IPC Channel Plus passed " + clPass + " times");
                    LogCommentInfo(CL, "IPC Channel Plus failed " + clFail + " times");

                    LogCommentInfo(CL, "End Iteration : " + i.ToString());

                }
                   
                catch(Exception ex)
                {
                    CL.IEX.GetSnapshot(ex.Message);
                }

               

            }

            if (gwFail > 200)
            {
                FailStep(GW, "Failed to perform Channel Plus more than 200 times in gateway");
            }
            if (clFail > 200)
            {
                FailStep(CL, "Failed to perform Channel Plus more than 200 times in IPC");
            }
                   
           // verify after DCA , Main Menu, Settings, standby after 15 hours

            if (isGWIdle.ToUpper() != "TRUE")
            {
                LogCommentInfo(GW, "Verifying Gateway...");
                LogCommentInfo(GW, "Gateway Channel Plus passed " + gwPass + " times");
                LogCommentInfo(GW, "Gateway Channel Plus failed " + gwFail + " times");

                _helper.VerifySTB(GW);
            }

            LogCommentInfo(CL, "Verifying IPC...");
            LogCommentInfo(CL, "IPC Channel Plus passed " + clPass + " times");
            LogCommentInfo(CL, "IPC Channel Plus failed " + clFail + " times");

            _helper.VerifySTB(CL);

            PassStep();
        }
    }
    #endregion
   
    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
       
       


    }
    #endregion
    #region helper class
    public class Helper : _Step
    {
        public override void Execute()
        {
           
        }
        public void VerifySTB(_Platform PL)
        {
            //DCA 720

            res = PL.EA.TuneToChannel(channel);

            //Launch Main Menu
            LogCommentInfo(PL, "Navigate To Main Menu");
            res = PL.IEX.MilestonesEPG.NavigateByName("STATE:MAIN MENU");
            if (!res.CommandSucceeded)
            {
                FailStep(PL, res, "Failed to Navigate to MAIN MENU");
            }

            PL.IEX.Wait(5);

            //Navigate To TV GUIDE
            LogCommentInfo(PL, "Navigating To TV GUIDE");

            try
            {
                PL.EA.UI.Guide.Navigate();
                CL.IEX.Wait(3);
                LogCommentInfo(PL, "Passed Navigate To TV GUIDE");
            }
            catch
            {

                FailStep(PL, res, "Failed to Navigate to TV GUIDE");

            }
          


           
            // set the timeout value on the box 

            PL.IEX.Wait(4);

            res = PL.EA.STBSettings.SetBannerDisplayTime(EnumChannelBarTimeout._15);

            if (!res.CommandSucceeded)
            {
                FailStep(PL, "Failed To set Menu TimeOut");
            }
            else
            {
                LogComment(PL, "Chanel bar timeout set");
            }

            res = PL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                FailStep(PL, "Failed To enter standby");
            }
            else
            {
                LogComment(PL, "Entered stand by");
            }


            PL.IEX.Wait(10);

            PL.EA.StandBy(true);
            if (!res.CommandSucceeded)
            {
                FailStep(PL, "Failed To exit standby");
            }
            else
            {
                LogComment(PL, "Wakeup from stand by");
            }
        }
    }
    #endregion
}