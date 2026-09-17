using System;
using CustomCampaignTools.Utilities.Patching;
using HarmonyLib;
using Il2CppSLZ.Bonelab;

namespace CustomCampaignTools.BonelabSupport.Patching;

public static class BodylogToggler
{
    private static PullCordDevice _lastFoundBodylog;

    [CampaignPatch(typeof(PullCordDevice), nameof(PullCordDevice.OnEnable), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void OnBodyLogEnabled(PullCordDevice __instance)
    {
        _lastFoundBodylog = __instance;

        if ((Campaign.Session.IsBodylogRestricted && !Campaign.Session.saveData.AvatarUnlocked) || !Campaign.Session.saveData.ManualBodylogToggle)
            __instance.gameObject.SetActive(false);
    }

    [CampaignPatch(typeof(CampaignSaveData), nameof(CampaignSaveData.UnlockAvatar), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void OnAvatarUnlockedHook(CampaignSaveData __instance)
    {
        if(__instance.campaign == Campaign.Session)
        {
            ForceSetBodylog(true);
            BoneLib.Player.UIRig.popUpMenu.AddAvatarsMenu();
        }
    }

    public static void ForceSetBodylog(bool active)
    {
        if(_lastFoundBodylog != null)
            _lastFoundBodylog.gameObject.SetActive(active);
    }
}