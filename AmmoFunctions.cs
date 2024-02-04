using BoneLib;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Labworks_Ammo_Saver
{
    public static class AmmoFunctions
    {
        public static int numberOfLevels = 12;

        public static List<string> levelBarcodes = new List<string> { 
            "volx4.LabWorksBoneworksPort.Level.Boneworks01Breakroom",
            "volx4.LabWorksBoneworksPort.Level.Boneworks02Museum",
            "volx4.LabWorksBoneworksPort.Level.Boneworks03Streets",
            "volx4.LabWorksBoneworksPort.Level.Boneworks04Runoff",
            "volx4.LabWorksBoneworksPort.Level.Boneworks05Sewers",
            "volx4.LabWorksBoneworksPort.Level.Boneworks06Warehouse",
            "volx4.LabWorksBoneworksPort.Level.Boneworks07CentralStation",
            "volx4.LabWorksBoneworksPort.Level.Boneworks08Tower",
            "volx4.LabWorksBoneworksPort.Level.Boneworks09TimeTower",
            "volx4.LabWorksBoneworksPort.Level.Boneworks10Dungeon",
            "volx4.LabWorksBoneworksPort.Level.Boneworks11Arena" };

        public static void SaveAmmo(int levelIndex)
        {
            var previousAmmo = GetPreviousLevelsAmmo(levelIndex);

            if(BonelibCreator.ammo.Value.Count == 0)
            {
                BonelibCreator.ammo.Value = new List<int>(numberOfLevels * 3) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }

            BonelibCreator.ammo.Value[levelIndex * 3] = Player.rigManager.AmmoInventory.GetCartridgeCount("light") - previousAmmo.x;
            BonelibCreator.ammo.Value[levelIndex * 3 + 1] = Player.rigManager.AmmoInventory.GetCartridgeCount("medium") - previousAmmo.y;
            BonelibCreator.ammo.Value[levelIndex * 3 + 2] = Player.rigManager.AmmoInventory.GetCartridgeCount("heavy") - previousAmmo.z;

            BonelibCreator.ammo.Save();
        }

        public static void ClearAmmo()
        {
            Player.rigManager.AmmoInventory.ClearAmmo();

            BonelibCreator.ammo.Value = new List<int>(3 * numberOfLevels);

            BonelibCreator.ammo.Save();
        }

        public static int GetAmmoTotalByLevel(int level)
        {
            List<int> ammoCount = BonelibCreator.ammo.Value;

            return ammoCount[level * 3] + ammoCount[level * 3 + 1] + ammoCount[level * 3 + 2];
        }

        public static Vector3Int GetPreviousLevelsAmmo(int currentLevel)
        {
            Vector3Int output = Vector3Int.zero;

            MelonLogger.Msg("Accumulating Previous Ammo for Calculations, Level Saving from is " + levelBarcodes[currentLevel]);

            if (currentLevel == 0) return output;

            List<int> ammoCount = BonelibCreator.ammo.Value;

            for (int i = 0; i < currentLevel; i++)
            {
                output.x += ammoCount[i * 3];
                output.y += ammoCount[i * 3 + 1];
                output.z += ammoCount[i * 3 + 2];

                MelonLogger.Msg("Added ammo from level, " + levelBarcodes[i]);
            }

            MelonLogger.Msg("Previous Ammo Accumulation complete, the previous levels ammo is " + output.ToString());

            return output;
        }

        public static void LoadAmmoAtLevel(int levelIndex)
        {
            for(int i = 0; i < levelIndex; i++)
            {
                GiveAmmoForLevel(i);
            }
        }

        private static void GiveAmmoForLevel(int levelIndex)
        {
            List<int> ammoCount = BonelibCreator.ammo.Value;

            Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.lightAmmoGroup, ammoCount[levelIndex * 3]);
            Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.mediumAmmoGroup, ammoCount[levelIndex * 3 + 1]);
            Player.rigManager.AmmoInventory.AddCartridge(Player.rigManager.AmmoInventory.heavyAmmoGroup, ammoCount[levelIndex * 3 + 2]);
        }

        public static int GetLevelIndexFromBarcode(string barcode)
        {
            MelonLogger.Msg("Index got from Barcode, returning " + levelBarcodes.IndexOf(barcode));
            return levelBarcodes.IndexOf(barcode);
        }
    }
}
