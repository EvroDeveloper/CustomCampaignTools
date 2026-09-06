using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CustomCampaignTools;

public partial class CampaignSaveData
{
    [JsonProperty]
    public Dictionary<string, int> SavedStats = [];

    public int ReadStat(string stat)
    {
        if(!SavedStats.ContainsKey(stat))
            return 0;
        
        return SavedStats[stat];
    }

    public void IncrementStat(string stat)
    {
        if(!SavedStats.ContainsKey(stat))
            SavedStats.Add(stat, 0);
        
        SavedStats[stat] += 1;
        SaveToDisk();
    }

    public void DecrementStat(string stat)
    {
        if(!SavedStats.ContainsKey(stat))
            SavedStats.Add(stat, 0);
        
        SavedStats[stat] -= 1;
        SaveToDisk();
    }

    public void ForceSetStat(string stat, int value)
    {
        if(!SavedStats.ContainsKey(stat))
            SavedStats.Add(stat, 0);
        
        SavedStats[stat] = value;
        SaveToDisk();
    }
}
