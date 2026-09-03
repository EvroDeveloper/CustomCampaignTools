using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using HarmonyLib;
using System;
using CustomCampaignTools.Utilities;
using CustomCampaignTools.Utilities.Patching;

namespace CustomCampaignTools.Patching;

public static class AmmoInventoryPatches
{
    public static Action<AmmoInventory> OnNextAwake = (a) => { };

    [CampaignPatch(typeof(AmmoInventory), nameof(AmmoInventory.Awake), CampaignPatchRunFlags.SessionActive)]
    [HarmonyPostfix]
    public static void AwakePostfix(AmmoInventory __instance)
    {
        var levelBarcode = SceneStreamer.Session.Level.Barcode;

        if (!CampaignUtilities.IsCampaignLevel(levelBarcode, out Campaign campaign, out CampaignLevelType levelType)) return;

        if (levelType != CampaignLevelType.MainLevel) return;

        int levelIndex = campaign.GetMainLevelIndex(levelBarcode);

        AmmoInventory.Instance.ClearAmmo();

        // Accumulate ammo saves from previous levels
        for (int i = 0; i < levelIndex; i++)
        {
            campaign.saveData.GetSavedAmmo(campaign.MainLevels[i].Barcode).AddToPlayer();
        }

        OnNextAwake.Invoke(__instance);
        OnNextAwake = (a) => { }; // clear it
    }
}
