using BoneLibNotif = BoneLib.Notifications.Notification;

namespace CustomCampaignTools.Notifications;

public class NotificationBase
{
    public virtual NotificationData ToNotification()
    {
        return new NotificationData()
        {
            Title = "Default Notification",
            Message = "This is a Default Notification",
        };
    }

    public virtual void PlayNotification()
    {
        Notifications.Send(this);
    }

    public static implicit operator NotificationData(NotificationBase notif)
    {
        return notif.ToNotification();
    }
}