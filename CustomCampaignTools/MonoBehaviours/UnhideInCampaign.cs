using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    //[RequireComponent(typeof(HideOnAwake))]
    public class UnhideInCampaign : MonoBehaviour
    {
        public UnhideInCampaign(IntPtr ptr) : base(ptr) { }

        private bool hasUnhidden = false;

        void OnDisable()
        {
            if(!hasUnhidden && Campaign.SessionActive)
            {
                gameObject.SetActive(true);
                hasUnhidden = true;
            }
        }
    }
}