using UnityEngine;
using MelonLoader;
using System;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class SavePoint : MonoBehaviour
    {
        public SavePoint(IntPtr ptr) : base(ptr) { }

        public void Save()
        {
            string barcode = SceneStreamer.Session.Level.Barcode.ID;
            SavepointFunctions.SavePlayer(barcode, transform.position, transform.position);
        }
    }
}