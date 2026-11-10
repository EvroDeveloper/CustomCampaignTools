using SimpleSerializables.Types;
using CustomCampaignTools.Patching;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;
using System;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools.SaveData.Version1;

public partial class CampaignSaveData
{
    public class SavePoint
    {
        public BarcodeSer LevelBarcode;
        public Vector3Ser Position;
        public float RotationAngle;

        public AmmoCount MidLevelAmmoSave;
        public InventoryData InventoryData;
        public List<BarcodePosRot> BoxContainedBarcodes;
        public List<int> DespawnedSpawners;
        public Dictionary<int, bool> ObjectEnabledSaves;

        public SavePoint()
        {

        }

        public SavePoint(Barcode levelBarcode, SimpleTransform transform, InventoryData inventoryData, AmmoCount ammoCount, List<BarcodePosRot> boxContainedBarcodes, List<int> savedDespawns, Dictionary<int, bool> savedEnableds)
        {
            LevelBarcode = new(levelBarcode);
            
            Position = new(transform.position);

            RotationAngle = transform.rotation.eulerAngles.y;

            InventoryData = inventoryData;
            MidLevelAmmoSave = ammoCount;

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
            if (Position == null || Position == Vector3.zero)
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

        public void LoadContinue(LevelCrateReference loadScene)
        {
            LoadContinue(loadScene.Barcode);
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
            return Quaternion.AngleAxis(RotationAngle, Vector3.up) * Vector3.forward;
        }
    }
}
