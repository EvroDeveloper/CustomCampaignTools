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
        public override void OnInitializeMelon()
        {
            BoneMenuCreator.CreateBoneMenu();
            BoneLib.Hooking.OnLevelInitialized += LevelInitialized;
        }

        public override void OnLateInitializeMelon()
        {
            LabworksSaving.LoadFromDisk();
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string palletTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcodeTitle = SceneStreamer.Session.Level.Barcode.ID;
#if DEBUG
            MelonLogger.Msg("Level initialized: " + palletTitle + " " + barcodeTitle);
#endif

            if (LevelParsing.IsLabworksCampaign(palletTitle, barcodeTitle))
            {
#if DEBUG
                MelonLogger.Msg("Level is Labworks!");
#endif

                AmmoFunctions.LoadAmmoAtLevel(barcodeTitle);

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
        }
    }
}
