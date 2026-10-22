using System.Collections.Generic;
using BoneLib.Notifications;

namespace CustomCampaignTools.Notifications;

/// <summary>
/// Handles the Sending of CampaignTools' Extended Notifications
/// </summary>
public static class Notifications
{
    public static readonly Dictionary<Notification, NotificationData> PendingNotificationLink = [];

    /// <summary>
    /// Send a NotificationData to BoneMenu's Notification System, with Links for extra features
    /// </summary>
    /// <param name="notificationData"></param>
    public static void Send(NotificationData notificationData)
    {
        Notification notification = notificationData;
        PendingNotificationLink.Add(notification, notificationData);
        Notifier.Send(notification);
    }
}