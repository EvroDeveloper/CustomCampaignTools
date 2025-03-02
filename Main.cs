using BoneLib;
using CustomCampaignTools.Bonemenu;
using Il2CppSLZ.Marrow.SceneStreaming;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace CustomCampaignTools
{
    internal class Main : MelonMod
    {
        public override void OnLateInitializeMelon()
        {
            // Create Bonemenu
            BoneMenuCreator.CreateBoneMenu();

            Campaign.OnInitialize();

            Hooking.OnLevelLoaded += LevelInitialized;
            Hooking.OnLevelUnloaded += LevelUnloaded;
            Hooking.OnUIRigCreated += OnUIRigCreated;

            string resourceName = "CampaignIcon.png";
            Assembly assembly = MelonAssembly.Assembly;

            MainMenuMangler.LoadSpriteFromEmbeddedResource(resourceName, assembly, new Vector2(0.5f, 0.5f));
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string barcode = info.barcode;

            MainMenuMangler.OnLevelLoaded(info);

            #region Save Data
            if (CampaignUtilities.IsCampaignLevel(barcode, out Campaign campaign, out var levelType))
            {
                if (levelType != CampaignLevelType.MainLevel) return;

                int levelIndex = campaign.GetLevelIndex(barcode, CampaignLevelType.MainLevel);
                string previousLevelBarcode = campaign.GetLevelBarcodeByIndex(levelIndex - 1, CampaignLevelType.MainLevel);

                if (SavepointFunctions.WasLastLoadByContinue)
                {
                    SavepointFunctions.LoadPlayerFromSave();
                }
                else
                {
                    SavepointFunctions.SavePlayer(barcode, Vector3.zero);
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

        private static void OnUIRigCreated()
        {
            Campaign.OnUIRigCreated();
        }
    }
}
