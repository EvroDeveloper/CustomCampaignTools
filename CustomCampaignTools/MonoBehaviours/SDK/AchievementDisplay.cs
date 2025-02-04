using UnityEngine;
using MelonLoader;
using System;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class AchievementDisplay : MonoBehaviour
    {
        public AchievementDisplay(IntPtr ptr) : base(ptr) { }

        public AchievementReferenceHolder[] achievementViews;
        private int _currentPage;
        private int _pageCount;

        public void NextPage()
        {
            
        }
        public void PrevPage()
        {

        }
    }
}