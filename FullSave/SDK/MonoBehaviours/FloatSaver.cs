using System;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppUltEvents;
using UnityEngine;

namespace FullSave.SDK.MonoBehaviours
{
    public class FloatSaver : MonoBehaviour
    {
#if MELONLOADER
        public Il2CppValueField<float> _float;
        public float floatValue
        {
            get => _float.Get();
            set => _float.Set(value);
        }
        public Il2CppReferenceField<UltEventHolder> _onStateRestored;
        public UltEventHolder onStateRestored => _onStateRestored.Get();
#else
        public float _float;
        public UltEventHolder _onStateRestored;
#endif

        public void SetValue(float value)
        {
#if MELONLOADER
            _float.Set(value);
#endif
        }

        public float GetValue()
        {
#if MELONLOADER
            return _float.Get();
#else
            return 0f;
#endif
        }
    }
}
