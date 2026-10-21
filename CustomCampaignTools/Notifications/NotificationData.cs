using System;
using BoneLib.Notifications;
using UnityEngine;

namespace CustomCampaignTools.Notifications;

/// <summary>
/// Holds all the data about a Notification
/// </summary>
public class NotificationData
{
    public NotificationText Title;
    public NotificationText Message;
    public float Duration = 2f;
    public Texture2D Icon;
    public NotificationType NotificationType;
    public AudioClip SoundEffect;
    public Action OnNotificationPlayed;

    public NotificationData()
    {
        
    }

    public NotificationData(NotificationText Title, NotificationText Message, float Duration = 2f, Texture2D Icon = null, NotificationType NotificationType = NotificationType.Information, AudioClip SoundEffect = null, Action OnNotificationPlayed = null)
    {
        this.Title = Title;
        this.Message = Message;
        this.Duration = Duration;
        this.Icon = Icon;
        this.NotificationType = Icon == null ? NotificationType : NotificationType.CustomIcon;
        this.SoundEffect = SoundEffect;
        this.OnNotificationPlayed = OnNotificationPlayed;
    }

    public static implicit operator Notification(NotificationData data)
    {
        return new Notification()
        {
            Title = data.Title,
            ShowTitleOnPopup = string.IsNullOrEmpty(data.Title.Text),
            Message = data.Message,
            CustomIcon = data.Icon,
            Type = data.Icon ? NotificationType.CustomIcon : data.NotificationType,
            PopupLength = data.Duration,
        };
    }
}