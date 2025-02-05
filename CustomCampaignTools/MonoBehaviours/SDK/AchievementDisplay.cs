using UnityEngine;
using MelonLoader;
using Il2CppTMPro;
using UnityEngine.UI;
using System;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class AchievementDisplay : MonoBehaviour
    {
        public AchievementDisplay(IntPtr ptr) : base(ptr) { }

        public AchievementReferenceHolder[] achievementViews;
        private int _currentPage;
        private int _lastPage => Mathf.FloorToInt(Campaign.Session.Achievements.Count / achievementViews.Length);

        private GameObject nextButton;
        private GameObject backButton;
        private TMP_Text pageText;
        private TMP_Text unlockCount;

        public void SetupReferences(Button nextButton, Button backButton, TMP_Text pageText, TMP_Text unlockCount)
        {
            this.achievementViews = GetComponentsInChildren<AchievementReferenceHolder>(true);

            this.nextButton = nextButton.gameObject;
            nextButton.onClick.AddListener(new Action(() => { NextPage(); }));

            this.backButton = backButton.gameObject;
            backButton.onClick.AddListener(new Action(() => { PrevPage(); }));

            this.pageText = pageText;
            this.unlockCount = unlockCount;
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
            if(_currentPage <= 0) backButton.SetActive(false);
            else backButton.SetActive(true);

            if(_currentPage >= _lastPage) nextButton.SetActive(false);
            else nextButton.SetActive(true);

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