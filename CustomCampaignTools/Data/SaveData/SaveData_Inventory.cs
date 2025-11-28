using System;
using System.Collections.Generic;
using System.Collections;
using MelonLoader;
using BoneLib;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal Dictionary<string, InventoryData> LoadedInventorySaves = [];

        public void SaveInventoryForLevel(string nextLevelBarcode)
        {
            if (!campaign.SaveLevelInventory) return;
            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager, campaign.InventorySaveLimit);
            LoadedInventorySaves[nextLevelBarcode] = inventoryData;
        }
    }
}