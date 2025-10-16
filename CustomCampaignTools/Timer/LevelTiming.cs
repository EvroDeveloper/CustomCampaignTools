using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCampaignTools.Timing
{
    public static class LevelTiming
    {
        private static Timer MainTimer = new Timer();
        private static Dictionary<string, Timer> TrialTimers = new();

        public static void OnCampaignLevelLoaded(Campaign c, string levelBarcode)
        {
            MainTimer.StartTimer();
        }

        public static void OnCampaignLevelUnloaded(Campaign c, string levelBarcode)
        {
            int seconds = (int)Timer.GetTimeSinceStart();
            c.saveData.AddTimeToLevel(levelBarcode, seconds);
        }

        public static void StartTrialTimer(string key)
        {
            if (!TrialTimers.ContainsKey(key))
            {
                TrialTimers[key] = new Timer();

                Timer TrialTimer = TrialTimers[key];
                TrialTimer.StartTimer();
            }
        }

        public static void PauseTrialTimer(string key)
        {
            if (!TrialTimers.ContainsKey(key)) return;

            Timer trialTimer = TrialTimers[key];
            trialTimer.PauseTimer();
        }

        public static void ResumeTrialTimer(string key)
        {
            if (!TrialTimers.ContainsKey(key)) return;

            Timer trialTimer = TrialTimers[key];
            trialTimer.ResumeTimer();
        }

        public static float EndTrialTimer(string key, bool save = true)
        {
            Timer trialTimer = TrialTimers[key];
            TrialTimers.Remove(key);

            float time = trialTimer.GetTimeSinceStart();

            if(save && Campaign.SessionActive)
            {
                Campaign.Session.saveData.AddTrialTime(key, time);
            }

            return time;
        }

        public static void SaveTrialTime(Campaign c, string key, float time)
        {
            c.saveData.AddTrialTime(key, time);
        }

        public static void ONGAMEPAUSE()
        {
            MainTimer.PauseTimer();
            foreach (var timer in TrialTimers.Values)
            {
                timer.PauseTimer();
            }
        }

        public static void ONGAMERESUME()
        {
            MainTimer.ResumeTimer();
            foreach (var timer in TrialTimers.Values)
            {
                timer.ResumeTimer();
            }
        }
    }
}
