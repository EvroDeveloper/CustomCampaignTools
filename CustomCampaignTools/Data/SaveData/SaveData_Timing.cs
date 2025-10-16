using CustomCampaignTools.Timing;
using System.Collections;
using System.Collections.Generic;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        public List<LevelTime> levelTimes = new List<LevelTime>();
        public List<TrialTime> TrialTimes = new List<TrialTime>();
        public void AddTimeToLevel(string levelBarcode, int seconds)
        {
        }

        public void AddTrialTime(string trialKey, float time)
        {
            TrialTime trial = TrialTimes.Find(t => t.TrialKey == trialKey);
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
            TrialTime trial = TrialTimes.Find(t => t.TrialKey == trialKey);
            if (trial == null) return -1;
            return trial.BestTime;
        }

        public float GetTrialAverage(string trialKey)
        {
            TrialTime trial = TrialTimes.Find(t => t.TrialKey == trialKey);
            if (trial == null) return -1;
            return trial.GetAverageTime();
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