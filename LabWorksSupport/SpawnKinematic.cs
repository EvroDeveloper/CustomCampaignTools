#if MELONLOADER
using MelonLoader;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Warehouse;
#else
using SLZ.Marrow.Interaction;
using SLZ.Marrow.Warehouse;
using SLZ.Marrow.Utilities;
#endif
using UnityEngine;
using System.Collections.Generic;
using System;

namespace LabWorksSupport
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class SpawnKinematic : MonoBehaviour
    {
#if MELONLOADER
        public SpawnKinematic(IntPtr ptr) : base(ptr) { }
        
        public Il2CppReferenceField<CrateSpawner> crateSpawner;
        public Il2CppValueField<bool> startKinematic;

        private MarrowEntity spawnedEntity;
        private List<MarrowEntity> allSpawnedEntities = [];
#else

        [ReadOnly]
        public CrateSpawner crateSpawner;
        public bool startKinematic = true;
#endif

#if MELONLOADER
        public void Awake()
        {
            crateSpawner.Get().onSpawnEvent._DynamicCalls += (Il2CppSystem.Action<CrateSpawner, GameObject>)OnSpawn;
        }

        public void OnSpawn(CrateSpawner spawner, GameObject gobj)
        {
            MarrowEntity entity = MarrowEntity.Cache.Get(gobj);
            if (entity == null) return;
            spawnedEntity = entity;
            foreach(MarrowBody body in entity.Bodies)
            {
                body._defaultRigidbodyInfo.isKinematic = startKinematic;
                body._rigidbody.isKinematic = startKinematic;
            }
        }
#endif

        public void SetKinematic(bool isKinematic)
        {
#if MELONLOADER
            if(spawnedEntity == null) return;
            foreach(MarrowBody body in spawnedEntity.Bodies)
            {
                body._defaultRigidbodyInfo.isKinematic = startKinematic;
                body._rigidbody.isKinematic = startKinematic;
            }
#endif
        }

        private void Reset()
        {
            TryGetComponent(out crateSpawner);
        }
    }
}