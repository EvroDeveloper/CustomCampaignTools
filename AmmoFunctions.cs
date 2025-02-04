using BoneLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using Il2CppSLZ.Utilities;
using Il2CppSLZ.Marrow;
using UnityEngine;
using CustomCampaignTools.Utilities;
using CustomCampaignTools;
using Il2CppSLZ.Marrow.SceneStreaming;

namespace CustomCampaignTools
{
    public static class AmmoFunctions
    {
        public static void ClearAmmo(Campaign campaign)
        {
            if (LevelParsing.IsCampaignLevel(SceneStreamer.Session.Level.Barcode.ID, out _, out _))
                AmmoInventory.Instance.ClearAmmo();

            campaign.saveData.LoadedAmmoSaves.Clear();

            campaign.saveData.SaveToDisk();
        }

        public static void LoadAmmoFromLevel(string levelBarcode, bool isLoadCheckpoint)
        {
            if(!CampaignUtilities.IsCampaignLevel(levelBarcode, out Campaign campaign, out CampaignLevelType levelType)) return;

            if(!campaign.SaveLevelAmmo) return;

            if(levelType != CampaignLevelType.MainLevel) return;
            

            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            try
            {
                AmmoInventory.Instance.ClearAmmo();
            }
            catch
            {
            }

            string[] mainLevels = campaign.MainLevels;

            for (int i = 0; i < levelIndex; i++)
            {
                level = mainLevels[i];
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, campaign.saveData.GetSavedAmmo(level).LightAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, campaign.saveData.GetSavedAmmo(level).MediumAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, campaign.saveData.GetSavedAmmo(level).HeavyAmmo);
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(AmmoInventory), nameof(AmmoInventory.Awake))]
    public static class AmmoInventoryAwake
    {
        public static void Postfix()
        {
            var levelBarcode = SceneStreamer.Session.Level.Barcode.ID;

            if (!CampaignUtilities.IsCampaignLevel(levelBarcode, out Campaign campaign, out CampaignLevelType levelType)) return;

            if (levelType != CampaignLevelType.MainLevel) return;

            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            AmmoInventory.Instance.ClearAmmo();

            for (int i = 0; i < levelIndex; i++)
            {
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).LightAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).MediumAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).HeavyAmmo);
            }
        }
    }
}
