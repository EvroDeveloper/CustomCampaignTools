using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using SLZ.Bonelab;
using SLZ.Marrow.Warehouse;
using Labworks.Bonemenu;
using Labworks.Utilities;
using Labworks.Data;
using MelonLoader;
using SLZ.Marrow.SceneStreaming;

namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class LabWorksContinueButton : MonoBehaviour
    {
        public LabWorksContinueButton(IntPtr ptr) : base(ptr) { }

        ButtonsToHubAndReset loader;

        void Start()
        {
            if (!SaveParsing.IsSavePointValid(LabworksSaving.LoadedSavePoint, out bool hasSpawnPoint))
                return;

            transform.GetChild(0).gameObject.SetActive(true);

            loader = gameObject.AddComponent<ButtonsToHubAndReset>();
            loader.loadScreenLevel = new LevelCrateReference("volx4.LabWorksBoneworksPort.Level.BoneworksLoadingScreen");
            loader.hubCrate = new LevelCrateReference("c2534c5a-6b79-40ec-8e98-e58c5363656e");
            loader.nextLevel = new LevelCrateReference(LabworksSaving.LoadedSavePoint.LevelBarcode);

            loader.vfxFadeOutSpawnable = new SLZ.Marrow.Data.Spawnable() { 
                crateRef = new SpawnableCrateReference("c1534c5a-dac0-44a1-b656-6c235646584c")
            };
        }

        public void OnButtonPressed()
        {
            SavepointFunctions.WasLastLoadByContinue = true;

            loader.NextLevel();
        }
    }
}