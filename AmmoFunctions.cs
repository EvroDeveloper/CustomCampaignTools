using BoneLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using SLZ.Utilities;
using UnityEngine;
using Labworks.Data;
using Labworks.Utilities;
using SLZ.Marrow.SceneStreaming;

namespace Labworks
{
    public static class AmmoFunctions
    {
        public static void ClearAmmo(Campaign campaign)
        {
            if (LevelParsing.IsCampaignLevel(SceneStreamer.Session.Level.Pallet.Title, SceneStreamer.Session.Level.Barcode))
                Player.rigManager.AmmoInventory.ClearAmmo();

            campaign.saveData.LoadedAmmoSaves.Clear();

            campaign.saveData.SaveToDisk();
        }

        public static int GetAmmoTotalByLevel(Campaign campaign, string levelBarcode)
        {
            CampaignSaveData.AmmoSave ammoSave = campaign.saveData.LoadedAmmoSaves.Find(x => x.LevelBarcode == levelBarcode);

            return ammoSave.GetCombinedTotal();
        }

        public static CampaignSaveData.AmmoSave GetAmmoFromLevel(Campaign campaign, string levelBarcode)
        {
            return campaign.saveData.LoadedAmmoSaves.Find(x => x.LevelBarcode == levelBarcode);
        }

        public static void LoadAmmoFromLevel(string levelBarcode, bool isLoadCheckpoint)
        {
            int levelIndex = LevelParsing.GetLevelIndex(levelBarcode);

            Player.rigManager.AmmoInventory.ClearAmmo();

            if (!isLoadCheckpoint)
                for (int i = 0; i < levelIndex; i++)
                {
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.lightAmmoGroup, GetAmmoFromLevel(LevelParsing.GetLevelBarcodeByIndex(i)).LightAmmo);
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.mediumAmmoGroup, GetAmmoFromLevel(LevelParsing.GetLevelBarcodeByIndex(i)).MediumAmmo);
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.heavyAmmoGroup, GetAmmoFromLevel(LevelParsing.GetLevelBarcodeByIndex(i)).HeavyAmmo);
                }
            else
                for (int i = 0; i <= levelIndex; i++)
                {
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.lightAmmoGroup, GetAmmoFromLevel(LevelParsing.GetLevelBarcodeByIndex(i)).LightAmmo);
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.mediumAmmoGroup, GetAmmoFromLevel(LevelParsing.GetLevelBarcodeByIndex(i)).MediumAmmo);
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.heavyAmmoGroup, GetAmmoFromLevel(LevelParsing.GetLevelBarcodeByIndex(i)).HeavyAmmo);
                }
        }

        public static CampaignSaveData.AmmoSave GetPreviousLevelsAmmoSave(Campaign campaign, string levelBarcode)
        {
            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            CampaignSaveData.AmmoSave previousLevelsAmmoSave = new();

            for (int i = 0; i < levelIndex; i++)
            {
                previousLevelsAmmoSave.LightAmmo += SaveParsing.GetSavedAmmo(LevelParsing.GetLevelBarcodeByIndex(campaign, i)).LightAmmo;
                previousLevelsAmmoSave.MediumAmmo += SaveParsing.GetSavedAmmo(LevelParsing.GetLevelBarcodeByIndex(campaign, i)).MediumAmmo;
                previousLevelsAmmoSave.HeavyAmmo += SaveParsing.GetSavedAmmo(LevelParsing.GetLevelBarcodeByIndex(campaign, i)).HeavyAmmo;
            }

            return previousLevelsAmmoSave;
        }

        [Obsolete("Save ammo via CampaignSaveData.SaveAmmoFromLevel")]
        public static void SaveAmmo(Campaign campaign, string levelBarcode)
        {
            CampaignSaveData.AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(campaign, levelBarcode);

            if (!SaveParsing.DoesSavedAmmoExist(campaign, levelBarcode))
            {
                LabworksSaving.LoadedAmmoSaves.Add(new CampaignSaveData.AmmoSave
                {
                    LevelBarcode = levelBarcode,
                    LightAmmo = AmmoInventory.Instance.GetCartridgeCount("light") - previousAmmoSave.LightAmmo,
                    MediumAmmo = AmmoInventory.Instance.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo,
                    HeavyAmmo = AmmoInventory.Instance.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo
                });
            } else
            {
                CampaignSaveData.AmmoSave previousHighScore = GetAmmoFromLevel(levelBarcode);

                for (int i = 0; i < campaign.saveData.LoadedAmmoSaves.Count; i++)
                {
                    if (campaign.saveData.LoadedAmmoSaves[i].LevelBarcode == levelBarcode)
                    {
                        campaign.saveData.LoadedAmmoSaves[i] = new CampaignSaveData.AmmoSave
                        {
                            LevelBarcode = levelBarcode,
                            LightAmmo = Math.Max(Player.rigManager.AmmoInventory.GetCartridgeCount("light") - previousAmmoSave.LightAmmo, previousHighScore.LightAmmo),
                            MediumAmmo = Math.Max(Player.rigManager.AmmoInventory.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo, previousHighScore.MediumAmmo),
                            HeavyAmmo = Math.Max(Player.rigManager.AmmoInventory.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo, previousHighScore.HeavyAmmo)
                        };
                    }
                }
            }

            campaign.saveData.SaveToDisk();
        }
    }
}
