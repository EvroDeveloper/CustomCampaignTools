#if MELONLOADER
using MelonLoader;
using Il2CppUltEvents;
using CustomCampaignTools;
#else
using UltEvents;
#endif
using UnityEngine;
using System;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [RequireComponent(typeof(UltEventHolder))]
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Invoke In Campaign")]
#endif
    public class InvokeInCampaign : MonoBehaviour
    {
#if MELONLOADER
        public InvokeInCampaign(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            if(!Campagin.SessionActive) return;
            
            if (TryGetComponent(out UltEventHolder ult))
            {
                ult.Invoke();
            }
        }
#endif
    }
}