using HarmonyLib;
using UnityEngine;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.UI;
using BoneLib.Notifications;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;

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
            __instance.popUpMenu.Deactivate();

            Notifier.Send(new Notification()
            {
                Title = Campaign.Session.Name,
                Message = "Avatar switching is locked on a first playthrough",
                Type = NotificationType.Error,
                ShowTitleOnPopup = true,
            });
        }
    }

    [HarmonyPatch(typeof(RigManager._SwapAvatarCrate_d__66))]
    public static class ForceAvatarSwitch
    {
        [HarmonyPatch(nameof(RigManager._SwapAvatarCrate_d__66.MoveNext))]
        [HarmonyPrefix]
        public static void OnAvatarSwapped(RigManager._SwapAvatarCrate_d__66 __instance)
        {
            if (!Campaign.SessionActive || !Campaign.Session.RestrictAvatar || Campaign.Session.saveData.AvatarUnlocked) return;

            __instance.barcode = new Barcode(Campaign.Session.CampaignAvatar);
        }
    }
}