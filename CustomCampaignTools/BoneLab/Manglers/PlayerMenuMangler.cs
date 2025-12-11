using System;
using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools.Games.BoneLab
{
    public class BoneLabPlayerMenuMangler : IMenuMangler
    {
        // Much of code taken from Bonemenu, sorry!
        PreferencesPanelView panelView;
        public GameObject optionsPanel;

        public Transform OptionsGrid
        {
            get
            {
                if (_optionsGrid is null || _optionsGrid.WasCollected)
                    return null;

                return _optionsGrid;
            }
        }
        private Transform _optionsGrid;

        private GameObject _optionButton;
        private Button _optionButtonComponent;

        public void MangleMenu()
        {
            if(!Campaign.SessionActive) return;

            panelView = Player.UIRig.popUpMenu.preferencesPanelView;
            optionsPanel = panelView.pages[panelView.defaultPage];
            _optionsGrid = optionsPanel.transform.Find("grid_Options");

            // need to ensure i'm copying a correct one
            _optionButton = GameObject.Instantiate(_optionsGrid.GetChild(4).gameObject, _optionsGrid);
            _optionButton.SetActive(true);

            var tmp = _optionButton.GetComponentInChildren<TMP_Text>(true);
            tmp.text = "Exit Campaign";

            _optionButton.transform.SetSiblingIndex(_optionsGrid.childCount - 1);

            _optionButtonComponent = _optionButton.GetComponent<Button>();
            _optionButtonComponent.onClick.m_PersistentCalls.Clear();
            _optionButtonComponent.onClick.m_Calls.ClearPersistent();
            _optionButtonComponent.onClick.m_Calls.Clear();
            _optionButtonComponent.onClick.AddListener(new Action(Campaign.Session.Exit));

            // Fix name later
            var menuButton = _optionsGrid.Find("button_MainMenu");
            var menuButtonComp = menuButton.Find("panel_MAINMENU").GetComponentInChildren<Button>(true);
            menuButtonComp.onClick.m_PersistentCalls.Clear();
            menuButtonComp.onClick.m_Calls.ClearPersistent();
            menuButtonComp.onClick.m_Calls.Clear();
            menuButtonComp.onClick.AddListener(new Action(() => FadeLoader.Load(Campaign.Session.MenuLevel, new Barcode(Campaign.Session.LoadScene))));
        }
    }
}