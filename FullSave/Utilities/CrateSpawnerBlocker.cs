using System;
using FullSave.MonoBehaviours;
using HarmonyLib;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;

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

    [HarmonyPatch(nameof(CrateSpawner.OnPooleeSpawn))]
    [HarmonyPostfix]
    public static void ApplyLinkToEntity(CrateSpawner __instance, GameObject go)
    {
        if(go.TryGetComponent<MarrowEntity>(out _))
        {
            go.AddComponent<EntitySpawnerLink>().birthgiver = __instance;
        }
    }
}
