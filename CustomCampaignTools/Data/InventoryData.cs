using Il2CppSLZ.Marrow;
using System.Collections;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    public class InventoryData
    {
        public List<InventoryItem> InventoryItems = new List<InventoryItem>();

        public void ApplyToRigmanager(RigManager rm)
        {
            for (int i = 0; i < InventoryItems.Count; i++)
            {
                var bodySlot = rm.inventory.bodySlots[i].inventorySlotReciever;
                var item = InventoryItems[i];

                item.SpawnInSlot(bodySlot);
            }
        }

        public static InventoryData GetFromRigmanager(RigManager rm)
        {
            InventoryData output = new InventoryData();
            foreach (var bodySlot in rm.inventory.bodySlots)
            {
                if(bodySlot.InventorySlotReciever != null)
                    output.InventoryItems.Add(InventoryItem.GetFromBodyslot(bodySlot.inventorySlotReciever));
            }
            return output;
        }
    }

    public struct InventoryItem
    {
        public string Barcode { get; set; }

        public InventoryItem(string Barcode)
        {
            this.Barcode = Barcode;
        }

        public void SpawnInSlot(InventorySlotReciever slot)
        {
            if(Barcode != string.Empty)
                slot.SpawnInSlotAsync(new Barcode(Barcode));
        }

        public static InventoryItem GetFromBodyslot(InventorySlotReciever slot)
        {
            if (slot._weaponHost == null) return new InventoryItem(string.Empty);

            var itemPoolee = slot._slottedWeapon.interactableHost.marrowEntity._poolee;
            if(itemPoolee == null) return new InventoryItem(string.Empty);

            if (itemPoolee.SpawnableCrate == null) return new InventoryItem(string.Empty);

            return new InventoryItem(itemPoolee.SpawnableCrate.Barcode.ID);
        }
    }

    public struct GunInfo
    {
        public bool MagInserted;
        public bool MagAmmoCount;
        public bool RoundChambered;
        // uhh anything else? might check hahoos gun saving
    }
}