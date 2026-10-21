#if MELONLOADER
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Pool;
using Il2CppInterop.Runtime.InteropTypes.Fields;
#else
using SLZ.Marrow.Warehouse;
using SLZ.Marrow.Utilities;
using UltEvents;
#endif
using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Saving/Spawner Despawn Saver")]
    [RequireComponent(typeof(CrateSpawner))]
    [ExecuteInEditMode]
#endif
    public class SpawnerDespawnSaver : MonoBehaviour
    {
#if MELONLOADER
        public SpawnerDespawnSaver(IntPtr ptr) : base(ptr) { }

        public Il2CppReferenceField<CrateSpawner> crateSpawner;
        public CrateSpawner CrateSpawner { get => crateSpawner.Get(); set => crateSpawner.Set(value); }
        public Il2CppValueField<int> uniqueID;

        private bool _loadedFromSave = false;
        private GameObject _objectToSave;
        public bool hasBeenDespawned;

        public void Awake()
        {
            if (SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                _loadedFromSave = true;
            }

            if(CrateSpawner == null && TryGetComponent(out CrateSpawner cs))
                CrateSpawner = cs;

            if(CrateSpawner != null)
                CrateSpawner.onSpawnEvent._DynamicCalls += (Action<CrateSpawner, GameObject>)OnSpawn;
        }

        public void OnSpawn(CrateSpawner c, GameObject g)
        {
            _objectToSave = g;
            if (g.TryGetComponent(out Poolee p))
            {
                var despawnHook = g.AddComponent<CrateDespawnerHook>();
                despawnHook.OnDespawnDelegate += (g) => { hasBeenDespawned = true; };
            }

            var brain = g.GetComponentInChildren<AIBrain>();
            if (brain)
            {
                brain.onDeathDelegate += (Action<AIBrain>)((g) => hasBeenDespawned = true);
            }
            if (_loadedFromSave && Campaign.Session.saveData.LoadedSavePoint.DespawnedSpawners.Contains(uniqueID.Get()))
            {
                if (g.TryGetComponent(out Poolee p2))
                {
                    p2.Despawn();
                    hasBeenDespawned = true;
                }
            }
        }
#else
        [ReadOnly]
        public CrateSpawner crateSpawner;

        [Tooltip("A unique ID for this object. Used to identify it in save data. A random ID will be assigned on Reset.")]
        public int uniqueID;
#endif

        [Obsolete("Manual Setup() for Spawner Despawn Saver is obsolete")]
        public void Setup(CrateSpawner c, GameObject g)
        {
        }
        
#if UNITY_EDITOR
        public void Awake()
        {
            if(crateSpawner == null) TryGetComponent(out crateSpawner);
            if(uniqueID == 0) uniqueID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public void Reset()
        {
            TryGetComponent(out crateSpawner);
            uniqueID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
#endif
    }
}