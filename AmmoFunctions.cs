using BoneLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using Il2CppSLZ.Utilities;
using Il2CppSLZ.Marrow;
using UnityEngine;
using Labworks.Utilities;
using CustomCampaignTools;
using Il2CppSLZ.Marrow.SceneStreaming;

namespace Labworks
{
    public static class AmmoFunctions
    {
        public static void ClearAmmo(Campaign campaign)
        {
            if (LevelParsing.IsCampaignLevel(SceneStreamer.Session.Level.Pallet.Title, SceneStreamer.Session.Level.Barcode.ID, out _))
                AmmoInventory.Instance.ClearAmmo();

            campaign.saveData.LoadedAmmoSaves.Clear();

            campaign.saveData.SaveToDisk();
        }

        public static void LoadAmmoFromLevel(string levelBarcode, bool isLoadCheckpoint)
        {
            if(!LevelParsing.IsCampaignLevel("", levelBarcode, out Campaign campaign)) return;
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
