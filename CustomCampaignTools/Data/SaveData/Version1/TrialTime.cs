namespace CustomCampaignTools.SaveData.Version1;

public class TrialTime
{
    public float BestTime = -1;
    public float AverageTime = 0;
    public float LatestTime = 0;
    public int TotalAttempts = 0;

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