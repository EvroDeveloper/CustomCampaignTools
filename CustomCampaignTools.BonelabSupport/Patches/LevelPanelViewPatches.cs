using System;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using System.Collections.Generic;


using System.Linq;
using Il2CppCysharp.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using Il2CppTMPro;
using Il2CppSLZ.Marrow.Utilities;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools.Patching;

[HarmonyPatch(typeof(LevelsPanelView))]
public static class LevelsPanelPatches
{
    public static bool SwipezActive = false;
    [HarmonyPatch(nameof(LevelsPanelView.CalculateSceneList))]
    [HarmonyPostfix]
    public static void CalculateSceneListPostfix(LevelsPanelView __instance)
    {
        ForceLevelList(__instance);
    }

    [HarmonyPatch(nameof(LevelsPanelView.UpdatePageItems))]
    [HarmonyPostfix]
    public static void UpdatePageItemsPostfix(LevelsPanelView __instance, int pageIdx, int maxItems)
    {
        if (Campaign.SessionLocked || ArgumentHandler.forcedCampaign || (Campaign.SessionActive && Campaign.Session.PrioritizeInLevelPanel))
        {
            int startingIndex = maxItems * pageIdx;

            for (int i = 0; i < maxItems; i++)
            {
                GameObject obj = __instance.items[i];

                int levelIndex = startingIndex + i;
                if(levelIndex < __instance._levelCrates.Count)
                {
                    obj.SetActive(true);

                    LevelCrate targetCrateAtButton = __instance._levelCrates[levelIndex];
                    CampaignLevel cLevel = Campaign.Session.GetLevel(targetCrateAtButton.Barcode);

                    if (cLevel == null) continue;

                    TMP_Text text = obj.GetComponentInChildren<TMP_Text>();
                    if (text == null) continue;

                    text.text = cLevel.Title;
                }
                else
                {
                    obj.SetActive(false);
                }
            } 
        }
    }

    public static void ForceLevelList(LevelsPanelView __instance)
    {
        if(SwipezActive) return;

        List<LevelCrate> panelCratesOverwrite = [];
        if (Campaign.SessionLocked || ArgumentHandler.forcedCampaign)
        {
            panelCratesOverwrite = Campaign.Session.GetUnlockedLevels().ToCrates();
        }
        else
        {
            // Sort Campaigns to be right after SLZ levels, and put them in the right order. Need to move this over to the previous function as well, just putting session campaign first.
            Campaign prioritizedCampaign = null;
            if (Campaign.SessionActive && Campaign.Session.PrioritizeInLevelPanel)
            {
                prioritizedCampaign = Campaign.Session;
            }

            // Stupid List Fuckery i hate il2cpp
            List<LevelCrate> instanceCrates = [.. __instance._levelCrates];

            List<LevelCrate> SLZCrates = [.. instanceCrates.Where(crate => crate.Pallet.IsInMarrowGame())];
            List<LevelCrate> NonCampaignCrates = [.. instanceCrates.Where(crate => !crate.Pallet.IsInMarrowGame() && !CampaignUtilities.TryGetFromLevel(crate.Barcode, out _))];

            List<CampaignLevel> CampaignCrates = [];
            foreach (Campaign c in CampaignUtilities.LoadedCampaigns)
            {
                if (prioritizedCampaign != null && prioritizedCampaign == c) continue;
                CampaignCrates.AddRange(c.GetUnlockedLevels());
            }

            panelCratesOverwrite = [.. SLZCrates, .. CampaignCrates, .. NonCampaignCrates];
            if (prioritizedCampaign != null) panelCratesOverwrite.InsertRange(0, prioritizedCampaign.GetUnlockedLevels().ToCrates());
        }

        __instance._levelCrates.Clear();
        foreach (LevelCrate c in panelCratesOverwrite) __instance._levelCrates.Add(c);
        __instance._totalScenes = __instance._levelCrates.Count;
        __instance._numberOfPages = (__instance._levelCrates.Count / __instance.items.Length) + 1;
    }
}