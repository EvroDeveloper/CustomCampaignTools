using BoneLib.Nullables;
using BoneLib;
using Labworks.Behaviors;
using Labworks.Data;
using Labworks.Utilities;
using MelonLoader;
using SLZ.Interaction;
using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;
using SLZ.Props.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Labworks
{
    internal class SavepointFunctions
    {
        public static bool WasLastLoadByContinue = false;

        public static void SavePlayer(string levelBarcode, Vector3 position, Vector3 boxCollectorPosition, List<string> boxBarcodes = null)
        {
            var sideLf = BoneLib.Player.rigManager.inventory.bodySlots[0];
            var backLf = BoneLib.Player.rigManager.inventory.bodySlots[2];
            var sideRt = BoneLib.Player.rigManager.inventory.bodySlots[5];
            var backRt = BoneLib.Player.rigManager.inventory.bodySlots[3];
            var backCt = BoneLib.Player.rigManager.inventory.bodySlots[4];

            string sideLeftBarcode = GetAmmoBarcodeFromSlot(sideLf);
            string backLeftBarcode = GetAmmoBarcodeFromSlot(backLf);
            string sideRightBarcode = GetAmmoBarcodeFromSlot(sideRt);
            string backRightBarcode = GetAmmoBarcodeFromSlot(backRt);
            string backCenterBarcode = GetAmmoBarcodeFromSlot(backCt);

            LabworksSaving.LoadedSavePoint = new LabworksSaving.SavePoint(levelBarcode, position, backCenterBarcode, sideLeftBarcode, sideRightBarcode, backLeftBarcode, backRightBarcode, boxBarcodes, boxCollectorPosition);

            LabworksSaving.SaveToDisk();
        }

        private static string GetAmmoBarcodeFromSlot(SlotContainer slot)
        {
            if (slot.inventorySlotReceiver._weaponHost != null)
                return slot.inventorySlotReceiver._slottedWeapon.interactableHost.GetComponentInParent<AssetPoolee>().spawnableCrate.Barcode.ID;

            return string.Empty;
        }


        public static void LoadPlayerFromSave()
        {
            WasLastLoadByContinue = false;

            if (!SaveParsing.IsSavePointValid(LabworksSaving.LoadedSavePoint, out bool hasSpawnPoint))
                return;

            if (hasSpawnPoint)
            {
                LabworksSaving.SavePoint savePoint = LabworksSaving.LoadedSavePoint;

                BoneLib.Player.rigManager.Teleport(new Vector3(savePoint.PositionX, savePoint.PositionY, savePoint.PositionZ));

                foreach(string barcode in savePoint.BoxContainedBarcodes)
                {
                    AssetSpawner.Spawn(new Spawnable() { crateRef = new SpawnableCrateReference(barcode) }, new Vector3(savePoint.BoxContainedX, savePoint.BoxContainedY, savePoint.BoxContainedZ));
                }
            }

            HolsterItemIfNotEmpty(LabworksSaving.LoadedSavePoint.LeftSidearmBarcode, 0);
            HolsterItemIfNotEmpty(LabworksSaving.LoadedSavePoint.LeftShoulderSlotBarcode, 2);
            HolsterItemIfNotEmpty(LabworksSaving.LoadedSavePoint.RightSidearmBarcode, 3);
            HolsterItemIfNotEmpty(LabworksSaving.LoadedSavePoint.RightShoulderSlotBarcode, 4);
            HolsterItemIfNotEmpty(LabworksSaving.LoadedSavePoint.BackSlotBarcode, 5);
        }

        public static void HolsterItemIfNotEmpty(string barcode, int arraySlot)
        {
            if (barcode == string.Empty) return;

            var slot = BoneLib.Player.rigManager.inventory.bodySlots[arraySlot];
            var head = Player.playerHead.transform;

            var reference = new SpawnableCrateReference(barcode);

            var spawnable = new Spawnable()
            {
                crateRef = reference
            };

            AssetSpawner.Register(spawnable);

            AssetSpawner.Spawn(spawnable, head.position + head.forward, default, new BoxedNullable<Vector3>(null), false, new BoxedNullable<int>(null), (Action<GameObject>)Action);
            return;

            void Action(GameObject go)
            {
                slot.GetComponent<InventorySlotReceiver>().DropWeapon();
                MelonLogger.Msg("Loaded object in holster with barcode ");
                slot.GetComponent<InventorySlotReceiver>().InsertInSlot(go.GetComponent<InteractableHost>());
            }
        }

        public static void ClearSavePoint()
        {
            LabworksSaving.LoadedSavePoint = new LabworksSaving.SavePoint(string.Empty, Vector3.zero, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, Vector3.zero);
            LabworksSaving.SaveToDisk();
        }

    }
}
