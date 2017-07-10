/// <summary>
///  Script Name : CHBAR_0504_ChannelLineup.cs
///  Test Name   : EPG-0504-ChannelBar-Channel line-up and information during playback from review buffer
///  TEST ID     : 63794
///  JIRA ID     : FC-283
///  QC Version  : 1
/// ----------------------------------------------- 
///  Modified by : Avinob Aich
///  Modified Date: 11/07/2013
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
using IEX.Tests.Reflections;

public class CHBAR_0504_ChannelLineup : _Test
{
    [ThreadStatic]
    static _Platform CL;
    //Channels used by the test
    static Service Channel_1;
    static Service RBchannel;
    static Service Channel_2;
    static Service ChanneltoCheck;

    static class Constants
    {
        public const int rbWait = 5; // in min
        public const int rbPlay = 1;
    }
    #region Create Structure
    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml File & and store a content of channel in RB");
        this.AddStep(new Step1(), "Step 1: Open Channel Bar and check if channel name & channel logo is present");
        this.AddStep(new Step2(), "Step 2: Press up arrow key");
        this.AddStep(new Step3(), "Step 3: Press numeric key on RC");

        //Get Client Platform
        CL = GetClient();
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
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();


            string RBlist;
            string[] RBarr;
            //Entering to Stand-By
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Enter StandBy. Cannot Flush the RB");
            }
            else
            {
                string standByWait = CL.EA.GetValueFromINI(EnumINIFile.Project, "STANDBY", "SAFE_DELAY_SEC");
                CL.IEX.Wait(Convert.ToDouble(standByWait));     //Wait for safe delay sec in StandBy

                //Exiting from StandBy
                res = CL.EA.StandBy(true);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Exit from StandBy");
                }
            }

            //Get Values from Channel.xml
            Channel_1 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "ParentalRating=High");
            RBchannel = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "LCN=" + Channel_1.LCN + ";ParentalRating=High");
            Channel_2 = CL.EA.GetServiceFromContentXML("Type=Video;HasChannelLogo=True", "LCN=" + Channel_1.LCN + "," + RBchannel.LCN + ";ParentalRating=High");
            
            //DCA to a particular channel
            res = CL.EA.TuneToChannel(RBchannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + RBchannel.LCN);
            }

            //wait for 5 min to store the content in RB
            CL.IEX.Wait(Constants.rbWait * 60);
            
            //DCA to other channel
            res = CL.EA.TuneToChannel(Channel_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + Channel_1.LCN);
            }

            //return to live to ensure no trickmode from channel bar
            res = CL.EA.ReturnToLiveViewing();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to return to Live", false);
            }

            //Get List of rewind speed in RB from project.ini
            RBlist = CL.EA.GetValueFromINI(EnumINIFile.Project, "RB", "LIST_TM_REW");
            RBarr = RBlist.Split(',');

            // Get the mid speed of Rewind speed
            double rbMid = Convert.ToDouble(RBarr[RBarr.Length / 2]);

            //Rewind in TrickMode to begining of the file at average speed of the rb
            res = CL.EA.PVR.SetTrickModeSpeed("RB", rbMid, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Rewind the RB at " + rbMid + " Speed to BOF");
            }

            //Play from RB
            res = CL.EA.PVR.SetTrickModeSpeed("RB", Constants.rbPlay, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to playback in RB");
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            string chName = "";
            string chLogo = "";
            string chNum = "";
            bool isPass = true;
            //Navigate to channel Bar
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to lauch Channel Bar");
            }

            //Get channel number from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out chNum);
            if(!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Channel Number from Channel Bar", false);
                isPass = false;
            }

            //Get channel from xml having channel number displayed in channel bar
            ChanneltoCheck = CL.EA.GetServiceFromContentXML("LCN=" + chNum, "");

            //Get the channel name
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out chName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get EPG info", false);
                isPass = false;
            }

            //Checking if Channel Name is Empty
            if (string.IsNullOrEmpty(chName))
            {
                FailStep(CL, "Channel name is Empty", false);
                isPass = false;
            }

            //Get the channel logo
            res = CL.IEX.MilestonesEPG.GetEPGInfo("channel_logo", out chLogo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Channel Logo in Channel Bar", false);
                isPass = false;
            }

            //Checking if Channel Logo is Empty
            if (string.IsNullOrEmpty(chLogo))
            {
                FailStep(CL, "Channel Logo is Empty");
            }

            //checking if the step pass
            if (!isPass)
            {
                FailStep(CL, "Captured in Previous");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            string chNum = "";
            bool isPass = true;
            Service ChBarNextChannel;
            //Surf channel once from channel Bar
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to do channel surf from Channel Bar");
            }

            //Get channel number in channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out chNum);
            if(!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Channel Number from Channel Bar", false);
                isPass = false;
            }

            //Get channel from content.xml whose channel number is in channel bar
            ChBarNextChannel = CL.EA.GetServiceFromContentXML("LCN=" + chNum, "");

            //checking if next channel is focussed
            if (ChBarNextChannel.PositionOnList.Equals(Convert.ToString((Convert.ToInt32(ChanneltoCheck.PositionOnList))+1)))
            {
                FailStep(CL, "Next Channel didnot focussed in Channel Bar");
                isPass = false;
            }

            //Check if video is present
            res = CL.EA.CheckForVideo(true, true, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Fail to verify video in RB");
            }
            if (!isPass)
            {
                FailStep(CL, "Captured in Previous");
            }
            PassStep();
        }
    }
    #endregion
    #region Step3
    public class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //DCA to a channel
            res = CL.EA.TuneToChannel(Channel_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to channel " + Channel_2.LCN);
            }
            // check if video is present
            res = CL.EA.CheckForVideo(true, true, 20);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Fail to verify video on channel " + Channel_2.LCN);
            }
            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}