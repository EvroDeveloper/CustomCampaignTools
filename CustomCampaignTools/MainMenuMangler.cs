using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppTMPro;
using Il2CppUltEvents;
using MelonLoader;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CustomCampaignTools
{
    public class MainMenuMangler
    {
        public static Sprite CampaignSprite;
        public static void OnLevelLoaded(LevelInfo info)
        {
            if(info.barcode == CommonBarcodes.Maps.VoidG114)
            {
                MangleMainMenu();
            }
        }

        public static void MangleMainMenu()
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

        public static void LoadSpriteFromEmbeddedResource(string resourceName, Assembly assembly, Vector2 pivot, float pixelsPerUnit = 100f)
        {
            byte[] bytes = [];

            foreach (string resource in assembly.GetManifestResourceNames())
            {
                if (resource.Contains(resourceName))
                {
                    using Stream resFilestream = assembly.GetManifestResourceStream(resource);
                    if (resFilestream == null) return;
                    byte[] byteArr = new byte[resFilestream.Length];
                    resFilestream.Read(byteArr, 0, byteArr.Length);
                    bytes = byteArr;
                }
            }

            Texture2D texture = new(2, 2);
            if (!texture.LoadImage(bytes))
            {
                MelonLogger.Error("Failed to load texture from embedded resource.");
                return;
            }
            texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
            
            CampaignSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
            CampaignSprite.hideFlags = HideFlags.DontUnloadUnusedAsset;
        }
    }
}