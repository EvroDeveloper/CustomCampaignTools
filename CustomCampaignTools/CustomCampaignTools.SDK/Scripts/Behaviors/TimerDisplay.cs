#if MELONLOADER
using MelonLoader;
using CustomCampaignTools;
using Il2CppTMPro;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppInterop.Runtime.Attributes;
#else
using TMPro;
#endif
using System;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Timer Display")]
#endif
    public class TimerDisplay : MonoBehaviour
    {
#if MELONLOADER
        public TimerDisplay(IntPtr ptr) : base(ptr) { }

        public Il2CppReferenceField<TMP_Text> textDisplay;
#else
        public TMPro.TextMeshProUGUI textDisplay;
#endif

#if MELONLOADER
        void Awake()
        {
            if (textDisplay.Get() == null)
            {
                TryGetComponent<TMP_Text>(out var tmp);
                textDisplay.Set(tmp);
            }
        }
#endif

        public void DisplayLevelTime()
        {
#if MELONLOADER
#endif
        }

        public void DisplayTrialTime(string trialKey, TimerDisplayType displayType = TimerDisplayType.Best)
        {
#if MELONLOADER
            if (textDisplay.Get() == null || !Campaign.SessionActive) return;

            float time = Campaign.Session.saveData.GetTrialTime(trialKey);
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            textDisplay.Get().text = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                timeSpan.Minutes,
                timeSpan.Seconds,
                timeSpan.Milliseconds
                );
#endif
        }
    }

    public enum TrialTimeDisplayType
    {
        Best,
        Latest,
        Average
    }
}