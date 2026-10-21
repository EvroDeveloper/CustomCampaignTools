using System.Collections.Generic;
using BoneLib.Notifications;

namespace CustomCampaignTools.Notifications;

/// <summary>
/// Handles the Sending of CampaignTools' Extended Notifications
/// </summary>
public static class Notifications
{
    public static Dictionary<Notification, NotificationData> PendingNotificationLink = [];

    public static void Send(NotificationData notificationData)
    {
        Notification notification = notificationData;
        PendingNotificationLink.Add(notification, notificationData);
        Notifier.Send(notification);
    }
}