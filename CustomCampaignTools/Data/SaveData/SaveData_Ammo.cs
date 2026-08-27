using System.Collections.Generic;
using CustomCampaignTools.Data;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;

namespace CustomCampaignTools;

public partial class CampaignSaveData
{
    [JsonProperty("LoadedAmmoSaves")]
    private List<AmmoSave> LegacyLoadedAmmoSaves
    {
        set
        {
            if (value == null)
                return;

            SavedAmmo ??= [];

            foreach (AmmoSave ammoSave in value)
            {
                string levelBarcode = ammoSave.LevelBarcode?.ID;
                if (string.IsNullOrEmpty(levelBarcode) || SavedAmmo.ContainsKey(levelBarcode))
                    continue;

                SavedAmmo[levelBarcode] = new AmmoCount(ammoSave.LightAmmo, ammoSave.MediumAmmo, ammoSave.HeavyAmmo);
            }

        }
    }

    [JsonProperty]
    public Dictionary<string, AmmoCount> SavedAmmo = [];

    public void SaveAmmoForLevel(Barcode levelBarcode)
    {
        if (!campaign.SaveLevelAmmo) return;
        if (string.IsNullOrEmpty(levelBarcode.ID) || !levelBarcode.IsValid()) return;

        AmmoCount previousAmmoSum = GetPreviousLevelsAmmoSave(levelBarcode);
        AmmoCount additionalAmmo = AmmoCount.GetFromPlayer() - previousAmmoSum;
        SavedAmmo ??= [];

        if (!DoesSavedAmmoExist(levelBarcode))
        {
            SavedAmmo[levelBarcode.ID] = additionalAmmo;
        }
        else
        {
            AmmoCount previousHighScore = GetSavedAmmo(levelBarcode);
            AmmoCount bestAmmo = AmmoCount.Max(additionalAmmo, previousHighScore);

            SavedAmmo[levelBarcode.ID] = bestAmmo;
        }

        campaign.saveData.SaveToDisk();
    }

    public AmmoCount GetPreviousLevelsAmmoSave(Barcode levelBarcode)
    {
        int levelIndex = campaign.GetMainLevelIndex(levelBarcode);

        AmmoCount previousLevelsAmmoSave = new AmmoCount();

        for (int i = 0; i < levelIndex; i++)
        {
            previousLevelsAmmoSave += GetSavedAmmo(campaign.MainLevels[i]);
        }

        return previousLevelsAmmoSave;
    }

    public AmmoCount GetSavedAmmo(CampaignLevel level)
    {
        return GetSavedAmmo(level.Barcode);
    }

    public AmmoCount GetSavedAmmo(Barcode levelBarcode)
    {
        if (!string.IsNullOrEmpty(levelBarcode?.ID)
            && SavedAmmo != null
            && SavedAmmo.TryGetValue(levelBarcode.ID, out AmmoCount ammoCount))
            return ammoCount;

        return new AmmoCount();
    }

    public bool DoesSavedAmmoExist(Barcode levelBarcode)
    {
        return !string.IsNullOrEmpty(levelBarcode?.ID)
            && SavedAmmo != null
            && SavedAmmo.ContainsKey(levelBarcode.ID);
    }

    public void ClearAmmoSave()
    {
        SavedAmmo ??= [];
        SavedAmmo.Clear();
        // Fill default ammo saves
        foreach (CampaignLevel level in campaign.MainLevels)
        {
            SavedAmmo[level.Barcode.ID] = new AmmoCount();
        }
    }

}
