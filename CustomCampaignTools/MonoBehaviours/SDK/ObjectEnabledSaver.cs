using MelonLoader;
using System;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class ObjectEnabledSaver : MonoBehaviour
    {
        public ObjectEnabledSaver(IntPtr ptr) : base(ptr) { }

        public void Awake()
        {
            if(SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                RestoreActiveState();
            }
        }

        public void RestoreActiveState()
        {
            gameObject.SetActive(Campaign.Session.saveData.LoadedSavePoint.GetEnabledStateFromName(gameObject.name, gameObject.activeSelf));
        }
    }
}