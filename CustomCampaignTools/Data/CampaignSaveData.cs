using BoneLib.Notifications;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools
{
    public class CampaignSaveData
    {
        public Campaign campaign;

        internal SavePoint LoadedSavePoint;
        internal List<AmmoSave> LoadedAmmoSaves = new List<AmmoSave>();
        internal List<FloatData> LoadedFloatDatas = new List<FloatData>();
        internal bool DevToolsUnlocked = false;
        internal bool AvatarUnlocked = false;
        internal List<string> UnlockedAchievements = new List<string>();

        public string SaveFolder { get => $"{MelonUtils.UserDataDirectory}/Campaigns/{campaign.Name}"; }
        public string SavePath { get => $"{SaveFolder}/save.json"; }

        public CampaignSaveData(Campaign c)
        {
            campaign = c;
            LoadFromDisk();
        }

        public void ResetSave()
        {
            ClearAmmoSave();
            LoadedSavePoint = new SavePoint();
            LoadedFloatDatas = new List<FloatData>();
            DevToolsUnlocked = false;
            AvatarUnlocked = false;
            UnlockedAchievements = new List<strin>();
        }

        #region Ammo Methods
        public void SaveAmmoForLevel(string levelBarcode)
        {
            AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode);

            if(!campaign.SaveLevelAmmo) return;

            if (!DoesSavedAmmoExist(levelBarcode))
            {
                LoadedAmmoSaves.Add(new AmmoSave
                {
                    LevelBarcode = levelBarcode,
                    LightAmmo = AmmoInventory.Instance.GetCartridgeCount("light") - previousAmmoSave.LightAmmo,
                    MediumAmmo = AmmoInventory.Instance.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo,
                    HeavyAmmo = AmmoInventory.Instance.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo
                });
            } 
            else
            {
                AmmoSave previousHighScore = GetSavedAmmo(levelBarcode);

                for (int i = 0; i < LoadedAmmoSaves.Count; i++)
                {
                    if (LoadedAmmoSaves[i].LevelBarcode == levelBarcode)
                    {
                        LoadedAmmoSaves[i] = new AmmoSave
                        {
                            LevelBarcode = levelBarcode,
                            LightAmmo = Mathf.Max(AmmoInventory.Instance.GetCartridgeCount("light") - previousAmmoSave.LightAmmo, previousHighScore.LightAmmo),
                            MediumAmmo = Mathf.Max(AmmoInventory.Instance.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo, previousHighScore.MediumAmmo),
                            HeavyAmmo = Mathf.Max(AmmoInventory.Instance.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo, previousHighScore.HeavyAmmo)
                        };
                    }
                }
            }

            campaign.saveData.SaveToDisk();
        }

        public AmmoSave GetPreviousLevelsAmmoSave(string levelBarcode)
        {
            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            AmmoSave previousLevelsAmmoSave = new AmmoSave();

            for (int i = 0; i < levelIndex; i++)
            {
                previousLevelsAmmoSave.LightAmmo += GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).LightAmmo;
                previousLevelsAmmoSave.MediumAmmo += GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).MediumAmmo;
                previousLevelsAmmoSave.HeavyAmmo += GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).HeavyAmmo;
            }

            return previousLevelsAmmoSave;
        }

        public AmmoSave GetSavedAmmo(string levelBarcode)
        {
            return LoadedAmmoSaves.FirstOrDefault(x => x.LevelBarcode == levelBarcode);
        }

        public bool DoesSavedAmmoExist(string levelBarcode)
        {
            if (LoadedAmmoSaves == null)
                return false;

            if (LoadedAmmoSaves.Count == 0)
                return false;

            if (LoadedAmmoSaves.Any(x => x.LevelBarcode == levelBarcode))
                return true;

            return false;
        }

        public void ClearAmmoSave()
        {
            LoadedAmmoSaves.Clear();
            // Fill default ammo saves
            foreach (string barcode in campaign.mainLevels)
            {
                LoadedAmmoSaves.Add(new AmmoSave()
                {
                    LevelBarcode = barcode,
                    LightAmmo = 0,
                    MediumAmmo = 0,
                    HeavyAmmo = 0,
                });
            }
        }
        #endregion

        #region Save Point Methods
        public void ClearSavePoint()
        {
            LoadedSavePoint = new SavePoint();
            SaveToDisk();
        }
        #endregion

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

            foreach(AchievementData achievement in campaign.Achievements)
            {
                if(achievement.Key != key) continue;

                if(achievement.IconGUID == string.Empty)
                {
                    Notifier.Send(new Notification() { Title = $"Achievement Get: {achievement.Name}", Message = achievement.Description, Type = NotificationType.Information, PopupLength = 5, ShowTitleOnPopup = true });
                }
                else
                {
                    achievement.LoadIcon((tex) => {
                        Notifier.Send(new Notification() { 
                            CustomIcon = tex,
                            Title = $"Achievement Get: {achievement.Name}",
                            Message = achievement.Description,
                            Type = NotificationType.CustomIcon,
                            PopupLength = 5,
                            ShowTitleOnPopup = true,
                        });
                    });
                }
                UnlockedAchievements.Add(key);
                SaveToDisk();
                return true;
            }
            return false;
        }
        #endregion

        #region Saving and Loading
        /// <summary>
        /// Saves the current loaded save data to file.
        /// </summary>
        internal void SaveToDisk()
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            SaveData saveData = new SaveData
            {
                SavePoint = LoadedSavePoint,
                AmmoSaves = LoadedAmmoSaves,
                FloatData = LoadedFloatDatas,
                DevToolsUnlocked = DevToolsUnlocked,
                AvatarUnlocked = AvatarUnlocked,
                UnlockedAchievements = UnlockedAchievements,
            };

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            string json = JsonConvert.SerializeObject(saveData, settings);

            File.WriteAllText(SavePath, json);
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

            LoadedSavePoint = saveData.SavePoint;
            LoadedAmmoSaves = saveData.AmmoSaves;
            LoadedFloatDatas = saveData.FloatData;
            DevToolsUnlocked = saveData.DevToolsUnlocked;
            AvatarUnlocked = saveData.AvatarUnlocked;
            UnlockedAchievements = saveData.UnlockedAchievements;
        }
        #endregion

        public class SaveData
        {
            public SavePoint SavePoint { get; set; }
            public List<AmmoSave> AmmoSaves { get; set; }
            public List<FloatData> FloatData { get; set; }
            public bool DevToolsUnlocked { get; set; }
            public bool AvatarUnlocked { get; set; }
            public List<string> UnlockedAchievements { get; set; }
        }

        public struct SavePoint(string levelBarcode, Vector3 position, string backSlotBarcode, string leftSidearmBarcode, string rightSidearmBarcode, string leftShoulderBarcode, string rightShoulderBarcode, List<string> boxContainedBarcodes, Vector3 boxContainerPosition)
        {
            public string LevelBarcode = levelBarcode;
            public float PositionX = position.x;
            public float PositionY = position.y;
            public float PositionZ = position.z;

            public string BackSlotBarcode = backSlotBarcode;
            public string LeftSidearmBarcode = leftSidearmBarcode;
            public string RightSidearmBarcode = rightSidearmBarcode;
            public string LeftShoulderSlotBarcode = leftShoulderBarcode;
            public string RightShoulderSlotBarcode = rightShoulderBarcode;

            public List<string> BoxContainedBarcodes = boxContainedBarcodes;

            public float BoxContainedX = boxContainerPosition.x;
            public float BoxContainedY = boxContainerPosition.y;
            public float BoxContainedZ = boxContainerPosition.z;

            /// <summary>
            /// Returns true if the save point has a level barcode.
            /// </summary>
            /// <param name="out hasSpawnPoint"></param>
            /// <returns></returns>
            public bool IsValid(out bool hasSpawnPoint)
            {
                if (GetPosition() == Vector3.zero)
                    hasSpawnPoint = false;
                else
                    hasSpawnPoint = true;

                if (LevelBarcode == string.Empty)
                    return false;

                return true;
            }

            public void LoadContinue()
            {
                LoadContinue(new Barcode(campaign.LoadScene));
            }

            public void LoadContinue(Barcode loadScene)
            {
                if(!IsValid(out _)) return;
                
                SavepointFunctions.WasLastLoadByContinue = true;
                FadeLoader.Load(new Barcode(LevelBarcode), loadScene);
            }

            public Vector3 GetPosition()
            {
                return new Vector3(PositionX, PositionY, PositionZ);
            }

            public Vector3 GetBoxPosition()
            {
                return new Vector3(BoxContainedX, BoxContainedY, BoxContainedZ);
            }
        }

        public struct AmmoSave
        {
            public string LevelBarcode { get; set; }
            public int LightAmmo { get; set; }
            public int MediumAmmo { get; set; }
            public int HeavyAmmo { get; set; }

            public int GetCombinedTotal()
            {
                return (LightAmmo + MediumAmmo + HeavyAmmo);
            }
        }

        public class FloatData
        {
            public string Key;
            public float Value;

            public FloatData(string key)
            {
                Key = key;
                Value = 0f;
            }
        }
    }
}