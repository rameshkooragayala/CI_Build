
// EA for DAY SKIP Functionality

// Created By Aswin Kollaikkal
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;
using System.Globalization;
using IEX.ElementaryActions.EPG;

namespace IEX.ElementaryActions.FunctionalityCS.EAs.Guide
{
    class DaySkipInGuide : IEX.ElementaryActions.BaseCommand
    {
        

        static string currentDate = "";
        static string expectedCurrentFocusDate = "";
        static string eventStartTimefromEPG;
        static string duration;
        static string category;
        static decimal eventStartTime;
        static string dateafter15;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
       
        private IEX.ElementaryActions.Functionality.Manager _manager;

        private EnumGuideViews _guideView;
        private bool _isForward;
        private bool _isGridInCurrentDate;
        private bool _isDisplayIconVerify;
        private int _numberOfPresses;
        IEXGateway._IEXResult res;

        public DaySkipInGuide(EnumGuideViews guideView, bool isForward, int numberOfPresses,bool isGridInCurrentDate,bool isDisplayIconVerify, IEX.ElementaryActions.Functionality.Manager pManager)
        {

           this._guideView= guideView;
           this._isForward=isForward;
           this._numberOfPresses=numberOfPresses;
           this._manager = pManager;
           this._isGridInCurrentDate = isGridInCurrentDate;
           this._isDisplayIconVerify = isDisplayIconVerify;
           EPG = this._manager.UI;
        }


        protected override void Execute()
        {
            switch (_guideView)
            {

                case EnumGuideViews.ALL_CHANNELS:
                    {
                        // Navigate to guide if     not in guide
                        if (!EPG.Guide.IsGuide())
                            EPG.Guide.Navigate();

                        EPG.Guide.VerifyDateStarTimeDisplayIcon(_isGridInCurrentDate,_numberOfPresses,_isForward,_isDisplayIconVerify);
                        
                       
                        break;
                    }
                case EnumGuideViews.ADJUST_TIMELINE:
                    {
                        if (_isGridInCurrentDate)
                         {
                         duration= EPG.Utils.GetValueFromTestIni("TEST PARAMS", "DURATION");
                         EPG.Guide.NavigateToGuideAdjustTimeline(duration);
                         }
                        EPG.Guide.VerifyDateStarTimeDisplayIcon(_isGridInCurrentDate, _numberOfPresses, _isForward, _isDisplayIconVerify);
                       

                        break;
                    }
                case EnumGuideViews.BY_GENRE:
                    {
                         if (_isGridInCurrentDate)
                        {
                            category = EPG.Utils.GetValueFromTestIni("TEST PARAMS", "CATEGORY");
                            EPG.Utils.EPG_Milestones_NavigateByName("STATE:BY GENRE");
                            EPG.Utils.EPG_Milestones_SelectMenuItem(category);
                            EPG.Utils.SendIR("SELECT");
                        }

                        EPG.Guide.VerifyDateStarTimeDisplayIcon(_isGridInCurrentDate, _numberOfPresses, _isForward, _isDisplayIconVerify);
                       

                        break;
                    }
                case EnumGuideViews.SINGLE_CHANNEL:
                    {
                        if (!EPG.Guide.IsGuideSingleChannel())
                            EPG.Guide.NavigateToGuideSingleChannel();

                        EPG.Guide.VerifyDateStarTimeDisplayIcon(_isGridInCurrentDate, _numberOfPresses, _isForward, _isDisplayIconVerify);
                       
                     
                        break;

                    }
                   
            }
           
        }

        

    }
}