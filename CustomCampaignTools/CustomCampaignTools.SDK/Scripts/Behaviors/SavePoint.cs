#if MELONLOADER
using MelonLoader;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Pool;
using Il2CppInterop.Runtime.InteropTypes.Fields;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Saving/Save Point")]
#endif
    public class SavePoint : MonoBehaviour
    {
#if MELONLOADER
        public SavePoint(IntPtr ptr) : base(ptr) { }

        public Il2CppReferenceField<Transform> playerSpawnPoint;
        public Il2CppReferenceField<BoxCollider> entitySaveZone;

        void Awake()
        {
            if(entitySaveZone.Get() == null && TryGetComponent(out BoxCollider collider))
            {
                entitySaveZone.Set(collider);
            }
            if (playerSpawnPoint.Get() == null)
            {
                playerSpawnPoint.Set(transform);
            }
        }
#else
        [Tooltip("If set, the player will spawn at this transform when loading from this save point. If not set, the player will spawn at this object's transform.")]
        public Transform playerSpawnPoint;

        [Tooltip("If set, all entities within this box collider will be saved and restored on load.")]
        public BoxCollider entitySaveZone;
#endif
        public void Save()
        {
#if MELONLOADER
            Barcode barcode = SceneStreamer.Session.Level.Barcode;

            if (entitySaveZone.Get() != null)
            {
                HashSet<MarrowEntity> boxVolEntities = [];
                HashSet<Poolee> pooleeSet = []; // Prevent double saving of separate entities referencing the same poolee. Ex - Gun and Mag both saving as the Gun

                BoxCollider collider = entitySaveZone.Get();

                Collider[] trackers = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, (int)BoneLib.GameLayers.ENTITY_TRACKER, QueryTriggerInteraction.Collide);
                foreach (Collider tracker in trackers)
                {
                    if (tracker.TryGetComponent(out Tracker t))
                    {
                        if (t.Entity == null) continue;
                        if (t.Entity._poolee == null) continue;
                        if (t.Entity._poolee.SpawnableCrate == null) continue;

                        try
                        {
                            if(pooleeSet.Contains(t.Entity._poolee)) continue;
                            pooleeSet.Add(t.Entity._poolee);

                            if (t.Entity._poolee.SpawnableCrate.Barcode.ID == MarrowGame.marrowSettings.DefaultPlayerRig.Barcode.ID) continue;
                            boxVolEntities.Add(t.Entity);
                        }
                        catch { }
                    }
                }

                List<CampaignSaveData.BarcodePosRot> Entities = [];
                foreach (MarrowEntity entity in boxVolEntities)
                {
                    if (entity._poolee == null) continue;
                    if (entity._poolee.SpawnableCrate == null) continue;
                    Entities.Add(new CampaignSaveData.BarcodePosRot(entity._poolee.SpawnableCrate.Barcode, entity.transform.position, entity.transform.rotation, entity.transform.lossyScale));
                }
                Campaign.Session.saveData.SavePlayer(barcode, new SimpleTransform(playerSpawnPoint.Get()), Entities);
            }
            else
            {
                Campaign.Session.saveData.SavePlayer(barcode, new SimpleTransform(playerSpawnPoint.Get()));
            }
#endif
        }

        public void ClearSave()
        {
#if MELONLOADER
            Campaign.Session.saveData.ClearSavePoint();
#endif
        }
    }
}