using System;
using HarmonyLib;
using Il2CppSLZ.Marrow;

namespace FullSave.Utilities;

[HarmonyPatch(typeof(AmmoInventory))]
public static class AmmoInventoryPatches
{
    public static Action OnNextAmmoInventoryAwake;

    [HarmonyPatch(nameof(AmmoInventory.Awake))]
    [HarmonyPostfix]
    public static void AwakePostfix(AmmoInventory __instance)
    {
        OnNextAmmoInventoryAwake.Invoke();
        OnNextAmmoInventoryAwake = null;
    }
}
