using BoneLib;
using CustomCampaignTools.AvatarRestriction;
using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.Utilities;
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

namespace CustomCampaignTools;

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
    public MonoDiscReference LoadSceneMusicReference { get; private set; } = new MonoDiscReference(Barcode.EmptyBarcode());
    public AudioClip LoadSceneMusic { get; private set; }
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
    public AudioClip AchievementUnlockSound { get; private set; }
    public MonoDiscReference AchievementUnlockSoundReference { get; private set; } = new MonoDiscReference(Barcode.EmptyBarcode());
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

    internal Campaign(CampaignLoadingData data)
    {
        if (data == null) throw new NullReferenceException("Campaign Loading Data cannot be Null");

        Name = data.Name;
        PalletBarcode = data.PalletBarcode;

        RegisterCampaignLevels();
        RegisterMenuStuff();
        RegisterCheatRestrictions();
        RegisterLevelSavings();
        RegisterCrateBullshit();
        RegisterAchievements();
        RegisterExperimentalFeatures();

        DEVMODE = data.DevBuild;

        void RegisterCampaignLevels()
        {
            if (data.InitialLevel.IsValid()) MenuLevel = new CampaignLevel(data.InitialLevel, CampaignLevelType.Menu);
            else MenuLevel = new CampaignLevel(data.MainLevels[0], CampaignLevelType.MainLevel);

            if (data.IntroLevel.IsValid()) IntroLevel = new CampaignLevel(data.IntroLevel, CampaignLevelType.Intro);
            else IntroLevel = new CampaignLevel(MenuLevel.BarcodeString, MenuLevel.Title, CampaignLevelType.Intro);

            MainLevels = [.. data.MainLevels.Select(l => new CampaignLevel(l, CampaignLevelType.MainLevel))];
            ExtraLevels = [.. data.ExtraLevels.Select(l => new CampaignLevel(l, CampaignLevelType.ExtraLevel))];
            foreach (CampaignLevel level in AllLevels)
            {
                barcodeToCampaignLevelRegistry[level.Barcode.ID] = level;
            }

            if (data.LoadScene.IsValid())
            {
                LoadScene = data.LoadScene;
            }

            if (data.LoadSceneMusic.IsValid())
            {
                LoadSceneMusicReference = data.LoadSceneMusic;
            }
        }

        void RegisterMenuStuff()
        {
            ShowInMenu = data.ShowInMenu;

            PrioritizeInLevelPanel = data.PrioritizeInLevelPanel;
            LockLevelsUntilEntered = data.UnlockableLevels;
        }

        void RegisterCheatRestrictions()
        {
            if (data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.EnforceWhitelist))
                avatarRestrictor = new WhitelistAvatarRestrictor([.. data.WhitelistedAvatars]);
            else if (data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.RestrictAvatar))
                avatarRestrictor = new DefaultAvatarRestrictor(data.CampaignAvatar, data.BaseGameFallbackAvatar);
            else if (data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.EnforceStatRange))
                avatarRestrictor = new StatBasedAvatarRestrictor(data.AvatarStatRanges);

            IsBodylogRestricted = data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.DisableBodyLog);

            RestrictDevTools = data.RestrictDevTools;
            LockInCampaign = data.LockInCampaign;
        }

        void RegisterLevelSavings()
        {
            SaveLevelInventory = data.SaveLevelWeapons;
            InventorySaveLimit = [.. data.InventorySaveLimit];
            SaveLevelAmmo = data.SaveLevelAmmo;
            CreateSaveOnLevelEnter = data.UpdateSaveOnLevelEnter;
        }

        void RegisterCrateBullshit()
        {
            if (data.CampaignUnlockCrates != null)
                CampaignUnlockCrates = [.. data.CampaignUnlockCrates];

            if (data.HideCratesFromGachapon != null)
                HiddenCrates = [.. data.HideCratesFromGachapon];
        }

        void RegisterAchievements()
        {
            Achievements = data.Achievements ?? [];
            foreach (AchievementData ach in Achievements)
            {
                ach.Init();
            }

            if (data.AchievementUnlockSound.IsValid())
            {
                AchievementUnlockSoundReference = data.AchievementUnlockSound;
            }
        }

        void RegisterExperimentalFeatures()
        {
            RigManagerOverride = data.RigManagerOverride;
            GameplayRigOverride = data.GameplayRigOverride;
            CampaignSupportAssembly = data.CampaignSupportAssembly.LoadAssembly((a) =>
            {
                AssemblyUtils.RegisterAssemblyMonoBehaviours(a);
                AssemblyUtils.HarmonyPatchAssembly(a, $"{data.Name}.supportassembly.patches");
            });
        }
    }

#region Campaign Registration
    internal static Campaign RegisterCampaign(CampaignLoadingData data)
    {
        try
        {
            Campaign campaign = new(data);
            campaign.saveData = CampaignSaveData.LoadFromDisk(campaign);

            AssetWarehouse._onReady += new Action(campaign.LoadRequiredAssets);

            CampaignUtilities.AddCampaign(campaign);

            return campaign;
        }
        catch (Exception ex)
        {
            CampaignLogger.Error($"Failed to register campaign {data.Name}: {ex} {ex.StackTrace}");
            return null;
        }
    }

    internal static Campaign RegisterCampaignFromPallet(Pallet pallet)
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

        return RegisterCampaignFromJson(json, pallet.Barcode);
    }

    internal static Campaign RegisterCampaignFromJson(string json, Barcode palletBarcode)
    {
        var settings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        };

        CampaignLoadingData campaignValueHolder = JsonConvert.DeserializeObject<CampaignLoadingData>(json, settings);
        campaignValueHolder.PalletBarcode = new(palletBarcode);
        return RegisterCampaign(campaignValueHolder);
    }

    internal void LoadRequiredAssets()
    {
        if (LoadSceneMusicReference.TryGetDataCard(out var _loadMusicDatacard))
        {
            _loadMusicDatacard.AudioClip.LoadAsset(new Action<AudioClip>((a) =>
            {
                LoadSceneMusic = a;
                LoadSceneMusic.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }));
        }


        if (AchievementUnlockSoundReference.TryGetDataCard(out var _achievementUnlockSoundDatacard))
        {
            _achievementUnlockSoundDatacard.AudioClip.LoadAsset(new Action<AudioClip>((a) =>
            {
                AchievementUnlockSound = a;
                AchievementUnlockSound.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }));
        }
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
        FadeLoader.Load(new Barcode(GameManager.currentGameConfiguration.mainMenuBarcode), MarrowGame.marrowSettings.DefaultLoadingLevel.Barcode);
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