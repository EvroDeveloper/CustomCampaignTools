using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
#if MELONLOADER
using SLZ.Bonelab;
using SLZ.Marrow.Warehouse;
using Labworks.Bonemenu;
using Labworks.Utilities;
using Labworks.Data;
using MelonLoader;
using SLZ.Marrow.SceneStreaming;
#endif

namespace Labworks.Behaviors
{
#if MELONLOADER
    [RegisterTypeInIl2Cpp]
#endif
    public class LabWorksContinueButton : MonoBehaviour
    {
#if MELONLOADER
        public LabWorksContinueButton(IntPtr ptr) : base(ptr) { }

        ButtonsToHubAndReset loader;
#endif

        void Start()
        {
#if MELONLOADER
            if (!SaveParsing.IsSavePointValid(LabworksSaving.LoadedSavePoint))
                return;

            transform.GetChild(0).gameObject.SetActive(true);

            loader = gameObject.AddComponent<ButtonsToHubAndReset>();
            loader.loadScreenLevel = new LevelCrateReference("volx4.LabWorksBoneworksPort.Level.BoneworksLoadingScreen");
            loader.hubCrate = new LevelCrateReference("c2534c5a-6b79-40ec-8e98-e58c5363656e");
            loader.nextLevel = new LevelCrateReference(LabworksSaving.LoadedSavePoint.LevelBarcode);

            loader.vfxFadeOutSpawnable = new SLZ.Marrow.Data.Spawnable() { 
                crateRef = new SpawnableCrateReference("c1534c5a-dac0-44a1-b656-6c235646584c")
            };
#endif
        }

        public void OnButtonPressed()
        {
#if MELONLOADER
            SavepointFunctions.WasLastLoadByContinue = true;

            loader.NextLevel();
#endif
        }
    }
}