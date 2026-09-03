using BoneLib.Notifications;
using CustomCampaignTools.Utilities.Patching;
using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppSLZ.UI;

namespace CustomCampaignTools.BonelabSupport.Patching;

public static class AvatarPanelEnable
{
    [CampaignPatch(typeof(AvatarsPanelView), nameof(AvatarsPanelView.Activate), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void OnPanelEnabled(AvatarsPanelView __instance)
    {
        if (Campaign.Session.saveData.AvatarUnlocked) return;
        if (Campaign.Session.avatarRestrictor.IsAvatarMenuAllowed()) return;

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