using CustomCampaignTools.Timing;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        public List<LevelTime> LevelTimes = [];
        public List<TrialTime> TrialTimes = [];

        public int GetLevelTime(string levelBarcode)
        {
            LevelTime levelTime = LevelTimes.Find(lt => lt.LevelBarcode == levelBarcode);
            if (levelTime == null) return 0;
            return levelTime.TimeInSeconds;
        }

        public int GetTotalCampaignTime()
        {
            int totalTime = 0;
            foreach (LevelTime levelTime in LevelTimes)
            {
                totalTime += levelTime.TimeInSeconds;
            }
            return totalTime;
        }

        public void AddTimeToLevel(string levelBarcode, int seconds)
        {
            if (string.IsNullOrEmpty(levelBarcode)) return;
            
            LevelTime levelTime = LevelTimes.Find(lt => lt.LevelBarcode == levelBarcode);
            if (levelTime == null)
            {
                levelTime = new LevelTime()
                {
                    LevelBarcode = levelBarcode,
                    TimeInSeconds = 0
                };
                LevelTimes.Add(levelTime);
            }
            levelTime.TimeInSeconds += seconds;
            SaveToDisk();
        }

        public bool AddTrialTime(string trialKey, float time)
        {
            TrialTime trial = GetTrialTimeData(trialKey);
            if (trial == null)
            {
                trial = new TrialTime()
                {
                    TrialKey = trialKey
                };
                TrialTimes.Add(trial);
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
            return trial.PreviousTimes[trial.PreviousTimes.Count - 1];
        }

        internal TrialTime GetTrialTimeData(string trialKey)
        {
            return TrialTimes.Find(t => t.TrialKey == trialKey);
        }
    }

    public class TrialTime
    {
        public string TrialKey;

        [JsonIgnore]
        public float BestTime { get; private set; }

        [JsonIgnore]
        public float AverageTime { get; private set; }
        public List<float> PreviousTimes = [];

        [OnDeserialized]
        public void OnDeserialize(StreamingContext context)
        {
            if (PreviousTimes == null || PreviousTimes.Count == 0)
            {
                BestTime = -1;
                AverageTime = 0;
                return;
            }
            float best = PreviousTimes[0];
            float total = 0;
            foreach (float time in PreviousTimes)
            {
                if (time < best)
                    best = time;
                total += time;
            }
            BestTime = best;
            AverageTime = total / PreviousTimes.Count;
        }

        public void UpdateAverageTime()
        {
            if (PreviousTimes == null || PreviousTimes.Count == 0)
            {
                AverageTime = 0;
                return;
            }
            float total = 0;
            foreach (float time in PreviousTimes)
            {
                total += time;
            }
            AverageTime = total / PreviousTimes.Count;
        }

        public bool AddTime(float time)
        {
            PreviousTimes.Add(time);
            if (time < BestTime || BestTime < 0)
            {
                BestTime = time;
                return true;
            }
            UpdateAverageTime();
                
            return false;
        }
    }

    public class LevelTime
    {
        public string LevelBarcode;
        public int TimeInSeconds;
    }
}