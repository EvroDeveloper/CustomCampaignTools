using UnityEngine;
using Newtonsoft.Json;
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
            return trial.LatestTime;
        }

        internal TrialTime GetTrialTimeData(string trialKey)
        {
            return TrialTimes.Find(t => t.TrialKey == trialKey);
        }
    }

    public class TrialTime
    {
        public string TrialKey;
        public float BestTime = -1;
        public float AverageTime = 0;
        public float LatestTime = 0;
        public int TotalAttempts = 0;

        [Obsolete("Too much data, replace with attempt average calculation")]
        public List<float> PreviousTimes { get; private set; } // Dont use this, we can do everything we need with Average and Attempts Count


        [OnDeserialized]
        public void OnDeserialize(StreamingContext context)
        {
            // Continue to use PreviousTimes to fix new fields. 
            CalculateFieldsFromLegacy();
        }

        private void CalculateFieldsFromLegacy()
        {
#pragma warning disable CS0618 // yes mr compiler i am aware that i am using obsolete code its called "backwards compatibility" ever heard of it?
            if (PreviousTimes == null) return;
            if (PreviousTimes.Count == 0)
            {
                PreviousTimes = null;
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
            PreviousTimes = null;
#pragma warning restore CS0618
        }

        public void UpdateAverageTime(float time)
        {
            // Given the Average Time, and number of attempts, calculate a new average
            float totalTimes = AverageTime * TotalAttempts;
            totalTimes += time;
            TotalAttempts++;
            AverageTime = totalTimes / TotalAttempts; // This code to me feels like the equivalent of that mr incredible typing at the computer meme gif
        }

        public bool AddTime(float time)
        {
            LatestTime = time;
            if (time < BestTime || BestTime < 0)
            {
                BestTime = time;
                return true;
            }
            UpdateAverageTime(time);
                
            return false;
        }
    }

    public class LevelTime
    {
        public string LevelBarcode;
        public int TimeInSeconds;
    }
}