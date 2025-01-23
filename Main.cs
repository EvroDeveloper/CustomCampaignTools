using MelonLoader;
using BoneLib;
using SLZ.Marrow.Warehouse;
using System;
using UnityEngine;
using SLZ.Marrow.SceneStreaming;
using System.Collections.Generic;
using Labworks.Utilities;
using Labworks.Bonemenu;
using Labworks.Data;

namespace Labworks
{
    internal class Main : MelonMod
    {
        public override void OnLateInitializeMelon()
        {
            // Create Bonemenu
            BoneMenuCreator.CreateBoneMenu();

            BoneLib.Hooking.OnLevelInitialized += LevelInitialized;

            RegisterCampaign("LabWorks", new string[] {
            "volx4.LabWorksBoneworksPort.Level.Boneworks01Breakroom",
            "volx4.LabWorksBoneworksPort.Level.Boneworks02Museum",
            "volx4.LabWorksBoneworksPort.Level.Boneworks03Streets",
            "volx4.LabWorksBoneworksPort.Level.Boneworks04Runoff",
            "volx4.LabWorksBoneworksPort.Level.Boneworks05Sewers",
            "volx4.LabWorksBoneworksPort.Level.Boneworks06Warehouse",
            "volx4.LabWorksBoneworksPort.Level.Boneworks07CentralStation",
            "volx4.LabWorksBoneworksPort.Level.Boneworks08Tower",
            "volx4.LabWorksBoneworksPort.Level.Boneworks09TimeTower",
            "volx4.LabWorksBoneworksPort.Level.Boneworks10Dungeon",
            "volx4.LabWorksBoneworksPort.Level.Boneworks11Arena",
            "volx4.LabWorksBoneworksPort.Level.Boneworks12ThroneRoom" });
            // Load Save Data
            LabworksSaving.LoadFromDisk();

            // Load Content
        }

        public override void OnInitializeMelon()
        {
            ContentLoader.OnBundleLoad();
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string palletTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcodeTitle = SceneStreamer.Session.Level.Barcode.ID;

            #region Save Data
            if (LevelParsing.IsCampaignLevel(palletTitle, barcodeTitle, out Campaign campaign))
            {
#if DEBUG
                MelonLogger.Msg("Level is Campaign!");
#endif

                
                int levelIndex = campaign.GetLevelIndex(barcodeTitle);
                string previousLevelBarcode = campaign.GetLevelBarcodeByIndex(levelIndex - 1);
                AmmoFunctions.LoadAmmoFromLevel(barcodeTitle, SavepointFunctions.WasLastLoadByContinue);

                if (SavepointFunctions.WasLastLoadByContinue)
                {
                    SavepointFunctions.LoadPlayerFromSave();
                }
                else
                {
#if DEBUG
                    MelonLogger.Msg("Loaded into a map without continue, saving default at scene " + barcodeTitle);
#endif
                    SavepointFunctions.SavePlayer(barcodeTitle, Vector3.zero, Vector3.zero);
                }
            }
            #endregion
        }
    }
}
