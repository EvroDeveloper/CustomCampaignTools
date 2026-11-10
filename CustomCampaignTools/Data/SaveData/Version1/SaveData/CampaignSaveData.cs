using SimpleSerializables.Utils;
using CustomCampaignTools.Debug;
using Il2CppSLZ.Marrow.SaveData;
using Il2CppSLZ.Marrow.Warehouse;
using Newtonsoft.Json;
using System.IO;
using CustomCampaignTools.GameSupport;

namespace CustomCampaignTools.SaveData.Version1;

public partial class CampaignSaveData : ISaveData
{
    [JsonIgnore]
    public Campaign campaign;

    [JsonIgnore]
    public bool IsLatestSaveData => true;

    public CampaignSaveData() {}

    public CampaignSaveData(Campaign c)
    {
        campaign = c;
        ClearAmmoSave();
    }

    public void ResetSave()
    {
        SaveToDisk(SaveDataUtilities.GetBackupPath(campaign));

        ClearAmmoSave();
        ClearSavePoint();
        InventorySaves = [];
        FloatData = [];
        DevToolsUnlocked = false;
        AvatarUnlocked = false;
        SkipIntro = false;
        UnlockedAchievements = [];
        UnlockedLevels = [];

        foreach (string barcode in campaign.CampaignUnlockCrates)
            GameManager.currentGameConfiguration.GameDataManager.ClearUnlockForBarcode(new Barcode(barcode));

        GameManager.currentGameConfiguration.GameDataManager.TrySaveActiveSave(SaveFlags.DefaultAndPlayerSettingsAndUnlocks);

        SaveToDisk();
    }

    /// <summary>
    /// Saves the current loaded save data to file.
    /// </summary>
    internal void SaveToDisk(string overwritePath = "")
    {
        if (!Directory.Exists(SaveDataUtilities.SaveFolder))
            Directory.CreateDirectory(SaveDataUtilities.SaveFolder);

        string savePath = (overwritePath != string.Empty) ? overwritePath : SaveDataUtilities.GetSavePath(campaign);
        
        CampaignLogger.Msg(campaign, "Saving SaveData to Disk at path: " + savePath);
        string json = SerializerUtils.SerializedObjectInferred(this);
        File.WriteAllText(savePath, json);
    }

    public ISaveData ConvertToNextVersion()
    {
        return this;
    }
}