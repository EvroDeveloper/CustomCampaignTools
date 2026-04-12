#if MELONLOADER
using MelonLoader;
using CustomCampaignTools;
using Il2CppUltEvents;
using Il2CppInterop.Runtime.InteropTypes.Fields;
#else
using UltEvents;
#endif
using UnityEngine;

namespace VoidTakeoverSupport
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class InvokeIfFun : MonoBehaviour
    {
#if MELONLOADER
        public Il2CppValueField<int> _minInclusive;
        public int minInclusive => _minInclusive;
        public Il2CppValueField<int> _maxInclusive;
        public int maxInclusive => maxInclusive;
        public Il2CppReferenceField<UltEventHolder> _targetEvent;
        public UltEventHolder targetEvent => _targetEvent;
        public Il2CppValueField<bool> _invokeOnAwake;
        public bool invokeOnAwake => _invokeOnAwake;
        private const string funValueKey = "fun";
        private const int minFunValue = 1;
        private const int maxFunValue = 100;
#else
        public int _minInclusive;
        public int _maxInclusive;
        public UltEventHolder _targetEvent;
        public bool invokeOnAwake;
#endif

#if MELONLOADER
        private void Awake()
        {
            if(invokeOnAwake)
            {
                TryInvoke();
            }
        }
#endif

        public void TryInvoke()
        {
#if MELONLOADER
            if(!Campaign.SessionActive) return;
            int funValue = GetFunValue();
            if(minInclusive <= funValue && maxInclusive >= funValue) targetEvent.Invoke();
#endif
        }

        public int GetFunValue()
        {
#if MELONLOADER
            return FunValueManager.GetFunValue(Campaign.Session);
#else
            return 0;
#endif
        }

        public void RefreshFunValue()
        {
#if MELONLOADER
            FunValueManager.RefreshFunValue(Campaign.Session);
#endif
        }

#if MELONLOADER
        private void EnsureFunValue()
        {
            if(!Campaign.SessionActive) return;

            if(Campaign.Session.saveData.GetValue(funValueKey) == 0f)
                Campaign.Session.saveData.SetValue(funValueKey, Random.Range(minFunValue, maxFunValue + 1));
        }
#endif
    }
}
