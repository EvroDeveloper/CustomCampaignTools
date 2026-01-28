using System;
using BoneLib;
using CustomCampaignTools.Data.SimpleSerializables;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;

namespace CustomCampaignTools.Data;

public class SceneEntityData
{
    // Level Entities - Hash from hierarchy and Name
    // Spawned Entities - Save barcode position and name
    public Dictionary<int, MarrowEntitySaveData> levelEntitySaves = [];
    public List<SpawnedEntitySave> spawnedEntitySaves = [];

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
            int hashCode = GetHierarchyHash(entity.transform);

            MarrowEntitySaveData entitySave = new(entity);
            levelEntitySaves.Add(hashCode, entitySave);
        }
        else
        {
            // From spawn entity, save.
            // Dont save spawned things that have been subsequently despawned.
            // Also probably not good to save spawnables that are under the asset spawner, only save root level spawned entities
            if(!entity.IsDespawned && entity.transform.parent == null)
            {
                spawnedEntitySaves.Add(new(entity));
            }
        }
    }


    public void RestoreLevelEntity(MarrowEntity entity)
    {
        int hashCode = GetHierarchyHash(entity.transform);
        if(levelEntitySaves.ContainsKey(hashCode))
        {
            levelEntitySaves[hashCode].ApplyToEntity(entity);
        }
    }

    public static int GetHierarchyHash(Transform transform)
    {
        HashCode hash2 = new();

        Transform checkingTransform = transform;
        while(checkingTransform != null)
        {
            hash2.Add(checkingTransform.gameObject.name);
            if(checkingTransform.parent != null) // I got differing hashes and I think its because of the jumbled nature of the root objects in the scene. Only check sibling index on non-root objects
                hash2.Add(checkingTransform.GetSiblingIndex());
            checkingTransform = checkingTransform.parent;
        }

        return hash2.ToHashCode();
    }
}

public class SpawnedEntitySave
{
    public BarcodeSer barcode;
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

                if(callback != null)
                    callback(entity);
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


    public MarrowEntitySaveData(MarrowEntity entity)
    {
        name = entity.gameObject.name;
        
        entityPosition = new(entity.transform, false);

        MarrowEntityPose readPose = new();
        entity.ReadPose(ref readPose);

        entityPose = new(readPose);

        isDespawned = entity.IsDespawned;
        if(!entity.IsDespawned)
        {
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
        }
    }

    public void ApplyToEntity(MarrowEntity entity)
    {
        entity.transform.position = entityPosition.position.ToVector3();
        entity.transform.rotation = entityPosition.rotation.ToQuaternion();

        entity.WritePose(entityPose.ToMarrowEntityPose());

        if(isDespawned)
        {
            entity.Despawn();
        }
        else if(bodyVelocities != null && bodyVelocities.Length != 0 && bodyVelocities.Length == entity.Bodies.Length)
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
    }
}
