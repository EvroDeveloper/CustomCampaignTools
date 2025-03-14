namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(AssetWarehouse.HideLevelCrateFilter))]
    public static class HideLevelFilterPatch
    {
        [HarmonyPatch(nameof(AssetWarehouse.HideLevelCrateFilter.Filter))]
        public static void FilterPostfix(LevelCrate crate, ref bool __result)
        {
            if(__result == false) return;
            if(IsCampaignLevel(crate.Barcode.ID, out Campaign c, out CampaignLevelType type))
            {
                if(type != CampaignLevelType.Menu && c.LockLevelsUntilEntered)
                {
                    __result = c.GetUnlockedLevels().Contains(crate);
                }
            }
        }
    }
}