using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Warehouse;
using Labworks.Bonemenu;
using Labworks.Utilities;
using MelonLoader;
using Il2CppSLZ.Marrow.SceneStreaming;
using Il2CppSLZ.Marrow.Zones;
using Il2CppUltEvents;
using CustomCampaignTools;

namespace Labworks.Behaviors
{
    [RegisterTypeInIl2Cpp]
    public class LabWorksContinueButton : MonoBehaviour
    {
        public LabWorksContinueButton(IntPtr ptr) : base(ptr) { }

        ZoneLoadLevel levelLoader;
        UltEventHolder invokeLoad;

        void Start()
        {
            Campaign campaign = Campaign.GetFromName(gameObject.name);

            if (!campaign.saveData.LoadedSavePoint.IsValid(out bool hasSpawnPoint))
                return;

            transform.GetChild(0).gameObject.SetActive(true);

            levelLoader = GetComponentInChildren<ZoneLoadLevel>();
            invokeLoad = GetComponentInChildren<UltEventHolder>();

            levelLoader.level = new LevelCrateReference(campaign.saveData.LoadedSavePoint.LevelBarcode);
        }

        public void OnButtonPressed()
        {
            SavepointFunctions.WasLastLoadByContinue = true;
            invokeLoad.Invoke();
        }
    }
}