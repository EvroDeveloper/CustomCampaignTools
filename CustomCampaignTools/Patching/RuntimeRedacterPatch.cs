using HarmonyLib;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CustomCampaignTools.Patching
{
    //[HarmonyPatch(typeof(Scannable))]
    public static class RuntimeRedacter
    {
        //[HarmonyPatch("get_Redacted")]
        //[HarmonyPostfix]
        //public static void GetRedactedPatch(Scannable __instance, ref bool result)
        //{
        //    // Early Returns my beloved
        //    if (__instance is not LevelCrate) return;
        //    MelonLogger.Msg(__instance.Title + " is a Campaign level");
        //    if (!CampaignUtilities.IsCampaignLevel(__instance.Barcode.ID, out Campaign campaign, out _)) return;
        //    MelonLogger.Msg(__instance.Title + " is a Campaign level");
        //    if (!campaign.LockLevelsUntilEntered) return;
        //    MelonLogger.Msg(__instance.Title + "'s campaign Locks Levels'");
        //    var unlocked = campaign.GetUnlockedLevels();
        //    bool locked = true;
        //    for (int i = 0; i < unlocked.Length; i++)
        //    {
        //        MelonLogger.Msg($"Testing Barcode {__instance.Barcode.ID} to {unlocked[i].Barcode.ID}");
        //        if (unlocked[i].Barcode.ID == __instance.Barcode.ID)
        //        {
        //            locked = false;
        //        }
        //    }
        //    if (!__result)
        //    {
        //        MelonLogger.Msg("Set locked of " + __instance.Title + " to " + (locked ? "True" : "False"));
        //        __result = locked;
        //    }
        //}
    }
}
