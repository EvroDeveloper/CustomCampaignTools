using SLZ.Marrow.SceneStreaming;
using SLZ.Marrow.Warehouse;
using HarmonyLib;
using MelonLoader;
using BoneLib;
using Labworks.Utilities;

namespace Labworks.Patching
{
    [HarmonyPatch(typeof(SceneStreamer))]
    public static class LevelLoadPatches
    {
        [HarmonyPatch(nameof(SceneStreamer.Load), typeof(string), typeof(string))]
        [HarmonyPrefix]
        public static void StringLoad(string levelBarcode, string loadLevelBarcode = "")
        {
            PreLoadSave();
        }

        [HarmonyPatch(nameof(SceneStreamer.Load), typeof(LevelCrateReference), typeof(LevelCrateReference))]
        [HarmonyPrefix]
        public static void CrateLoad(LevelCrateReference level, LevelCrateReference loadLevel)
        {
            PreLoadSave();
        }

        private static void PreLoadSave()
        {
            string previousTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcode = SceneStreamer.Session.Level.Barcode;

            if (LevelParsing.IsLabworksCampaign(previousTitle, barcode))
            {
#if DEBUG
                MelonLogger.Msg("Current level is Labworks! Saving ammo...");
#endif
                AmmoFunctions.SaveAmmo(barcode);
            }
        }
    }
}
