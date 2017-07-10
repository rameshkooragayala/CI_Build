/// <summary>
///  Script Name        : FullSanity_1801.cs
///  Test Name          : Fullsanity-1801-AMS-Report Live
///  TEST ID            : 17120
///  QC Version         : 2
///  QC Repository      : FR_FUSION/UPC
///  Variations from QC : None
/// -----------------------------------------------
///  Modified by        : Madhu Kumar k
///  Modified Date      : 30th OCT, 2014
/// </summary>

using System;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Engine;
using FailuresHandler;
using System.IO;
using System.Linq;

public class FullSanity_1801 : _Test
{
    [ThreadStatic]
    private static _Platform CL;

    //Channels used by the test

    private static Service Service_1;
    private static Service Service_2;
    private static Service Service_3;
    private static Service Service_4;
    private static Service Service_5;
    private static Service Service_6;
    #region Create Structure

    public override void CreateStructure()
    {

        this.AddStep(new PreCondition(), "Precondition:Fetch services from XML file and Sync");
        this.AddStep(new Step1(), "Step 1: Do Channel surf and wait for minimum veiwing duration for different services and verify the AMS tags");
        this.AddStep(new Step2(), "Step 2: Set the Master Personalization flag to OFF ");
        this.AddStep(new Step3(), "Step 3: Perform some actions and verify that there are no AMS files getting generated");
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
            string defaultService = CL.EA.GetValueFromINI(EnumINIFile.Test, "TEST PARAMS", "DEFAULT_SERVICE");
            if (defaultService == "")
            {
                FailStep(CL,"Failed to get the from Test ini");
            }
            LogCommentImportant(CL,"Fetched Default service from test ini is "+defaultService);
            Service_1 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True;LCN=" + defaultService, "ParentalRating=High");
            if (Service_1 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_1.LCN);
            }

            Service_2 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN);
            if (Service_2 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_2.LCN);
            }
            Service_3 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN);
            if (Service_3 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_3.LCN);
            }
            Service_4 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN);
            if (Service_4 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_4.LCN);
            }
            Service_5 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN + "," + Service_4.LCN);
            if (Service_5 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_5.LCN);
            }
            Service_6 = CL.EA.GetServiceFromContentXML("Type=Video;IsEITAvailable=True", "ParentalRating=High;LCN=" + Service_1.LCN + "," + Service_2.LCN + "," + Service_3.LCN + "," + Service_4.LCN + "," + Service_5.LCN);
            if (Service_6 == null)
            {
                FailStep(CL, "Failed to get channel number from ContentXML");
            }
            else
            {
                LogCommentImportant(CL, "Favourite service fetched from content xml " + Service_6.LCN);
            }
			
			if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("YES"))
            {
                FailStep(CL, res, "Failed to set the Personalization to YES");
            }
			
            res = CL.EA.ChannelSurf(EnumSurfIn.Live,Service_1.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL,res,"Failed to tune to service "+Service_1.LCN);
            }
            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_2.LCN);
            }
            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_3.LCN);
            }
            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_4.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_4.LCN);
            }
            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_5.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_5.LCN);
            }
            CL.IEX.Wait(60);
            res = CL.EA.ChannelSurf(EnumSurfIn.Live, Service_6.LCN);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to tune to service " + Service_6.LCN);
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
            CL.IEX.Wait(420);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.LiveViewEvent, service: Service_1);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.LiveViewEvent, service: Service_2);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.LiveViewEvent, service: Service_3);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.LiveViewEvent, service: Service_4);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);
            res = CL.EA.VerifyAMSTags(EnumAMSEvent.LiveViewEvent, service: Service_5);
            if (!res.CommandSucceeded)
            {
                FailStep(CL, "Failed to find the Live View event");
            }
            CL.IEX.Wait(10);



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
            if (!CL.EA.UI.Utils.SetPersonalizedRecommendationActivation("NO"))
            {
                FailStep(CL, res, "Failed to set the Personalization to NO");
            }
            CL.EA.ChannelSurf(EnumSurfIn.Live, Service_1.LCN);
            CL.IEX.Wait(30);
            CL.EA.ChannelSurf(EnumSurfIn.Live, Service_2.LCN);
            CL.IEX.Wait(5);
            CL.EA.ChannelSurf(EnumSurfIn.Live, Service_3.LCN);
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
           CL.IEX.Wait(600);
            string boxID = CL.EA.UI.Utils.GetValueFromEnvironment("BOX_ID");
             string AMSCommand = "";
            string AMSlogFolderPath = "";
            string RFFeed = CL.EA.UI.Utils.GetValueFromEnvironment("RFPort");
            if (RFFeed.ToUpper() == "NL")
            {
                //Command which is used to connect to the the AMS Server
                AMSCommand = CL.EA.UI.Utils.GetValueFromEnvironment("AMSServerConnectionCmd_NL");
                //AMS SERVER log folder path--from where we will be fetching the AMS files
                AMSlogFolderPath = CL.EA.UI.Utils.GetValueFromProject("AMS", "LOG_FOLDER_PATH_NL");
            }
            else
            {
                //Command which is used to connect to the the AMS Server
                AMSCommand = CL.EA.UI.Utils.GetValueFromEnvironment("AMSServerConnectionCmd_UM");
                //AMS SERVER log folder path--from where we will be fetching the AMS files
                AMSlogFolderPath = CL.EA.UI.Utils.GetValueFromProject("AMS", "LOG_FOLDER_PATH_UM");
            }
            string _FinalOutput = CL.EA.UI.OTA.StartProcess(AMSCommand, "ERROR");
            DirectoryInfo directory=new DirectoryInfo(AMSlogFolderPath); 
                        var latestFile = directory.GetFiles("*" + boxID + "*Format1*").Where(f => f.CreationTime >= DateTime.Now.AddMinutes(-10));
            if(latestFile.Count()>0)
            {
                FailStep(CL,"Recieved AMS files"+ latestFile.Count()+" after setting the Personalized Recommendation Activation to NO");
            }
            LogCommentImportant(CL,"Did not recieve any AMS files after setting the Personalized Recommendation Activation to NO");
            PassStep();
        }
    }

    #endregion Step3



    #endregion Steps

    #region PostExecute

    public override void PostExecute()
    {


    }

    #endregion PostExecute
}
