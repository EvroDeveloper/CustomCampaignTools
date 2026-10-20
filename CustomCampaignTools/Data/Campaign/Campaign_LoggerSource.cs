using CustomCampaignTools.Debug;

namespace CustomCampaignTools;

public partial class Campaign : ILoggerSource
{
    public bool ForceMessages()
    {
        return false;
    }

    public string GetName()
    {
        return Name;
    }
}