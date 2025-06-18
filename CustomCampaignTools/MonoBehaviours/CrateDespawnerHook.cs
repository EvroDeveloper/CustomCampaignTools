using Il2CppSLZ.Marrow.Interaction;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools
{
    [RegisterTypeInIl2Cpp]
    public class CrateDespawnerHook : MonoBehaviour
    {
        public CrateDespawnerHook(IntPtr ptr) : base(ptr) { }

        public Action<GameObject> OnDespawnDelegate { get; set; }

        private void OnDisable()
        {
            var ent = gameObject.GetComponentInChildren<MarrowEntity>();
            if (!ent) return;
            if (!ent.IsDespawned) return;
            OnDespawnDelegate.Invoke(gameObject);
            Destroy(this);
        }
    }
}