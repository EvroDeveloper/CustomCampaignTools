#if MELONLOADER
#else
using SLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
#endif
using System;

namespace CustomCampaignTools
{
    [Flags]
    public enum AvatarRestrictionType
    {
        None = 0,
        [Obsolete("Use CampaignLoadingData.DisableBodyLog instead")]
        DisableBodyLog = 1,
        RestrictAvatar = 2,
        EnforceWhitelist = 4,
        EnforceStatRange = 8
    }
}
