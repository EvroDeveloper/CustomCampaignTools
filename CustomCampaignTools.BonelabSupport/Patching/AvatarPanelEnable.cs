using BoneLib.Notifications;
using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.UI;

namespace CustomCampaignTools.BonelabSupport.Patching;

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
}