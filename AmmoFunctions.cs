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

            return ammoSave.LightAmmo + ammoSave.MediumAmmo + ammoSave.HeavyAmmo;
        }

        public static void LoadAmmoAtLevel(string levelBarcode)
        {
            foreach (var savedAmmo in LabworksSaving.LoadedAmmoSaves)
            {
                if (savedAmmo.LevelBarcode == levelBarcode)
                {
                    Player.rigManager.AmmoInventory.ClearAmmo();

                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.lightAmmoGroup, savedAmmo.LightAmmo);
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.mediumAmmoGroup, savedAmmo.MediumAmmo);
                    Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.heavyAmmoGroup, savedAmmo.HeavyAmmo);
                }
            }
        }

        public static void SaveAmmo(string levelBarcode)
        {
            if (!SaveParsing.DoesSavedAmmoExist(levelBarcode))
            {
                LabworksSaving.LoadedAmmoSaves.Add(new LabworksSaving.AmmoSave
                {
                    LevelBarcode = levelBarcode,
                    LightAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("light"),
                    MediumAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("medium"),
                    HeavyAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("heavy")
                });
            }
            else
            {
                for (int i = 0; i < LabworksSaving.LoadedAmmoSaves.Count; i++)
                {
                    if (LabworksSaving.LoadedAmmoSaves[i].LevelBarcode == levelBarcode)
                    {
                        LabworksSaving.LoadedAmmoSaves[i] = new LabworksSaving.AmmoSave
                        {
                            LevelBarcode = levelBarcode,
                            LightAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("light"),
                            MediumAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("medium"),
                            HeavyAmmo = Player.rigManager.AmmoInventory.GetCartridgeCount("heavy")
                        };
                    }
                }
            }

            LabworksSaving.SaveToDisk();
        }
    }
}
