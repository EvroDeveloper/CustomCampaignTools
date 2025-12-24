using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomCampaignTools;
using System.Security.Cryptography.X509Certificates;

namespace CustomCampaignTools.Debug
{
    public class CampaignLogger
    {
        private static readonly MelonLogger.Instance loggerInstance = new("CustomCampaignTools");

#if DEBUG
        const bool EnableDebug = true;
#else
        const bool EnableDebug = false;
#endif
        public static void Msg(object message, bool force = false)
        {
            if (!EnableDebug && !force) return;
            loggerInstance.Msg($"[CampaignLogger] {message}");
        }

        public static void Msg(Campaign campaign, object message, bool force = false)
        {
            if(campaign == null)
            {
                Msg(message, force);
                return;
            }
            if (!EnableDebug && !campaign.DEVMODE && !force) return;
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
}
