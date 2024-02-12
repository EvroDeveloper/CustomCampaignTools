
using MelonLoader;
using SLZ.Marrow.SceneStreaming;
using System;
using System.Collections;
using UnityEngine;
using UltEvents;
using System.Collections.Generic;
using SLZ.Marrow.Pool;

namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class LabWorksSavePoint : MonoBehaviour
    {
        public LabWorksSavePoint(IntPtr ptr) : base(ptr) { }

        public List<GameObject> CurrentEnteredObjects = new List<GameObject>();

        public BoxCollider ItemBox;

        void Start()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void ActivateSave()
        {
            string barcode = SceneStreamer.Session.Level.Barcode;

            List<string> gatheredBarcodes = new List<string>();

            RaycastHit[] hits = Physics.BoxCastAll(ItemBox.center, ItemBox.size / 2, Vector3.up, Quaternion.identity);
            foreach (RaycastHit potentialObject in hits)
            {
                gatheredBarcodes.Add(potentialObject.collider.transform.GetComponentInParent<AssetPoolee>().spawnableCrate.Barcode.ID);
            }

            // do something with the spawnable crates
            SavepointFunctions.SavePlayer(barcode, transform.position, gatheredBarcodes);
        }
    }
}