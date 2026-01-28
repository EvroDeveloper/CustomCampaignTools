using System;

namespace CustomCampaignTools.Data.SavePoints;

public interface ISavePoint
{
    Campaign campaign { get; set; }
    bool IsValid();

    void LoadContinue();

    void OnSceneLoadedFromContinue();
}
