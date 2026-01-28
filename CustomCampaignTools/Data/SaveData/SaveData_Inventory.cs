using BoneLib;
using CustomCampaignTools.Data;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal Dictionary<string, InventoryData> InventorySaves = [];

        public void SaveInventoryForLevel(string nextLevelBarcode)
        {
            if (!campaign.SaveLevelInventory) return;
            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager, campaign.InventorySaveLimit);
            InventorySaves[nextLevelBarcode] = inventoryData;
        }
    }
}