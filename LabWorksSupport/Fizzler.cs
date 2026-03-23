#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Marrow.Interaction;
using MelonLoader;

#else
#endif
using System;
using System.Collections;
using UnityEngine;

namespace LabWorksSupport
{
    public class BoneworksFizzler : MonoBehaviour
    {
#if MELONLOADER
        public Il2CppReferenceField<Material> despawnMaterial;
#else
        public Material despawnMaterial;
#endif
        private HashSet<MarrowEntity> claimedEntities;

        void Awake()
        {
            claimedEntities = new HashSet<MarrowEntity>();
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.attachedRigidbody == null) return;
            if(Tracker.Cache.TryGet(other.gameObject, out Tracker t))
            {
                if(claimedEntities.Contains(t.Entity)) return;
                claimedEntities.Add(t.Entity);

                MelonCoroutines.Start(CoDelayDespawn(t.Entity));
            }
        }

        IEnumerator CoDelayDespawn(MarrowEntity entity)
        {
            yield return new WaitForSeconds(0.025f);

            // Clone all Mesh Renderers and SkinnedMeshRenderers to another thingas
            GameObject despawnVfxParent = new GameObject("Boneworks Despawn VFX - " + entity.gameObject.name);
            MeshRenderer[] entityMeshes = entity.GetComponentsInChildren<MeshRenderer>();
            SkinnedMeshRenderer[] entitySkinnedMeshes = entity.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach(MeshRenderer renderer in entityMeshes)
            {
                CloneMeshRenderer(renderer, despawnVfxParent.transform);
            }
            foreach(SkinnedMeshRenderer renderer in entitySkinnedMeshes)
            {
                CloneSkinnedMeshRenderer(renderer, despawnVfxParent.transform);
            }

            entity._poolee.Despawn();

            yield return new WaitForSeconds(2f);

            claimedEntities.Remove(entity);
            Destroy(despawnVfxParent);
        }

        void CloneMeshRenderer(MeshRenderer renderer, Transform parent)
        {
            GameObject clonedGobj = Instantiate(renderer.gameObject, parent);
            clonedGobj.name = "DespawnMesh - " + renderer.gameObject.name;
            clonedGobj.transform.SetPositionAndRotation(renderer.transform.position, renderer.transform.rotation);
            clonedGobj.transform.localScale = renderer.transform.lossyScale;

            DeleteChildren(clonedGobj);

            MeshRenderer clonedRenderer = clonedGobj.GetComponent<MeshRenderer>();

            var clonedComponents = clonedGobj.GetComponents<Component>();

            foreach(Component c in clonedComponents)
            {
                if(c is Transform) continue;
                if(c is MeshRenderer) continue;
                if(c is MeshFilter) continue;

                Destroy(c);
            }

            FillAllMaterials(clonedRenderer, despawnMaterial.Get());
        }

        void CloneSkinnedMeshRenderer(SkinnedMeshRenderer renderer, Transform parent)
        {
            GameObject clonedGobj = Instantiate(renderer.gameObject, parent);
            clonedGobj.name = "DespawnSkinnedMesh - " + renderer.gameObject.name;
            clonedGobj.transform.SetPositionAndRotation(renderer.transform.position, renderer.transform.rotation);
            clonedGobj.transform.localScale = renderer.transform.lossyScale;

            DeleteChildren(clonedGobj);

            SkinnedMeshRenderer clonedRenderer = clonedGobj.GetComponent<SkinnedMeshRenderer>();

            for (int i = 0; i < renderer.bones.Count; i++)
            {
                Transform bone = renderer.bones[i];
                GameObject clonedBone = new GameObject("ClonedBone - " + bone.gameObject.name);
                clonedBone.transform.SetParent(clonedGobj.transform);
                clonedBone.transform.SetPositionAndRotation(bone.position, bone.rotation);

                clonedRenderer.bones[i] = clonedBone.transform;
            }

            var clonedComponents = clonedGobj.GetComponents<Component>();
            foreach(Component c in clonedComponents)
            {
                if(c is Transform) continue;
                if(c is SkinnedMeshRenderer) continue;

                Destroy(c);
            }

            FillAllMaterials(clonedRenderer, despawnMaterial.Get());
        }

        void DeleteChildren(GameObject gobj)
        {
            for(int i = gobj.transform.childCount; i > 0; i--)
            {
                Destroy(gobj.transform.GetChild(i - 1));
            }
        }

        void FillAllMaterials(Renderer renderer, Material targetmaterial)
        {
            for(int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                renderer.sharedMaterials[i] = targetmaterial;
            }
        }
    }
}
