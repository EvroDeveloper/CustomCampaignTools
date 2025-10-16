namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal List<AmmoSave> LoadedAmmoSaves = [];
        
        public void SaveAmmoForLevel(string levelBarcode)
        {
            AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode);

            if (!campaign.SaveLevelAmmo) return;

            if (!DoesSavedAmmoExist(levelBarcode))
            {
                LoadedAmmoSaves.Add(new AmmoSave
                {
                    LevelBarcode = levelBarcode,
                    LightAmmo = AmmoInventory.Instance.GetCartridgeCount("light") - previousAmmoSave.LightAmmo,
                    MediumAmmo = AmmoInventory.Instance.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo,
                    HeavyAmmo = AmmoInventory.Instance.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo
                });
            }
            else
            {
                AmmoSave previousHighScore = GetSavedAmmo(levelBarcode);

                for (int i = 0; i < LoadedAmmoSaves.Count; i++)
                {
                    if (LoadedAmmoSaves[i].LevelBarcode == levelBarcode)
                    {
                        LoadedAmmoSaves[i] = new AmmoSave
                        {
                            LevelBarcode = levelBarcode,
                            LightAmmo = Mathf.Max(AmmoInventory.Instance.GetCartridgeCount("light") - previousAmmoSave.LightAmmo, previousHighScore.LightAmmo),
                            MediumAmmo = Mathf.Max(AmmoInventory.Instance.GetCartridgeCount("medium") - previousAmmoSave.MediumAmmo, previousHighScore.MediumAmmo),
                            HeavyAmmo = Mathf.Max(AmmoInventory.Instance.GetCartridgeCount("heavy") - previousAmmoSave.HeavyAmmo, previousHighScore.HeavyAmmo)
                        };
                    }
                }
            }

            campaign.saveData.SaveToDisk();
        }

        public AmmoSave GetPreviousLevelsAmmoSave(string levelBarcode)
        {
            int levelIndex = campaign.GetLevelIndex(levelBarcode);

            AmmoSave previousLevelsAmmoSave = new AmmoSave();

            for (int i = 0; i < levelIndex; i++)
            {
                previousLevelsAmmoSave.LightAmmo += GetSavedAmmo(campaign.mainLevels[i]).LightAmmo;
                previousLevelsAmmoSave.MediumAmmo += GetSavedAmmo(campaign.mainLevels[i]).MediumAmmo;
                previousLevelsAmmoSave.HeavyAmmo += GetSavedAmmo(campaign.mainLevels[i]).HeavyAmmo;
            }

            return previousLevelsAmmoSave;
        }

        public AmmoSave GetSavedAmmo(CampaignLevel level)
        {
            return GetSavedAmmo(level.sBarcode);
        }

        public AmmoSave GetSavedAmmo(string levelBarcode)
        {
            return LoadedAmmoSaves.FirstOrDefault(x => x.LevelBarcode == levelBarcode);
        }

        public bool DoesSavedAmmoExist(string levelBarcode)
        {
            if (LoadedAmmoSaves == null)
                return false;

            if (LoadedAmmoSaves.Count == 0)
                return false;

            if (LoadedAmmoSaves.Any(x => x.LevelBarcode == levelBarcode))
                return true;

            return false;
        }

        public void ClearAmmoSave()
        {
            LoadedAmmoSaves.Clear();
            // Fill default ammo saves
            foreach (CampaignLevel level in campaign.mainLevels)
            {
                LoadedAmmoSaves.Add(new AmmoSave()
                {
                    LevelBarcode = level.sBarcode,
                    LightAmmo = 0,
                    MediumAmmo = 0,
                    HeavyAmmo = 0,
                });
            }
        }
    }
}