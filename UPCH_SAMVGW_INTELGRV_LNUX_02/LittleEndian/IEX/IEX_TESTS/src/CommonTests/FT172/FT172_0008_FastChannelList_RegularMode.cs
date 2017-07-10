/// <summary>
///  Script Name        : FT172_0008_fast_channel_list_regular_mode.cs
///  Test Name          : FT172-0008-DCA-in-fast-channel-list-regular-mode
///  TEST ID            : 74273
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 21st June, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class FT172_0008 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static string Error_Low_service;
    private static string Error_High_service;
    private static string Error_Middle_service;
    private static string NextTo_Error_Middle_Service;
    private static Service FvrtService_1;
    private static Service FvrtService_2;
    private static Service Service_1;
    private static string Lowest_Service_Number;
    private static string Highest_Service_Number;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        //Performs DCA surfing with Error channel number
        //Based on QualityCenter - 3
        //Variations from QualityCenter: None
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml and ini File, and set two service as Favourite");
        this.AddStep(new Step1(), "Step 1: Channel Surf to the service which is Favourite, and verify Service LCN");
        this.AddStep(new Step2(), "Step 2: Channel Surf to the service which is not Favourite, and verify Service LCN");
        this.AddStep(new Step3(), "Step 3: Channel Surf With Low Digit Channel Number, not exist");
        this.AddStep(new Step4(), "Step 4: Channel Surf With Mid Digit Channel Number, not exist");
        this.AddStep(new Step5(), "Step 5: Channel Surf With Highest Digit Channel Number, not exist");

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

            //Get Values From ini File
            //Fetching the lowest service number from the channels ini
            Lowest_Service_Number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Lowest_Service_Number");
            if (string.IsNullOrEmpty(Lowest_Service_Number))
              {
                  Lowest_Service_Number = CL.EA.GetValue("Lowest_Service_Number");
                  if (Lowest_Service_Number == "")
                  {
                      FailStep(CL, "Lowest_Service_Number is not defined in the channels ini");
                  }
              }
            
            LogComment(CL, "Lowest_Service_Number fetched from channels ini is " + Lowest_Service_Number);

            //Fetching the max service number from the channels ini
            Highest_Service_Number = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "Highest_Service_Number");
            if (string.IsNullOrEmpty(Highest_Service_Number))
            {
                Highest_Service_Number = CL.EA.GetValue("Highest_Service_Number");
                if (Highest_Service_Number == "")
                {
                    FailStep(CL, "Highest_Service_Number is not defined in the channels ini");
                }
            }

            LogComment(CL, "Highest_Service_Number fetched from channels ini is " + Highest_Service_Number);

            //Fetching the service in between the channel line up which does not exist
            Error_Middle_service = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ERROR_MIDDLE_SERVICE");
            if (Error_Middle_service == "")
            {
                FailStep(CL, "Error_Middle_service is not defined in the test ini");
            }
            else
            {
                LogComment(CL, "Error_Middle_service fetched from test ini is " + Error_Middle_service);
            }
            //Fetching the next available service after the error middle service which does not exist
            NextTo_Error_Middle_Service = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "NEXT_TO_ERROR_MIDDLE_SERVICE");
            if (NextTo_Error_Middle_Service == "")
            {
                FailStep(CL, "NEXT_TO_ERROR_MIDDLE_SERVICE is not defined in the test ini");
            }
            else 
            {
                LogComment(CL, "NextTo_Error_Middle_Service fetched from test ini is "+NextTo_Error_Middle_Service);
            }
            //Fetching the lowest service which is the least service LCN and which does not exist
            Error_Low_service = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ERROR_LOW_SERVICE");
            if (Error_Low_service == "")
            {
                FailStep(CL, "Error_Low_service is not defined in the test ini");
            }
            else
            {
                LogComment(CL, "Error_Low_service fetched from test ini is " + Error_Low_service);
            }

            //Fetching the highest service which is the maximum service LCN and which does not exist
            Error_High_service = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "ERROR_HIGH_SERVICE");
            if (Error_High_service == "")
            {
                FailStep(CL, "Error_High_service is not defined in the test ini");
            }
            else
            {
                LogComment(CL, "Error_High_service fetched from test ini is " + Error_High_service);
            }

            FvrtService_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (FvrtService_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL,"Favourite service fetched from content xml "+FvrtService_1.LCN);
            }

            FvrtService_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + FvrtService_1.LCN);
            if (FvrtService_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + FvrtService_2.LCN);
            }


            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + FvrtService_1.LCN + "," + FvrtService_2.LCN);
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "service fetched from content xml " + Service_1.LCN);
            }

            res = CL.EA.STBSettings.SetFavoriteChannelNameList(FvrtService_1.Name + "," + FvrtService_2.Name, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Service : " + FvrtService_1.LCN + "," + FvrtService_2.LCN + " as Favourite from Settings");
            }

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
            //Navigating to channel line up
            res = CL.IEX.MilestonesEPG.NavigateByName("STATE:CHANNEL LINEUP");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Navigate to Channel line Up");
            }
            CL.IEX.Wait(5);
            //After sending the sequence waiting for few seconds for tuning. We cant use ChannelSurf or Tune to channel as we are not getting the dca_number milestone
            CL.EA.UI.Utils.SendChannelAsIRSequence(FvrtService_1.LCN);

            CL.IEX.Wait(10);
            //Verify if the zapped channel is same as sent through IR sequence
            string chNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out chNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get channel number From Channel Bar");
            }
            if (chNum != FvrtService_1.LCN)
            {
                FailStep(CL, res, "Failed to zap to " + FvrtService_1.LCN);
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

            CL.EA.UI.Utils.SendChannelAsIRSequence(Service_1.LCN);

            CL.IEX.Wait(10);
            //Verify if the zapped channel is same as sent through IR sequence
            string chNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out chNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get channel number From Channel Bar");
            }
            if (chNum != Service_1.LCN)
            {
                FailStep(CL, res, "Failed to zap to " + FvrtService_1.LCN);
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

            // Tune With 1 Digit Channel Number
            CL.EA.UI.Utils.SendChannelAsIRSequence(Error_Low_service);

            CL.IEX.Wait(10);

            //Get Channel Number
            string ChNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out ChNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Number");
            }

            // Verify That The Current Channel Is The First One (Since there Is No 1 Digit Channel)
            if (ChNumber != Lowest_Service_Number)
            {
                FailStep(CL, "Received Differnt Channel Number (" + ChNumber + ") Than Expected  (" + Lowest_Service_Number + ")");
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

            // Tune to service which is not exist
            CL.EA.UI.Utils.SendChannelAsIRSequence(Error_Middle_service);
            //Get Channel Number
            CL.IEX.Wait(10);
            string ChNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out ChNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Number");
            }

            // Verify That The Current Channel Is The nearest Higher Channel
            if (ChNumber != NextTo_Error_Middle_Service)
            {
                FailStep(CL, "Received Differnt Channel Number (" + ChNumber + ") Than Expected  (" + Highest_Service_Number + ")");
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

            // Tune With highest Channel Number
            CL.EA.UI.Utils.SendChannelAsIRSequence(Error_High_service);

            CL.IEX.Wait(10);
            //Get Channel Number
            string ChNumber = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chnum", out ChNumber);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Get Channel Number");
            }

            // Verify That The Current Channel Is The Higher Channel which is exist
            if (ChNumber != Highest_Service_Number)
            {
                FailStep(CL, "Received Differnt Channel Number (" + ChNumber + ") Than Expected  (" + Highest_Service_Number + ")");
            }

            PassStep();
        }
    }


    #endregion Step5


    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //Removing all the channel numbers from the list
        res = CL.EA.STBSettings.SetFavoriteChannelNameList(FvrtService_1.Name + "," + FvrtService_2.Name, EnumFavouriteIn.Settings);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL,"Failed to unset the Favourite channel list from settings");
        }

    }

    #endregion PostExecute
}
