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
        public static void ClearAmmo()
        {
            if (LevelParsing.IsLabworksCampaign(SceneStreamer.Session.Level.Pallet.Title, SceneStreamer.Session.Level.Barcode))
                Player.rigManager.AmmoInventory.ClearAmmo();

            LabworksSaving.LoadedAmmoSaves.Clear();

            LabworksSaving.SaveToDisk();
        }

        public static int GetAmmoTotalByLevel(string levelBarcode)
        {
            LabworksSaving.AmmoSave ammoSave = LabworksSaving.LoadedAmmoSaves.Find(x => x.LevelBarcode == levelBarcode);

            return ammoSave.GetCombinedTotal();
        }

        public static LabworksSaving.AmmoSave GetAmmoFromLevel(string levelBarcode)
        {
            return LabworksSaving.LoadedAmmoSaves.Find(x => x.LevelBarcode == levelBarcode);
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

        public static LabworksSaving.AmmoSave GetPreviousLevelsAmmoSave(string levelBarcode)
        {
            int levelIndex = LevelParsing.GetLevelIndex(levelBarcode);

            LabworksSaving.AmmoSave previousLevelsAmmoSave = new();

            for (int i = 0; i < levelIndex; i++)
            {
                previousLevelsAmmoSave.LightAmmo += SaveParsing.GetSavedAmmo(LevelParsing.GetLevelBarcodeByIndex(i)).LightAmmo;
                previousLevelsAmmoSave.MediumAmmo += SaveParsing.GetSavedAmmo(LevelParsing.GetLevelBarcodeByIndex(i)).MediumAmmo;
                previousLevelsAmmoSave.HeavyAmmo += SaveParsing.GetSavedAmmo(LevelParsing.GetLevelBarcodeByIndex(i)).HeavyAmmo;
            }

            return previousLevelsAmmoSave;
        }

        public static void SaveAmmo(string levelBarcode)
        {
            LabworksSaving.AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode);

            if (!SaveParsing.DoesSavedAmmoExist(levelBarcode))
            {
                LabworksSaving.LoadedAmmoSaves.Add(new LabworksSaving.AmmoSave
                {
                    LevelBarcode = levelBarcode,
                    LightAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("light") - previousAmmoSave.LightAmmo,
                    MediumAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo,
                    HeavyAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo
                });
            } else
            {
                LabworksSaving.AmmoSave previousHighScore = GetAmmoFromLevel(levelBarcode);

                for (int i = 0; i < LabworksSaving.LoadedAmmoSaves.Count; i++)
                {
                    if (LabworksSaving.LoadedAmmoSaves[i].LevelBarcode == levelBarcode)
                    {
                        LabworksSaving.LoadedAmmoSaves[i] = new LabworksSaving.AmmoSave
                        {
                            LevelBarcode = levelBarcode,
                            LightAmmo = Math.Max(Player.rigManager.AmmoInventory.GetCartridgeCount("light") - previousAmmoSave.LightAmmo, previousHighScore.LightAmmo),
                            MediumAmmo = Math.Max(Player.rigManager.AmmoInventory.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo, previousHighScore.MediumAmmo),
                            HeavyAmmo = Math.Max(Player.rigManager.AmmoInventory.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo, previousHighScore.HeavyAmmo)
                        };
                    }
                }
            }

            LabworksSaving.SaveToDisk();
        }
    }
}
