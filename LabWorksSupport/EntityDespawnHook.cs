using System;
using HarmonyLib;
using Il2CppSLZ.Marrow.Interaction;

namespace LabWorksSupport;

[HarmonyPatch(typeof(MarrowEntity))]
public class EntityDespawnHook
{
    [HarmonyPatch(nameof(MarrowEntity.Despawn))]
    [HarmonyPrefix]
    public static bool OnBeforeDespawn(MarrowEntity __instance)
    {
        //if(DespawnMeshVFX.Cache.TryGet(__instance.gameObject, out var despawner))
        if (__instance.TryGetComponent(out DespawnMeshVFX despawner))
        {
            // Despawn via DespawnMeshVFX rather than Entity
            despawner.Despawn();
            return false;
        }
        else
        {
            return true;
        }
    }
}
