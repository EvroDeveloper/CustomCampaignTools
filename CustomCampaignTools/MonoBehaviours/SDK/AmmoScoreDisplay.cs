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
            Campaign campaign = Campaign.GetFromLevel(barcode);
            CampaignSaveData.AmmoSave ammoSave = campaign.saveData.GetSavedAmmo(barcode);
            GetComponent<TMP_Text>().text = ammoSave.GetCombinedTotal().ToString();
        }
    }
}