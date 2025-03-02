using System;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    internal class CampaignLoadingData
    {
        public string Name { get; set; }
        public string InitialLevel { get; set; }
        public List<string> MainLevels { get; set; }
        public List<string> ExtraLevels { get; set; }
        public string LoadScene { get; set; }
        public bool ShowInMenu { get; set; }
        public bool RestrictDevTools { get; set; }
        public bool RestrictAvatar { get; set; }
        public string CampaignAvatar { get; set; }
        public string[] WhitelistedAvatars { get; set; }
        public bool SaveLevelWeapons { get; set; }
        public bool SaveLevelAmmo { get; set; }
        public List<AchievementData> Achievements { get; set; }
        public AvatarRestrictionType AvatarRestrictionType { get; set; }

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