
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.SceneStreaming;

namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class LabWorksSavePoint : MonoBehaviour
    {
        public LabWorksSavePoint(IntPtr ptr) : base(ptr) { }

        public List<string> CurrentEnteredObjects = new List<string>();
        public GameObject ItemBox;

        void Start()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void OnEntityEnter(MarrowEntity entity)
        {
            if(entity._poolee.SpawnableCrate != null)
            {
                CurrentEnteredObjects.Add(entity._poolee.SpawnableCrate.Barcode.ID);
            }
        }

        public void OnEntityExit(MarrowEntity entity)
        {
            if(entity._poolee.SpawnableCrate != null)
            {
                CurrentEnteredObjects.Remove(entity._poolee.SpawnableCrate.Barcode.ID);
            }
        }

        public void ActivateSave()
        {
            string barcode = SceneStreamer.Session.Level.Barcode.ID;

            SavepointFunctions.SavePlayer(barcode, transform.position, ItemBox.transform.position, CurrentEnteredObjects);
        }
    }
}