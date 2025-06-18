using BoneLib;
using BoneLib.Notifications;
using CustomCampaignTools.SDK;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Il2CppSLZ.Bonelab.SaveData;
using AmmoInventory = Il2CppSLZ.Marrow.AmmoInventory;
using System;

namespace CustomCampaignTools
{
    public class CampaignSaveData
    {
        public Campaign campaign;

        internal SavePoint LoadedSavePoint;
        internal List<AmmoSave> LoadedAmmoSaves = [];
        internal Dictionary<string, InventoryData> LoadedInventorySaves = [];
        internal List<FloatData> LoadedFloatDatas = [];
        internal bool DevToolsUnlocked = false;
        internal bool AvatarUnlocked = false;
        internal List<string> UnlockedAchievements = [];
        internal List<string> UnlockedLevels = [];

        public string SaveFolder { get => $"{MelonUtils.UserDataDirectory}/Campaigns/{campaign.Name}"; }
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
            UnlockedAchievements = [];
            UnlockedLevels = [];

            foreach(string barcode in campaign.CampaignUnlockCrates)
                DataManager.ActiveSave.Unlocks.ClearUnlockForBarcode(new Barcode(barcode));

            DataManager.TrySaveActiveSave(Il2CppSLZ.Marrow.SaveData.SaveFlags.DefaultAndPlayerSettingsAndUnlocks);

            SaveToDisk();
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

        public AmmoSave GetSavedAmmo(CampaignLevel level)
        {
            return GetSavedAmmo(level.sBarcode);
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
            foreach (CampaignLevel level in campaign.mainLevels)
            {
                LoadedAmmoSaves.Add(new AmmoSave()
                {
                    LevelBarcode = level.sBarcode,
                    LightAmmo = 0,
                    MediumAmmo = 0,
                    HeavyAmmo = 0,
                });
            }
        }
        #endregion

        #region InventorySave Methods

        public void SaveInventoryForLevel(string nextLevelBarcode)
        {
            LogNull(campaign, "Campaign");

            if (!campaign.SaveLevelInventory) return;

            LogNull(Player.RigManager, "Player RigManager");
            LogNull(campaign.InventorySaveLimit, "Inventory Save Limit");

            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager, campaign.InventorySaveLimit);
            LogNull(LoadedInventorySaves, "LoadedInventorySaves");
            LoadedInventorySaves[nextLevelBarcode] = inventoryData;

            void LogNull(object obj, string name)
            {
                if(obj == null)
                {
                    MelonLogger.Error($"HELLO EVRO, {name.ToUpper()} IS NULL");
                }
            }
        }

        public InventoryData GetInventory(string levelBarcode)
        {
            foreach(string barcode in LoadedInventorySaves.Keys)
            {
                if (levelBarcode != barcode) continue;
                return LoadedInventorySaves[barcode];
            }
            return null;
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

            if (campaign.SaveLevelAmmo && position != Vector3.zero)
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

            List<string> savedDespawns = [];
            foreach (SpawnerDespawnSaver stateSaver in UnityEngine.Object.FindObjectsOfType<SpawnerDespawnSaver>())
            {
                if(stateSaver.DontSpawnAgain(out string id))
                {
                    savedDespawns.Add(id);
                }
            }

            Dictionary<string, bool> savedEnableds = [];
            foreach(ObjectEnabledSaver saver in GameObject.FindObjectsOfType<ObjectEnabledSaver>())
            {
                savedEnableds.Add(saver.gameObject.name, saver.gameObject.activeSelf);
            }

            LoadedSavePoint = new SavePoint(levelBarcode, position, inventoryData, ammoSave, boxBarcodes, savedDespawns, savedEnableds);

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
        internal void SaveToDisk(string overwritePath = "")
        {
            if (!Directory.Exists(SaveFolder))
                Directory.CreateDirectory(SaveFolder);

            SaveData saveData = new()
            {
                SavePoint = LoadedSavePoint,
                AmmoSaves = LoadedAmmoSaves,
                InventorySaves = LoadedInventorySaves,
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

            LoadedSavePoint = saveData.SavePoint;
            LoadedAmmoSaves = saveData.AmmoSaves;
            LoadedInventorySaves = saveData.InventorySaves ?? new Dictionary<string, InventoryData>();
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
            public Dictionary<string, InventoryData> InventorySaves { get; set; }
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
            public List<string> DespawnedSpawners;
            public Dictionary<string, bool> ObjectEnabledSaves;

            public SavePoint()
            {

            }

            public SavePoint(string levelBarcode, Vector3 position, InventoryData inventoryData, AmmoSave ammoSave, List<BarcodePosRot> boxContainedBarcodes, List<string> savedDespawns, Dictionary<string, bool> savedEnableds)
            {
                LevelBarcode = levelBarcode;
                PositionX = position.x;
                PositionY = position.y;
                PositionZ = position.z;

                InventoryData = inventoryData;
                MidLevelAmmoSave = ammoSave;

                BoxContainedBarcodes = boxContainedBarcodes;
                DespawnedSpawners = savedDespawns;
                ObjectEnabledSaves = savedEnableds;
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
                    
                FadeLoader.Load(new Barcode(LevelBarcode), loadScene);
            }

            public bool GetEnabledStateFromName(string name, bool defaultEnabled)
            {
                if(ObjectEnabledSaves.Keys.Contains(name))
                    return ObjectEnabledSaves[name];
                else
                    return defaultEnabled;
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

        public class FloatData(string key)
        {
            public string Key = key;
            public float Value = 0f;
        }
    }
}