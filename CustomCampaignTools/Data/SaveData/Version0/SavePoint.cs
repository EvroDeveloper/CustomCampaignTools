using SimpleSerializables.Types;
using System.Runtime.Serialization;
using UnityEngine;
using System.Collections.Generic;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.SaveData.Version0;

public class SavePoint
{
    public BarcodeSer LevelBarcode = "null.empty.barcode";
    public Vector3Ser Position = new Vector3Ser(0, 0, 0);

    public float PositionX = 0;
    public float PositionY = 0;
    public float PositionZ = 0;

    public float RotationAngle = 0;

    public InventoryData InventoryData = new();
    public Version1.AmmoCount MidLevelAmmoSave = new();
    public List<Version1.CampaignSaveData.BarcodePosRot> BoxContainedBarcodes = [];
    public List<int> DespawnedSpawners = [];
    public Dictionary<int, bool> ObjectEnabledSaves = [];

    [OnDeserialized]
    public void OnDeserialized(StreamingContext context)
    {
        if (Position == null || (/*Position == Vector3.zero && */(PositionX != 0 || PositionY != 0 || PositionZ != 0)))
        {
            Position = new(PositionX, PositionY, PositionZ);
        }
    }

    public Version1.CampaignSaveData.SavePoint Convert()
    {
        return new Version1.CampaignSaveData.SavePoint()
        {
            LevelBarcode = LevelBarcode,
            Position = Position,
            RotationAngle = RotationAngle,
            InventoryData = InventoryData?.Convert(),
            MidLevelAmmoSave = MidLevelAmmoSave,
            BoxContainedBarcodes = BoxContainedBarcodes,
            DespawnedSpawners = DespawnedSpawners,
            ObjectEnabledSaves = ObjectEnabledSaves,
        };
    }
}
