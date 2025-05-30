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

        public void ShowAchievement(AchievementData achievement)
        {
            bool unlocked = Campaign.Session.saveData.UnlockedAchievements.Contains(achievement.Key);
            
            titleTMP.text = achievement.Name;
            descriptionTMP.text = unlocked ? achievement.Description : !achievement.Hidden ? achievement.Description : "???";
            
            //achievement.LoadIcon((t) => { achievementIcon.sprite = t; });
            achievementIcon.color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }
}