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
}
