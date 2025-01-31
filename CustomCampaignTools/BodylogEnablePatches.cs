using HarmonyLib;
using UnityEngine;
using Il2CppSLZ.Bonelab;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(PullCordDevice))]
    public static class BodylogEnablePatches
    {
        [HarmonyPatch(nameof(PullCordDevice.OnEnable))]
        public static void OnDevToolsAdded(PullCordDevice __instance)
        {
            if(!Campaign.SessionActive || !Campaign.Session.RestrictDevTools) return;

            __instance.gameObject.SetActive();
        }
    }
}