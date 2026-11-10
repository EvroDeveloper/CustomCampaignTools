using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools.SaveData.Version0
{
    public class InventoryData
    {
        public List<InventoryItem> InventoryItems = [];

        public Version1.InventoryData Convert()
        {
            return new Version1.InventoryData()
            {
                InventoryItems = [.. InventoryItems.Select(i => i.Convert())]
            };
        }
    }

    public struct InventoryItem
    {
        public string SlotName { get; set; }
        public string Barcode { get; set; }

        public Version1.InventoryItem Convert()
        {
            return new Version1.InventoryItem(SlotName, Barcode);
        }
    }
}