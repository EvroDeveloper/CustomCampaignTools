using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class HideInCampaign : MonoBehaviour
    {
        public HideInCampaign(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}