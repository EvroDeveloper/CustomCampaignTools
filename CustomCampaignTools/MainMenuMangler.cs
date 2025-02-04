using BoneLib;
using Il2CppSLZ.Bonelab;
using Il2CppTMPro;
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
            CampaignGrid.transform.localPosition = Vector3.zero;
            CampaignGrid.transform.localRotation = LevelsGrid.transform.localRotation;

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

            if (Main.CampaignSprite != null && uiImage != null)
            {
                uiImage.sprite = Main.CampaignSprite;
            }

            CampaignGrid.SetActive(false);
        }

        public static Sprite LoadSpriteFromEmbeddedResource(string resourceName, Assembly assembly, Vector2 pivot, float pixelsPerUnit = 100f)
        {
            // Get the embedded resource stream
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                {
                    Debug.LogError($"Resource '{resourceName}' not found in assembly '{assembly.FullName}'.");
                    return null;
                }

                // Read the resource into a byte array
                byte[] resourceData = new byte[resourceStream.Length];
                resourceStream.Read(resourceData, 0, resourceData.Length);

                // Convert the byte array into a Texture2D
                Texture2D texture = new Texture2D(2, 2);
                if (!texture.LoadImage(resourceData))
                {
                    Debug.LogError("Failed to load texture from embedded resource.");
                    return null;
                }

                // Create a Sprite from the Texture2D
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelsPerUnit);
                return sprite;
            }
        }
    }
}