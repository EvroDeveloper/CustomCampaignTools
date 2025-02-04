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
            titleTMP.text = achievement.Title;
            descriptionTMP.text = achievement.Description;
            achievement.LoadIcon((t) => { achievement.image = t; });
        }
    }
}