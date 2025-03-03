using System;
using HarmonyLib;
using SLZ.Bonelab
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

                if(MarrowGame.assetWarehouse.TryGetCrate<LevelCrate>(Campaign.Session.MenuLevel, out Crate menuCrate))
                {
                    __instance._levelCrates.Add(menuCrate);
                }

                foreach(string mainLevel in Campaign.Session.mainLevels)
                {
                    if(MarrowGame.assetWarehouse.TryGetCrate<LevelCrate>(mainLevel, out Crate mainLevelCrate))
                    {
                        __instance._levelCrates.Add(mainLevelCrate);
                    }
                }

                foreach(string extraLevel in Campaign.Session.extraLevels)
                {
                    if(MarrowGame.assetWarehouse.TryGetCrate<LevelCrate>(extraLevel, out Crate extraLevelCrate))
                    {
                        __instance._levelCrates.Add(extraLevelCrate);
                    }
                }
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

                // Need to get Level Name from Barcode... todo later

                container.AddEntry(c.MenuLevel, () => FadeLoader.Load(new Barcode(c.MenuLevel), new Barcode(c.LoadScene)));

                foreach(string Level in c.mainLevels)
                {
                    container.AddEntry(Level, () => FadeLoader.Load(new Barcode(Level), new Barcode(c.LoadScene)));
                }
            }
        }

        [HarmonyPatch(nameof(LevelPanelOverride.OnInitialize))]
        [HarmonyPostfix]
        public static void MenuInitContainerOverride(LevelPanelOverride __instance)
        {
            if(Campaign.SessionActive)
            {
                __instance.OpenContainer(CampaignToContainerOpen[Campaign.Session]);
            }
        }
    }

    #endregion
}