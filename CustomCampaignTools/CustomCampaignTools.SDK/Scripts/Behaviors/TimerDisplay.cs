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
        public Il2CppValueField<bool> displayHours;
        public Il2CppValueField<bool> displayMinutes;
        public Il2CppValueField<bool> displaySeconds;
        public Il2CppValueField<bool> displayMilliseconds;
        private string textPrefix = "";
        private string textPostfix = "";
#else
        public TMPro.TMP_Text textDisplay;

        [Tooltip("Displays Hours (if greater than one)")]
        public bool displayHours = true;

        [Tooltip("Displays Minutes")]
        public bool displayMinutes = true;

        [Tooltip("Displays Seconds")]
        public bool displaySeconds = true;

        [Tooltip("Displays milliseconds (if applicable)")]
        public bool displayMilliseconds = true;
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

        public void SetTextPrefix(string prefix)
        {
#if MELONLOADER
            textPrefix = prefix;
#endif
        }

        public void SetTextPostfix(string postfix)
        {
#if MELONLOADER
            textPostfix = postfix;
#endif
        }

        public void DisplayCampaignTime()
        {
#if MELONLOADER
            if (!Campaign.SessionActive) return;

            int time = Campaign.Session.saveData.GetTotalCampaignTime();
            DisplayTimeSpan(time);
#endif
        }

        public void DisplayLevelTime(string levelBarcode)
        {
#if MELONLOADER
            if (!Campaign.SessionActive) return;

            int time = Campaign.Session.saveData.GetLevelTime(levelBarcode);
            DisplayTimeSpan(time);
#endif
        }

        public void DisplayTrialTime(string trialKey, TrialTimeDisplayType displayType = TrialTimeDisplayType.Best)
        {
#if MELONLOADER
            if (!Campaign.SessionActive) return;

            if (displayType == TrialTimeDisplayType.Best) DisplayTimeSpan(Campaign.session.saveData.GetTrialBest(trialKey));
            else if (displayType == TrialTimeDisplayType.Latest) DisplayTimeSpan(Campaign.session.saveData.GetTrailLatest(trialKey));
            else if (displayType == TrialTimeDisplayType.Average) DisplayTimeSpan(Campaign.session.saveData.GetTrialAverage(trialKey));
#endif
        }

#if MELONLOADER

        private void DisplayTimeSpan(int time)
        {
            if (textDisplay.Get() == null) return;

            int seconds = time;
            int minutes = 0;
            int hours = 0; 
            if(displayMinutes)
            {
                minutes = Mathf.FloorToInt(time / 60);
                seconds = seconds % 60;
            }
            if(displayHours)
            {
                hours = Mathf.FloorToInt(time / (60*60));
                seconds = seconds % (60*60);
            }

            string output = "";
            if(displayHours && hours > 0) output += $"{hours}:";
            if(displayMinutes) output += $"{minutes}:";
            if(displaySeconds) output += $"{seconds}";

            if (output.EndsWith(":"))
                output = output.Substring(0, output.Length - 1);

            textDisplay.Get().text = output;
        }

        private void DisplayTimeSpan(float time)
        {
            if (textDisplay.Get() == null) return;

            float milliseconds = time % 1f;
            int seconds = (int)time;
            int minutes = 0;
            int hours = 0; 
            if(displayMinutes)
            {
                minutes = Mathf.FloorToInt(time / 60);
                seconds = seconds % 60;
            }
            if(displayHours)
            {
                hours = Mathf.FloorToInt(time / (60*60));
                seconds = seconds % (60*60);
            }

            string output = "";
            if(displayHours && hours > 0) output += $"{hours}:";
            if(displayMinutes) output += $"{minutes}:";
            if(displaySeconds) output += $"{seconds}";
            if(displayMilliseconds) output += $"{milliseconds}";
            
            if (output.EndsWith(":"))
                output = output.Substring(0, output.Length - 1);

            textDisplay.Get().text = textPrefix + output + textPostfix;
        }
#endif
    }

    public enum TrialTimeDisplayType
    {
        Best,
        Latest,
        Average
    }
}