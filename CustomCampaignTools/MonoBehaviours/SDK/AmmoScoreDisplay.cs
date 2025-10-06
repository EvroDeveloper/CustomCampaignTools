using UnityEngine;
using MelonLoader;
using Il2CppTMPro;
using System;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class AmmoScoreDisplay : MonoBehaviour
    {
        public AmmoScoreDisplay(IntPtr ptr) : base(ptr) { }

        public void SetTargetBarcode(string barcode) 
        {
            Campaign campaign = CampaignUtilities.GetFromLevel(barcode);
            if(campaign == null) 
            {
                MelonLogger.Msg($"[AmmoScoreDisplay] Could not find campaign for barcode {barcode}");
                return;
            }
            CampaignSaveData.AmmoSave ammoSave = campaign.saveData.GetSavedAmmo(barcode);
            GetComponent<TMP_Text>().text = ammoSave.GetCombinedTotal().ToString();
        }
    }
}