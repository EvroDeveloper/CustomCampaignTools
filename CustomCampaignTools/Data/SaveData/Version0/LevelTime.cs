using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CustomCampaignTools.SaveData.Version0;

public class LevelTime
{
    public string LevelBarcode;
    public int TimeInSeconds;
}

public class TrialTime
{
    public string TrialKey;
    public float BestTime = -1;
    public float AverageTime = 0;
    public float LatestTime = 0;
    public int TotalAttempts = 0;
    public List<float> PreviousTimes { get; private set; }

    public Version1.TrialTime Convert()
    {
        return new Version1.TrialTime()
        {
            BestTime = BestTime,
            AverageTime = AverageTime,
            LatestTime = LatestTime,
            TotalAttempts = TotalAttempts,
        };
    }

    [OnDeserialized]
    public void OnDeserialize(StreamingContext context)
    {
        // Continue to use PreviousTimes to fix new fields. 
        CalculateFieldsFromLegacy();
    }

    private void CalculateFieldsFromLegacy()
    {
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
    }
}