using BoneLib;
using CustomCampaignTools.Bonemenu;
using CustomCampaignTools.Patching;
using CustomCampaignTools.Timing;
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

            foreach (MelonMod registeredMelon in MelonMod.RegisteredMelons)
            {
                if (HelperMethods.CheckIfAssemblyLoaded("BrowsingPlus"))
                {
                    PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason();
                }
            }
        }

        private void PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason()
        {
            SwipezPanelPatches.ManualPatch();
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string barcode = info.barcode;

            MainMenuMangler.OnLevelLoaded(info);

            if (CampaignUtilities.IsCampaignLevel(barcode, out Campaign campaign, out var levelType))
            {
                LevelTiming.OnCampaignLevelLoaded(campaign, barcode);

                if (levelType != CampaignLevelType.MainLevel) return;

                if (SavepointFunctions.CurrentLevelLoadedByContinue)
                {
                    SavepointFunctions.LoadPlayerFromSave();
                }
                else
                {
                    campaign.saveData.SavePlayer(barcode, Vector3.zero);
                    if(campaign.SaveLevelInventory && campaign.saveData.LoadedInventorySaves.ContainsKey(barcode))
                    {
                        MelonCoroutines.Start(SavepointFunctions.ApplyInventoryDataAfterTime(campaign.saveData.LoadedInventorySaves[barcode]));
                    }
                }
            }
        }

        private static void LevelUnloaded()
        {
            if (Campaign.SessionActive)
            {
                Campaign.Session.saveData.SaveAmmoForLevel(Campaign.lastLoadedCampaignLevel);
                LevelTiming.OnCampaignLevelUnloaded(Campaign.Session, Campaign.lastLoadedCampaignLevel);
            }
        }

        private static void OnUIRigCreated()
        {
            PlayerMenuMangler.Initialize();
            Campaign.OnUIRigCreated();
        }
    }
}
