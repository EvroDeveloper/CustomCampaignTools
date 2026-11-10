using System;
using System.IO;
using CustomCampaignTools.Debug;
using CustomCampaignTools.SaveData.Version1;
using MelonLoader.Utils;
using SimpleSerializables.Utils;
using UnityEngine;

namespace CustomCampaignTools.SaveData;

public static class SaveDataUtilities
{
    public static string SaveFolder { get => Path.Combine(Application.persistentDataPath, "Saves"); }
    public static string GetSavePath(Campaign c) => $"{SaveFolder}/slot_Campaign_{c.Name}.save.json";
    public static string GetBackupPath(Campaign c) => $"{SaveFolder}/slot_Campaign.{c.Name}.save_backup.json";
    public static string GetLegacySavePath(Campaign c) => $"{MelonEnvironment.UserDataDirectory}/Campaigns/{c.Name}/save.json";
    
    /// <summary>
    /// Loads the save data from file. This should *probably* only be called at initialization.
    /// </summary>
    internal static CampaignSaveData LoadFromDisk(Campaign c)
    {
        CampaignLogger.Msg(c, "Loading SaveData from Disk");
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string savePathToUse = GetSavePath(c);

        if (!File.Exists(savePathToUse))
        {
            CampaignLogger.Msg(c, "Could not find save at new save path. Checking Legacy save path");
            if (File.Exists(GetLegacySavePath(c)))
            {
                CampaignLogger.Msg(c, "Found save at legacy save path. Loading from there");
                savePathToUse = GetLegacySavePath(c);
            }
            else
            {
                // Create a blank save, none exists in either save path
                CampaignLogger.Msg(c, "Could not find save at either save path. Creating blank save.");
                CampaignSaveData newSave = new(c);
                
                newSave.SaveToDisk();
                return newSave;
            }
        }

        CampaignLogger.Msg(c, "Deserializing SaveData from path " + savePathToUse);

        string json = File.ReadAllText(savePathToUse);
        bool isModernSaveData = SerializerUtils.TryDeserializeObjectInferred(json, out ISaveData saveDataToUpgrade, out _);
        if (!isModernSaveData)
        {
            Version0.CampaignSaveData oldSaveData = SerializerUtils.LoadObjectFromFile<Version0.CampaignSaveData>(savePathToUse);
            saveDataToUpgrade = oldSaveData;
        }

        CampaignSaveData campaignSaveData = UpgradeLegacySave(saveDataToUpgrade);

        campaignSaveData.campaign = c;
        return campaignSaveData;
    }

    internal static CampaignSaveData UpgradeLegacySave(ISaveData saveData)
    {
        while(!saveData.IsLatestSaveData)
        {
            saveData = saveData.ConvertToNextVersion();
        }

        if(saveData is CampaignSaveData campaignSaveData)
        {
            return campaignSaveData;
        }
        else
        {
            return null;
        }
    }
}