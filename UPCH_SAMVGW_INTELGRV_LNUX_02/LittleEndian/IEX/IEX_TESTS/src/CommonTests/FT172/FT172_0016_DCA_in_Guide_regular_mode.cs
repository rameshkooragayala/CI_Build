using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;



[Test("FT172_0016")]
public class FT172_0016 : _Test
{

    [ThreadStatic]
    private static _Platform CL;

    private static string focusedChnum = "";
    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    private static string First_number = "1";
    private static string Highest_number = "999";
    private static string Lowest_Service_Number;
    private static string RealHighest_number ;
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
            //Get Values From ini File
            RealHighest_number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Highest_Service_Number");
            if (string.IsNullOrEmpty(RealHighest_number))
            {
                RealHighest_number = CL.EA.GetValue("Highest_Service_Number");
                if (RealHighest_number == null)
                {
                    FailStep(CL, "Failed to fetch Highest_Service_Number from ini file.");
                }
            }
            //Get Values From ini File
            Lowest_Service_Number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Lowest_Service_Number");
              if (string.IsNullOrEmpty(Lowest_Service_Number))
              {
                  Lowest_Service_Number = CL.EA.GetValue("Lowest_Service_Number");
                  if (Lowest_Service_Number == null)
                  {
                      FailStep(CL, "Failed to fetch Lowest_Service_Number from ini file.");
                  }
              }
            //Get Values From xml File
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video","ParentalRating=High");
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
            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video","ParentalRating=High;LCN="+channel1);
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
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video","ParentalRating=High;LCN="+channel2+","+channel1);
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
            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video","ParentalRating=High;LCN="+channel1+","+channel2+ ","+channel3);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }
            else
            {
                LogCommentInfo(CL, "Service fetched is : " + Service_4.LCN);
            }
            //remove all favorites channels 
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to UnsetAllFavChannels");
            }
            // Set favorite channels
            _favoriteChannelList = "" + Service_1.Name + ", " + Service_2.Name + "";
            res = CL.EA.STBSettings.SetFavoriteChannelNameList(_favoriteChannelList, EnumFavouriteIn.Settings);
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

        public void ZapOnWhenOnGuide(string ChannelNumber)
        {
            //we have to separate limit case otherwise we will get an error 
            if (ChannelNumber == Highest_number || ChannelNumber == First_number)
            {
                // Zap to channel number we have to separate limit case otherwise we will get an error 
                res = CL.EA.ChannelSurf(EnumSurfIn.Guide, ChannelNumber);
            }
            else
            {
                // Zap to channel number 
                res = CL.EA.ChannelSurf(EnumSurfIn.Guide, ChannelNumber);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel With DCA");
                }

            }
            //Obtaining CH_Num on Grid State
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out focusedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }

            //Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID
            if (ChannelNumber == First_number)
            {
                if (Lowest_Service_Number == focusedChnum)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Lowest_Service_Number + " " + focusedChnum);
                }
                else
                {
                    FailStep(CL, res, "received diffrent logical channel number " + focusedChnum + "expected chennel nuber is" + Lowest_Service_Number);
                }
            }
            else if (ChannelNumber == Highest_number)
            {
                if (RealHighest_number == focusedChnum)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + RealHighest_number + " " + focusedChnum);

                }
                else
                {
                    FailStep(CL, res, "received diffrent logical channel number " + focusedChnum + "expected chennel nuber is" + RealHighest_number);
                }
            }
            else if (ChannelNumber == focusedChnum)
            {
                LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + ChannelNumber + " " + focusedChnum);
            }
            else
            {
                FailStep(CL, "Failed to Focus on the Tuned Channel in Grid");
            }
        }
        public override void Execute()
        {
            StartStep();

            //Zap on one channel in favorite list
            ZapOnWhenOnGuide(Service_2.LCN);
            CL.IEX.Wait(5);
            //Zap to channel not in favorite list 
            ZapOnWhenOnGuide(Service_3.LCN);
            CL.IEX.Wait(5);
            //Zap to firts channel in favorite lits 
            ZapOnWhenOnGuide(First_number);
            CL.IEX.Wait(5);
            //Zap to channel not in favorite list 
            ZapOnWhenOnGuide(Service_4.LCN);
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
            //we have to separate limit case otherwise we will get an error 
            if (ChannelNumber == Highest_number || ChannelNumber == First_number)
            {
                // Zap to channel number 
                res = CL.EA.ChannelSurf(EnumSurfIn.GuideAdjustTimeline, ChannelNumber, GuideTimeline: Time);
            }
            else
            {
                // Zap to channel number 
                res = CL.EA.ChannelSurf(EnumSurfIn.GuideAdjustTimeline, ChannelNumber, GuideTimeline: Time);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to Tune to Channel With DCA");
                }
                
            }
            //Obtaining CH_Num on Grid State
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out focusedChnum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Focussed item  Channel number in Grid");
            }
            //Checking the Tuned Channel Num is same as the One Focussed in Guide->GRID
            if (ChannelNumber == First_number)
            {
                if (Lowest_Service_Number == focusedChnum)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + Service_1.LCN + " " + focusedChnum);
                }
                else
                {
                    FailStep(CL, res, "received diffrent logical channel number " + focusedChnum + "expected chennel nuber is" + Lowest_Service_Number);
                }
            }
            else if (ChannelNumber == Highest_number)
            {
                if (RealHighest_number == focusedChnum)
                {
                    LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + RealHighest_number + " " + focusedChnum);
                }
                else
                {
                    FailStep(CL, res, "received diffrent logical channel number " + focusedChnum + "expected chennel nuber is" + RealHighest_number);
                }
            }
            else if (ChannelNumber == focusedChnum)
            {
                LogCommentInfo(CL, "The Tuned Channel is same as the Focussed Channel in Grid " + ChannelNumber + " " + focusedChnum);
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
                ZapOnWhenOnAdjustTimeLine(Service_2.LCN, adjustTimeLine);
                CL.IEX.Wait(5);
                //Zap to channel not in favorite list 
                ZapOnWhenOnAdjustTimeLine(Service_3.LCN, adjustTimeLine);
                CL.IEX.Wait(5);
                ////Zap to firts channel in favorite lits 
                ZapOnWhenOnAdjustTimeLine(First_number, adjustTimeLine);
                CL.IEX.Wait(5);
                ////Zap to channel not in favorite list 
                ZapOnWhenOnAdjustTimeLine(Service_4.LCN, adjustTimeLine);
                CL.IEX.Wait(5);
                ////Zap to highest channel 
                ZapOnWhenOnAdjustTimeLine(Highest_number, adjustTimeLine);


            }
            PassStep();
        }
    }
    #endregion
}

