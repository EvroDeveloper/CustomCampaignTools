using System;
using HarmonyLib;
using Il2CppSLZ.Bonelab;

namespace CustomCampaignTools.BonelabSupport.Patches
{
    [HarmonyPatch(typeof(PullCordDevice))]
    public static class BodylogToggler
    {
        private static PullCordDevice _lastFoundBodylog;

        [HarmonyPatch(nameof(PullCordDevice.OnEnable))]
        [HarmonyPostfix]
        public static void OnBodyLogEnabled(PullCordDevice __instance)
        {
            if (!Campaign.SessionActive) return;

            if (Campaign.Session.IsBodylogRestricted && !Campaign.Session.saveData.AvatarUnlocked)
                __instance.gameObject.SetActive(false);
            
            __instance.gameObject.SetActive(Campaign.Session.saveData.ManualBodylogToggle);
        }

        public static void ForceSetBodylog(bool active)
        {
            _lastFoundBodylog?.gameObject.SetActive(active);
        }
    }
}