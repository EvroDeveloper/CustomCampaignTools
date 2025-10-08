#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppInterop.Runtime.Attributes;
using Il2CppTMPro;
using MelonLoader;
#else
using TMPro;
#endif

using UnityEngine;
using UnityEngine.UI;
using System;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Achievements/Achievement Reference Holder")]
#endif
    public class AchievementReferenceHolder : MonoBehaviour
    {
#if MELONLOADER
        public AchievementReferenceHolder(IntPtr ptr) : base(ptr) { }
        
        public Il2CppReferenceField<Image> achievementIcon;
        public Il2CppReferenceField<TMP_Text> titleTMP;
        public Il2CppReferenceField<TMP_Text> descriptionTMP;
        public Il2CppReferenceField<GameObject> lockIcon;

        public void ShowAchievement(AchievementData achievement)
        {
            bool unlocked = Campaign.Session.saveData.UnlockedAchievements.Contains(achievement.Key);
            
            titleTMP.Get().text = achievement.Name;
            descriptionTMP.Get().text = unlocked ? achievement.Description : !achievement.Hidden ? achievement.Description : "???";

            if(lockIcon.Get() != null)
            {
                lockIcon.Get().SetActive(!unlocked);
            }
            
            if(achievement.cachedSprite != null)
            {
                achievementIcon.Get().sprite = achievement.cachedSprite;
            }
            achievementIcon.Get().color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1);
        }
#else
        [Tooltip("Image to show the achievement's Icon.")]
        public Image achievementIcon;
        
        [Tooltip("Text to show the achievement's title.")]
        public TMP_Text titleTMP;

        [Tooltip("Text to show the achievement's description.")]
        public TMP_Text descriptionTMP;

        [Tooltip("Optional icon to show when the achievement is locked.")]
        public GameObject lockIcon;
#endif
    }
}