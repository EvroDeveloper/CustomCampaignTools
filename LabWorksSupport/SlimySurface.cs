#if MELONLOADER
using MelonLoader;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using BoneLib;
#else
using SLZ.Marrow;
#endif
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace LabWorksSupport
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class SlimySurface : MonoBehaviour
    {
#if MELONLOADER
        private Collider _collider;
        private PhysicMaterial _physMat;

        void Awake()
        {
            if (TryGetComponent(out _collider))
            {
                _physMat = _collider.sharedMaterial;
            }
        }

        void Start()
        {
            // RigManager may not be spawned in when the script is started. Wait for RigManager before assigning materials.
            MelonCoroutines.Start(IWaitForRigmanager((r) =>
            {
                PhysGrounder grounder = r.physicsRig.physG;
                List<PhysicMaterial> currentSlimeMats = [.. grounder.slimeMats];

                if (!currentSlimeMats.Contains(_physMat))
                {
                    currentSlimeMats.Add(_physMat);
                }
            }));
        }

        IEnumerator IWaitForRigmanager(Action<RigManager> callback)
        {
            while (Player.RigManager == null)
            {
                yield return null;
            }

            callback.Invoke(Player.RigManager);
        }
#endif
    }
}