using UnityEngine;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;

namespace CustomCampaignTools.SaveData.Version1;

public partial class CampaignSaveData
{
    [JsonProperty]
    public Dictionary<string, int> LevelTimes = [];
    [JsonProperty]
    public Dictionary<string, TrialTime> TrialTimes = [];

    public int GetLevelTime(string levelBarcode)
    {
        if(!LevelTimes.ContainsKey(levelBarcode)) return 0;
        return LevelTimes[levelBarcode];
    }

    public int GetTotalCampaignTime()
    {
        int totalTime = 0;
        foreach (int levelTime in LevelTimes.Values)
        {
            totalTime += levelTime;
        }
        return totalTime;
    }

    public void AddTimeToLevel(string levelBarcode, int seconds)
    {
        if (string.IsNullOrEmpty(levelBarcode)) return;

        if(!LevelTimes.ContainsKey(levelBarcode)) LevelTimes.Add(levelBarcode, 0);

        LevelTimes[levelBarcode] += seconds;

        SaveToDisk();
    }

    public bool AddTrialTime(string trialKey, float time)
    {
        TrialTime trial = GetTrialTimeData(trialKey);
        if (trial == null)
        {
            trial = new TrialTime();
            TrialTimes.Add(trialKey, trial);
        }
        bool isBest = trial.AddTime(time);
        SaveToDisk();
        return isBest;
    }

    public float GetTrialBest(string trialKey)
    {
        TrialTime trial = GetTrialTimeData(trialKey);
        if (trial == null) return 0;
        return Mathf.Max(trial.BestTime, 0);
    }

    public float GetTrialAverage(string trialKey)
    {
        TrialTime trial = GetTrialTimeData(trialKey);
        if (trial == null) return 0;
        return Mathf.Max(trial.AverageTime, 0);
    }

    public float GetTrialLatest(string trialKey)
    {
        TrialTime trial = GetTrialTimeData(trialKey);
        if (trial == null) return 0;
        return trial.LatestTime;
    }

    internal TrialTime GetTrialTimeData(string trialKey)
    {
        return TrialTimes[trialKey];
    }
}
