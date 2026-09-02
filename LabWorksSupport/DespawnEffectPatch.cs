using System;
using CustomCampaignTools;
using HarmonyLib;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.VFX;
using UnityEngine;

namespace LabWorksSupport;

[HarmonyPatch(typeof(SpawnEffects))]
public class SpawnEffectsPatch
{
    [HarmonyPatch(nameof(SpawnEffects.CallDespawnEffect))]
    [HarmonyPrefix]
    public static bool CallDespawnEffectPrefix(MarrowEntity ThisEntity)
    {
        if (Campaign.SessionActive && Campaign.Session.Name == "LabWorks")
        {
            DespawnMeshVFX.DespawnEntity(ThisEntity);
            SpawnEffects.FireSFXAsync(ThisEntity, MarrowSettings.RuntimeInstance.DespawnSFX, 0.5f);
            return false;
        }
        else
        {
            if (ThisEntity.TryGetComponent(out DespawnMeshVFX despawner))
            {
                despawner.Despawn();
                SpawnEffects.FireSFXAsync(ThisEntity, MarrowSettings.RuntimeInstance.DespawnSFX, 0.5f);
                return false;
            }
            return true;
        }
    }
}
