namespace CustomCampaignTools;

public partial class Campaign
{
    public static Campaign Session;
    public static CampaignLevel lastLoadedCampaignLevel;
    public static bool SessionActive { get => Session != null; }
    public static bool SessionLocked { get => SessionActive && _sessionLocked; }
    private static bool _sessionLocked;
}