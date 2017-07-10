using System;
using FailuresHandler;
using IEX.ElementaryActions.Functionality;

namespace EAImplementation
{
    /// <summary>
    /// Verify asset details page
    /// </summary>
    public class VerifyAssetDetails : IEX.ElementaryActions.BaseCommand
    {
        private VODAsset _vodAsset;
        private bool _isPurchased;
        private IEX.ElementaryActions.EPG.SF.UI EPG;
        private IEX.ElementaryActions.Functionality.Manager _manager;

        /// <summary>
        /// Verify asset details page
        /// </summary>
        /// <remarks>
        /// Possible Error Codes:
        /// <para>322 - VerificationFailure</para> 
        /// </remarks>
        public VerifyAssetDetails(VODAsset vodAsset, bool isPurchased, IEX.ElementaryActions.Functionality.Manager pManager)
        {
            this._vodAsset = vodAsset;
            this._manager = pManager;
            this.EPG = this._manager.UI;
            this._isPurchased = isPurchased;
        }

        /// <summary>
        /// Verify asset details page
        /// </summary>
        protected override void Execute()
        {
            // Check if thumbnail is present
            string thumbnailPath = "";
            string defaultThumbnail = _manager.GetValue("DefaultThumbnail");
            EPG.Utils.GetEpgInfo("thumbnail", ref thumbnailPath);
            if (string.IsNullOrEmpty(thumbnailPath) || thumbnailPath.Equals(defaultThumbnail) || thumbnailPath.Length.Equals(0))
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Thumbnail received is either null or default thumbnail: " + thumbnailPath));
            }

            // Check title
            string title = EPG.Vod.GetAssetTitle();
            if (title != _vodAsset.Title)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong title received. Expected: " + _vodAsset.Title + ", Received: " + title));
            }

            // ### TODO #### Missing milestone for asset genre CQ 1974366
            // Check genre
            /*string genre = EPG.Vod.GetAssetGenre();
            if (genre != _vodAsset.Genre)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong asset genre. Expected: " + _vodAsset.Genre + ", Received: " + genre));
            }*/

            // Check asset duration
            int assetDuration = EPG.Vod.GetAssetDuration();
            if (assetDuration.ToString() != _vodAsset.AssetDuration)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong asset duration received. Expected: " + _vodAsset.AssetDuration + ", Received: " + assetDuration));
            }

            // Check synopsis
            string synopsis = EPG.Vod.GetAssetSynopsis();
            if (synopsis != _vodAsset.Synopsis)
            {
                ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong synopsis received. Expected: " + _vodAsset.Synopsis + ", Received: " + synopsis));
            }

            if (Double.Parse(_vodAsset.Price) == 0)
            {
                // Check FREE is displayed             
                string description = "";
                EPG.Utils.GetEpgInfo("description", ref description);
                if (!description.StartsWith("FREE"))
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong information displayed. Expected: 'FREE', Received:" + description));
                }

                // Check available items (PLAY or RESUME/PLAY FROM START)               
                try
                {
                    EPG.Utils.EPG_Milestones_SelectMenuItem("PLAY");
                }
                catch
                {
                    string[] itemNames = { "PLAY FROM BEGINNING", "PLAY FROM LAST VIEWED" };
                    for (int i = 0; i < itemNames.Length; i++)
                    {
                        try
                        {
                            EPG.Utils.EPG_Milestones_SelectMenuItem(itemNames[i]);
                        }
                        catch
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Failed to select '" + itemNames[i] + "' item"));
                        }
                    }
                }
            }
            else if (_isPurchased)
            {
                if (_vodAsset.Type == "TVOD")
                {
                    // Check remaining rental duration
                    int remainingRentalDuration = EPG.Vod.GetAssetRemainingRentalDuration();
                    if ((remainingRentalDuration <= 0) || (remainingRentalDuration > Int32.Parse(_vodAsset.RentalDuration)))
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong asset remaining rental duration. Received: " + remainingRentalDuration));
                    }
                }
                else if (_vodAsset.Type == "SVOD")
                {
                    // Check AVAILABLE is displayed             
                    string description = "";
                    EPG.Utils.GetEpgInfo("description", ref description);
                    if (!description.StartsWith("AVAILABLE"))
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong information displayed. Expected: 'AVAILABLE - Genre', Received:" + description));
                    }
                }

                // Check available items (PLAY or RESUME/PLAY FROM START)
                try
                {
                    EPG.Utils.EPG_Milestones_SelectMenuItem("PLAY");
                }
                catch
                {
                    string[] itemNames = { "PLAY FROM BEGINNING", "PLAY FROM LAST VIEWED" };
                    for (int i = 0; i < itemNames.Length; i++)
                    {
                        try
                        {
                            EPG.Utils.EPG_Milestones_SelectMenuItem(itemNames[i]);
                        }
                        catch
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Failed to select '" + itemNames[i] + "' item"));
                        }
                    }
                }

                // Check BUY item is not available
                bool buyItemFound = false;
                try
                {
                    buyItemFound = EPG.Utils.EPG_Milestones_SelectMenuItem("BUY");
                }
                catch { }
                if (buyItemFound)
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "BUY item is available. Expected: BUY item must be replaced by RESUME and PLAY FROM START items"));
                }
            }
            else
            {
                if (_vodAsset.Type == "TVOD")
                {
                    // Check price
                    double price = EPG.Vod.GetAssetPrice();
                    if (price != Double.Parse(_vodAsset.Price))
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong asset price. Expected: " + _vodAsset.Price + ", Received: " + price));
                    }

                    // Check rental duration
                    int rentalDuration = EPG.Vod.GetAssetRentalDuration();
                    if (rentalDuration.ToString() != _vodAsset.RentalDuration)
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong asset rental duration. Expected: " + _vodAsset.RentalDuration + ", Received: " + rentalDuration));
                    }

                    // Check available items in menu (BUY, INFO, MORE LIKE THIS)         
                    string[] itemNames = { "BUY", "INFO", "MORE LIKE THIS" };
                    for (int i = 0; i < itemNames.Length; i++)
                    {
                        try
                        {
                            EPG.Utils.EPG_Milestones_SelectMenuItem(itemNames[i]);
                        }
                        catch
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Failed to select '" + itemNames[i] + "' item"));
                        }
                    }
                }
                else if (_vodAsset.Type == "SVOD")
                {
                    // Check Monthly subscription is displayed
                    string description = "";
                    EPG.Utils.GetEpgInfo("description", ref description);
                    string expText = EPG.Utils.GetValueFromDictionary("DIC_MONTHLY_SUBSCRIPTION");
                    if (!description.StartsWith(expText))
                    {
                        ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Wrong information displayed. Expected: 'Monthly subscription - Genre', Received:" + description));
                    }

                    // Check available items in menu (SUBSCRIBE, INFO, MORE LIKE THIS)         
                    string[] itemNames = { "SUBSCRIBE", "INFO", "MORE LIKE THIS" };
                    for (int i = 0; i < itemNames.Length; i++)
                    {
                        try
                        {
                            EPG.Utils.EPG_Milestones_SelectMenuItem(itemNames[i]);
                        }
                        catch
                        {
                            ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Failed to select '" + itemNames[i] + "' item"));
                        }
                    }
                }
            }

            // Check trailer option (if any)
            if (Boolean.Parse(_vodAsset.Trailer))
            {
                try
                {
                    EPG.Utils.EPG_Milestones_SelectMenuItem("WATCH TRAILER");
                }
                catch
                {
                    ExceptionUtils.ThrowEx(new EAException(ExitCodes.VerificationFailure, "Failed to select 'WATCH TRAILER' item"));
                }
            }
        }
    }
}



