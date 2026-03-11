#if MELONLOADER
using MelonLoader;
using Il2CppTMPro;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppInterop.Runtime.Attributes;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Data;
using Il2CppSLZ.Marrow.Warehouse;
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
            Barcode mBarcode = new Barcode(barcode);
            if (!CampaignUtilities.TryGetFromLevel(mBarcode, out Campaign campaign))
            {
                CampaignLogger.SessionMsg($"AmmoScoreDisplay could not find campaign for barcode {barcode}");
                return;
            }
            AmmoSave ammoSave = campaign.saveData.GetSavedAmmo(mBarcode);
            if(textMeshPro.Get() == null)
                textMeshPro.Set(GetComponent<TMP_Text>());
            GetComponent<TMP_Text>().text = ammoSave.GetCombinedTotal().ToString();
#endif
        }
    }
}