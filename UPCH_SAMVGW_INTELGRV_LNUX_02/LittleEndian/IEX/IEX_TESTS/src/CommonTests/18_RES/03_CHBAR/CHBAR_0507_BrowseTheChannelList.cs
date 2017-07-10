/// <summary>
///  Script Name : CHBAR_0507_BrowseTheChannelList.cs
///  Test Name   : EPG-0507-Channel Bar-Browse The Channel List
///  TEST ID     : 63838
///  JIRA TASK   : FC-317
///  QC Version  : 1
///  Variations from QC: NONE
/// -----------------------------------------------
///  Modified by : Varsha Deshpande
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using IEX.Tests.Reflections;

[Test("CHBAR_0507_ChannelBrowse_FromChannelList")]
public class CHBAR_0507 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Shared members between steps
    private static Service videoService;
    private static EnumChannelBarTimeout maxChannelBarTimeOutVal;
    private static EnumChannelBarTimeout defaultChannelBarTimeOutVal;
    static string isSetBannerDisplayTimeSupporetd;

    private const string PRECONDITION_DESCRIPTION = "Precondition: Get Channel Numbers From ini File";
    private const string STEP1_DESCRIPTION = "Step 1: Launch channel bar, focus on another service and verify PIP";
    private const string STEP2_DESCRIPTION = "Step 2: Launch Channel List and zap to another service from channel list";

    #region Create Structure

    [CreateStructure()]
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), PRECONDITION_DESCRIPTION);
        this.AddStep(new Step1(), STEP1_DESCRIPTION);
        this.AddStep(new Step2(), STEP2_DESCRIPTION);

        //Get Client Platform
        CL = GetClient();
    }

    #endregion Create Structure

    #region PreExecute

    [PreExecute()]
    public override void PreExecute()
    {
        base.PreExecute(); //Implement Mount Function
    }

    #endregion PreExecute

    #region Steps

    #region PreCondition

    [Step(0, PRECONDITION_DESCRIPTION)]
    private class PreCondition : _Step
    {
        public override void Execute()
        {
            StartStep();

            string bannerTimeout = "";
             try
            {
                isSetBannerDisplayTimeSupporetd = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "SUPPORTED");
            }
            catch (Exception ex)
            {
                FailStep(CL, "Failed to get value from Project.ini file. Exception : " + ex.Message.ToString());
            }
            videoService = CL.EA.GetServiceFromContentXML("Type=Video", "ParentalRating=High;IsDefault=True");
            if (videoService == null)
            {
                FailStep(CL, "Video Service fetched from content.xml is null");
            }
            LogCommentInfo(CL, "Retrieved Value From Content XML File: videoService = " + videoService.LCN);

            if (Convert.ToBoolean(isSetBannerDisplayTimeSupporetd))
            
            {
               bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "MAX");
               if (string.IsNullOrEmpty(bannerTimeout))
               {
                   FailStep(CL, "CHANNEL_BAR_TIMEOUT, MAX fetched from Project.ini is null or empty", false);
               }
               Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out maxChannelBarTimeOutVal);
               LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> MAX = " + maxChannelBarTimeOutVal);

               bannerTimeout = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "DEFAULT");
               if (string.IsNullOrEmpty(bannerTimeout))
               {
                   FailStep(CL, "CHANNEL_BAR_TIMEOUT, DEFAULT fetched from Project.ini is null or empty", false);
               }
               Enum.TryParse<EnumChannelBarTimeout>(bannerTimeout, out defaultChannelBarTimeOutVal);
               LogCommentInfo(CL, "Retrieved Value From Project.ini File: CHANNEL_BAR -> DEFAULT = " + defaultChannelBarTimeOutVal);

            }

             else
                 
                LogCommentWarning(CL, "Skipping Set Banner Display Timeout related actions as it is not supported in Project");

            //Channel Surf to videoService
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, videoService.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to Tune to Channel" + videoService.LCN);
            }

             if (Convert.ToBoolean(isSetBannerDisplayTimeSupporetd))
            {
               // Change Timeout Duration in Channel Bar Timeout settings
               res = CL.EA.STBSettings.SetBannerDisplayTime(maxChannelBarTimeOutVal);
               if (!res.CommandSucceeded)
               {
                   FailStep(CL, res, "Failed to change Banner Display Time to:" + maxChannelBarTimeOutVal, false);
               }
               
             }
               else

                 LogCommentWarning(CL, "Skipping Set Banner Display Timeout related actions as it is not supported in Project");

            PassStep();
        }
    }

    #endregion PreCondition

    #region Step1

    [Step(1, STEP1_DESCRIPTION)]
    private class Step1 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Focus on another channel from channel bar
            //Verify PIP
            //Zap to focused channel
            res = CL.EA.ChannelSurf(EnumSurfIn.ChannelBar, "", true, 1, EnumPredicted.Ignore, true);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to zap to another channel from Channel Bar");
            }
            PassStep();
        }
    }

    #endregion Step1

    #region Step2

    [Step(2, STEP2_DESCRIPTION)]
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();

            //Verify channel number varies from previous channel
            string chNum = "";
            res = CL.IEX.MilestonesEPG.GetEPGInfo("chNum", out chNum);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to get channel number");
            }
            if (chNum == videoService.LCN)
            {
                FailStep(CL, res, "Failed to zap to another service from channel lineup ");
            }
			
			LogCommentInfo(CL, "Currently zapped to channel: " + chNum);

            PassStep();
        }
    }

    #endregion Step2

    #endregion Steps

    #region PostExecute

    [PostExecute()]
    public override void PostExecute()
    {
        IEXGateway._IEXResult res;

        try
        {
            isSetBannerDisplayTimeSupporetd = CL.EA.GetValueFromINI(EnumINIFile.Project, "CHANNEL_BAR_TIMEOUT", "SUPPORTED");
        }
        catch (Exception ex)
        {
            LogCommentFailure(CL, "Failed to get value from Project.ini file. Exception : " + ex.Message.ToString());
        }

        if (Convert.ToBoolean(isSetBannerDisplayTimeSupporetd))
        {
        //Set Channel Bar Time Out to Default
            res = CL.EA.STBSettings.SetBannerDisplayTime(defaultChannelBarTimeOutVal);
            if (!res.CommandSucceeded)
            {
               LogCommentFailure(CL, "Failed to change Banner Display Time to:" + defaultChannelBarTimeOutVal);
            }
         }
            
        else
                 
             LogCommentWarning(CL, "Skipping Set Banner Display Timeout related actions as it is not supported in Project");
    }

    #endregion PostExecute
}