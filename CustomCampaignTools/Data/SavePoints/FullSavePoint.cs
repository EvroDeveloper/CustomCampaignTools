using System;
using BoneLib;
using CustomCampaignTools.Data.SimpleSerializables;
using CustomCampaignTools.Patching;
using Il2CppSLZ.Marrow;
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
            playerPosition = new(Player.RigManager.physicsRig.centerOfPressure.position),
            playerForward = new(Player.RigManager.physicsRig.centerOfPressure.forward),
            playerAmmoSave = AmmoSave.CreateFromPlayer(),
            playerInventoryData = InventoryData.GetFromRigmanager(Player.RigManager),
            sceneEntityData = SceneEntityData.GenerateSceneEntitySave(),
        };

        return savePoint;
    }

    public void LoadSave()
    {
        if(SceneStreamer.Session.Level.Barcode != levelBarcode.ToBarcode() || true) // force on for now for easy reload testing
        {
            // Start a scene load and then manage all the shit at specific times in the scene streamer
            // Might be a headache overriding crate spawners
            FadeLoader.Load(levelBarcode.ToBarcode(), new Il2CppSLZ.Marrow.Warehouse.Barcode(CommonBarcodes.Maps.LoadDefault));

            // Temporarily disable Crate Spawner spawning
            CrateSpawnerAwakePatch.BlockCrateSpawns = true;

            // Set up things to happen
            LevelLoadingPatches.OnNextSceneLoaded += () => 
            { 
                CrateSpawnerAwakePatch.BlockCrateSpawns = false; 
                Player.RigManager.Teleport(playerPosition.ToVector3());
                playerInventoryData.ApplyToRigManagerDelayed();
                sceneEntityData.RestoreAllLevelEntities();
                sceneEntityData.SpawnAllSpawnedEntities();
            };
            AmmoInventoryPatches.OnNextAwake += (a) => playerAmmoSave.AddToPlayer();
        }
        else
        {
            Player.RigManager.Teleport(playerPosition.ToVector3(), playerForward.ToVector3());
            AmmoInventory.Instance.ClearAmmo();
            playerAmmoSave.AddToPlayer();
            playerInventoryData.ApplyToRigmanager(Player.RigManager);
            sceneEntityData.RestoreAllLevelEntities();
            // Also need to figure out how to despawn all cratespawners, and then respawn them from the scene entity data. Doing that later
        }
    }
}
