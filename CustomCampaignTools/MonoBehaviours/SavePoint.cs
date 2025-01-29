using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class SavePoint : MonoBehaviour
    {
        public SavePoint(IntPtr ptr) : base(ptr) { }

        public void Save()
        {
            string barcode = SceneStreamer.Session.Level.Barcode.ID;

            if(TryGetComponent(out BoxCollider collider))
            {
                HashSet<MarrowEntity> boxVolEntities = new HashSet<MarrowEntity>();

                Collider[] trackers = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, LayerMask.GetMask("EntityTracker"));
                foreach (Collider tracker in trackers)
                {
                    if(tracker.TryGetComponent(out Tracker t))
                    {
                        boxVolEntities.Add(t.entity);
                    }
                }

                List<string> entityBarcodes = new List<string>();
                foreach(MarrowEntity entity in boxVolEntities)
                {
                    entityBarcodes.Add(entity.Poolee.Spawnable.Barcode);
                }
                SavepointFunctions.SavePlayer(barcode, transform.position, collider.bounds.center, entityBarcodes)
            }
            else {
                SavepointFunctions.SavePlayer(barcode, transform.position, transform.position);
            }
        }
    }
}