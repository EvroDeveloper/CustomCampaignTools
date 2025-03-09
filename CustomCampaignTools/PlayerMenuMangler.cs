using System;
using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools
{
    public static class PlayerMenuMangler
    {
        // Much of code taken from Bonemenu, sorry!
        static PreferencesPanelView panelView;
        public static GameObject optionsPanel;

        public static Transform OptionsGrid
        {
            get
            {
                if (_optionsGrid is null || _optionsGrid.WasCollected)
                    return null;

                return _optionsGrid;
            }
        }
        private static Transform _optionsGrid;

        private static GameObject _optionButton;
        private static Button _optionButtonComponent;


        public static void Initialize()
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
            menuButtonComp.onClick.AddListener(new Action(() => FadeLoader.Load(new Barcode(Campaign.Session.MenuLevel), new Barcode(Campaign.Session.LoadScene))));
        }
    }
}