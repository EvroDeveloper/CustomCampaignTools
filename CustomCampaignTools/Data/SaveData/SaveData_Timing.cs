using CustomCampaignTools.Timing;

namespace CustomCampaignTools
{
    public partial class CampaignSaveData
    {
        public List<LevelTime> LevelTimes = new List<LevelTime>();
        public List<TrialTime> TrialTimes = new List<TrialTime>();
        public void AddTimeToLevel(string levelBarcode, int seconds)
        {
            if (levelTimes.ContainsKey(levelBarcode))
            {
                levelTimes[levelBarcode] += seconds;
            }
            else
            {
                levelTimes[levelBarcode] = seconds;
            }
        }

        public void AddTrialTime(string trialKey, float time)
        {
            TrialTime trial = TrialTimes.Find(t => t.TrialKey == trialKey);
            if (trial == null)
            {
                trial = new TrialTime()
                {
                    TrialKey = trialKey,
                    BestTime = time,
                    PreviousTimes = new List<float>() { time }
                };
                TrialTimes.Add(trial);
            }
            else
            {
                trial.PreviousTimes.Add(time);
                if (time < trial.BestTime) trial.BestTime = time;
            }
        }
    }

    public class TrialTime
    {
        public string TrialKey;
        public float BestTime;
        public List<float> PreviousTimes;
    }

    public class LevelTime
    {
        public string LevelBarcode;
        public int TimeInSeconds;
    }
}