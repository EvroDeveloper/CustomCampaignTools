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
        ZoneLevelLoader levelLoader;
        UltEventHolder invokeLoad;

        void Start()
        {
            if (!SaveParsing.IsSavePointValid(LabworksSaving.LoadedSavePoint, out bool hasSpawnPoint))
                return;

            transform.GetChild(0).gameObject.SetActive(true);

            levelLoader = GetComponentInChildren<ZoneLevelLoader>();
            invokeLoad = GetComponentInChildren<UltEventHolder>();

            levelLoader.level = new LevelCrateReference(LabworksSaving.LoadedSavePoint.LevelBarcode);
        }

        public void OnButtonPressed()
        {
            SavepointFunctions.WasLastLoadByContinue = true;
            invokeLoad.Invoke();
        }
    }
}