using BoneLib.Notifications;
using HarmonyLib;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.UI;
using MelonLoader;
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
            MelonLogger.Msg($"BodylogEnable Session: {(Campaign.SessionActive ? "Active" : "Inactive")}, Avatar: {(Campaign.Session.saveData.AvatarUnlocked ? "Unlocked" : "Locked")}");

            if (!Campaign.SessionActive || Campaign.Session.saveData.AvatarUnlocked) return;

            MelonLogger.Msg($"BodylogEnable Type Includes Bodylog: {(Campaign.Session.AvatarRestrictionType.HasFlag(AvatarRestrictionType.DisableBodyLog) ? "True" : "False")}");

            if (Campaign.Session.AvatarRestrictionType.HasFlag(AvatarRestrictionType.DisableBodyLog))
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

            if (Campaign.Session.AvatarRestrictionType.HasFlag(AvatarRestrictionType.RestrictAvatar))
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
    }

    [HarmonyPatch(typeof(RigManager))]
    public static class ForceAvatarSwitch
    {
        [HarmonyPatch(nameof(RigManager.SwapAvatarCrate))]
        [HarmonyPrefix]
        public static void OnAvatarSwapped(RigManager __instance, ref Barcode barcode, ref UniTask<bool> __result)
        {
            if (!Campaign.SessionActive || Campaign.Session.saveData.AvatarUnlocked) return;

            if (Campaign.Session.AvatarRestrictionType.HasFlag(AvatarRestrictionType.EnforceWhitelist))
            {
                if (!Campaign.Session.WhitelistedAvatars.Contains(barcode.ID))
                {
                    Notifier.Send(new Notification()
                    {
                        Title = Campaign.Session.Name,
                        Message = "This avatar is not allowed at this time",
                        Type = NotificationType.Error,
                        ShowTitleOnPopup = true,
                    });

                    if (Campaign.Session.WhitelistedAvatars.Contains(__instance.AvatarCrate.Barcode.ID))
                    {
                        barcode = __instance.AvatarCrate.Barcode;
                    }
                    else
                    {
                        barcode = new Barcode(Campaign.Session.WhitelistedAvatars[0]);
                    }
                }
            }
            else if ((Campaign.Session.AvatarRestrictionType & AvatarRestrictionType.RestrictAvatar) != AvatarRestrictionType.None)
            {
                barcode = new Barcode(Campaign.Session.CampaignAvatar);
            }
        }
    }
}