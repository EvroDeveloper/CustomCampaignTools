namespace CustomCampaignTools.SaveData;


public interface ISaveData
{
    public bool IsLatestSaveData { get; }
    public ISaveData ConvertToNextVersion();
}