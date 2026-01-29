using System;
using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;

namespace FullSave.Utilities;

[HarmonyPatch(typeof(CrateSpawner))]
public static class CrateSpawnerBlocker
{
    public static bool BlockCrateSpawns = false;

    [HarmonyPatch(nameof(CrateSpawner.Awake))]
    [HarmonyPrefix]
    public static bool SpawnerAwakePatch()
    {
        return !BlockCrateSpawns;
    }
}
