using System;
using BoneLib;
using CustomCampaignTools.BonelabSupport;
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
            var optionButtonsParent = panelView.pages[panelView.defaultPage].transform.Find("grid_Options");

            if (!CampaignForcing.forcedCampaign)
            {
                var exitCampaignButton = GameObject.Instantiate(optionButtonsParent.GetChild(4).gameObject, optionButtonsParent);
                exitCampaignButton.SetActive(true);
                exitCampaignButton.transform.SetSiblingIndex(optionButtonsParent.childCount - 1);
                exitCampaignButton.GetComponentInChildren<TMP_Text>(true).text = "Exit Campaign";
                exitCampaignButton.GetComponent<Button>().onClick.ClearAndSetCallback(new Action(Campaign.Exit));

                if (BoneLabGameConfiguration.LogoSprite != null)
                {
                    var spriteRenderer = exitCampaignButton.transform.GetChild(3).GetComponent<Image>();
                    spriteRenderer.sprite = BoneLabGameConfiguration.LogoSprite;
                }
            }

            optionButtonsParent.Find("button_MainMenu/panel_MAINMENU").GetComponentInChildren<Button>(true).onClick.ClearAndSetCallback(new Action(() => FadeLoader.Load(Campaign.Session.MenuLevel, Campaign.Session.LoadScene)));
        }
    }
}