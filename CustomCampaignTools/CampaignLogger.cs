using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomCampaignTools;

namespace CustomCampaignTools.Debug
{
    public class CampaignLogger
    {
        const bool EnableDebug = false;
        public static void Msg(object message)
        {
            if (!EnableDebug) return;
            MelonLogger.Msg($"[CampaignLogger] {message}");
        }

        public static void Msg(Campaign campaign, object message)
        {
            if(campaign == null)
            {
                Msg(message);
                return;
            }
            if (!EnableDebug && !campaign.DEVMODE) return;
            MelonLogger.Msg($"[CampaignLogger - {campaign.Name}] {message}");
        }

        public static void SessionMsg(object message)
        {
            Msg(Campaign.Session, message);
        }
    }
}
