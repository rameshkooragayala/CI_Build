/// <summary>
///  Script Name : PC_0020_ChannelLock_PPContent.cs
///  Test Name   : PC-0020-Channel lock PP content
///  TEST ID     : 73393
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
using IEX.Tests.Reflections;

[Test("PC_0020")]
public class PC_0020 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    static Service videoService;
    static Service serviceTobeLocked;

    public const int timeToCheckForVideo = 2;

    static bool isLocked = false;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Manually lock a channel";
    private const string STEP1_DESCRIPTION = "Step 1: Tune to locked channel and check whether av is not present, Unlock and check av is present";

    #region Create Structure
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
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Failed to fetch service from Content.xml. " + videoService);
            }
            LogCommentInfo(CL, "Video Service fetched from conenet.xml is: " + videoService.LCN);

            serviceTobeLocked = CL.EA.GetServiceFromContentXML("Type=Video", "LCN=" + videoService.LCN + ";ParentalRating=High");
            if (serviceTobeLocked == null)
            {
                FailStep(CL, "Failed to fetch service from Content.xml. " + serviceTobeLocked);
            }
            LogCommentInfo(CL, "Video Service fetched from conenet.xml is: " + serviceTobeLocked.LCN);

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to surf to " + videoService.LCN);
            }

            res = CL.EA.STBSettings.SetLockChannel(serviceTobeLocked.Name);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to set lock on channel: " + serviceTobeLocked.LCN);
            }
            isLocked = true;

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
            
            res = CL.EA.TuneToLockedChannel(serviceTobeLocked.LCN, false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to " + serviceTobeLocked.LCN);
            }

            //Verify that video is not present 
            res = CL.EA.CheckForVideo(true,true,timeToCheckForVideo);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to verify that video is not present in PIP");
            }
            PassStep();
        }
    }
    #endregion

    #endregion

    #region PostExecute
    [PostExecute()]
    public override void PostExecute()
    {
        if (isLocked)
        {
            IEXGateway._IEXResult res;
           
            res = CL.EA.STBSettings.SetUnLockChannel(serviceTobeLocked.Name);
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to unlock on channel: " + serviceTobeLocked.LCN);
            }
        }
    }
    #endregion
}