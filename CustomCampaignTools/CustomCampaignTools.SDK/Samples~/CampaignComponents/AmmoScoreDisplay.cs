#if MELONLOADER
using MelonLoader;
using Il2CppTMPro;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using CustomCampaignTools.Debug;
using CustomCampaignTools.Data;
using CustomCampaignTools.Utilities;
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
            if (!CampaignUtilities.TryGetCampaignLevel(new Barcode(barcode), out CampaignLevel campaignLevel))
            {
                CampaignLogger.SessionMsg($"AmmoScoreDisplay could not find campaign for barcode {barcode}");
                return;
            }
            AmmoCount ammoCount = campaignLevel.campaign.saveData.GetSavedAmmo(campaignLevel);
            if(textMeshPro.Get() == null)
                textMeshPro.Set(GetComponent<TMP_Text>());
            GetComponent<TMP_Text>().text = ammoCount.Total.ToString();
#endif
        }
    }
}
