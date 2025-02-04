using System.Collections;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    public static class CampaignUtilities
    {
        public static List<Campaign> LoadedCampaigns = new List<Campaign>();

        public static Campaign GetFromName(string name)
        {
            return LoadedCampaigns.FirstOrDefault(x => x.Name == name);
        }

        public static Campaign GetFromLevel(string barcode)
        {
            return LoadedCampaigns.FirstOrDefault(x => x.AllLevels.Contains(barcode));
        }

        public static Campaign GetFromLevel(Barcode barcode) => GetFromLevel(barcode.ID);

        public static Campaign GetFromLevel(LevelCrateReference level) => GetFromLevel(level.Barcode.ID);

        public static Campaign GetFromLevel() => GetFromLevel(SceneStreamer.Session.Level.Barcode.ID);

        public static bool IsCampaignLevel(string levelBarcode, out Campaign campaign, out CampaignLevelType levelType)
        {
            campaign = Campaign.GetFromLevel(levelBarcode);

            if (campaign != null)
                levelType = campaign.TypeOfLevel(levelBarcode);

            else levelType = CampaignLevelType.None;

            return campaign != null;
        }
    }
}