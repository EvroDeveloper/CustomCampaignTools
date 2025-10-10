using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCampaignTools.Timer
{
    public static class LevelTiming
    {

        public static void OnCampaignLevelLoaded(Campaign c, string levelBarcode)
        {

        }

        public static void OnCampaignLevelUnloaded(Campaign c, string levelBarcode)
        {
            // Take the current time, and find number of seconds since the level was started.
            // Add 
        }

        public static void StartRecordingTime()
        {

        }

        public static int StopRecordingTime()
        {
            return 0;
        }
    }
}
