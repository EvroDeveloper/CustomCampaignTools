using SLZ.Marrow.SceneStreaming;
using SLZ.Marrow.Warehouse;
using HarmonyLib;
using MelonLoader;
using BoneLib;

namespace Labworks_Ammo_Saver
{
    [HarmonyPatch(typeof(SceneStreamer), "Load", typeof(LevelCrateReference), typeof(LevelCrateReference))]
    public static class OnLoadPatch1
    {
        [HarmonyPrefix]
        public static void Load(LevelCrateReference level, LevelCrateReference loadLevel)
        {
            string previousTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcode = SceneStreamer.Session.Level.Barcode;
            if (previousTitle == "LabWorksBoneworksPort" && barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksRedactedChamber" && barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksMainMenu" && barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksLoadingScreen")
            {
                MelonLogger.Msg("Load Patch Triggered, running Save Ammo with parameter " + AmmoFunctions.GetLevelIndexFromBarcode(barcode) + " where the barcode is " + barcode);
                AmmoFunctions.SaveAmmo(AmmoFunctions.GetLevelIndexFromBarcode(barcode));
            }
        }
    }

    [HarmonyPatch(typeof(SceneStreamer), "Load", typeof(string), typeof(string))]
    public static class OnLoadPatch2
    {
        [HarmonyPrefix]
        public static void Load(string level, string loadLevel)
        {
            string previousTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcode = SceneStreamer.Session.Level.Barcode;
            if (previousTitle == "LabWorksBoneworksPort" && barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksRedactedChamber" && barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksMainMenu" && barcode != "volx4.LabWorksBoneworksPort.Level.BoneworksLoadingScreen")
            {
                AmmoFunctions.SaveAmmo(AmmoFunctions.GetLevelIndexFromBarcode(barcode));
            }
        }
    }
}
