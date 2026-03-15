using BoneLib;
using CustomCampaignTools.AvatarRestriction;
using CustomCampaignTools.Bonemenu;
using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport.BoneLab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSystem.Numerics;
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

    public class Campaign
    {
        public string Name;
        public Barcode PalletBarcode;

        public CampaignLevel IntroLevel { get; private set; }
        public CampaignLevel MenuLevel { get; private set; }

        public CampaignLevel InitialLevel
        {
            get
            {
                if (!saveData.SkipIntro && IntroLevel.IsValid())
                    return IntroLevel;
                else
                    return MenuLevel;
            }
        }
        public CampaignLevel[] MainLevels { get; private set; }
        public CampaignLevel[] ExtraLevels { get; private set; }
        public CampaignLevel[] AllLevels { get; private set; }
        private readonly Dictionary<string, CampaignLevel> barcodeToCampaignLevelRegistry = [];

        public LevelCrateReference LoadScene;
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
        public bool IsBodylogRestricted;
        public IAvatarRestrictor avatarRestrictor;

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

// EXPERIMENTAL FEATURES
        public SpawnableCrateReference RigManagerOverride;
        public SpawnableCrateReference GameplayRigOverride;
        public Assembly CampaignSupportAssembly;

        public bool DEVMODE { get; private set; } = false;

        public CampaignSaveData saveData;

        public static Campaign Session;
        public static CampaignLevel lastLoadedCampaignLevel;
        public static bool SessionActive { get => Session != null; }
        public static bool SessionLocked { get => SessionActive && _sessionLocked; }

        private static bool _sessionLocked;


        internal static Campaign RegisterCampaign(CampaignLoadingData data)
        {
            Campaign campaign = new();
            try
            {
                campaign.Name = data.Name;
                campaign.PalletBarcode = data.PalletBarcode;

                campaign.saveData = CampaignSaveData.LoadFromDisk(campaign);

                if (data.InitialLevel.levelBarcode == Barcode.EMPTY) campaign.MenuLevel = new CampaignLevel(data.MainLevels[0], CampaignLevelType.MainLevel);
                else campaign.MenuLevel = new CampaignLevel(data.InitialLevel, CampaignLevelType.Menu);

                if (data.IntroLevel == null || data.IntroLevel.levelBarcode == Barcode.EMPTY) campaign.IntroLevel = new CampaignLevel(campaign.MenuLevel.BarcodeString, campaign.MenuLevel.Title, CampaignLevelType.Intro);
                else campaign.IntroLevel = new CampaignLevel(data.IntroLevel, CampaignLevelType.Intro);

                campaign.MainLevels = [.. data.MainLevels.Select(l => new CampaignLevel(l, CampaignLevelType.MainLevel))];
                campaign.ExtraLevels = [.. data.ExtraLevels.Select(l => new CampaignLevel(l, CampaignLevelType.ExtraLevel))];
                campaign.AllLevels = [.. campaign.MainLevels, .. campaign.ExtraLevels, campaign.MenuLevel, campaign.IntroLevel];
                foreach(CampaignLevel level in campaign.AllLevels)
                {
                    campaign.barcodeToCampaignLevelRegistry[level.Barcode.ID] = level;
                }

                campaign.LoadScene = data.LoadScene.IsValid() ? data.LoadScene : new LevelCrateReference(CommonBarcodes.Maps.LoadMod);

                if (data.LoadSceneMusic.IsValid())
                {
                    data.LoadSceneMusic.ToScannableReference().TryGetDataCard(out campaign._loadMusicDatacard);
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

                if(data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.EnforceWhitelist))
                    campaign.avatarRestrictor = new WhitelistAvatarRestrictor([.. data.WhitelistedAvatars]);
                else if(data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.RestrictAvatar))
                    campaign.avatarRestrictor = new DefaultAvatarRestrictor(data.CampaignAvatar, data.BaseGameFallbackAvatar);
                else if(data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.EnforceStatRange))
                    campaign.avatarRestrictor = new StatBasedAvatarRestrictor(data.AvatarStatRanges);
                
                campaign.IsBodylogRestricted = data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.DisableBodyLog);

                campaign.RestrictDevTools = data.RestrictDevTools;

                campaign.SaveLevelInventory = data.SaveLevelWeapons;
                campaign.InventorySaveLimit = data.InventorySaveLimit;
                campaign.SaveLevelAmmo = data.SaveLevelAmmo;
                campaign.Achievements = data.Achievements;

                campaign.LockInCampaign = data.LockInCampaign;

                if(data.CampaignUnlockCrates != null)
                    campaign.CampaignUnlockCrates = [.. data.CampaignUnlockCrates];

                if(data.AchievementUnlockSound.IsValid())
                {
                    if(data.AchievementUnlockSound.ToScannableReference().TryGetDataCard(out campaign._achievementUnlockSoundDatacard))
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

                if(data.HideCratesFromGachapon != null)
                    GashaponHider.AddCratesToHide([.. data.HideCratesFromGachapon]);

                campaign.RigManagerOverride = new(data.RigManagerOverride);
                campaign.GameplayRigOverride = new(data.GameplayRigOverride);
                campaign.CampaignSupportAssembly = data.CampaignSupportAssembly.LoadAssembly();

                campaign.DEVMODE = data.DevBuild;

                CampaignUtilities.AddCampaign(campaign);
            }
            catch (Exception ex)
            {
                CampaignLogger.Error(campaign, $"Failed to register campaign {data.Name}: {ex} {ex.StackTrace}");
            }

            return campaign;
        }

        public static Campaign RegisterCampaignFromPallet(Pallet pallet)
        {
            if (!AssetWarehouse.Instance.TryGetPalletManifest(pallet.Barcode, out var manifest))
            {
                CampaignLogger.Error($"Failed to register campaign from pallet {pallet.Barcode}: Pallet manifest not found");
                return null;
            }

            string campaignJsonPath = Path.Combine(Path.GetDirectoryName(manifest.PalletPath), "campaign.json.bundle");
            if (!File.Exists(campaignJsonPath)) return null;
            CampaignLogger.Msg("Json FOUND for Pallet " + pallet.Title);

            string json = File.ReadAllText(campaignJsonPath);

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            CampaignLoadingData campaignValueHolder = JsonConvert.DeserializeObject<CampaignLoadingData>(json, settings);
            campaignValueHolder.PalletBarcode = new(pallet.Barcode);
            return RegisterCampaign(campaignValueHolder);
        }

        public void Enter()
        {
            if(LockInCampaign)
            {
                _sessionLocked = true;
            }
            //Campaign.Session = this;
            
            FadeLoader.Load(InitialLevel.Barcode, LoadScene.Barcode);
        }

        public static void Exit()
        {
            _sessionLocked = false;
            FadeLoader.Load(new Barcode(CommonBarcodes.Maps.VoidG114), new Barcode(CommonBarcodes.Maps.LoadDefault));
        }

        public int GetMainLevelIndex(Barcode levelBarcode)
        {
            return MainLevels.ToList().FindIndex(l => l.Barcode == levelBarcode);
        }

        public CampaignLevel GetLevel(Barcode levelBarcode)
        {
            return barcodeToCampaignLevelRegistry[levelBarcode.ID];
        }

        public CampaignLevel[] GetUnlockedLevels(bool includeRedacted = false)
        {
            HashSet<CampaignLevel> levels = [];

            levels.Add(MenuLevel);
            foreach (CampaignLevel mainLevel in MainLevels)
            {
                if(!saveData.UnlockedLevels.Contains(mainLevel.BarcodeString) && LockLevelsUntilEntered) continue;
                if (!includeRedacted && mainLevel.Redacted) continue;
                levels.Add(mainLevel);
            }
            foreach (CampaignLevel extraLevel in ExtraLevels)
            {
                if(!saveData.UnlockedLevels.Contains(extraLevel.BarcodeString) && LockLevelsUntilEntered) continue;
                if (!includeRedacted && extraLevel.Redacted) continue;
                levels.Add(extraLevel);
            }
            return [.. levels];
        }

        public static List<string> RegisteredJsonPaths = [];

        public static void OnInitialize()
        {
            Hooking.OnLevelLoaded += OnLevelLoaded;
        }

        public static void OnLevelLoaded(LevelInfo info)
        {
            if(!CampaignUtilities.TryGetFromLevel(info.levelReference.Barcode, out Session, out var campaignLevel)) return;

            lastLoadedCampaignLevel = campaignLevel;
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
