using System;
using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;

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
                MelonLogger.Msg($"Found Poolee {g.name}, hooking action into Delegates");
                p.OnDespawnDelegate += new Action<GameObject>((g) => { MelonLogger.Msg($"Poolee {g.name} was Despawned"); saveDontSpawn = true; } );
                p.OnRecycleDelegate += new Action<GameObject>((g) => { MelonLogger.Msg($"Poolee {g.name} was Recycled"); saveDontSpawn = true; } );
            }

            // From here down WORKS
            var brain = g.GetComponentInChildren<AIBrain>()
            if(brain)
            {
                brain.onDeathDelegate += new Action<GameObject>((g) => saveDontSpawn = true);
            }
            if(LoadedFromSave && Campaign.Session.saveData.LoadedSavePoint.DontSpawnAgain.Contains(GetCrateID()))
            {
                if(g.TryGetComponent(out Poolee p))
                {
                    p.Despawn();
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
            return position.ToString("F2");
        }
    }
}