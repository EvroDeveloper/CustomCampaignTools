using System.Collections.Generic;
using Newtonsoft.Json;

namespace CustomCampaignTools;

internal partial class CampaignSaveData
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