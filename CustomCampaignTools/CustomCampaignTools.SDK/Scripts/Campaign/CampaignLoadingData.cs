using System;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    internal class CampaignLoadingData
    {
        public int Version { get; set; } = 1;
        public string Name { get; set; }
        public string PalletBarcode { get; set; }
        public SerializedLevelSetup IntroLevel { get; set; }
        public SerializedLevelSetup InitialLevel { get; set; }
        public List<SerializedLevelSetup> MainLevels { get; set; }
        public List<SerializedLevelSetup> ExtraLevels { get; set; }
        public string LoadScene { get; set; }
        public string LoadSceneMusic { get; set; }
        public bool UnlockableLevels { get; set; }
        public bool ShowInMenu { get; set; }
        public bool PrioritizeInLevelPanel { get; set; }
        public bool RestrictDevTools { get; set; }
        public AvatarRestrictionType AvatarRestrictionType { get; set; }
        public string CampaignAvatar { get; set; }
        public string BaseGameFallbackAvatar { get; set; }
        public List<string> WhitelistedAvatars { get; set; }
        public AvatarStatRanges AvatarStatRanges { get; set; }
        public bool SaveLevelWeapons { get; set; }
        public List<string> InventorySaveLimit { get; set; }
        public bool SaveLevelAmmo { get; set; }
        public bool UpdateSaveOnLevelEnter { get; set; }
        public string AchievementUnlockSound { get; set; }
        public List<AchievementData> Achievements { get; set; }
        public bool LockInCampaign { get; set; }
        public List<string> CampaignUnlockCrates { get; set; }
        public List<string> HideCratesFromGachapon { get; set; }
        public string RigManagerOverride { get; set; }
        public string GameplayRigOverride { get; set; }
        public bool DevBuild { get; set; } = false;

// i just made some BULLSHITTTTT
#if UNITY_EDITOR
        public static CampaignLoadingData CopyFrom(CampaignSettings campaignSettings)
        {
            return new CampaignLoadingData()
            {
                Name = campaignSettings.Name,
                IntroLevel = campaignSettings.IntroLevel.Serialize(),
                InitialLevel = campaignSettings.MainMenu.Serialize(),
                MainLevels = SerializedLevelSetup.CopyFrom(campaignSettings.MainLevels),
                ExtraLevels = SerializedLevelSetup.CopyFrom(campaignSettings.ExtraLevels),
                LoadScene = campaignSettings.LoadScene.Barcode.ID,
                LoadSceneMusic = campaignSettings.LoadSceneMusic.Barcode.ID,
                UnlockableLevels = campaignSettings.UnlockableLevels,
                ShowInMenu = campaignSettings.ShowCampaignInMenu,
                PrioritizeInLevelPanel = campaignSettings.PrioritizeInLevelPanel,
                RestrictDevTools = campaignSettings.RestrictDevTools,
                AvatarRestrictionType = campaignSettings.AvatarRestriction,
                WhitelistedAvatars = CampaignSettings.CrateArrayToBarcodes(campaignSettings.WhitelistedAvatars),
                CampaignAvatar = campaignSettings.CampaignAvatar.Barcode.ID,
                BaseGameFallbackAvatar = campaignSettings.BaseGameFallbackAvatar.Barcode.ID,
                SaveLevelWeapons = campaignSettings.SaveInventoryBetweenLevels,
                InventorySaveLimit = CampaignSettings.CrateArrayToBarcodes(campaignSettings.SaveInventoryFilter),
                SaveLevelAmmo = campaignSettings.SaveAmmoBetweenLevels,
                AchievementUnlockSound = campaignSettings.AchievementUnlockSound.Barcode.ID,
                Achievements = SerializableAchievement.ConvertToData(campaignSettings.Achievements),
                LockInCampaign = campaignSettings.LockPlayerInCampaign,
                CampaignUnlockCrates = CampaignSettings.CrateArrayToBarcodes(campaignSettings.CampaignUnlockCrates),
                HideCratesFromGachapon = CampaignSettings.CrateArrayToBarcodes(campaignSettings.HideCratesFromGachapon),
                DevBuild = campaignSettings.DevBuild,
            };
        }
#endif
    }

    public class SerializedLevelSetup
    {
        public string levelBarcode;
        public string levelName;
    }

    [Flags]
    public enum AvatarRestrictionType
    {
        None = 0,
        DisableBodyLog = 1,
        RestrictAvatar = 2,
        EnforceWhitelist = 4,
        EnforceStatRange = 8
    }

    public class AvatarStatRanges
    {
        public float heightRangeLow;
        public float heightRangeHigh;

        public float massRangeLow;
        public float massRangeHigh;
        
        public float armRangeLow;
        public float armRangeHigh;
    }
}
