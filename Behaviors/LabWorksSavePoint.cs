
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

        public List<string> CurrentEnteredObjects = new List<string>();

        void Start()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void OnEntityEnter(MarrowEntity entity)
        {
            if(entity.poolee.SpawnableCrate != null)
            {
                CurrentEnteredObjects.Add(entity.poolee.SpawnableCrate.Barcode.ID);
            }
        }

        public void OnEntityExit(MarrowEntity entity)
        {
            if(entity.poolee.SpawnableCrate != null)
            {
                CurrentEnteredObjects.Remove(entity.poolee.SpawnableCrate.Barcode.ID);
            }
        }

        public void ActivateSave()
        {
            string barcode = SceneStreamer.Session.Level.Barcode;

            SavepointFunctions.SavePlayer(barcode, transform.position, ItemBox.transform.position, CurrentEnteredObjects);
        }
    }
}