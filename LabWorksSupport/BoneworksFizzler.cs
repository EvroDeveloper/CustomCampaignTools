#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Utilities;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.Zones;
using MelonLoader;
#else
using SLZ.Bonelab;
using SLZ.Marrow.Interaction;
using SLZ.Marrow.Utilities;
using SLZ.Marrow.Warehouse;
using SLZ.Marrow.Zones;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabWorksSupport
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class BoneworksFizzler : MonoBehaviour
    {
#if MELONLOADER
        public BoneworksFizzler(IntPtr ptr) : base(ptr) { }
        public Il2CppReferenceField<Material> despawnMaterial;
        public Il2CppReferenceField<AudioClip> despawnSound;
        public Il2CppValueField<Color> despawnColor;
#else
        public Material despawnMaterial;
        public AudioClip despawnSound;
        public Color despawnColor;
#endif
        private HashSet<MarrowEntity> claimedEntities;

        void Awake()
        {
#if MELONLOADER
            claimedEntities = new HashSet<MarrowEntity>();
            if(TryGetComponent<Fizzler>(out var fizzy))
            {
                Destroy(fizzy);
            }
            if(TryGetComponent<FilteredDespawn>(out var filty))
            {
                Destroy(filty);
            }
#endif
        }

        void OnTriggerEnter(Collider other)
        {
#if MELONLOADER
            if(other.attachedRigidbody == null) return;
            if(Tracker.Cache.TryGet(other.gameObject, out Tracker t))
            {
                DespawnEntity(t.Entity);
            }
#endif
        }

        public void DespawnEntity(MarrowEntity entity)
        {
#if MELONLOADER
            foreach(BoneTagReference entityTag in entity.Tags.Tags)
            {
                if(entityTag.Barcode.ID == MarrowGame.marrowSettings.BeingTag.Barcode.ID)
                {
                    return;
                }
            }
            if(claimedEntities.Contains(entity)) return;
            claimedEntities.Add(entity);

            MelonCoroutines.Start(CoDelayDespawn(entity));
#endif
        }

#if MELONLOADER
        IEnumerator CoDelayDespawn(MarrowEntity entity)
        {
            yield return new WaitForSeconds(0.025f);

            if(entity.TryGetComponent<DespawnMeshVFX>(out var vfx))
            {
                vfx.Despawn(despawnColor);
            }
            else
            {
                // Clone all Mesh Renderers and SkinnedMeshRenderers to another thingas
                MeshRenderer[] entityMeshes = entity.GetComponentsInChildren<MeshRenderer>();
                SkinnedMeshRenderer[] entitySkinnedMeshes = entity.GetComponentsInChildren<SkinnedMeshRenderer>();

                DespawnMeshVFX.DespawnMeshes(entityMeshes, entitySkinnedMeshes, despawnMaterial, despawnColor, entity._poolee, despawnSound);
            }

            yield return new WaitForSeconds(1f);

            claimedEntities.Remove(entity);
        }
#endif
    }
}
