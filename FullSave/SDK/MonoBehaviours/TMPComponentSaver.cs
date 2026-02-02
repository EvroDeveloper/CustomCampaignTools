#if MELONLOADER
using MelonLoader;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppTMPro;
using Il2CppUltEvents;
#else
using TMPro;
using UltEvents;
#endif
using UnityEngine;

namespace FullSave.SDK.MonoBehaviours
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class TMP_TextSaver : MonoBehaviour
    {
#if MELONLOADER
        public Il2CppReferenceField<TMP_Text> _textMeshPro;
        public TMP_Text textMeshPro => _textMeshPro.Get();
        public Il2CppReferenceField<UltEventHolder> _onStateRestored;
        public UltEventHolder onStateRestored => _onStateRestored.Get();
#else
        public TMP_Text _textMeshPro;
        public UltEventHolder _onStateRestored;
#endif
    }
}
