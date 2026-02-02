using System;

namespace CustomCampaignTools.Data.SaveData;

public partial class CampaignSaveData
{
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
    }

    public void DecrementStat(string stat)
    {
        if(!SavedStats.ContainsKey(stat))
            SavedStats.Add(stat, 0);
        
        SavedStats[stat] -= 1;
    }

    public void ForceSetStat(string stat, int value)
    {
        if(!SavedStats.ContainsKey(stat))
            SavedStats.Add(stat, 0);
        
        SavedStats[stat] = value;
    }
}
