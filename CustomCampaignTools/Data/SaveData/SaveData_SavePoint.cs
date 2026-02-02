using UnityEngine;
using BoneLib;
using Il2CppSLZ.Marrow.Warehouse;
using CustomCampaignTools.SDK;
using CustomCampaignTools.Bonemenu;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Data;
using CustomCampaignTools.Data.SimpleSerializables;
using CustomCampaignTools.Patching;
using System.Runtime.Serialization;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        public SavePoint LoadedSavePoint = new();

        public void ClearSavePoint()
        {
            LoadedSavePoint = new SavePoint();
            CampaignBoneMenu.RefreshCampaignPage(campaign);
            SaveToDisk();
        }

        public void SavePlayer(Barcode levelBarcode, Transform transform, List<BarcodePosRot> boxBarcodes = null)
        {
            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager);

            boxBarcodes ??= [];

            AmmoSave ammoSave = new();

            if (campaign.SaveLevelAmmo && transform != null)
            {
                AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode.ID);

                ammoSave = AmmoSave.CreateFromPlayer(levelBarcode) - previousAmmoSave;
            }

            List<int> savedDespawns = [];
            foreach (SpawnerDespawnSaver stateSaver in UnityEngine.Object.FindObjectsOfType<SpawnerDespawnSaver>())
            {
                if (stateSaver.hasBeenDespawned)
                {
                    savedDespawns.Add(stateSaver.uniqueID.Get());
                }
            }

            Dictionary<int, bool> savedEnableds = [];
            foreach (ObjectEnabledSaver saver in GameObject.FindObjectsOfType<ObjectEnabledSaver>())
            {
                savedEnableds.Add(saver.uniqueID.Get(), saver.gameObject.activeSelf);
            }

            LoadedSavePoint = new SavePoint(levelBarcode, transform, inventoryData, ammoSave, boxBarcodes, savedDespawns, savedEnableds);

            CampaignLogger.Msg(campaign, $"Saved player at {transform.position} in level {levelBarcode}");

            SaveToDisk();
            CampaignBoneMenu.RefreshCampaignPage(campaign);
        }

        public class SavePoint
        {
            public BarcodeSer LevelBarcode;
            public Vector3Ser Position;

            [Obsolete("Use new Position Vector3Ser")] public float PositionX { get; private set; }
            [Obsolete("Use new Position Vector3Ser")] public float PositionY { get; private set; }
            [Obsolete("Use new Position Vector3Ser")] public float PositionZ { get; private set; }

            public float RotationAngle;

            public InventoryData InventoryData;
            public AmmoSave MidLevelAmmoSave;
            public List<BarcodePosRot> BoxContainedBarcodes;
            public List<int> DespawnedSpawners;
            public Dictionary<int, bool> ObjectEnabledSaves;

            [OnDeserialized]
            public void OnDeserialized(StreamingContext context)
            {
#pragma warning disable CS0618
                if(Position == Vector3.zero && (PositionX != 0 || PositionY != 0 || PositionZ != 0))
                {
                    Position = new(new Vector3(PositionX, PositionY, PositionZ)); // hehe new new
                }
#pragma warning restore CS0618
            }

            public SavePoint()
            {

            }

            public SavePoint(Barcode levelBarcode, Transform transform, InventoryData inventoryData, AmmoSave ammoSave, List<BarcodePosRot> boxContainedBarcodes, List<int> savedDespawns, Dictionary<int, bool> savedEnableds)
            {
                LevelBarcode = new(levelBarcode);
                
                Position = new(transform.position);

                RotationAngle = transform.eulerAngles.y;

                InventoryData = inventoryData;
                MidLevelAmmoSave = ammoSave;

                BoxContainedBarcodes = boxContainedBarcodes;
                DespawnedSpawners = savedDespawns;
                ObjectEnabledSaves = savedEnableds;
            }

            /// <summary>
            /// Returns true if the save point has a level barcode.
            /// </summary>
            /// <param name="out hasSpawnPoint"></param>
            /// <returns></returns>
            public bool IsValid(out bool hasSpawnPoint)
            {
                if (Position == Vector3.zero)
                    hasSpawnPoint = false;
                else
                    hasSpawnPoint = true;

                if (LevelBarcode == null || LevelBarcode == string.Empty)
                    return false;

                return true;
            }

            public void LoadContinue(Campaign campaign)
            {
                LoadContinue(campaign.LoadScene);
            }

            public void LoadContinue(Barcode loadScene)
            {
                if (!IsValid(out _)) return;

                SavepointFunctions.WasLastLoadByContinue = true;

                FadeLoader.Load(LevelBarcode, loadScene);

                // Prepare things to happen on next load
                AmmoInventoryPatches.OnNextAwake += (a) => Campaign.Session.saveData.LoadedSavePoint.MidLevelAmmoSave.AddToPlayer();
                LevelLoadingPatches.OnNextSceneLoaded += SavepointFunctions.LoadPlayerFromSave;
            }

            public bool GetEnabledStateFromID(int id, bool defaultEnabled)
            {
                if (ObjectEnabledSaves.ContainsKey(id))
                    return ObjectEnabledSaves[id];
                else
                    return defaultEnabled;
            }

            public Vector3 GetForwardVector()
            {
                return Quaternion.Euler(0, RotationAngle, 0) * new Vector3(0, 0, 1);
            }
        }

        public struct BarcodePosRot
        {
            public BarcodeSer barcode;
            public Vector3Ser position;
            public QuaternionSer rotation;
            public Vector3Ser scale;

            public BarcodePosRot(Barcode barcode, Vector3 position, Quaternion rotation, Vector3 scale)
            {
                this.barcode = new(barcode);

                this.position = new(position);
                this.rotation = new(rotation);
                this.scale = new(scale);
            }
        }
    }
}