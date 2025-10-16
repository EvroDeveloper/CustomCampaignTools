using Il2CppSLZ.Marrow;
using MelonLoader;
using HarmonyLib;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(TimeManager))]
    public static class TimeManagerPatches
    {
        [HarmonyPatch(nameof(TimeManager.PAUSE))]
        [HarmonyPostfix]
        public static void OnPause()
        {
            LevelTiming.ONGAMEPAUSE();
        }

        [HarmonyPatch(nameof(TimeManager.UNPAUSE))]
        [HarmonyPostfix]
        public static void OnUnpause()
        {
            LevelTiming.ONGAMERESUME();
        }
    }
}