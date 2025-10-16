namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal Dictionary<string, InventoryData> LoadedInventorySaves = [];
        
        public void SaveInventoryForLevel(string nextLevelBarcode)
        {
            LogNull(campaign, "Campaign");

            if (!campaign.SaveLevelInventory) return;

            LogNull(Player.RigManager, "Player RigManager");
            LogNull(campaign.InventorySaveLimit, "Inventory Save Limit");

            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager, campaign.InventorySaveLimit);
            LogNull(LoadedInventorySaves, "LoadedInventorySaves");
            LoadedInventorySaves[nextLevelBarcode] = inventoryData;

            void LogNull(object obj, string name)
            {
                if (obj == null)
                {
                    MelonLogger.Error($"HELLO EVRO, {name.ToUpper()} IS NULL");
                }
            }
        }

        public InventoryData GetInventory(string levelBarcode)
        {
            foreach (string barcode in LoadedInventorySaves.Keys)
            {
                if (levelBarcode != barcode) continue;
                return LoadedInventorySaves[barcode];
            }
            return null;
        }
    }
}