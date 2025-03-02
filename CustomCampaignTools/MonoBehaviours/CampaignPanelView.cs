using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools
{
    [RegisterTypeInIl2Cpp]
    public class CampaignPanelView : MonoBehaviour
    {
        public CampaignPanelView(IntPtr ptr) : base(ptr) {}

        private int _currentPage;
        private int _lastPage => Mathf.FloorToInt(CampaignUtilities.CampaignsToShowInMenu.Count / Buttons.Length);

        public GameObject[] Buttons;

        public GameObject backButton;
        public GameObject nextButton;

        public TMP_Text pageText;

        private bool _hasSelected = false;

        public void SetupButtons()
        {
            for(int i = 0; i < Buttons.Length; i++)
            {
                Button button = Buttons[i].GetComponent<Button>();

                button.onClick.m_PersistentCalls.Clear();
                button.onClick.m_Calls.ClearPersistent();
                button.onClick.m_Calls.Clear();
                int selection = i;
                button.onClick.AddListener(new Action(() => { Select(selection); }));
            }
        }

        public void NextPage()
        {
            _currentPage = Mathf.Min(_lastPage, _currentPage + 1);
            UpdateVisualization();
        }

        public void BackPage()
        {
            _currentPage = Mathf.Max(0, _currentPage - 1);
            UpdateVisualization();
        }

        public void Select(int index)
        {
            if(_hasSelected) return;

            MelonLogger.Msg(index);
            int campaignIndex = (Buttons.Length * _currentPage) + index;
            Campaign c = CampaignUtilities.CampaignsToShowInMenu[campaignIndex];

            _hasSelected = true;

            FadeLoader.Load(new Barcode(c.MenuLevel), new Barcode(c.LoadScene));
        }

        public void Activate()
        {
            UpdateVisualization();
        }

        private void UpdateVisualization()
        {
            if(_currentPage <= 0) backButton.SetActive(false);
            else backButton.SetActive(true);

            if(_currentPage >= _lastPage) nextButton.SetActive(false);
            else nextButton.SetActive(true);

            pageText.text = $"{_currentPage+1}/{_lastPage+1}";

            for(int i = 0; i < Buttons.Length; i++)
            {
                GameObject currentButton = Buttons[i];
                int campaignIndex = (Buttons.Length * _currentPage) + i;

                if(campaignIndex < CampaignUtilities.CampaignsToShowInMenu.Count)
                {
                    currentButton.SetActive(true);
                    currentButton.GetComponentInChildren<TMP_Text>(true).text = CampaignUtilities.CampaignsToShowInMenu[campaignIndex].Name;
                }
                else
                {
                    currentButton.SetActive(false);
                }
            }
        }
    }
}