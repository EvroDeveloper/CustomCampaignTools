using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;
using HarmonyLib;

namespace CustomCampaignTools
{
    [HarmonyPatch(typeof(AmmoInventory))]
    public static class AmmoInventoryPatches
    {
        [HarmonyPatch(nameof(AmmoInventory.Awake))]
        [HarmonyPostfix]
        public static void AwakePostfix()
        {
            var levelBarcode = SceneStreamer.Session.Level.Barcode.ID;

            if (!CampaignUtilities.IsCampaignLevel(levelBarcode, out Campaign campaign, out CampaignLevelType levelType)) return;

            if (levelType != CampaignLevelType.MainLevel) return;

            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            AmmoInventory.Instance.ClearAmmo();

            // Accumulate ammo saves from previous levels
            for (int i = 0; i < levelIndex; i++)
            {
                CampaignSaveData.AmmoSave levelAmmo = campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i));
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, levelAmmo.LightAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, levelAmmo.MediumAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, levelAmmo.HeavyAmmo);
            }

            if(SavepointFunctions.CurrentLevelLoadedByContinue)
            {
                var savePoint = Campaign.Session.saveData.LoadedSavePoint;
                // Save Points can have a mid-level ammo score, loading into a level only gives previous ammo high scores so I have to add extra from the save point
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, savePoint.MidLevelAmmoSave.LightAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, savePoint.MidLevelAmmoSave.MediumAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, savePoint.MidLevelAmmoSave.HeavyAmmo);
            }
        }
    }
}
