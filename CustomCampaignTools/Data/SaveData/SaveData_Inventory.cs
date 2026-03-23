using System.Collections.Generic;
using BoneLib;
using CustomCampaignTools.Data;
using Newtonsoft.Json;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        [JsonProperty]
        public Dictionary<string, InventoryData> InventorySaves = [];

        public void SaveInventoryForLevel(string nextLevelBarcode)
        {
            if (!campaign.SaveLevelInventory) return;
            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager, campaign.InventorySaveLimit);
            InventorySaves[nextLevelBarcode] = inventoryData;
        }
    }
}