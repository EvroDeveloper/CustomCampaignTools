#if MELONLOADER
using System.Collections;
using MelonLoader;
#endif
using UnityEngine;
using System;

namespace LabWorksSupport
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class DespawnMesh : MonoBehaviour
    {
        public DespawnMesh(IntPtr ptr) : base(ptr) { }
        public Color color;
        public Gradient despawnGradient;

        private MeshRenderer _renderer;
        private MeshFilter _filter;

        public void CloneMeshRenderer(MeshRenderer mr, Material matthew)
        {
            transform.position = mr.transform.position;
            transform.rotation = mr.transform.rotation;
            transform.localScale = mr.transform.lossyScale;

            SetupSelfRenderer(mr.materials.Length, matthew);

            _filter.sharedMesh = mr.GetComponent<MeshFilter>().sharedMesh;
        }

        public void CloneSkinnedMeshRenderer(SkinnedMeshRenderer mr, Material matthew)
        {
            transform.position = mr.transform.position;
            transform.rotation = mr.transform.rotation;
            transform.localScale = mr.transform.lossyScale;

            SetupSelfRenderer(mr.materials.Length, matthew);

            Mesh bakedSkinnedMesh = new Mesh();
            mr.BakeMesh(bakedSkinnedMesh);

            _filter.sharedMesh = bakedSkinnedMesh;
        }

        private void SetupSelfRenderer(int materialCount, Material matthew)
        {
            _filter ??= gameObject.AddComponent<MeshFilter>();

            _renderer ??= gameObject.AddComponent<MeshRenderer>();
            Material[] rendererMaterials = new Material[materialCount];
            for(int i = 0; i < materialCount; i++)
            {
                rendererMaterials[i] = matthew;
            }
            _renderer.sharedMaterials = rendererMaterials;
        }



        public void StartDespawn()
        {
#if MELONLOADER
            MelonCoroutines.Start(CoDespawn());
#endif
        }

        IEnumerator CoDespawn()
        {
            float time = 0f;
            float duration = 1f;
            Vector3 position = transform.position;
            bool moveToggle = false;

            while(time <= duration)
            {
                if(moveToggle)
                    transform.position = position + new Vector3(UnityEngine.Random.Range(-0.005f, 0.005f), UnityEngine.Random.Range(-0.005f, 0.005f), UnityEngine.Random.Range(-0.005f, 0.005f));
                
                moveToggle = !moveToggle; // Guh? Only move every other frame i suppose

                for(int i = 0; i < _renderer.materials.Length; i++)
                {
                    _renderer.materials[i].SetFloat("_Dissolve", time / duration);
                    _renderer.materials[i].SetColor("_DespawnColor", color);
                }

                time += Time.unscaledDeltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
