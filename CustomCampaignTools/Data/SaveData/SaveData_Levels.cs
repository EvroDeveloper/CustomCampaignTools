using System.Collections.Generic;
using Newtonsoft.Json;

namespace CustomCampaignTools;

public partial class CampaignSaveData
{
    [JsonProperty]
    public List<string> UnlockedLevels = [];

    public void UnlockLevel(string barcode)
    {
        if (!UnlockedLevels.Contains(barcode))
        {
            UnlockedLevels.Add(barcode);
            SaveToDisk();
        }
    }

    public void LockLevel(string barcode)
    {
        if (UnlockedLevels.Contains(barcode))
        {
            UnlockedLevels.Remove(barcode);
            SaveToDisk();
        }
    }
}