
using MelonLoader;
using SLZ.Marrow.SceneStreaming;
using System;
using System.Collections;
using UnityEngine;
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
            //ItemBox = transform.GetChild(1).GetComponent<BoxCollider>();
            ItemBox = transform.parent.Find("ItemCollector").GetComponent<BoxCollider>();
        }

        public void ActivateSave()
        {
            string barcode = SceneStreamer.Session.Level.Barcode;

            List<string> gatheredBarcodes = new List<string>();

            //TODO: Uncomment Later once build is done
            RaycastHit[] hits = Physics.BoxCastAll(ItemBox.center, ItemBox.size / 2, Vector3.up, Quaternion.identity, 1f);
            foreach (RaycastHit potentialObject in hits)
            {
                if(HasAssetPoolee(potentialObject.collider, out var poolee))
                    gatheredBarcodes.Add(poolee.spawnableCrate.Barcode.ID);
            }

            // do something with the spawnable crates
            SavepointFunctions.SavePlayer(barcode, transform.position, ItemBox.transform.position, gatheredBarcodes);
        }

        private bool HasAssetPoolee(Collider collider, out AssetPoolee poolee)
        {
            if (collider.transform.GetComponentInParent<AssetPoolee>() != null)
            {
                poolee = collider.transform.GetComponentInParent<AssetPoolee>();
                return true;
            }
            poolee = null;
            return false;
        }
    }
}