using MelonLoader;
using BoneLib;
using Il2CppSLZ.Marrow.Warehouse;
using System;
using UnityEngine;
using Il2CppSLZ.Marrow.SceneStreaming;
using System.Collections.Generic;
using CustomCampaignTools.Utilities;
using CustomCampaignTools.Bonemenu;
using CustomCampaignTools;
using System.Reflection;
using Il2CppSLZ.Bonelab;
using HarmonyLib;
using CustomCampaignTools.SDK;

namespace CustomCampaignTools
{
    internal class Main : MelonMod
    {

        public static Sprite CampaignSprite { get; private set; }

        public override void OnLateInitializeMelon()
        {
            // Create Bonemenu
            BoneMenuCreator.CreateBoneMenu();

            Campaign.OnInitialize();
            MainMenuMangler.OnInitialize();

            Hooking.OnLevelLoaded += LevelInitialized;
            Hooking.OnLevelUnloaded += LevelUnloaded;

            string resourceName = "CustomCampaignTools.Resources.CampaignIcon.png";
            Assembly assembly = MelonAssembly.Assembly;

            Sprite sprite = MainMenuMangler.LoadSpriteFromEmbeddedResource(resourceName, assembly, new Vector2(0.5f, 0.5f));

            //Hooking.CreateHook(typeof(HideOnAwake).GetMethod(nameof(HideOnAwake.Awake), AccessTools.all), typeof(UnhideInCampaign).GetMethod(nameof(UnhideInCampaign.OnHideOnAwakeAwoken), AccessTools.all));

        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string palletTitle = SceneStreamer.Session.Level.Pallet.Title;
            string barcode = info.barcode;

            #region Save Data
            if (LevelParsing.IsCampaignLevel(barcode, out Campaign campaign, out var levelType))
            {
                MelonLogger.Msg("Level is in Campaign!");
                if (levelType != CampaignLevelType.MainLevel) return;
                MelonLogger.Msg("Main Level!");

                int levelIndex = campaign.GetLevelIndex(barcode, CampaignLevelType.MainLevel);
                string previousLevelBarcode = campaign.GetLevelBarcodeByIndex(levelIndex - 1, CampaignLevelType.MainLevel);

            if (SavepointFunctions.WasLastLoadByContinue)
            {
                SavepointFunctions.LoadPlayerFromSave();
            }
            else
            {
                MelonLogger.Msg("Loaded into a map without continue, saving default at scene " + barcode);
                SavepointFunctions.SavePlayer(barcode, Vector3.zero, Vector3.zero);
            }
        }
            #endregion
        }

        private static void LevelUnloaded()
        {
            if (Campaign.SessionActive)
            {
#if DEBUG
                MelonLogger.Msg("Current level is a Campaign! Saving ammo to Save Data...");
#endif
                Campaign.Session.saveData.SaveAmmoForLevel(Campaign.lastLoadedCampaignLevel);
            }
        }
    }
}
