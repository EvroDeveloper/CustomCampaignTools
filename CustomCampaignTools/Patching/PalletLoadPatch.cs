using CustomCampaignTools.Debug;
using HarmonyLib;
using Il2CppSLZ.Marrow.Forklift.Model;
using Il2CppSLZ.Marrow.Warehouse;
namespace CustomCampaignTools.Patching
{
    [HarmonyPatch(typeof(AssetWarehouse))]
    public static class PalletLoadPatch
    {
        [HarmonyPatch(nameof(AssetWarehouse.AddPallet))]
        [HarmonyPostfix]
        public static void Postfix(AssetWarehouse __instance, Pallet pallet)
        {
            Campaign.RegisterCampaignFromPallet(pallet);
        }
    }
}