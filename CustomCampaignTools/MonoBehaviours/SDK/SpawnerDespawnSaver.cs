using System;
using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.Marrow.AI;
using Il2CppSLZ.Marrow.Pool;

namespace CustomCampaignTools.SDK
{
    // Require CrateSpawner
    [RegisterTypeInIl2Cpp]
    public class SpawnerDespawnSaver : MonoBehaviour
    {
        public SpawnerDespawnSaver(IntPtr ptr) : base(ptr) { }

        public bool LoadedFromSave = false;

        private GameObject _objectToSave;
        private bool saveDontSpawn;

        public void Awake()
        {
            if(SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                LoadedFromSave = true;
            }
        }

        public void Setup(CrateSpawner c, GameObject g)
        {
            _objectToSave = g;
            if(g.TryGetComponent(out Poolee p))
            {
                var despawnHook = g.AddComponent<CrateDespawnerHook>();
                despawnHook.OnDespawnDelegate += new Action<GameObject>((g) => { saveDontSpawn = true; });
            }

            // From here down WORKS
            var brain = g.GetComponentInChildren<AIBrain>();
            if(brain)
            {
                brain.onDeathDelegate += new Action<AIBrain>((g) => saveDontSpawn = true);
            }
            if(LoadedFromSave && Campaign.Session.saveData.LoadedSavePoint.DespawnedSpawners.Contains(GetCrateID()))
            {
                if(g.TryGetComponent(out Poolee p2))
                {
                    p2.Despawn();
                }
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
    }
}