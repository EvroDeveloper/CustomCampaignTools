using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(Scannable))]
    public static class RuntimeRedacter
    {
        [HarmonyPatch("get_Redacted")]
        [HarmonyPostfix]
        public static void GetRedactedPatch(Scannable __instance, ref bool result)
        {
            // Early Returns my beloved
            if(!__instance is LevelCrate level) return;
            if(!CampaignUtilities.IsCampaignLevel(level.Barcode.ID, out Campaign campaign, out _)) return;
            if(!campaign.LockLevelsUntilEntered) return;
            if(!campaign.GetUnlockedLevels().Contains(level)) result = true;
        }
    }
}