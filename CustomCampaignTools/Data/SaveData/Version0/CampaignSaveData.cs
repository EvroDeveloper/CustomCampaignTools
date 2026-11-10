using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CustomCampaignTools.SaveData.Version0;

public class CampaignSaveData : ISaveData
{
    [JsonProperty]
    public List<string> UnlockedAchievements = [];
    [JsonProperty]
    public List<AmmoSave> LoadedAmmoSaves = [];
    [JsonProperty]
    public bool DevToolsUnlocked = false;
    [JsonProperty]
    public bool AvatarUnlocked = false;
    [JsonProperty]
    public bool ManualBodylogToggle = true;
    [JsonProperty]
    public bool SkipIntro = false;
    [JsonProperty]
    public Dictionary<string, float> FloatData = [];
    [JsonProperty]
    public Dictionary<string, InventoryData> InventorySaves = [];
    [JsonProperty]
    public List<string> UnlockedLevels = [];
    [JsonProperty]
    public SavePoint LoadedSavePoint = new();
    [JsonProperty]
    public Dictionary<string, int> SavedStats = [];
    [JsonProperty]
    public List<LevelTime> LevelTimes = [];
    [JsonProperty]
    public List<TrialTime> TrialTimes = [];

    public bool IsLatestSaveData => false;

    public ISaveData ConvertToNextVersion()
    {
        return new Version1.CampaignSaveData
        {
            UnlockedAchievements = UnlockedAchievements,
            SavedAmmo = ConvertAmmo(),
            DevToolsUnlocked = DevToolsUnlocked,
            AvatarUnlocked = AvatarUnlocked,
            ManualBodylogToggle = ManualBodylogToggle,
            SkipIntro = SkipIntro,
            FloatData = FloatData,
            InventorySaves = ConvertInventory(),
            UnlockedLevels = UnlockedLevels,
            LoadedSavePoint = LoadedSavePoint.Convert(),
            SavedStats = SavedStats,
            LevelTimes = ConvertLevelTimes(),
            TrialTimes = ConvertTrialTimes(),
        };
    }

    private Dictionary<string, Version1.TrialTime> ConvertTrialTimes()
    {
        if(TrialTimes == null) return [];

        Dictionary<string, Version1.TrialTime> output = [];
        foreach(TrialTime time in TrialTimes)
        {
            output.Add(time.TrialKey, time.Convert());
        }
        return output;
    }

    private Dictionary<string, Version1.AmmoCount> ConvertAmmo()
    {
        if (LoadedAmmoSaves == null) return [];

        Dictionary<string, Version1.AmmoCount> SavedAmmo = [];

        foreach (AmmoSave ammoSave in LoadedAmmoSaves)
        {
            string levelBarcode = ammoSave.LevelBarcode?.ID;
            if (string.IsNullOrEmpty(levelBarcode) || SavedAmmo.ContainsKey(levelBarcode))
                continue;

            SavedAmmo[levelBarcode] = new Version1.AmmoCount(ammoSave.LightAmmo, ammoSave.MediumAmmo, ammoSave.HeavyAmmo);
        }
        return SavedAmmo;
    }

    private Dictionary<string, Version1.InventoryData> ConvertInventory()
    {
        if(InventorySaves == null) return [];

        Dictionary<string, Version1.InventoryData> Inventory = [];

        foreach(var thingy in InventorySaves)
        {
            Inventory.Add(thingy.Key, thingy.Value.Convert());
        }

        return Inventory;
    }

    private Dictionary<string, int> ConvertLevelTimes()
    {
        if(LevelTimes == null) return [];

        Dictionary<string, int> output = [];

        foreach(LevelTime levelTime in LevelTimes)
        {
            output.Add(levelTime.LevelBarcode, levelTime.TimeInSeconds);
        }

        return output;
    }
}