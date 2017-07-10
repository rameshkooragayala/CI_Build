/// <summary>
///  Script Name : LIVE_0302_Zapping_LockedChannel.cs
///  Test Name   : LIVE-0302-Channel Change Locked Channel
///  TEST ID     : 
///  QC Version  : 
/// ----------------------------------------------- 
///  Modified by : Israel Itzhakov
///  Modified by : Anshul Upadhyay (23/9/2013)
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
 * @class   LIVE_0302
 *
 * @brief   Live 0302.
 *
 * @author  Anshulu
 * @date    01/10/13
 **************************************************************************************************/

public class LIVE_0302 : _Test
{
    /**********************************************************************************************//**
     * @brief   The cl.
     **************************************************************************************************/

    [ThreadStatic]
    static _Platform CL;

    /**********************************************************************************************//**
     * @brief   The second all PC events.
     **************************************************************************************************/

    static string All_PC_Events_2;

    /**********************************************************************************************//**
     * @brief   The second fta 1st multiplexer.
     **************************************************************************************************/

    static string FTA_1st_Mux_2;

    /**********************************************************************************************//**
     * @brief   The is channel locked.
     **************************************************************************************************/

    static bool isChannelLocked = false;

    /**********************************************************************************************//**
     * @brief   The is channel unlocked.
     **************************************************************************************************/

    static bool isChannelUnlocked = false;

    /**********************************************************************************************//**
     * @brief   Name of the locked.
     **************************************************************************************************/

    static string lockedName = "";

    #region Create Structure

    /**********************************************************************************************//**
     * @fn  public override void CreateStructure()
     *
     * @brief   Creates the structure.
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    public override void CreateStructure()
    {
        //Variations from QualityCenter: We blocks the channel, not PP rated, since we don't have stable channel for that. But this is the same funtionality and the same test

        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From ini File ");
        this.AddStep(new Step1(), "Step 1: Block Channel S1 ");
        this.AddStep(new Step2(), "Step 2: Tune To S1, Verify It Is Blocked, Enter Pin Code, Verify AV Is Seen");
        
        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region PreCondition

    /**********************************************************************************************//**
     * @class   PreCondition
     *
     * @brief   Precondition: Get Channel Numbers From ini File
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    private class PreCondition : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Get Values From ini File
            All_PC_Events_2 = CL.EA.GetValue("All_PC_Events_2");
            FTA_1st_Mux_2 = CL.EA.GetValue("FTA_1st_Mux_2");

            PassStep();
        }
    }
    #endregion
    #region Step1

    /**********************************************************************************************//**
     * @class   Step1
     *
     * @brief   Step 1: Block Channel S1
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    private class Step1 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();


            //Tune to a service, then lock it and verify that PIN Code is not asked for
            //Enter a Valid PIN Code
            //Verify PIN Screen is Removed and Video is Resumed

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, All_PC_Events_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }
            //get channel name
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chname", out lockedName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Name");
            }

            //lock the channel
            res = CL.EA.STBSettings.SetLockChannel(lockedName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Lock Channel");
            }
            isChannelLocked = true;
            PassStep();
        }
    }
    #endregion
    #region Step2

    /**********************************************************************************************//**
     * @class   Step2
     *
     * @brief   Step 2: Tune To S1, Verify It Is Blocked, Enter Pin Code, Verify AV Is Seen
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    private class Step2 : _Step
    {
        /**********************************************************************************************//**
         * @fn  public override void Execute()
         *
         * @brief   Executes this object.
         *
         * @author  Anshulu
         * @date    01/10/13
         **************************************************************************************************/

        public override void Execute()
        {
            StartStep();

            //Tune to another live Service and then tune back to the locked channel           
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FTA_1st_Mux_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to FTA Channel With DCA");
            }

            res = CL.EA.TuneToLockedChannel(All_PC_Events_2, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to a  locked Channel");
            }

            //unlock the channel
            res = CL.EA.STBSettings.SetUnLockChannel(lockedName);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to unLock Channel");
            }
            isChannelUnlocked = true;
            PassStep();
        }
    }
    #endregion
    #endregion

    #region PostExecute

    /**********************************************************************************************//**
     * @fn  public override void PostExecute()
     *
     * @brief   unlock the channel
     *
     * @author  Anshulu
     * @date    01/10/13
     **************************************************************************************************/

    [PostExecute()]
    public override void PostExecute()
    {
        LogComment(CL, "Enter PostExecute");

        IEXGateway._IEXResult res;
        if (!isChannelLocked == true && isChannelUnlocked == false)
        {
            res = CL.EA.STBSettings.SetUnLockChannel(lockedName);
            if (!res.CommandSucceeded)
            {
              CL.IEX.FailStep("Failed to unlock channel");
            }
        }

        LogComment(CL, "Exit PostExecute");
    
    }
    #endregion
}