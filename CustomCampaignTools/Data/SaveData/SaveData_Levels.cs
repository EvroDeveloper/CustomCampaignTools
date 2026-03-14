using Newtonsoft.Json;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        [JsonProperty]
        public List<string> UnlockedLevels = [];

        public void UnlockLevel(string barcode)
        {
            if (!UnlockedLevels.Contains(barcode))
            {
                UnlockedLevels.Add(barcode);
            }
        }
    }
}