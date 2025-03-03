using BoneLib.Notifications;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class TriggerableNotification : MonoBehaviour
    {
        public TriggerableNotification(IntPtr ptr) : base(ptr) { }

        public void SendNotification(string Title, string Message, Texture2D CustomIcon, NotifType Type, float Length)
        {
            Texture2D resizedTexture = CustomIcon.ProperResize(336, 336);
            Notifier.Send(new Notification()
            {
                Title = Title,
                Message = Message,
                CustomIcon = resizedTexture,
                Type = (NotificationType)Type,
                PopupLength = Length,
                ShowTitleOnPopup = Title != string.Empty,
            });
        }
    }

    public enum NotifType
    {
        Information = 0,
        Warning = 1,
        Error = 2,
        Success = 3,
        CustomIcon = 4
    }
}