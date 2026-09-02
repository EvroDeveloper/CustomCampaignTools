#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Fields;
using MelonLoader;
#endif
using System;
using UnityEngine;

namespace LabWorksSupport
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class DespawnVFXSetup : MonoBehaviour
    {
#if MELONLOADER
        public DespawnVFXSetup(IntPtr ptr) : base(ptr) {}
        public Il2CppReferenceField<Material> despawnMaterial;
        public Il2CppValueField<Color> despawnColor;
#else
        public Material despawnMaterial;
        public Color despawnColor;
#endif

#if MELONLOADER
        public void Awake()
        {
            if(DespawnMeshVFX.DespawnMaterial == null)
            {
                DespawnMeshVFX.DespawnMaterial = despawnMaterial.Get();
                DespawnMeshVFX.DespawnMaterial.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }

            DespawnMeshVFX.DespawnColor = despawnColor.Get();
        }
#endif
    }
}