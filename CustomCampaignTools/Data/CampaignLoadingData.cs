using System;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    internal class CampaignLoadingData
    {
        public int Version { get; set; } = 0;
        public string Name { get; set; }
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
        public bool SaveLevelWeapons { get; set; }
        public List<string> InventorySaveLimit { get; set; }
        public bool SaveLevelAmmo { get; set; }
        public string AchievementUnlockSound { get; set; }
        public List<AchievementData> Achievements { get; set; }
        public bool LockInCampaign { get; set; }
        public List<string> CampaignUnlockCrates { get; set; }
        public List<string> HideCratesFromGachapon { get; set; }
        public bool DevBuild { get; set; } = false;
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
        EnforceWhitelist = 4
    }
}
