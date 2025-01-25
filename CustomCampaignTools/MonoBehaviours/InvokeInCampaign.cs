using UnityEngine;
using MelonLoader;
using Il2CppUltEvents;
using System;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    //[RequireComponent(typeof(UltEventHolder))]
    public class InvokeInCampaign : MonoBehaviour
    {
        public InvokeInCampaign(IntPtr ptr) : base(ptr) { }

        void Awake()
        {
            if(TryGetComponent(out UltEventHolder ult))
            {
                ult.Invoke();
            }
        }
    }
}