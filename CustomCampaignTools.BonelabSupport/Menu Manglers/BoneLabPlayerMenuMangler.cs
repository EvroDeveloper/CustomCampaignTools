using System;
using BoneLib;
using CustomCampaignTools.Utilities;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools.GameSupport.BoneLab
{
    public static class BoneLabPlayerMenuMangler
    {
        public static void MangleMenu()
        {
            if(!Campaign.SessionActive) return;

            var panelView = Player.UIRig.popUpMenu.preferencesPanelView;
            var optionsPanel = panelView.pages[panelView.defaultPage];
            var _optionsGrid = optionsPanel.transform.Find("grid_Options");

            if (!CampaignForcing.forcedCampaign)
            {

                // need to ensure i'm copying a correct one
                var _optionButton = GameObject.Instantiate(_optionsGrid.GetChild(4).gameObject, _optionsGrid);
                _optionButton.SetActive(true);

                var tmp = _optionButton.GetComponentInChildren<TMP_Text>(true);
                tmp.text = "Exit Campaign";

                _optionButton.transform.SetSiblingIndex(_optionsGrid.childCount - 1);

                var _optionButtonComponent = _optionButton.GetComponent<Button>();
                _optionButtonComponent.onClick.m_PersistentCalls.Clear();
                _optionButtonComponent.onClick.m_Calls.ClearPersistent();
                _optionButtonComponent.onClick.m_Calls.Clear();
                _optionButtonComponent.onClick.AddListener(new Action(Campaign.Exit));
            }

            // Fix name later
            var menuButton = _optionsGrid.Find("button_MainMenu");
            var menuButtonComp = menuButton.Find("panel_MAINMENU").GetComponentInChildren<Button>(true);
            menuButtonComp.onClick.m_PersistentCalls.Clear();
            menuButtonComp.onClick.m_Calls.ClearPersistent();
            menuButtonComp.onClick.m_Calls.Clear();
            menuButtonComp.onClick.AddListener(new Action(() => FadeLoader.Load(Campaign.Session.MenuLevel, Campaign.Session.LoadScene)));
        }
    }
}