using System;
using BoneLib;
using CustomCampaignTools.Data.SimpleSerializables;
using Il2CppSLZ.Marrow.SceneStreaming;

namespace CustomCampaignTools.Data.SavePoints;

public class FullSavePoint
{
    public BarcodeSer levelBarcode;
    public Vector3Ser playerPosition;
    public Vector3Ser playerForward;
    public AmmoSave playerAmmoSave;
    public InventoryData playerInventoryData;
    public SceneEntityData sceneEntityData;


    public static FullSavePoint CreateSavePoint()
    {
        FullSavePoint savePoint = new()
        {
            levelBarcode = new(SceneStreamer.Session.Level.Barcode),
            playerPosition = new(Player.RigManager.transform.position),
            playerAmmoSave = AmmoSave.CreateFromPlayer(),
            playerInventoryData = InventoryData.GetFromRigmanager(Player.RigManager),
            sceneEntityData = SceneEntityData.GenerateSceneEntitySave(),
        };

        return savePoint;
    }

    public bool IsValid()
    {
        return true;
    }

    public void LoadContinue()
    {
    }

    public void OnSceneLoadedFromContinue()
    {
        // Ideally prevent all normal crate spawners from spawning
        // Teleport Player
        Player.RigManager.Teleport(playerPosition.ToVector3(), playerForward.ToVector3());

        // Apply Ammo Save
        // Apply Scene Entity Data
        // Apply Inventory Save (inventory entities shouldnt end up saved in the first place)
        playerInventoryData.ApplyToRigmanager(Player.RigManager);
    }
}
