namespace CustomCampaignTools
{
    public class Campaign
    {
        public string Name;
        public string PalletBarcode;
        public string[] mainLevels;

        public CampaignSaveData saveData;

        public static List<Campaign> LoadedCampaigns = new List<Campaign>();

        public static Campaign RegisterCampaign(string Name, string[] mainLevels)
        {
            Campaign campaign = new Campaign();
            campaign.Name = name;
            campaign.mainLevels = mainLevels;
            campaign.saveData = new CampaignSaveData(campaign);
            LoadedCampaigns.Add(campaign);
            return campaign;
        }

        public int GetLevelIndex(string levelBarcode)
        {
            return Array.IndexOf(mainLevels, levelBarcode);
        }

        public string GetLevelBarcodeByIndex(int index)
        {
            return mainLevels[index];
        }
    }
}