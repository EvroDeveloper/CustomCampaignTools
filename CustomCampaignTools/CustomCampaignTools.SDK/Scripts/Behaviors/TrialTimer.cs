#if MELONLOADER
using MelonLoader;
using CustomCampaignTools.Timing;
#endif
using UnityEngine;
using System;

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
#endif
        public void StartTimer(string trialKey)
        {
#if MELONLOADER
            LevelTiming.StartTrialTimer(trialKey);
#endif
        }

        public float EndTimer(string trialKey)
        {
#if MELONLOADER
            return LevelTiming.EndTrialTimer(trialKey);
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