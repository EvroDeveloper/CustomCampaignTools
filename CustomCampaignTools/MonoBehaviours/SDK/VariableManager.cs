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

        void Update()
        {
            if(_campaignSaveData == null && Campaign.Session != null)
            {
                _campaignSaveData = Campaign.Session.saveData;
            }
        }

        public void SetValue(string key, float value)
        {
            Campaign.Session.saveData.SetValue(key, value);
        }
        public void IncrementValue(string key, float value)
        {
            Campaign.Session.saveData.SetValue(key, Campaign.Session.saveData.GetValue(key) + value);
        }
        public float GetValue(string key)
        {
            return Campaign.Session.saveData.GetValue(key);
        }
        public void InvokeIf(string key, ComparisonType comparison, float compareValue, UltEventHolder ultEvent)
        {
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