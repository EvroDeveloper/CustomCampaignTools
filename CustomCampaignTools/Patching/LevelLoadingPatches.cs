using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Warehouse;
using HarmonyLib;
using MelonLoader;
using BoneLib;
using Labworks.Utilities;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(SceneStreamer))]
    public static class LevelLoadingPatches
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
            string barcode = SceneStreamer.Session.Level.Barcode.ID;

            if (LevelParsing.IsCampaignLevel(previousTitle, barcode, out Campaign campaign))
            {
#if DEBUG
                MelonLogger.Msg("Current level is a Campaign! Saving ammo to Save Data...");
#endif
                campaign.saveData.SaveAmmoForLevel(barcode);
            }
        }
    }
}
