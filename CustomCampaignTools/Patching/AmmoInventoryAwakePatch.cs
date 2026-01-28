using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using HarmonyLib;
using CustomCampaignTools.Data;

namespace CustomCampaignTools
{
    [HarmonyPatch(typeof(AmmoInventory))]
    public static class AmmoInventoryPatches
    {
        public static Action<AmmoInventory> OnNextAwake = (a)=>{};

        [HarmonyPatch(nameof(AmmoInventory.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix(AmmoInventory __instance)
        {
            var levelBarcode = SceneStreamer.Session.Level.Barcode.ID;

            if (!CampaignUtilities.IsCampaignLevel(levelBarcode, out Campaign campaign, out CampaignLevelType levelType)) return;

            if (levelType != CampaignLevelType.MainLevel) return;

            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            AmmoInventory.Instance.ClearAmmo();

            // Accumulate ammo saves from previous levels
            for (int i = 0; i < levelIndex; i++)
            {
                campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).AddToPlayer();
            }

            OnNextAwake.Invoke(__instance);
            OnNextAwake = (a)=>{}; // clear it
        }
    }
}
