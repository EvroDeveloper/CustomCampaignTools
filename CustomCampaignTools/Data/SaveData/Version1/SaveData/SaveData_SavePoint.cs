using BoneLib;
using SimpleSerializables.Types;
using CustomCampaignTools.Debug;
using CustomCampaignTools.SDK;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;
using UnityEngine;
using CustomCampaignTools.GameSupport;
using System.Collections.Generic;

namespace CustomCampaignTools.SaveData.Version1;

public partial class CampaignSaveData
{
    [JsonProperty]
    public SavePoint LoadedSavePoint = new();

    public void ClearSavePoint()
    {
        LoadedSavePoint = new SavePoint();
        GameManager.currentGameConfiguration.RefreshCampaignMenu(campaign);
        SaveToDisk();
    }

    public bool TryGetSavePoint(out SavePoint loadedSavePoint)
    {
        loadedSavePoint = LoadedSavePoint;
        return LoadedSavePoint != null && LoadedSavePoint.IsValid(out _);
    }

    public void SavePlayer(CampaignLevel campaignLevel, SimpleTransform transform, List<BarcodePosRot> boxBarcodes = null)
    {
        InventoryData inventoryData = InventoryData.GetFromRigmanager(Player.RigManager);

        boxBarcodes ??= [];

        AmmoCount ammoCount = new();

        if (campaign.SaveLevelAmmo)
        {
            ammoCount = AmmoCount.GetFromPlayer();
            if(campaignLevel is MainLevel mainLevel)
            {
                ammoCount -= GetPreviousLevelsAmmoSave(mainLevel);
            }
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

        LoadedSavePoint = new SavePoint(campaignLevel.Barcode, transform, inventoryData, ammoCount, boxBarcodes, savedDespawns, savedEnableds);

        CampaignLogger.Msg(campaign, $"Saved player at {transform.position} in level {campaignLevel.Title}");

        SaveToDisk();
        GameManager.currentGameConfiguration.RefreshCampaignMenu(campaign);
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
