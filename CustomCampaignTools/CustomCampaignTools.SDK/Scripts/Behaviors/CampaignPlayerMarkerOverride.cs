#if MELONLOADER
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
using Il2CppTMPro;
#else
using SLZ.Marrow.Warehouse;
using TMPro;
#endif
using UnityEngine;
using System;

namespace CustomCampaignTools.SDK
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class CampaignPlayerMarkerOverride : MonoBehaviour
    {
#if MELONLOADER
        public CampaignPlayerMarkerOverride(IntPtr ptr) : base(ptr) {}

        private SpawnableCrateReference _rigManagerOverride;
        private SpawnableCrateReference _gameplayRigOverride;
        public SpawnableCrateReference RigManagerOverride
        {
            get
            {
                _rigManagerOverride ??= new SpawnableCrateReference(GetCommentLine(0));
                return _rigManagerOverride;
            }
        }
        public SpawnableCrateReference GameplayRigOverride
        {
            get
            {
                _gameplayRigOverride ??= new SpawnableCrateReference(GetCommentLine(1));
                return _gameplayRigOverride;
            }
        }

        private string GetCommentLine(int lineIdx)
        {
            if(TryGetComponent<TMP_Text>(out var textHolder))
            {
                string[] lines = textHolder.text.Split('\n');
                return lines[lineIdx];
            }
            return "";
        }

#else
        public SpawnableCrateReference rigManagerOverride;
        public SpawnableCrateReference gameplayRigOverride;

        public void OnValidate()
        {
            // set comments // holy shit comment about comments this is crazy
            if(!TryGetComponent<TMP_Text>(out var textHolder))
            {
                textHolder.text = $"{rigManagerOverride.Barcode.ID}\n{gameplayRigOverride.Barcode.ID}";
            }
        }
#endif
    }
}

