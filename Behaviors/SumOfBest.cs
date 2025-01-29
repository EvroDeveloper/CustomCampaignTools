using CustomCampaignTools;
using CustomCampaignTools.Utilities;
using Il2CppTMPro;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class SumOfBest : MonoBehaviour
    {
        public SumOfBest(IntPtr ptr) : base(ptr) { }

        private string levelBarcode;

        // Use this for initialization
        void Start()
        {
            levelBarcode = gameObject.name;

            Campaign campaign = Campaign.GetFromLevel(levelBarcode);
			
            CampaignSaveData.AmmoSave ammoSave = campaign.saveData.GetSavedAmmo(levelBarcode);

            transform.parent.GetComponent<TextMeshProUGUI>().text = ammoSave.GetCombinedTotal().ToString();
        }
    }
}