using BoneLib;
using BoneLib.Notifications;
using CustomCampaignTools.SDK;
using CustomCampaignTools.Bonemenu;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Il2CppSLZ.Bonelab.SaveData;
using Il2CppSLZ.Marrow.Audio;
using System;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        public Campaign campaign;

        internal List<FloatData> LoadedFloatDatas = [];
        internal bool DevToolsUnlocked = false;
        internal bool AvatarUnlocked = false;
        internal bool SkipIntro = false;
        internal List<string> UnlockedAchievements = [];
        internal List<string> UnlockedLevels = [];

        public string SaveFolder { get => $"{MelonEnvironment.UserDataDirectory}/Campaigns/{campaign.Name}"; }
        public string SavePath { get => $"{SaveFolder}/save.json"; }

        public CampaignSaveData(Campaign c)
        {
            campaign = c;
            LoadFromDisk();
        }

        public void ResetSave()
        {
            SaveToDisk($"{SaveFolder}/save_backup.json");

            ClearAmmoSave();
            ClearSavePoint();
            LoadedInventorySaves = [];
            LoadedFloatDatas = [];
            DevToolsUnlocked = false;
            AvatarUnlocked = false;
            SkipIntro = false;
            UnlockedAchievements = [];
            UnlockedLevels = [];


            foreach (string barcode in campaign.CampaignUnlockCrates)
                DataManager.ActiveSave.Unlocks.ClearUnlockForBarcode(new Barcode(barcode));

            DataManager.TrySaveActiveSave(Il2CppSLZ.Marrow.SaveData.SaveFlags.DefaultAndPlayerSettingsAndUnlocks);

            SaveToDisk();
        }

        #region Float Data
        public void SetValue(string key, float value)
        {
            GetFloatDataEntry(key).Value = value;
            SaveToDisk();
        }
        public float GetValue(string key)
        {
            return GetFloatDataEntry(key).Value;
        }
        private FloatData GetFloatDataEntry(string key)
        {
            FloatData found = null;
            try
            {
                found = LoadedFloatDatas.First(f => f.Key == key);
            }
            catch
            {
                found = new FloatData(key);
                LoadedFloatDatas.Add(found);
            }
            return found;
        }
        #endregion

        #region Cheats Unlocking
        public void UnlockDevTools()
        {
            DevToolsUnlocked = true;
            SaveToDisk();
        }

        public void UnlockAvatar()
        {
            AvatarUnlocked = true;
            SaveToDisk();
        }
        #endregion

        #region Achievements
        public bool UnlockAchievement(string key)
        {
            if (UnlockedAchievements == null) UnlockedAchievements = new List<string>();
            if (UnlockedAchievements.Contains(key)) return false;

            foreach (AchievementData achievement in campaign.Achievements)
            {
                if (achievement.Key != key) continue;

                if (campaign.AchievementUnlockSound != null)
                    Audio3dManager.Play2dOneShot(campaign.AchievementUnlockSound, Audio3dManager.ui, new Il2CppSystem.Nullable<float>(1f), new Il2CppSystem.Nullable<float>(1f));

                if (achievement.cachedTexture != null)
                {
                    Notifier.Send(new Notification()
                    {
                        CustomIcon = achievement.cachedTexture,
                        Title = $"Achievement Get: {achievement.Name}",
                        Message = achievement.Description,
                        Type = NotificationType.CustomIcon,
                        PopupLength = 5,
                        ShowTitleOnPopup = true,
                    });
                }
                else
                {
                    Notifier.Send(new Notification()
                    {
                        Title = $"Achievement Get: {achievement.Name}",
                        Message = achievement.Description,
                        Type = NotificationType.Information,
                        PopupLength = 5,
                        ShowTitleOnPopup = true
                    });
                }

                UnlockedAchievements.Add(key);
                SaveToDisk();
                CampaignBoneMenu.RefreshCampaignPage(campaign);
                return true;
            }
            return false;
        }

        public void LockAchievement(string key)
        {
            if (UnlockedAchievements.Contains(key))
            {
                UnlockedAchievements.Remove(key);
            }
        }
        #endregion

        #region Levels

        public void UnlockLevel(string barcode)
        {
            if (!UnlockedLevels.Contains(barcode))
            {
                UnlockedLevels.Add(barcode);
            }
        }

        #endregion

        #region Saving and Loading
        /// <summary>
        /// Saves the current loaded save data to file.
        /// </summary>
        internal void SaveToDisk(string overwritePath = "")
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            SaveData saveData = new(this);

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            string json = JsonConvert.SerializeObject(saveData, settings);

            File.WriteAllText((overwritePath != string.Empty) ? overwritePath : SavePath, json);
        }

        /// <summary>
        /// Loads the save data from file. This should *probably* only be called at initialization.
        /// </summary>
        internal void LoadFromDisk()
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            if (!File.Exists(SavePath))
            {
                ClearAmmoSave();
                LoadedSavePoint = new SavePoint();
                LoadedFloatDatas = new List<FloatData>();
                DevToolsUnlocked = false;
                AvatarUnlocked = false;
                SaveToDisk();
                return;
            }

            string json = File.ReadAllText(SavePath);

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, settings);

            saveData.LoadSaveData(this);
        }
        #endregion

        public class SaveData
        {
            public SavePoint SavePoint { get; set; }
            public List<AmmoSave> AmmoSaves { get; set; }
            public Dictionary<string, InventoryData> InventorySaves { get; set; }
            public List<FloatData> FloatData { get; set; }
            public bool DevToolsUnlocked { get; set; }
            public bool AvatarUnlocked { get; set; }
            public bool SkipIntro { get; set; }
            public List<string> UnlockedAchievements { get; set; }
            public List<string> UnlockedLevels { get; set; }
            public List<LevelTime> LevelTimes = new List<LevelTime>();
            public List<TrialTime> TrialTimes = new List<TrialTime>();

            public SaveData(CampaignSaveData parent)
            {
                SavePoint = parent.LoadedSavePoint;
                AmmoSaves = parent.LoadedAmmoSaves;
                InventorySaves = parent.LoadedInventorySaves;
                FloatData = parent.LoadedFloatDatas;
                DevToolsUnlocked = parent.DevToolsUnlocked;
                AvatarUnlocked = parent.AvatarUnlocked;
                SkipIntro = parent.SkipIntro;
                UnlockedAchievements = parent.UnlockedAchievements;
                UnlockedLevels = parent.UnlockedLevels;
                LevelTimes = parent.LevelTimes;
                TrialTimes = parent.TrialTimes;
            }

            public void LoadSaveData(CampaignSaveData parent)
            {
                parent.LoadedSavePoint = SavePoint;
                parent.LoadedAmmoSaves = AmmoSaves;
                parent.LoadedInventorySaves = InventorySaves ?? [];
                parent.LoadedFloatDatas = FloatData ?? [];
                parent.DevToolsUnlocked = DevToolsUnlocked ?? false;
                parent.AvatarUnlocked = AvatarUnlocked ?? false;
                parent.SkipIntro = SkipIntro ?? false;
                parent.UnlockedAchievements = UnlockedAchievements ?? [];
                parent.UnlockedLevels = UnlockedLevels ?? [];
                parent.LevelTimes = LevelTimes ?? [];
                parent.TrialTimes = TrialTimes ?? [];
            }
        }

        public class FloatData(string key)
        {
            public string Key = key;
            public float Value = 0f;
        }
    }
}