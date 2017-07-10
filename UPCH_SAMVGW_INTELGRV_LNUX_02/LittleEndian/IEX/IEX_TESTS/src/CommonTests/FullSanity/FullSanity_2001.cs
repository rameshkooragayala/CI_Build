/// <summary>
///  Script Name : FullSanity_2001.cs
///  Test Name   : FullSanity-2001-VOD-Browse VOD catalogue
///  TEST ID     : 17347
///  QC Version  : 4
///  QC Domain   : FR_FUSION
///  QC Project  : UPC
///  QC Path     : GO/NO GO/Full Sanity/20 Sanity-VOD
/// -----------------------------------------------
///  Modified by : Frederic Luu
/// </summary>


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//FullSanity-2001-VOD-Browse VOD catalogue
public class FullSanity_2001 : _Test
{
    [ThreadStatic]
    static _Platform CL;

    static string defaultThumbnail;

    #region Create Structure
    public override void CreateStructure()
    {
        this.AddStep(new PreCondition(), "Precondition: Get values from ini files");
        this.AddStep(new Step1(), "Step 1: Focus 'ON DEMAND'");
        this.AddStep(new Step2(), "Step 2: Go to a category with showcase");
        this.AddStep(new Step3(), "Step 3: Go to a category of type 1 (with sub categories)");
        this.AddStep(new Step4(), "Step 4: Go to a leaf category and browse assets");

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

            defaultThumbnail = CL.EA.GetValue("DefaultThumbnail");

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
            
            // Focus ON DEMAND in main menu            
            res = CL.EA.NavigateAndHighlight("STATE:STORE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to focus 'ON DEMAND'");
            }

            // ### TODO #### Check thumbnail link of mini showcase ('m_imgurl' milestone) (CQ 1997467)
            // Check mini showcase
            /*string thumbnail = "";
            CL.EA.UI.Utils.GetEpgInfo("m_imgurl", ref thumbnail);  
            if (string.IsNullOrEmpty(thumbnail) || thumbnail.Equals(defaultThumbnail) || thumbnail.Length.Equals(0))
            {
                FailStep(CL, res, "Thumbnail received is either null or default thumbnail " + thumbnail);
            }*/

            PassStep();
        }
    }
    #endregion

    #region Step2
    private class Step2 : _Step
    {
        public override void Execute()
        {
            StartStep();    
        
            // Go to a category with showcase
            res = CL.EA.NavigateAndHighlight("STATE:STORE_ASSET_IN_SHOWCASE");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to a category with showcase");
            }

            // Check thumbnails of the showcase (showcase menu is circular)
            string assetTitle = "";           
            string initialAssetTitle = "";
            string thumbnail = "";
            CL.EA.UI.Utils.GetEpgInfo("title", ref initialAssetTitle);
            do
            {
                // Focus next asset in showcase
                CL.EA.UI.Vod.NextAsset();
                CL.EA.UI.Utils.GetEpgInfo("title", ref assetTitle);   
                
                // Check asset thumbnail             
                CL.EA.UI.Utils.GetEpgInfo("m_imgurl", ref thumbnail);                
                if (string.IsNullOrEmpty(thumbnail) || thumbnail.Equals(defaultThumbnail) || thumbnail.Length.Equals(0))
                {
                    FailStep(CL, res, "Asset thumbnail received is either null or default thumbnail " + thumbnail);

                }
            } while (assetTitle != initialAssetTitle);  
                        
            PassStep();
        }
    }
    #endregion

    #region Step3
    private class Step3 : _Step
    {
        public override void Execute()
        {
            StartStep();
            
            // Go to a category of type 1 (with sub categories)
            res = CL.EA.NavigateAndHighlight("STATE:STORE_SUBCATEGORY_IN_CATEGORY_TYPE_1");
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to a category with sub categories");
            }

            // Check sub categories are displayed with mini showcases 
            // ### TODO #### Check thumbnail link of mini showcase ('m_imgurl' milestone) (CQ 1997467)
            string categoryTitle = "";              
            string initialCategoryTitle = "";
            string previousCategoryTitle = "";
            string thumbnail = "";
            CL.EA.UI.Utils.GetEpgInfo("title", ref initialCategoryTitle);
            do
            {
                // Check category mini-showcase              
                /*CL.EA.UI.Utils.GetEPGInfo("m_imgurl", ref thumbnail); 
                if (string.IsNullOrEmpty(thumbnail) || thumbnail.Equals(defaultThumbnail) || thumbnail.Length.Equals(0))
                {
                    FailStep(CL, res, "Category thumbnail received is either null or default thumbnail " + thumbnail);

                }*/

                // Focus next sub-category
                CL.EA.UI.Vod.NextCategory();
                previousCategoryTitle = categoryTitle;
                CL.EA.UI.Utils.GetEpgInfo("title", ref categoryTitle);

            } while ((categoryTitle != initialCategoryTitle) && (categoryTitle != previousCategoryTitle));

            PassStep();
        }
    }
    #endregion

    #region Step4
    private class Step4 : _Step
    {
        public override void Execute()
        {
            StartStep();

            // Go to a leaf category
            res = CL.EA.NavigateAndHighlight("STATE:STORE_ASSET_IN_LEAF_CATEGORY"); 
            if (!res.CommandSucceeded)
            {
                FailStep(CL, res, "Failed to go to a leaf category");
            }

            // Check thumbnails of assets in the leaf category
            string assetTitle = "";
            string initialAssetTitle = "";
            string previousAssetTitle = "";
            string thumbnail = "";
            CL.EA.UI.Utils.GetEpgInfo("title", ref initialAssetTitle);
            do
            {
                // Check asset thumbnail      
                // WARNING: sometimes m_imgurl is no not available in EPG dictionary!!!!!
                CL.EA.UI.Utils.GetEpgInfo("m_imgurl", ref thumbnail);
                if (string.IsNullOrEmpty(thumbnail) || thumbnail.Equals(defaultThumbnail) || thumbnail.Length.Equals(0))
                {
                    FailStep(CL, res, "Asset thumbnail received is either null or default thumbnail " + thumbnail);

                }

                // Focus next asset in the leaf category
                CL.EA.UI.Vod.NextAsset();
                previousAssetTitle = assetTitle;
                CL.EA.UI.Utils.GetEpgInfo("title", ref assetTitle);

            } while ((assetTitle != initialAssetTitle) && (assetTitle != previousAssetTitle));

            PassStep();
        }
    }
    #endregion
    #endregion
}
