/// <summary>
///  Script Name : PLB_0401_EndofEventReachedFF
///  Test Name   : PLB-0401-End of event is reached in Fast Forward mode
///  TEST ID     : 71303
///  Repository  :STB_DIVISION
///  Jira ID     : FC-722
///  QC Version  : 1
///  Variations from QC:None
/// ----------------------------------------------- 
///  Modified by :Scripted by : Appanna Kangira
/// Last modified : 022 Oct 2013
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**********************************************************************************************//**
 * @class   PLB_0401
 *
 * @brief   When the end of recording is reached in FF mode.Verify the recordings list is displayed with
 *          the last recording being focussed
 *
 * @author  Appannak
 * @date    22-Oct-13
 **************************************************************************************************/

[Test("PLB_0401_EndofEventreachedFF")]
public class PLB_0401 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The first service to be recorded.
     **************************************************************************************************/

    private static Service serviceToBeRecorded1;

    /**********************************************************************************************//**
     * @brief   The second service to be recorded.
     **************************************************************************************************/

    private static Service serviceToBeRecorded2;

    /**********************************************************************************************//**
     * @brief   The firstevent to be recorded.
     **************************************************************************************************/

    private static string firsteventToBeRecorded = "EVENT1";

    /**********************************************************************************************//**
     * @brief   The secondevent to be recorded.
     **************************************************************************************************/

    private static string secondeventToBeRecorded = "EVENT2";

    /**********************************************************************************************//**
     * @brief   Name of the second recording.
     **************************************************************************************************/

    private static string secondeventRecordedName;

    /**********************************************************************************************//**
     * @brief   The focussedrec name aftr plybck.
     **************************************************************************************************/

    private static string focussedrecNameAftrPlybck;

    /**********************************************************************************************//**
     * @brief   The EOF playbackdestination.
     **************************************************************************************************/

    private static string eofPlaybackdestination;

    /**********************************************************************************************//**
     * @brief   The trickmode f fmin speed in string.
     **************************************************************************************************/




    private static string TM_FFStr;
    private static double TM_FF;

    private static class Constants
    {
        /**********************************************************************************************//**
         * @brief   The seconds to play.
         **************************************************************************************************/

        public const int secsToPlay = 0;

        /**********************************************************************************************//**
         * @brief   The minimum time before event end.
         **************************************************************************************************/

        public const int minTimeBeforeEventEnd = 1;

        /**********************************************************************************************//**
         * @brief   The first minimum time before event end.
         **************************************************************************************************/

        public const int minTimeBeforeEventEnd1 = 4;

        /**********************************************************************************************//**
         * @brief   The security to wait for record.
         **************************************************************************************************/

        public const int secToWaitForRecord = 60;
    }

    /**********************************************************************************************//**
     * @brief   Event queue for all listeners interested in PRECONDITION_DESCRIPTION events.
     **************************************************************************************************/

    private const string PRECONDITION_DESCRIPTION = "Precondition:Fetch services required for the test case.Initiate Two event based recordings on different services";

    /**********************************************************************************************//**
     * @brief   Information describing the step 1.
     **************************************************************************************************/

    private const string STEP1_DESCRIPTION = "Step 1:Wait for a While & Stop all the Initiated Recordings ";

    /**********************************************************************************************//**
     * @brief   Information describing the step 2.
     **************************************************************************************************/

    private const string STEP2_DESCRIPTION = "Step 2:Play the recording till End OF File in Fast Forward mode and Verify the Destination after Playback & Highlight on Last Played Item ";


    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Appannak
     * @date    22-Oct-13
     **************************************************************************************************/

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute

    /**********************************************************************************************//**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Appannak
     * @date    22-Oct-13
     **************************************************************************************************/

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   A pre condition.
     *
     * @author  Appannak
     * @date    22-Oct-13
     **************************************************************************************************/

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    22-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Channel Values From XML File
            serviceToBeRecorded1 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True;IsMinEventDuration=True", "ParentalRating=High");
            if (serviceToBeRecorded1 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            //Get Channel Values From XML File
            serviceToBeRecorded2 = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "LCN=" + serviceToBeRecorded1.LCN + ";ParentalRating=High");
            if (serviceToBeRecorded2 == null)
            {
                FailStep(CL, "Failed to fetch a service which matches the passed criteria.");
            }

            //Fetch the PlayBack EOF  Destination STATE from PROJECT INI.

            eofPlaybackdestination = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "PLAYBACK_DESTINATION");

            if (String.IsNullOrEmpty(eofPlaybackdestination))
            {
                LogCommentFailure(CL, "Fetched PLAYBACK_DESTINATION Value is null or empty");
            }

            LogCommentInfo(CL, " The Fetched PlayBack EOF  Destination STATE:" + eofPlaybackdestination);


            //Fetch the trick Mode FF MIN Speed from PROJECT INI.

            TM_FFStr = CL.EA.GetValueFromINI(EnumINIFile.Project, "PLAYBACK", "FWD_MIN");
            TM_FF = double.Parse(TM_FFStr);
            LogCommentInfo(CL, "The FFMIN speed is " + TM_FF);
            

         

            //Tune to the service whose event will be recorded.
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded1.LCN);
            }

            ////Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(firsteventToBeRecorded, Constants.minTimeBeforeEventEnd,false,true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule first  recording");
            }


            //Tune to the Second service whose event will be recorded
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, serviceToBeRecorded2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + serviceToBeRecorded1.LCN);
            }

            ////Schedule a record
            res = CL.EA.PVR.RecordCurrentEventFromBanner(secondeventToBeRecorded, Constants.minTimeBeforeEventEnd1,false,false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to schedule second recording");
            }


            ////Fetch the name of the event which is getting recorded
            secondeventRecordedName = CL.EA.GetEventInfo(secondeventToBeRecorded, EnumEventInfo.EventName);
            if (String.IsNullOrEmpty(secondeventRecordedName))
            {
                FailStep(CL, "Failed to fetch the name of the second event to be recorded");
            }
            LogCommentInfo(CL, "Fetched the name of the event which is getting recorded :" + secondeventRecordedName);

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   A step 1.
     *
     * @author  Appannak
     * @date    22-Oct-13
     **************************************************************************************************/

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    22-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            LogCommentInfo(CL, "Waiting for " + Constants.secToWaitForRecord + " secs to ensure sufficient duration of recording");
            res = CL.IEX.Wait(Constants.secToWaitForRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait for minimum event time required for the recording to go on ");
            }

           //Stopping the recordign from banner

            CL.EA.PVR.StopRecordingFromBanner(secondeventToBeRecorded);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Stop the secondeventRecorded From Archive");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   A step 2.
     *
     * @author  Appannak
     * @date    22-Oct-13
     **************************************************************************************************/

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Appannak
         * @date    22-Oct-13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();
            //Play the recording from Archive
            res = CL.EA.PVR.PlaybackRecFromArchive(secondeventToBeRecorded, Constants.secsToPlay, true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Playback the Event " + secondeventToBeRecorded + " From Archive");
            }


          

            //Set the Trick Mode Speed to FF_MIN Value & Verify EOF.
            res = CL.EA.PVR.SetTrickModeSpeed(secondeventToBeRecorded, TM_FF, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set the FF_MIN  Speed :" + TM_FF);
            }


            if (String.Equals(eofPlaybackdestination, "MY RECORDINGS") == true)
            {
                if (CL.EA.UI.Utils.VerifyState(eofPlaybackdestination))
                {
                    res = CL.IEX.MilestonesEPG.GetEPGInfo("evtName", out focussedrecNameAftrPlybck);
                    if (!res.CommandSucceeded)
                    {
                        FailStep(CL, res, "Failed to get the focussed item");
                    }
                    //Verify the Recording name focussed in Recordings List is that of one that is Last Played
                    if (String.Equals(secondeventRecordedName, focussedrecNameAftrPlybck, StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        LogCommentInfo(CL, "Focused Item matches the Last Played back recording :" + focussedrecNameAftrPlybck);
                    }
                    else
                    {
                        FailStep(CL, "Failed to Verify the Focus on Last Played recording");
                    }

                }
                else
                {
                    FailStep(CL, "STATE:" + eofPlaybackdestination + "is not a VALID Playback destination");
                }
            }
            else
            {
                FailStep(CL, "Not handled to verify STATE other than MY RECORDINGS.To be Handled in Future");
            }
            PassStep();
        }
    }
    #endregion


    #endregion

    #region PostExecute

    /**********************************************************************************************//**
     * @fn  public override void PostExecute()
     *
     * @brief   Posts the execute.
     *
     * @author  Appannak
     * @date    22-Oct-13
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        res = CL.EA.PVR.StopRecordingFromBanner(firsteventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to Stop the firsteventRecorded From Archive");
        }
        //Delete the recorded event  from Archive
        res = CL.EA.PVR.DeleteRecordFromArchive(firsteventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to delete the first recorded event  from archive!");
        }
        res = CL.EA.PVR.DeleteRecordFromArchive(secondeventToBeRecorded);
        if (!res.CommandSucceeded)
        {
            CL.IEX.FailStep("Failed to delete the second recorded event  from archive!");
        }
    }
    #endregion
}
