using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader.TinyJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools
{
    public static class CampaignUtilities
    {
        public static List<Campaign> LoadedCampaigns = [];

        private static Dictionary<string, Campaign> levelToCampaignRegistry = [];

        internal static void AddCampaign(Campaign campaign)
        {
            CampaignLogger.Msg("Adding Campaign: " + campaign.Name);
            bool loaded = false;
            foreach (var c in LoadedCampaigns)
            {
                if (c.PalletBarcode == campaign.PalletBarcode)
                {
                    CampaignLogger.Msg("Campaign Already Found, Replacing");
                    LoadedCampaigns.Remove(c);
                    c.saveData.SaveToDisk();
                    LoadedCampaigns.Add(campaign);
                    loaded = true;
                    break;
                }
            }
            if (!loaded) 
            { 
                LoadedCampaigns.Add(campaign);
            }
            foreach (CampaignLevel c in campaign.AllLevels)
            {
                levelToCampaignRegistry[c.Barcode.ID] = campaign;
            }
            _menuCampaigns = null;
            GameManager.currentGameConfiguration.RefreshCampaignMenu(campaign);
        }

        public static List<Campaign> CampaignsToShowInMenu
        {
            get
            {
                _menuCampaigns ??= [.. LoadedCampaigns.Where(c => c.ShowInMenu)];
                return _menuCampaigns;
            }
        }
        private static List<Campaign> _menuCampaigns;


        public static Campaign GetFromPallet(Barcode pallet)
        {
            if (pallet == null) return null;
            return LoadedCampaigns.FirstOrDefault(x => x.PalletBarcode == pallet);
        }

        public static Campaign GetFromPallet(PalletReference pallet) => GetFromPallet(pallet.Barcode);

        public static bool TryGetFromLevel(Barcode barcode, out Campaign campaign)
        {
            return levelToCampaignRegistry.TryGetValue(barcode.ID, out campaign);
        }

        public static bool TryGetFromLevel(Barcode barcode, out Campaign campaign, out CampaignLevel campaignLevel)
        {
            if(levelToCampaignRegistry.TryGetValue(barcode.ID, out campaign))
            {
                campaignLevel = campaign.GetLevel(barcode);
                return true;
            }
            campaign = null;
            campaignLevel = null;
            return false;
        }

        public static Campaign GetFromLevel(Barcode barcode)
        {
            if (!levelToCampaignRegistry.ContainsKey(barcode.ID)) return null;
            return levelToCampaignRegistry[barcode.ID];
        }

        public static Campaign GetFromLevel(LevelCrateReference level) => GetFromLevel(level.Barcode);

        public static Campaign GetFromLevel() => GetFromLevel(SceneStreamer.Session.Level.Barcode);


        public static bool IsCampaignLevel(Barcode levelBarcode, out Campaign campaign, out CampaignLevelType levelType)
        {
            campaign = GetFromLevel(levelBarcode);

            if (campaign != null)
                levelType = campaign.GetLevel(levelBarcode).type;

            else levelType = CampaignLevelType.None;

            return campaign != null;
        }
    }
}
