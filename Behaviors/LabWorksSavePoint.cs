#if MELONLOADER
using MelonLoader;
using SLZ.Marrow.SceneStreaming;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace Labworks.Behaviors
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class LabWorksSavePoint : MonoBehaviour
    {
#if MELONLOADER
        public LabWorksSavePoint(IntPtr ptr) : base(ptr) { }
#endif

        void Start()
        {
#if MELONLOADER
            transform.GetChild(0).gameObject.SetActive(true);
#endif
        }

        public void ActivateSave()
        {
#if MELONLOADER
            string barcode = SceneStreamer.Session.Level.Barcode;

            SavepointFunctions.SavePlayer(barcode, transform.position);
#endif
        }
    }
}