
using MelonLoader;
using SLZ.Marrow.SceneStreaming;
using System;
using System.Collections;
using UnityEngine;

namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class LabWorksSavePoint : MonoBehaviour
    {
        public LabWorksSavePoint(IntPtr ptr) : base(ptr) { }

        void Start()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        public void ActivateSave()
        {
            string barcode = SceneStreamer.Session.Level.Barcode;

            SavepointFunctions.SavePlayer(barcode, transform.position);
        }
    }
}