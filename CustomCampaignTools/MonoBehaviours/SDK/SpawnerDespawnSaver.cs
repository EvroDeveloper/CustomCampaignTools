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

        public bool LoadedFromSave = false;

        private GameObject _objectToSave;
        private bool saveDontSpawn;

        public void Awake()
        {
            if (SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                LoadedFromSave = true;
            }
        }

        public bool DontSpawnAgain(out string id)
        {
            id = GetCrateID();
            return saveDontSpawn;
        }

        public string GetCrateID()
        {
            return transform.position.ToString("F2");
        }
#endif
        public void Setup(CrateSpawner c, GameObject g)
        {
#if MELONLOADER
            _objectToSave = g;
            if (g.TryGetComponent(out Poolee p))
            {
                var despawnHook = g.AddComponent<CrateDespawnerHook>();
                despawnHook.OnDespawnDelegate += new Action<GameObject>((g) => { saveDontSpawn = true; });
            }

            // From here down WORKS
            var brain = g.GetComponentInChildren<AIBrain>();
            if (brain)
            {
                brain.onDeathDelegate += new Action<AIBrain>((g) => saveDontSpawn = true);
            }
            if (LoadedFromSave && Campaign.Session.saveData.LoadedSavePoint.DespawnedSpawners.Contains(GetCrateID()))
            {
                if (g.TryGetComponent(out Poolee p2))
                {
                    p2.Despawn();
                }
            }
#endif
        }

#if UNITY_EDITOR
        void Reset()
        {
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