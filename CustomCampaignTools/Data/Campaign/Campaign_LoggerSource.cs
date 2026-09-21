using CustomCampaignTools.Debug;

namespace CustomCampaignTools;

public partial class Campaign : ILoggerSource
{
    public string GetName()
    {
        return Name;
    }
}