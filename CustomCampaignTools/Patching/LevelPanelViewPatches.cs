using System;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using BrowsingPlus.OverrideImplements;
using Il2CppSLZ.Marrow.Warehouse;
using System.Collections.Generic;


using System.Linq;
using BrowsingPlus.PanelUI;
using Il2CppCysharp.Threading.Tasks;


namespace CustomCampaignTools.Patching
{
    #region SLZ Level Panel

    [HarmonyPatch(typeof(LevelsPanelView._PopulateMenuAsync_d__25))]
    public static class LevelsPanelPatches
    {
        [HarmonyPatch(nameof(LevelsPanelView._PopulateMenuAsync_d__25.MoveNext))] // Dunno what method to patch, hopefully refreshing _levelCrates on activate makes it show the right ones
        [HarmonyPostfix]
        public static void ActivatePrefix(LevelsPanelView._PopulateMenuAsync_d__25 __instance)
        {
            ForceLevelList(__instance.__4__this);
        }

        public static void ForceLevelList(LevelsPanelView __instance)
        {
            if (Campaign.SessionActive && Campaign.SessionLocked)
            {
                __instance._levelCrates.Clear();
                foreach (LevelCrate c in Campaign.Session.GetUnlockedLevels()) __instance._levelCrates.Add(c);
            }
            else
            {
                // Sort Campaigns to be right after SLZ levels, and put them in the right order. Need to move this over to the previous function as well, just putting session campaign first.

                Campaign prioritizedCampaign = null;
                if (Campaign.SessionActive)
                {
                    prioritizedCampaign = Campaign.Session;
                }

                // Stupid List Fuckery i hate il2cpp
                List<LevelCrate> instanceCrates = [.. __instance._levelCrates];

                List<LevelCrate> SLZCrates = instanceCrates.Where(crate => crate.Barcode.ID.StartsWith("SLZ")).ToList();
                List<LevelCrate> NonCampaignCrates = instanceCrates.Where(crate => !crate.Barcode.ID.StartsWith("SLZ") && !CampaignUtilities.IsCampaignLevel(crate.Barcode.ID, out _, out _)).ToList();

                List<LevelCrate> CampaignCrates = new List<LevelCrate>();
                foreach (Campaign c in CampaignUtilities.LoadedCampaigns)
                {
                    if (prioritizedCampaign != null && prioritizedCampaign == c) continue;
                    CampaignCrates.AddRange(c.GetUnlockedLevels());
                }

                List<LevelCrate> outputCrates = [.. SLZCrates, .. CampaignCrates, .. NonCampaignCrates];
                if (prioritizedCampaign != null) outputCrates.InsertRange(0, prioritizedCampaign.GetUnlockedLevels());

                __instance._levelCrates.Clear();
                foreach (LevelCrate c in outputCrates) __instance._levelCrates.Add(c);
            }
        }
    }

    #endregion

    #region Swipez Extended Panel

    [HarmonyPatch(typeof(LevelPanelOverride))]
    public static class SwipezPanelPatches
    {
        private static Dictionary<Campaign, PanelContainer> campaignToContainerOpen = new Dictionary<Campaign, PanelContainer>();

        [HarmonyPatch(nameof(LevelPanelOverride.PopulateMenus))]
        [HarmonyPostfix]
        public static void MenuPopulationOverride(LevelPanelOverride __instance)
        {
            PanelContainer campaignContainer = __instance.mainContainer.MakeContainer("Campaigns");

            foreach(Campaign c in CampaignUtilities.CampaignsToShowInMenu)
            {
                PanelContainer container = campaignContainer.MakeContainer(c.Name);

                campaignToContainerOpen.Add(c, container);

                foreach(LevelCrate crate in c.GetUnlockedLevels())
                {
                    container.AddEntry(crate.Title, () => FadeLoader.Load(crate.Barcode, new Barcode(c.LoadScene)));
                }
            }
        }

        //[HarmonyPatch(nameof(LevelPanelOverride.OnInitialized))]
        //[HarmonyPostfix]
        public static void MenuInitContainerOverride(LevelPanelOverride __instance)
        {
            if(Campaign.SessionActive && campaignToContainerOpen.Keys.Contains(Campaign.Session))
            {
                __instance.OpenContainer(campaignToContainerOpen[Campaign.Session]);
            }
        }
    }

    #endregion
}