using System;
using BoneLib;
using FullSave.Data.SimpleSerializables;
using FullSave.Utilities;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using Newtonsoft.Json;

namespace FullSave.Data;

public class FullSavePoint
{
    public DateTime creationDate;
    public BarcodeSer levelBarcode;
    public string levelName;
    public Vector3Ser playerPosition;
    public Vector3Ser playerForward;
    public AmmoSave playerAmmoSave;
    public InventoryData playerInventoryData;
    public SceneEntityData sceneEntityData;


    public static FullSavePoint CreateSavePoint()
    {
        FullSavePoint savePoint = new()
        {
            creationDate = DateTime.Now,
            levelBarcode = new(SceneStreamer.Session.Level.Barcode),
            levelName = SceneStreamer.Session.Level.Title,
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
            CrateSpawnerBlocker.BlockCrateSpawns = true;

            // Set up things to happen
            SceneLoaderUtils.OnNextSceneLoaded += () => 
            { 
                CrateSpawnerBlocker.BlockCrateSpawns = false; 
                Player.RigManager.Teleport(playerPosition.ToVector3());
                TeleportBlocker.BlockTeleportsForDelay();
                playerInventoryData.ApplyToRigManagerDelayed();
                sceneEntityData.RestoreAllLevelEntities();
                sceneEntityData.SpawnAllSpawnedEntities();
            };
            AmmoInventoryPatches.OnNextAmmoInventoryAwake += playerAmmoSave.AddToPlayer;
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
