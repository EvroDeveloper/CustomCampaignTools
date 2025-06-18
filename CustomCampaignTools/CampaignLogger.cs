using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
