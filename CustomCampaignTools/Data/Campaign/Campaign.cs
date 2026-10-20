using BoneLib;
using CustomCampaignTools.AvatarRestriction;
using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.Utilities;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCampaignTools;

public partial class Campaign
{
    public string Name;
    public Barcode PalletBarcode;

#region Levels
    private CampaignLevel _introLevel;
    private MenuLevel _menuLevel;
    public CampaignLevel MenuLevel
    {
        get
        {
            if(_menuLevel != null) return _menuLevel;
            return MainLevels[0];
        }
    }

    public CampaignLevel InitialLevel
    {
        get
        {
            if (!saveData.SkipIntro && _introLevel != null && _introLevel.IsValid())
                return _introLevel;
            else
                return MenuLevel;
        }
    }
    public MainLevel[] MainLevels { get; private set; }
    public ExtraLevel[] ExtraLevels { get; private set; }
    public CampaignLevel[] AllLevels { get; private set; }
    private readonly Dictionary<string, CampaignLevel> barcodeToCampaignLevelRegistry = [];

#region Load Scene
    public LevelCrateReference LoadScene = new(Barcode.EmptyBarcode());
    public MonoDiscReference LoadSceneMusicReference { get; private set; } = new MonoDiscReference(Barcode.EmptyBarcode());
    public AudioClip LoadSceneMusic { get; private set; }
#endregion // Load Scene

#endregion // Levels

    public bool ShowInMenu;
    public bool PrioritizeInLevelPanel = true;

    public bool RestrictDevTools;
    public bool IsBodylogRestricted;
    public AvatarRestrictor avatarRestrictor;
    public bool ShouldRestrictAvatar => avatarRestrictor != null && !saveData.AvatarUnlocked;

    public bool SaveLevelInventory;
    public List<SpawnableCrateReference> InventorySaveLimit = [];
    public bool SaveLevelAmmo;
    public bool CreateSaveOnLevelEnter = false;
    public MonoDiscReference AchievementUnlockSoundReference { get; private set; } = new MonoDiscReference(Barcode.EmptyBarcode());
    public Dictionary<string, AchievementData> Achievements = [];
    public List<AchievementData> OrderedAchievements = [];
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
            if (data.InitialLevel.IsValid()) _menuLevel = new MenuLevel(this, data.InitialLevel);

            MainLevels = [.. data.MainLevels.Select(l => new MainLevel(this, l))];
            ExtraLevels = [.. data.ExtraLevels.Select(l => new ExtraLevel(this, l))];

            if (data.IntroLevel.IsValid()) _introLevel = new IntroLevel(this, data.IntroLevel);

            List<CampaignLevel> allLevels = [.. MainLevels, .. ExtraLevels];
            if(_menuLevel != null) allLevels.Insert(0, _menuLevel);
            if(_introLevel != null) allLevels.Insert(0, _introLevel);
            AllLevels = [.. allLevels];
            
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
                avatarRestrictor = new StatBasedAvatarRestrictor(data.AvatarStatRanges, data.CampaignAvatar, data.BaseGameFallbackAvatar);

            // DEBUG TESTING
            avatarRestrictor = new StatBasedAvatarRestrictor(new AvatarStatRanges(new Vector2(1.5f, 2f), new Vector2(0, float.PositiveInfinity), new Vector2(0, float.PositiveInfinity)), data.CampaignAvatar, data.BaseGameFallbackAvatar);

#pragma warning disable CS0618 // Type or member is obsolete
            IsBodylogRestricted = data.AvatarRestrictionType.HasFlag(AvatarRestrictionType.DisableBodyLog) || data.DisableBodyLog;
#pragma warning restore CS0618 // Type or member is obsolete

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
            foreach (AchievementData ach in data.Achievements ?? [])
            {
                Achievements.Add(ach.Key, ach);
                OrderedAchievements.Add(ach);
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
        FadeLoader.Load(GameManager.currentGameConfiguration.MainMenu, MarrowSettings.RuntimeInstance.DefaultLoadingLevel);
    }

    public int GetMainLevelIndex(Barcode levelBarcode)
    {
        if(TryGetLevel(levelBarcode, out var level) && level is MainLevel mainLevel)
        {
            return GetMainLevelIndex(mainLevel);
        }
        else
        {
            throw new ArgumentException("levelBarcode is not in the campaign or is not a main level");
        }
    }

    public int GetMainLevelIndex(MainLevel mainLevel)
    {
        return Array.IndexOf(MainLevels, mainLevel);
    }

    public CampaignLevel GetLevel(Barcode levelBarcode)
    {
        if (TryGetLevel(levelBarcode, out var level))
            return level;
        else
            return null;
    }

    public bool TryGetLevel(Barcode levelBarcode, out CampaignLevel campaignLevel)
    {
        return barcodeToCampaignLevelRegistry.TryGetValue(levelBarcode.ID, out campaignLevel);
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
        if(!CampaignUtilities.TryGetCampaignLevel(info.levelReference.Barcode, out CampaignLevel.Session)) return;

        Session = CampaignLevel.Session.campaign;

        lastLoadedCampaignLevel = CampaignLevel.Session;
    }

    public override string ToString()
    {
        return Name;
    }
}
