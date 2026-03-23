using SimpleSerializables.Utils;
using CustomCampaignTools.Debug;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader.Utils;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

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

            string savePath = (overwritePath != string.Empty) ? overwritePath : GetSavePath(campaign);
            
            CampaignLogger.Msg(campaign, "Saving SaveData to Disk at path: " + savePath);
            SerializerUtils.SaveObjectToFile(this, savePath);
        }

        /// <summary>
        /// Loads the save data from file. This should *probably* only be called at initialization.
        /// </summary>
        internal static CampaignSaveData LoadFromDisk(Campaign c)
        {
            CampaignLogger.Msg(c, "Loading SaveData from Disk");
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            string savePathToUse = GetSavePath(c);

            if (!File.Exists(savePathToUse))
            {
                CampaignLogger.Msg(c, "Could not find save at new save path. Checking Legacy save path");
                if (File.Exists(GetLegacySavePath(c)))
                {
                    CampaignLogger.Msg(c, "Found save at legacy save path. Loading from there");
                    savePathToUse = GetLegacySavePath(c);
                }
                else
                {
                    // Create a blank save, none exists in either save path
                    CampaignLogger.Msg(c, "Could not find save at either save path. Creating blank save.");
                    CampaignSaveData newSave = new(c);
                    
                    newSave.SaveToDisk();
                    return newSave;
                }
            }

            CampaignLogger.Msg(c, "Deserializing SaveData from path " + savePathToUse);
            CampaignSaveData saveData = SerializerUtils.LoadObjectFromFile<CampaignSaveData>(savePathToUse);
            saveData.campaign = c;
            return saveData;
        }

        #endregion
    }
}