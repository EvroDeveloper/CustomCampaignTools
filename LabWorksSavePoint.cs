using MelonLoader;
using SLZ.Marrow.SceneStreaming;
using System;
using System.Collections;
using UnityEngine;

namespace Labworks_Ammo_Saver
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

            int levelIndex = AmmoFunctions.levelBarcodes.IndexOf(barcode);

            SavepointFunctions.SavePlayer(levelIndex, transform.position);
        }
    }
}