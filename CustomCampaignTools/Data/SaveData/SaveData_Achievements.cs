using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BoneLib.Notifications;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.Utilities;
using Il2CppCysharp.Threading.Tasks;
using Il2CppSLZ.Marrow.Audio;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomCampaignTools;

public partial class CampaignSaveData
{
    [JsonProperty]
    public List<string> UnlockedAchievements = [];
    
    public bool UnlockAchievement(string key)
    {
        UnlockedAchievements ??= [];
        if (UnlockedAchievements.Contains(key)) return false;

        AchievementData achievement = campaign.Achievements[key];

        if(achievement == null) return false;

        UnlockedAchievements.Add(key);
        SaveToDisk();
        GameManager.currentGameConfiguration.RefreshCampaignMenu(campaign);

        MelonCoroutines.Start(FireAchievementNotification(achievement, campaign.AchievementUnlockSoundReference));
        return true;
    }

    public void FireAchievementNotification(AchievementData achievementData)
    {
        MelonCoroutines.Start(FireAchievementNotification(achievementData, campaign.AchievementUnlockSoundReference));
    }

    private static IEnumerator FireAchievementNotification(AchievementData achievementData, MonoDiscReference achievementSoundReference)
    {
        AudioClip achievementSound = null;
        if(achievementSoundReference.TryGetDataCard(out var disc))
        {
            yield return new AwaitTask<AudioClip>(disc.AudioClip.LoadAssetAsync(), out var result);
            achievementSound = result.Value;
        }

        Texture2D achievementIcon;
        if(achievementData.Type == AchievementData.IconType.Bytes)
        {
            achievementIcon = achievementData.LoadIconBytes();
        }
        else
        {
            yield return new AwaitTask<Texture2D>(achievementData.LoadIconTask(), out var tex);
            achievementIcon = tex.Value;
        }
        achievementIcon = achievementIcon.CPUResize(336, 336);
        achievementIcon.hideFlags = HideFlags.DontUnloadUnusedAsset;

        NotificationType notificationType = achievementIcon == null ? NotificationType.Information : NotificationType.CustomIcon;
        Notifier.Send(new Notification()
        {
            CustomIcon = achievementIcon,
            Title = $"Achievement Get: {achievementData.Name}",
            Message = achievementData.Description,
            Type = notificationType,
            PopupLength = 5,
            ShowTitleOnPopup = true,
        });

        if(achievementSound != null)
        {
            Audio3dManager.Play2dOneShot(achievementSound, Audio3dManager.ui, new(1f), new(1f));
        }

        yield return new WaitForSeconds(5f);
        
        achievementIcon.hideFlags = HideFlags.None;
    }

    public void LockAchievement(string key)
    {
        if (UnlockedAchievements.Contains(key))
        {
            UnlockedAchievements.Remove(key);
        }
    }
}