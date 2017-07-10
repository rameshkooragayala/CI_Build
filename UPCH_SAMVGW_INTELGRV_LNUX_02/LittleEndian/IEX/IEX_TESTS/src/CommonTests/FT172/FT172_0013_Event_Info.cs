/// <summary>
///  Script Name : FT172_0013_Event_Info.cs
///  Test Name   : FAV-FT172-0013-Info-in-GRID
///  TEST ID     : 74645
///  QC Version  : 2
/// -----------------------------------------------
///  Modified by : Madhu Kumar k
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using System.Collections.Generic;

public class FT172_0013 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service FvrtService_1;
    private static Service FvrtService_2;
    private static Service FvrtService_3_Locked;
    private static Service Locked_Service_1;
    private static string namedNavigationGrid;
    static Helper helper = new Helper();

    private static Dictionary<string, string> infoDictionary = new Dictionary<string, string>();
    #region Create Structure

    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml and ini File, and set three services as Favourite and lock one of them");
        this.AddStep(new Step1(), "Step1: Navigate to TV Guide in Favourite Mode");
        this.AddStep(new Step2(), "Step2: Verify the EPG Info of Service S1");
        this.AddStep(new Step3(), "Step3: Tune to Service S2 in Guide and verify the EPG info");
        this.AddStep(new Step4(), "Step4: Disable favourite mode");
        this.AddStep(new Step5(), "Step5: Navigate to Locked service S3 in Guide and verify the EPG info");
        this.AddStep(new Step6(), "Step6: Navigate to Locked Programme S4 in Guide and verify the EPG info");
        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();
            FvrtService_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;HasChannelLogo=True", "ParentalRating=High");
            if (FvrtService_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + FvrtService_1.LCN);
            }

            FvrtService_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;HasChannelLogo=False", "ParentalRating=High");
            if (FvrtService_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + FvrtService_2.LCN);
            }

           string serviceToBeLocked = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "SERVICE_LCN");
           if (serviceToBeLocked == "")
            {
                FailStep(CL, "SERVICE_LCN is not defined in the Test ini");
            }

            FvrtService_3_Locked = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;HasChannelLogo=True;LCN="+serviceToBeLocked, "ParentalRating=High");
            if (FvrtService_3_Locked == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + FvrtService_3_Locked.LCN);
            }
            Locked_Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;ParentalRating=High", "");
            if (Locked_Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Locked_Service_1.LCN);
            }
            namedNavigationGrid = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NAMED_NAVIGATION_GRID");
            if (namedNavigationGrid == "")
            {
                FailStep(CL, "NAMED_NAVIGATION_GRID is not defined in the Test ini");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            //Unsetting all the favourite services
            res = CL.EA.STBSettings.UnsetAllFavChannels();
            if (!res.CommandSucceeded)
            {
                LogCommentFailure(CL, "Failed to UnsetAllFavChannels");
            }
            res = CL.EA.STBSettings.SetFavoriteChannelNameList(FvrtService_1.Name + "," + FvrtService_2.Name + "," + FvrtService_3_Locked.Name, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Service : " + FvrtService_1.LCN + "," + FvrtService_2.LCN + " as Favourite from Settings");
            }
            res = CL.EA.STBSettings.SetLockChannel(FvrtService_3_Locked.Name);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to lock the Channel service " + FvrtService_3_Locked.LCN + " from settings");
            }
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, FvrtService_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + FvrtService_1.LCN);
            }
            //Enabling the Favourite mode
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:ENABLE FAVOURITE MODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable the favourite mode");
            }
            //Tuning to service 1 in favourite mode
            CL.EA.ChannelSurf(EnumSurfIn.Live, "1");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }
            //Waiting 5 seconds for the grid to complete launch and for milestones to arrive
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }

            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Adding the Key and value and verifying it with the EPG
            infoDictionary.Add("channel_logo", FvrtService_1.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", "1");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }
            PassStep();
        }
    }

    #endregion Step2

    #region Step3

    private class Step3 : _Step
    {

        public override void Execute()
        {
            StartStep();
            infoDictionary.Clear();
            //tuning to service by sending keys
            CL.EA.UI.Utils.SendChannelAsIRSequence("2");
            //Waiting few seconds to tune
            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            //Adding to the dictionary and verifying the EPG info
            infoDictionary.Add("chname", FvrtService_2.Name);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", "2");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                LogCommentFailure(CL, "Please check the above failures");
            }

            PassStep();
        }
    }

    #endregion Step3
    #region Step4

    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Disabling the Favourite mode
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:DISABLE FAVOURITE MODE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to enable the favourite mode");
            }

            PassStep();
        }
    }


    #endregion Step4

    #region Step5

    private class Step5 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //Navigating to TV Guide
            res = CL.IEX.MilestonesEPG.NavigateByName(namedNavigationGrid);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to navigate to state TV Guide");
            }
            res = CL.IEX.Wait(5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            //Tune to the locked favourite service
            CL.EA.UI.Utils.SendChannelAsIRSequence(FvrtService_3_Locked.LCN);
            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }
            infoDictionary.Clear();
            infoDictionary.Add("channel_logo", FvrtService_3_Locked.ChannelLogo);
            infoDictionary.Add("IsFavourite", "True");
            infoDictionary.Add("chNum", FvrtService_3_Locked.LCN);
            infoDictionary.Add("evtname", "LOCKED CHANNEL");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");  
            }
            PassStep();
        }
    }


    #endregion Step5
    #region Step6

    private class Step6 : _Step
    {
        public override void Execute()
        {
            StartStep();
            //tuning to a locked program
            CL.EA.UI.Utils.SendChannelAsIRSequence(Locked_Service_1.LCN);
            res = CL.IEX.Wait(10);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to wait few seconds");
            }

            infoDictionary.Clear();
            infoDictionary.Add("chname", Locked_Service_1.Name);
            infoDictionary.Add("chNum", Locked_Service_1.LCN);
            infoDictionary.Add("evtname", "LOCKED PROGRAMME");
            infoDictionary.Add("IsFavourite", "False");
            if (!helper.VerifyEpgInfo(infoDictionary))
            {
                FailStep(CL, "Please check the above failures");
            }
            PassStep();
        }
    }


    #endregion Step6

    #endregion Steps


    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        int count = 1;

        res = CL.EA.STBSettings.SetUnLockChannel(FvrtService_3_Locked.Name);
        if (!res.CommandSucceeded)
        {
            while (res.CommandSucceeded == true || count < 5)
            {
                LogComment(CL, "Attempt " + count);
                CL.EA.ReturnToLiveViewing();
                res = CL.EA.STBSettings.SetUnLockChannel(FvrtService_3_Locked.Name);
                count++;
            }

        }
    }


    #endregion PostExecute
    #region Helper
    public class Helper : _Step
    {
        public override void Execute() { }

        /// <summary>
        /// Verifies the Expected info with the obtained EPG info
        /// </summary>
        /// <returns>bool</returns>
        public bool VerifyEpgInfo(Dictionary<string, string> infoDictionary)
        {
            string obtainedValue = "";
            bool isPass = true;
            foreach (KeyValuePair<string, string> keyValue in infoDictionary)
            {
                switch (keyValue.Key)
                {
                    case "channel_logo":
                        string obtaionedChannelLogo = CL.EA.GetChannelLogo();
                        if (obtaionedChannelLogo != keyValue.Value)
                        {
                            LogCommentFailure(CL, "Channel logo fetched is different " + obtaionedChannelLogo + " from the expected" + keyValue.Value);
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, "Channel logo fetched " + obtaionedChannelLogo + " is same as expected" + keyValue.Value);
                        }
                        break;
                    default:
                        CL.IEX.MilestonesEPG.GetEPGInfo(keyValue.Key, out obtainedValue);
                        if (keyValue.Value != obtainedValue)
                        {
                            LogCommentFailure(CL, "obtained Value " + obtainedValue + " is different from Expected " + keyValue.Value);
                            isPass = false;
                        }
                        else
                        {
                            LogCommentImportant(CL, keyValue.Key + " fetched from EPG" + keyValue.Value + " is same as expected " + obtainedValue);
                        }

                        break;
                }
            }
            return isPass;
        }
    }

    #endregion
}

    

