using CustomCampaignTools.Bonemenu;
using CustomCampaignTools.Debug;
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

        internal static void AddCampaign(Campaign campaign)
        {
            CampaignLogger.Msg("Adding Campaign: " + campaign.Name);
            bool loaded = false;
            foreach (var c in LoadedCampaigns)
            {
                if (c.Name == campaign.Name)
                {
                    CampaignLogger.Msg("Campaign Already Found, Replacing");
                    LoadedCampaigns.Remove(c);
                    c.saveData.SaveToDisk();
                    LoadedCampaigns.Add(campaign);
                    loaded = true;
                    break;
                }
            }
            if(!loaded) LoadedCampaigns.Add(campaign);
            _menuCampaigns = null;
            CampaignBoneMenu.CreateCampaignPage(campaign);
        }

        public static List<Campaign> CampaignsToShowInMenu
        {
            get
            {
                _menuCampaigns ??= LoadedCampaigns.Where(c => c.ShowInMenu).ToList();
                return _menuCampaigns;
            }
        }
        private static List<Campaign> _menuCampaigns;

        public static Campaign GetFromName(string name)
        {
            return LoadedCampaigns.FirstOrDefault(x => x.Name == name);
        }

        public static Campaign GetFromLevel(string barcode)
        {
            return LoadedCampaigns.FirstOrDefault(x => x.AllLevels.Select(l => l.Barcode.ID).Contains(barcode));
        }

        public static Campaign GetFromLevel(Barcode barcode) => GetFromLevel(barcode.ID);

        public static Campaign GetFromLevel(LevelCrateReference level) => GetFromLevel(level.Barcode.ID);

        public static Campaign GetFromLevel() => GetFromLevel(SceneStreamer.Session.Level.Barcode.ID);

        public static bool IsCampaignLevel(string levelBarcode)
        {
            return GetFromLevel(levelBarcode) != null;
        }

        public static bool IsCampaignLevel(string levelBarcode, out Campaign campaign, out CampaignLevelType levelType)
        {
            campaign = GetFromLevel(levelBarcode);

            if (campaign != null)
                levelType = campaign.TypeOfLevel(levelBarcode);

            else levelType = CampaignLevelType.None;

            return campaign != null;
        }
    }
}
