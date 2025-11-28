#if MELONLOADER
using MelonLoader;
using CustomCampaignTools.Timing;
#endif
using UnityEngine;
using System;
using System.Collections.Generic;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Trial Timer")]
#endif
    public class TrialTimer : MonoBehaviour
    {
#if MELONLOADER
        public TrialTimer(IntPtr ptr) : base(ptr) { }

        public HashSet<string> activeTimers = new();

        private void OnDestroy()
        {
            foreach(var timer in activeTimers)
            {
                EndTimer(timer);
            }
        }
#endif
        public void StartTimer(string trialKey)
        {
#if MELONLOADER
            activeTimers.Add(trialKey);
            LevelTiming.StartTrialTimer(trialKey);
#endif
        }

        public float EndTimer(string trialKey)
        {
#if MELONLOADER
            activeTimers.Remove(trialKey);
            return LevelTiming.EndTrialTimer(trialKey, out _);
#else
            return 0f;
#endif
        }
        
        public bool EndTimer_IsBest(string trialKey)
                {
#if MELONLOADER
            activeTimers.Remove(trialKey);
            LevelTiming.EndTrialTimer(trialKey, out bool best);
            return best;
#else
            return false;
#endif

        }

        public void PauseTimer(string trialKey)
        {
#if MELONLOADER
            LevelTiming.PauseTrialTimer(trialKey);
#endif
        }

        public void ResumeTimer(string trialKey)
        {
#if MELONLOADER
            LevelTiming.ResumeTrialTimer(trialKey);
#endif
        }
    }
}