#if MELONLOADER
using BoneLib.Notifications;
using MelonLoader;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Triggerable Notification")]
#endif
    public class TriggerableNotification : MonoBehaviour
    {
#if MELONLOADER
        public TriggerableNotification(IntPtr ptr) : base(ptr) { }
#endif

        public void SendNotification(string Title, string Message, Texture2D CustomIcon, NotifType Type, float Length)
        {
#if MELONLOADER
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
#endif
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