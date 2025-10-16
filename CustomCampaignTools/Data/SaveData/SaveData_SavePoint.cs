using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppSLZ.Marrow;
using AmmoInventory = Il2CppSLZ.Marrow.AmmoInventory;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal SavePoint LoadedSavePoint;

        public void ClearSavePoint()
        {
            LoadedSavePoint = new SavePoint();
            CampaignBoneMenu.RefreshCampaignPage(campaign);
            SaveToDisk();
        }

        public void SavePlayer(string levelBarcode, Vector3 position, List<BarcodePosRot> boxBarcodes = null)
        {
            InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager);

            boxBarcodes ??= new List<BarcodePosRot>();

            AmmoSave ammoSave = new AmmoSave();

            if (campaign.SaveLevelAmmo && position != Vector3.zero)
            {
                AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode);

                ammoSave = new AmmoSave
                {
                    LevelBarcode = levelBarcode,
                    LightAmmo = AmmoInventory.Instance.GetCartridgeCount("light") - previousAmmoSave.LightAmmo,
                    MediumAmmo = AmmoInventory.Instance.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo,
                    HeavyAmmo = AmmoInventory.Instance.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo
                };
            }

            List<string> savedDespawns = [];
            foreach (SpawnerDespawnSaver stateSaver in UnityEngine.Object.FindObjectsOfType<SpawnerDespawnSaver>())
            {
                if (stateSaver.DontSpawnAgain(out string id))
                {
                    savedDespawns.Add(id);
                }
            }

            Dictionary<int, bool> savedEnableds = [];
            foreach (ObjectEnabledSaver saver in GameObject.FindObjectsOfType<ObjectEnabledSaver>())
            {
                savedEnableds.Add(saver.uniqueID.Get(), saver.gameObject.activeSelf);
            }

            LoadedSavePoint = new SavePoint(levelBarcode, position, inventoryData, ammoSave, boxBarcodes, savedDespawns, savedEnableds);

            SaveToDisk();
            CampaignBoneMenu.RefreshCampaignPage(campaign);

        }
    }

    public struct SavePoint
    {
        public string LevelBarcode;
        public float PositionX;
        public float PositionY;
        public float PositionZ;

        public InventoryData InventoryData;
        public AmmoSave MidLevelAmmoSave;
        public List<BarcodePosRot> BoxContainedBarcodes;
        public List<string> DespawnedSpawners;
        public Dictionary<int, bool> ObjectEnabledSaves;

        public SavePoint()
        {

        }

        public SavePoint(string levelBarcode, Vector3 position, InventoryData inventoryData, AmmoSave ammoSave, List<BarcodePosRot> boxContainedBarcodes, List<string> savedDespawns, Dictionary<int, bool> savedEnableds)
        {
            LevelBarcode = levelBarcode;
            PositionX = position.x;
            PositionY = position.y;
            PositionZ = position.z;

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
            if (GetPosition() == Vector3.zero)
                hasSpawnPoint = false;
            else
                hasSpawnPoint = true;

            if (LevelBarcode == null || LevelBarcode == string.Empty)
                return false;

            return true;
        }

        public void LoadContinue(Campaign campaign)
        {
            LoadContinue(new Barcode(campaign.LoadScene));
        }

        public void LoadContinue(Barcode loadScene)
        {
            if (!IsValid(out _)) return;

            SavepointFunctions.WasLastLoadByContinue = true;

            FadeLoader.Load(new Barcode(LevelBarcode), loadScene);
        }

        public bool GetEnabledStateFromID(int id, bool defaultEnabled)
        {
            if (ObjectEnabledSaves.Keys.Contains(id))
                return ObjectEnabledSaves[id];
            else
                return defaultEnabled;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(PositionX, PositionY, PositionZ);
        }
    }

    public struct BarcodePosRot
    {
        public string barcode;

        public float x;
        public float y;
        public float z;

        public float qX;
        public float qY;
        public float qZ;
        public float qW;

        public float sX;
        public float sY;
        public float sZ;

        public BarcodePosRot(Barcode barcode, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            this.barcode = barcode.ID;

            x = position.x;
            y = position.y;
            z = position.z;

            qX = rotation.x;
            qY = rotation.y;
            qZ = rotation.z;
            qW = rotation.w;

            sX = scale.x;
            sY = scale.y;
            sZ = scale.z;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(x, y, z);
        }

        public Quaternion GetRotation()
        {
            return new Quaternion(qX, qY, qZ, qW);
        }

        public Vector3 GetScale()
        {
            return new Vector3(sX, sY, sZ);
        }
    }
}