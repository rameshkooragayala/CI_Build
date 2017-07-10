// ----------------------------------------------- 
//  Script Name : Thumbnails on Action Menu
//  Source test name : Action -Menu-Live-viewing-Info
//  MQC Project : FR_FUSION \ UPC 
//  TEST ID : LightSanity_280
//  Variation from Quality Center : None 
// ----------------------------------------------- 
//  Scripted by : Madhu Renukaradhya 
//  Last modified : 02 May 2013 
// -----------------------------------------------

using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-280-EPG-Thumbnail display
public class FullSanity_0280 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    //Test Duration

    //Channels used by the test
    static string Thumbnail_ch;
    static string DefaultThumbnail;


    //Shared members between steps

    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description:
        //Pre-conditions:
        //Based on QualityCenter test version 
        //Variations from QualityCenter: 
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File & Mount GW");
        this.AddStep(new Step1(), "Step 1: Tune to service S1");
        this.AddStep(new Step2(), "Step 2: Launch action menu and verify Thumbnail & Synopsis of the event is displayed.");

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
            Thumbnail_ch = CL.EA.GetValue("Thumbnail_ch");
            CL.IEX.LogComment("Retrieved Value From ini File: Thumbnail_ch = " + Thumbnail_ch);

            DefaultThumbnail = CL.EA.GetValue("DefaultThumbnail");
            CL.IEX.LogComment("Retrieved Value From ini File: DefaultThumbnail = " + DefaultThumbnail);

            //{
            //    bool ret;
            //    string cmd;
            //    string cmdc_server_ip = "10.201.96.23";
            //    ret = CL.EA.TelnetLogIn(false);
            //    if (ret == false)
            //    {
            //        FailStep(CL, res, "Failed to TELNET the STB");
            //    }

            //    cmd = "cd /NDS/config/; sed -i 's/CMDC_SERVER=\"\"/CMDC_SERVER=\""+"\\\\"+cmdc_server_ip+"\"/g' spm.cfg";
            //    ret = CL.EA.SendCmd(cmd, false);
            //    if (ret == false)
            //    {
            //        FailStep(CL, res, "Failed to change the spm.cfg file");
            //    }

            //    cmd = "../bin/FusionConfigImport CFG2BIN ./ *";
            //    ret = CL.EA.SendCmd(cmd, false);
            //    if (ret == false)
            //    {
            //        FailStep(CL, res, "Failed to conver cfg 2 bin");
            //    }

            //    CL.EA.TelnetDisconnect(false);
            //}

            PassStep();
        }
    }
    #endregion
    #region Step1
    private class Step1 : _Step
    {
        //Step 1: Tune to Channel S1
        public override void Execute()
        {
            StartStep();

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Thumbnail_ch);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel " + Thumbnail_ch);
            }

            res = CL.EA.CheckForVideo(true, false, 10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Verify Video is Present " + Thumbnail_ch + " After DCA");
            }

            PassStep();
        }
    }
    #endregion
    #region Step2
    private class Step2 : _Step
    {
        //Step 2: Navigate to Action Menu and verify thumnail and Synopsis is displayed.
        public override void Execute()
        {

            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ACTION BAR");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to launch Action Menu ");
            }
            CL.IEX.Wait(10);

            string thumbnail = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("thumbnail", out thumbnail);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get thumbnail_url from Action Menu" + thumbnail);
            }

            //Validate whether the thumbnail received is default or not
            if (string.IsNullOrEmpty(thumbnail) || thumbnail.Equals(DefaultThumbnail) || thumbnail.Length.Equals(0))
            {
                FailStep(CL, res, "Thumbnail received is either null or default thumbnail " + thumbnail);

            }
            else
            {
                CL.IEX.LogComment("Thumbnail Received " + thumbnail);
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