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
                RestoreActiveState(Campaign.Session.saveData.LoadedSavePoint);
            }
        }

        public void RestoreActiveState(CampaignSaveData.SavePoint savePoint)
        {
            gameObject.SetActive(savePoint.GetEnabledStateFromName(gameObject.name, gameObject.activeSelf));
        }
    }
}