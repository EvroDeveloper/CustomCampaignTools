using System;
using CustomCampaignTools.Utilities.Patching;
using HarmonyLib;
using Il2CppSLZ.Bonelab;

namespace CustomCampaignTools.BonelabSupport.Patching;

[HarmonyPatch(typeof(PullCordDevice))]
public static class BodylogToggler
{
    private static PullCordDevice _lastFoundBodylog;

    [CampaignPatch(typeof(PullCordDevice), nameof(PullCordDevice.OnEnable), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void OnBodyLogEnabled(PullCordDevice __instance)
    {
        _lastFoundBodylog = __instance;

        if (Campaign.Session.IsBodylogRestricted && !Campaign.Session.saveData.AvatarUnlocked)
            __instance.gameObject.SetActive(false);
        
        __instance.gameObject.SetActive(Campaign.Session.saveData.ManualBodylogToggle);
    }

    public static void ForceSetBodylog(bool active)
    {
        if(_lastFoundBodylog != null)
            _lastFoundBodylog.gameObject.SetActive(active);
    }
}