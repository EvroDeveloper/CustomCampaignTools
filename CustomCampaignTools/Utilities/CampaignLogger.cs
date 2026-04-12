using MelonLoader;

namespace CustomCampaignTools.Debug;

public class CampaignLogger
{
    private static readonly MelonLogger.Instance loggerInstance = new("CustomCampaignTools");

#if DEBUG
    const bool EnableLogging = true;
#else
    const bool EnableLogging = false;
#endif
    private static bool VerboseLogging = true;

    public static void Msg(object message, bool force = false)
    {
        if (!EnableLogging && !force) return;
        loggerInstance.Msg($"[CampaignLogger] {message}");
    }

    public static void Msg(Campaign campaign, object message, bool force = false)
    {
        if(campaign == null)
        {
            Msg(message, force);
            return;
        }
        if (!EnableLogging && !campaign.DEVMODE && !force) return;
        loggerInstance.Msg($"[CampaignLogger - {campaign.Name}] {message}");
    }

    public static void MsgVerbose(object message)
    {
        if (!VerboseLogging) return;
        loggerInstance.Msg($"[CampaignLogger] {message}");
    }

    public static void MsgVerbose(Campaign campaign, object message)
    {
        if(campaign == null)
        {
            Msg(message);
            return;
        }
        if (!VerboseLogging) return;
        loggerInstance.Msg($"[CampaignLogger - {campaign.Name}] {message}");
    }

    public static void Error(Campaign campaign, object message)
    {
        loggerInstance.Error($"[CampaignLogger - {campaign.Name}] {message}");
    }

    public static void Error(object message)
    {
        loggerInstance.Error($"[CampaignLogger] {message}");
    }

    public static void SessionMsg(object message)
    {
        Msg(Campaign.Session, message);
    }
}
