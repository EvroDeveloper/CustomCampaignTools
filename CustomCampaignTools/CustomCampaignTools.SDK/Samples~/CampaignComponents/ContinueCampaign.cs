#if MELONLOADER
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
using CustomCampaignTools.Utilities;
#endif
using UnityEngine;
using System;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Saving/Continue Campaign")]
#endif
    public class ContinueCampaign : MonoBehaviour
    {
#if MELONLOADER
        public ContinueCampaign(IntPtr ptr) : base(ptr) { }
#endif
        public void Continue()
        {
#if MELONLOADER
            if (!Campaign.Session.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;
            
            Campaign.Session.saveData.LoadedSavePoint.LoadContinue(Campaign.Session.LoadScene);
#endif
        }

        public void EnableIfValidSave(GameObject obj)
        {
#if MELONLOADER
            if (Campaign.Session.saveData.LoadedSavePoint.IsValid(out _)) obj.SetActive(true);
#endif
        }
    }
}