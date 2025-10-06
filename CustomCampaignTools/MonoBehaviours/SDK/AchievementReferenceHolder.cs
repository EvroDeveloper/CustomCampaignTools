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
#endif
    public class AchievementReferenceHolder : MonoBehaviour
    {
#if MELONLOADER
        public AchievementReferenceHolder(IntPtr ptr) : base(ptr) { }
        
        public Il2CppReferenceField<Image> achievementIcon;
        public Il2CppReferenceField<TMP_Text> titleTMP;
        public Il2CppReferenceField<TMP_Text> descriptionTMP;

        public void ShowAchievement(AchievementData achievement)
        {
            bool unlocked = Campaign.Session.saveData.UnlockedAchievements.Contains(achievement.Key);
            
            titleTMP.Get().text = achievement.Name;
            descriptionTMP.Get().text = unlocked ? achievement.Description : !achievement.Hidden ? achievement.Description : "???";
            
            //achievement.LoadIcon((t) => { achievementIcon.sprite = t; });
            achievementIcon.Get().color = unlocked ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1);
        }
#else
        public Image achievementIcon;
        public TMP_Text titleTMP;
        public TMP_Text descriptionTMP;
#endif
    }
}