using BoneLib;
using CustomCampaignTools.AvatarRestriction;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Utilities;
using Il2CppSLZ.Bonelab;
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

    public class Campaign
    {
        public string Name;
        public Barcode PalletBarcode;

#region Levels
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
        public CampaignLevel[] AllLevels
        {
            get
            {
                _allLevels ??= [.. MainLevels, .. ExtraLevels, MenuLevel, IntroLevel];
                return _allLevels;
            }
        }
        private CampaignLevel[] _allLevels;
        private readonly Dictionary<string, CampaignLevel> barcodeToCampaignLevelRegistry = [];

#region Load Scene
        public LevelCrateReference LoadScene = new LevelCrateReference(Barcode.EmptyBarcode());
        public AudioClip LoadSceneMusic { get; private set; }
        private MonoDisc _loadMusicDatacard;
        #endregion // Load Scene

        #endregion // Levels

        public bool ShowInMenu;
        public bool PrioritizeInLevelPanel = true;

        public bool RestrictDevTools;
        public bool IsBodylogRestricted;
        public IAvatarRestrictor avatarRestrictor;

        public bool SaveLevelInventory;
        public List<SpawnableCrateReference> InventorySaveLimit = [];
        public bool SaveLevelAmmo;
        public bool CreateSaveOnLevelEnter = false;
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
        public List<SpawnableCrateReference> HiddenCrates = [];

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

#region Campaign Registration
        internal static Campaign RegisterCampaign(CampaignLoadingData data)
        {
            Campaign campaign = new();
            try
            {
#if DEBUG
                NullReferenceCheck();
#endif
                campaign.Name = data.Name;
                campaign.PalletBarcode = data.PalletBarcode;

                RegisterCampaignLevels();
                RegisterMenuStuff();
                RegisterCheatRestrictions();
                RegisterLevelSavings();
                RegisterAchievements();
                RegisterCrateBullshit();
                RegisterExperimentalFeatures();

                campaign.DEVMODE = data.DevBuild;

                campaign.saveData = CampaignSaveData.LoadFromDisk(campaign);

                CampaignUtilities.AddCampaign(campaign);
            }
            catch (Exception ex)
            {
                CampaignLogger.Error(campaign, $"Failed to register campaign {data.Name}: {ex} {ex.StackTrace}");
            }

            return campaign;

#if DEBUG
            void NullReferenceCheck()
            {
                string[] nullFields = CampaignDebugger.GetAllNullFieldsInObject(data);
                if(nullFields.Length == 0)
                {
                    CampaignLogger.Msg(campaign, "No null fields in loading data.");
                }
                else
                {
                    CampaignLogger.Error(campaign, "The following fields are null in the loading data: ");
                    foreach(string nullBruh in nullFields)
                    {
                        CampaignLogger.Error(nullBruh);
                    }
                }
            }
#endif

            void RegisterCampaignLevels()
            {
                if (data.InitialLevel.IsValid()) campaign.MenuLevel = new CampaignLevel(data.InitialLevel, CampaignLevelType.Menu);
                else campaign.MenuLevel = new CampaignLevel(data.MainLevels[0], CampaignLevelType.MainLevel);

                if (data.IntroLevel.IsValid()) campaign.IntroLevel = new CampaignLevel(data.IntroLevel, CampaignLevelType.Intro);
                else campaign.IntroLevel = new CampaignLevel(campaign.MenuLevel.BarcodeString, campaign.MenuLevel.Title, CampaignLevelType.Intro);

                campaign.MainLevels = [.. data.MainLevels.Select(l => new CampaignLevel(l, CampaignLevelType.MainLevel))];
                campaign.ExtraLevels = [.. data.ExtraLevels.Select(l => new CampaignLevel(l, CampaignLevelType.ExtraLevel))];
                foreach(CampaignLevel level in campaign.AllLevels)
                {
                    campaign.barcodeToCampaignLevelRegistry[level.Barcode.ID] = level;
                }

                if (data.LoadScene.IsValid())
                {
                    campaign.LoadScene = data.LoadScene;
                }

                if (data.LoadSceneMusic.IsValid())
                {
                    if(data.LoadSceneMusic.ToScannableReference() == null) CampaignLogger.Msg("What the fuck are we doing here");
                    if(data.LoadSceneMusic.ToScannableReference().TryGetDataCard(out campaign._loadMusicDatacard))
                    {
                        campaign._loadMusicDatacard.AudioClip.LoadAsset(new Action<AudioClip>((a) =>
                        {
                            campaign.LoadSceneMusic = a;
                            campaign.LoadSceneMusic.hideFlags = HideFlags.DontUnloadUnusedAsset;
                        }));
                    }
                }
                else
                {
                    CampaignLogger.Msg("Invalid Load Scene Music");
                }
            }

            void RegisterMenuStuff()
            {
                campaign.ShowInMenu = data.ShowInMenu;
                
                campaign.PrioritizeInLevelPanel = data.PrioritizeInLevelPanel;
                campaign.LockLevelsUntilEntered = data.UnlockableLevels;
            }

            void RegisterCheatRestrictions()
            {
                if(data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.EnforceWhitelist))
                    campaign.avatarRestrictor = new WhitelistAvatarRestrictor([.. data.WhitelistedAvatars]);
                else if(data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.RestrictAvatar))
                    campaign.avatarRestrictor = new DefaultAvatarRestrictor(data.CampaignAvatar, data.BaseGameFallbackAvatar);
                else if(data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.EnforceStatRange))
                    campaign.avatarRestrictor = new StatBasedAvatarRestrictor(data.AvatarStatRanges);
                
                campaign.IsBodylogRestricted = data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.DisableBodyLog);

                campaign.RestrictDevTools = data.RestrictDevTools;
                campaign.LockInCampaign = data.LockInCampaign;
            }

            void RegisterLevelSavings()
            { 
                campaign.SaveLevelInventory = data.SaveLevelWeapons;
                campaign.InventorySaveLimit = [.. data.InventorySaveLimit];
                campaign.SaveLevelAmmo = data.SaveLevelAmmo;
                campaign.CreateSaveOnLevelEnter = data.UpdateSaveOnLevelEnter;
            }

            void RegisterCrateBullshit()
            {
                if(data.CampaignUnlockCrates != null)
                    campaign.CampaignUnlockCrates = [.. data.CampaignUnlockCrates];

                if(data.HideCratesFromGachapon != null)
                    campaign.HiddenCrates = [.. data.HideCratesFromGachapon];
            }

            void RegisterAchievements()
            {
                campaign.Achievements = data.Achievements ?? [];
                foreach (AchievementData ach in campaign.Achievements)
                {
                    ach.Init();
                }

                if(data.AchievementUnlockSound.IsValid())
                {
                    if(data.AchievementUnlockSound.ToScannableReference().TryGetDataCard(out campaign._achievementUnlockSoundDatacard))
                    campaign._achievementUnlockSoundDatacard.AudioClip.LoadAsset(new Action<AudioClip>((a) =>
                    {
                        campaign._achievementUnlockSound = a;
                        campaign._achievementUnlockSound.hideFlags = HideFlags.DontUnloadUnusedAsset;
                    }));
                }
            }

            void RegisterExperimentalFeatures()
            {
                campaign.RigManagerOverride = data.RigManagerOverride;
                campaign.GameplayRigOverride = data.GameplayRigOverride;
                campaign.CampaignSupportAssembly = data.CampaignSupportAssembly.LoadAssembly((a) => 
                {
                    AssemblyUtils.RegisterAssemblyMonoBehaviours(a);
                    AssemblyUtils.HarmonyPatchAssembly(a, $"{data.Name}.supportassembly.patches");
                });
            }
        }

        public static Campaign RegisterCampaignFromPallet(Pallet pallet)
        {
            if (!AssetWarehouse.Instance.TryGetPalletManifest(pallet.Barcode, out var manifest))
            {
                CampaignLogger.Error($"Failed to register campaign from pallet {pallet.Barcode}: Pallet manifest not found");
                return null;
            }

            string campaignJsonPath = Path.Combine(Path.GetDirectoryName(manifest.PalletPath), CampaignConstants.CampaignJsonFileName);
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
#endregion
        
        public void Enter()
        {
            if(LockInCampaign)
            {
                _sessionLocked = true;
            }
            
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
            if(barcodeToCampaignLevelRegistry.ContainsKey(levelBarcode.ID))
                return barcodeToCampaignLevelRegistry[levelBarcode.ID];
            else
                return null;
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
                popUpMenu.crate_SpawnGun = new GenericCrateReference(Barcode.EmptyBarcode());
                popUpMenu.crate_Nimbus = new GenericCrateReference(Barcode.EmptyBarcode());
            }
#if false
            if(!Session.avatarRestrictor.IsAvatarMenuAllowed())
            {
                Player.UIRig.popUpMenu.radialPageView.onActivated += (Il2CppSystem.Action<PageView>)((p) =>
                {
                    popUpMenu.RemoveAvatarsMenu();
                });
            }
#endif
        }
    }
}
