using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools
{
    public class Campaign
    {
        public string Name;
        public string PalletBarcode;
        public string[] mainLevels;

        public CampaignSaveData saveData;

        public static List<Campaign> LoadedCampaigns = new List<Campaign>();

        public static Campaign RegisterCampaign(string Name, string[] mainLevels)
        {
            Campaign campaign = new Campaign();
            campaign.Name = Name;
            campaign.mainLevels = mainLevels;
            campaign.saveData = new CampaignSaveData(campaign);
            LoadedCampaigns.Add(campaign);
            return campaign;
        }

        public int GetLevelIndex(string levelBarcode)
        {
            return Array.IndexOf(mainLevels, levelBarcode);
        }

        public string GetLevelBarcodeByIndex(int index)
        {
            return mainLevels[index];
        }

        public static Campaign GetFromName(string name)
        {
            return LoadedCampaigns.First(x => x.Name == name);
        }

        public static Campaign GetFromLevel(string barcode)
        {
            return LoadedCampaigns.First(x => x.mainLevels.Contains(barcode));
        }

        public static Campaign GetFromLevel(Barcode barcode) => GetFromLevel(barcode.ID);

        public static Campaign GetFromLevel(LevelCrateReference level) => GetFromLevel(level.Barcode.ID);

        public static Campaign GetFromLevel() => GetFromLevel(SceneStreamer.Session.Level.Barcode.ID);
    }
}