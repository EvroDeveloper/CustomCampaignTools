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

    /// <summary>
    /// Prints a message to the Console when built with DEBUG or forced is enabled
    /// </summary>
    /// <param name="message"></param>
    /// <param name="force">If true, sends message to console even without Debug mode</param>
    public static void Msg(object message, bool force = false)
    {
        if (!EnableLogging && !force) return;
        loggerInstance.Msg($"[CampaignLogger] {message}");
    }

    /// <summary>
    /// Prints a Campaign message to the Console if the campaign is a Dev build, when built with DEBUG, or forced is enabled
    /// </summary>
    /// <param name="campaign">The Campaign to send a log for</param>
    /// <param name="message"></param>
    /// <param name="force">If true, sends message to console even without Debug or Dev mode</param>
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

    /// <summary>
    /// Prints a verbose message to the Console when built with DEBUG or forced is enabled
    /// </summary>
    /// <param name="message"></param>
    public static void MsgVerbose(object message)
    {
        if (!VerboseLogging) return;
        loggerInstance.Msg($"[CampaignLogger] {message}");
    }

    /// <summary>
    /// Prints a verbose Campaign message to the Console if the campaign is a Dev build, when built with DEBUG, or forced is enabled
    /// </summary>
    /// <param name="campaign">The Campaign to send a log for</param>
    /// <param name="message"></param>
    public static void MsgVerbose(Campaign campaign, object message)
    {
        if (campaign != null)
        {
            if (!VerboseLogging) return;
            loggerInstance.Msg($"[CampaignLogger - {campaign.Name}] {message}");
        }
        else Msg(message);
    }

    /// <summary>
    /// Prints an Error from a specific campaign
    /// </summary>
    /// <param name="campaign"></param>
    /// <param name="message"></param>
    public static void Error(Campaign campaign, object message)
    {
        if(campaign != null) loggerInstance.Error($"[CampaignLogger - {campaign.Name}] {message}");
        else Error(message);
    }

    /// <summary>
    /// Prints an Error from CampaignLogger
    /// </summary>
    /// <param name="message"></param>
    public static void Error(object message)
    {
        loggerInstance.Error($"[CampaignLogger] {message}");
    }

    /// <summary>
    /// Shorthand for CampaignLogger.Msg(Campaign.Session, message, force);
    /// </summary>
    /// <param name="message"></param>
    public static void SessionMsg(object message, bool force = false)
    {
        Msg(Campaign.Session, message, force);
    }
}
