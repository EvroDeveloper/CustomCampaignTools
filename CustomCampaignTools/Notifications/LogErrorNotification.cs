using System;
using BoneLib.Notifications;
using CustomCampaignTools.Debug;

namespace CustomCampaignTools.Notifications;

public class LogErrorNotification : NotificationBase
{
    public readonly string Title;
    public readonly string Message;

    public LogErrorNotification(object message)
    {
        Title = "ERROR";
        Message = message.ToString();
    }

    public LogErrorNotification(ILoggerSource loggerSource, object message)
    {
        Title = loggerSource.GetName();
        Message = message.ToString();
    }

    public LogErrorNotification(Exception exception)
    {
        Title = exception.GetType().Name;
        Message = exception.Message;
    }

    public override NotificationData ToNotification()
    {
        return new NotificationData(Title, Message, NotificationType: NotificationType.Error);
    }
}