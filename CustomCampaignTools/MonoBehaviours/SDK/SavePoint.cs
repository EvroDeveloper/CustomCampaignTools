using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow;
using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

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

                Collider[] trackers = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, (int)BoneLib.GameLayers.ENTITY_TRACKER, QueryTriggerInteraction.Collide);
                foreach (Collider tracker in trackers)
                {
                    if(tracker.TryGetComponent(out Tracker t))
                    {
                        boxVolEntities.Add(t.Entity);
                    }
                }

                List<CampaignSaveData.BarcodePosRot> Entities = new List<CampaignSaveData.BarcodePosRot>();
                foreach(MarrowEntity entity in boxVolEntities)
                {
                    Entities.Add(new CampaignSaveData.BarcodePosRot(entity._poolee.SpawnableCrate.Barcode, entity.transform.position, entity.transform.rotation, entity.transform.lossyScale));
                }
                Campaign.Session.saveData.SavePlayer(barcode, transform.position, Entities);
            }
            else {
                Campaign.Session.saveData.SavePlayer(barcode, transform.position);
            }
        }
    }
}