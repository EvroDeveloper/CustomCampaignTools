#if MELONLOADER
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Pool;
#else
using SLZ.Marrow.Warehouse;
using UltEvents;
#endif
using System;
using UnityEngine;
using Il2CppInterop.Runtime.InteropTypes.Fields;
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
#endif
    public class SpawnerDespawnSaver : MonoBehaviour
    {
#if MELONLOADER
        public SpawnerDespawnSaver(IntPtr ptr) : base(ptr) { }

        public Il2CppValueField<int> uniqueID;

        public bool LoadedFromSave = false;

        private GameObject _objectToSave;
        public bool hasBeenDespawned;

        public void Awake()
        {
            if (SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                LoadedFromSave = true;
            }
        }
#else
        [Tooltip("A unique ID for this object. Used to identify it in save data. A random ID will be assigned on Reset().")]
        public int uniqueID;
#endif
        public void Setup(CrateSpawner c, GameObject g)
        {
#if MELONLOADER
            _objectToSave = g;
            if (g.TryGetComponent(out Poolee p))
            {
                var despawnHook = g.AddComponent<CrateDespawnerHook>();
                despawnHook.OnDespawnDelegate += new Action<GameObject>((g) => { hasBeenDespawned = true; });
            }

            // From here down WORKS
            var brain = g.GetComponentInChildren<AIBrain>();
            if (brain)
            {
                brain.onDeathDelegate += new Action<AIBrain>((g) => hasBeenDespawned = true);
            }
            if (LoadedFromSave && Campaign.Session.saveData.LoadedSavePoint.DespawnedSpawners.Contains(uniqueID.Get()))
            {
                if (g.TryGetComponent(out Poolee p2))
                {
                    p2.Despawn();
                }
            }
#endif
        }

#if UNITY_EDITOR
        public void Reset()
        {
            uniqueID = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            
            CrateSpawner spawner = GetComponent<CrateSpawner>();
            var call = spawner.onSpawnEvent.AddPersistentCall((Action<CrateSpawner, GameObject>)Setup);

            typeof(PersistentArgument).GetField("_Type", UltEventUtils.AnyAccessBindings).SetValue(call.PersistentArguments[0], PersistentArgumentType.Parameter);
            call.PersistentArguments[0].ParameterIndex = 0;
            typeof(PersistentArgument).GetField("_Type", UltEventUtils.AnyAccessBindings).SetValue(call.PersistentArguments[1], PersistentArgumentType.Parameter);
            call.PersistentArguments[1].ParameterIndex = 1;
        }
#endif
    }
}