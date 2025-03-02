using UnityEngine;
using MelonLoader;
using System;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class ContinueCampaign : MonoBehaviour
    {
        public ContinueCampaign(IntPtr ptr) : base(ptr) { }

        public void Continue()
        {
            Campaign campaign = CampaignUtilities.GetFromLevel();

            if (!campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;
            
            campaign.saveData.LoadedSavePoint.LoadContinue(new Barcode(campaign.LoadScene));
        }

        public void EnableIfValidSave(GameObject obj)
        {
            Campaign campaign = CampaignUtilities.GetFromLevel();

            if (campaign.saveData.LoadedSavePoint.IsValid(out _)) obj.SetActive(true);
        }
    }
}