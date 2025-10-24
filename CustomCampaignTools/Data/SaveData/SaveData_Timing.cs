using CustomCampaignTools.Timing;
using System.Collections;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        public List<LevelTime> LevelTimes = [];
        public List<TrialTime> TrialTimes = [];
        public void AddTimeToLevel(string levelBarcode, int seconds)
        {
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
        }

        public void AddTrialTime(string trialKey, float time)
        {
            TrialTime trial = GetTrialTime(trialKey);
            if (trial == null)
            {
                trial = new TrialTime()
                {
                    TrialKey = trialKey
                };
                TrialTimes.Add(trial);
            }
            trial.AddTime(time);
        }

        public float GetTrialTime(string trialKey)
        {
            TrialTime trial = GetTrialTime(trialKey);
            if (trial == null) return -1;
            return trial.BestTime;
        }

        public float GetTrialAverage(string trialKey)
        {
            TrialTime trial = GetTrialTime(trialKey);
            if (trial == null) return -1;
            return trial.GetAverageTime();
        }

        internal TrialTime GetTrialTime(string trialKey)
        {
            return TrialTimes.Find(t => t.TrialKey == trialKey);
        }
    }

    public class TrialTime
    {
        public string TrialKey;
        public float BestTime;
        public List<float> PreviousTimes = new();

        public float GetAverageTime()
        {
            if (PreviousTimes == null || PreviousTimes.Count == 0) return -1;
            float total = 0;
            foreach (float time in PreviousTimes)
            {
                total += time;
            }
            return total / PreviousTimes.Count;
        }

        public void AddTime(float time)
        {
            PreviousTimes.Add(time);
            if (time < BestTime) BestTime = time;
        }
    }

    public class LevelTime
    {
        public string LevelBarcode;
        public int TimeInSeconds;
    }
}