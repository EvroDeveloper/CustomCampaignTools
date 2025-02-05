using BoneLib;
using MelonLoader;
using Il2CppSLZ.Interaction;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Il2CppSLZ.Marrow;
using CustomCampaignTools;

namespace CustomCampaignTools
{
    public static class SavepointFunctions
    {
        public static bool WasLastLoadByContinue = false;

        public static void SavePlayer(string levelBarcode, Vector3 position, Vector3 boxCollectorPosition, List<string> boxBarcodes = null)
        {
            Campaign campaign = CampaignUtilities.GetFromLevel(levelBarcode);

            var sideLf = Player.RigManager.inventory.bodySlots[0].inventorySlotReceiver;
            var backLf = Player.RigManager.inventory.bodySlots[2].inventorySlotReceiver;
            var sideRt = Player.RigManager.inventory.bodySlots[5].inventorySlotReceiver;
            var backRt = Player.RigManager.inventory.bodySlots[3].inventorySlotReceiver;
            var backCt = Player.RigManager.inventory.bodySlots[4].inventorySlotReceiver;

            string sideLeftBarcode = GetAmmoBarcodeFromSlot(sideLf);
            string backLeftBarcode = GetAmmoBarcodeFromSlot(backLf);
            string sideRightBarcode = GetAmmoBarcodeFromSlot(sideRt);
            string backRightBarcode = GetAmmoBarcodeFromSlot(backRt);
            string backCenterBarcode = GetAmmoBarcodeFromSlot(backCt);

            campaign.saveData.LoadedSavePoint = new CampaignSaveData.SavePoint(levelBarcode, position, backCenterBarcode, sideLeftBarcode, sideRightBarcode, backLeftBarcode, backRightBarcode, boxBarcodes, boxCollectorPosition);

            campaign.saveData.SaveToDisk();
        }

        private static string GetAmmoBarcodeFromSlot(InventorySlotReceiver slot)
        {
            if (slot._weaponHost == null) return string.Empty;

            var itemPoolee = slot._slottedWeapon.interactableHost.marrowEntity._poolee;
            if(itemPoolee == null) return string.Empty;

            if (itemPoolee.SpawnableCrate == null) return string.Empty;

            return itemPoolee.SpawnableCrate.Barcode.ID;
        }


        public static void LoadPlayerFromSave()
        {
            WasLastLoadByContinue = false;

            Campaign campaign = CampaignUtilities.GetFromLevel();

            CampaignSaveData.SavePoint savePoint = campaign.saveData.LoadedSavePoint;


            if (!campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;

            if (hasSpawnPoint)
            {
                Player.RigManager.Teleport(savePoint.GetPosition()); // Definately getting to here, and not erroring afterward

                foreach(string barcode in savePoint.BoxContainedBarcodes)
                {
                    MelonLogger.Msg($"Spawning Crate {barcode} at Box Container");
                    HelperMethods.SpawnCrate(barcode, savePoint.GetBoxPosition(), Quaternion.identity, Vector3.one, spawnAction: ((g) => { MelonLogger.Msg($"Successfully Spawned {barcode} in Box"); }));
                }
            }

            var bodySlots = Player.RigManager.inventory.bodySlots;

            bodySlots[0].inventorySlotReceiver.HolsterItemIfNotEmpty(campaign.saveData.LoadedSavePoint.LeftSidearmBarcode);
            bodySlots[2].inventorySlotReceiver.HolsterItemIfNotEmpty(campaign.saveData.LoadedSavePoint.LeftShoulderSlotBarcode);
            bodySlots[5].inventorySlotReceiver.HolsterItemIfNotEmpty(campaign.saveData.LoadedSavePoint.RightSidearmBarcode);
            bodySlots[3].inventorySlotReceiver.HolsterItemIfNotEmpty(campaign.saveData.LoadedSavePoint.RightShoulderSlotBarcode);
            bodySlots[4].inventorySlotReceiver.HolsterItemIfNotEmpty(campaign.saveData.LoadedSavePoint.BackSlotBarcode);
        }

        public static void HolsterItemIfNotEmpty(this InventorySlotReceiver slot, string barcode, Vector3 spawnPosition = default)
        {
            var task = slot.SpawnInSlotAsync(new Barcode(barcode));
        }

    }
}
