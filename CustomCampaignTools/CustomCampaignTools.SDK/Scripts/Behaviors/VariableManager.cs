#if MELONLOADER
using Il2CppUltEvents;
using MelonLoader;
#else
using UltEvents;
#endif
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#else
    [AddComponentMenu("CustomCampaignTools/UltEvent Utilities/Variable Manager")]
#endif
    public class VariableManager : MonoBehaviour
    {
#if MELONLOADER
        public VariableManager(IntPtr ptr) : base(ptr) { }

        CampaignSaveData _campaignSaveData;

        void Update()
        {
            if (_campaignSaveData == null && Campaign.Session != null)
            {
                _campaignSaveData = Campaign.Session.saveData;
            }
        }
#endif

        public void SetValue(string key, float value)
        {
#if MELONLOADER
            Campaign.Session.saveData.SetValue(key, value);
#endif
        }
        public void IncrementValue(string key, float value)
        {
#if MELONLOADER
            Campaign.Session.saveData.SetValue(key, Campaign.Session.saveData.GetValue(key) + value);
#endif
        }
        public float GetValue(string key)
        {
#if MELONLOADER
            return Campaign.Session.saveData.GetValue(key);
#else
            return 0f;
#endif
        }
        public void InvokeIf(string key, ComparisonType comparison, float compareValue, UltEventHolder ultEvent)
        {
#if MELONLOADER
            float value = Campaign.Session.saveData.GetValue(key);
            bool evaluation = false;
            switch (comparison)
            {
                case ComparisonType.Equal:
                    evaluation = value == compareValue;
                    break;
                case ComparisonType.NotEqual:
                    evaluation = value != compareValue;
                    break;
                case ComparisonType.GreaterThan:
                    evaluation = value > compareValue;
                    break;
                case ComparisonType.GreaterThanOrEqual:
                    evaluation = value >= compareValue;
                    break;
                case ComparisonType.LessThan:
                    evaluation = value < compareValue;
                    break;
                case ComparisonType.LessThanOrEqual:
                    evaluation = value <= compareValue;
                    break;
                default:
                    break;
            }

            if (evaluation)
            {
                ultEvent.Invoke();
            }
#endif
        }
    }

    public enum ComparisonType
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
    }
}