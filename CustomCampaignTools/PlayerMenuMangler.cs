using System;
using SLZ.Bonelab;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools
{
    public static class PlayerMenuMangler()
    {
        // Much of code taken from Bonemenu, sorry!
        PreferencesPanelView panelView;
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


        public static void Initialize()
        {
            if(!Campaign.SessionActive) return;

            panelView = Player.UIRig.popUpMenu.preferencesPanelView;
            optionsPanel = panelView.pages[panelView.defaultPage];
            _optionsGrid = optionsPanel.transform.Find("grid_Options");

            // need to ensure i'm copying a correct one
            _optionButton = GameObject.Instantiate(_optionsGrid.GetChild(5).gameObject, _optionsGrid);
            _optionButton.SetActive(true);

            var tmp = _optionButton.GetComponentInChildren<TMP_Text>(true);
            tmp.text = "Exit Campaign";

            _optionButton.transform.SetSiblingIndex(_optionsGrid.childCount - 1);

            _optionButtonComponent = _optionButton.GetComponent<Button>();
            _optionButtonComponent.onClick.RemoveAllListeners();
            _optionButtonComponent.onClick.AddListener(new System.Action(() => Campaign.Session.Exit()));
        }
    }
}