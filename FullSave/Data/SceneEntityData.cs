using System;
using BoneLib;
using FullSave.ComponentSavers;
using FullSave.Data.SimpleSerializables;
using FullSave.Utilities;
using Il2CppInterop.Runtime;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using UnityEngine;

namespace FullSave.Data;

public class SceneEntityData
{
    // Level Entities - Hash from hierarchy and Name
    // Spawned Entities - Save barcode position and name
    public Dictionary<int, MarrowEntitySaveData> levelEntitySaves = [];
    public List<SpawnedEntitySave> spawnedEntitySaves = [];
    public List<SavedComponent> globalSavedComponents;

    static readonly Barcode[] invalidSaves =
    [
        new("yo rig manager heh")
    ];


    public static SceneEntityData GenerateSceneEntitySave()
    {
        SceneEntityData output = new();

        var entities = UnityEngine.Object.FindObjectsOfType<MarrowEntity>(true);

        foreach(MarrowEntity entity in entities)
        {
            output.SaveEntity(entity);
        }

        return output;
    }

    public void SaveEntity(MarrowEntity entity)
    {
        if(entity.GetComponentInParent<AssetSpawner>() != null) return; // Ignore template spawnables
        if(entity.GetComponentInParent<RigManager>() != null) return; // Ignore Rigmanager entities

        if(entity._poolee.SpawnableCrate == null)
        {
            // From-level entity, hash and save data
            // hash is necessary to find the same scene entity later when loading.
            int hashCode = HashingUtility.GetHierarchyHash(entity.transform);

            MarrowEntitySaveData entitySave = new(entity);
            levelEntitySaves.Add(hashCode, entitySave);
        }
        else
        {
            // From spawn entity, save.
            // Dont save spawned things that have been subsequently despawned. // Ammend, DO save despawned items so we can simulate an OnSpawn event before despawning them again
            // Also probably not good to save spawnables that are under the asset spawner, only save root level spawned entities
            if(entity.transform.parent == null)
            {
                spawnedEntitySaves.Add(new(entity));
            }
        }
    }

    public void RestoreAllLevelEntities()
    {
        foreach(MarrowEntity entity in UnityEngine.Object.FindObjectsOfType<MarrowEntity>(true))
        {
            try
            { 
                if(entity._poolee.SpawnableCrate == null)
                {
                    RestoreLevelEntity(entity);
                }
            }
            catch (Exception e)
            {
                MelonLogger.Msg(e.Message);
            }
        }

        foreach(SavedComponent component in globalSavedComponents)
        {
            component.ApplySave();
        }
    }

    public void SpawnAllSpawnedEntities()
    {
        foreach(SpawnedEntitySave spawnedEntitySave in spawnedEntitySaves)
        {
            spawnedEntitySave.Spawn();
        }
    }

    public void RestoreLevelEntity(MarrowEntity entity)
    {
        int hashCode = HashingUtility.GetHierarchyHash(entity.transform);
        if(levelEntitySaves.ContainsKey(hashCode))
        {
            levelEntitySaves[hashCode].ApplyToEntity(entity);
        }
    }
}

public class SpawnedEntitySave
{
    public BarcodeSer barcode;
    public int CrateSpawnerHash;
    public MarrowEntitySaveData saveData;

    public SpawnedEntitySave(MarrowEntity entity)
    {
        if(entity._poolee.SpawnableCrate != null)
        {
            barcode = new(entity._poolee.SpawnableCrate.Barcode);
            saveData = new(entity);
        }
    }

    public void Spawn(Action<MarrowEntity> callback = null)
    {
        HelperMethods.SpawnCrate(
            new SpawnableCrateReference(barcode.ToBarcode()),
            saveData.entityPosition.position.ToVector3(),
            saveData.entityPosition.rotation.ToQuaternion(),
            saveData.entityPosition.scale.ToVector3(),
            spawnAction: (gobj) =>
            {
                MarrowEntity entity = gobj.GetComponent<MarrowEntity>();

                saveData.ApplyToEntity(entity);

                if(CrateSpawnerHash != 0)
                {
                    var foundSpawner = HashingUtility.FindComponentWithMatchingHash<CrateSpawner>(CrateSpawnerHash);
                    foundSpawner?.OnPooleeSpawn(gobj); // Call the spawn event for things like decorators and ofc ultevents. Unsure about Nail decorators but ehh
                }

                callback?.Invoke(entity);
            }
        );
    }
}

public class MarrowEntitySaveData
{
    public string name;
    public bool isDespawned;
    public TransformSer entityPosition;
    public MarrowEntityPoseSer entityPose;
    public Vector3Ser[] bodyVelocities;
    public Vector3Ser[] bodyAngularVelocities;
    public List<SavedComponent> savedComponents;

    public MarrowEntitySaveData(MarrowEntity entity)
    {
        isDespawned = entity.IsDespawned;
        if(entity.IsDespawned) return; // Dont waste data on despawned things, just despawn it where it stands it doent matter

        name = entity.gameObject.name;

        entityPosition = new(entity.transform, false);

        MarrowEntityPose readPose = new();
        entity.ReadPose(ref readPose);

        entityPose = new(readPose);

        bool shouldApplyVelocities = false;
        List<Vector3Ser> tempBodyVelocities = [];
        List<Vector3Ser> tempBodyAngularVelocities = [];

        foreach(MarrowBody body in entity.Bodies)
        {
            if(body.TryGetRigidbody(out var rb))
            {
                if(!rb.IsSleeping())
                {
                    shouldApplyVelocities = true;
                }
                tempBodyVelocities.Add(new(rb.velocity));
                tempBodyAngularVelocities.Add(new(rb.angularVelocity));
            }
            else
            {
                tempBodyVelocities.Add(new(Vector3.zero));
                tempBodyAngularVelocities.Add(new(Vector3.zero));
            }
        }

        if(shouldApplyVelocities)
        {
            bodyVelocities = [.. tempBodyVelocities];
            bodyAngularVelocities = [.. tempBodyAngularVelocities];
        }

        // for every possible component saver, find components in children and save them
        foreach(Type type in ComponentSaverManager.GetValidComponentTypes())
        {
            IComponentSaver saver = ComponentSaverManager.GetComponentSaver(type);
            foreach(Component c in entity.gameObject.GetComponentsInChildren(Il2CppType.From(type)))
            {
                savedComponents.Add(new(c, saver, entity.transform));
            }
        }
    }

    public void ApplyToEntity(MarrowEntity entity)
    {
        if(isDespawned)
        {
            entity.Despawn();
            // Dont even trip dawg, no need to apply shit if its been despawned. Early returning right this instant...
            return;
        }

        entity.transform.position = entityPosition.position.ToVector3();
        entity.transform.rotation = entityPosition.rotation.ToQuaternion();

        entity.WritePose(entityPose.ToMarrowEntityPose());

        if(bodyVelocities != null && bodyVelocities.Length != 0 && bodyVelocities.Length == entity.Bodies.Length)
        {
            for(int i = 0; i < entity.Bodies.Length; i++)
            {
                if(entity.Bodies[i].TryGetRigidbody(out var rb))
                {
                    rb.velocity = bodyVelocities[i].ToVector3();
                    rb.angularVelocity = bodyAngularVelocities[i].ToVector3();
                }
            }
        }

        foreach(SavedComponent component in savedComponents)
        {
            component.ApplySave(entity);
        }
    }
}

public class SavedComponent
{
    public int componentHash;
    public string componentType;
    public byte[] savedState;

    public SavedComponent()
    {
        
    }

    public SavedComponent(Component component, IComponentSaver componentSaver, Transform root = null)
    {
        componentHash = HashingUtility.GetComponentHash(component, root);
        componentType = component.GetType().FullName;
        savedState = componentSaver.SaveComponentState(component);
    }

    public void ApplySave(MarrowEntity referenceEntity = null)
    {
        // Evaluate Type from componentType string
        Type realComponentType = Type.GetType(componentType);
        // Find ComponentSaver type for handling a LOT of things
        IComponentSaver componentSaver = ComponentSaverManager.GetComponentSaver(realComponentType);
        if(componentSaver == null)
        {
            MelonLogger.Error($"Uhh couldnt find a component saver for type {componentType}");
            return;
        }
        componentSaver.RestoreComponentState(referenceEntity.transform, componentHash, savedState); // i dont like this, generics pmo
    }
}
