#if MELONLOADER
using Il2CppSLZ.Bonelab;
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
    //[RequireComponent(typeof(HideOnAwake))]
    [AddComponentMenu("CustomCampaignTools/Unhide In Campaign")]
#endif
    public class UnhideInCampaign : MonoBehaviour
    {
#if MELONLOADER
        public UnhideInCampaign(IntPtr ptr) : base(ptr) { }
#endif
    }
}