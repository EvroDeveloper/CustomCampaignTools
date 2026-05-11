#if MELONLOADER
using MelonLoader;
using Il2CppUltEvents;
using Il2CppInterop.Runtime.InteropTypes.Fields;
#else
using UltEvents;
#endif
using System;
using UnityEngine;

namespace FullSave.SDK.MonoBehaviours
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class ObjectEnabledSaver : MonoBehaviour
    {
#if MELONLOADER
        public Il2CppReferenceField<UltEventHolder> _onStateRestored;
        public UltEventHolder onStateRestored => _onStateRestored.Get();
#else
        public UltEventHolder _onStateRestored;
#endif
    }
}
