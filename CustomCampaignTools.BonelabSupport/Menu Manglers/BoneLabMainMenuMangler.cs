using System;
using System.Reflection;
using BoneLib;
using CustomCampaignTools.Debug;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.Utilities;
using Il2CppSLZ.Bonelab;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools.BonelabSupport;

public class BoneLabMainMenuMangler : IMenuMangler
{
    public static Sprite CampaignSprite;

    public void MangleMenu()
    {
        var LevelsGrid = GameObject.Find("CANVAS_UX").transform.Find("MENU").GetChild(8).gameObject;
        var CampaignGrid = GameObject.Instantiate(LevelsGrid, LevelsGrid.transform.parent);
        CampaignGrid.name = "group_CAMPAIGNS";
        CampaignGrid.transform.localPosition = Vector3.zero;
        CampaignGrid.transform.localRotation = LevelsGrid.transform.localRotation;
        CampaignGrid.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "CAMPAIGNS";

        BonelabLevelsPanelView oldPanelComponent = CampaignGrid.GetComponentInChildren<BonelabLevelsPanelView>(true);
        CampaignPanelView cPanel = oldPanelComponent.gameObject.AddComponent<CampaignPanelView>();

        cPanel.Buttons = oldPanelComponent.items;
        cPanel.nextButton = oldPanelComponent.forwardButton;
        cPanel.backButton = oldPanelComponent.backButton;
        cPanel.pageText = oldPanelComponent.pageText;

        cPanel.SetupButtons();

        UnityEngine.Object.Destroy(oldPanelComponent);


        // Clone a menu button and edit it's text and icon
        var MainGrid = GameObject.Find("CANVAS_UX").transform.Find("MENU").GetChild(3).gameObject;
        var targetButton = MainGrid.transform.GetChild(4).gameObject;

        var newButton = GameObject.Instantiate(targetButton, targetButton.transform.parent);

        newButton.GetComponentInChildren<TMP_Text>(true).text = "CAMPAIGNS";
        var onClick = newButton.GetComponentInChildren<Button>(true).onClick;
        onClick.m_PersistentCalls.Clear();
        onClick.m_Calls.ClearPersistent();
        onClick.m_Calls.Clear();
        onClick.AddListener(new Action(() => { MainGrid.SetActive(false); CampaignGrid.SetActive(true); cPanel.Activate(); }));
        var uiImage = newButton.GetComponentInChildren<Button>(true).targetGraphic.GetComponent<Image>();

        if (CampaignSprite != null && uiImage != null)
        {
            uiImage.sprite = CampaignSprite;
        }

        CampaignGrid.SetActive(false);

        // Add a CampaignSelectionView to choose to load intomenu or continue n shit
    }
}