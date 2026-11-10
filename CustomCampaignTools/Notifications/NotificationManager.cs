using System.Collections.Generic;
using BoneLib.Notifications;

namespace CustomCampaignTools.Notifications;

/// <summary>
/// Handles the Sending of CampaignTools' Extended Notifications
/// </summary>
public static class NotificationManager
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

    /// <summary>
    /// Send a Campaign Notification
    /// </summary>
    /// <param name="notificationBase">The notification to send</param>
    public static void Send(NotificationBase notificationBase)
    {
        notificationBase.PlayNotification();
    }
}