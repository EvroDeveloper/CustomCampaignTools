using UnityEngine;
using UnityEngine.UI;
using Il2CppTMPro;
using MelonLoader;
using System;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class AchievementReferenceHolder : MonoBehaviour
    {
        public AchievementReferenceHolder(IntPtr ptr) : base(ptr) { }
        
        public Image achievementIcon;
        public TMP_Text titleTMP;
        public TMP_Text descriptionTMP;

        public void SetReferences(Image img, TMP_Text title, TMP_Text description)
        {
            achievementIcon = img;
            titleTMP = title;
            description = description;
        }

        public void ShowAchievement(AchievementData achievement)
        {
            bool unlocked = Campaign.Session.saveData.UnlockedAchievements.Contains(achievement.Key);
            
            titleTMP.text = achievement.Title;
            descriptionTMP.text = unlocked ? achievement.Description : !achievement.Hidden ? achievement.Description : "???";
            
            achievement.LoadIcon((t) => { achievementIcon.image = t; });
            achievementIcon.tintColor = unlocked ? Color.white : new Color(0.5, 0.5, 0.5, 1);
        }
    }
}