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
                HashSet<MarrowEntity> boxVolEntities = [];

                Collider[] trackers = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, (int)BoneLib.GameLayers.ENTITY_TRACKER, QueryTriggerInteraction.Collide);
                foreach (Collider tracker in trackers)
                {
                    if(tracker.TryGetComponent(out Tracker t))
                    {
                        if(t.Entity == null) continue;
                        try
                        {
                            if (t.Entity._poolee.SpawnableCrate.Barcode.ID == "SLZ.BONELAB.Core.Spawnable.RigManagerBlank") continue;
                        }
                        catch { }

                        boxVolEntities.Add(t.Entity);
                    }
                }

                List<CampaignSaveData.BarcodePosRot> Entities = new List<CampaignSaveData.BarcodePosRot>();
                foreach(MarrowEntity entity in boxVolEntities)
                {
                    if(entity._poolee == null) continue;
                    if(entity._poolee.SpawnableCrate == null) continue;
                    Entities.Add(new CampaignSaveData.BarcodePosRot(entity._poolee.SpawnableCrate.Barcode, entity.transform.position, entity.transform.rotation, entity.transform.lossyScale));
                }
                Campaign.Session.saveData.SavePlayer(barcode, transform.position, Entities);
            }
            else {
                Campaign.Session.saveData.SavePlayer(barcode, transform.position);
            }
        }

        public void ClearSave()
        {
            Campaign.Session.saveData.ClearSavePoint();
        }
    }
}