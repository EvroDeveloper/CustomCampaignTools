using System.Collections.Generic;
using System.Linq;
using CustomCampaignTools.Data;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        [JsonProperty]
        public List<AmmoSave> LoadedAmmoSaves = [];

        public void SaveAmmoForLevel(Barcode levelBarcode)
        {
            if (!campaign.SaveLevelAmmo) return;
            if (string.IsNullOrEmpty(levelBarcode.ID) || !levelBarcode.IsValid()) return;

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

        public AmmoSave GetPreviousLevelsAmmoSave(Barcode levelBarcode)
        {
            int levelIndex = campaign.GetMainLevelIndex(levelBarcode);

            AmmoSave previousLevelsAmmoSave = new AmmoSave();

            for (int i = 0; i < levelIndex; i++)
            {
                previousLevelsAmmoSave.LightAmmo += GetSavedAmmo(campaign.MainLevels[i]).LightAmmo;
                previousLevelsAmmoSave.MediumAmmo += GetSavedAmmo(campaign.MainLevels[i]).MediumAmmo;
                previousLevelsAmmoSave.HeavyAmmo += GetSavedAmmo(campaign.MainLevels[i]).HeavyAmmo;
            }

            return previousLevelsAmmoSave;
        }

        public AmmoSave GetSavedAmmo(CampaignLevel level)
        {
            return GetSavedAmmo(level.Barcode);
        }

        public AmmoSave GetSavedAmmo(Barcode levelBarcode)
        {
            return LoadedAmmoSaves.FirstOrDefault(x => x.LevelBarcode == levelBarcode);
        }

        public bool DoesSavedAmmoExist(Barcode levelBarcode)
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
            LoadedAmmoSaves ??= [];
            LoadedAmmoSaves.Clear();
            // Fill default ammo saves
            foreach (CampaignLevel level in campaign.MainLevels)
            {
                LoadedAmmoSaves.Add(new AmmoSave(level, 0, 0, 0));
            }
        }

    }
}