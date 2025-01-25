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

namespace Labworks
{
    public static class SavepointFunctions
    {
        public static bool WasLastLoadByContinue = false;

        public static void SavePlayer(string levelBarcode, Vector3 position, Vector3 boxCollectorPosition, List<string> boxBarcodes = null)
        {
            Campaign campaign = Campaign.GetFromLevel(levelBarcode);

            var sideLf = Player.RigManager.inventory.bodySlots[0];
            var backLf = Player.RigManager.inventory.bodySlots[2];
            var sideRt = Player.RigManager.inventory.bodySlots[5];
            var backRt = Player.RigManager.inventory.bodySlots[3];
            var backCt = Player.RigManager.inventory.bodySlots[4];

            string sideLeftBarcode = GetAmmoBarcodeFromSlot(sideLf);
            string backLeftBarcode = GetAmmoBarcodeFromSlot(backLf);
            string sideRightBarcode = GetAmmoBarcodeFromSlot(sideRt);
            string backRightBarcode = GetAmmoBarcodeFromSlot(backRt);
            string backCenterBarcode = GetAmmoBarcodeFromSlot(backCt);

            campaign.saveData.LoadedSavePoint = new CampaignSaveData.SavePoint(levelBarcode, position, backCenterBarcode, sideLeftBarcode, sideRightBarcode, backLeftBarcode, backRightBarcode, boxBarcodes, boxCollectorPosition);

            campaign.saveData.SaveToDisk();
        }

        private static string GetAmmoBarcodeFromSlot(SlotContainer slot)
        {
            if (slot.inventorySlotReceiver._weaponHost != null)
                return slot.inventorySlotReceiver._slottedWeapon.interactableHost.GetComponentInParent<Poolee>().SpawnableCrate.Barcode.ID;

            return string.Empty;
        }


        public static void LoadPlayerFromSave()
        {
            WasLastLoadByContinue = false;

            Campaign campaign = Campaign.GetFromLevel();

            CampaignSaveData.SavePoint savePoint = campaign.saveData.LoadedSavePoint;


            if (!campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;

            if (hasSpawnPoint)
            {
                Player.RigManager.Teleport(savePoint.GetPosition());

                foreach(string barcode in savePoint.BoxContainedBarcodes)
                {
                    AssetSpawner.Spawn(new Spawnable() { crateRef = new SpawnableCrateReference(barcode) }, new Vector3(savePoint.BoxContainedX, savePoint.BoxContainedY, savePoint.BoxContainedZ));
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
            if (barcode == string.Empty) return;

            spawnPosition = slot.transform.position;

            var spawnable = new Spawnable()
            {
                crateRef = new SpawnableCrateReference(barcode)
            };

            AssetSpawner.Register(spawnable);
            AssetSpawner.Spawn(spawnable, spawnPosition, spawnCallback: (Action<GameObject>)Action);
            return;

            void Action(GameObject go)
            {
                slot.DropWeapon();
                MelonLogger.Msg("Loaded object in holster with barcode ");
                slot.InsertInSlot(go.GetComponentInChildren<InteractableHost>());
            }
        }

    }
}
