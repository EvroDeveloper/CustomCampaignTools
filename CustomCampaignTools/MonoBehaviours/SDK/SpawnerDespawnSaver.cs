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
            // if loaded from save, and save data.collectedAmmos contains this CrateID
            // DESTROY IT :fire emoji:
            if(LoadedFromSave && Campaign.Session.saveData.LoadedSavePoint.DontSpawnAgain.Contains(GetCrateID()))
            {
                if(g.TryGetComponent(out Poolee p))
                {
                    p.Despawn();
                }
            }
            else
            {
                _objectToSave = g;
                if(g.TryGetComponent(out Poolee p))
                {
                    p.OnDespawnDelegate += OnDespawned;
                }

                var brain = g.GetComponentInChildren<AIBrain>()
                if(brain)
                {
                    brain.OnDeathDelegate += OnDeathPerchance;
                }
            }
        }

        public void OnDespawned(GameObject g)
        {
            saveDontSpawn = true;
        }

        public void OnDeathPerchance(AIBrain b)
        {
            saveDontSpawn = true;
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