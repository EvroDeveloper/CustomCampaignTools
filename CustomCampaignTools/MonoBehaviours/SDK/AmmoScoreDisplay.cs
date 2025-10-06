#if MELONLOADER
using MelonLoader;
using Il2CppTMPro;
#else
using TMPro;
#endif

using UnityEngine;
using System;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("CustomCampaignTools/Ammo Score Display")]
#endif
    public class AmmoScoreDisplay : MonoBehaviour
    {
#if MELONLOADER
        public AmmoScoreDisplay(IntPtr ptr) : base(ptr) { }
#endif

        public void SetTargetBarcode(string barcode)
        {
#if MELONLOADER
            Campaign campaign = CampaignUtilities.GetFromLevel(barcode);
            if (campaign == null)
            {
                MelonLogger.Msg($"[AmmoScoreDisplay] Could not find campaign for barcode {barcode}");
                return;
            }
            CampaignSaveData.AmmoSave ammoSave = campaign.saveData.GetSavedAmmo(barcode);
            GetComponent<TMP_Text>().text = ammoSave.GetCombinedTotal().ToString();
#endif
        }
    }
}