using System;
using HarmonyLib;
using SLZ.Bonelab;
using BrowsingPlus.OverrideImplements;


namespace CustomCampaignTools.Patching
{
    #region SLZ Level Panel

    [HarmonyPatch(typeof(LevelsPanelView))]
    public static class LevelsPanelPatches
    {
        [HarmonyPatch(nameof(LevelsPanelView.Activate))] // Dunno what method to patch, hopefully refreshing _levelCrates on activate makes it show the right ones
        [HarmonyPrefix]
        public static void ActivatePrefix(LevelsPanelView __instance)
        {
            if(Campaign.SessionLocked)
            {
                __instance._levelCrates.Clear();
                __instance._levelCrate.AddRange(Campaign.Session.GetUnlockedLevels());
            }
            else
            {
                __instance._levelCrate.InsertRange(0, Campaign.Session.GetUnlockedLevels());
            }
        }
    }

    #endregion

    #region Swipez Extended Panel

    [HarmonyPatch(typeof(LevelPanelOverride))]
    public static class SwipezPanelPatches
    {
        private Dictionary<Campaign, PanelContainer> CampaignToContainerOpen = new Dictionary<Campaign, PanelContainer>();

        [HarmonyPatch(nameof(LevelPanelOverride.PopulateMenus))]
        [HarmonyPostfix]
        public static void MenuPopulationOverride(LevelPanelOverride __instance)
        {
            PanelContainer campaignContainer = palletsContainer.MakeContainer("Campaigns");

            foreach(Campaign c in CampaignUtilities.CampaignsToShowInMenu)
            {
                PanelContainer container = campaignContainer.MakeContainer(c.Name);

                CampaignToContainerOpen.Add(c, container);

                foreach(LevelCrate crate in c.GetUnlockedLevels())
                {
                    container.AddEntry(crate.title, () => FadeLoader.Load(crate.Barcode), new Barcode(c.LoadScene));
                }
            }
        }

        [HarmonyPatch(nameof(LevelPanelOverride.OnInitialize))]
        [HarmonyPostfix]
        public static void MenuInitContainerOverride(LevelPanelOverride __instance)
        {
            if(Campaign.SessionActive && CampaignToContainerOpen.Keys.Contains(Campaign.Session))
            {
                __instance.OpenContainer(CampaignToContainerOpen[Campaign.Session]);
            }
        }
    }

    #endregion
}