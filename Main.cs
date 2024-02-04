using MelonLoader;
using BoneLib;
using SLZ.Marrow.Warehouse;
using System;
using UnityEngine;
using SLZ.Marrow.SceneStreaming;
using System.Collections.Generic;

namespace Labworks_Ammo_Saver
{
    internal partial class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
        }

        public override void OnLateInitializeMelon()
        {
            base.OnLateInitializeMelon();
            BonelibCreator.CreateBoneMenu();

            BoneLib.Hooking.OnLevelInitialized += LevelInitialized;
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string palletTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcodeTitle = SceneStreamer.Session.Level.Barcode;
            if (palletTitle == "LabWorksBoneworksPort" && barcodeTitle != "volx4.LabWorksBoneworksPort.Level.BoneworksRedactedChamber" && barcodeTitle != "volx4.LabWorksBoneworksPort.Level.BoneworksMainMenu" && barcodeTitle != "volx4.LabWorksBoneworksPort.Level.BoneworksLoadingScreen")
            {
                MelonLogger.Msg("Level is Labworks!");
                AmmoFunctions.LoadAmmoAtLevel(AmmoFunctions.GetLevelIndexFromBarcode(barcodeTitle));

                if (SavepointFunctions.WasLastLoadByContinue)
                {
                    SavepointFunctions.LoadPlayerFromSave();
                }
                else
                {
                    MelonLogger.Msg("Loaded into a map without continue, saving default at scene " + AmmoFunctions.GetLevelIndexFromBarcode(barcodeTitle));
                    SavepointFunctions.SavePlayer(AmmoFunctions.GetLevelIndexFromBarcode(barcodeTitle), Vector3.zero);
                }
            }

            if (SceneStreamer.Session.Level.Title == "15 - Void G114")
            {
                AssetBundle Contentbundle = 

                GameObject shop = GameObject.Instantiate(FusionContentLoader.PointShopPrefab);


                shop.SetActive(false);
                shop.transform.position = Vector3.zero;
                shop.transform.rotation = Quaternion.identity;
                shop.transform.localScale = Vector3.one;
                shop.SetActive(true);
            }
        }
    }
}
