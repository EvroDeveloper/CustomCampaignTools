using CustomCampaignTools.Data;
using CustomCampaignTools.Debug;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        [JsonIgnore]
        public Campaign campaign;

        [JsonIgnore]
        public static string SaveFolder { get => Path.Combine(Application.persistentDataPath, "Saves"); }
        public static string GetSavePath(Campaign c) => $"{SaveFolder}/slot_Campaign_{c.Name}.save.json";
        [JsonIgnore]
        public string BackupSavePath { get => $"{SaveFolder}/slot_Campaign.{campaign.Name}.save_backup.json"; }
        public static string GetLegacySavePath(Campaign c) => $"{MelonEnvironment.UserDataDirectory}/Campaigns/{c.Name}/save.json";

        public CampaignSaveData() {}

        public CampaignSaveData(Campaign c)
        {
            campaign = c;
            ClearAmmoSave();
        }

        public void ResetSave()
        {
            SaveToDisk(BackupSavePath);

            ClearAmmoSave();
            ClearSavePoint();
            InventorySaves = [];
            FloatData = [];
            DevToolsUnlocked = false;
            AvatarUnlocked = false;
            SkipIntro = false;
            UnlockedAchievements = [];
            UnlockedLevels = [];


            foreach (string barcode in campaign.CampaignUnlockCrates)
                DataManager._instance._activeSave.Unlocks.ClearUnlockForBarcode(new Barcode(barcode));

            DataManager.TrySaveActiveSave(SaveFlags.DefaultAndPlayerSettingsAndUnlocks);

            SaveToDisk();
        }

        #region Saving and Loading
        /// <summary>
        /// Saves the current loaded save data to file.
        /// </summary>
        internal void SaveToDisk(string overwritePath = "")
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);
            
            SerializerUtils.SaveObjectToFile(this, (overwritePath != string.Empty) ? overwritePath : GetSavePath(campaign));
        }

        /// <summary>
        /// Loads the save data from file. This should *probably* only be called at initialization.
        /// </summary>
        internal static CampaignSaveData LoadFromDisk(Campaign c)
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            string savePathToUse = GetSavePath(c);

            if (!File.Exists(savePathToUse))
            {
                if (File.Exists(GetLegacySavePath(c)))
                {
                    savePathToUse = GetLegacySavePath(c);
                }
                else
                {
                    // Create a blank save, none exists in either save path
                    CampaignSaveData newSave = new(c);
                    
                    newSave.SaveToDisk();
                    return newSave;
                }
            }

            CampaignSaveData saveData = SerializerUtils.LoadObjectFromFile<CampaignSaveData>(savePathToUse);
            saveData.campaign = c;
            return saveData;
        }

        #endregion
    }
}