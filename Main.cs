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
            // Load Save Data
            LabworksSaving.LoadFromDisk();

            BoneMenuCreator.CreateBoneMenu();
            BoneLib.Hooking.OnLevelInitialized += LevelInitialized;
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string palletTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcodeTitle = SceneStreamer.Session.Level.Barcode.ID;
#if DEBUG
            MelonLogger.Msg("Level initialized: " + palletTitle + " " + barcodeTitle);
#endif

            #region Save Data
            if (LevelParsing.IsLabworksCampaign(palletTitle, barcodeTitle))
            {
#if DEBUG
                MelonLogger.Msg("Level is Labworks!");
#endif
                
                int levelIndex = LevelParsing.GetLevelIndex(barcodeTitle);
                string previousLevelBarcode = LevelParsing.GetLevelBarcodeByIndex(levelIndex - 1);
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
                    SavepointFunctions.SavePlayer(barcodeTitle, Vector3.zero);
                }
            }
            #endregion
        }
    }
}
