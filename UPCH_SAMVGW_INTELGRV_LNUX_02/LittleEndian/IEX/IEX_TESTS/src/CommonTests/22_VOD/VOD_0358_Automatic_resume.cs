/// <summary>
///  Script Name : VOD-0358-Automatic-resume  
///  Test Name   : VOD-0358-Automatic-resume  
///  TEST ID     : 74514
///  QC Version  : 2
///  QC Domain   : FR_FUSION
///  QC Project  : UPC
///  QC Path     : 
/// -----------------------------------------------
///  Modified by : Achraf Harchay
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;
using IEX.Tests.Reflections;

[Test("VOD358")]
class VOD358 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static VODAsset vodAsset;



    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Play/Pause an asset for T time");




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
            // Get a VOD asset with several  subtitle languages
            vodAsset = CL.EA.GetVODAssetFromContentXML("Type=TVOD");
            if (vodAsset == null)
            {
                FailStep(CL, res, "Failed to get a VOD asset from ini file");
            }

            // Set purchase protection to ON
            res = CL.EA.STBSettings.SetPurchaseProtection(true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable purchase protection");
            }

            // Set parental rating threshold to 'Unlock all'
            res = CL.EA.STBSettings.SetParentalControlAgeLimit(EnumParentalControlAge.UNLOCK_ALL);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Parental Control Age Limit to UNLOCK ALL");
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
            // Play an asset 
            res = CL.EA.VOD.PlayAsset(vodAsset);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to play an asset ");
            }
            CL.IEX.Wait(10);
            string timeStamp = "";
           res= CL.IEX.SendIRCommand("PAUSE", -1, ref timeStamp);
           if (!res.CommandSucceeded)
           {
               FailStep(CL,res,"Failed to send the IR command PAUSE");
           }

            //res = CL.EA.PVR.SetTrickModeSpeed("RB", 0, false);
            //if (!res.CommandSucceeded)
            //{
            //    FailStep(CL, res, "Failed perform pause on vod asset");
            //}
            CL.IEX.Wait(840); // wait for 14 minute 


            // Get the lowest forward speed
            string minFwdSpeed = CL.EA.GetValueFromINI(EnumINIFile.Milestones, "MILESTONES", "TrickModeSpeed_0or1");
            if (string.IsNullOrEmpty(minFwdSpeed))
            {
                FailStep(CL, "Minimum forward speed not present in Project.ini file.");
            }


            CL.IEX.Wait(5);


            // Check any of the component goes to shutdown run level
            res = CL.IEX.Debug.BeginWaitForMessage(minFwdSpeed + "1000", 0, 5 * 60, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res);
            }

            string actual_msg, maginlines;
            res = CL.IEX.Debug.EndWaitForMessage(minFwdSpeed + "1000", out actual_msg, out maginlines, IEXGateway.DebugDevice.Udp);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed To Verify Message :" + actual_msg);
            }

            PassStep();
        }
    }
    #endregion


}





