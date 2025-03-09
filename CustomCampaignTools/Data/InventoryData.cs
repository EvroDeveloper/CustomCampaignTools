using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools
{
    public class InventoryData
    {
        public List<InventoryItem> InventoryItems = new List<InventoryItem>();

        public void ApplyToRigmanager(RigManager rm)
        {
            for (int i = 0; i < InventoryItems.Count; i++)
            {
                var item = InventoryItems[i];
                try
                {
                    var realBodySlot = rm.inventory.bodySlots.Where((s) => { return s.gameObject.name == item.SlotName; }).First().inventorySlotReceiver;
                    if (realBodySlot != null)
                    {
                        item.SpawnInSlot(realBodySlot);
                    }
                }
                catch
                {
                }
            }
        }

        public static InventoryData GetFromRigmanager(RigManager rm)
        {
            InventoryData output = new();
            foreach (var bodySlot in rm.inventory.bodySlots)
            {
                if (bodySlot.inventorySlotReceiver != null)
                {
                    var item = InventoryItem.GetFromBodyslot(bodySlot);
                    if (item.Barcode != string.Empty)
                    {
                        output.InventoryItems.Add(item);
                    }
                }
            }
            return output;
        }
    }

    public struct InventoryItem
    {
        public string SlotName { get; set; }
        public string Barcode { get; set; }

        public InventoryItem(string SlotName, string Barcode)
        {
            this.SlotName = SlotName;
            this.Barcode = Barcode;
        }

        public InventoryItem()
        {
            SlotName = string.Empty;
            Barcode = string.Empty;
        }

        public void SpawnInSlot(InventorySlotReceiver slot)
        {
            if(Barcode != null && Barcode != string.Empty && slot != null)
                slot.SpawnInSlotAsync(new Barcode(Barcode));
        }

        public static InventoryItem GetFromBodyslot(SlotContainer slot)
        {
            if (slot.inventorySlotReceiver._weaponHost == null) return new InventoryItem();

            var itemPoolee = slot.inventorySlotReceiver._slottedWeapon.interactableHost.marrowEntity._poolee;
            if(itemPoolee == null) return new InventoryItem();

            if (itemPoolee.SpawnableCrate == null) return new InventoryItem();

            return new InventoryItem(slot.gameObject.name, itemPoolee.SpawnableCrate.Barcode.ID);
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