using Newtonsoft.Json;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        [JsonProperty]
        public bool DevToolsUnlocked = false;
        [JsonProperty]
        public bool AvatarUnlocked = false;
        [JsonProperty]
        public bool ManualBodylogToggle = false;

        [JsonProperty]
        public bool SkipIntro = false;

        public void UnlockDevTools()
        {
            DevToolsUnlocked = true;
            SaveToDisk();
        }

        public void UnlockAvatar()
        {
            AvatarUnlocked = true;
            SaveToDisk();
        }

        public void SetManualBodylogToggle(bool value)
        {
            ManualBodylogToggle = value;
            SaveToDisk();
        }
    }
}