using BoneLib;
using CustomCampaignTools.Bonemenu;
using CustomCampaignTools.GameSupport;
using CustomCampaignTools.GameSupport.BoneLab;
using CustomCampaignTools.Patching;
using CustomCampaignTools.Timing;
using MelonLoader;
using System.Reflection;
using UnityEngine;

namespace CustomCampaignTools
{
    internal class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            GameManager.InitializeGameConfiguration();
            ArgumentHandler.HandleArguments(Environment.GetCommandLineArgs());
        }

        public override void OnLateInitializeMelon()
        {
            // Create Bonemenu
            GameManager.OnLateInitialize();

            Campaign.OnInitialize();

            Hooking.OnLevelLoaded += LevelInitialized;
            Hooking.OnLevelUnloaded += LevelUnloaded;
            Hooking.OnUIRigCreated += OnUIRigCreated;

            string resourceName = "CampaignIcon.png";
            Assembly assembly = MelonAssembly.Assembly;

            BoneLabMainMenuMangler.LoadSpriteFromEmbeddedResource(resourceName, assembly, new Vector2(0.5f, 0.5f));

            foreach (MelonMod registeredMelon in MelonMod.RegisteredMelons)
            {
                if (HelperMethods.CheckIfAssemblyLoaded("BrowsingPlus"))
                {
                    PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason();
                }
            }
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if(buildIndex == 0)
            {
                GameManager.OnBootstrapSceneLoaded();
            }
        }

        private void PatchSwipezBecauseLemonloaderKeepsFuckingFailingIfIPutThisMethodInOnLateInitializeMelonForSomeReason()
        {
            SwipezPanelPatches.ManualPatch();
        }

        internal static void LevelInitialized(LevelInfo info)
        {
            string barcode = info.barcode;

            GameManager.OnLevelLoaded(info);

            SafeActions.InvokeActionSafe(LevelLoadingPatches.OnNextSceneLoaded);
            LevelLoadingPatches.OnNextSceneLoaded = () => {};
        }

        private static void LevelUnloaded()
        {
            if (Campaign.SessionActive)
            {
                if (Campaign.lastLoadedCampaignLevel == null) return;
                Campaign.Session.saveData.SaveAmmoForLevel(Campaign.lastLoadedCampaignLevel.Barcode);
                LevelTiming.OnCampaignLevelUnloaded(Campaign.Session, Campaign.lastLoadedCampaignLevel.BarcodeString);
            }
        }

        private static void OnUIRigCreated()
        {
            GameManager.ManglePlayerMenu();
            Campaign.OnUIRigCreated();
        }
    }
}
