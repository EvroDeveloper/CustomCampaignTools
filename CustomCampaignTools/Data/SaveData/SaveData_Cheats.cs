namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal bool DevToolsUnlocked = false;
        internal bool AvatarUnlocked = false;
        internal bool SkipIntro = false;

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
    }
}