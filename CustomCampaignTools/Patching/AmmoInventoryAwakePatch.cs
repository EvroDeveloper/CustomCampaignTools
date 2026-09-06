using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using HarmonyLib;
using System;
using CustomCampaignTools.Utilities;
using CustomCampaignTools.Utilities.Patching;

namespace CustomCampaignTools.Patching;

public static class AmmoInventoryPatches
{
    public static event Action<AmmoInventory> OnNextAwake = (a) => { };

    [CampaignPatch(typeof(AmmoInventory), nameof(AmmoInventory.Awake), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void AwakePostfix(AmmoInventory __instance)
    {
        if (CampaignLevel.Session is not MainLevel mainLevel) return;

        AmmoInventory.Instance.ClearAmmo();

        Campaign campaign = Campaign.Session;
        
        // Accumulate ammo saves from previous levels
        for (int i = 0; i < mainLevel.LevelIndex; i++)
        {
            campaign.saveData.GetSavedAmmo(campaign.MainLevels[i].Barcode).AddToPlayer();
        }

        OnNextAwake.Invoke(__instance);
        OnNextAwake = (a) => { }; // clear it
    }
}
