using CustomCampaignTools.Timing;
using System.Collections;
using System.Collections.Generic;

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
            return trial.BestTime;
        }

        public float GetTrialAverage(string trialKey)
        {
            TrialTime trial = GetTrialTimeData(trialKey);
            if (trial == null) return 0;
            return trial.GetAverageTime();
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
        public float BestTime = -1f;
        public List<float> PreviousTimes = [];

        public float GetAverageTime()
        {
            if (PreviousTimes == null || PreviousTimes.Count == 0) return 0;
            float total = 0;
            foreach (float time in PreviousTimes)
            {
                total += time;
            }
            return total / PreviousTimes.Count;
        }

        public bool AddTime(float time)
        {
            PreviousTimes.Add(time);
            if (time < BestTime || BestTime <= 0f)
            {
                BestTime = time;
                return true;
            } 
                
            return false;
        }
    }

    public class LevelTime
    {
        public string LevelBarcode;
        public int TimeInSeconds;
    }
}