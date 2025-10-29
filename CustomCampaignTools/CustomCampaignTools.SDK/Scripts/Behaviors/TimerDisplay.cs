#if MELONLOADER
using MelonLoader;
using CustomCampaignTools;
using CustomCampaignTools.Timing;
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
        private bool _displayHours => displayHours.Get();

        public Il2CppValueField<bool> displayMinutes;
        private bool _displayMinutes => displayMinutes.Get();

        public Il2CppValueField<bool> displaySeconds;
        private bool _displaySeconds => displaySeconds.Get();

        public Il2CppValueField<bool> displayMilliseconds;
        private bool _displayMilliseconds => displayMilliseconds.Get();

        public Il2CppValueField<int> millisecondsDepth;
        private int _millisecondsDepth => millisecondsDepth.Get();

        public Il2CppValueField<bool> alwaysUpdate;
        private bool _alwaysUpdate => alwaysUpdate.Get();

        public Il2CppValueField<int> serializedTimerDisplayType;
        private TimerDisplayType timerDisplayType => (TimerDisplayType)serializedTimerDisplayType.Get();

        public Il2CppValueField<int> serializedTrialDisplayType;
        private TrialTimeDisplayType trialDisplay => (TrialTimeDisplayType)serializedTrialDisplayType.Get();

        private string textPrefix = "";
        private string textPostfix = "";
        private string targetLevelBarcode = "";
        private string targetTrial = "";
#else
        public TMPro.TMP_Text textDisplay;
        public TimerDisplayType displayType;
        public int serializedTimerDisplayType;
        public TrialTimeDisplayType trialDisplayType;
        public int serializedTrialDisplayType;

        [Tooltip("Always update the Timer Display. May be laggy perhaps, i wouldnt recommend. but its an option ig")]
        public bool alwaysUpdate = false;

        [Tooltip("Displays Hours (if greater than zero)")]
        public bool displayHours = true;

        [Tooltip("Displays Minutes")]
        public bool displayMinutes = true;

        [Tooltip("Displays Seconds")]
        public bool displaySeconds = true;

        [Tooltip("Displays milliseconds (if applicable)")]
        public bool displayMilliseconds = true;

        [Tooltip("Number of decimal places to show for Milliseconds")]
        public int millisecondsDepth = 2;
#endif

#if MELONLOADER
        void Awake()
        {
            if (textDisplay.Get() == null)
            {
                if(TryGetComponent<TMP_Text>(out var tmp))
                {
                    textDisplay.Set(tmp);
                }
            }
        }

        void Start()
        {
            UpdateText();
        }

        void Update()
        {
            if(_alwaysUpdate)
            {
                UpdateText();
            }
        }
#else
        public void OnValidate()
        {
            serializedTimerDisplayType = (int)displayType;
            serializedTrialDisplayType = (int)trialDisplayType;
        }
#endif

        public void UpdateText()
        {
#if MELONLOADER
            if(textDisplay.Get() == null) return;
            if(!Campaign.SessionActive) return;

            if(timerDisplayType == TimerDisplayType.TotalCampaignTime)
            {
                DisplayCampaignTime();
            }
            else if (timerDisplayType == TimerDisplayType.LevelTime)
            {
                DisplayLevelTime();
            }
            else if (timerDisplayType == TimerDisplayType.TrialTime)
            {
                DisplayTrialTime();
            }
#endif
        }
        
        public void SetTargetLevel(string barcode)
        {
#if MELONLOADER
            targetLevelBarcode = barcode;
#endif
        }
        
        public void SetTargetTrial(string trialKey)
        {
#if MELONLOADER
            targetTrial = trialKey;
#endif
        }

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

#if MELONLOADER
        public void DisplayCampaignTime()
        {
            int time = Campaign.Session.saveData.GetTotalCampaignTime();
            if(_alwaysUpdate) time += LevelTiming.GetMainTime();
            DisplayTimeSpan(time);
        }

        public void DisplayLevelTime()
        {
            int time = Campaign.Session.saveData.GetLevelTime(targetLevelBarcode);
            if(_alwaysUpdate && Campaign.lastLoadedCampaignLevel == targetLevelBarcode) time += LevelTiming.GetMainTime();
            DisplayTimeSpan(time);
        }

        public void DisplayTrialTime()
        {
            if (!Campaign.SessionActive) return;

            if (trialDisplay == TrialTimeDisplayType.Best) DisplayTimeSpan(Campaign.Session.saveData.GetTrialBest(targetTrial));
            else if (trialDisplay == TrialTimeDisplayType.Latest) DisplayTimeSpan(Campaign.Session.saveData.GetTrialLatest(targetTrial));
            else if (trialDisplay == TrialTimeDisplayType.Average) DisplayTimeSpan(Campaign.Session.saveData.GetTrialAverage(targetTrial));
        }

        private void DisplayTimeSpan(int time)
        {
            if (textDisplay.Get() == null) return;

            int seconds = time;
            int minutes = 0;
            int hours = 0; 
            if(_displayMinutes)
            {
                minutes = Mathf.FloorToInt(time / 60);
                seconds = seconds % 60;
            }
            if(_displayHours)
            {
                hours = Mathf.FloorToInt(time / (60*60));
                minutes = minutes % 60;
                seconds = seconds % (60*60);
            }

            string output = "";
            if(_displayHours && hours > 0) output += $"{hours}:";
            if(_displayMinutes) output += $"{minutes:D2}:";
            if(_displaySeconds) output += $"{seconds:D2}";

            if (output.EndsWith(":"))
                output = output.Substring(0, output.Length - 1);

            textDisplay.Get().text = output;
        }

        private void DisplayTimeSpan(float rawTime)
        {
            if (textDisplay.Get() == null) return;

            float time = Mathf.Max(time, 0f);

            int msMultForRound = (int)Mathf.Pow(10, _millisecondsDepth);
            int milliseconds = Mathf.FloorToInt((time % 1f) * msMultForRound);

            int seconds = (int)time;
            int minutes = 0;
            int hours = 0; 
            if(_displayMinutes)
            {
                minutes = Mathf.FloorToInt(time / 60);
                seconds = seconds % 60;
            }
            if(_displayHours)
            {
                hours = Mathf.FloorToInt(time / (60*60));
                minutes = minutes % 60;
                seconds = seconds % (60*60);
            }

            string output = "";
            if(_displayHours && hours > 0) output += $"{hours}:";
            if(_displayMinutes) output += $"{minutes:D2}:";
            if(_displaySeconds) output += $"{seconds:D2}";
            if(_displayMilliseconds) output += $".{milliseconds.ToString($"D{_millisecondsDepth}")}";
            
            if (output.EndsWith(":"))
                output = output.Substring(0, output.Length - 1);

            textDisplay.Get().text = textPrefix + output + textPostfix;
        }
#endif
    }

    public enum TimerDisplayType
    {
        TotalCampaignTime,
        LevelTime,
        TrialTime,
    }

    public enum TrialTimeDisplayType
    {
        Best,
        Latest,
        Average
    }
}