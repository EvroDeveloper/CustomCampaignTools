#if MELONLOADER
using MelonLoader;
using Il2CppTMPro;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppInterop.Runtime.Attributes;
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
    [AddComponentMenu("CustomCampaignTools/Ammo Score Display")]
#endif
    public class AmmoScoreDisplay : MonoBehaviour
    {
#if MELONLOADER
        public AmmoScoreDisplay(IntPtr ptr) : base(ptr) { }

        public Il2CppReferenceField<TMP_Text> textMeshPro;
#else
        public TMP_Text textMeshPro;
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
            if(textMeshPro.Get() == null)
                textMeshPro.Set(GetComponent<TMP_Text>());
            GetComponent<TMP_Text>().text = ammoSave.GetCombinedTotal().ToString();
#endif
        }
    }
}