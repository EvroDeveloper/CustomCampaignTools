using System.Collections.Generic;
using MelonLoader;
using SLZ.Marrow.SceneStreaming;
using System;
using System.Collections;
using UnityEngine;
using SLZ.Bonelab;
using SLZ.Marrow.Warehouse;

namespace Labworks_Ammo_Saver
{
    [RegisterTypeInIl2Cpp]
    public class LabWorksContinueButton : MonoBehaviour
    {
        public LabWorksContinueButton(IntPtr ptr) : base(ptr) { }

        ButtonsToHubAndReset loader;

        void Start()
        {
            if (BonelibCreator.savePoint.Value == null) return; 

            transform.GetChild(0).gameObject.SetActive(true);

            List<float> saveData = BonelibCreator.savePoint.Value;

            loader = gameObject.AddComponent<ButtonsToHubAndReset>();
            loader.loadScreenLevel = new LevelCrateReference("volx4.LabWorksBoneworksPort.Level.BoneworksLoadingScreen");
            loader.hubCrate = new LevelCrateReference("c2534c5a-6b79-40ec-8e98-e58c5363656e");
            loader.nextLevel = new LevelCrateReference(AmmoFunctions.levelBarcodes[(int)saveData[0]]);

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