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
        private static MelonLogger.Instance loggerInstance = new MelonLogger.Instance("CustomCampaignTools");
        const bool EnableDebug = true;
        public static void Msg(object message, bool force = false)
        {
            if (!EnableDebug && !force) return;
            MelonLogger.Msg($"[CampaignLogger] {message}");
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
            MelonLogger.Error($"[CampaignLogger] {message}");
        }

        public static void SessionMsg(object message)
        {
            Msg(Campaign.Session, message);
        }
    }
}
