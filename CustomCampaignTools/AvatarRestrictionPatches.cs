using HarmonyLib;
using UnityEngine;
using Il2CppSLZ.Bonelab;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(PullCordDevice))]
    public static class BodylogEnablePatches
    {
        [HarmonyPatch(nameof(PullCordDevice.OnEnable))]
        [HarmonyPostfix]
        public static void OnBodyLogEnabled(PullCordDevice __instance)
        {
            if(!Campaign.SessionActive || !Campaign.Session.RestrictAvatar || Campaign.Session.saveData.AvatarUnlocked) return;

            __instance.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(AvatarsPanelView))]
    public static class AvatarPanelEnable
    {
        [HarmonyPatch(nameof(AvatarsPanelView.OnEnable))]
        [HarmonyPostfix]
        public static void OnPanelEnabled(AvatarsPanelView __instance)
        {
            if(!Campaign.SessionActive || !Campaign.Session.RestrictAvatar || Campaign.Session.saveData.AvatarUnlocked) return;

            __instance.Deactivate();
        }
    }
}