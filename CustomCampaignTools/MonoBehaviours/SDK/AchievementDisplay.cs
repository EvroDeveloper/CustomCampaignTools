#if MELONLOADER
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppInterop.Runtime.Attributes;
using Il2CppTMPro;
using MelonLoader;
#else
using TMPro;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class AchievementDisplay : MonoBehaviour
    {
#if MELONLOADER
        public AchievementDisplay(IntPtr ptr) : base(ptr) { }

        public AchievementReferenceHolder[] achievementViews;
        private int _currentPage;
        private int _lastPage => Mathf.FloorToInt(Campaign.Session.Achievements.Count / achievementViews.Length);

        public Il2CppReferenceField<Button> nextButton;
        public Button NextButton => nextButton.Get();
        public Il2CppReferenceField<Button> backButton;
        public Button BackButton => backButton.Get();
        public Il2CppReferenceField<TMP_Text> pageText;
        public TMP_Text PageText => pageText.Get();
        public Il2CppReferenceField<TMP_Text> unlockCount;
        public TMP_Text UnlockCount => unlockCount.Get();


        private void UpdateVisualization()
        {
            if (_currentPage <= 0) BackButton.gameObject.SetActive(false);
            else BackButton.gameObject.SetActive(true);

            if (_currentPage >= _lastPage) NextButton.gameObject.SetActive(false);
            else NextButton.gameObject.SetActive(true);

            if (PageText != null)
                PageText.text = $"{_currentPage + 1}/{_lastPage + 1}";

            for (int i = 0; i < achievementViews.Length; i++)
            {
                var currentView = achievementViews[i];
                int achievementIndex = (achievementViews.Length * _currentPage) + i;

                if (achievementIndex < Campaign.Session.Achievements.Count)
                {
                    currentView.gameObject.SetActive(true);
                    currentView.ShowAchievement(Campaign.Session.Achievements[achievementIndex]);
                }
                else
                {
                    currentView.gameObject.SetActive(false);
                }
            }

            if (UnlockCount != null)
                UnlockCount.text = $"{Campaign.Session.saveData.UnlockedAchievements.Count}/{Campaign.Session.Achievements.Count} Achievements Unlocked";
        }
#else 
        public Button nextButton;
        public Button backButton;
        public TMP_Text pageText;
        public TMP_Text unlockCount;
#endif

        public void Awake()
        {
#if MElONLOADER
            achievementViews = GetComponentsInChildren<AchievementReferenceHolder>(true);
            nextButton.Get().onClick.AddListener(new Action(NextPage));
            backButton.Get().onClick.AddListener(new Action(PrevPage));
#endif
        }

        public void Activate()
        {
#if MELONLOADER
            UpdateVisualization();
#endif
        }

        public void NextPage()
        {
#if MELONLOADER
            _currentPage = Mathf.Min(_lastPage, _currentPage + 1);
            UpdateVisualization();
#endif
        }
        public void PrevPage()
        {
#if MELONLOADER
            _currentPage = Mathf.Max(0, _currentPage - 1);
            UpdateVisualization();
#endif
        }
    }
}