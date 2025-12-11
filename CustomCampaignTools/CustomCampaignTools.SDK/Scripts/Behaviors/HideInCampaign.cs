#if MELONLOADER
using MelonLoader;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Hide In Campaign")]
#endif
    public class HideInCampaign : MonoBehaviour
    {
#if MELONLOADER
        public HideInCampaign(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            if(Campaign.SessionActive)
                gameObject.SetActive(false);
        }
#endif
    }
}