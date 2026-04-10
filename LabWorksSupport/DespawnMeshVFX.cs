#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Utilities;
using MelonLoader;
#else
using SLZ.Marrow.Pool;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LabWorksSupport
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class DespawnMeshVFX : MonoBehaviour
    {
        private static ComponentCache<DespawnMeshVFX> _cache;
        public static ComponentCache<DespawnMeshVFX> Cache
        {
            get
            {
                _cache ??= new ComponentCache<DespawnMeshVFX>();
                return _cache;
            }
        }
#if MELONLOADER
        public DespawnMeshVFX(IntPtr ptr) : base(ptr) { }
        public Il2CppReferenceField<Poolee> poolee;
        public Il2CppReferenceArray<MeshRenderer> meshRenderers;
        public Il2CppReferenceArray<SkinnedMeshRenderer> skinnedMeshRenderers;
        public Il2CppReferenceField<Material> despawnMaterial;
        public Il2CppReferenceField<AudioClip> despawnSfx;
#else
        public Poolee poolee;
        public MeshRenderer[] meshRenderers;
        public SkinnedMeshRenderer[] skinnedMeshRenderers;
        public Material despawnMaterial;
        public AudioClip despawnSfx;
#endif

        public void OnEnable()
        {
            Cache.Add(gameObject, this);
        }

        public void OnDisable()
        {
            Cache.Remove(gameObject, this);
        }

        public void Despawn()
        {
            Despawn(Color.red);
        }

        public void Despawn(Color color)
        {
#if MELONLOADER
            // Clone all Mesh Renderers and SkinnedMeshRenderers to another thingas
            DespawnMeshes(meshRenderers, skinnedMeshRenderers, despawnMaterial, color, poolee.Get(), despawnSfx.Get());
#endif
        }

#if MELONLOADER
        public static void DespawnMeshes(MeshRenderer[] meshes, SkinnedMeshRenderer[] skinnedMeshes, Material matthew, Color color, Poolee poolee, AudioClip despawnSfx = null)
        {
            foreach(MeshRenderer renderer in meshes)
            {
                GameObject despawnMesh = new GameObject("Despawn Mesh - " + renderer.gameObject.name);
                DespawnMesh despawnComponent = despawnMesh.AddComponent<DespawnMesh>();
                despawnComponent.color = color;
                despawnComponent.CloneMeshRenderer(renderer, matthew);
                despawnComponent.StartDespawn();
            }
            foreach(SkinnedMeshRenderer renderer in skinnedMeshes)
            {
                GameObject despawnMesh = new GameObject("Despawn Mesh - " + renderer.gameObject.name);
                DespawnMesh despawnComponent = despawnMesh.AddComponent<DespawnMesh>();
                despawnComponent.color = color;
                despawnComponent.CloneSkinnedMeshRenderer(renderer, matthew);
                despawnComponent.StartDespawn();
            }

            if(despawnSfx != null)
            {
                Audio3dManager.PlayAtPoint(despawnSfx, poolee.transform.position, Audio3dManager.hardInteraction);
            }
            else
            {
                MarrowGame.marrowSettings.DespawnSFX.LoadAsset(new Action<AudioClip>((sfx) =>
                {
                    Audio3dManager.PlayAtPoint(sfx, poolee.transform.position, Audio3dManager.hardInteraction);
                }));
            }
            poolee.Despawn();
        }
#endif
    }
}
