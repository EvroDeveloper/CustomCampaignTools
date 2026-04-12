using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using HarmonyLib;
using CustomCampaignTools.Utilities;

namespace CustomCampaignTools.BonelabSupport
{
    [HarmonyPatch(typeof(Control_Gashapon))]
    public static class GashaponPatches
    {
        [HarmonyPatch(nameof(Control_Gashapon.Start))]
        [HarmonyPrefix]
        public static void StartPrefix(Control_Gashapon __instance)
        {
            foreach (Campaign campaign in CampaignUtilities.LoadedCampaigns)
            {
                foreach (SpawnableCrateReference crateRef in campaign.HiddenCrates)
                {
                    if(crateRef.TryGetCrate(out var crate))
                    {
                        __instance.blackList.Add(crate);
                        __instance.blackListStrings.Add(crate.Barcode);
                    }
                }
            }
        }
    }
}