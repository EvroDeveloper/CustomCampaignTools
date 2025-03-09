using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.SceneStreaming;

namespace CustomCampaignTools
{
    public static class AmmoFunctions
    {
        public static void ClearAmmo(Campaign campaign)
        {
            if (CampaignUtilities.IsCampaignLevel(SceneStreamer.Session.Level.Barcode.ID, out _, out _))
                AmmoInventory.Instance.ClearAmmo();

            campaign.saveData.LoadedAmmoSaves.Clear();

            campaign.saveData.SaveToDisk();
        }

        public static void LoadAmmoFromLevel(string levelBarcode, bool isLoadCheckpoint)
        {
            if(!CampaignUtilities.IsCampaignLevel(levelBarcode, out Campaign campaign, out CampaignLevelType levelType)) return;

            if(!campaign.SaveLevelAmmo) return;

            if(levelType != CampaignLevelType.MainLevel) return;
            

            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            try
            {
                AmmoInventory.Instance.ClearAmmo();
            }
            catch
            {
            }

            string[] mainLevels = campaign.mainLevels;

            for (int i = 0; i < levelIndex; i++)
            {
                var level = mainLevels[i];
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, campaign.saveData.GetSavedAmmo(level).LightAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, campaign.saveData.GetSavedAmmo(level).MediumAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, campaign.saveData.GetSavedAmmo(level).HeavyAmmo);
            }
        }
    }

    [HarmonyLib.HarmonyPatch(typeof(AmmoInventory), nameof(AmmoInventory.Awake))]
    public static class AmmoInventoryAwake
    {
        public static void Postfix()
        {
            var levelBarcode = SceneStreamer.Session.Level.Barcode.ID;

            if (!CampaignUtilities.IsCampaignLevel(levelBarcode, out Campaign campaign, out CampaignLevelType levelType)) return;

            if (levelType != CampaignLevelType.MainLevel) return;

            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            AmmoInventory.Instance.ClearAmmo();

            for (int i = 0; i < levelIndex; i++)
            {
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).LightAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).MediumAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, campaign.saveData.GetSavedAmmo(campaign.GetLevelBarcodeByIndex(i)).HeavyAmmo);
            }

            if(SavepointFunctions.LoadByContine_AmmoPatchHint)
            {
                // Save Points can have a mid-level ammo score, loading into a level only gives previous ammo high scores so I have to add extra from the save point
                SavepointFunctions.LoadByContine_AmmoPatchHint = false;
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.lightAmmoGroup, campaign.saveData.LoadedSavePoint.MidLevelAmmoSave.LightAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.mediumAmmoGroup, campaign.saveData.LoadedSavePoint.MidLevelAmmoSave.MediumAmmo);
                AmmoInventory.Instance.AddCartridge(AmmoInventory.Instance.heavyAmmoGroup, campaign.saveData.LoadedSavePoint.MidLevelAmmoSave.HeavyAmmo);
            }
        }
    }
}
