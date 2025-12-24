using BoneLib.Notifications;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.UI;
using MelonLoader;
using System;
using System.Linq;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(PullCordDevice))]
    public static class BodylogEnablePatches
    {
        [HarmonyPatch(nameof(PullCordDevice.OnEnable))]
        [HarmonyPostfix]
        public static void OnBodyLogEnabled(PullCordDevice __instance)
        {
            if (!Campaign.SessionActive || Campaign.Session.saveData.AvatarUnlocked) return;

            if (Campaign.Session.IsBodylogRestricted)
            {
                __instance.gameObject.SetActive(false);
            }
        }
    }

    [HarmonyPatch(typeof(AvatarsPanelView))]
    public static class AvatarPanelEnable
    {
        [HarmonyPatch(nameof(AvatarsPanelView.Activate))]
        [HarmonyPostfix]
        public static void OnPanelEnabled(AvatarsPanelView __instance)
        {
            if (!Campaign.SessionActive || Campaign.Session.saveData.AvatarUnlocked) return;

            if (!Campaign.Session.avatarRestrictor.IsAvatarMenuAllowed())
            {
                __instance.Deactivate();
                __instance.popUpMenu.Deactivate();

                Notifier.Send(new Notification()
                {
                    Title = Campaign.Session.Name,
                    Message = "Avatar switching is currently locked",
                    Type = NotificationType.Error,
                    ShowTitleOnPopup = true,
                });
            }
        }

        [HarmonyPatch(nameof(AvatarsPanelView.SelectItem))]
        [HarmonyPrefix]
        public static bool OnElementSelected()
        {
            if (!Campaign.SessionActive || Campaign.Session.saveData.AvatarUnlocked) return true;

            return true;
        }
    }

    [HarmonyPatch(typeof(RigManager))]
    public static class ForceAvatarSwitch
    {
        public static void OnAvatarSwapped(RigManager __instance, Barcode barcode)
        {
            if (!Campaign.SessionActive || Campaign.Session.saveData.AvatarUnlocked) return;

            if(Campaign.Session.avatarRestrictor != null)
            {
                if(!Campaign.Session.avatarRestrictor.IsAvatarAllowed(barcode))
                {
                    Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch(__instance);
                }
            }
        }

        [HarmonyPatch(nameof(RigManager.Awake))]
        [HarmonyPrefix]
        public static void OnRigmanagerAwake(RigManager __instance)
        {
            __instance.onAvatarSwapped2 += new Action<Barcode>((b) => { OnAvatarSwapped(__instance, b); });
        }
    }
}