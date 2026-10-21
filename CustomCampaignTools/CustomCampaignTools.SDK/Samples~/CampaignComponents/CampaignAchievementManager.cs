#if MELONLOADER
using MelonLoader;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/Achievements/Achievement Manager")]
#endif
    public class CampaignAchievementManager : MonoBehaviour
    {
#if MELONLOADER
        public CampaignAchievementManager(IntPtr ptr) : base(ptr) { }
#endif

        public bool UnlockAchievement(string Key)
        {
#if MELONLOADER
            if (Campaign.SessionActive)
            {
                return Campaign.Session.saveData.UnlockAchievement(Key);
            }
#endif
            return false;
        }

        public void RelockAchievement(string Key)
        {
#if MELONLOADER
            if (Campaign.SessionActive)
            {
                Campaign.Session.saveData.LockAchievement(Key);
            }
#endif
        }

        public bool IsAchievementUnlocked(string Key)
        {
#if MELONLOADER
            if (Campaign.SessionActive)
            {
                return Campaign.Session.saveData.UnlockedAchievements.Contains(Key);
            }
#endif
            return false;
        }

        public int GetAchievementsTotal()
        {
#if MELONLOADER
            if (Campaign.SessionActive)
                return Campaign.Session.Achievements.Count;
#endif
            return 0;
        }

        public int GetAchievementsCompleted()
        {
#if MELONLOADER
            if (Campaign.SessionActive)
                return Campaign.Session.saveData.UnlockedAchievements.Count;
#endif
            return 0;
        }

        public float GetAchievementsCompletedPercentage()
        {
#if MELONLOADER
            if (Campaign.SessionActive)
            {
                int total = Campaign.Session.Achievements.Count;
                if (total == 0) return 0f;
                int completed = Campaign.Session.saveData.UnlockedAchievements.Count;
                return (float)completed / (float)total;
            }
#endif
            return 0f;
        }
    }
}