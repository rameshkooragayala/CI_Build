/// <summary>
///  Script Name        : FT148_0161_ReminderNotification_ZappingToNonFavorites.cs
///  Test Name          : FT148-0161-Reminder notification & zapping among to non-Favorites
///  TEST ID            : 74671
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 12th Sept, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;

public class FT148_0161 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test
    private static Service FvrtService_1;
    private static Service FvrtService_2;
    private static Service Service_1;

    #region Create Structure

    public override void CreateStructure()
    {
        //Brief Description:
        this.AddStep(new PreCondition(), "Precondition: Get Channel Numbers From xml and set two service as Favourite, enable favourite mode and book future reminder");
        this.AddStep(new Step1(), "Step 1: Wait Until the Reminder and accept the reminder");
        this.AddStep(new Step2(), "Step 2: Verify that you tuned to the non Favourite service");

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region Steps

    #region PreCondition

    private class PreCondition : _Step
    {
        // Enable favorite mode in action menu
        private static bool enableFavoriteMode()
        {
            try
            {
                CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ENABLE FAVOURITE MODE");
                return true;
            }
            catch (Exception)
            {
                try
                {
                    CL.EA.UI.Utils.EPG_Milestones_NavigateByName("STATE:ACTION BAR");
                    if (CL.EA.UI.Utils.EPG_Milestones_SelectMenuItem("DISABLE FAVOURITE MODE"))
                    {
                        CL.EA.UI.Utils.SendIR("RETOUR");
                        return true;
                    }
                }
                catch (Exception) { }
            }
            return false;
        }
        public override void Execute()
        {
            StartStep();

            //Get Values From ini File

            FvrtService_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High");
            if (FvrtService_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + FvrtService_1.LCN);
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
            res = CL.EA.BookReminderFromGuide("ReminderEvent", Service_1.LCN, NumberOfPresses: 2, VerifyBookingInPCAT: false);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to book reminder from Guide");
            }
            res = CL.IEX.Wait(60);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,"");
            }
            res = CL.EA.STBSettings.SetFavoriteChannelNumList(FvrtService_1.LCN + "," + FvrtService_2.LCN, EnumFavouriteIn.Settings);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Set Service : " + FvrtService_1.LCN + "," + FvrtService_2.LCN + " as Favourite from Settings");
            }
            
            //enable favorite mode 
            enableFavoriteMode();
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
            //Step 1  tune to service 1 
            CL.EA.ChannelSurf(EnumSurfIn.Live, "1");
            res = CL.EA.WaitUntilReminder("ReminderEvent");
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to wait until reminders");
            }
            res = CL.EA.HandleReminder("ReminderEvent", EnumReminderActions.Accept);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to handle reminder");
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

            res = CL.EA.ChannelSurf(EnumSurfIn.Live, "");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + FvrtService_1.LCN);
            }
            string chNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out chNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get the EPG Info from EPG");
            }
            if (chNum == "1" || chNum == "2")
            {
                FailStep(CL, "Still in favourite mode after tuning");
            }
            else
            {
                LogCommentImportant(CL,"Exited Favourite mode and we are currently present in Non-Favourite mode");
            }

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {
        IEXGateway._IEXResult res;
        //Removing all the channel numbers from the list
        res = CL.EA.STBSettings.UnsetFavoriteChannelNumList(FvrtService_1.LCN + "," + FvrtService_2.LCN, EnumFavouriteIn.Settings);
        if (!res.CommandSucceeded)
        {
            LogCommentFailure(CL, "Failed to unset the Favourite channel list from settings");
        }
    }

    #endregion PostExecute
}
