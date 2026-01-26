namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal List<string> UnlockedLevels = [];

        public void UnlockLevel(string barcode)
        {
            if (!UnlockedLevels.Contains(barcode))
            {
                UnlockedLevels.Add(barcode);
            }
        }
    }
}