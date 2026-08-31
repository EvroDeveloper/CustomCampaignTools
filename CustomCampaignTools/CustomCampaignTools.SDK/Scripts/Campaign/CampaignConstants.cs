using System;

namespace CustomCampaignTools
{
    public static class CampaignConstants
    {
        internal const string ModName = "CustomCampaignTools";
        internal const string ModDescription = "Allows Modders to add their own Campaign functionality to Marrow Campaigns.";
        internal const string ModAuthor = "EvroDev";
        internal const string ModCompany = "LabWorks";

        public const string CampaignJsonFileName = "campaign.json.bundle";

        public const string Version = "1.2.0";
        public const uint CurrentVersionMajor = 1;
        public const uint CurrentVersionMinor = 2;
        public const uint CurrentVersionPatch = 0;

        public static readonly CampaignVersion CurrentVersion = new CampaignVersion(Version);
    }
}
