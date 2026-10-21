using System.Collections.Generic;
using System.Reflection;
using BoneLib;
using BoneLib.Notifications;
using HarmonyLib;
using Il2CppSLZ.Marrow.Audio;

namespace CustomCampaignTools.Notifications;

/// <summary>
/// Patch Notification Sounds and Actions into BoneLib's Notification Queue system thingy
/// </summary>
[HarmonyPatch(typeof(Notifier))]
public static class NotificationSoundPatches
{
    [HarmonyPatch("DequeueNotification")]
    [HarmonyPrefix]
    public static void DequeueNotificationPostfix()
    {
        FieldInfo queuedNotifsField = typeof(Notifier).GetField("QueuedNotifications", BindingFlags.Static | BindingFlags.NonPublic);
        Notification upNext = ((Queue<Notification>)queuedNotifsField.GetValue(null)).Peek();
        if(Notifications.PendingNotificationLink.ContainsKey(upNext))
        {
            NotificationData notificationData = Notifications.PendingNotificationLink[upNext];
            if(notificationData.SoundEffect != null)
            {
                Audio3dManager.Play2dOneShot(notificationData.SoundEffect, Audio3dManager.ui, new(1f), new(1f));
            }
            if(notificationData.OnNotificationPlayed != null)
            {
                SafeActions.InvokeActionSafe(notificationData.OnNotificationPlayed);
            }
            Notifications.PendingNotificationLink.Remove(upNext);
        }
    }
}