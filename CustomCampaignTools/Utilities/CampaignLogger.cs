using BoneLib.Notifications;
using CustomCampaignTools.Notifications;
using CustomCampaignTools.Utilities;
using MelonLoader;

namespace CustomCampaignTools.Debug;

public class CampaignLogger : ILoggerSource
{
    private static readonly MelonLogger.Instance loggerInstance = new("CustomCampaignTools");

#if DEBUG
    static bool EnableLogging = true;
#else
    static bool EnableLogging = false;
#endif
    private static bool VerboseLogging = true;

    private static bool EnableErrorNotifications = false;

    /// <summary>
    /// Prints a message to the Console when built with DEBUG or forced is enabled
    /// </summary>
    /// <param name="message"></param>
    /// <param name="force">If true, sends message to console even without Debug mode</param>
    public static void Msg(object message, bool force = false)
    {
        if (!EnableLogging && !force) return;
        Msg(instance, message, force);
    }

    /// <summary>
    /// Prints a Campaign message to the Console if the campaign is a Dev build, when built with DEBUG, or forced is enabled
    /// </summary>
    /// <param name="campaign">The Campaign to send a log for</param>
    /// <param name="message"></param>
    /// <param name="force">If true, sends message to console even without Debug or Dev mode</param>
    public static void Msg(ILoggerSource campaign, object message, bool force = false)
    {
        campaign ??= instance;
        if (!EnableLogging && !campaign.ForceMessages() && !force) return;
        loggerInstance.Msg($"[{campaign.GetName()}] {message}");
    }

    /// <summary>
    /// Prints a verbose message to the Console when built with DEBUG or forced is enabled
    /// </summary>
    /// <param name="message"></param>
    public static void MsgVerbose(object message)
    {
        if (!VerboseLogging) return;
        MsgVerbose(instance, message);
    }

    /// <summary>
    /// Prints a verbose Campaign message to the Console if the campaign is a Dev build, when built with DEBUG, or forced is enabled
    /// </summary>
    /// <param name="loggerSource">The ILoggerSource to send a log for</param>
    /// <param name="message"></param>
    public static void MsgVerbose(ILoggerSource loggerSource, object message)
    {
        if (!VerboseLogging) return;

        loggerSource ??= instance;
        loggerInstance.Msg($"[{loggerSource.GetName()}] {message}");
    }

    /// <summary>
    /// Prints an Error from a specific campaign
    /// </summary>
    /// <param name="campaign"></param>
    /// <param name="message"></param>
    public static void Error(ILoggerSource loggerSource, object message, bool forceNotify = false)
    {
        loggerSource ??= instance;
        loggerInstance.Error($"[{loggerSource.GetName()}] {message}");
        if(EnableErrorNotifications || forceNotify) new LogErrorNotification(loggerSource, message).PlayNotification();
    }

    /// <summary>
    /// Prints an Error from CampaignLogger
    /// </summary>
    /// <param name="message"></param>
    public static void Error(object message)
    {
        Error(instance, message);
    }

    /// <summary>
    /// Shorthand for CampaignLogger.Msg(Campaign.Session, message, force);
    /// </summary>
    /// <param name="message"></param>
    public static void SessionMsg(object message, bool force = false)
    {
        Msg(Campaign.Session, message, force);
    }


    [CampaignArgument("-customcampaigntools.debug")]
    private static void EnterDebugMode()
    {
        EnableLogging = true;
    }

    private static CampaignLogger instance = new CampaignLogger();
    
    public string GetName() => "CampaignLogger";
    public bool ForceMessages() => false;
}

public interface ILoggerSource
{
    public string GetName();
    public bool ForceMessages();
}
