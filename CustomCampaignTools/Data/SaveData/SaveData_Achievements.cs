using BoneLib.Notifications;
using CustomCampaignTools.Bonemenu;
using Il2CppSLZ.Marrow.Audio;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal List<string> UnlockedAchievements = [];
        
        public bool UnlockAchievement(string key)
        {
            UnlockedAchievements ??= [];
            if (UnlockedAchievements.Contains(key)) return false;

            foreach (AchievementData achievement in campaign.Achievements)
            {
                if (achievement.Key != key) continue;

                if (campaign.AchievementUnlockSound != null)
                    Audio3dManager.Play2dOneShot(campaign.AchievementUnlockSound, Audio3dManager.ui, new Il2CppSystem.Nullable<float>(1f), new Il2CppSystem.Nullable<float>(1f));

                if (achievement.cachedTexture != null)
                {
                    Notifier.Send(new Notification()
                    {
                        CustomIcon = achievement.cachedTexture,
                        Title = $"Achievement Get: {achievement.Name}",
                        Message = achievement.Description,
                        Type = NotificationType.CustomIcon,
                        PopupLength = 5,
                        ShowTitleOnPopup = true,
                    });
                }
                else
                {
                    Notifier.Send(new Notification()
                    {
                        Title = $"Achievement Get: {achievement.Name}",
                        Message = achievement.Description,
                        Type = NotificationType.Information,
                        PopupLength = 5,
                        ShowTitleOnPopup = true
                    });
                }

                UnlockedAchievements.Add(key);
                SaveToDisk();
                CampaignBoneMenu.RefreshCampaignPage(campaign);
                return true;
            }
            return false;
        }

        public void LockAchievement(string key)
        {
            if (UnlockedAchievements.Contains(key))
            {
                UnlockedAchievements.Remove(key);
            }
        }
    }
}