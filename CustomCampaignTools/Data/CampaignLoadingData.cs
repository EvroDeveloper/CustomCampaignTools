using System;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    internal class CampaignLoadingData
    {
        public string Name { get; set; }
        public SerializedLevelSetup InitialLevel { get; set; }
        public List<SerializedLevelSetup> MainLevels { get; set; }
        public List<SerializedLevelSetup> ExtraLevels { get; set; }
        public string LoadScene { get; set; }
        public string LoadSceneMusic { get; set; }
        public bool UnlockableLevels { get; set; }
        public bool ShowInMenu { get; set; }
        public bool RestrictDevTools { get; set; }
        public AvatarRestrictionType AvatarRestrictionType { get; set; }
        public string CampaignAvatar { get; set; }
        public string BaseGameFallbackAvatar { get; set; }
        public List<string> WhitelistedAvatars { get; set; }
        public bool SaveLevelWeapons { get; set; }
        public bool SaveLevelAmmo { get; set; }
        public List<AchievementData> Achievements { get; set; }
        public bool LockInCampaign { get; set; }
        public List<string> CampaignUnlockCrates { get; set; }
    }

    internal class SerializedLevelSetup
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
