using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using HarmonyLib;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    public static class GashaponHider
    {
        public static HashSet<Crate> HiddenCrates = new HashSet<Crate>();

        public static void AddCratesToHide(List<string> crateBarcodes)
        {
            foreach (string barcode in crateBarcodes)
            {
                if (AssetWarehouse.Instance.TryGetCrate(new Barcode(barcode), out Crate crate))
                {
                    HiddenCrates.Add(crate);
                }
            }
        }
    }

    [HarmonyPatch(typeof(Control_Gashapon))]
    public static class GashaponPatches
    {
        [HarmonyPatch(nameof(Control_Gashapon.Start))]
        [HarmonyPrefix]
        public static void StartPrefix(Control_Gashapon __instance)
        {
            foreach (Crate crate in GashaponHider.HiddenCrates)
            {
                __instance.blackList.Add(crate);
                __instance.blackListStrings.Add(crate.Barcode);
            }
        }
    }
}