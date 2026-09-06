using HarmonyLib;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using UnityEngine;
using Il2CppTMPro;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools.BonelabSupport.Patching;

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
        if (Campaign.SessionLocked || CampaignForcing.forcedCampaign || (Campaign.SessionActive && Campaign.Session.PrioritizeInLevelPanel))
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
                    if(!Campaign.Session.TryGetLevel(targetCrateAtButton.Barcode, out CampaignLevel cLevel)) continue;

                    TMP_Text tmp = obj.GetComponentInChildren<TMP_Text>();
                    if (tmp == null) continue;
                    tmp.text = cLevel.Title;
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

        if (Campaign.SessionLocked || CampaignForcing.forcedCampaign)
        {
            SetLevelPanelCrates(__instance, Campaign.Session.GetUnlockedLevels().ToCrates());
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
            List<LevelCrate> NonCampaignCrates = [.. instanceCrates.Where(crate => !crate.Pallet.IsInMarrowGame() && !CampaignUtilities.TryGetCampaign(crate.Barcode, out _))];

            List<CampaignLevel> CampaignCrates = [];
            foreach (Campaign c in CampaignUtilities.LoadedCampaigns)
            {
                if (prioritizedCampaign != null && prioritizedCampaign == c) continue;
                CampaignCrates.AddRange(c.GetUnlockedLevels());
            }

            List<LevelCrate> panelCratesOverwrite = [.. SLZCrates, .. CampaignCrates, .. NonCampaignCrates];
            if (prioritizedCampaign != null) panelCratesOverwrite.InsertRange(0, prioritizedCampaign.GetUnlockedLevels().ToCrates());

            SetLevelPanelCrates(__instance, panelCratesOverwrite);
        }
    }

    public static void SetLevelPanelCrates(LevelsPanelView levelPanel, List<LevelCrate> levelCrates)
    {
        levelPanel._levelCrates.Clear();
        foreach(LevelCrate c in levelCrates) levelPanel._levelCrates.Add(c);
        levelPanel._totalScenes = levelPanel._levelCrates.Count;
        levelPanel._numberOfPages = (levelPanel._levelCrates.Count / levelPanel.items.Length) + 1;
    }
}