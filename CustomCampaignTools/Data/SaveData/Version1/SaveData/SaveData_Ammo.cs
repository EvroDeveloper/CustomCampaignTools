using System.Collections.Generic;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;

namespace CustomCampaignTools.SaveData.Version1;

public partial class CampaignSaveData
{
    [JsonProperty]
    public Dictionary<string, AmmoCount> SavedAmmo = [];

    public void SaveAmmoForLevel(CampaignLevel campaignLevel)
    {
        if (!campaign.SaveLevelAmmo) return;
        if(campaignLevel is not MainLevel mainLevel) return;
        var levelBarcode = campaignLevel.Barcode;
        if (string.IsNullOrEmpty(levelBarcode.ID) || !levelBarcode.IsValid()) return;

        AmmoCount previousAmmoSum = GetPreviousLevelsAmmoSave(mainLevel);
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

    public AmmoCount GetPreviousLevelsAmmoSave(MainLevel mainLevel)
    {
        int levelIndex = mainLevel.LevelIndex;

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
