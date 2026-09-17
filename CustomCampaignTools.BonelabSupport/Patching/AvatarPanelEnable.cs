using CustomCampaignTools.Utilities.Patching;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.UI;
using BoneLib.Notifications;

namespace CustomCampaignTools.BonelabSupport.Patching;

public static class AvatarPanelEnable
{
    [CampaignPatch(typeof(AvatarsPanelView), nameof(AvatarsPanelView.Activate), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void OnPanelEnabled(AvatarsPanelView __instance)
    {
        if (!Campaign.Session.ShouldRestrictAvatar) return;
        if (Campaign.Session.avatarRestrictor.IsAvatarMenuAllowed()) return;

        __instance.CloseMenu();
        __instance.popUpMenu.Deactivate();

        Notifier.Send(new Notification()
        {
            Title = Campaign.Session.Name,
            Message = "Avatar switching is currently locked",
            Type = NotificationType.Error,
            ShowTitleOnPopup = true,
        });
    }

    [CampaignPatch(typeof(PopUpMenuView), nameof(PopUpMenuView.AddAvatarsMenu), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPrefix]
    public static bool OnMenuAddAvatarMenu(PopUpMenuView __instance)
    {
        if (!Campaign.Session.ShouldRestrictAvatar) return true;
        return Campaign.Session.avatarRestrictor.IsAvatarMenuAllowed();
    }
}