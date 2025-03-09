using BoneLib;
using BoneLib.Notifications;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

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
        internal List<string> UnlockedLevels = new List<string>();

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
            ClearSavePoint();
            LoadedFloatDatas = new List<FloatData>();
            DevToolsUnlocked = false;
            AvatarUnlocked = false;
            UnlockedAchievements = new List<string>();
            UnlockedLevels = new List<string>();
        }

        #region Ammo Methods
        public void SaveAmmoForLevel(string levelBarcode)
        {
            AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode);

            if (!campaign.SaveLevelAmmo) return;

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
                previousLevelsAmmoSave.LightAmmo += GetSavedAmmo(campaign.mainLevels[i]).LightAmmo;
                previousLevelsAmmoSave.MediumAmmo += GetSavedAmmo(campaign.mainLevels[i]).MediumAmmo;
                previousLevelsAmmoSave.HeavyAmmo += GetSavedAmmo(campaign.mainLevels[i]).HeavyAmmo;
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

        public void SavePlayer(string levelBarcode, Vector3 position, List<BarcodePosRot> boxBarcodes = null)
        {
            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager);

            boxBarcodes ??= new List<BarcodePosRot>();

            AmmoSave ammoSave = new AmmoSave();

            if (campaign.SaveLevelAmmo)
            {
                AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode);

                ammoSave = new AmmoSave
                {
                    LevelBarcode = levelBarcode,
                    LightAmmo = AmmoInventory.Instance.GetCartridgeCount("light") - previousAmmoSave.LightAmmo,
                    MediumAmmo = AmmoInventory.Instance.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo,
                    HeavyAmmo = AmmoInventory.Instance.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo
                };
            }

            LoadedSavePoint = new SavePoint(levelBarcode, position, inventoryData, ammoSave, boxBarcodes);

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

            foreach (AchievementData achievement in campaign.Achievements)
            {
                if (achievement.Key != key) continue;

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
                    Notifier.Send(new Notification() { 
                        Title = $"Achievement Get: {achievement.Name}", 
                        Message = achievement.Description, 
                        Type = NotificationType.Information, 
                        PopupLength = 5, 
                        ShowTitleOnPopup = true 
                    });
                }

                UnlockedAchievements.Add(key);
                SaveToDisk();
                return true;
            }
            return false;
        }

        public void LockAchievement(string key)
        {
            if(UnlockedAchievements.Contains(key))
            {
                UnlockedAchievements.Remove(key);
            }
        }
        #endregion

        #region Levels

        public void UnlockLevel(string barcode)
        {
            if(!UnlockedLevels.Contains(barcode))
            {
                UnlockedLevels.Add(barcode);
            }
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
                UnlockedLevels = UnlockedLevels,
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
            UnlockedAchievements = saveData.UnlockedAchievements ?? new List<string>();
            UnlockedLevels = saveData.UnlockedLevels ?? new List<string>();
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
            public List<string> UnlockedLevels { get; set; }
        }

        public struct SavePoint
        {
            public string LevelBarcode;
            public float PositionX;
            public float PositionY;
            public float PositionZ;

            public InventoryData InventoryData;
            public AmmoSave MidLevelAmmoSave;
            public List<BarcodePosRot> BoxContainedBarcodes;

            public SavePoint(string levelBarcode, Vector3 position, InventoryData inventoryData, AmmoSave ammoSave, List<BarcodePosRot> boxContainedBarcodes)
            {
                LevelBarcode = levelBarcode;
                PositionX = position.x;
                PositionY = position.y;
                PositionZ = position.z;

                InventoryData = inventoryData;
                MidLevelAmmoSave = ammoSave;

                BoxContainedBarcodes = boxContainedBarcodes;
            }

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

                if (LevelBarcode == null || LevelBarcode == string.Empty)
                    return false;

                return true;
            }

            public void LoadContinue(Campaign campaign)
            {
                LoadContinue(new Barcode(campaign.LoadScene));
            }

            public void LoadContinue(Barcode loadScene)
            {
                if (!IsValid(out _)) return;

                SavepointFunctions.WasLastLoadByContinue = true;
                SavepointFunctions.LoadByContine_AmmoPatchHint = true;
                FadeLoader.Load(new Barcode(LevelBarcode), loadScene);
            }

            public Vector3 GetPosition()
            {
                return new Vector3(PositionX, PositionY, PositionZ);
            }
        }

        public struct BarcodePosRot
        {
            public string barcode;

            public float x;
            public float y;
            public float z;

            public float qX;
            public float qY;
            public float qZ;
            public float qW;

            public float sX;
            public float sY;
            public float sZ;

            public BarcodePosRot(Barcode barcode, Vector3 position, Quaternion rotation, Vector3 scale)
            {
                this.barcode = barcode.ID;

                x = position.x;
                y = position.y;
                z = position.z;

                qX = rotation.x;
                qY = rotation.y;
                qZ = rotation.z;
                qW = rotation.w;

                sX = scale.x;
                sY = scale.y;
                sZ = scale.z;
            }

            public Vector3 GetPosition()
            {
                return new Vector3(x, y, z);
            }

            public Quaternion GetRotation()
            {
                return new Quaternion(qX, qY, qZ, qW);
            }

            public Vector3 GetScale()
            {
                return new Vector3(sX, sY, sZ);
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