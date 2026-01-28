using System;
using Il2CppSLZ.Marrow.Warehouse;

namespace CustomCampaignTools.Patching;

[HarmonyLib.HarmonyPatch(typeof(CrateSpawner))]
public static class CrateSpawnerAwakePatch
{
    public static bool BlockCrateSpawns = false;

    [HarmonyLib.HarmonyPatch(nameof(CrateSpawner.Awake))]
    [HarmonyLib.HarmonyPrefix]
    public static bool AwakePatch(CrateSpawner __instance)
    {
        return !BlockCrateSpawns;
    }
}
