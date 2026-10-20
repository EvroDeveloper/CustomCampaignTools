#if MELONLOADER
#pragma warning disable IDE0028 // Simplify collection initialization
#else
using SLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
#endif
using SimpleSerializables.Types;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools
{
    internal class CampaignLoadingData
    {
        public string Version { get; set; } = "0.0.0"; // Default version to 0 if Version doesnt exist in Json
#region 1.0.0
        public string Name { get; set; }
        public SerializedLevelSetup InitialLevel { get; set; }
        public List<SerializedLevelSetup> MainLevels { get; set; }
        public List<SerializedLevelSetup> ExtraLevels { get; set; }
        public LevelCrateRefSer LoadScene { get; set; }
        public MonoDiscRefSer LoadSceneMusic { get; set; }
        public bool UnlockableLevels { get; set; }
        public bool ShowInMenu { get; set; }
        public bool RestrictDevTools { get; set; }
        public AvatarRestrictionType AvatarRestrictionType { get; set; }
        public AvatarCrateRefSer CampaignAvatar { get; set; } = new AvatarCrateRefSer();
        public AvatarCrateRefSer BaseGameFallbackAvatar { get; set; } = new AvatarCrateRefSer();
        public List<AvatarCrateRefSer> WhitelistedAvatars { get; set; } = new List<AvatarCrateRefSer>();
        public bool SaveLevelWeapons { get; set; } = false;
        public List<SpawnableCrateRefSer> InventorySaveLimit { get; set; } = new List<SpawnableCrateRefSer>();
        public bool SaveLevelAmmo { get; set; } = true;
        public List<AchievementData> Achievements { get; set; } = new List<AchievementData>();
        public bool LockInCampaign { get; set; } = false;
        public List<SpawnableCrateRefSer> CampaignUnlockCrates { get; set; } = new List<SpawnableCrateRefSer>();
#endregion

#region 1.1.0
        public BarcodeSer PalletBarcode { get; set; } // Set by Campaign Registerer
        public SerializedLevelSetup IntroLevel { get; set; } = new SerializedLevelSetup();
        public bool PrioritizeInLevelPanel { get; set; } = true;
        public MonoDiscRefSer AchievementUnlockSound { get; set; } = new MonoDiscRefSer();
        public List<SpawnableCrateRefSer> HideCratesFromGachapon { get; set; } = new List<SpawnableCrateRefSer>();
        public bool DevBuild { get; set; } = false;
#endregion

#region 1.2.0
        public bool DisableBodyLog { get; set; }
        public AvatarStatRanges AvatarStatRanges { get; set; }
        public bool UpdateSaveOnLevelEnter { get; set; } = true;
        public SpawnableCrateRefSer RigManagerOverride { get; set; } = new SpawnableCrateRefSer();
        public SpawnableCrateRefSer GameplayRigOverride { get; set; } = new SpawnableCrateRefSer();
        public AssemblySer CampaignSupportAssembly { get; set; } = new AssemblySer();
#endregion

        // i just made some BULLSHITTTTT
#if UNITY_EDITOR
        public static CampaignLoadingData CopyFrom(CampaignSettings campaignSettings)
        {
            return new CampaignLoadingData()
            {
                Version = CampaignConstants.CurrentVersion.ToString(),
                Name = campaignSettings.Name,
                IntroLevel = campaignSettings.IntroLevel.Serialize(),
                InitialLevel = campaignSettings.MainMenu.Serialize(),
                MainLevels = SerializedLevelSetup.CopyFrom(campaignSettings.MainLevels),
                ExtraLevels = SerializedLevelSetup.CopyFrom(campaignSettings.ExtraLevels),
                LoadScene = new LevelCrateRefSer(campaignSettings.LoadScene.Barcode),
                LoadSceneMusic = new MonoDiscRefSer(campaignSettings.LoadSceneMusic.Barcode),
                UnlockableLevels = campaignSettings.UnlockableLevels,
                ShowInMenu = campaignSettings.ShowCampaignInMenu,
                PrioritizeInLevelPanel = campaignSettings.PrioritizeInLevelPanel,
                RestrictDevTools = campaignSettings.RestrictDevTools,
                AvatarRestrictionType = campaignSettings.AvatarRestriction,
                DisableBodyLog = campaignSettings.DisableBodyLog,
                WhitelistedAvatars = campaignSettings.WhitelistedAvatars.Select(a => { return new AvatarCrateRefSer(a); }).ToList(),
                CampaignAvatar = new AvatarCrateRefSer(campaignSettings.CampaignAvatar),
                BaseGameFallbackAvatar = new AvatarCrateRefSer(campaignSettings.BaseGameFallbackAvatar),
                AvatarStatRanges = new AvatarStatRanges(campaignSettings.AvatarHeightRange, campaignSettings.AvatarMassRange, campaignSettings.AvatarArmRange),
                SaveLevelWeapons = campaignSettings.SaveInventoryBetweenLevels,
                InventorySaveLimit = campaignSettings.SaveInventoryFilter.Select(a => { return new SpawnableCrateRefSer(a); }).ToList(),
                SaveLevelAmmo = campaignSettings.SaveAmmoBetweenLevels,
                UpdateSaveOnLevelEnter = campaignSettings.UpdateSaveOnLevelEnter,
                AchievementUnlockSound = new MonoDiscRefSer(campaignSettings.AchievementUnlockSound),
                Achievements = campaignSettings.Achievements.ToList(),
                LockInCampaign = campaignSettings.LockPlayerInCampaign,
                CampaignUnlockCrates = campaignSettings.CampaignUnlockCrates.Select(a => { return new SpawnableCrateRefSer(a); }).ToList(),
                HideCratesFromGachapon = campaignSettings.HideCratesFromGachapon.Select(a => { return new SpawnableCrateRefSer(a); }).ToList(),
                RigManagerOverride = new(campaignSettings.RigManagerOverride),
                GameplayRigOverride = new(campaignSettings.GameplayRigOverride),
                CampaignSupportAssembly = new(campaignSettings.CampaignSupportAssembly),
                DevBuild = campaignSettings.DevBuild,
            };
        }
#endif
    }
}

#if MELONLOADER
#pragma warning restore IDE0028 // Simplify collection initialization
#endif
