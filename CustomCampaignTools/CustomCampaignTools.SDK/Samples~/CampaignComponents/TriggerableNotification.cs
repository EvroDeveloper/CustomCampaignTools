#if MELONLOADER
using BoneLib.Notifications;
using CustomCampaignTools.Notifications;
using CustomCampaignTools.Utilities;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppUltEvents;
using MelonLoader;
#else
using UltEvents;
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
        public Il2CppReferenceField<UltEventHolder> OnNotificationPlay;
#else
        public UltEventHolder OnNotificationPlay;
#endif

        public void SendNotification(string Title, string Message, Texture2D CustomIcon, NotifType Type, float Duration, AudioClip SoundEffect)
        {
#if MELONLOADER
            MelonCoroutines.Start(CoSendNotification(Title, Message, CustomIcon, Type, Duration, SoundEffect));
#endif
        }

#if MELONLOADER
        public IEnumerator CoSendNotification(string Title, string Message, Texture2D CustomIcon, NotifType Type, float Duration, AudioClip SoundEffect)
        {
            yield return new AwaitTask<Color32[]>(CustomIcon.CPUResizeBackground(336, 336), out var pixels);
            CustomIcon = new Texture2D(336, 336, TextureFormat.RGBA32, false);
            CustomIcon.SetPixels32(pixels.Value);
            CustomIcon.Apply();

            NotificationManager.Send(new NotificationData(Title, Message,  Duration, CustomIcon, (NotificationType)Type, SoundEffect));
        }
#endif
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