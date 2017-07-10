using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//Robustness test - Tune Via TV Guide
public class Robustness_TuneViaTVGuide : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Shared members between steps
    //Channels used by the test
    private static Service videoService;
   
    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: Robustness to tests Zapping

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Sync");
        this.AddStep(new Step1(), "Step 1: Tune via TV Guide in loop for 400 times ");


        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
			
		
            //Get Values From ini File
            videoService = CL.EA.GetServiceFromContentXML("Type=Video;IsRecordable=True", "ParentalRating=High");
            if (videoService == null)
            {
                FailStep(CL, "Video service fetched from Content.xml is null");
            }

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to surf to Video service: " + videoService.LCN);  
            }

            PassStep();
        }
    }
    #endregion
    #region Step1
    //create not predicted by DCA to the other direction 

    private class Step1 : _Step
    {
        //Step 1: Channel UP 10 times and channel down 10 times in loop
        public override void Execute()
        {

        StartStep();
        for (int iteration = 1; iteration < 400; )
        {   
			res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to surf to Video service: " + videoService.LCN);  
            }
                 CL.IEX.Wait(20);
 
            //Zap 4 times from Guide.          
            for (int zaps_up = 1; zaps_up <= 4; zaps_up++)
               {
				res = CL.EA.ChannelSurf(EnumSurfIn.Guide, "", true, 2, EnumPredicted.Default, true);
				if (!res.CommandSucceeded)
				{
                FailStep(CL, res, "Failed to Tune to The Next Service from Guide");
				}

                 CL.IEX.Wait(25);

                LogCommentInfo(CL, "Tune Via TV Guide iterations: " + iteration);
 
                iteration = iteration + 1;

				}

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