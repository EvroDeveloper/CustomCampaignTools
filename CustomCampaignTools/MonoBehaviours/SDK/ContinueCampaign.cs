#if MELONLOADER
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
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
            Campaign campaign = CampaignUtilities.GetFromLevel();

            if (!campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;
            
            campaign.saveData.LoadedSavePoint.LoadContinue(new Barcode(campaign.LoadScene));
#endif
        }

        public void EnableIfValidSave(GameObject obj)
        {
#if MELONLOADER
            Campaign campaign = CampaignUtilities.GetFromLevel();

            if (campaign.saveData.LoadedSavePoint.IsValid(out _)) obj.SetActive(true);
#endif
        }
    }
}