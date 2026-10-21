using System.Collections;
using CustomCampaignTools.Utilities;
using MelonLoader;
using UnityEngine;

namespace CustomCampaignTools.Notifications;

public class AchievementGetNotification : NotificationBase
{
    public readonly AchievementData achievementData;
    public readonly Campaign campaign;
    private Texture2D cachedIcon;
    private AudioClip cachedAchievementSound;
    public AchievementGetNotification(Campaign campaign, AchievementData achievementData)
    {
        this.achievementData = achievementData;
        this.campaign = campaign;
    }

    public override void PlayNotification()
    {
        MelonCoroutines.Start(CoPlayNotification());
    }

    public IEnumerator CoPlayNotification()
    {
        if(campaign.AchievementUnlockSoundReference.TryGetDataCard(out var disc))
        {
            yield return new AwaitTask<AudioClip>(disc.AudioClip.LoadAssetAsync(), out var result);
            cachedAchievementSound = result.Value;
        }

        if(achievementData.Type == AchievementData.IconType.Bytes)
        {
            cachedIcon = achievementData.LoadIconBytes();
        }
        else
        {
            yield return new AwaitTask<Texture2D>(achievementData.LoadIconTask(), out var tex);
            cachedIcon = tex.Value;
        }
        yield return new AwaitTask<Color32[]>(cachedIcon.CPUResizeBackground(336, 336), out var pixels);

        cachedIcon = new Texture2D(336, 336, TextureFormat.RGBA32, false);
        cachedIcon.SetPixels32(pixels.Value);
        cachedIcon.Apply();
        
        cachedIcon.hideFlags = HideFlags.DontUnloadUnusedAsset;

        Notifications.Send(this);

        yield return new WaitForSeconds(5f);
        
        cachedIcon.hideFlags = HideFlags.None;
    }

    public override NotificationData ToNotification()
    {
        return new NotificationData 
        (
            Title: $"Achievement Get: {achievementData.Name}",
            Message: achievementData.Description,
            Duration: 2f, 
            Icon: cachedIcon,
            SoundEffect: cachedAchievementSound
        );
    }
}