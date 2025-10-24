using BoneLib;
using BoneLib.Notifications;
using CustomCampaignTools.Bonemenu;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCampaignTools
{
    public enum CampaignLevelType
    {
        None,
        Menu,
        MainLevel,
        ExtraLevel // Extra levels do not save ammo
    }

    public class Campaign
    {
        public string Name;
        public string PalletBarcode;
        public CampaignLevel IntroLevel;
        public CampaignLevel MenuLevel;
        public CampaignLevel[] mainLevels;
        public CampaignLevel[] extraLevels;
        public string LoadScene;
        public AudioClip LoadSceneMusic
        {
            get
            {
                return _loadSceneMusic;
            }
        }
        private MonoDisc _loadMusicDatacard;
        private AudioClip _loadSceneMusic;

        public bool ShowInMenu;
        public bool PrioritizeInLevelPanel = true;

        public bool RestrictDevTools;
        public AvatarRestrictionType AvatarRestrictionType = AvatarRestrictionType.DisableBodyLog | AvatarRestrictionType.RestrictAvatar;
        public string CampaignAvatar
        {
            get
            {
                if(_cachedAvatar == string.Empty || _cachedAvatar == null)
                {
                    if (MarrowGame.assetWarehouse.HasCrate(new Barcode(defaultCampaignAvatar)))
                        _cachedAvatar = defaultCampaignAvatar;
                    else
                        _cachedAvatar = fallbackAvatar;
                }
                return _cachedAvatar;
            }
        }
        private string defaultCampaignAvatar;
        private string fallbackAvatar;
        private string _cachedAvatar;

        public string[] WhitelistedAvatars;

        public bool SaveLevelInventory;
        public List<string> InventorySaveLimit = [];
        public bool SaveLevelAmmo;
        public AudioClip AchievementUnlockSound
        {
            get
            {
                return _achievementUnlockSound;
            }
        }
        private MonoDisc _achievementUnlockSoundDatacard;
        private AudioClip _achievementUnlockSound;
        public List<AchievementData> Achievements = [];
        public bool LockInCampaign;
        public bool LockLevelsUntilEntered;

        public List<string> CampaignUnlockCrates = [];
        public bool DEVMODE { get; private set; } = false;

        public CampaignLevel[] AllLevels
        {
            get
            {
                return [.. mainLevels, .. extraLevels, MenuLevel];
            }
        }

        public CampaignSaveData saveData;

        public static Campaign Session;
        public static string lastLoadedCampaignLevel;
        public static bool SessionActive { get => Session != null; }
        public static bool SessionLocked { get => SessionActive && _sessionLocked; }

        private static bool _sessionLocked;


        internal static Campaign RegisterCampaign(CampaignLoadingData data)
        {
            Campaign campaign = new();
            try
            {
                campaign.Name = data.Name;

                if (data.InitialLevel.levelBarcode == "null.empty.barcode" || data.InitialLevel == null) campaign.MenuLevel = new CampaignLevel(data.MainLevels[0], CampaignLevelType.MainLevel);
                else campaign.MenuLevel = new CampaignLevel(data.InitialLevel, CampaignLevelType.Menu);

                if (data.IntroLevel.levelBarcode == "null.empty.barcode" || data.IntroLevel == null) campaign.IntroLevel = new CampaignLevel(campaign.MenuLevel);
                else campaign.MenuLevel = new CampaignLevel(data.InitialLevel, CampaignLevelType.Menu);

                campaign.mainLevels = [.. data.MainLevels.Select(l => new CampaignLevel(l, CampaignLevelType.MainLevel))];
                campaign.extraLevels = [.. data.ExtraLevels.Select(l => new CampaignLevel(l, CampaignLevelType.ExtraLevel))];

                if (data.LoadScene == "null.empty.barcode") campaign.LoadScene = CommonBarcodes.Maps.LoadMod;
                else campaign.LoadScene = data.LoadScene;
                
                campaign.saveData = new CampaignSaveData(campaign);

                if(data.LoadSceneMusic != null && data.LoadSceneMusic != "null.empty.barcode")
                {
                    MarrowGame.assetWarehouse.TryGetDataCard(new Barcode(data.LoadSceneMusic), out campaign._loadMusicDatacard);
                    campaign._loadMusicDatacard.AudioClip.LoadAsset(new Action<AudioClip>((a) =>
                    {
                        campaign._loadSceneMusic = a;
                        campaign._loadSceneMusic.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    }));
                }

                campaign.ShowInMenu = data.ShowInMenu;
                
                if (data.Version == 0)
                    campaign.PrioritizeInLevelPanel = true;
                else
                    campaign.PrioritizeInLevelPanel = data.PrioritizeInLevelPanel;

                campaign.LockLevelsUntilEntered = data.UnlockableLevels;

                campaign.AvatarRestrictionType = data.AvatarRestrictionType;
                campaign.WhitelistedAvatars = [.. data.WhitelistedAvatars];

                campaign.RestrictDevTools = data.RestrictDevTools;

                campaign.defaultCampaignAvatar = data.CampaignAvatar;
                campaign.fallbackAvatar = data.BaseGameFallbackAvatar;

                campaign.SaveLevelInventory = data.SaveLevelWeapons;
                campaign.InventorySaveLimit = data.InventorySaveLimit;
                campaign.SaveLevelAmmo = data.SaveLevelAmmo;
                campaign.Achievements = data.Achievements;

                campaign.LockInCampaign = data.LockInCampaign;

                if(data.CampaignUnlockCrates != null)
                    campaign.CampaignUnlockCrates = data.CampaignUnlockCrates;

                if(data.AchievementUnlockSound != null && data.AchievementUnlockSound != "null.empty.barcode")
                {
                    MarrowGame.assetWarehouse.TryGetDataCard(new Barcode(data.AchievementUnlockSound), out campaign._achievementUnlockSoundDatacard);
                    campaign._achievementUnlockSoundDatacard.AudioClip.LoadAsset(new Action<AudioClip>((a) =>
                    {
                        campaign._achievementUnlockSound = a;
                        campaign._achievementUnlockSound.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    }));
                }

                if (campaign.Achievements != null)
                {
                    foreach (AchievementData ach in campaign.Achievements)
                    {
                        ach.Init();
                    }
                }

                campaign.DEVMODE = data.DevBuild;

                GashaponHider.AddCratesToHide(data.HideCratesFromGachapon);

                CampaignUtilities.LoadedCampaigns.Add(campaign);

                CampaignBoneMenu.CreateCampaignPage(BoneMenuCreator.campaignCategory, campaign);

            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to register campaign {data.Name}: {ex.Message}");
            }

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

            return RegisterCampaign(campaignValueHolder);
        }

        public void Enter()
        {
            if(LockInCampaign)
            {
                _sessionLocked = true;
            }
            Campaign.Session = this;
            FadeLoader.Load(MenuLevel.Barcode, new Barcode(LoadScene));
        }

        public void Exit()
        {
            _sessionLocked = false;
            FadeLoader.Load(new Barcode(CommonBarcodes.Maps.VoidG114), new Barcode(CommonBarcodes.Maps.LoadDefault));
        }

        public int GetLevelIndex(string levelBarcode, CampaignLevelType levelType = CampaignLevelType.MainLevel)
        {
            return Array.IndexOf(GetBarcodeArrayOfLevelType(levelType), levelBarcode);
        }

        public string GetLevelBarcodeByIndex(int index, CampaignLevelType levelType = CampaignLevelType.MainLevel)
        {
            return GetBarcodeArrayOfLevelType(levelType)[index];
        }

        private string[] GetBarcodeArrayOfLevelType(CampaignLevelType type)
        {
            string[] output;
            switch (type)
            {
                case CampaignLevelType.Menu:
                    output = [MenuLevel.Barcode.ID];
                    break;
                case CampaignLevelType.MainLevel:
                    output = [.. mainLevels.ToBarcodeStrings()];
                    break;
                case CampaignLevelType.ExtraLevel:
                    output = [.. extraLevels.ToBarcodeStrings()];
                    break;
                default:
                    output = [.. AllLevels.ToBarcodeStrings()];
                    break;
            }
            return output;
        }

        public CampaignLevelType TypeOfLevel(string barcode)
        {
            if (MenuLevel.Barcode.ID == barcode) return CampaignLevelType.Menu;
            else if (mainLevels.ToBarcodeStrings().Contains(barcode)) return CampaignLevelType.MainLevel;
            else return CampaignLevelType.ExtraLevel;
        }

        public CampaignLevel[] GetUnlockedLevels()
        {
            List<CampaignLevel> levels = [];

            levels.Add(MenuLevel);
            foreach (CampaignLevel mainLevel in mainLevels)
            {
                if(!saveData.UnlockedLevels.Contains(mainLevel.sBarcode) && LockLevelsUntilEntered) continue;
                levels.Add(mainLevel);
            }
            foreach (CampaignLevel extraLevel in extraLevels)
            {
                if(!saveData.UnlockedLevels.Contains(extraLevel.sBarcode) && LockLevelsUntilEntered) continue;
                levels.Add(extraLevel);
            }
            return [.. levels];
        }

        public static List<string> RegisteredJsonPaths = [];

        public static void OnInitialize()
        {
            Hooking.OnLevelLoaded += OnLevelLoaded;
            AssetWarehouse._onReady += new Action(() =>
            {
                try
                {
                    LoadCampaignsFromMods();
                }
                catch (Exception ex)
                {
                    MelonLogger.Error("Coudnt load the campaigns from the mods folder: " + ex.Message);
                }

                AssetWarehouse.Instance.OnPalletAdded += new Action<Barcode>((barcode) =>
                {
                    LoadCampaignsFromMods();
                });
            });
        }

        public static void LoadCampaignsFromMods()
        {
            string[] modPaths = Directory.GetDirectories(MarrowSDK.RuntimeModsPath);

            foreach (string mod in modPaths)
            {
                string[] jsonPaths2 = Directory.GetFiles(mod, "campaign.json.bundle");

                if (jsonPaths2.Length != 0 && !RegisteredJsonPaths.Contains(jsonPaths2[0]))
                {
                    RegisteredJsonPaths.Add(jsonPaths2[0]);
                    string jsonContent2 = File.ReadAllText(jsonPaths2[0]);
                    try
                    {
                        RegisterCampaignFromJson(jsonContent2);
                    }
                    catch
                    {

                    }
                }
                
            }
        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            if(!CampaignUtilities.IsCampaignLevel(info.barcode, out Session, out _)) return;

            lastLoadedCampaignLevel = info.barcode;
        }

        public static void OnUIRigCreated()
        {
            if (!SessionActive) return;

            var popUpMenu = Player.UIRig.popUpMenu;

            if (Session.RestrictDevTools && !Session.saveData.DevToolsUnlocked)
            {
                popUpMenu.crate_SpawnGun = new GenericCrateReference(new Barcode("null.empty.barcode"));
                popUpMenu.crate_Nimbus = new GenericCrateReference(new Barcode("null.empty.barcode"));
            }
        }
    }
}
