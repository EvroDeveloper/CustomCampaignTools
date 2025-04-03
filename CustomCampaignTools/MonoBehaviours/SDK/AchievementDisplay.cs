using Il2CppTMPro;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class AchievementDisplay : MonoBehaviour
    {
        public AchievementDisplay(IntPtr ptr) : base(ptr) { }

        public AchievementReferenceHolder[] achievementViews;
        private int _currentPage;
        private int _lastPage => Mathf.FloorToInt(Campaign.Session.Achievements.Count / achievementViews.Length);

        private Button nextButton;
        private Button backButton;
        private TMP_Text pageText;
        private TMP_Text unlockCount;

        public void Awake()
        {
            achievementViews = GetComponentsInChildren<AchievementReferenceHolder>(true);
            nextButton.onClick.AddListener(new Action(() => { NextPage(); }));
            backButton.onClick.AddListener(new Action(() => { PrevPage(); }));
        }

        public void Activate()
        {
            UpdateVisualization();
        }

        public void NextPage()
        {
            _currentPage = Mathf.Min(_lastPage, _currentPage + 1);
            UpdateVisualization();
        }
        public void PrevPage()
        {
            _currentPage = Mathf.Max(0, _currentPage - 1);
            UpdateVisualization();
        }

        private void UpdateVisualization()
        {
            if(_currentPage <= 0) backButton.gameObject.SetActive(false);
            else backButton.gameObject.SetActive(true);

            if(_currentPage >= _lastPage) nextButton.gameObject.SetActive(false);
            else nextButton.gameObject.SetActive(true);

            if(pageText != null)
                pageText.text = $"{_currentPage+1}/{_lastPage+1}";

            for(int i = 0; i < achievementViews.Length; i++)
            {
                var currentView = achievementViews[i];
                int achievementIndex = (achievementViews.Length * _currentPage) + i;

                if(achievementIndex < Campaign.Session.Achievements.Count)
                {
                    currentView.gameObject.SetActive(true);
                    currentView.ShowAchievement(Campaign.Session.Achievements[achievementIndex]);
                }
                else
                {
                    currentView.gameObject.SetActive(false);
                }
            }

            if(unlockCount != null)
                unlockCount.text = $"{Campaign.Session.saveData.UnlockedAchievements.Count}/{Campaign.Session.Achievements.Count} Achievements Unlocked";
        }
    }
}