using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class CampaignAchievementManager : MonoBehaviour
    {
        public CampaignAchievementManager(IntPtr ptr) : base(ptr) { }

        public bool UnlockAchievement(string Key)
        {
            if(Campaign.SessionActive)
            {
                return Campaign.Session.saveData.UnlockAchievement(Key);
            }
            return false;
        }

        public void RelockAchievement(string Key)
        {
            if(Campaign.SessionActive)
            {
                Campaign.Session.saveData.LockAchievement(Key);
            }
        }
    }
}