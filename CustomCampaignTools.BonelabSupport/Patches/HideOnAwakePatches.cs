using CustomCampaignTools.SDK;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(HideOnAwake))]
    public static class HideOnAwakePatches
    {
        [HarmonyPatch(nameof(HideOnAwake.Awake))]
        [HarmonyPostfix]
        public static void UnhideInCampaign(HideOnAwake __instance)
        {
            if(__instance.TryGetComponent<UnhideInCampaign>(out _))
            {
                __instance.gameObject.SetActive(true);
                Object.Destroy(__instance);
            }
        }
    }
}
