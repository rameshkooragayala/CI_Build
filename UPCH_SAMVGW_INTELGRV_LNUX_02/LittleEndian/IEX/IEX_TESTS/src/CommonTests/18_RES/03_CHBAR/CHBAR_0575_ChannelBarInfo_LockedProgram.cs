/// <summary>
///  Script Name : CHBAR_0575_ChannelBarInfo_LockedProgram.cs
///  Test Name   : CHBAR-0575-Channel-Bar-Information-Locked Program
///  TEST ID     : 68111
///  JIRA ID     : FC-470
///  QC Version  : 1
///  Variations from QC: Progress bar is not tested when channel bar is launched
/// ----------------------------------------------- 
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

/**
 * @class   CHBAR_0575
 *
 * @brief   ChannelBar Info on LockedProgram.
 *
 * @author  Varshad
 * @date    11-Sep-13
 */

[Test("CHBAR_0575")]
public class CHBAR_0575 : _Test
{
    /**
     * @brief   The cl.
     */

    [ThreadStatic]
    static _Platform CL;

    /**
     * @brief   The adult channel.
     */

    static Service adultChannel;

    /**
     * @brief   The video service.
     */

    static Service videoService;

    /**
     * @brief   Name of the expected adult event.
     */

    static string expectedAdultEventName;


    /**
     * @brief   Log for record ongoing.
     */

    static string recordOngoing;

    /**
     * @brief   Precondition: Get Channel Numbers From ini File.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";

    /**
     * @brief   Tune to service with locked program and verify Start time, End times, Recording indication, Parental rating, Message indicating the program is locked.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Tune to service with locked program and verify Start time, End times, Recording indication, Parental rating, Message indicating the program is locked";


    /**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Varshad
     * @date    11-Sep-13
     */

    static class Constants
    {
        /**
         * @brief timeBeforeEventEnds  in seconds.
         */

        public const int timeBeforeEventEnds = 2;
    
    }

    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Varshad
     * @date    11-Sep-13
     */

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);


        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region PreExecute

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Varshad
     * @date    11-Sep-13
     */

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }
    #endregion

    #region Steps
    #region PreCondition

    /**
     * @class   PreCondition
     *
     * @brief   Pre condition.
     *
     * @author  Varshad
     * @date    11-Sep-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Precondition: 
         *          1) Get Channel Numbers From xml File.
         *          2) Get project specific values form project.ini  
         *          3) Entering to Standby and exit in order to pop up insert pin when tuned to locked channel.
                       To address feature diversity, where in once tuned to locked channel, STB does not ask 
                       for the pin again inless entry and exit standny is done. 
         *          4) Channel surf where AV is avialable
         *
         * @author  Varshad
         * @date    11-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            adultChannel = CL.EA.GetServiceFromContentXML("Type=Video;ParentalRating=High;IsEITAvailable=True", "");
            
            if (adultChannel == null || videoService == null)
            {
                FailStep(CL, "One of the Services fetched from content.xml is null. Adult Channel: "+adultChannel+
                    " ,Video Service: "+videoService);
            }
            LogCommentInfo(CL, "Locked service: " + adultChannel.LCN + " , Video Service: "+videoService.LCN);

            expectedAdultEventName = CL.EA.GetValueFromINI(EnumINIFile.Project, "EVENT", "LOG_RATING_LOCKED_EVTNAME");
            recordOngoing = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR", "LOG_RECORD_ONGOING");
            string standByWait = CL.EA.GetValueFromINI(EnumINIFile.Project, "STANDBY", "SAFE_DELAY_SEC");

            if(string.IsNullOrEmpty(expectedAdultEventName) || string.IsNullOrEmpty(recordOngoing))
            {
                LogCommentInfo(CL,"One of the values is null or empty in Project.ini.  LOG_RATING_LOCKED_EVTNAME: "+expectedAdultEventName +
                     ", LOG_RECORD_ONGOING: " + recordOngoing);
            }

            //Channel Surf to Video Service 
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA: " + videoService.LCN);
            }

            
            res = CL.EA.TuneToLockedChannel(adultChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to locked Channel" + adultChannel.LCN);
            }

            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event", Constants.timeBeforeEventEnds);           
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res,"Failed to record current event from banner");
            }

            //Entering to Standby and exit in order to pop up insert pin when tuned to locked channel.
            //To address feature diversity, where in once tuned to locked channel, STB does not ask 
            // for the pin again inless entry and exit standny is done. 
            res = CL.EA.StandBy(false);
            if (!res.CommandSucceeded)
            {
                LogCommentImportant(CL, "Failed to put Box in Standby");
            }
            else
            {
                //min time to stay on StandBy
                CL.IEX.Wait(double.Parse(standByWait));
                res = CL.EA.StandBy(true);

                if (!res.CommandSucceeded)
                {
                    FailStep(CL, res, "Failed to exit standby");
                }
            }

            //Channel Surf to Video Service to be on 
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel With DCA: " + videoService.LCN);
            }
            
            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * @class   Step1
     *
     * @brief   Tune to service with locked program and verify Start time, End times, 
     *          Recording indication, Parental rating, Message indicating the program is locked
     *
     * @author  Varshad
     * @date    11-Sep-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Tune to service with locked program and verify Start time, End times, 
     *              Recording indication, Parental rating, Message indicating the program is locked
         *
         * @author  Varshad
         * @date    11-Sep-13
         */

        public override void Execute()
        {
            StartStep();

            //Clearing EPG info
            res = CL.IEX.MilestonesEPG.ClearEPGInfo();
            if (!res.CommandSucceeded)
            {
                LogCommentWarning(CL, "Failed to Clear EPG Info");
            }

            //Tune to locked program 
            res = CL.EA.TuneToChannel(adultChannel.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + adultChannel.LCN);
            }

            // Verify if the alternate title is displayed as event name
            string obtainedValue = "";
            //Get Event Name from channel bar
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtname", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event name from channel bar");
            }
            LogComment(CL, "Obtained Real Title after unlocking: " + obtainedValue);

            if (!obtainedValue.Equals(expectedAdultEventName))
            {
                FailStep(CL, "Alternate title is not displayed. Obtained value is: " + obtainedValue);
            }

            //Verify Parental rating : ParentalRatingUrl
            res = CL.IEX.MilestonesEPG.GetEPGInfo("ParentalRatingUrl", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get ParentalRatingUrl from channel bar");
            }

            if (string.IsNullOrEmpty(obtainedValue))
            {
                FailStep(CL, "Parental rating URL is null or empty: ");
            }
            LogCommentInfo(CL, "Obtained Prenatl rating URL is: " + obtainedValue);

            //Verify Start time and end time
            res = CL.IEX.MilestonesEPG.GetEPGInfo("evtTime", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get event time from channel bar");
            }

            if (string.IsNullOrEmpty(obtainedValue))
            {
                FailStep(CL, "Event time is null or empty: ");
            }
            LogCommentInfo(CL, "Obtained event time is: " + obtainedValue);         

            //Verify Recording indication
            res = CL.IEX.MilestonesEPG.GetEPGInfo("RecordingStatus", out obtainedValue);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get Recording indication from channel bar");
            }

            if (!recordOngoing.Equals(obtainedValue))
            {
                FailStep(CL, "Recording indication is: "+obtainedValue);
            }
            LogCommentInfo(CL, "Obtained Recording indication is: " + obtainedValue);

            PassStep(); 
        }
    }
    #endregion

    #endregion

    #region PostExecute

    /**
     * @fn  public override void PostExecute()
     *
     * @brief   Stop ongoing recording
     *
     * @author  Varshad
     * @date    11-Sep-13
     */

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        
        //Stop Recording
        res = CL.EA.PVR.StopRecordingFromBanner("Event");
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to stop the record");
        }
    }
    #endregion
}