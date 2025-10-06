#if MELONLOADER
using MelonLoader;
#endif
using System;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Saving/GameObject Enabled Saver")]
#endif
    public class ObjectEnabledSaver : MonoBehaviour
    {
#if MELONLOADER
        public ObjectEnabledSaver(IntPtr ptr) : base(ptr) { }

        public void Awake()
        {
            if (SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                RestoreActiveState(Campaign.Session.saveData.LoadedSavePoint);
            }
        }

        public void RestoreActiveState(CampaignSaveData.SavePoint savePoint)
        {
            gameObject.SetActive(savePoint.GetEnabledStateFromName(gameObject.name, gameObject.activeSelf));
        }
#endif
    }
}