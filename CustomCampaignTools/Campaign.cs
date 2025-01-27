using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using BoneLib;

namespace CustomCampaignTools
{
    public class Campaign
    {
        public string Name;
        public string PalletBarcode;
        public string MenuLevel;
        public string[] mainLevels;
        public string[] extraLevels;
        public string defaultLoadScreen;

        public string[] AllLevels 
        {
            get => mainLevels.Concat(extraLevels).Insert(0, MenuLevel).ToArray();
        }

        public static Campaign Session;
        public static bool SessionActive { get => Session != null; }

        public CampaignSaveData saveData;

        public static List<Campaign> LoadedCampaigns = new List<Campaign>();

        public static Campaign RegisterCampaign(string Name, string initLevel, string[] mainLevels, string[] extraLevels, string loadScene)
        {
            Campaign campaign = new Campaign();
            campaign.Name = Name;
            campaign.MenuLevel = initLevel;
            campaign.mainLevels = mainLevels;
            campaign.extraLevels = extraLevels;
            campaign.defaultLoadScreen = loadScene;
            campaign.saveData = new CampaignSaveData(campaign);
            LoadedCampaigns.Add(campaign);
            return campaign;
        }

        public static Campaign RegisterCampaignFromJson(string json)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            CampaignLoadingData campaignValueHolder = JsonConvert.DeserializeObject<CampaignLoadingData>(json, settings);

            return RegisterCampaign(campaignValueHolder.Name, campaignValueHolder.InitialLevel, campaignValueHolder.MainLevels, campaignValueHolder.ExtraLevels, campaignValueHolder.LoadScene)
        }

        public int GetLevelIndex(string levelBarcode)
        {
            return Array.IndexOf(mainLevels, levelBarcode);
        }

        public string GetLevelBarcodeByIndex(int index)
        {
            return AllLevels[index];
        }

        public static Campaign GetFromName(string name)
        {
            return LoadedCampaigns.First(x => x.Name == name);
        }

        public static Campaign GetFromLevel(string barcode)
        {
            return LoadedCampaigns.First(x => x.AllLevels.Contains(barcode));
        }

        public static Campaign GetFromLevel(Barcode barcode) => GetFromLevel(barcode.ID);

        public static Campaign GetFromLevel(LevelCrateReference level) => GetFromLevel(level.Barcode.ID);

        public static Campaign GetFromLevel() => GetFromLevel(SceneStreamer.Session.Level.Barcode.ID);

        public static void OnInitialize()
        {
            BoneLib.Hooking.OnLevelLoaded += OnLevelLoaded();
            LoadCampaignsFromMods();
        }

        public static void LoadCampaignsFromMods()
        {

        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            Session = GetFromLevel(info.barcode);
        }
    }

    internal class CampaignLoadingData
    {
        public string Name { get; set; }
        public string InitialLevel { get; set; }
        public List<string> MainLevels { get; set; }
        public List<string> ExtraLevels { get; set; }
        public string LoadScene { get; set; }
    }
}