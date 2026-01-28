using System;
using Il2CppSLZ.Marrow.Warehouse;
using HarmonyLib;
using UnityEngine;
using MelonLoader;

namespace CustomCampaignTools.Patching;

[HarmonyPatch(typeof(CrateSpawner))]
public static class CrateSpawnerAwakePatch
{
    public static bool BlockCrateSpawns = false;

    [HarmonyPatch(nameof(CrateSpawner.Awake))]
    [HarmonyPrefix]
    public static bool AwakePatch(CrateSpawner __instance)
    {
        return !BlockCrateSpawns;
    }

    [HarmonyPatch(nameof(CrateSpawner.OnPooleeSpawn))]
    [HarmonyPostfix]
    public static void PooleeSpawnPostfix(CrateSpawner __instance)
    {
        // Somehow link entities back to their cratespawner, might be a better way of doing it especially for mid-scene edits
        // Works somewhat for now though so i aint complaining
    }
}
