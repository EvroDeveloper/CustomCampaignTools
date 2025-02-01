using Il2CppUltEvents;
using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace CustomCampaignTools.SDK
{
    [RegisterTypeInIl2Cpp]
    public class VariableManager : MonoBehaviour
    {
        public VariableManager(IntPtr ptr) : base(ptr) { }

        CampaignSaveData _campaignSaveData;

        void Awake()
        {
            _campaignSaveData = Campaign.Session.saveData;
        }

        public void SetValue(string key, float value)
        {
            _campaignSaveData.SetValue(key, value);
        }
        public void IncrementValue(string key, float value)
        {
            _campaignSaveData.SetValue(key, _campaignSaveData.GetValue(key) + value);
        }
        public float GetValue(string key)
        {
            return _campaignSaveData.GetValue(key);
        }
        public void InvokeIf(string key, ComparisonType comparison, float compareValue, UltEventHolder ultEvent)
        {
            float value = _campaignSaveData.GetValue(key);
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