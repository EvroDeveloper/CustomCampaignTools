using System;
using HarmonyLib;
using Il2CppSLZ.Bonelab;
using BrowsingPlus.OverrideImplements;
using Il2CppSLZ.Marrow.Warehouse;
using System.Collections.Generic;


using System.Linq;
using BrowsingPlus.PanelUI;
using Il2CppCysharp.Threading.Tasks;
using MelonLoader;


namespace CustomCampaignTools.Patching
{
    #region SLZ Level Panel

    [HarmonyPatch(typeof(LevelsPanelView))]
    public static class LevelsPanelPatches
    {
        public static bool SwipezActive = false;
        [HarmonyPatch(nameof(LevelsPanelView.CalculateSceneList))]
        [HarmonyPostfix]
        public static void PopulatePostfix(LevelsPanelView __instance)
        {
            ForceLevelList(__instance);
        }

        public static void ForceLevelList(LevelsPanelView __instance)
        {
            if(SwipezActive) return;

            MelonLogger.Msg("Level Panel is OVERWRITING");
            if (Campaign.SessionLocked)
            {
                __instance._levelCrates.Clear();
                CampaignLevel[] unlockedLevels = Campaign.Session.GetUnlockedLevels();
                foreach (CampaignLevel c in unlockedLevels) __instance._levelCrates.Add(c.crate);
                __instance._totalScenes = unlockedLevels.Length;
                __instance._numberOfPages = (unlockedLevels.Length / __instance.items.Length) + 1;
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

                List<LevelCrate> SLZCrates = instanceCrates.Where(crate => crate.Pallet.Barcode.ID.StartsWith("SLZ")).ToList();
                List<LevelCrate> NonCampaignCrates = instanceCrates.Where(crate => !crate.Pallet.Barcode.ID.StartsWith("SLZ") && !CampaignUtilities.IsCampaignLevel(crate.Barcode.ID)).ToList();

                List<LevelCrate> CampaignCrates = [];
                foreach (Campaign c in CampaignUtilities.LoadedCampaigns)
                {
                    if (prioritizedCampaign != null && prioritizedCampaign == c) continue;
                    CampaignCrates.AddRange(c.GetUnlockedLevels().ToCrates());
                }

                List<LevelCrate> outputCrates = [.. SLZCrates, .. CampaignCrates, .. NonCampaignCrates];
                if (prioritizedCampaign != null) outputCrates.InsertRange(0, prioritizedCampaign.GetUnlockedLevels().ToCrates());

                __instance._levelCrates.Clear();
                foreach (LevelCrate c in outputCrates) __instance._levelCrates.Add(c);
                __instance._totalScenes = outputCrates.Count;
                __instance._numberOfPages = (outputCrates.Count / __instance.items.Length) + 1;
            }
        }
    }

    #endregion

    #region Swipez Extended Panel

    //[HarmonyPatch(typeof(LevelPanelOverride))]
    public static class SwipezPanelPatches
    {
        private static Dictionary<Campaign, PanelContainer> campaignToContainerOpen = [];


        public static void ManualPatch()
        {
            var harmony = new HarmonyLib.Harmony("swipez.panel.populate");

            harmony.Patch(typeof(LevelPanelOverride).GetMethod(nameof(LevelPanelOverride.PopulateMenus)), postfix: new HarmonyMethod(typeof(SwipezPanelPatches), "MenuPopulationOverride"));
            harmony.Patch(typeof(LevelPanelOverride).GetMethod(nameof(LevelPanelOverride.OnInitialized)), postfix: new HarmonyMethod(typeof(SwipezPanelPatches), "MenuInitContainerOverride"));
            LevelsPanelPatches.SwipezActive = true;
        }

        //[HarmonyPatch(nameof(LevelPanelOverride.PopulateMenus))]
        //[HarmonyPostfix]
        public static void MenuPopulationOverride(LevelPanelOverride __instance)
        {
            PanelContainer campaignContainer = __instance.GetOrMakeCampaignContainer("Campaigns");
            campaignContainer.Clear();
            campaignToContainerOpen.Clear();

            foreach(Campaign c in CampaignUtilities.CampaignsToShowInMenu)
            {
                PanelContainer container = campaignContainer.MakeContainer(c.Name);

                campaignToContainerOpen.Add(c, container);

                foreach(CampaignLevel level in c.GetUnlockedLevels())
                {
                    if(level.crate && !level.crate.Redacted)
                    {
                        container.AddEntry(level.Title, () => FadeLoader.Load(level.Barcode, new Barcode(c.LoadScene)));
                    }
                }
            }
        }

        private static Dictionary<LevelPanelOverride, PanelContainer> mainToCampaignContainer = [];

        public static PanelContainer GetOrMakeCampaignContainer(this LevelPanelOverride container, string name)
        {
            if(!mainToCampaignContainer.ContainsKey(container))
            {
                mainToCampaignContainer.Add(container, container.mainContainer.MakeContainer(name));
            }
            return mainToCampaignContainer[container];
        }

        public static void MenuInitContainerOverride(LevelPanelOverride __instance)
        {
            if (!Campaign.SessionActive) return;

            if(campaignToContainerOpen.Keys.Contains(Campaign.Session))
            {
                __instance.OpenContainer(campaignToContainerOpen[Campaign.Session]);
            }

            if(Campaign.SessionLocked)
            {
                campaignToContainerOpen[Campaign.Session].parent = null;
            }
            else
            {
                campaignToContainerOpen[Campaign.Session].parent = __instance.mainContainer;
            }
        }
    }

    #endregion
}
