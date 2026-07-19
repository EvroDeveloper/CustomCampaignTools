using BoneLib;
using BoneLib.Notifications;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.UI;
using Il2CppSLZ.VRMK;
using System;

namespace CustomCampaignTools.Patching
{
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

        //[HarmonyPatch(nameof(AvatarsPanelView.SelectItem))]
        //[HarmonyPrefix]
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
                    Campaign.Session.avatarRestrictor.ForceRigManagerAvatar(__instance);
                }
            }
        }

        public static bool AllowAvatarSwap(RigManager __instance, Avatar newAvatar)
        {
            if (!Campaign.SessionActive || Campaign.Session.saveData.AvatarUnlocked || newAvatar == null || Campaign.Session.avatarRestrictor == null) return true; // Always allow avatar swap when not in campaign or avatar is unlocked

            bool avatarAllowed = Campaign.Session.avatarRestrictor.IsAvatarAllowed(newAvatar);

            if(!avatarAllowed) Campaign.Session.avatarRestrictor.OnFailedAvatarSwitch(__instance);

            return avatarAllowed;
        }

        [HarmonyPatch(nameof(RigManager.Awake))]
        [HarmonyPostfix]
        public static void OnRigManagerAwake(RigManager __instance)
        {
            if(Player.RigManager != __instance) return;

            __instance.onAvatarSwapped2 += new Action<Barcode>((b) => { OnAvatarSwapped(__instance, b); });
        }

        [HarmonyPatch(nameof(RigManager.SwitchAvatar))]
        [HarmonyPrefix]
        public static bool SwitchAvatarPrefix(RigManager __instance, Avatar newAvatar)
        {
            return AllowAvatarSwap(__instance, newAvatar);
        }

        [HarmonyPatch(nameof(RigManager.SwapAvatar))]
        [HarmonyPrefix]
        public static bool SwapAvatarPrefix(RigManager __instance, Avatar newAvatar)
        {
            return AllowAvatarSwap(__instance, newAvatar);
        }
    }
}