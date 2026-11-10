using System.Collections.Generic;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.Notifications;
using Newtonsoft.Json;

namespace CustomCampaignTools.SaveData.Version1;

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

        NotificationManager.Send(new AchievementGetNotification(campaign, achievement));
        
        return true;
    }

    public void LockAchievement(string key)
    {
        if (UnlockedAchievements.Contains(key))
        {
            UnlockedAchievements.Remove(key);
        }
    }
}