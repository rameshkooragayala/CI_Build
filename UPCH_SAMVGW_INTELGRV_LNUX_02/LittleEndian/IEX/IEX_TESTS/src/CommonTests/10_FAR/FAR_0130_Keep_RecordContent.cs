/// <summary>
///  Script Name : FAR_0130_Keep_RecordContent.cs
///  Test Name   : FAR-0130-Disk Formatting with Keep recorded content
///  TEST ID     : 68966
///  QC Version  : 1
///  Variations from QC: None
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
using FailuresHandler;
using IEX.Tests.Reflections;

/**
 * @class   FAR_0130
 *
 * @brief   Far 0130.
 *
 * @author  Varshad
 * @date    08-Oct-13
 */

[Test("FAR_0130_Keep_RecordContent")]
public class FAR_0130 : _Test
{
    /**
     * @property    static _Platform CL,GW
     *
     * @brief   Gets the gw.
     *
     * @return  The gw.
     */

    [ThreadStatic]
    static _Platform CL, GW;

    /**
     * @brief   The ishomenet.
     */

    static bool ishomenet = false;

    /**
     * @brief   The video service.
     */

    static Service videoService;
	
	private static string defaultPin;

    /**
     * @brief   Precondition: Create a record content and booking.
     */

    private const string PRECONDITION_DESCRIPTION = "Precondition: Create a record content and booking";

    /**
     * @brief   Step 1: Perform Factory reset.
     */

    private const string STEP1_DESCRIPTION = "Step 1: Perform Factory reset";

    /**
     * @brief   Step 2: Verify recorded content is present even after factory reset.
     */

    private const string STEP2_DESCRIPTION = "Step 2: Verify recorded content is present even after factory reset";

    /**
     * @brief   Step 3: Verify booked event is not present planner
     */

    private const string STEP3_DESCRIPTION = "Step 3: Verify booked event is not present planner";

    /**
     * @class   Constants
     *
     * @brief   Constants.
     *
     * @author  Varshad
     * @date    08-Oct-13
     */

    private static class Constants
    {
        /**
         * @brief   in seconds.
         */

        public const int waitPeriodForRecord = 60;

        /**
         * @brief   in mins.
         */

        public const int minTimeBeforeEvtEnds = 2;

        /**
         * @brief   number of Presses.
         */
        public const int nofPresses = 10;

        /**
         * @brief   The keep recordingd.
         */
        public const bool keepRecordings = true;

        /**
         * @brief   The keep settings.
         */
        public const bool keepSettings = false;
		

    }

    #region Create Structure

    /**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Varshad
     * @date    08-Oct-13
     */

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);
        this.AddStep(new Step3(), STEP3_DESCRIPTION);

        //Get Client
        CL = GetClient();
        string isHomeNetwork = CL.EA.GetTestParams("IsHomeNetwork");

        //If Home network is true perform GetGateway
        ishomenet = Convert.ToBoolean(isHomeNetwork);
        if (ishomenet)
        {
            //Get gateway platform
            GW = GetGateway();
        }
    }
    #endregion

    #region PreExecute

    /**
     * @fn  public override void PreExecute()
     *
     * @brief   Pre execute.
     *
     * @author  Varshad
     * @date    08-Oct-13
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
     * @brief   Create a record content and booking.
     *
     * @author  Varshad
     * @date    08-Oct-13
     */

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    08-Oct-13
         */

        public override void Execute()
        {
            StartStep();
			
			defaultPin = CL.EA.GetValueFromINI(EnumINIFile.Environment, "IEX", "DefaultPIN");
            if (string.IsNullOrEmpty(defaultPin))
            {
                FailStep(CL, "Failed to fetch DefaultPIN from Environment.ini");
            }

            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video service fetched from Content.xml is null");
            }
			
			res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel - " + videoService.LCN);
            }
			
            //Record and Event
            res = CL.EA.PVR.RecordCurrentEventFromBanner("Event", Constants.minTimeBeforeEvtEnds);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to record an event");
            }

            LogCommentInfo(CL, "Waiting for " + Constants.waitPeriodForRecord + " seconds to record an event");
            res = CL.IEX.Wait(Constants.waitPeriodForRecord);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to wait for " + Constants.waitPeriodForRecord);
            }

            res = CL.EA.PVR.StopRecordingFromBanner("Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to stop recording from banner");
            }

            //Verify event in Archive
            res = CL.EA.PVR.VerifyEventInArchive("Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to verify an event in archieve");
            }

            res = CL.EA.PVR.BookFutureEventFromGuide("Future_Event", videoService.LCN, Constants.nofPresses);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to book a future Event from guide");
            }

            //Verify Event in planner
            res = CL.EA.PVR.VerifyEventInPlanner("Future_Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res.FailureReason);
            }

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**
     * @class   Step1
     *
     * @brief   Perform Factory reset
     *
     * @author  Varshad
     * @date    08-Oct-13
     */

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    08-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //Perform Factory reset with Keep Recording option Yes
            res = CL.EA.STBSettings.FactoryReset(Constants.keepRecordings, Constants.keepSettings, defaultPin);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to perform factory reset");
            }

            if (ishomenet)
            {
                res = GW.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount gateway");
                }
                res = CL.EA.MountClient(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount client");
                }
            }
            else
            {
                res = CL.EA.MountGw(EnumMountAs.FACTORY_RESET);
                if (!res.CommandSucceeded)
                {
                    FailStep(CL, "Failed to mount cleint");
                }
            }

            PassStep();
        }
    }
    #endregion

    #region Step2

    /**
     * @class   Step2
     *
     * @brief   Verify recorded content is not present even after factory reset.
     *
     * @author  Varshad
     * @date    08-Oct-13
     */

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    08-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //After factory reset Verify Whether recording are present in My Recordings.
            //Recordings should be present
            res = CL.EA.PVR.VerifyEventInArchive("Event");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Records is present in Archive. Supposed to be deleted ");
            }

            PassStep();
        }
    }
    #endregion
    #region Step3

    /**
     * @class   Step3
     *
     * @brief    Verify booked event is not present planner.
     *
     * @author  Varshad
     * @date    08-Oct-13
     */

    [Step(3, STEP3_DESCRIPTION)]
    public class Step3 : _Step
    {
        /**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Varshad
         * @date    08-Oct-13
         */

        public override void Execute()
        {
            StartStep();

            //Verify that Bookings are deleted
            res = CL.EA.PVR.VerifyEventInPlanner("Future_Event", true, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Bookings is present in Planner. Booked events are supposed to be deleted after factory reset ");
            }

            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute

    /**
     * @fn  public override void PostExecute()
     *
     * @brief   Posts the execute.
     *
     * @author  Varshad
     * @date    08-Oct-13
     */

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        res = CL.EA.PVR.DeleteAllRecordsFromArchive();
        if (!res.CommandSucceeded)
        {
            CL.IEX.LogComment("Failed to deleted all records from archieve");
        }

    }
    #endregion
}