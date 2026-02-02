using System;
using System.Collections;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using MelonLoader;
using UnityEngine;

namespace FullSave.Utilities;

[HarmonyPatch(typeof(RigManager))]
public static class TeleportBlocker
{
    public static bool BlockTeleports = false;

    [HarmonyPatch(nameof(RigManager.Teleport), argumentTypes: [typeof(Vector3), typeof(bool)])]
    [HarmonyPrefix]
    public static bool Patch1()
    {
        return !BlockTeleports;
    }
    [HarmonyPatch(nameof(RigManager.Teleport), argumentTypes: [typeof(Vector3), typeof(Vector3), typeof(bool)])]
    [HarmonyPrefix]
    public static bool Patch2()
    {
        return !BlockTeleports;
    }
    [HarmonyPatch(nameof(RigManager.Teleport), argumentTypes: [typeof(Transform), typeof(bool)])]
    [HarmonyPrefix]
    public static bool Patch3()
    {
        return !BlockTeleports;
    }

    public static void BlockTeleportsForDelay()
    {
        BlockTeleports = true;
        MelonCoroutines.Start(CoBlockTeleportsDelay());
    }

    private static IEnumerator CoBlockTeleportsDelay()
    {
        yield return new WaitForSeconds(2);
        BlockTeleports = false;
    }
}
