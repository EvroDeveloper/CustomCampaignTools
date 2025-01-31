using UnityEngine;
using MelonLoader;
using System;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class ContinueCampaign : MonoBehaviour
    {
        public ContinueCampaign(IntPtr ptr) : base(ptr) { }

        public void Continue()
        {
            Campaign campaign = Campaign.GetFromLevel();

            if (!campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;
            
            campaign.saveData.LoadedSavePoint.LoadContinue(new Barcode(campaign.LoadScene));
        }

        public void EnableIfValidSave(GameObject obj)
        {
            Campaign campaign = Campaign.GetFromLevel();

            if (campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint)) obj.SetActive(true);
        }
    }
}