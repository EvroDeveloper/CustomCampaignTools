using BrowsingPlus.OverrideImplements;
using System.Collections.Generic;
using BrowsingPlus.PanelUI;
using BoneLib;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools.Patching;
#region Swipez Extended Panel

//[HarmonyPatch(typeof(LevelPanelOverride))]
public static class SwipezPanelPatches
{
    private static Dictionary<Campaign, PanelContainer> campaignToContainerOpen = [];

    public static void ManualPatch()
    {
        Hooking.CreateHook(typeof(LevelPanelOverride).GetMethod(nameof(LevelPanelOverride.PopulateMenus)), typeof(SwipezPanelPatches).GetMethod(nameof(MenuPopulationOverride)));
        Hooking.CreateHook(typeof(LevelPanelOverride).GetMethod(nameof(LevelPanelOverride.OnInitialized)), typeof(SwipezPanelPatches).GetMethod(nameof(MenuInitContainerOverride)));

        // var harmony = new HarmonyLib.Harmony("swipez.panel.populate");
        // harmony.Patch(typeof(LevelPanelOverride).GetMethod(nameof(LevelPanelOverride.PopulateMenus)), postfix: new HarmonyMethod(typeof(SwipezPanelPatches), "MenuPopulationOverride"));
        // harmony.Patch(typeof(LevelPanelOverride).GetMethod(nameof(LevelPanelOverride.OnInitialized)), postfix: new HarmonyMethod(typeof(SwipezPanelPatches), "MenuInitContainerOverride"));

        LevelsPanelPatches.SwipezActive = true;
    }

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
                if(level.Crate && !level.Crate.Redacted)
                {
                    container.AddEntry(level.Title, () => FadeLoader.Load(level, c.LoadScene));
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

        if(campaignToContainerOpen.ContainsKey(Campaign.Session))
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
