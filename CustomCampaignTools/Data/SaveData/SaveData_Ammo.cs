using CustomCampaignTools.Data;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        internal List<AmmoSave> LoadedAmmoSaves = [];

        public void SaveAmmoForLevel(string levelBarcode)
        {
            if (!campaign.SaveLevelAmmo) return;
            if (string.IsNullOrEmpty(levelBarcode)) return;

            AmmoSave previousAmmoSave = GetPreviousLevelsAmmoSave(levelBarcode);

            if (!DoesSavedAmmoExist(levelBarcode))
            {
                LoadedAmmoSaves.Add(AmmoSave.CreateFromPlayer(levelBarcode) - previousAmmoSave);
            }
            else
            {
                AmmoSave previousHighScore = GetSavedAmmo(levelBarcode);

                for (int i = 0; i < LoadedAmmoSaves.Count; i++)
                {
                    if (LoadedAmmoSaves[i].LevelBarcode == levelBarcode)
                    {
                        LoadedAmmoSaves[i] = AmmoSave.SumOfBest(AmmoSave.CreateFromPlayer(levelBarcode) - previousAmmoSave, previousHighScore);
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
            return GetSavedAmmo(level.BarcodeString);
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
                LoadedAmmoSaves.Add(new AmmoSave(level.BarcodeString, 0, 0, 0));
            }
        }

    }
}