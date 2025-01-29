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
            
            SavepointFunctions.WasLastLoadByContinue = true;
            FadeLoader.Load(new Barcode(campaign.saveData.LoadedSavePoint.LevelBarcode), new Barcode(campaign.LoadScene));
        }

        public void EnableIfValidSave(GameObject obj)
        {
            Campaign campaign = Campaign.GetFromLevel();

            if (campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint)) obj.SetActive(true);
        }
    }
}