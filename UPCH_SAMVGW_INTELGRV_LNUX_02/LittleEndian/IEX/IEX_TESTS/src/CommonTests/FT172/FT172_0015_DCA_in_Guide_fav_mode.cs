using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;



[Test("FT172_0015")]
public class FT172_0015 : _Test
{

    [ThreadStatic]
    private static _Platform CL;

    
    private static string focusedChname = "";
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    private static Service Service_5;
    private static string First_number = "1";
    private static string Forth_number = "4";
    private static string SixHundredandOne = "601";
    private static string Highest_number = "999";
    static string[] adjustTimeLineValues = { "" };
    private static string _favoriteChannelList = "";


    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition:Get values from Ini files");
        this.AddStep(new Step1(), "Step 1: launch Guide ALL channel and surf to favorite and no favorite channels ");
        this.AddStep(new Step2(), "Step 2: launch Guide adjust timeline and surf to favorite and no favorite channels  ");
        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
            CL.IEX.Wait(10);
            //Get Values From xml File
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service_1.LCN);
            }
            //Get Values From xml File
            int channel1 = Convert.ToInt32(Service_1.LCN);
            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + channel1);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service_2.LCN);
            }
            int channel2 = Convert.ToInt32(Service_2.LCN);
            //Get Values From xml File
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + channel2 + "," + channel1);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service_3.LCN);
            }
            //Get Values From xml File
            int channel3 = Convert.ToInt32(Service_3.LCN);
            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;LCN=" + channel1 + "," + channel2 + "," + channel3);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service_4.LCN);
            }
            //Get Values From xml File
            int channel4 = Convert.ToInt32(Service_4.LCN);
            Service_5 = CL.EA.GetServiceFromContentXML("Type=Video", "IsDefault=True;ParentalRating=High;LCN=" + channel1 + "," + channel2 + "," + channel3+"," + channel4);
            if (Service_5 == null)
            {
                FailStep(CL, "Failed to get a service matching the given criteria.");
            }
             //remove all favorites channels 
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to UnsetAllFavChannels");
            }
            // Set favorite channels
            _favoriteChannelList = "Service_1.LCN,Service_2.LCN";
            res = CL.EA.STBSettings.SetFavoriteChannelNameList("" + Service_1.Name + ", " + Service_2.Name + ", " + Service_3.Name +", " + Service_4.Name +", " + Service_5.Name +"", EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to set channels " + _favoriteChannelList + " as favorites");
            }
            
            PassStep();
        }
    }
    #endregion

    #region Step1
    private class Step1 : _Step
    {

        // Enable favorite mode in action menu
        private static bool enableFavoriteMode()
        {
            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ENABLE FAVOURITE MODE");
                return true;
            }
            catch (Exception)
            {
                try
                {
                    CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR");
                    if (CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("DISABLE FAVOURITE MODE"))
                    {
                        CL.EA.UI.Utils.SendIR("RETOUR");
                        return true;
                    }
                }
                catch (Exception) { }
            }
            return false;
        }
        public void ZapOnWhenOnGuide(string ChannelNumber)
        {

            // Zap to channel number 
            res = CL.EA.ChannelSurf(EnumSurfIn.Guide, ChannelNumber);

            //Obtaining CH_Num on Grid State
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out focusedChname);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }

            //Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID
            if (ChannelNumber == First_number)
            {
                if (Service_1.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_1.Name + " " + focusedChname);
                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_1.Name + "actual service" + focusedChname);
                }

                
            }
            else if (ChannelNumber == Forth_number)
            {
                if (Service_4.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_4.Name + " " + focusedChname);

                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_4.Name + "actual service" + focusedChname);
                }
            }
            else if (ChannelNumber == SixHundredandOne)
            {
                if (Service_5.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_5.Name + " " + focusedChname);

                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_5.Name + "actual service" + focusedChname);
                }
            }
            else if (ChannelNumber == Highest_number)
            {
                if (Service_5.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_5.Name + " " + focusedChname);

                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_5.Name + "actual service" + focusedChname);
                }
            }
            else
            {
                FailStep(CL, "Failed to Focus on the Tuned Channel in Grid");
            }
        }
        public override void Execute()
        {
            StartStep();
            // Enable favorite mode in action menu
            enableFavoriteMode();
            CL.IEX.Wait(5);
            //Zap to firts channel in favorite lits 
            ZapOnWhenOnGuide(First_number);
            CL.IEX.Wait(5);
            //Zap on one channel in favorite list
            ZapOnWhenOnGuide(Forth_number);
            CL.IEX.Wait(5);
            //Zap to channel not in favorite list 
            ZapOnWhenOnGuide(SixHundredandOne);
            CL.IEX.Wait(5);
            //Zap to highest channel 
            ZapOnWhenOnGuide(Highest_number);

            PassStep();
        }
    }
    #endregion

    #region Step2
    private class Step2 : _Step
    {
        public void ZapOnWhenOnAdjustTimeLine(string ChannelNumber, string Time)
        {

            // Zap to channel number 
            res = CL.EA.ChannelSurf(EnumSurfIn.GuideAdjustTimeline, ChannelNumber, GuideTimeline: Time);

            //Obtaining CH_Num on Grid State
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chName", out focusedChname);// check by channel name 
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }
            //Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID
            if (ChannelNumber == First_number)
            {
                if (Service_1.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_1.LCN + " " + focusedChname);
                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_1.Name + "actual service" + focusedChname);
                }
            }
            else if (ChannelNumber == Forth_number)
            {
                if (Service_4.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_4.LCN + " " + focusedChname);

                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_4.Name + "actual service" + focusedChname);
                }
            }
            else if (ChannelNumber == SixHundredandOne)
            {
                if (Service_5.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_5.LCN + " " + focusedChname);

                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_5.Name + "actual service" + focusedChname);
                }
            }
            else if (ChannelNumber == Highest_number)
            {
                if (Service_5.Name == focusedChname)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_5.LCN + " " + focusedChname);

                }
                else
                {
                    FailStep(CL, res, "expected service " + Service_5.Name + "actual service" + focusedChname);
                }
            }
            else
            {
                FailStep(CL, "Failed to Focus on the Tuned Channel in Grid");
            }
        }
        public override void Execute()
        {
            StartStep();

            adjustTimeLineValues = CL.EA.GetValueFromINI(EnumINIFile.Project, "ADJUST TIMELINE", "LIST_ADJ_TL").Split(',');

            foreach (string adjustTimeLine in adjustTimeLineValues)
            {
                //Zap on one channel in favorite list
                ZapOnWhenOnAdjustTimeLine(First_number, adjustTimeLine);
                CL.IEX.Wait(10);
                //Zap to channel not in favorite list 
                ZapOnWhenOnAdjustTimeLine(Forth_number, adjustTimeLine);
                CL.IEX.Wait(10);
                //Zap to firts channel in favorite lits 
                ZapOnWhenOnAdjustTimeLine(SixHundredandOne, adjustTimeLine);
                CL.IEX.Wait(10);
                //Zap to channel not in favorite list 
                ZapOnWhenOnAdjustTimeLine(Highest_number, adjustTimeLine);
                CL.IEX.Wait(10);


            }
            PassStep();
        }
    }
    #endregion
}

